using System;
using GMap.NET.WindowsPresentation;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows;

namespace ObjectTrackerFront.Models
{
    public class MapObject
    {
        public string Id { get; set; }
        public GMapMarker Marker { get; set; }
        public RotateTransform Rotation { get; set; }
        public DateTime LastUpdate { get; set; }

        public bool IsOutdated
        {
            get => _isOutdated;
            set
            {
                _isOutdated = value;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var triangle = ((Marker.Shape as StackPanel)?.Children[0]) as Polygon;
                    if (triangle != null)
                        triangle.Fill = value ? Brushes.Gray : Brushes.Blue;
                });
            }
        }

        private bool _isOutdated = false;
    }
}
