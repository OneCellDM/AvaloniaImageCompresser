using AvaloniaImageCompress.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using SkiaSharp;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaImageCompress.ViewModels
{
    public class ZipViewModel:ReactiveObject
    {
        private string _SaveFolder;
       
        public delegate void CloseZipView();
        public event CloseZipView CloseZipViewEvent;
       

        [Reactive]
        public bool Finished { get; set; }
        public int ProcessCount { get; set; }
        public IEnumerable<ImageModel> Images { get; set; }
        public IReactiveCommand ExitCommand { get; set; }
        public IReactiveCommand OpenResultFolderCommand { get; set; }
        private int StepZipp { get; set; }
        public ZipViewModel(IEnumerable<ImageModel> imageModels, string folder, int step)
        {
            ExitCommand = ReactiveCommand.Create(() => CloseZipViewEvent?.Invoke());
            OpenResultFolderCommand = ReactiveCommand.Create(()=> Process.Start(folder));
            Images = imageModels;
            this._SaveFolder = folder;

            foreach(var image in imageModels)
                image.CompressedStatus = CompressedStatusEnum.Wait;
            
            this.StepZipp = step;
            StartProcess();
        }
       

        private SKEncodedImageFormat GetImageFormat(string extension)
        {

            return extension.Remove(0, 1) switch
            {
                "png" => SKEncodedImageFormat.Png,
                "jpg" or "jpeg" => SKEncodedImageFormat.Jpeg,
                "bmp" => SKEncodedImageFormat.Bmp,
                _=> SKEncodedImageFormat.Jpeg,

            };
          
        }

        private bool CompressQualitty(string filePath, string folder, int compressValue)
        {

            try
            {
                var bitmap = SKBitmap.Decode(filePath);


                SKEncodedImageFormat imageFormat = GetImageFormat(Path.GetExtension(filePath));


                var newImage = SKImage.FromBitmap(bitmap);

                var imageData = newImage.Encode(imageFormat, 100 - (compressValue * 10));

                var newFileName = Path.Combine(folder, "Compress_" + Path.GetFileName(filePath));

                var stream = new FileStream(newFileName, FileMode.Create, FileAccess.Write);

                imageData.SaveTo(stream);
                stream.Dispose();
                newImage.Dispose();
                imageData.Dispose();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public void StartProcess()
        {

            ProcessCount = Images.Count();
            Task.Run(() =>
            {
                foreach (var image in Images)
                {
                    image.CompressedStatus = CompressedStatusEnum.processed;
                    var res = CompressQualitty(image.Path, _SaveFolder, StepZipp);
                    image.CompressedStatus = res ? CompressedStatusEnum.Finished : CompressedStatusEnum.Error;
                    ProcessCount--;

                    Finished = ProcessCount == 0;

                }
            });
           
            
        }
    }
}
