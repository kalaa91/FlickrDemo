using FlickrDemo.ViewModel;
using FlickrNet;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace FlickrDemo.Views
{
    public sealed partial class ImagePivotPage : Page
    {
        Photo currentPhoto;
        public ImagePivotPage()
        {
            this.InitializeComponent();
            Loaded += ImagePivotPage_Loaded;
        }

        private void ImagePivotPage_Loaded(object sender, RoutedEventArgs e)
        {
            App.flickrImagesViewModel.IsLoadingPropertyChanged += FlickrImagesViewModel_IsLoadingPropertyChanged;
            PivotControl.DataContext = App.flickrImagesViewModel;
            PivotControl.SelectedItem = App.flickrImagesViewModel.Where(a => a.PhotoId == App.flickrImagesViewModel.SelectedPhotoIndex).FirstOrDefault();
        }

        private void FlickrImagesViewModel_IsLoadingPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (App.flickrImagesViewModel.IsLoading)
            {
                progressBar.Visibility = Visibility.Visible;
                progressBar.IsIndeterminate = true;
            }
            else
            {
                progressBar.Visibility = Visibility.Collapsed;
                progressBar.IsIndeterminate = false;
            }
        }

        private async void PivotControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PivotControl.SelectedItem != null)
            {
                currentPhoto = (PivotControl.SelectedItem as Photo);

                if (currentPhoto.Latitude != 0 && currentPhoto.Longitude != 0)
                {
                    commandBar.Visibility = Visibility.Visible;
                }
                else
                {
                    commandBar.Visibility = Visibility.Collapsed;
                }

                if (PivotControl.DataContext != null && ((double)(PivotControl.SelectedIndex + 1) / (double)(PivotControl.DataContext as FlickrImagesViewModel).Count) >= 0.75)
                {
                    App.flickrImagesViewModel.CurrentPage++;
                    await App.flickrImagesViewModel.PopulateMoreData();
                }
            }
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            App.mapInfoViewModel.Latitude = currentPhoto.Latitude;
            App.mapInfoViewModel.Longitude = currentPhoto.Longitude;
            App.mapInfoViewModel.Title = currentPhoto.Title;
            Frame.Navigate(typeof(ImageMapPage));

        }

    }
}
