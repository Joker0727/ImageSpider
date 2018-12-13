using ImageSpiderApi.EF;
using ImageSpiderApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace ImageSpiderApi.Controllers
{
    /// <summary>
    /// ImageSpiderApi
    /// </summary>
    [RoutePrefix("imagespiderapi")]
    public class CatalogController : ApiController
    {
        public ImageSpiderEntities ise = new ImageSpiderEntities();

        /// <summary>
        /// 获取目录
        /// </summary>
        /// <param name="page">第几页</param>
        /// <param name="count">每页的数量</param>
        /// <returns></returns>
        [HttpPost, Route("getcatalog"), ResponseType(typeof(GetCatalogDto))]
        public async Task<IHttpActionResult> GetCatalogAsync(int page = 1, int count = 6)
        {
            if (page < 1)
                return BadRequest("页数不能小于1！");
            List<GetCatalogDto> catalogList = new List<GetCatalogDto>();
            try
            {
                catalogList = await ise.CatalogTables
                    .OrderByDescending(o => o.Id)
                    .Skip(count * (page - 1))
                    .Take(count)
                    .Select(s => new GetCatalogDto
                    {
                        Id = s.Id,
                        Describe = s.Describe,
                        CatalogUrl = s.CatalogUrl,
                        TotalImages = s.TotalImages,
                        WebSiteUrl = s.WebSiteUrl,
                        IsDownLoad = s.IsDownLoad,
                        CoverUrl = ise.ImageTables.Where(w => w.CatalogId == s.Id).Select(e => new
                        {
                            e.NewUrl
                        }).FirstOrDefault().NewUrl.ToString()
                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                return BadRequest("查询错误：" + ex.ToString());
            }
            return Ok(catalogList);
        }
    }
}