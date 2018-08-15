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
        ObservableCollection<ObjectInfo> downloadList = new ObservableCollection<ObjectInfo>();
        ZtiLib.OssHelper ossHelper;

        int globalIndex = 0;
        double step = 0;
        double remain = 0;

        public string Key { get; set; }

        public string Secret { get; set; }

        public string EndPoint { get; set; }

        public string BucketName { get; set; }

        public string DownloadBucketName { get; set; }

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
            remain = 100 % uploadList.Count;
            step = 100 / uploadList.Count;
            
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
                    this.tab.SelectedIndex = 2;
                });               
                return;
            }

            string folder = "";

            Dispatcher.Invoke(() => {
                ossHelper.BucketName = this.combox_Buckets.Text;
                progress_Upload.Value = 0;
                folder =  this.tbox_Folder.Text;
            });

             
            
            for (globalIndex = 0; globalIndex < uploadList.Count; globalIndex++)
            {
                if(!string.IsNullOrEmpty(folder))
                    ossHelper.PutFile(uploadList[globalIndex].FilePath, folder + "/" + uploadList[globalIndex].FileName);
                else
                    ossHelper.PutFile(uploadList[globalIndex].FilePath, uploadList[globalIndex].FileName);

                Dispatcher.Invoke(() => {
                    if (remain == 0)
                    {
                        progress_Upload.Value += step;
                    }
                    else
                    {
                        if ( globalIndex == ( uploadList.Count - 1))
                        {
                            progress_Upload.Value += step + remain;
                        }
                        else
                        {
                            progress_Upload.Value += step;
                        }
                    }
                });
            }
        }

        private void btn_ConfirmAccessKey_Click(object sender, RoutedEventArgs e)
        {
            Key = this.tbox_Key.Text.Trim();
            Secret = this.tbox_Secret.Text.Trim();
            EndPoint = this.combox_EndPoint.Text.Substring(5);

            if(this.combox_EndPoint.SelectedIndex == -1)
            {
                MessageBox.Show("请选择EndPoint");
                return;
            }

            ossHelper = new ZtiLib.OssHelper(Key,Secret,EndPoint,"",false);
            IEnumerable<Bucket> bucketList = ossHelper.ListBuckets();
            this.combox_Buckets.ItemsSource = bucketList.Select(x => x.Name);
            this.combox_DownloadBuckets.ItemsSource = bucketList.Select(x => x.Name);
            BucketName = bucketList.ElementAt(0).Name;
            ossHelper.BucketName = BucketName;
            MessageBox.Show("配置成功");
        }

        private List<string> EndPointList()
        {
            List<string> list = new List<string>();
            list.Add("华东 1-http://oss-cn-hangzhou.aliyuncs.com");
            list.Add("华东 2-http://oss-cn-shanghai-internal.aliyuncs.com");
            return list;
        }

        private void listview_Upload_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = this.listview_Upload.SelectedIndex;
            if(index != -1)
            {
                if(uploadList[index].FileType == ".jpg" || uploadList[index].FileType == ".bmp" || uploadList[index].FileType == ".png")
                    this.image_Preview.Source = new BitmapImage(new Uri(uploadList[index].FilePath));
            }
        }

        private void combox_DownloadBuckets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(this.combox_DownloadBuckets.SelectedIndex != -1)
            {
                DownloadBucketName = this.combox_DownloadBuckets.SelectedItem.ToString();
                ossHelper.BucketName = DownloadBucketName;
            }
        }

        private void btn_ListFile_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Key) || string.IsNullOrEmpty(Secret))
            {                
                MessageBox.Show("请先配置访问密钥");             
                this.tab.SelectedIndex = 2;
                return;
            }

            if(string.IsNullOrEmpty(DownloadBucketName))
            {
                MessageBox.Show("请选择Bucket");
                return;
            }

            IEnumerable<OssObjectSummary> fileList = ossHelper.ListObjects();

            var result = fileList.Select(x => new ObjectInfo{
                IsCheck = false,
                FileName = x.Key,
                FileSize = (x.Size / 1024).ToString() + "KB"
            });
            foreach (var item in result)
            {
                downloadList.Add(item);
            }
            this.listview_Download.ItemsSource = downloadList;
        }

        private void btn_Download_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cbox_CheckAll_Checked(object sender, RoutedEventArgs e)
        {
            if (cbox_CheckAll.IsChecked.Value == true)            
                downloadList.ToList().ForEach(x => x.IsCheck = true);                       
            else
                downloadList.ToList().ForEach(x => x.IsCheck = false);

            this.listview_Download.ItemsSource = null;
            this.listview_Download.ItemsSource = downloadList;
        }

        private void btn_BrowseDownloadFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.tbox_DownloadFolder.Text = dialog.SelectedPath;
            }
        }
    }
}
