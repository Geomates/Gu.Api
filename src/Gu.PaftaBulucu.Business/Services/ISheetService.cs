using System.Collections.Generic;
using Gu.PaftaBulucu.Business.Dtos;

namespace Gu.PaftaBulucu.Business.Services
{
    public interface ISheetService
    {
        SheetDto GetSheetByName(string name, int scale);
        IEnumerable<SheetDto> GetSheetsByCoordinate(double latitude, double longitude, int scale);
        Dictionary<int,string> GetSheetParts(string sheetName);
    }
}