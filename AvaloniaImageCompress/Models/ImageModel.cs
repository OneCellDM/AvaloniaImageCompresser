using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace AvaloniaImageCompress.Models
{
    public class ImageModel:ReactiveObject
    {
        public string Title { get; set; }
        public string Path { get; set; }
        public long FileSize { get; set; }
        [Reactive]
       
        public CompressedStatusEnum CompressedStatus { get; set; }


    }

}
