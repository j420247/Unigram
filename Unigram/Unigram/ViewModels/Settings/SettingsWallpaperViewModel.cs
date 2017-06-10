﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Api.Aggregator;
using Telegram.Api.Helpers;
using Telegram.Api.Services;
using Telegram.Api.Services.Cache;
using Telegram.Api.TL;
using Unigram.Common;
using Unigram.Core.Common;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Navigation;

namespace Unigram.ViewModels.Settings
{
    public class SettingsWallPaperViewModel : UnigramViewModelBase
    {
        public SettingsWallPaperViewModel(IMTProtoService protoService, ICacheService cacheService, ITelegramEventAggregator aggregator)
            : base(protoService, cacheService, aggregator)
        {
            Items = new MvxObservableCollection<TLWallPaperBase>();
            ProtoService.GetWallpapersAsync(result =>
            {
                var defa = result.FirstOrDefault(x => x.Id == 1000001);
                if (defa != null)
                {
                    result.Remove(defa);
                    result.Insert(0, defa);
                }

                Items.ReplaceWith(result);
                UpdateView();
            });
        }

        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            UpdateView();
            return base.OnNavigatedToAsync(parameter, mode, state);
        }

        private void UpdateView()
        {
            if (Items == null)
            {
                return;
            }

            var selected = ApplicationSettings.Current.SelectedBackground;
            if (selected == -1)
            {
                IsLocal = true;
                SelectedItem = null;
            }
            else
            {
                SelectedItem = Items.FirstOrDefault(x => x.Id == selected) ?? Items.FirstOrDefault(x => x.Id == 1000001) ?? new TLWallPaper { Id = 1000001 };
            }
        }

        private bool _isLocal;
        public bool IsLocal
        {
            get
            {
                return _isLocal;
            }
            set
            {
                Set(ref _isLocal, value);
            }
        }

        private TLWallPaperBase _selectedItem;
        public TLWallPaperBase SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                Set(ref _selectedItem, value);
                
                if (value != null)
                {
                    IsLocal = false;
                }
            }
        }

        public MvxObservableCollection<TLWallPaperBase> Items { get; private set; }

        public RelayCommand LocalCommand => new RelayCommand(LocalExecute);
        private async void LocalExecute()
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.AddRange(Constants.PhotoTypes);

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                var result = await FileUtils.CreateTempFileAsync("temp_wallpaper.jpg");
                await file.CopyAndReplaceAsync(result);

                IsLocal = true;
                SelectedItem = null;
            }
        }

        public RelayCommand DoneCommand => new RelayCommand(DoneExecute);
        private async void DoneExecute()
        {
            if (_selectedItem is TLWallPaper wallpaper)
            {
                if (wallpaper.Id != 1000001)
                {
                    var photoSize = wallpaper.Full as TLPhotoSize;
                    var location = photoSize.Location as TLFileLocation;
                    var fileName = string.Format("{0}_{1}_{2}.jpg", location.VolumeId, location.LocalId, location.Secret);

                    var item = await ApplicationData.Current.LocalFolder.TryGetItemAsync(FileUtils.GetTempFilePath(fileName));
                    if (item is StorageFile file)
                    {
                        var result = await FileUtils.CreateTempFileAsync("wallpaper.jpg");
                        await file.CopyAndReplaceAsync(result);
                    }
                }

                ApplicationSettings.Current.SelectedBackground = wallpaper.Id;
                ApplicationSettings.Current.SelectedColor = 0;
            }
            else if (_selectedItem is TLWallPaperSolid solid)
            {
                ApplicationSettings.Current.SelectedBackground = solid.Id;
                ApplicationSettings.Current.SelectedColor = solid.BgColor;
            }
            else if (_selectedItem == null && _isLocal)
            {
                var item = await ApplicationData.Current.LocalFolder.TryGetItemAsync(FileUtils.GetTempFilePath("temp_wallpaper.jpg"));
                if (item is StorageFile file)
                {
                    var result = await FileUtils.CreateTempFileAsync("wallpaper.jpg");
                    await file.MoveAndReplaceAsync(result);
                }

                ApplicationSettings.Current.SelectedBackground = -1;
                ApplicationSettings.Current.SelectedColor = 0;
            }

            Aggregator.Publish("Wallpaper");
            NavigationService.GoBack();
        }
    }
}