using System.Web.Http;
using WebActivatorEx;
using ImageSpiderApi;
using Swashbuckle.Application;
using ImageSpiderApi.Filter;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace ImageSpiderApi
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", "ImageSpiderApi");
                        c.IncludeXmlComments(GetXmlCommentsPath());//读取ImageSpiderApi.XML
                        c.DescribeAllEnumsAsStrings();
                        c.OperationFilter<HttpHeaderFilter>();  // 权限过滤
                        c.OperationFilter<UploadFilter>();
                    })
                .EnableSwaggerUi(c =>
                    {
                        c.DocumentTitle("系统开发接口");
                        // 使用中文
                        c.InjectJavaScript(thisAssembly, "ImageSpiderApi.scripts.Swagger.Swagger_lang.js");
                    });
        }
        /// <summary>
        /// 读取ImageSpiderApi.XML
        /// </summary>
        /// <returns></returns>
        private static string GetXmlCommentsPath()
        {
            return string.Format("{0}/bin/ImageSpiderApi.XML", System.AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}
