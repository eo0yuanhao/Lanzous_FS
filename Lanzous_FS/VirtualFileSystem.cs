using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lanzous_FS
{

    namespace VirtualFileSystem {
        public enum IconType
        {
            Dir,Music,Video,Text,Application,Picture ,Unknow
        }
        public class FaceDir
        {
            public string Name { get; set; }
            public string Size { get => null; }
            public string UploadTime { get => null; }
            public int id;
            public IconType IconType
            {
                get => IconType.Dir;
            }
        }
        public class FaceDir_Ext: FaceDir
        {
            public bool isRetrievedSubDir=false;
        }
        public class File
        {
            public string Name { get; set; }
            public long Size { get; set; }
            public int id;
            public string UploadTime { get; set; }
            public IconType IconType
            {
                get {
                    string ext = Name.Substring(Name.LastIndexOf('.') + 1);
                    switch (ext.ToLower())
                    {
                        case "mp3": return IconType.Music;
                        case "exe": return IconType.Application;
                        case "mp4": return IconType.Video;
                        case "txt": return IconType.Text;
                        default:
                            return IconType.Unknow;
                    }
                }
            }
            public Dictionary<int, int> partFile;
        }
        public class Directory
        {
            public string name;
            public int id;
            protected List<File> _subFiles;
            protected List<Directory> _subDirs;
            public List<File> SubFiles
            {
                get => _subFiles;
                set => _subFiles = value;
            }
            public List<Directory> SubDirs
            {
                get => _subDirs;
                set => _subDirs = value;
            }
        }
        public class Directory2: INotifyPropertyChanged
        {
            public string Name { get; set; }
            public int id;
            public bool AccessedSubDir = false;
            protected ObservableCollection<Directory2> _subDirs;
            public ObservableCollection<Directory2> SubDirs
            {
                get => _subDirs;
                set { _subDirs = value; OnPropertyChanged("SubDirs"); } 
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(string propName)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
            public void OnPropertyChanged(PropertyChangedEventArgs e)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, e);
            }
        }
        public class NetFileInfo
        {
            public int size;
            public string name;
            public DateTime uploadTime;
        }
        public class FileManager
        {
            protected readonly Directory _root = new Directory();
            public Directory Root
            {
                get => _root;
            }
        }
    }
}
