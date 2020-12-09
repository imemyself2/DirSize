using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.WinForms;
namespace DirSize
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Point lastPoint;
        private void Form1_Load(object sender, EventArgs e)
        {


            //Program p = new Program();
            string filePath = "";
            try
            {
                filePath = Environment.GetCommandLineArgs()[1];
            }
            catch (Exception)
            {
                filePath = "D:\\Work";
            }
            getFileSize(filePath);
            getDirs(filePath);
            Dictionary<string, decimal> finalAns = calcSize(filePath);
            var sortedSize = finalAns.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            foreach (KeyValuePair<string, decimal> kvp in sortedSize)
            {
                if ((kvp.Value / 1024 / 1024) < 1)
                {
                    // finalAns[kvp.Key] = kvp.Value / 1024;
                    Console.WriteLine("Dir = {0}, Size = {1}", kvp.Key, (kvp.Value / 1024).ToString("#.##") + " KB");
                }
                else if ((kvp.Value / 1024 / 1024 / 1024) < 1)
                {
                    // finalAns[kvp.Key] = kvp.Value/1024;
                    Console.WriteLine("Dir = {0}, Size = {1}", kvp.Key, (kvp.Value / 1024 / 1024).ToString("#.##") + " MB");
                }
                else
                {
                    Console.WriteLine("Dir = {0}, Size = {1}", kvp.Key, (kvp.Value / 1024 / 1024 / 1024).ToString("#.##") + " GB");
                }

            }
            DataChart.Series = new SeriesCollection { };
            DataChart.LegendLocation = LegendLocation.None;
            DataChart.HoverPushOut = 0;

            var tooltip = (LiveCharts.Wpf.DefaultTooltip)DataChart.DataTooltip;
            tooltip.SelectionMode = LiveCharts.TooltipSelectionMode.OnlySender;

            ListView DataList = new ListView();
            DataList.Name = "Distribution";
            DataList.Location = new System.Drawing.Point(28, 57);
            DataList.Size = new System.Drawing.Size(377, 316);
            DataList.Scrollable = true;
            DataList.View = View.Details;
            

            // Add columns
            ColumnHeader header = new ColumnHeader();
            header.Text = "Directory";
            header.Name = "Directory";
            DataList.Columns.Add(header);

            ColumnHeader size = new ColumnHeader();
            size.Text = "Size";
            size.Name = "Size";
            DataList.Columns.Add(size);

            Controls.Add(DataList);

            foreach (KeyValuePair<string, decimal> kvp in sortedSize)
            {
                if(kvp.Key != filePath)
                {
                    //DataChart.Series["dirSize"].Points.AddXY(kvp.Key, Decimal.ToDouble(kvp.Value));
                    string sizeType = "";
                    if ((kvp.Value / 1024 / 1024) < 1)
                    {
                        // finalAns[kvp.Key] = kvp.Value / 1024;
                        sizeType = (kvp.Value / 1024).ToString("#.##") + " KB";
                    }
                    else if ((kvp.Value / 1024 / 1024 / 1024) < 1)
                    {
                        // finalAns[kvp.Key] = kvp.Value/1024;
                        sizeType = (kvp.Value / 1024 / 1024).ToString("#.##") + " MB";
                    }
                    else
                    {
                        sizeType = (kvp.Value / 1024 / 1024 / 1024).ToString("#.##") + " GB";
                    }
                    if(sizeType == " KB")
                    {
                        sizeType = "0 KB";
                    }
                    Func<ChartPoint, string> labelPoint = chartPoint => (chartPoint.Y / 1024 / 1024) < 1 ? string.Format("({0:#.##} KB)", (chartPoint.Y / 1024)) :
                                                                        (chartPoint.Y / 1024 / 1024 / 1024) < 1 ? string.Format("({0:#.##} MB)", (chartPoint.Y / 1024 / 1024)) :
                                                                        string.Format("({0:#.##} GB)", (chartPoint.Y / 1024 / 1024 / 1024));
                    string name = kvp.Key;
                    if (name.Contains(filePath))
                    {
                        name = name.Substring(filePath.Length+1);
                    }
                    string[] DataRow = { name, sizeType };
                    var DataListItem = new ListViewItem(DataRow);
                    DataList.Items.Add(DataListItem);
                    DataList.Columns[0].Width = -1;
                    DataList.Columns[1].Width = -1;

                    DataChart.Series.Add(new PieSeries {
                        Title = kvp.Key,
                        Values = new ChartValues<decimal> {kvp.Value},
                        PushOut = 0,
                        DataLabels = false,
                        StrokeThickness = 0,
                        LabelPoint = labelPoint,
                    });
                }     
            }
        }
        static Dictionary<string, decimal> calcSize(String dir)
        {
            // Recurse into folders and calculate sizes
            string[] MainDirs = getDirs(dir);
            Dictionary<string, decimal> finalDirSize = new Dictionary<string, decimal>();
            if (MainDirs.Length == 0)
            {
                // Folder has no subDirs, calc size
                Dictionary<string, decimal> totalDirSize = getFileSize(dir);
                Dictionary<string, decimal> innerDirSize = new Dictionary<string, decimal>();
                decimal sum = 0;
                foreach (KeyValuePair<string, decimal> kvp in totalDirSize)
                {
                    if (kvp.Key != dir)
                        sum += kvp.Value;
                }
                finalDirSize.Add(dir, sum);
                return finalDirSize;

            }
            else
            {
                Dictionary<string, decimal> fileDict = getFileSize(dir);
                decimal fileSum = 0;
                foreach (KeyValuePair<string, decimal> kvp in fileDict)
                {
                    if (kvp.Key != dir)
                        fileSum += kvp.Value;
                }
                decimal dirSum = 0;
                foreach (string name in MainDirs)
                {
                    // Call CalcSize to find inner subDirs
                    Dictionary<string, decimal> ans = calcSize(name);
                    dirSum += ans[name];
                    finalDirSize.Add(name, ans[name]);
                }
                finalDirSize.Add(dir, dirSum + fileSum);
                finalDirSize.Add("Files", fileSum);
                return finalDirSize;
            }


        }
        static string[] getDirs(String dir)
        {
            string[] dirs = Directory.GetDirectories(dir);
            return dirs;
        }

        static Dictionary<string, decimal> getFileSize(String dir)
        {
            Dictionary<string, decimal> files = new Dictionary<string, decimal>();
            string[] filesArr = Directory.GetFiles(dir);
            decimal totalSize = 0;
            foreach (string name in filesArr)
            {
                FileInfo info = new FileInfo(name);
                decimal size = info.Length;
                totalSize += size;
                files.Add(name, size);
            }
            files.Add(dir, totalSize);
            return files;
        }

        private void DataChart_Click(object sender, EventArgs e)
        {

        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastPoint.X;
                this.Top += e.Y - lastPoint.Y;
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint = new Point(e.X, e.Y);
        }

        private void DataChart_ChildChanged(object sender, System.Windows.Forms.Integration.ChildChangedEventArgs e)
        {

        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
