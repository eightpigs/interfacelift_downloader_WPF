using System;
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

            selectedScreenSize = cbScreenSize.SelectedValue.ToString();
            selectedResulotion = cbResolution.SelectedValue.ToString();

            lblError.Visibility = Visibility.Hidden;

            return true;
        }

        /// <summary>
        /// 初始化 屏幕尺寸以及屏幕分辨率
        /// </summary>
        private void InitSR()
        {
            // iPhone 
            List<Resolution> iphone_resolutions = new List<Resolution>();
            iphone_resolutions.Add(new Resolution("iPhone 6 Plus", "iPhone 6 Plus", "iphone_6_plus"));
            iphone_resolutions.Add(new Resolution("iPhone 6", "iPhone 6", "iphone_6"));
            iphone_resolutions.Add(new Resolution("iPhone 5s, 5c, 5", "iPhone 5s, 5c, 5", "iphone_5s,_5c,_5"));
            iphone_resolutions.Add(new Resolution("iPhone 4, 4S", "iPhone 4, 4S", "iphone_4,_4s"));
            iphone_resolutions.Add(new Resolution("iPhone, 3G, 3GS", "iPhone, 3G, 3GS", "iphone,_3g,_3gs"));

            // iPad
            List<Resolution> ipad_resolutions = new List<Resolution>();
            ipad_resolutions.Add(new Resolution("iPad Air, 4, 3, Retina Mini", "iPad Air, 4, 3, Retina Mini", "ipad_air,_4,_3,_retina_mini"));
            ipad_resolutions.Add(new Resolution("iPad Mini, iPad 1, 2", "iPad Mini, iPad 1, 2", "ipad_mini,_ipad_1,_2"));

            // Android
            List<Resolution> android_resolutions = new List<Resolution>();
            android_resolutions.Add(new Resolution("1080x1920 Phone", "1080x1920 Phone", "1080x1920_phone"));
            android_resolutions.Add(new Resolution("720x1280 Phone", "720x1280 Phone", "720x1280_phone"));
            android_resolutions.Add(new Resolution("480x854 Phone", "480x854 Phone", "480x854_phone"));
            android_resolutions.Add(new Resolution("480x800 Phone", "480x800 Phone", "480x800_phone"));
            android_resolutions.Add(new Resolution("1280x800 Tablet", "1280x800 Tablet", "1280x800_tablet"));

            // 16:10
            List<Resolution> wide_16_10_resolutions = new List<Resolution>();
            wide_16_10_resolutions.Add(new Resolution("3840x2400", "3840x2400", "3840x2400"));
            wide_16_10_resolutions.Add(new Resolution("3360x2100", "3360x2100", "3360x2100"));
            wide_16_10_resolutions.Add(new Resolution("2880x1800", "2880x1800", "2880x1800"));
            wide_16_10_resolutions.Add(new Resolution("2560x1600", "2560x1600", "2560x1600"));
            wide_16_10_resolutions.Add(new Resolution("1920x1200", "1920x1200", "1920x1200"));
            wide_16_10_resolutions.Add(new Resolution("1680x1050", "1680x1050", "1680x1050"));
            wide_16_10_resolutions.Add(new Resolution("1440x900", "1440x900", "1440x900"));

            // 16:9
            List<Resolution> wide_16_9_resolutions = new List<Resolution>();
            wide_16_9_resolutions.Add(new Resolution("5120x2880", "5120x2880", "5120x2880"));
            wide_16_9_resolutions.Add(new Resolution("3840x2160", "3840x2160", "3840x2160"));
            wide_16_9_resolutions.Add(new Resolution("2880x1620", "2880x1620", "2880x1620"));
            wide_16_9_resolutions.Add(new Resolution("2560x1440", "2560x1440", "2560x1440"));
            wide_16_9_resolutions.Add(new Resolution("1920x1080", "1920x1080", "1920x1080"));
            wide_16_9_resolutions.Add(new Resolution("1600x900", "1600x900", "1600x900"));
            wide_16_9_resolutions.Add(new Resolution("1280x720", "1280x720", "1280x720"));

            // 21:9
            List<Resolution> wide_21_9_resolutions = new List<Resolution>();
            wide_21_9_resolutions.Add(new Resolution("2560x1080", "2560x1080", "2560x1080"));


            screenSizes.Add(new ScreenSize("iPhone", "iPhone", iphone_resolutions));
            screenSizes.Add(new ScreenSize("iPad", "iPad", ipad_resolutions));
            screenSizes.Add(new ScreenSize("Android", "Android", android_resolutions));
            screenSizes.Add(new ScreenSize("Wide 16:10", "Wide 16:10", wide_16_10_resolutions));
            screenSizes.Add(new ScreenSize("Wide 16:9", "Wide 16:9_目前最流行的电脑屏幕尺寸", wide_16_9_resolutions));
            screenSizes.Add(new ScreenSize("Wide 21:9", "Wide 21:9", wide_21_9_resolutions));

        }
        Thread thread = null;
        private void getImageUrl(object url){
            // 获取HTML内容
            string html = HttpUtils.getContent(HttpUtils.SendGet((string)url));
            // 正则匹配
            Regex imgRegex = new Regex(@"/wallpaper/[^.\s]{0,}_" + resolution + ".jpg");
            MatchCollection results = imgRegex.Matches(html);

            foreach (Match item in results)
            {
                HttpUtils.downFile("http://interfacelift.com/" + item.Value, path + "\\"); 
            }
            if (++startPageNo == inputedEndPageNo)
            {
                //lblError.Content = "已经将 " + inputedStartPageNo + " - " + inputedEndPageNo + " 的图片下载完成";
                lblError.Content = "下载完成";
                btnChooseDir.Focus();
                thread.Abort();
                return;
            }
            down();
        }

        private void down()
        {
            if(thread != null)
                thread.Abort();
            thread = null;
            thread = new Thread(new ParameterizedThreadStart(getImageUrl));
            thread.IsBackground = true;
            thread.Start("https://interfacelift.com/wallpaper/downloads/downloads/" + selectedScreenSize + "/" + selectedResulotion + "/index_" + startPageNo + ".html");
        }

        #endregion


        #region 事件绑定

        void btnSave_Click(object sender, RoutedEventArgs e)
        {
            
            // 检查参数
            if (!check())
                return;

            // 如果该目录不存在, 创建
            DirectoryInfo dir = new DirectoryInfo(tbFilePath.Text);
            if (!dir.Exists)
            {
                dir.Create();
            }
            path = tbFilePath.Text;
            // 下载
            Thread thread = new Thread(new ParameterizedThreadStart(getImageUrl));
            thread.IsBackground = true;
            thread.Start("https://interfacelift.com/wallpaper/downloads/downloads/" + cbScreenSize.SelectedValue.ToString() + "/" + cbResolution.SelectedValue.ToString() + "/index_" + tbEndPageNo.Text + ".html");
            //getImageUrl("https://interfacelift.com/wallpaper/downloads/downloads/"+cbScreenSize.SelectedValue.ToString()+"/"+cbResolution.SelectedValue.ToString()+"/index_"+tbEndPageNo.Text+".html");
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


        private void previewPanel_Loaded(object sender, RoutedEventArgs e)
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
                resolution = cbResolution.SelectedValue.ToString();
            }
            catch { }
        }
        #endregion



    }
}
