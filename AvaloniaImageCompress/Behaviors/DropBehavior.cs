using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactions.DragAndDrop;

using AvaloniaImageCompress.ViewModels;

namespace AvaloniaImageCompress.Behaviors
{
    public class DataGridDropHandler : DropHandlerBase
    {
        public override void Enter(object? sender, DragEventArgs e, object? sourceContext, object? targetContext)
        {
            if (Validate(sender, e, sourceContext, targetContext, null) == false)
            {
                e.DragEffects = DragDropEffects.None;
              
                e.Handled = true;
            }
            else
            {
                e.DragEffects |= DragDropEffects.Copy;
                e.Handled = true;
            }
        }
        public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state) =>
              (e.Data != null && targetContext is MainWindowViewModel);
        

        public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
        {
            try
            {
                MainWindowViewModel? ViewModel = targetContext as MainWindowViewModel;

                var items = e.Data.GetFileNames();
                foreach (var item in items)
                {
                    if (item.PathIsDir())
                    {
                        ViewModel?.Images.AddImageModelFromFileInfo(item.GetFilesFromFolder());

                    }
                    else ViewModel?.Images.AddImageModelFromFileInfo(item.StringToFileInfo());
                }
                return true;
            }
            catch (Exception) { return false; }
            
            
        }
    }
}
