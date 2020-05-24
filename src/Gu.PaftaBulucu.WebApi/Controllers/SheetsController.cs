using Gu.PaftaBulucu.Business.Dtos;
using Gu.PaftaBulucu.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gu.PaftaBulucu.WebApi.Controllers
{
    [Route("[controller]")]
    public class SheetsController : Controller
    {
        private readonly ISheetService _sheetService;
        public SheetsController(ISheetService sheetService)
        {
            _sheetService = sheetService;
        }

        [HttpGet("{scale}/{name}")]
        public SheetDto Get(int scale, string name)
        {
            return _sheetService.GetSheetByName(name, scale);
        }

        [HttpGet("{scale}/{lat}/{lon}")]
        public IActionResult Get(int scale, double lat, double lon)
        {
            return new OkObjectResult(
                _sheetService.GetSheetsByCoordinate(lat, lon, scale)    
            );
        }


    }
}
