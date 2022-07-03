using AvaloniaImageCompress.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

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
        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(2, 3);
        public delegate void CloseZipView();
        public event CloseZipView CloseZipViewEvent;
       

        [Reactive]
        public bool Finished { get; set; }
        public int ProcessCount { get; set; }
        public IEnumerable<ImageModel> Images { get; set; }
        public IReactiveCommand ExitCommand { get; set; }
        private int StepZipp { get; set; }
        public ZipViewModel(IEnumerable<ImageModel> imageModels, string folder, int step)
        {
            ExitCommand = ReactiveCommand.Create(() => CloseZipViewEvent?.Invoke());
            Images = imageModels;
            this._SaveFolder = folder;

            foreach(var image in imageModels)
                image.CompressedStatus = CompressedStatusEnum.Wait;
            
            this.StepZipp = step;
            StartProcess();
        }
        public ImageCodecInfo GetimageCodecInfo(string FileExstention)
        {
            switch (FileExstention.ToLower())
            {
                case "jpg": return ImageCodecInfo.GetImageEncoders()[1];
                case "jpeg": return ImageCodecInfo.GetImageEncoders()[1];
                case "png": return ImageCodecInfo.GetImageEncoders()[4];
            }
            return null;
        }
        private bool LowQualitty(string filePath, string folder, long compressValue)
        {
            using (Bitmap bmp = new Bitmap(filePath))
            {
                try
                {
                    ImageCodecInfo Codec = GetimageCodecInfo(filePath.Split('.')[filePath.Split('.').Length - 1]);
                    if (Codec != null)
                    {
                        var v = 100L - (compressValue * 10);
                        bmp.Save(
                            folder + "\\compressed_" + filePath.Split('\\')[filePath.Split('\\').Length - 1],
                            Codec,
                            new EncoderParameters()
                            {
                                Param = new EncoderParameter[] {
                                new EncoderParameter(System.Drawing.Imaging.Encoder.Quality,v),

                                }

                            });
                        return true;
                    }
                    else return false;
                }
                catch(Exception) { return false; }

            }
           

        }

        public void StartProcess()
        {

            ProcessCount = Images.Count();
                foreach (var image in Images)
                {
                    Task.Run(() =>
                    {
                        semaphoreSlim.Wait();

                        var res = LowQualitty(image.Path, _SaveFolder, StepZipp );
                        image.CompressedStatus = res ? CompressedStatusEnum.Finished : CompressedStatusEnum.Error;
                        ProcessCount--;
                        
                        Finished = ProcessCount == 0;
                        
                           
                        
                        semaphoreSlim.Release();
                    });
                   
                }
           
            
        }
    }
}
