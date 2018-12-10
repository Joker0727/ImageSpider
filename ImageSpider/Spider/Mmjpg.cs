using HtmlAgilityPack;
using ImageSpider.EF;
using MyTool;
using MyTools;
using OpenQA.Selenium;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace ImageSpider.Spider
{
    public class Mmjpg
    {
        private string mainUrl = "http://www.mmjpg.com/";
        private string AllURL = "https://www.mzitu.com/all/";
        public ImageSpiderEntities ise = null;
        public HttpHelper hh = null;
        private CookieContainer cookie = new CookieContainer();
        public MyUtils myUtils = null;
        private int TotalPage = 0;
        public string downLoadPath = string.Empty;
        public SeleniumHelper sel = null;


        public Mmjpg(string downLoadPath)
        {
            ise = new ImageSpiderEntities();
            hh = new HttpHelper();
            myUtils = new MyUtils();
            this.downLoadPath = downLoadPath;
        }

        public void StartDownLoad(int option)
        {
            GetTotalPage();
            switch (option)
            {
                case 0:
                    {
                        DownLoadCatalogUrl();
                        MessageBox.Show("目录连接保存完成！", "ImageSpider");
                        break;
                    }
                case 1:
                    {
                        DownLoadImageUrl();
                        MessageBox.Show("图片链接保存完成！", "ImageSpider");
                        break;
                    }
                case 2:
                    {
                        DownLoadImage();
                        break;
                    }
                default:
                    break;
            }
        }
        /// <summary>
        /// 获取总页数
        /// </summary>
        public void GetTotalPage()
        {
            ArrayList arrayList = hh.GetHtmlData(mainUrl, cookie);

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(arrayList[1].ToString());
            HtmlNode totalPageNode = doc.DocumentNode.SelectSingleNode("//em[@class='info']");
            string totalText = totalPageNode?.InnerText;
            TotalPage = myUtils.GetNumber(totalText);
        }
        /// <summary>
        /// 下载目录
        /// </summary>
        public void DownLoadCatalogUrl()
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            ArrayList arrayList = null;
            string pageUrl = mainUrl;

            for (int i = 1; i < TotalPage + 1; i++)
            {
                try
                {
                    if (i == 1)
                        pageUrl = mainUrl;
                    else
                        pageUrl = mainUrl + $"home/{i}";

                    arrayList = hh.GetHtmlData(pageUrl, cookie);
                    doc.LoadHtml(arrayList[1].ToString());
                    HtmlNodeCollection aNodes = doc.DocumentNode.SelectNodes("//div[@class='main']/div[@class='pic']/ul/li/span[@class='title']/a");
                    if (aNodes != null)
                    {
                        ArrayList arrList = null;
                        HtmlAgilityPack.HtmlDocument hdc = null;
                        string catalogUrl = string.Empty, title = string.Empty;
                        foreach (var aNode in aNodes)
                        {
                            try
                            {
                                catalogUrl = aNode.GetAttributeValue("href", "");
                                title = aNode.InnerText;
                                if (!string.IsNullOrEmpty(catalogUrl))
                                {
                                    CatalogTable catalogInfo = new CatalogTable();
                                    catalogInfo.WebSiteUrl = mainUrl;
                                    catalogInfo.CatalogUrl = catalogUrl;
                                    catalogInfo.Describe = title;
                                    arrList = hh.GetHtmlData(catalogUrl, cookie);
                                    hdc = new HtmlAgilityPack.HtmlDocument();
                                    hdc.LoadHtml(arrList[1].ToString());
                                    HtmlNodeCollection pages = hdc.DocumentNode.SelectNodes("//div[@id='page']/a");
                                    int total = int.Parse(pages[pages.Count - 2].InnerText);
                                    catalogInfo.TotalImages = total;
                                    catalogInfo.IsDownLoad = false;
                                    ise.CatalogTables.Add(catalogInfo);
                                    ise.SaveChanges();
                                }
                            }
                            catch (Exception ex)
                            {
                                myUtils.WriteLog("保存目录连接出错 " + ex);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    myUtils.WriteLog("查询目录节点出错 " + e);
                }
            }
        }
        /// <summary>
        /// 下载图片链接
        /// </summary>
        public void DownLoadImageUrl()
        {
            sel = new SeleniumHelper(1);
            sel.driver.Navigate().GoToUrl(mainUrl);
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            ArrayList arrayList = null;
            string imageUrl = mainUrl;
            var CatalogList = ise.CatalogTables.Where(w => w.IsDownLoad == false).Select(s => new
            {
                s.Id,
                s.Describe,
                s.CatalogUrl,
                s.TotalImages
            }).ToList();

            foreach (var catalog in CatalogList)
            {
                string catalogUrl = catalog.CatalogUrl;
                int totalImages = (int)catalog.TotalImages;
                int fkId = catalog.Id;


                string pageUrl = string.Empty, imgUrl = string.Empty, alt = string.Empty;
                for (int i = 1; i < totalImages + 1; i++)
                {
                    try
                    {
                        if (i == 1)
                            pageUrl = catalogUrl;
                        else
                            pageUrl = catalogUrl + $"/{i}";
                        arrayList = hh.GetHtmlData(pageUrl, cookie);
                        doc.LoadHtml(arrayList[1].ToString());
                        HtmlNode imgNode = doc.DocumentNode.SelectSingleNode("//div[@id='content']/a/img");
                        if (imgNode != null)
                        {
                            imgUrl = imgNode.GetAttributeValue("src", "");
                            alt = imgNode.GetAttributeValue("alt", "");
                        }
                        ImageTable imageInfo = new ImageTable();
                        imageInfo.Guid = Guid.NewGuid().ToString("N");
                        imageInfo.OriginalUrl = imgUrl;
                        imageInfo.NewUrl = imgUrl;
                        imageInfo.WebSiteUrl = mainUrl;
                        imageInfo.Alt = alt;
                        imageInfo.CatalogId = fkId;
                        imageInfo.IsDownLoad = false;
                        imageInfo.Width = 0;
                        imageInfo.Height = 0;
                        ise.ImageTables.Add(imageInfo);
                        ise.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        myUtils.WriteLog("保存图片链接出错" + e);
                    }
                }
                try
                {
                    var catalogObj = ise.CatalogTables.Where(w => w.Id == fkId).FirstOrDefault();
                    catalogObj.IsDownLoad = true;
                    ise.SaveChanges();
                }
                catch (Exception e)
                {
                    myUtils.WriteLog("重置目录下载状态出错 " + e);
                }
            }
        }
        /// <summary>
        /// 下载图片资源
        /// </summary>
        public void DownLoadImage()
        {
            var imgUrlList = ise.ImageTables.Where(w => w.IsDownLoad == false).Select(s => new
            {
                s.Id,
                s.Guid,
                s.OriginalUrl,
                s.CatalogId
            }).ToList();
            ArrayList arrayList = hh.GetHtmlData(mainUrl, cookie);
            string imageFullPath = string.Empty;
            int index = 0;
            foreach (var imgObj in imgUrlList)
            {
                index++;
                   CatalogTable catalogTable = ise.CatalogTables.Where(w => w.Id == imgObj.CatalogId).FirstOrDefault();
                string referer = string.Empty;
                if (index == 1)
                    referer = catalogTable.CatalogUrl;
                else
                    referer = catalogTable.CatalogUrl+@"/"+ index;
                try
                {
                    imageFullPath = downLoadPath + @"\" + imgObj.Guid + ".jpg";
                    byte[] imgByteArr = hh.DowloadCheckImg(imgObj.OriginalUrl, cookie, referer);
                    Image image = myUtils.GetImageByBytes(imgByteArr);
                    image.Save(imageFullPath, ImageFormat.Jpeg);

                    var imageObj = ise.ImageTables.Where(w => w.Id == imgObj.Id).FirstOrDefault();
                    imageObj.IsDownLoad = true;
                    imageObj.Width = image.Width;
                    imageObj.Height = image.Height;
                    imageObj.NewUrl = imageFullPath;
                    imageObj.DownLoadTime = DateTime.Now;
                    ise.SaveChanges();
                }
                catch (Exception ex)
                {
                    myUtils.WriteLog($"图片【{imgObj.OriginalUrl}】 下载失败！" + ex);
                }
                Thread.Sleep(1000 * 3);
            }
        }
    }
}
