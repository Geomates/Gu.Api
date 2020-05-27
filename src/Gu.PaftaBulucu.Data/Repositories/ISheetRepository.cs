using Gu.PaftaBulucu.Data.Models;

namespace Gu.PaftaBulucu.Data.Repositories
{
    public interface ISheetRepository
    {
        ISheetRepository SetS3BucketName(string s3BucketName);
        Sheet FindByCoordinatesAndScale(int lat, int lon, int scale);
        Sheet FindByName(string name);
    }
}