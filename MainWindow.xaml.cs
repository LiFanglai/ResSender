using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
        List<List<string>> instList = new List<List<string>>()
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

            List<ResFile> files = new List<ResFile>()
            {
                new ResFile(){path = "111", level = InstLevel.其他, instIndex = 0, keyword = "test"},
                new ResFile(){path = "111", level = InstLevel.其他, instIndex = 0, keyword = "test"},
                new ResFile(){path = "111", level = InstLevel.其他, instIndex = 0, keyword = "test"},
                new ResFile(){path = "111", level = InstLevel.其他, instIndex = 0, keyword = "test"},
                new ResFile(){path = "111", level = InstLevel.其他, instIndex = 0, keyword = "test"},
                new ResFile(){path = "111", level = InstLevel.其他, instIndex = 0, keyword = "test"},
            };

            fileDataGrid.ItemsSource = files;
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            output.Text = datePicker.SelectedDate.Value.ToString("yyyyMMdd");
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            var openFiledlg = new FolderBrowserDialog();
            var result = openFiledlg.ShowDialog();
            output.Text = openFiledlg.SelectedPath.ToString();
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
    }

    public class ResFile
    {
        public string path = "";
        public InstLevel level = InstLevel.其他;
        public int instIndex = 0;
        public string keyword = "";
    }

}
