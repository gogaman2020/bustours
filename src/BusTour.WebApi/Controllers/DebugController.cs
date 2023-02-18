using BusTour.AppServices.FileConvertingService;
using Infrastructure.Common.DI;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BusTour.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [InjectAsSingleton]
    public class DebugController : BusTourControllerBase
    {
        private readonly IFileConvertingService _fileConvertingService;

        public DebugController(
            IFileConvertingService fileConvertingService
        )
        {
            _fileConvertingService = fileConvertingService;
        }

        //[HttpGet("DebugConvertToPDF")]
        //public ActionResult DebugConvertToPDF()
        //{
        //    try
        //    {
        //        byte[] pdf = _fileConvertingService.ConvertToPDF();
        //        return new JsonResult(pdf);
        //    }
        //    catch (Exception e)
        //    {
        //        return new JsonResult(e);
        //    }
        //}
    }
}
