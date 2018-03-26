using System.Collections.Generic;
using Gu.PaftaBulucu.Data.Models;
using Sheet = Gu.PaftaBulucu.Business.Models.Sheet;

namespace Gu.PaftaBulucu.Business.Services
{
    public interface ISheetService
    {
        Sheet GetSheetByName(string name, int scale);
        IEnumerable<Sheet> GetSheetsByCoordinate(double latitude, double longitude, int scale);
    }
}