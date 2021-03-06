﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace interfacelift_downloader
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        // 起始页编码
        private static int startPageNo = 0;
        // 结束页编码
        private static int endPageNo = 0;
        // 屏幕尺寸
        private static string screenSize = null;
        // 分辨率
        private static string resolution = null;
        // 路径
        private static string path = null;
        
        // 输入的页码 , 已经选中的屏幕尺寸以及屏幕分辨率
        private static int inputedEndPageNo = 0;
        private static int inputedStartPageNo = 0;

        private static string selectedScreenSize = null;
        private static string selectedResulotion = null;

        // 屏幕尺寸
        private static List<ScreenSize> screenSizes = new List<ScreenSize>();


        Thread thread = null;



        // iPhone 
        List<Resolution> iphone_resolutions = new List<Resolution>();
        // iPad
        List<Resolution> ipad_resolutions = new List<Resolution>();
        // Android
        List<Resolution> android_resolutions = new List<Resolution>();
        // 16:10
        List<Resolution> wide_16_10_resolutions = new List<Resolution>();
        // 16:9
        List<Resolution> wide_16_9_resolutions = new List<Resolution>();
        // 21:9
        List<Resolution> wide_21_9_resolutions = new List<Resolution>();

        public MainWindow()
        {
            InitializeComponent();

            // 标题栏 鼠标按下事件
            titlePanel.MouseDown += titlePanel_MouseDown;
            // 标题字体部分鼠标按下事件
            lblTitle.MouseLeftButtonDown += lblTitle_MouseLeftButtonDown;
            // 最小化按钮按下事件
            btnMin.Click += btnMin_Click;
            // 关闭按钮按下事件
            btnClose.Click += btnClose_Click;
            // 选择文件夹
            btnChooseDir.Click += btnChooseDir_Click;
            // 默认第一个文本框焦点
            tbStartPageNo.Focus();
            
            btnSave.Click += btnSave_Click;
        }

        # region 功能方法
        /// <summary>
        /// 检查参数
        /// </summary>
        /// <returns></returns>
        private bool check()
        {
            lblError.Visibility = Visibility.Visible;

            if (!CommonUtils.IsNumeric(tbStartPageNo.Text))
            {
                lblError.Content = "能不能长点心 , 起始页怎么可能非数字 ? ";
                tbStartPageNo.Focus();
                return false;
            }
            if (!CommonUtils.IsNumeric(tbEndPageNo.Text))
            {
                lblError.Content = "能不能长点心 , 结束页怎么可能非数字 ? ";
                tbEndPageNo.Focus();
                return false;
            }
            if (Int32.Parse(tbEndPageNo.Text) < Int32.Parse(tbStartPageNo.Text))
            {
                lblError.Content = "能不能长点心 , 结束页怎么可能比起始页小 ? ";
                tbEndPageNo.Focus();
                return false;
            }
            if (screenSize == null || screenSize == "" || screenSize.Length == 0)
            {
                lblError.Content = "能不能长点心 , 重新选择屏幕尺寸 ? ";
                cbScreenSize.Focus();
                return false;
            }
            if (resolution == null || resolution == "" || resolution.Length == 0)
            {
                lblError.Content = "能不能长点心 , 重新选择屏幕分辨率 ? ";
                cbResolution.Focus();
                return false;
            }
            if (tbFilePath.Text == "" || tbFilePath.Text.Length < 2)
            {
                lblError.Content = "能不能长点心 , 重新选择正确的存储路径";
                btnChooseDir.Focus();
                return false;
            }

            startPageNo = Int32.Parse(tbStartPageNo.Text);
            inputedStartPageNo = startPageNo;
            inputedEndPageNo = Int32.Parse(tbEndPageNo.Text);

            selectedScreenSize = screenSizes.Find(s => s.Name.Equals(cbScreenSize.SelectedValue)).Value;
            
            selectedResulotion = cbResolution.SelectedValue.ToString();
            if (selectedResulotion.IndexOf("(") != -1)
            {
                selectedResulotion = selectedResulotion.Substring(0, selectedResulotion.IndexOf("("));
            }
            lblError.Visibility = Visibility.Hidden;

            return true;
        }

        /// <summary>
        /// 初始化 屏幕尺寸以及屏幕分辨率
        /// </summary>
        private void InitSR()
        {
            iphone_resolutions.Add(new Resolution("iPhone 6 Plus", "iPhone 6 Plus (2208x2208)", "iphone_6_plus(2208x2208)", "2208x2208"));
            iphone_resolutions.Add(new Resolution("iPhone 6 Plus", "iPhone 6 Plus (1242x2208)", "iphone_6_plus(1242x2208)", "1242x2208"));
            iphone_resolutions.Add(new Resolution("iPhone 6", "iPhone 6", "iphone_6", "750x1334"));
            iphone_resolutions.Add(new Resolution("iPhone 5s, 5c, 5", "iPhone 5s, 5c, 5", "iphone_5s,_5c,_5", "640x1136"));
            iphone_resolutions.Add(new Resolution("iPhone 4, 4S", "iPhone 4, 4S", "iphone_4,_4s", "640x960"));
            iphone_resolutions.Add(new Resolution("iPhone, 3G, 3GS", "iPhone, 3G, 3GS", "iphone,_3g,_3gs", "320x480"));

            ipad_resolutions.Add(new Resolution("iPad Air, 4, 3, Retina Mini", "iPad Air, 4, 3, Retina Mini", "ipad_air,_4,_3,_retina_mini","2048x2048"));
            ipad_resolutions.Add(new Resolution("iPad Mini, iPad 1, 2", "iPad Mini, iPad 1, 2", "ipad_mini,_ipad_1,_2", "1024x1024"));

            android_resolutions.Add(new Resolution("1080x1920 Phone", "1080x1920 Phone", "1080x1920_phone", "2160x1920"));
            android_resolutions.Add(new Resolution("720x1280 Phone", "720x1280 Phone", "720x1280_phone", "1440x1280"));
            android_resolutions.Add(new Resolution("480x854 Phone", "480x854 Phone", "480x854_phone", "960x854"));
            android_resolutions.Add(new Resolution("480x800 Phone", "480x800 Phone", "480x800_phone", "960x800"));
            android_resolutions.Add(new Resolution("1280x800 Tablet", "1280x800 Tablet", "1280x800_tablet", "1920x1408"));

            wide_16_10_resolutions.Add(new Resolution("3840x2400", "3840x2400", "3840x2400", "3840x2400"));
            wide_16_10_resolutions.Add(new Resolution("3360x2100", "3360x2100", "3360x2100", "3360x2100"));
            wide_16_10_resolutions.Add(new Resolution("2880x1800", "2880x1800", "2880x1800", "2880x1800"));
            wide_16_10_resolutions.Add(new Resolution("2560x1600", "2560x1600", "2560x1600", "2560x1600"));
            wide_16_10_resolutions.Add(new Resolution("1920x1200", "1920x1200", "1920x1200", "1920x1200"));
            wide_16_10_resolutions.Add(new Resolution("1680x1050", "1680x1050", "1680x1050", "1680x1050"));
            wide_16_10_resolutions.Add(new Resolution("1440x900", "1440x900", "1440x900", "1440x900"));

            wide_16_9_resolutions.Add(new Resolution("5120x2880", "5120x2880", "5120x2880", "5120x2880"));
            wide_16_9_resolutions.Add(new Resolution("3840x2160", "3840x2160", "3840x2160", "3840x2160"));
            wide_16_9_resolutions.Add(new Resolution("2880x1620", "2880x1620", "2880x1620", "2880x1620"));
            wide_16_9_resolutions.Add(new Resolution("2560x1440", "2560x1440", "2560x1440", "2560x1440"));
            wide_16_9_resolutions.Add(new Resolution("1920x1080", "1920x1080", "1920x1080", "1920x1080"));
            wide_16_9_resolutions.Add(new Resolution("1600x900", "1600x900", "1600x900", "1600x900"));
            wide_16_9_resolutions.Add(new Resolution("1280x720", "1280x720", "1280x720", "1280x720"));

            wide_21_9_resolutions.Add(new Resolution("2560x1080", "2560x1080", "2560x1080", "2560x1080"));


            screenSizes.Add(new ScreenSize("iPhone", "iPhone","iphone", iphone_resolutions));
            screenSizes.Add(new ScreenSize("iPad", "iPad","ipad", ipad_resolutions));
            screenSizes.Add(new ScreenSize("Android", "Android","android", android_resolutions));
            screenSizes.Add(new ScreenSize("Wide 16:10", "Wide 16:10","wide_16:10", wide_16_10_resolutions));
            screenSizes.Add(new ScreenSize("Wide 16:9", "Wide 16:9_目前最流行的电脑屏幕尺寸","wide_19:9", wide_16_9_resolutions));
            screenSizes.Add(new ScreenSize("Wide 21:9", "Wide 21:9","wide_21:9", wide_21_9_resolutions));

        }

        // 下载标识
        int downResult = 0;

        // iPhone 6s plus 分辨率有两个.需要使用一个变量判断并下载到正确的一个
        bool iponeRepeat = false;

        private void getImageUrl(object url){

            this.Dispatcher.Invoke(new Action(() =>
            {
                tbConsole1.ScrollToEnd();
                tbConsole2.ScrollToEnd();
                tbConsole3.ScrollToEnd();
            }));

            this.Dispatcher.Invoke(new Action(() =>
            {
                tbConsole1.Text += "\n---------------\n";
                tbConsole2.Text += "\n\t\tpageNo : " + startPageNo + "\n";
                tbConsole3.Text += "\n---------------\n";
            }));

            // 获取HTML内容
            string html = HttpUtils.getContent(HttpUtils.SendGet((string)url));
            // 正则匹配
            Regex imgRegex = new Regex(@"/wallpaper/[^.\s]{0,}_" + resolution + ".jpg");
            MatchCollection results = imgRegex.Matches(html);

            // 在没找到的情况下,再次判断是不是iPhone 6s plus 两个分辨率的问题
            if (results.Count <= 0)
            {
                imgRegex = new Regex(@"/wallpaper/[^.\s]{0,}_1242x2208.jpg");
                results = imgRegex.Matches(html);
                iponeRepeat = true;
            }
            if (results.Count <= 0)
            {
                MessageBox.Show("没有指定的壁纸信息.请下载其他尺寸/分辨率的壁纸");
                this.Dispatcher.Invoke(new Action(() =>
                {
                    argsPanel.Visibility = Visibility.Visible;
                    previewPanel.Visibility = Visibility.Hidden;
                    btnSave.IsEnabled = true;

                    tbConsole1.Clear();
                    tbConsole2.Clear();
                    tbConsole3.Clear();
                }));
               
                thread.Abort();
                return;
            }

            string fileUrl = null;
   
            foreach (Match item in results)
            {
                fileUrl = item.Value;
                if (iponeRepeat)
                {
                    fileUrl = item.Value.Replace("_1242x2208", "_2208x2208");
                }
                // 获取文件名
                string fileName = item.Value.Substring(item.Value.IndexOf("_") + 1);
                fileName = fileName.Substring(0, fileName.IndexOf("_")) + item.Value.Substring(item.Value.LastIndexOf("."));
                // 操作组件
                this.Dispatcher.Invoke(new Action(() =>
                {
                    tbConsole1.Text += "\ndownloading";
                    tbConsole2.Text += "\n" + fileName;
                    tbConsole3.Text += "\n ...";
                }));

                
                // 下载文件
                downResult = HttpUtils.downFile("http://interfacelift.com/" + fileUrl, path + "\\");

                // 操作组件
                this.Dispatcher.Invoke(new Action(() =>
                {
                    tbConsole3.Text = tbConsole3.Text.Substring(0, tbConsole3.Text.Length - 3);
                    
                    if (downResult == 0)
                        tbConsole3.Text += "failure";
                    else if (downResult == 1)
                        tbConsole3.Text += "successful";
                    else
                        tbConsole3.Text += "already exists";

                    tbConsole1.ScrollToEnd();
                    tbConsole2.ScrollToEnd();
                    tbConsole3.ScrollToEnd();
                }));
            }

            // 下载完一页后换行
            this.Dispatcher.Invoke(new Action(() =>
            {
                tbConsole1.Text += "\n";
                tbConsole2.Text += "\n";
                tbConsole3.Text += "\n";
                tbConsole1.ScrollToEnd();
                tbConsole2.ScrollToEnd();
                tbConsole3.ScrollToEnd();
            }));


            // 如果已经到结束页
            if (startPageNo++ == inputedEndPageNo)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    tbConsole1.Text += "\n---------------\n";
                    tbConsole2.Text += "\n\t已经将 " + inputedStartPageNo + " - " + inputedEndPageNo + " 的图片下载完成\n";
                    tbConsole3.Text += "\n---------------\n";

                    tbConsole1.ScrollToEnd();
                    tbConsole2.ScrollToEnd();
                    tbConsole3.ScrollToEnd();

                }));
                thread.Abort();
            }
            
            // 继续下载
            getImageUrl("https://interfacelift.com/wallpaper/downloads/downloads/" + selectedScreenSize + "/" + selectedResulotion + "/index" + startPageNo + ".html");
            //down();
        }

        private void down()
        {
            thread = new Thread(new ParameterizedThreadStart(getImageUrl));
            thread.IsBackground = true;
            Clipboard.SetText("https://interfacelift.com/wallpaper/downloads/downloads/" + selectedScreenSize + "/" + selectedResulotion + "/index" + tbStartPageNo.Text + ".html");
            thread.Start("https://interfacelift.com/wallpaper/downloads/downloads/" + selectedScreenSize + "/" + selectedResulotion + "/index" + tbStartPageNo.Text + ".html");

        }

        #endregion


        #region 事件绑定

        void btnSave_Click(object sender, RoutedEventArgs e)
        {
            
            // 检查参数
            if (!check())
                return;

            path = tbFilePath.Text + @"\" + selectedScreenSize.Replace(":","_") + @"\" + selectedResulotion + @"\";
            //path = path.Replace(":", "_").Replace("x","_");
            // 如果该目录不存在, 创建
            DirectoryInfo dir = new DirectoryInfo(path);
            if (!dir.Exists)
            {
                dir.Create();
            }
            // 下载
            down();

            argsPanel.Visibility = Visibility.Hidden;
            previewPanel.Visibility = Visibility.Visible;
            btnSave.IsEnabled = false;
        }
        void btnChooseDir_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                tbFilePath.Text = dialog.SelectedPath;
            }
        }
        void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        void btnMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        void lblTitle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch
            { }

        }

        void titlePanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch
            { }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 初始化
            InitSR();

            cbScreenSize.ItemsSource = screenSizes;
            cbScreenSize.DisplayMemberPath = "Intro";
            cbScreenSize.SelectedValuePath = "Name";
        }


        private void cbScreenSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            screenSize = cbScreenSize.SelectedValue.ToString();
            cbResolution.ItemsSource = screenSizes.Find(s => s.Name.Equals(screenSize)).Resolutions;
            cbResolution.DisplayMemberPath = "Intro";
            cbResolution.SelectedValuePath = "Value";
        }
        private void cbResolution_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //resolution = cbResolution.SelectedValue.ToString();
                //resolution = iphone_resolutions.Find(i => i.Value.Equals(cbResolution.SelectedValue.ToString())).Px;

                if (cbResolution.SelectedValue.ToString().IndexOf("iphone") != -1)
                {
                    resolution = iphone_resolutions.Find(i => i.Value.Equals(cbResolution.SelectedValue.ToString())).Px;
                }
                else if (cbResolution.SelectedValue.ToString().IndexOf("ipad") != -1)
                {
                    resolution = ipad_resolutions.Find(i => i.Value.Equals(cbResolution.SelectedValue.ToString())).Px;
                }
                else if (cbResolution.SelectedValue.ToString().IndexOf("_tablet") != -1 || cbResolution.SelectedValue.ToString().IndexOf("_phone") != -1)
                {
                    resolution = android_resolutions.Find(i => i.Value.Equals(cbResolution.SelectedValue.ToString())).Px;
                }
                else 
                {
                    Resolution res = wide_16_10_resolutions.Find(i => i.Value.Equals(cbResolution.SelectedValue.ToString()));
                    if (res != null)
                        resolution = res.Px;
                    else
                    {
                        res = wide_16_9_resolutions.Find(i => i.Value.Equals(cbResolution.SelectedValue.ToString()));
                        if (res != null)
                            resolution = res.Px;
                        else 
                        {
                            res = wide_21_9_resolutions.Find(i => i.Value.Equals(cbResolution.SelectedValue.ToString()));
                            if (res != null)
                                resolution = res.Px;
                            else
                                resolution = "1920x1080";
                        }
                            
                    }
                }
                Console.Write(resolution);
            }
            catch { }
        }
        #endregion

        private void tbConsole1_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //tbConsole2.ScrollToVerticalOffset(tbConsole1.VerticalOffset);
            //tbConsole3.ScrollToVerticalOffset(tbConsole1.VerticalOffset);
        }

        private void tbConsole2_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //tbConsole1.ScrollToVerticalOffset(tbConsole2.VerticalOffset);
            //tbConsole3.ScrollToVerticalOffset(tbConsole2.VerticalOffset);
        }

        private void tbConsole3_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //tbConsole2.ScrollToVerticalOffset(tbConsole3.VerticalOffset);
            //tbConsole1.ScrollToVerticalOffset(tbConsole3.VerticalOffset);
        }

    }
}
