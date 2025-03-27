using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using Microsoft.AspNetCore.SignalR.Client;
using ObjectTrackerFront.Models;
using ObjectTrackerFront.ViewModels;

namespace ObjectTrackerFront
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private const int AliveDelayS = -10;
        //private const int CleanupTimerDelayS = 3;
        //private const string DefaultURL = ""; 

        //private HubConnection _connection = null;
        //private System.Timers.Timer _cleanupTimer;
        //private Dictionary<string, MapObject> _objectsOnMap = new Dictionary<string, MapObject>();


        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            InitUpdaters();
        }

        private void InitUpdaters()
        {
            if (DataContext is MainViewModel vm)
            {
                vm.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(vm.Log))
                    {
                        Dispatcher.Invoke(() => LogBox.ScrollToEnd());
                    }
                };
            }
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.Password = PasswordBox.Password;
                if (vm.ConnectCommand.CanExecute(null))
                {
                    vm.ConnectCommand.Execute(null);
                }
            }
        }

        private void MainMap_Loaded(object sender, RoutedEventArgs e)
        {

            MainMap.MapProvider = GMapProviders.GoogleMap;
            MainMap.Position = new PointLatLng(50.4501, 30.5234);
            MainMap.MinZoom = 2;
            MainMap.MaxZoom = 18;
            MainMap.Zoom = 12;
            MainMap.ShowCenter = false;
            MainMap.MouseWheelZoomType = MouseWheelZoomType.ViewCenter;

            if (DataContext is MainViewModel vm)
            {
                vm.OnMarkerRemove = marker =>
                {
                    MainMap.Markers.Remove(marker);
                };

                vm.ObjectsOnMap.CollectionChanged += (s, args) =>
                {
                    foreach (var item in args.NewItems?.OfType<MapObject>() ?? Enumerable.Empty<MapObject>())
                    {
                        Dispatcher.Invoke(() =>
                        {
                            MainMap.Markers.Add(item.Marker);
                        });
                    }
                };
            }
        }
    }
}
