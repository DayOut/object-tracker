using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using GMap.NET;
using GMap.NET.WindowsPresentation;
using Microsoft.AspNetCore.SignalR.Client;
using ObjectTrackerFront.Commands;
using ObjectTrackerFront.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ObjectTrackerFront.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private const int AliveDelayS = -10;
        private const int RemoveDelayS = -300;
        private const int CleanupTimerDelayS = 3;
        private const string LoginPath = "/api/auth/login";

        private HubConnection _connection;
        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    OnPropertyChanged();

                    (ConnectCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }
        private Timer _cleanupTimer;

        public Action<GMapMarker> OnMarkerRemove;

        public ObservableCollection<MapObject> ObjectsOnMap { get; } = new ObservableCollection<MapObject>();

        private string _log;
        public string Log
        {
            get => _log;
            set {

                _log = value;
                OnPropertyChanged();
            }
        }

        public string Host { get; set; } = "https://localhost:5000";
        public string Username { get; set; }
        public string Password { get; set; }

        public ICommand ConnectCommand { get; }

        public MainViewModel()
        {
            ConnectCommand = new RelayCommand(
                async () => await ExecuteConnect(),
                () => !IsConnected
            );
            StartCleanupTimer();
        }

        private async Task ExecuteConnect()
        {
            if (_connection != null && _connection.State != HubConnectionState.Disconnected)
            {
                await _connection.StopAsync();
                AppendLog("Disconnected");
                IsConnected = false;
                return;
            }

            var token = await GetAccessTokenAsync(Username, Password);

            if (string.IsNullOrEmpty(token))
            {
                AppendLog("Не вдалося авторизуватись.");
                return;
            }

            string finalUrl = Host + "/trackHub?key=" + token;
            _connection = new HubConnectionBuilder()
                .WithUrl(finalUrl)
                .WithAutomaticReconnect()
                .Build();

            _connection.On<IEnumerable<TrackedObject>>("ReceiveObjects", objects =>
            {

                foreach (var obj in objects)
                {
                    AddOrUpdateObject(obj.Id, obj.Latitude, obj.Longitude, obj.Heading);
                }
            });

            await _connection.StartAsync();
            AppendLog("З'єднання встановлено");
            IsConnected = true;
        }

        private async Task<string> GetAccessTokenAsync(string username, string password)
        {
            var client = new HttpClient();

            var request = new LoginRequest { Username = username, Password = password };
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            string url = Host + LoginPath;
            AppendLog($"Attempting to connect via {url}");
            try
            {
                var response = await client.PostAsync(url, content);
                if (!response.IsSuccessStatusCode)
                {
                    AppendLog($"Помилка авторизації: {response.StatusCode}");
                    return null;
                }
                var json = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);
                return doc.RootElement.GetProperty("token").GetString();
            }
            catch (Exception ex)
            {
                AppendLog($"Помилка авторизації: {ex.Message}");

            }
            return null;
        }

        private void StartCleanupTimer()
        {
            _cleanupTimer = new Timer(CleanupTimerDelayS * 1000);
            _cleanupTimer.Elapsed += (s, e) =>
            {
                var ActiveThreshold = DateTime.UtcNow.AddSeconds(AliveDelayS);
                var RemoveThreshhold = DateTime.UtcNow.AddSeconds(RemoveDelayS);
                var toRemove = ObjectsOnMap
                   .Where(o => o.LastUpdate < RemoveThreshhold)
                   .ToList();

                foreach (var obj in ObjectsOnMap)
                {    
                    if (!obj.IsOutdated && obj.LastUpdate < ActiveThreshold)
                    {
                        obj.IsOutdated = true;
                        AppendLog($"Об'єкт {obj.Id} був втрачений");
                    }
                }

                Application.Current?.Dispatcher.Invoke(() =>
                {
                    foreach (var obj in toRemove)
                    {
                        AppendLog($"Об'єкт {obj.Id} був видалений");
                        OnMarkerRemove?.Invoke(obj.Marker);
                        ObjectsOnMap.Remove(obj);
                    }
                });
                
            };
            _cleanupTimer.Start();
        }

        public void AddOrUpdateObject(string id, double lat, double lng, double angle)
        {
            var existing = ObjectsOnMap.FirstOrDefault(o => o.Id == id);
            var pos = new PointLatLng(lat, lng);

            Application.Current.Dispatcher.Invoke(() =>
            {
                if (existing != null)
                {
                    existing.Marker.Position = pos;
                    existing.LastUpdate = DateTime.UtcNow;
                }
                else
                {
                    var marker = createMapObj(id, pos, angle);
                    var rotation = new RotateTransform(angle);
                    var newObj = new MapObject
                    {
                        Id = id,
                        Marker = marker,
                        Rotation = rotation,
                        LastUpdate = DateTime.UtcNow,
                    };
                    ObjectsOnMap.Add(newObj);
                }
            });
        }
 
        private UIElement createTriangle(double angle)
        {
            var rotation = new RotateTransform(angle);
            return new Polygon
            {
                Points = new PointCollection
                    {
                        new Point(0, -10),
                        new Point(5, 5),
                        new Point(-5, 5),
                    },
                Fill = Brushes.Blue,
                Stroke = Brushes.White,
                StrokeThickness = 1,
                RenderTransform = rotation,
                RenderTransformOrigin = new Point(0.5, 0.5)
            };
        }

        private GMapMarker createMapObj(string name, PointLatLng pos, double angle)
        {
            return new GMapMarker(pos)
            {
                Shape = new StackPanel
                {
                    RenderTransformOrigin = new Point(0.5, 0.5),
                    Orientation = Orientation.Vertical,
                    Children =
                    {
                        createTriangle(angle),
                        new TextBlock
                        {
                            Text = name,
                            Foreground = Brushes.Black,
                            FontSize = 10,
                            HorizontalAlignment = HorizontalAlignment.Center
                        }
                    }
                },
                Offset = new Point(-5, -15)
            };
        }

        public void AppendLog(string message)
        {
            Log += $"[{DateTime.Now:T}] {message}\n";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
