using MyTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MyTool
{
    public class MyUtils
    {
        /// <summary>
        /// 过滤字符串
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string RegexFilterString(string text)
        {
            string resultText = string.Empty;
            try
            {
                text = Regex.Replace(text, "[&nbsp;]", "");//去 &nbsp;
                text = Regex.Replace(text, "[\r\n\t]", "");//去 \r\n\t
                text = Regex.Replace(text, "\\s{2,}", "");//去空格
                resultText = text;
            }
            catch (Exception)
            {
                throw;
            }
            return resultText;
        }
        /// <summary>
        /// 删除非法字符
        /// </summary>
        /// <param name="fileName"></param>
        public string FilterPath(string filePath)
        {
            try
            {
                filePath = Regex.Replace(filePath, "[&nbsp;]", "");//去 &nbsp;
                filePath = Regex.Replace(filePath, "[|「」，<>：*？\"/]", "");//去 \r\n\t
                filePath = Regex.Replace(filePath, "\\s{2,}", "");//去空格
                filePath = filePath.Replace("/", "");
                filePath = filePath.Replace("？", "");
                filePath = InvalidFileName(filePath);
                filePath = InvalidPath(filePath);
            }
            catch (Exception ex)
            {
                WriteLog(ex);
            }
            return filePath;
        }
        /// <summary>
        /// 去除掉文名中的非法字符
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="repStr"></param>
        /// <returns></returns>
        public string InvalidFileName(string fileName = "", string repStr = "_")
        {
            // 例如： fileName = "文件/名称"
            foreach (char invalidChar in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(invalidChar.ToString(), repStr);
            }
            //结果：文件_名称
            return fileName;
        }
        /// <summary>
        /// 去掉路径中的非法字符
        /// </summary>
        /// <param name="path"></param>
        /// <param name="repStr"></param>
        /// <returns></returns>
        public string InvalidPath(string path, string repStr = "_")
        {
            //剔除路径字符串中非法的字符
            //例如 path = "路径\ds"
            foreach (char invalidChar in Path.GetInvalidPathChars())
            {
                path = path.Replace(invalidChar.ToString(), repStr);
            }

            //结果：路径_ds
            return path;
        }
        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="oldStr">原来的字符串</param>
        /// <param name="splitKey">分割字符</param>
        /// <returns></returns>
        public string[] SplitByStr(string oldStr, string splitKey)
        {
            return Regex.Split(oldStr, splitKey, RegexOptions.IgnoreCase);
        }
        /// <summary>
        /// 判断是否是数字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsNumeric(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
        }

        /// <summary>
        /// 更新ListBox
        /// </summary>
        /// <param name="listBox"></param>
        /// <param name="item"></param>
        public void UpdateListBox(ListBox listBox, string item)
        {
            if (listBox.InvokeRequired)
            {
                // 当一个控件的InvokeRequired属性值为真时，说明有一个创建它以外的线程想访问它
                Action<string> actionDelegate = (x) =>
                {
                    listBox.Items.Add(item);
                    listBox.TopIndex = listBox.Items.Count - 1;
                };
                // 或者
                // Action<string> actionDelegate = delegate(string txt) { this.label2.Text = txt; };
                listBox.Invoke(actionDelegate, item);
            }
            else
            {
                listBox.Items.Add(item);
                listBox.TopIndex = listBox.Items.Count - 1;
            }
        }
        /// <summary>
        /// 更新更新面板数据
        /// </summary>
        /// <param name="label"></param>
        /// <param name="count"></param>
        public void UpdateLabel(Label label, int count)
        {
            if (label.InvokeRequired)
            {
                // 当一个控件的InvokeRequired属性值为真时，说明有一个创建它以外的线程想访问它
                Action<int> actionDelegate = (x) =>
                {
                    label.Text = x.ToString();
                };
                // 或者
                // Action<string> actionDelegate = delegate(string txt) { this.label2.Text = txt; };
                label.Invoke(actionDelegate, count);
            }
            else
            {
                label.Text = count.ToString();
            }
        }
        ///
        /// <summary>
        /// 下载网页图片
        /// </summary>
        /// <param name="url">下载路径</param>
        /// <param name="desPath">目标路径</param>
        /// <returns></returns>
        public void DownLoadImage(string url, string path, CookieContainer cookie)
        {
            try
            {
                HttpHelper hh = new HttpHelper();
                byte[] byteArr = hh.DowloadCheckImg(url, cookie);
                System.Drawing.Image image = GetImageByBytes(byteArr);
                image.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (Exception e)
            {
                WriteLog(e);
            }
        }
        /// <summary>
        /// 读取byte[]并转化为图片
        /// </summary>
        /// <param name="bytes">byte[]</param>
        /// <returns>Image</returns>
        public System.Drawing.Image GetImageByBytes(byte[] bytes)
        {
            System.Drawing.Image image = null;
            try
            {
                MemoryStream ms = new MemoryStream(bytes);
                image = System.Drawing.Image.FromStream(ms);
            }
            catch (Exception e)
            {
                WriteLog(e);
            }
            return image;
        }
        /// <summary>
        /// 日志打印
        /// </summary>
        /// <param name="log"></param>
        public void WriteLog(object logObj)
        {
            string log = logObj.ToString();
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "log\\";//日志文件夹
                DirectoryInfo dir = new DirectoryInfo(path);
                if (!dir.Exists)//判断文件夹是否存在
                    dir.Create();//不存在则创建

                FileInfo[] subFiles = dir.GetFiles();//获取该文件夹下的所有文件
                foreach (FileInfo f in subFiles)
                {
                    string fname = Path.GetFileNameWithoutExtension(f.FullName); //获取文件名，没有后缀
                    DateTime start = Convert.ToDateTime(fname);//文件名转换成时间
                    DateTime end = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));//获取当前日期
                    TimeSpan sp = end.Subtract(start);//计算时间差
                    if (sp.Days > 30)//大于30天删除
                        f.Delete();
                }

                string logName = DateTime.Now.ToString("yyyy-MM-dd") + ".log";//日志文件名称，按照当天的日期命名
                string fullPath = path + logName;//日志文件的完整路径
                string contents = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " -> " + log + "\r\n";//日志内容

                File.AppendAllText(fullPath, contents, Encoding.UTF8);//追加日志

            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 获取指定文件夹下的所有图片名称
        /// </summary>
        /// <param name="imgFolder"></param>
        public List<string> GetImgs(string imgFolder)
        {
            string imgtype = "*.BMP|*.JPG|*.GIF|*.PNG";
            string[] ImageType = imgtype.Split('|');
            List<string> imgList = new List<string>();
            DirectoryInfo dir = new DirectoryInfo(imgFolder);

            //获取该路径下的所有文件的列表
            FileInfo[] fileInfo = dir.GetFiles();
            //开始得到图片名称
            foreach (FileInfo subinfo in fileInfo)
            {
                //判断扩展名是否相同
                if (subinfo.Extension.ToLower() == ".jpg")
                {
                    string strname = subinfo.Name.Replace(".jpg", ""); //获取文件名称
                    imgList.Add(strname); //把文件名称保存在泛型集合中
                }
            }
            return imgList;
        }
        /// <summary>
        /// 提取字符串中的数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public int GetNumber(string str)
        {
            string result = System.Text.RegularExpressions.Regex.Replace(str, @"[^0-9]+", "");
            if (IsNumeric(result))
                return int.Parse(result);
            else
                return 0;
        }
    }
}
