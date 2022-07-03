using Avalonia.Controls;

using AvaloniaImageCompress.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AvaloniaImageCompress.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private  List<string> Exstentions = new List<string>{ "png", "jpg", "jpeg", "bmp" };
        private List<int> Level { get;  } = new List<int>()
        {
            1,2,3,4,5,6,7,8,9
        };
        [Reactive]
        public int SelectedLevel { get; set; }
        [Reactive]
        public ObservableCollection<ImageModel> Images { get; set; }

        [Reactive]
        public  ZipViewModel ZipViewModel { get; set; }
        [Reactive]
        public bool ZipViewModelVisible { get; set; } = false;

        public static Window? WindowInstance { get; set; }

        public IReactiveCommand? AddFile { get; set; }
        public IReactiveCommand? AddFolder { get; set; }
        public IReactiveCommand? ClearList { get; set; }

        public IReactiveCommand? Compress { get; set; }

        public MainWindowViewModel()
        {
            Images = new ObservableCollection<ImageModel>();

            AddFile = ReactiveCommand.CreateFromTask(AddFileAsync);
            AddFolder = ReactiveCommand.CreateFromTask(AddFolderAsync);

            Compress = ReactiveCommand.Create(async () =>
            {
                if (Images.Count > 0&&SelectedLevel!=null)
                {
                    if (WindowInstance != null)
                    {
                      
                        OpenFolderDialog openFolderDialog = new OpenFolderDialog();
                        openFolderDialog.Title = "����� ����������";
                        string path = await openFolderDialog.ShowAsync(WindowInstance);
                        if (path != null)
                        {
                            ZipViewModelVisible = true;
                            ZipViewModel = new ZipViewModel(Images,path,  SelectedLevel);
                            ZipViewModel.CloseZipViewEvent += ZipViewModel_CloseZipViewEvent;
                        }
                        
                    }
                }
            });

            ClearList = ReactiveCommand.Create(() => Images?.Clear());
        }

        private void ZipViewModel_CloseZipViewEvent()
        {
            ZipViewModel.CloseZipViewEvent -= ZipViewModel_CloseZipViewEvent;
            ZipViewModelVisible = false;
            ZipViewModel = null;

        }

        public async Task AddFileAsync()
        {
            if (WindowInstance != null)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    Filters = new System.Collections.Generic.List<FileDialogFilter>()
                    {
                        new FileDialogFilter()
                        {
                            Name = "images",
                            Extensions =Exstentions
                        }
                    }
                };

                var files = (await openFileDialog.ShowAsync(WindowInstance))?
                            .Select(x => new FileInfo(x));

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        AddImageModelFromFile(file);
                    }
                }
            }

        }
        public async Task AddFolderAsync()
        {
            if (WindowInstance != null)
            {
                OpenFolderDialog openFolderDialog = new OpenFolderDialog();
                var result = await openFolderDialog.ShowAsync(WindowInstance);
                if (result != null)
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(result);
                    var files = directoryInfo.GetFiles()
                                             .Where( x => Exstentions.Contains( x.Extension.Remove(0,1) ) );

                    foreach(var file in files)
                    {
                        AddImageModelFromFile(file);
                    }
                }
            }

        }
      
        private void AddImageModelFromFile(FileInfo file)
        {
            Images.Add(new ImageModel()
            {
                Path = file.FullName,
                Title = file.Name,
                FileSize = file.Length,
                
            });
        } 
    }
}
