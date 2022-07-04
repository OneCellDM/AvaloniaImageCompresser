using AvaloniaImageCompress.Models;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaImageCompress
{
    public static class IOHelper
    {
        public static List<string> Exstentions = new List<string> { "png", "jpg", "jpeg", "bmp" };

        public static bool PathIsDir(this string path)=>
            File.GetAttributes(path).HasFlag(FileAttributes.Directory);
         
        public static bool CheckValideExtension(this string path)=>
            Exstentions.Contains(Path.GetExtension(path).Remove(0, 1));
          
        public static  IEnumerable<FileInfo>? GetFilesFromFolder (this string path)
        {
           
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                var files = directoryInfo.GetFiles()
                                         .Where(x => Exstentions.Contains(x.Extension.Remove(0, 1)));
                return files;
          
        }
        public static IEnumerable<FileInfo> StringToFileInfo(this string[] paths) =>
            (paths)?.Select(x=>x?.StringToFileInfo())?.Where(x => x != null);
        
        
        public static FileInfo? StringToFileInfo(this string path)=>
            CheckValideExtension(path)?new FileInfo(path): null;

        public static void AddImageModelFromFileInfo(this ObservableCollection<ImageModel> collection, FileInfo file)
        {
            if(file is not null)
                collection.Add(new ImageModel()
                {
                    Path = file.FullName,
                    Title = file.Name,
                    FileSize = file.Length,

                });
        }

        public static void AddImageModelFromFileInfo(this ObservableCollection<ImageModel> collection, IEnumerable<FileInfo> files)
        {
            foreach(var file in files)
                collection.AddImageModelFromFileInfo(file);
            
        }
    }
}
