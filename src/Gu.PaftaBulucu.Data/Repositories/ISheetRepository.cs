using Gu.PaftaBulucu.Data.Models;

namespace Gu.PaftaBulucu.Data.Repositories
{
    public interface ISheetRepository
    {
        Sheet FindByCoordinatesAndScale(int lat, int lon, int scale);
        Sheet FindByNameAndScale(string name, int scale);
    }
}