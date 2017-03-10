using FlickrDemo.ViewModel;
using FlickrNet;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation.Metadata;
using Windows.UI.Notifications;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;


namespace FlickrDemo.Views
{
    public sealed partial class ImageGridPage : Page
    {
        public ImageGridPage()
        {
            this.InitializeComponent();
            try
            {
                Loaded += MainPage_Loaded;
                Window.Current.SizeChanged += (sender, args) =>
                {
                    AdjustColumnsOrientation();
                };
            }
            catch (Exception ex)
            {
                ShowToastNotification("Loading Failed", "No Internet Connection");
            }
        }

        void AdjustColumnsOrientation()
        {
            ApplicationView currentView = ApplicationView.GetForCurrentView();

            if (ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1))
            {
                // do code for mobile
                if (currentView.Orientation == ApplicationViewOrientation.Landscape)
                {
                    // when args.Size.Width > args.Size.Height
                    ((ItemsWrapGrid)((ItemsControl)GridViewMain).ItemsPanelRoot).MaximumRowsOrColumns = 4;
                }
                else if (currentView.Orientation == ApplicationViewOrientation.Portrait)
                {
                    // when args.Size.Width < args.Size.Height
                    ((ItemsWrapGrid)((ItemsControl)GridViewMain).ItemsPanelRoot).MaximumRowsOrColumns = 2;
                }
            }
            else
            {
                // do code for other
                if (currentView.Orientation == ApplicationViewOrientation.Landscape)
                {
                    // when args.Size.Width > args.Size.Height
                    ((ItemsWrapGrid)((ItemsControl)GridViewMain).ItemsPanelRoot).MaximumRowsOrColumns = 16;
                }
                else if (currentView.Orientation == ApplicationViewOrientation.Portrait)
                {
                    // when args.Size.Width < args.Size.Height
                    ((ItemsWrapGrid)((ItemsControl)GridViewMain).ItemsPanelRoot).MaximumRowsOrColumns = 8;
                }
            }
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                AdjustColumnsOrientation();
                App.flickrImagesViewModel = new FlickrImagesViewModel(Window.Current.Dispatcher);
               

                App.flickrImagesViewModel.LoadingFailedPropertyChanged += FlickrImagesViewModel_LoadingFailedPropertyChanged;
                App.flickrImagesViewModel.IsLoadingPropertyChanged += FlickrImagesViewModel_IsLoadingPropertyChanged;
                GridViewMain.ItemsSource = App.flickrImagesViewModel;
            }
            catch (Exception ex)
            {
                ShowToastNotification("Loading Failed", "No Internet Connection");
            }
        }

        private void FlickrImagesViewModel_LoadingFailedPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            /* Show PopUp for internet connection failure*/
            if (App.flickrImagesViewModel.LoadingFailed)
            {
                ShowToastNotification("Loading Failed", "No Internet Connection");
            }
        }

        private void ShowToastNotification(string title, string stringContent)
        {
            ToastNotifier ToastNotifier = ToastNotificationManager.CreateToastNotifier();
            Windows.Data.Xml.Dom.XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            Windows.Data.Xml.Dom.XmlNodeList toastNodeList = toastXml.GetElementsByTagName("text");
            toastNodeList.Item(0).AppendChild(toastXml.CreateTextNode(title));
            toastNodeList.Item(1).AppendChild(toastXml.CreateTextNode(stringContent));
            Windows.Data.Xml.Dom.IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            ToastNotification toast = new ToastNotification(toastXml);
            toast.ExpirationTime = DateTime.Now.AddSeconds(4);
            ToastNotifier.Show(toast);
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

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            searchBox.Text = string.Empty;
            if (searchBox.Visibility == Visibility.Collapsed)
            {
                searchBox.Visibility = Visibility.Visible;
                searchBox.Focus(FocusState.Keyboard);
            }
            else
            {
                searchBox.Visibility = Visibility.Collapsed;
            }
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(searchBox.Text))
                {
                    GridViewMain.ItemsSource = App.flickrImagesViewModel;
                }
                else
                {
                    List<Photo> FilteredList = new List<Photo>();
                    FilteredList = App.flickrImagesViewModel.Where(a => a.Title.ToLower().Contains(searchBox.Text.ToLower()) || (!string.IsNullOrEmpty(a.Description) && a.Description.ToLower().Contains(searchBox.Text.ToLower()))).ToList();
                    GridViewMain.ItemsSource = FilteredList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Photo photo = ((sender as Image).DataContext as Photo);
            App.flickrImagesViewModel.SelectedPhotoIndex = photo.PhotoId;
            Frame.Navigate(typeof(ImagePivotPage));
        }
    }
}
