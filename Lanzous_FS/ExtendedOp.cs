using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Lanzous_FS
{
    using VFile = VirtualFileSystem.File;
    using Kis = System.Collections.Generic.Dictionary<int, VirtualFileSystem.NetFileInfo>;
    using Sis = System.Collections.Generic.Dictionary<string, string>;
    public class Example_Work
    {
        public string Get_Html(string url)
        {
            string text = "";
            HttpWebRequest httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
            httpWebRequest.Method = "GET";
            httpWebRequest.Timeout = 6000;
            httpWebRequest.UserAgent = "Mozilla/5.0 (Linux; U; Android 2.2; en-us; Nexus One Build/FRF91) AppleWebKit/533.1 (KHTML, like Gecko) Version/4.0 Mobile Safari/533.1";
            string result;
            try
            {
                using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8))
                    {
                        text = streamReader.ReadToEnd();
                        streamReader.Close();
                        httpWebResponse.Close();
                    }
                }
                result = text;
            }
            catch
            {
                result = null;
            }
            string text7 = "urlp = '";
            string text8 = "'";
            string text9 = new Regex(string.Concat(new string[]
            {
                                "(?<=(",
                                text7,
                                "))[.\\s\\S]*?(?=(",
                                text8,
                                "))"
            }), RegexOptions.Multiline | RegexOptions.Singleline).Match(text).Value;
            string text10 = "submit.href = urlp";
            string text11 = "}";
            string text12 = new Regex(string.Concat(new string[]
            {
                                "(?<=(",
                                text10,
                                ")).*?(?=(",
                                text11,
                                "))"
            }), RegexOptions.Multiline | RegexOptions.Singleline).Match(text).Value;
            result = (text9 + text12).Replace("'", "").Replace("   ?", "?");
            if (result != "")
            {
                HttpWebRequest httpWebRequest8 = (HttpWebRequest)WebRequest.Create(result);
                httpWebRequest8.Headers.Add("Accept-Language:zh-CN,zh;q=0.8");
                WebResponse response = httpWebRequest8.GetResponse();
                result = response.ResponseUri.ToString();
                response.Close();
            }
            return result;
        }


        public static string Browser_GetHtmlDoc_phone(string url)
        {

            //伪装成手机浏览器
            var hr = (HttpWebRequest)WebRequest.Create(url);
            var pua = "Mozilla/5.0 (Linux; Android 8.0; Pixel 2 Build/OPD3.170816.012)"
                      + " AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.75 Mobile Safari/537.36";
            hr.UserAgent = pua;
            var hostIndex = url.IndexOf(".com");
            hr.Referer = url.Substring(0, hostIndex + 4);
            hr.Headers.Add(HttpRequestHeader.AcceptLanguage, "zh-cn,en-us;q=0.5");
            var resp = hr.GetResponse();
            var bufLen = resp.ContentLength;
            var buf = new byte[bufLen];
            //resp.GetResponseStream().Read(buf, 0,(int) bufLen);
            //string hdoc = System.Text.UTF8Encoding.UTF8.GetString(buf);
            var streamReader = new StreamReader(resp.GetResponseStream(), UTF8Encoding.UTF8);
            string hdoc = streamReader.ReadToEnd();
            return hdoc;
        }
        public static void Browser_work1()
        {
            //var link = siteBox.Text;// "www.lanzous.com/i4iufah";// "https://www.lanzous.com/i4iufah";
            //var apiSite = "https://api.805ds.com/lan/api.php?url=" + link+ "&pwd=";
            //var req =WebRequest.Create(apiSite) as HttpWebRequest;
            //var resp = req.GetResponse();
            //var stream = resp.GetResponseStream();
            //var buf = new byte[10*1024];
            //stream.Read(buf, 0,(int) resp.ContentLength);
            ////var rt = buf.ToString();
            //var rt = System.Text.UTF8Encoding.UTF8.GetString(buf);
            //MessageBox.Show(rt.Length> 50? rt.Substring(0, 50):rt);


            var fid = "i5ydxrc";
            var lanz = "https://www.lanzous.com/tp/";

            var hdoc = Browser_GetHtmlDoc_phone(lanz + fid);

            var jsMatch = Regex.Match(hdoc, "text/javascript\">");
            if (!jsMatch.Success)
                return;
            var jsText = hdoc.Substring(jsMatch.Index + jsMatch.Length);
            var fileAddr1 = Regex.Match(jsText, @"\nvar dpost = '(.*?)';");
            //var fileAddr2 = Regex.Match(jsText, "submit.href = dpost \\+ \\\"(.*?)\\\"");//       "submit.href = dpost + \""+ @"\(\s*?\)" +  "\""  );
            var fileAddr2 = Regex.Match(jsText, @"submit.href = dpost \+ ""(.*?)""");
            var fileAddr_tmp = fileAddr1.Groups[1].Value + fileAddr2.Groups[1].Value;

            var infoStartIndex = hdoc.IndexOf("<div class=\"md\">");
            var infoEndIndex = hdoc.IndexOf("发布者:</span>", infoStartIndex + 15);
            var fileInfoText = hdoc.Substring(infoStartIndex, infoEndIndex - infoStartIndex);
            var fileTitle = Regex.Match(fileInfoText, "<div class=\"md\">(.*?)<span ").NextMatch().Value;
            var fileSize = Regex.Match(fileInfoText, "<span class=\"mtt\">(.*?)</span>").NextMatch().Value;
            var uploadTime = Regex.Match(fileInfoText, "时间:</span>(.*?)<span").NextMatch().Value;

            var doc_fileaddr = Browser_GetHtmlDoc_phone(fileAddr_tmp);
            Clipboard.SetText(doc_fileaddr);
        }

        public static string Post(string Url, Dictionary<string, string> data, string cookies)
        {
            //var postDataStr = data.Select(x => x.Key +"="+ x.Value).Sum
            string postDataStr = data.Aggregate("", ((a, k) => a + "&" + k.Key + "=" + k.Value));
            return Post(Url, postDataStr, cookies);
        }
        public static string Post(string Url, string postDataStr, string cookies)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            ///设置POST很重要
            request.Method = "POST";
            if (cookies != null)
                request.Headers.Add("Cookie", cookies);
            ///设置ContentType很重要
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postDataStr.Length;
            StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.ASCII);
            writer.Write(postDataStr);
            writer.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string encoding = response.ContentEncoding;
            if (encoding == null || encoding.Length < 1)
            {
                encoding = "UTF-8"; //默认编码  
            }
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
            string retString = reader.ReadToEnd();
            return retString;
        }
        public static string Post(HttpWebRequest fresh_request, string postDataStr)
        {
            var request = fresh_request;
            ///设置POST很重要
            request.Method = "POST";
            ///设置ContentType很重要
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postDataStr.Length;
            StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.ASCII);
            writer.Write(postDataStr);
            writer.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string encoding = response.ContentEncoding;
            if (encoding == null || encoding.Length < 1)
            {
                encoding = "UTF-8"; //默认编码  
            }
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
            string retString = reader.ReadToEnd();
            return retString;
        }
        public static string Post(HttpWebRequest fresh_request, Dictionary<string, string> data)
        {
            string postDataStr = data.Aggregate("", ((a, k) => a + "&" + k.Key + "=" + k.Value));
            return Post(fresh_request, postDataStr);
        }
        public static string Get(HttpWebRequest fresh_request)
        {
            var request = fresh_request;
            ///设置POST很重要
            //request.Method = "GET";
            ///设置ContentType很重要
            //request.ContentType = "application/x-www-form-urlencoded";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string encoding = response.ContentEncoding;
            if (encoding == null || encoding.Length < 1)
            {
                encoding = "UTF-8"; //默认编码  
            }
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
            string retString = reader.ReadToEnd();
            return retString;
        }
    }


    public class Lanzou
    {
        public static string pcUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Safari/537.36";
        public static string phoneUserAgent = "Mozilla / 5.0(Linux; Android 8.0; Pixel 2 Build/OPD3.170816.012) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.75 Mobile Safari/537.36";
        bool isLogin = false;
        public CookieContainer cookieBag = new CookieContainer();
        HttpClient _hc;
        Dictionary<string, int> curFolders;// = new Dictionary<string, int>();
        Dictionary<string, int> curFiles;// = new Dictionary<string, int>();

        public void restoreCookies(CookieContainer cookies)
        {
            cookieBag = cookies;
            HttpClientHandler hander = new HttpClientHandler()
            {
                CookieContainer = cookieBag,
                AllowAutoRedirect = true,
                UseCookies = true
            };
            _hc = new HttpClient(hander);
            isLogin = true;
        }
        public bool login_forInter(string username, string password)
        {
            var loginUrl = "https://pc.woozooo.com/account.php";
            var data = new Dictionary<string, string>();
            data["action"] = "login";
            data["task"] = "login";
            data["ref"] = "https://up.woozooo.com/";
            data["formhash"] = "0af1aa15";
            data["username"] = username;
            data["password"] = password;

            HttpClientHandler hander = new HttpClientHandler()
            {
                CookieContainer = cookieBag,
                AllowAutoRedirect = true,
                UseCookies = true
            };
            _hc = new HttpClient(hander);
            var httpContent = new FormUrlEncodedContent(data);
            var resp__ = _hc.PostAsync(loginUrl, httpContent);
            resp__.Wait();
            var hdoc__ = resp__.Result.Content.ReadAsStringAsync();
            hdoc__.Wait();
            isLogin = hdoc__.Result.Contains("登录成功");
            return isLogin;
        }
        public bool login(string name, string password)
        {

            var loginUrl = "https://pc.woozooo.com/account.php";
            var data = new Dictionary<string, string>();
            data["action"] = "login";
            data["task"] = "login";
            data["ref"] = "https://up.woozooo.com/";
            data["formhash"] = "0af1aa15";
            data["username"] = "15549071306";
            data["password"] = "foxlink0.";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(loginUrl);

            req.CookieContainer = cookieBag;
            var hdoc = Example_Work.Post(req, data);
            isLogin = hdoc.Contains("登录成功");

            HttpClientHandler hander = new HttpClientHandler()
            {
                CookieContainer = cookieBag,
                AllowAutoRedirect = true,
                UseCookies = true
            };         
            _hc = new HttpClient(hander);
            return isLogin;

            //var hr = (HttpWebRequest)WebRequest.Create(loginUrl);
            //hr.Referer = "https://www.lanzous.com";
            //hr.Headers.Add(HttpRequestHeader.AcceptLanguage, "zh-cn,en-us;q=0.5");
            //hr.UserAgent = phoneUserAgent;
            //hr

            //var fid = "i5ydxrc";
            //var lanz = "https://www.lanzous.com/tp/";
        }
        //public static void set_chrome_phoneDeviceInfo(HttpWebRequest hr)
        //{
        //    //伪装成手机浏览器
        //    var pua = "Mozilla/5.0 (Linux; Android 8.0; Pixel 2 Build/OPD3.170816.012)"
        //              + " AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.75 Mobile Safari/537.36";
        //    hr.UserAgent = pua;

        //    var hostIndex = url.IndexOf(".com");
        //    hr.Referer = url.Substring(0, hostIndex + 4);
        //    hr.Headers.Add(HttpRequestHeader.AcceptLanguage, "zh-cn,en-us;q=0.5");
        //}
        public bool testLogin()
        { 
            var data = new Sis()
            {
                { "task","18"},
                { "folder_id","-1" }
            };
            HttpContent httpContent = new FormUrlEncodedContent(data);
            var result_task = _hc.PostAsync("https://pc.woozooo.com/doupload.php", httpContent);
            result_task.Wait();
            var json_task = result_task.Result.Content.ReadAsStringAsync();
            json_task.Wait();
            Newtonsoft.Json.Linq.JObject json_data = Newtonsoft.Json.Linq.JObject.Parse( json_task.Result);
            if (json_data["zt"].ToObject<int>() != 1)
                return false;
            var tt = json_data["info"]["des"].ToString();
            if(json_data["info"]["des"].ToString()==null)
                return false;
            return true;
        }
        public async Task<string> get_dirDescription(int dirId)
        {
            var data = new Sis()
                {
                    { "task","18"},
                    { "folder_id",dirId.ToString() }
                };
            HttpContent httpContent = new FormUrlEncodedContent(data);
            var result = await _hc.PostAsync("https://pc.woozooo.com/doupload.php", httpContent);
            Newtonsoft.Json.Linq.JObject json_data = Newtonsoft.Json.Linq.JObject.Parse(await result.Content.ReadAsStringAsync());
            if (json_data["zt"].ToObject<int>() == 1)
                return json_data["info"]["des"].ToString();
            return null;
        }
        public async void get_fileList__pre1(string path)
        {
            string folderId = get_folderId(path).ToString();
            var d = new Dictionary<string, string>();
            ///  py: dict(task = 5, folder_id = folder_id, pg = page, headers = self.header_pc)
            //d.Add("task", "5");
            //d.Add("folder_id", folderId);
            //d.Add("pg", "5");
            //d.Add("headers", "5");

            //operator << ()
            //Func<Tuple<string,string >,Dictinary<string,string>> 
            //    [Serializable]
            //    public class transData
            //{
            //    int am;
            //    string name;
            //    transData child;
            //} 
            //FormUrlEncodedContent content = new FormUrlEncodedContent(d);
            //String.Format()
            var folders = new Dictionary<string, int>();
            var files = new Dictionary<string, int>();
            string jsonContentUnformatted = "{{\"task\":5,\"folder_id\":" + folderId + ",\"pg\":{0:D},\"headers\":{{\"User-Agent\":\""
                        + pcUserAgent + "\",\"Accept-Language\":\"zh-CN,zh;q=0.9\"}}}}";
            for (int page = 1; ; page++)
            {
                string json = String.Format(jsonContentUnformatted, page);
                //HttpWebRequest req = (HttpWebRequest) WebRequest.Create("https://pc.woozooo.com/doupload.php");
                //req.CookieContainer = cookieBag;
                //req.UserAgent = pcUserAgent;
                //req.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
                //string hdoc = Example_Work.Post(req, json);

                HttpContent httpContent = new StringContent("");
                httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                var hResp = await _hc.PostAsync("https://pc.woozooo.com/doupload.php", httpContent);
                string hdoc = await hResp.Content.ReadAsStringAsync();
                Newtonsoft.Json.Linq.JObject json_data = Newtonsoft.Json.Linq.JObject.Parse(hdoc);
                int info = json_data["info"].ToObject<int>();
                if (info != 1)
                    break;
                //for file in r["text"]:
                //    file_list[file["name_all"]] = file["id"]
                //var ovv= from x in json_data["text"] select  x["name_all"]
                foreach (var v in json_data["text"])
                {
                    files.Add(v["name_all"].ToString(), v["id"].ToObject<int>());
                }
                curFiles = files;

            }

            //var ret = new Tuple<List<string>, List<string> >(folders, files);
            //return ret;
        }
        public async Task<bool> changeDirInfo(int dirId, string name, string description)
        {//'task':4,'folder_id':folid,'folder_name':name,'folder_description':desc
            var data = new Sis()
                {
                    { "task","4"},

                    { "folder_id",dirId.ToString() },
                    { "folder_name",name },
                    { "folder_description",description }
                };
            HttpContent httpContent = new FormUrlEncodedContent(data);
            var result = await _hc.PostAsync("https://pc.woozooo.com/doupload.php", httpContent);
            Newtonsoft.Json.Linq.JObject json_data = Newtonsoft.Json.Linq.JObject.Parse(await result.Content.ReadAsStringAsync());
            if (json_data["zt"].ToObject<int>() == 1)
                return true;
            return false;
        }
        public async Task<bool> changeFileInfo(int dirId, string name, string description)
        {//'task':11,'file_id':fid,'desc':desc
            var data = new Sis()
                {
                    { "task","11"},
                    { "file_id",dirId.ToString() },
                    { "folder_name",name },
                    { "desc",description }
                };
            HttpContent httpContent = new FormUrlEncodedContent(data);
            var result = await _hc.PostAsync("https://pc.woozooo.com/doupload.php", httpContent);
            Newtonsoft.Json.Linq.JObject json_data = Newtonsoft.Json.Linq.JObject.Parse(await result.Content.ReadAsStringAsync());
            if (json_data["zt"].ToObject<int>() == 1)
                return true;
            return false;
        }
        public Kis get_subFolders(int parentId)
        {
            // Range not supported by website
            //_hc.DefaultRequestHeaders.Range = new System.Net.Http.Headers.RangeHeaderValue(2285 + 26, null);
            var hdoc_task = _hc.GetStringAsync("https://pc.woozooo.com/mydisk.php?item=files&action=index&folder_id=" + parentId);
            hdoc_task.Wait();
            var hdoc = hdoc_task.Result;

            var matches = Regex.Matches(hdoc, "&nbsp;(.+?)</a>&nbsp;.+folkey\\((.+?)\\)");
            var folders = new Kis();
            foreach (Match m in matches)
            {
                var fi = new VirtualFileSystem.NetFileInfo();
                fi.name = m.Groups[1].Value;
                folders.Add(int.Parse(m.Groups[2].Value), fi);
                //folders[i].name = ;
            }
            return folders;
        }
        public Kis get_subFiles(int parentId)
        {
            var files = new Kis();
            string folderId = parentId.ToString();
            var d = new Dictionary<string, string>();
            d.Add("task", "5");
            d.Add("folder_id", folderId);

            //var folders = new Dictionary<string, int>();
            for (int page = 1; ; page++)
            {
                d["pg"] = page.ToString();
                HttpContent httpContent = new FormUrlEncodedContent(d);
                var hResp_task = _hc.PostAsync("https://pc.woozooo.com/doupload.php", httpContent);
                hResp_task.Wait();
                var hjson_task = hResp_task.Result.Content.ReadAsStringAsync();
                hjson_task.Wait();
                Newtonsoft.Json.Linq.JObject json_data = Newtonsoft.Json.Linq.JObject.Parse(hjson_task.Result);
                int info = json_data["info"].ToObject<int>();
                if (info != 1)
                    break;
                foreach (var v in json_data["text"])
                {
                    var fi = new VirtualFileSystem.NetFileInfo();
                    fi.name = v["name_all"].ToString();

                    files.Add(v["id"].ToObject<int>(), fi);
                }
            }
            return files;
        }
        public async void get_fileList(string path)
        {
            string folderId = get_folderId(path).ToString();
            var d = new Dictionary<string, string>();
            d.Add("task", "5");
            d.Add("folder_id", folderId);

            var files = new Dictionary<string, int>();
            var folders = new Dictionary<string, int>();
            for (int page = 1; ; page++)
            {
                d["pg"] = page.ToString();
                HttpContent httpContent = new FormUrlEncodedContent(d);
                var hResp = await _hc.PostAsync("https://pc.woozooo.com/doupload.php", httpContent);
                string hjson = await hResp.Content.ReadAsStringAsync();
                Newtonsoft.Json.Linq.JObject json_data = Newtonsoft.Json.Linq.JObject.Parse(hjson);
                int info = json_data["info"].ToObject<int>();
                if (info != 1)
                    break;
                //for file in r["text"]:
                //    file_list[file["name_all"]] = file["id"]
                //var ovv= from x in json_data["text"] select  x["name_all"]
                foreach (var v in json_data["text"])
                {
                    files.Add(v["name_all"].ToString(), v["id"].ToObject<int>());
                }

                var hdoc = await _hc.GetStringAsync("https://pc.woozooo.com/mydisk.php?item=files&action=index&folder_id=" + folderId);
                var matches = Regex.Matches(hdoc, "&nbsp;(.+?)</a>&nbsp;.+folkey\\((.+?)\\)");
                foreach (Match m in matches)
                {
                    folders[m.Groups[1].Value] = int.Parse(m.Groups[2].Value);
                }

            }
            curFiles = files;
            curFolders = folders;
        }
        private int get_folderId(string path)
        {
            return -1;
        }

        public string get_shareId(int fid)
        {
            //fid = "11769053";
            var lanz = "https://pc.woozooo.com/doupload.php";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(lanz);
            req.CookieContainer = cookieBag;
            //req.UserAgent = phoneUserAgent;
            var data = new Dictionary<string, string>();
            data["task"] = "22";
            data["file_id"] = fid.ToString();
            var retJson = Newtonsoft.Json.Linq.JObject.Parse(Example_Work.Post(req, data));
            if (retJson["zt"].ToObject<int>() != 1)
                return null;
            return retJson["info"]["f_id"].ToString();
        }

        public static string get_tempUrl(string tempUrl)
        {
            HttpWebRequest req2 = (HttpWebRequest)WebRequest.Create(tempUrl);
            req2.UserAgent = phoneUserAgent;
            req2.AllowAutoRedirect = false;
            req2.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            var location = req2.GetResponse().Headers["Location"];
            return location;
        }
        public static string get_validationUrl(string shareId)
        {
            /// 直接GET请求 https://www.lanzous.com/tp/ID 提取直连，此时需要UA设置为手机，PC版无法提取此链接
            /// 手机端返回的html中有一段js暴露了服务器IP和相关参数
            var url = "https://www.lanzous.com/tp/" + shareId;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            /// <script type="text/javascript">
            /// var urlpt = 'http://120.55.32.134/file/';   # 这个是IP
            ///     (function(document) {
            /// var submit = document.getElementById('submit');
            ///         //var urlpt = 'http://120.55.32.134/file/';
            /// submit.onfocus = submit.onmousedown = function() {
            /// submit.href = urlpt + "XXXXXXXXXXX"  # 这个是参数
            ///     }})(document);
            /// </script>
            req.UserAgent = phoneUserAgent;
            var hdoc = Example_Work.Get(req);
            var hostMatch = Regex.Match(hdoc, @"\nvar dpost = '(.*?)';");
            var host = hostMatch.Groups[1].Value;
            var href = Regex.Match(hdoc, @"submit.href = dpost \+ ""(.*?)""").Groups[1].Value;

            var temp_url = host + href;
            return temp_url;
        }

        public static bool download_file(string shareId, string path)
        {
            var stm = get_fileStream(shareId);
            if (stm == null)
                return false;
            try
            {
                saveStream(stm, path);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public void upload_file(string path, int folder_id)
        {
            //post_data = {
            //    "task": "1",
            //    "folder_id": str(folder_id),
            //    "id": "WU_FILE_0",
            //    "name": file_name,
            //    "type": "application/octet-stream",
            //    "upload_file": (file_name, open(file_path, 'rb'), 'application/octet-stream')
            //}   
            string file_name = System.IO.Path.GetFileName(path);
            var dataContent = new MultipartFormDataContent();
            var d = dataContent;
            d.Add(new StringContent("1"), "task");
            d.Add(new StringContent("-1"), "folder_id");
            d.Add(new StringContent("WU_FILE_0"), "id");
            d.Add(new StringContent(file_name), "name");
            d.Add(new StringContent("application/octet-stream"), "type");
            var sc = new StreamContent(File.OpenRead(path));

            sc.Headers.ContentType = d.Headers.ContentType;
            //d.Add(new StringContent("WU_FILE_0"), "upload_file");

            d.Add(sc, "upload_file", file_name);
            //sc.Headers.ContentDisposition
            var result = _hc.PostAsync("http://pc.woozooo.com/fileup.php", d);
            result.Wait();

        }
        public static Stream get_fileStream(string shareId)
        {
            var tid = get_validationUrl(shareId);
            var dui = get_tempUrl(tid);
            HttpWebRequest req = WebRequest.Create(dui) as HttpWebRequest;
            //req.AddRange(1, 3);
            //var stm1 = req.GetResponse().GetResponseStream();
            //saveStream(stm1, "a:/mmc1.txt");
            //stm1.Close();
            ///req.Headers.Remove("Range");
            ///直接移除Range错误，
            //req.Headers[HttpRequestHeader.Range] = "bytes=4-6";
            //req.UserAgent = phoneUserAgent;
            //if (!req.HaveResponse)
            //    return null;
            try
            {
                var r2 = req.GetResponse();
                return req.GetResponse().GetResponseStream();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public int mkdir(string dirName, int parentId)
        {
            var d = new Dictionary<string, string>()
                {
                    { "task","2"},
                    { "parent_id", parentId.ToString()},
                    { "folder_name", dirName},
                    { "folder_description", ""}
                };
            //d.Add("task", "2");
            //d.Add("parent_id", parentId.ToString());
            //d.Add("folder_name", dirName);
            //d.Add("folder_description", "");
            var httpContent = new FormUrlEncodedContent(d);
            var msg_task = _hc.PostAsync("https://pc.woozooo.com/doupload.php", httpContent);
            msg_task.Wait();
            var hjson_task = msg_task.Result.Content.ReadAsStringAsync();
            hjson_task.Wait();
            var json_data = Newtonsoft.Json.Linq.JObject.Parse(hjson_task.Result);
            if (json_data["zt"].ToObject<int>() != 1)
            {
                return 0;
            }
            //获取新建的文件夹的id

            return 1;
        }
        public void mkdir(string dirName, string parentPath)
        {

        }
        public static void saveStream(Stream stm, string path)
        {
            using (var fs = File.Create(path))
                stm.CopyTo(fs);
        }
    }
    public static class CommonMethod
    {
        public class DicAdder
        {
            public Dictionary<string, string> d;
            public Dictionary<string, string> bind(Dictionary<string, string> dic) => d = dic;
            public static Dictionary<string, string> operator +(DicAdder tis, ValueTuple<string, string> t)
            {
                tis.d.Add(t.Item1, t.Item2); return tis.d;
            }
        }
        public class AA
        {
            public string name;
            public int id;
            public Dictionary<string, string> headers;
        }
        //public static TResult AppendSource<TInput, TResult>(this TInput input, Func<TInput, TResult> func) where TInput : IAudioSource;
        //public static Dictionary<string, string> operator << (this Dictionary<string, string> dic, Tuple<string, string> dat)
        //{
        //    Dictionary<string, string> aaa = new Dictionary<string, string>();

        //    return aaa;
        //}
    }


    internal class ClickCounter
    {
        readonly Func<Point> mousePosition;
        internal object ClickedObject;
        internal bool IsRunning { get; set; }

        internal ClickCounter(Func<Point> mousePosition)
        {
            this.mousePosition = mousePosition;
            _clickTimer.Tick += TimeTick;
            _clickTimer.Interval = TimeSpan.FromMilliseconds(500);

        }

        internal int DownCount { get; set; }
        internal int UpCount { get; set; }
        readonly System.Windows.Threading.DispatcherTimer _clickTimer = new System.Windows.Threading.DispatcherTimer();
        internal Point LastDownClickPosition;

        internal void AddMouseDown()
        {
            if (!IsRunning)
            {
                DownCount = 0;
                UpCount = 0;
                _clickTimer.Start();
                IsRunning = true;

            }
            LastDownClickPosition = this.mousePosition();
            DownCount++;
        }

        internal void AddMouseUp()
        {
            const double minDistanceForClickDownAndUp = 0.1;
            if (IsRunning)
            {
                if ((mousePosition() - LastDownClickPosition).Length > minDistanceForClickDownAndUp)
                {
                    //it is not a click
                    UpCount = 0;
                    DownCount = 0;
                    _clickTimer.Stop();
                    IsRunning = false;
                }
                else
                    UpCount++;
            }
        }


        void TimeTick(object sender, EventArgs e)
        {
            _clickTimer.Stop();
            IsRunning = false;
            OnElapsed();
        }

        public event EventHandler<EventArgs> Elapsed;

        protected virtual void OnElapsed()
        {
            EventHandler<EventArgs> handler = Elapsed;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }

}
