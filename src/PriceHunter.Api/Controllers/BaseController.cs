using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PriceHunter.Common.Application;

namespace PriceHunter.Api.Controllers
{
    /// <summary>
    /// Api base controller
    /// </summary>
    [ApiController]
    //[Produces(AppConstants.JsonContentType)]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class BaseController : Controller
    {
        /// <summary>
        /// Auto Mapper
        /// </summary>
        public IMapper Mapper { get; set; }

        /// <summary>
        /// Base Controller
        /// </summary>
        public BaseController()
        {
            var services = ApplicationContext.Context.HttpContext.RequestServices;
            Mapper = (IMapper)services.GetService(typeof(IMapper));
        }
    }
}
