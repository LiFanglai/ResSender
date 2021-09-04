using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
namespace ResSender
{
    public enum InstLevel
    {
        省国资委 = 0,
        省属国有企业 = 1,
        市县国资监管机构 = 2,
        其他 = 3,
    };

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static List<List<string>> instList = new List<List<string>>()
        {
            new List<string>()
            {
                "委领导班子成员",
                "委机关各处室",
                "研究中心",
                "浙江产权交易所",
            },

            new List<string>()
            {
                "浙江省国有资本运营有限公司",
                "物产中大集团股份有限公司",
                "浙江省建设投资集团股份有限公司",
                "浙江省机电集团有限公司",
                "浙江省国际贸易集团有限公司",
                "浙江省旅游集团有限责任公司",
                "杭州钢铁集团有限公司",
                "巨化集团有限公司",
                "浙江省能源集团有限公司",
                "浙江省交通投资集团有限公司",
                "浙江省农村发展集团有限公司",
                "浙江省机场集团有限公司",
                "浙江省海港投资运营集团有限公司",
                "浙江省二轻集团有限公司",
                "浙江安邦护卫集团有限公司",
                "浙商银行股份有限公司",
                "浙江省农村信用社联合社",
                "浙江财通证券股份有限公司",
                "浙江出版联合集团有限公司",
                "浙江省文化产业投资集团有限公司",
                "浙江省金融控股有限公司",
                "浙江省担保集团有限公司",
                "浙江浙勤集团有限公司",
                "浙江省财务开发有限责任公司",
            },

            new List<string>()
            {
                "杭州市国资委",
                "宁波市国资委",
                "温州市国资委",
                "湖州市国资委",
                "嘉兴市国资委",
                "绍兴市国资委",
                "金华市国资委",
                "衢州市国资委",
                "舟山市财政局",
                "台州市国资委",
                "丽水市国资委",
            },

            new List<string>()
            {
                "",
            },
        };
        static List<ResFile> resList = new List<ResFile>();
        static string rootFolder = "";
        static string serverIP = "192.168.2.207";
        static int port = 2309;

        public MainWindow()
        {
            InitializeComponent();

            var enumDataSrc = System.Enum.GetValues(typeof(InstLevel));

            comboLevel.ItemsSource = enumDataSrc;
            comboLevel.SelectionChanged += OnComboLevelChanged;
            comboLevel.SelectedIndex = 0;

            datePicker.SelectedDate = DateTime.Today;

            fileDataGrid.SelectionMode = DataGridSelectionMode.Extended;
            fileDataGrid.SelectionUnit = DataGridSelectionUnit.FullRow;
            fileDataGrid.CanUserAddRows = false;
            fileDataGrid.CanUserDeleteRows = false;
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            if (fileDataGrid.SelectedItems.Count > 0)
            {
                for (int i = 0; i < fileDataGrid.SelectedItems.Count; i++)
                {
                    ResFile rf = fileDataGrid.SelectedItems[i] as ResFile;
                    rf.Level = (InstLevel)comboLevel.SelectedItem;
                    rf.InstIndex = comboInst.SelectedIndex;
                    rf.keyword = textKeyword.Text;
                }
            }

            fileDataGrid.Items.Refresh();
            textKeyword.Text = "";
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            var openFiledlg = new FolderBrowserDialog();
            var result = openFiledlg.ShowDialog();
            rootFolder = openFiledlg.SelectedPath;

            resList.Clear();
            getFileRecur(rootFolder, rootFolder);

            fileDataGrid.ItemsSource = resList;
            fileDataGrid.Items.Refresh();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnComboLevelChanged(object sender, SelectionChangedEventArgs e)
        {
            comboInst.ItemsSource = instList[comboLevel.SelectedIndex];
            comboInst.SelectedIndex = 0;
        }

        public static void getFileRecur(string path, string root)
        {
            if (Directory.Exists(path))
            {
                foreach (string p in Directory.GetFiles(path))
                {
                    resList.Add(new ResFile() { Path = p, Level = InstLevel.其他, InstIndex = 0, keyword = "" });
                }

                foreach (string subDir in Directory.GetDirectories(path))
                {
                    getFileRecur(subDir, root);
                }
            }
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            if (resList.Count <= 0)
            {
                System.Windows.MessageBox.Show("还未选择任何文件", "提示");
                return;
            }

            try
            {
                TcpClient client = new TcpClient(serverIP, port);
                NetworkStream ns = client.GetStream();
                StreamWriter sr = new StreamWriter(ns);
                byte[] response = new byte[4];

                //文件总数
                sr.WriteLine(resList.Count);
                sr.Flush();

                //YYYYDDMM格式的时间
                sr.WriteLine(datePicker.SelectedDate.Value.ToString("yyyyMMdd"));
                sr.Flush();

                for (int i = 0; i < resList.Count; i++)
                {
                    //文件字节长度
                    byte[] fileBytes = File.ReadAllBytes(resList[i].Path);
                    sr.WriteLine(fileBytes.Length);
                    sr.Flush();

                    //文件相对路径
                    string fileName = Path.GetRelativePath(rootFolder, resList[i].Path);
                    sr.WriteLine(fileName);
                    sr.Flush();

                    //级别
                    sr.WriteLine((int)resList[i].Level);
                    sr.Flush();

                    //单位id
                    sr.WriteLine(resList[i].InstIndex);
                    sr.Flush();

                    //关键词
                    sr.WriteLine(resList[i].keyword);
                    sr.Flush();

                    //文件内容
                    ns.Read(response);
                    client.Client.SendFile(resList[i].Path);

                    ns.Read(response);
                }

                sr.Close();
                client.Close();
                System.Windows.MessageBox.Show("上传完毕", "提示");
            }
            catch(Exception exc)
            {
                System.Windows.MessageBox.Show(exc.ToString(), "警告");
            }
        }
    }

    public class ResFile
    {
        public string Path { get; set; }
        public InstLevel Level { get; set; }
        public int InstIndex { get; set; }
        public string keyword { get; set; }
        public string InstName
        {
            set { }
            get
            {
                return MainWindow.instList[Convert.ToInt32(Level)][InstIndex];
            }
        }
    }

}
