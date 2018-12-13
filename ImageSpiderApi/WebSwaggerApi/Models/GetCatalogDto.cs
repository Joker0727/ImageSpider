using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageSpiderApi.Models
{
    /// <summary>
    /// 获取目录接口的实体类
    /// </summary>
    public class GetCatalogDto
    {
        /// <summary>
        /// 目录标识
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 来源网站
        /// </summary>
        public string WebSiteUrl { get; set; }
        /// <summary>
        /// 目录链接
        /// </summary>
        public string CatalogUrl { get; set; }
        /// <summary>
        /// 描述字段
        /// </summary>
        public string Describe { get; set; }
        /// <summary>
        /// 总的图片数
        /// </summary>
        public Nullable<int> TotalImages { get; set; }
        /// <summary>
        /// 是否已经下载完毕
        /// </summary>
        public Nullable<bool> IsDownLoad { get; set; }
        /// <summary>
        /// 封面图片
        /// </summary>
        public string CoverUrl { get; set; }
    }
}