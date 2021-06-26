using Windows.Storage.FileProperties;

namespace EMS.Data.Data
{
    public class AttachedFile
    {
        public StorageItemThumbnail Thumbnail { get; set; }
        public string FileName { get; set; }
    }
}