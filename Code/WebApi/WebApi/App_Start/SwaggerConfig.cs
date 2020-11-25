using Swashbuckle.Application;
using System.Web.Http;

namespace WebApi.App_Start
{
    public class SwaggerConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config
            .EnableSwagger()
            .EnableSwaggerUi();
        }
    }
}
