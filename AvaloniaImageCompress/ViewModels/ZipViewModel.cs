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
      
        private bool LowQualitty(string filePath, string folder, int compressValue)
        {
            try
            {
                
                int quallity = 100 - (compressValue * 10);
                T4Image.Input input = new T4Image.Input(filePath);
               
                input.File();
                if (input.StreamFile != null)
                {
                    T4Image.Output output =
                        new T4Image.Output(T4Image.Output.LevelOptimal.Storage, quallity,
                                           folder, input.FileName, input.FileExtension);

                    T4Image.Optimizer optimizer = new T4Image.Optimizer(input,output);
                    optimizer.ExportFile();
                }
            }
            catch(Exception ex)
            {
                return false;
            }
            
            return true;


        }

        public void StartProcess()
        {

            ProcessCount = Images.Count();
            Task.Run(() =>
            {
                foreach (var image in Images)
                {

                    var res = LowQualitty(image.Path, _SaveFolder, StepZipp);
                    image.CompressedStatus = res ? CompressedStatusEnum.Finished : CompressedStatusEnum.Error;
                    ProcessCount--;

                    Finished = ProcessCount == 0;

                }
            });
           
            
        }
    }
}
