using FlickrNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml.Data;

namespace FlickrDemo.ViewModel
{
    public class FlickrImagesViewModel : ObservableCollection<Photo>, ISupportIncrementalLoading, INotifyPropertyChanged
    {
        public int LastItem = 1;
        public int CurrentPage = 1;
        public string SelectedPhotoIndex { get; set; }
        private bool _isLoading;
        CoreDispatcher coreDispatcher;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                RaisePropertyChanged("IsLoading");
            }
        }


        private bool _loadingFailed;
        public bool LoadingFailed
        {
            get { return _loadingFailed; }
            set
            {
                _loadingFailed = value;
                RaisePropertyChanged("LoadingFailed");
            }
        }


        public bool HasMoreItems
        {
            get
            {
                if (LastItem == 20)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public event PropertyChangedEventHandler IsLoadingPropertyChanged;
        public event PropertyChangedEventHandler LoadingFailedPropertyChanged;
        protected void RaisePropertyChanged(string name)
        {
            if (IsLoadingPropertyChanged != null)
            {
                IsLoadingPropertyChanged(this, new PropertyChangedEventArgs(name));
            }

            if (LoadingFailedPropertyChanged != null)
            {
                LoadingFailedPropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public FlickrImagesViewModel(CoreDispatcher dispatcher)
        {
            coreDispatcher = dispatcher;
        }

        public async Task<LoadMoreItemsResult> PopulateMoreData(uint count = 20)
        {
            try
            {
                var f = new Flickr(Conts.FlickrKey);
                await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    IsLoading = true;
                });

                List<Photo> items = new List<Photo>();

                //// For the sake of the demo , in order to get geo info i have to include it in the request, 
                //// so all the images would be with geo info or all the images won't, so i made two requests
                PhotoCollection photosWithNoGeoTag = await f.PhotosSearchAsync(
                                new PhotoSearchOptions
                                {
                                    Tags = "colorful",
                                    PerPage = 10,
                                    Page = CurrentPage,
                                    SortOrder = PhotoSearchSortOrder.InterestingnessDescending
                                }).AsAsyncOperation();

                PhotoCollection photosWithGeoTag = await f.PhotosSearchAsync(
                                new PhotoSearchOptions
                                {
                                    Tags = "colorful",
                                    PerPage = 10,
                                    Page = CurrentPage,
                                    SortOrder = PhotoSearchSortOrder.InterestingnessDescending,
                                    HasGeo = true,
                                    Extras = PhotoSearchExtras.Geo
                                }).AsAsyncOperation();


                foreach (var item in photosWithNoGeoTag)
                {
                    items.Add(item);
                }

                foreach (var item in photosWithGeoTag)
                {
                    items.Add(item);
                }


                await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    foreach (var item in items)
                    {
                        this.Add(item);
                    }
                    CurrentPage++;

                    IsLoading = false;
                });

                return new LoadMoreItemsResult() { Count = count };
            }
            catch (Exception ex)
            {
                //LoadingFailed = true;
                throw ex;
            }
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            try
            {
                return Task.Run<LoadMoreItemsResult>(async () =>
                {
                    return await PopulateMoreData(count);
                }).AsAsyncOperation();
            }
            catch (Exception ex)
            {
                //LoadingFailed = true;
                throw ex;
            }
        }
    }


}
