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
                        c.IncludeXmlComments(GetXmlCommentsPath());//��ȡImageSpiderApi.XML
                        c.DescribeAllEnumsAsStrings();
                        c.OperationFilter<HttpHeaderFilter>();  // Ȩ�޹���
                        c.OperationFilter<UploadFilter>();
                    })
                .EnableSwaggerUi(c =>
                    {
                        c.DocumentTitle("ϵͳ�����ӿ�");
                        // ʹ������
                        c.InjectJavaScript(thisAssembly, "ImageSpiderApi.scripts.Swagger.Swagger_lang.js");
                    });
        }
        /// <summary>
        /// ��ȡImageSpiderApi.XML
        /// </summary>
        /// <returns></returns>
        private static string GetXmlCommentsPath()
        {
            return string.Format("{0}/bin/ImageSpiderApi.XML", System.AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}
