using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Lanzous_FS
{
    public partial class MainWindow
    {

        private void DownloadBtn_Click(object sender, RoutedEventArgs e)
        {
            //Lanzou.download_file("i5znnij", "a:/abc.mp3");
            Lanzou.download_file("i62p51e", "a:/mmc.txt");
            //var pua = "Mozilla/5.0 (Linux; Android 8.0; Pixel 2 Build/OPD3.170816.012) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.75 Mobile Safari/537.36";
            //var req = getReq("https://www.lanzous.com/tp/i5znnij");
            //req.UserAgent = pua;
            //var ret = reqOut(req);
            //Clipboard.SetDataObject(ret);
        }

        private void UploadBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_lanz == null)
            {
                MessageBox.Show("not login!");
                return;
            }
            _lanz.upload_file("a:/tts.ini.txt", -1);
        }

        private void MkdirBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_lanz == null)
            {
                MessageBox.Show("not login!");
                return;
            }
            _lanz.mkdir("run", -1);
        }

        private void RenameBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_lanz == null)
            {
                MessageBox.Show("not login!");
                return;
            }
            var ret = _lanz.changeDirInfo(966654, "4444", "change once by code");
            ///var ret = _lanz.changeDirInfo(947801,"__SYS__", "{sys:9946,\"info\":\"<t>\"}");
            ret.Wait();
        }

        private void DBBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Data.SQLite.SQLiteConnection con = new System.Data.SQLite.SQLiteConnection("data source=a:\\sqlite.db");
            con.Open();
            var createCommand = con.CreateCommand();
            createCommand.CommandText =
            @"
                CREATE TABLE Tis (
                    name TEXT
                )
            ";
            createCommand.ExecuteNonQuery();
            con.Close();
        }

        private void InsertBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Data.SQLite.SQLiteConnection con = new System.Data.SQLite.SQLiteConnection("data source=a:\\sqlite.db");
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText =
            @"
                insert into Tis values(
                    ""tis_mike""
                )
            ";
            cmd.ExecuteNonQuery();
            con.Close();
        }

        private void SelectBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Data.SQLite.SQLiteConnection con = new System.Data.SQLite.SQLiteConnection("data source=a:\\sqlite.db");
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText =
            @"
                select * from Tis
            ";
            var dr = cmd.ExecuteReader();
            //dr.FieldCount;
            int i = 0;

            for (; dr.Read(); i++)
            {

            }
            MessageBox.Show(i.ToString());

            con.Close();
        }
    }
}
