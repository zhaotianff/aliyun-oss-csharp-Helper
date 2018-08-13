using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using Aliyun.OSS;

namespace OssDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<ObjectInfo> uploadList = new ObservableCollection<ObjectInfo>();
        ZtiLib.OssHelper ossHelper;

        int globalIndex = 0;
        double step = 0;
        double remain = 0;

        public string Key { get; set; }

        public string Secret { get; set; }

        public string EndPoint { get; set; }

        public string BucketName { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.combox_EndPoint.ItemsSource = EndPointList();
            this.listview_Upload.ItemsSource = uploadList;
        }

        private void btn_AddFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "全部文件|*.*";
            openDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if(openDialog.ShowDialog() == true)
            {
                AddFile(openDialog.FileName);
            }
        }

        private void btn_AddFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderDialog.RootFolder = Environment.SpecialFolder.Desktop;

            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                List<string> list = System.IO.Directory.GetFiles(folderDialog.SelectedPath).ToList();
                list.ForEach(x => AddFile(x));
            }
        }

        private void btn_Upload_Click(object sender, RoutedEventArgs e)
        {
            System.Threading.Thread thread = new System.Threading.Thread(Upload);
            thread.IsBackground = true;
            thread.Start();
        }

        private void AddFile(string fileName)
        {
           
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileName);
            ObjectInfo objectInfo = new ObjectInfo() 
            {
                FileName = fileInfo.Name,
                FilePath = fileInfo.FullName,
                FileSize = (fileInfo.Length /1024).ToString("0.00") + "KB",
                FileType = fileInfo.Extension,
                Progress = "0%"
            };

            uploadList.Add(objectInfo);
            
        }

        public void Upload()
        {
            if(uploadList.Count == 0)
            {
                Dispatcher.Invoke(() => {
                    MessageBox.Show("请先添加文件");
                }); 
                return;
            }

            if(string.IsNullOrEmpty(Key) || string.IsNullOrEmpty(Secret))
            {
                Dispatcher.Invoke(() => {
                    MessageBox.Show("请先配置访问密钥");
                });
                this.tab.SelectedIndex = 2;
                return;
            }

            Dispatcher.Invoke(() => {
                ossHelper.BucketName = this.combox_Buckets.Text;
            }); 

            for (globalIndex = 0; globalIndex < uploadList.Count; globalIndex++)
            {
                ossHelper.PutFile(uploadList[globalIndex].FilePath, uploadList[globalIndex].FileName);
            }
        }

        private void btn_ConfirmAccessKey_Click(object sender, RoutedEventArgs e)
        {
            Key = this.tbox_Key.Text.Trim();
            Secret = this.tbox_Secret.Text.Trim();
            EndPoint = this.combox_EndPoint.Text.Substring(3);

            if(this.combox_EndPoint.SelectedIndex == -1)
            {
                MessageBox.Show("请选择EndPoint");
                return;
            }

            ossHelper = new ZtiLib.OssHelper(Key,Secret,EndPoint,"",false);
            IEnumerable<Bucket> bucketList = ossHelper.ListBuckets();
            this.combox_Buckets.ItemsSource = bucketList.Select(x => x.Name);
            BucketName = bucketList.ElementAt(0).Name;
            ossHelper.BucketName = BucketName;
            MessageBox.Show("配置成功");
        }

        private List<string> EndPointList()
        {
            List<string> list = new List<string>();
            list.Add("华东1http://oss-cn-hangzhou.aliyuncs.com");
            list.Add("华东2http://oss-cn-shanghai-internal.aliyuncs.com");
            return list;
        }
    }
}
