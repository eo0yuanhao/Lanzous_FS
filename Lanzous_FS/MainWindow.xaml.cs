using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
//using System.Windows.Shapes;
using System.Threading;


using System.Net.Http;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace Lanzous_FS
{
    using VFile = VirtualFileSystem.File;
    using Kis = System.Collections.Generic.Dictionary<int, VirtualFileSystem.NetFileInfo>;

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>


    public partial class MainWindow : Window
    {
        Lanzou _lanz = null;
        string username ;
        string password ;
        string db_tempPath;  //eg:"A:/vfs/"
        string fileDownloadPath;
        int sys_dir_id = 947801;
        Kis files;
        Kis folders;
        VirtualFileSystem.Directory facedir = new VirtualFileSystem.Directory();
        public event EventHandler FaceDirUpdated;
        Dictionary<int, VirtualFileSystem.Directory2> treeViewDict = new Dictionary<int, VirtualFileSystem.Directory2>();
        VirtualFileSystem.Directory2 dirTreeViewRoot = new VirtualFileSystem.Directory2() { id = -1,Name="悟空" };

        System.Data.SQLite.SQLiteConnection _sqlcon;
        ClickCounter _lvClickCounter;
        public MainWindow()
        {
            InitializeComponent();

            //_sqlcon = new System.Data.SQLite.SQLiteConnection("data source=a:/__vfs.sqlite.db");
            //_sqlcon.Open();
            //CreateFaceFiles(-1, "/");

            listView.Items.Clear();
            //_lvClickCounter = new ClickCounter(() => Mouse.GetPosition((IInputElement)listView));
            //_lvClickCounter.Elapsed += ClickCounterElapsed;
            //listView.MouseDown += (s, e) => { _lvClickCounter.AddMouseDown(); };
            //listView.MouseUp += (s, e) => { _lvClickCounter.AddMouseUp(); };
            FaceDirUpdated += FaceDirUpdated_do;
            loadApplicationConfig();


        }
        public void ListViewItem_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var obj = listView.SelectedItem;
            if (obj == null)
                return;
            var folder = obj as VirtualFileSystem.FaceDir;
            if (folder != null)
            {
                UpdateCurrentDir(folder.id, folder.Name);
            }
            var file = obj as VirtualFileSystem.File;
            if (file != null)
            {
                ProcessFaceFile(file);
            }
        }


        public void ClickCounterElapsed(object sender, EventArgs e)
        {
            var cc = _lvClickCounter;
            if (cc.UpCount == cc.DownCount && cc.UpCount == 2)
            {
                var obj = listView.SelectedItem;
                if (obj == null)
                    return;
                var folder = obj as VirtualFileSystem.FaceDir;
                if (folder != null)
                {
                    UpdateCurrentDir(folder.id, folder.Name);
                }
                var file = obj as VirtualFileSystem.File;
                if (file != null)
                {
                    ProcessFaceFile(file);
                }

            }
        }

        private void ProcessFaceFile(VFile file)
        {
            var DirectAccessSet = new HashSet<string>() { "jpg", "txt", "mp3", "mp4" };

            string ext = Path.GetExtension(file.Name).Substring(1).ToLower();
            if (DirectAccessSet.Contains(ext))
            {
                if (file.Size < 10 * 1024 * 1024)
                {
                    var sid = _lanz.get_shareId(file.id);
                    var stm = Lanzou.get_fileStream(sid);
                    OpenStreamByProperApplication(stm, file.Name, ext);
                }
                else downloadFaceFile(file);
            }
            else
                downloadFaceFile(file);
        }
        public void OpenStreamByProperApplication(Stream stm, string filename, string ext)
        {
            Action<Stream> saveStream = (Stream s) => { using (var fs = File.Create(fileDownloadPath + filename)) s.CopyTo(fs); };
            if (ext == "txt")
            {
                saveStream(stm);
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "Notepad.exe";
                psi.UseShellExecute = false;
                //psi.CreateNoWindow = true;
                psi.Arguments = "\"" + fileDownloadPath + filename + "\"";
                Process.Start(psi);
            }
        }
        public void downloadFaceFile(VFile file)
        {
            if (file.partFile == null)
            {
                var shareId = _lanz.get_shareId(file.id);
                Lanzou.download_file(shareId, fileDownloadPath + file.Name);
            }
            else
            {
                var fs = File.Create(fileDownloadPath + file.Name);

                foreach (var x in file.partFile)
                {
                    var sid = _lanz.get_shareId(x.Value);
                    var stm = Lanzou.get_fileStream(sid);
                    stm.CopyTo(fs);
                }
                fs.Close();
            }
        }
        public void UpdateCurrentDir(int dirId, string name)
        {
            folders = _lanz.get_subFolders(dirId);
            files = _lanz.get_subFiles(dirId);
            UpdateTreeViewDirs(dirId);
            CreateFaceFiles(dirId, name);
            if (FaceDirUpdated != null)
                FaceDirUpdated(this, null);
        }

        private void UpdateTreeViewDirs(int dirId)
        {
            if (!treeViewDict.ContainsKey(dirId) || treeViewDict[dirId].AccessedSubDir)
                return;
            var item = treeViewDict[dirId];
            var dirs =new ObservableCollection<VirtualFileSystem.Directory2>();
            
            foreach( var d in folders)
            {
                var nd = new VirtualFileSystem.Directory2() { id = d.Key, Name = d.Value.name };
                dirs.Add(nd);
                treeViewDict.Add(d.Key, nd);
            }
            item.AccessedSubDir = true;
            item.SubDirs = dirs;
            //if( listView.ItemContainerGenerator.Status != System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            //    listView.UpdateLayout();
            //TreeViewItem container = listView.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
            //container.UpdateLayout();
            //container.
            
        }

        public void FaceDirUpdated_do(object sender, EventArgs evt)
        {
            var faces = new List<object>();
            foreach (var f in folders)
            {
                var vf = new VirtualFileSystem.FaceDir();
                vf.Name = f.Value.name;
                vf.id = f.Key;
                faces.Add(vf);
            }
            faces.AddRange(facedir.SubFiles);
            listView.ItemsSource = faces;
        }
        public string getHost(string url)
        {
            //char[] domains = new string[]{ ".com", ".net" };
            var ind = url.IndexOf(".com");
            if (ind == -1)
                return null;
            return url.Substring(0, ind + 4);
        }

        public void storeCookieContainer()
        {
            if(_lanz != null)
            {
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                var fs = File.Create(db_tempPath + "lanzou.cookie");
                formatter.Serialize(fs, _lanz.cookieBag);
                fs.Close();
            }
        }
        public bool restoreCookieContainer()
        {
            if(_lanz != null)
            {
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                try
                {
                    using (var fs = File.OpenRead(db_tempPath + "lanzou.cookie"))
                    {
                        _lanz.restoreCookies((CookieContainer)formatter.Deserialize(fs));
                        //_lanz.cookieBag = (CookieContainer)formatter.Deserialize(fs);
                        /// test  cookie expired or not
                        //var t = test_lanzou_login();
                        //t.Wait();
                        var t = _lanz.testLogin();
                        return t;
             
                    }
                } catch (Exception ex)
                {
                    return false;
                }
    
            }
            return true;
        }

        public async Task<bool> test_lanzou_login()
        {
            var des__ = await _lanz.get_dirDescription(-1);
            // des__.Wait();
            string des = des__;//.Result;
            return des != null;
        }

        public void loadApplicationConfig()
        {
            Newtonsoft.Json.Linq.JObject appConfig_json;
            using (var configFile = File.OpenText("./config.json"))
                appConfig_json = Newtonsoft.Json.Linq.JObject.Parse(configFile.ReadToEnd());
            username = appConfig_json["username"].ToString();
            password = appConfig_json["password"].ToString();
            var tempPath = appConfig_json["tempPath"].ToString();
            fileDownloadPath = appConfig_json["downloadPath"].ToString();
            sys_dir_id = appConfig_json["SYS_DIR_ID"].ToObject<int>(); 
            if (tempPath.Last() != '\\' || tempPath.Last() != '/')
            {
                tempPath.Append('/');
            }
            db_tempPath = tempPath;
        }
        private async void LoginBtn_Click(object sender, RoutedEventArgs e)
        {            
            _lanz = new Lanzou();
            if (!restoreCookieContainer())
            {
                bool b =_lanz.login(username, password);
                if (b)
                    storeCookieContainer();
            }
                
            if (!await vfs_db_download())
            {
                MessageBox.Show("vfs db 下载失败");
                return;
            }
          
            var its = new ObservableCollection<VirtualFileSystem.Directory2>() { dirTreeViewRoot };
            dirTreeView.ItemsSource = its;
            treeViewDict.Clear();
            treeViewDict.Add(-1, dirTreeViewRoot);
            UpdateCurrentDir(-1, null);
        }
        public void CreateFaceFiles(int dirId,string dirname)
        {
            if (facedir.SubFiles == null)
                facedir.SubFiles = new List<VFile>();
            facedir.id = dirId;
            facedir.name = dirname;
            facedir.SubFiles.Clear();

            var cmd =_sqlcon.CreateCommand();
            cmd.CommandText = "select NetFileId,PartNo,tn.FaceId as FaceId,Size,Name from [NetFileMappedTable] as tn join [FaceFileTable] as tf on tn.FaceId= tf.FaceId where dirId=@dirId order by FaceId,PartNo";
            cmd.Parameters.AddWithValue("@dirId", dirId);
            var rd = cmd.ExecuteReader();
         
            var mappedNetFiles = new HashSet<int>();
            VFile lastfile=null;
            while (rd.Read())
            {
                //var fff = rd["NetFileId"];
                //fff.GetType().
                var netfileid = (int)rd["NetFileId"];
                var part = (short)rd["PartNo"];
                if (part == 0)
                {
                    var f = new VFile()
                    {                
                        id = (int)rd["FaceId"],
                        Name = (string)rd["Name"],
                        Size = (Int64)rd["Size"],
                        partFile = new Dictionary<int, int>() { { 0, netfileid } }
                    };  
                    facedir.SubFiles.Add(f);
                    lastfile = f;
                }
                else
                {
                    lastfile.partFile[part] = netfileid;            
                }
                mappedNetFiles.Add(netfileid);
            }
            //create direct files
            foreach( var x in files)
            {
                if(! mappedNetFiles.Contains(x.Key))
                {
                    var f = new VFile();
                    f.id = x.Key;
                    f.Name = x.Value.name;
                    f.Size = x.Value.size;
                    f.UploadTime = x.Value.uploadTime.ToString();
                    facedir.SubFiles.Add(f);
                }

            }
        }
        public async Task<bool> vfs_db_download()
        {
            
            try
            {
                string vvv = await _lanz.get_dirDescription(sys_dir_id);
                Newtonsoft.Json.Linq.JObject json_data = Newtonsoft.Json.Linq.JObject.Parse(vvv);
                var sysFileId = json_data["sys"].ToObject<int>();
                var shareId = _lanz.get_shareId(sysFileId);
                Lanzou.download_file(shareId, db_tempPath + "__vfs.sqlite.db");
                _sqlcon = new System.Data.SQLite.SQLiteConnection("data source=" + db_tempPath + "__vfs.sqlite.db");
                _sqlcon.Open();
            }
            catch (Exception ex)
            {                
                return false;
            }
            return true;
        }

        private void DirTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = (VirtualFileSystem.Directory2)e.NewValue;
            UpdateCurrentDir(item.id, item.Name);
            
        }

        private void UploadFaceFile(object sender, RoutedEventArgs e)
        {

        }
        private void DownloadFaceFile(object sender, RoutedEventArgs e)
        {

        }

        private void DelectFile(object sender, RoutedEventArgs e)
        {

        }
    }
}
