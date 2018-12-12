using ImageSpider.Entity;
using ImageSpider.Spider;
using System;
using System.Threading;
using System.Windows.Forms;

namespace ImageSpider
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;
            this.comboBox1.SelectedIndex = 1;
        }
        private void textBox1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            path.ShowDialog();
            this.textBox1.Text = path.SelectedPath;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string downLoadPath = this.textBox1.Text;
            int option = this.comboBox1.SelectedIndex;
            if (string.IsNullOrEmpty(downLoadPath))
            {
                MessageBox.Show("请选择图片下载目录！", "ImageSpider");
                return;
            }

            Thread thread = new Thread(StartWord);
            thread.IsBackground = true;
            thread.Start(new PathOptionModel { DownLoadPath = downLoadPath, Option = option });
        }
        /// <summary>
        /// 开始下载任务
        /// </summary>
        public void StartWord(object obj)
        {
            PathOptionModel pathOption = obj as PathOptionModel;
            Mmjpg mmjpg = new Mmjpg(pathOption.DownLoadPath, this.label4);
            mmjpg.StartDownLoad(pathOption.Option);
        }
    }
}
