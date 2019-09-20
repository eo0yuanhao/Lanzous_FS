using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Chrome = OpenQA.Selenium.Chrome;
using System.Threading;


namespace Lanzous_FS
{
    class WebBrowserController
    {

        public struct FileInfo
        {
            public string name;
            public string size;
        }
        Chrome.ChromeDriver wbb; // = new Chrome.ChromeDriver();
        private void getFileList_Cloud()
        {
            Thread.Sleep(200);
            var frame = wbb.FindElementByName("mainframe");
            var link = frame.GetAttribute("src");
            //wbb.Url = getHost(wbb.Url) + "/" + link;
            wbb.Url = link;
            Thread.Sleep(200);
            var foldListElem = wbb.FindElementById("sub_folder_list");
            var foldNameElems = foldListElem.FindElements(By.ClassName("f_name2"));
            var foldNames = foldNameElems.Select(x => x.Text);

            ///var fileElems1 = wbb.FindElementById("filelist");
            var fileElems = wbb.FindElements(By.XPath("//div[@id='filelist']/div[@class='f_tb']"));
            var fileInfos = new List<FileInfo>();
            foreach (var x in fileElems)
            {
                FileInfo inf = new FileInfo();
                inf.name = x.FindElement(By.XPath("./div[@class='f_name']/a")).Text;
                inf.size = x.FindElement(By.CssSelector("div.f_size")).Text;
                fileInfos.Add(inf);
            }
            //this.Title = fileInfos.First().name;
        }

        private void login_LanzouCloud()
        {
            //var url = "http://www.lanzou.com/u";
            /// 蓝奏云的重定向地址            
            var reUrl = "https://pc.woozooo.com/mydisk.php";
            wbb.Url = reUrl;
            try
            {
                System.Threading.Thread.Sleep(300);
                //this.Title = wbb.Title;
                //string doc = driver.PageSource;

                var userBox = wbb.FindElementByName("username");
                userBox.SendKeys("15549071306");
                var pwdBox = wbb.FindElementByName("password");
                pwdBox.SendKeys("foxlink0.");
                var confirm = wbb.FindElement(By.Id("s3"));
                confirm.Click();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void loadWebBrowser()
        {
            Chrome.ChromeDriverService driverSvc = Chrome.ChromeDriverService.CreateDefaultService();
            driverSvc.HideCommandPromptWindow = true;

            Chrome.ChromeOptions ops = new Chrome.ChromeOptions();
            //ops.AddArgument("--headless");
            wbb = new Chrome.ChromeDriver(driverSvc, ops);

            //开启无头模式
            //chrome_options = Options()
            //chrome_options.add_argument('--headless')
            //driver = webdriver.Chrome(chrome_options = chrome_options)

            //禁止显示命令行界面
            //var cdSvc = ChromeDriverService.CreateDefaultService();
            //cdSvc.HideCommandPromptWindow = true;
            //IWebDriver driver = new ChromeDriver(cdSvc);

        }
    }


}
