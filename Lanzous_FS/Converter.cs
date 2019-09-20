using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Lanzous_FS
{
    using IconType = Lanzous_FS.VirtualFileSystem.IconType;
    
    public class IconConverter : IValueConverter
    {
        static BitmapImage fileImage = new BitmapImage(new Uri("/res/file.ico", UriKind.RelativeOrAbsolute));
        static BitmapImage musicImage = new BitmapImage(new Uri("/res/music.ico", UriKind.RelativeOrAbsolute));
        static BitmapImage videoImage = new BitmapImage(new Uri("/res/video.ico", UriKind.RelativeOrAbsolute));
        static BitmapImage textImage = new BitmapImage(new Uri("/res/textfile.ico", UriKind.RelativeOrAbsolute));
        static BitmapImage folderImage = new BitmapImage(new Uri("/res/folder.ico", UriKind.RelativeOrAbsolute));
        static BitmapImage appImage = new BitmapImage(new Uri("/res/app.ico", UriKind.RelativeOrAbsolute));
        static BitmapImage pictureImage = new BitmapImage(new Uri("/res/pic.ico", UriKind.RelativeOrAbsolute));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IconType ico = (IconType)value;
            switch (ico)
            {
                case IconType.Application:
                    return appImage;
                case IconType.Dir:
                    return folderImage;
                case IconType.Music:
                    return musicImage;
                case IconType.Text:
                    return textImage;
                case IconType.Video:
                    return videoImage;
                case IconType.Picture:
                    return pictureImage;
                default:
                    return fileImage;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
