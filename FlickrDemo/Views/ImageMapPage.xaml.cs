using System;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;

namespace FlickrDemo.Views
{
    public sealed partial class ImageMapPage : Page
    {
        RandomAccessStreamReference mapIconStreamReference;
        bool MarkerAdded = false;

        public ImageMapPage()
        {
            this.InitializeComponent();
            map.MapServiceToken = Conts.MapKey;
        }

        
        private void map_LoadingStatusChanged(MapControl sender, object args)
        {
            if (map.LoadingStatus == MapLoadingStatus.Loading)
            {
                progressBar.Visibility = Visibility.Visible;
                progressBar.IsIndeterminate = true;
            }
            else if (map.LoadingStatus == MapLoadingStatus.Loaded)
            {
                progressBar.Visibility = Visibility.Collapsed;
                progressBar.IsIndeterminate = false;

                if (!MarkerAdded)
                {
                    mapIconStreamReference = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/pin.png"));
                    map.ZoomLevel = 17;
                    Windows.Devices.Geolocation.BasicGeoposition imageLocation = new Windows.Devices.Geolocation.BasicGeoposition();
                    imageLocation.Latitude = App.mapInfoViewModel.Latitude;
                    imageLocation.Longitude = App.mapInfoViewModel.Longitude;
                    map.Center = new Windows.Devices.Geolocation.Geopoint(imageLocation);

                    MapIcon mapIcon1 = new MapIcon();
                    mapIcon1.Location = map.Center;
                    mapIcon1.NormalizedAnchorPoint = new Point(0.5, 1.0);
                    mapIcon1.Title = App.mapInfoViewModel.Title;
                    mapIcon1.Image = mapIconStreamReference;
                    mapIcon1.ZIndex = 0;
                    map.MapElements.Add(mapIcon1);
                    MarkerAdded = true;
                }
            }
        }
    }
}
