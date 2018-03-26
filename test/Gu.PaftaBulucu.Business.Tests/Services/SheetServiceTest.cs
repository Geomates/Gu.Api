using System.Linq;
using Amazon;
using Amazon.S3;
using Gu.PaftaBulucu.Business.Services;
using Gu.PaftaBulucu.Data.Respositories;
using Xunit;

namespace Gu.PaftaBulucu.Business.Tests.Services
{
    public class SheetServiceTest
    {
        private readonly ISheetRepository _sheetRepository;

        public SheetServiceTest()
        {
            _sheetRepository = new SheetRepository(new AmazonS3Client(new AmazonS3Config()
                {
                    RegionEndpoint = RegionEndpoint.EUWest1
                }));
        }

        [Theory]
        [InlineData(37.0000028, 41.5000028, 100, "Mardin-N 47", 1)]
        [InlineData(37, 41.5, 100, "Mardin-N 46,Mardin-N 47,Hasec-O 47,Hasec-O 46", 4)]
        [InlineData(37, 41.5000028, 100, "Mardin-N 47,Hasec-O 47", 2)]
        [InlineData(37.0000028, 41.5, 100, "Mardin-N 46,Mardin-N 47", 2)]
        [InlineData(37.2500028, 41.7500028, 50, "Mardin-N 47-b", 1)]
        [InlineData(37.25, 41.75, 50, "Mardin-N 47-a,Mardin-N 47-b,Mardin-N 47-c,Mardin-N 47-d", 4)]
        [InlineData(37.25, 41.7500028, 50, "Mardin-N 47-b,Mardin-N 47-c", 2)]
        [InlineData(37.2500028, 41.75, 50, "Mardin-N 47-a,Mardin-N 47-b", 2)]
        [InlineData(37.3000028, 41.8000028, 10, "Mardin-N 47-b-17", 1)]
        [InlineData(37.30, 41.80, 10, "Mardin-N 47-b-16,Mardin-N 47-b-17,Mardin-N 47-b-22,Mardin-N 47-b-21", 4)]
        [InlineData(37.30, 41.8000028, 10, "Mardin-N 47-b-17,Mardin-N 47-b-22", 2)]
        [InlineData(37.3000028, 41.80, 10, "Mardin-N 47-b-16,Mardin-N 47-b-17", 2)]
        public void GetSheetsByCoordinate(double lat, double lon, int scale, string sheets, int count)
        {
            var sheetService = new SheetService(_sheetRepository);
            var result = sheetService.GetSheetsByCoordinate(lat, lon, scale).ToArray();

            Assert.Equal(count, result.Length);

            var sheetNames = sheets.Split(',');

            for (int i = 0; i < count; i++)
            {
                Assert.Equal(sheetNames[i], result[i].Name);
            }
        }

        [Theory]
        [InlineData("Mardin-N 47", 100, 37, 41.5)]
        [InlineData("Mardin-N 47-b", 50, 37.25, 41.75)]
        [InlineData("Mardin-N 47-b1", 25, 37.375, 41.75)]
        [InlineData("Mardin-N 47-b-17", 10, 37.3, 41.8)]
        [InlineData("Mardin-N 47-b-10", 10, 37.40, 41.95)]
        [InlineData("Mardin-N 47-b-17-b", 5, 37.325, 41.825)]
        [InlineData("Mardin-N 47-b-17-b-1", 2, 37.3375, 41.825)]
        [InlineData("Mardin-N 47-b-17-b-1-c", 1, 37.3375, 41.83125)]
        public void GetSheetByName(string sheetName, int scale, double lat, double lon)
        {
            var sheetService = new SheetService(_sheetRepository);
            var result = sheetService.GetSheetByName(sheetName, scale);

            Assert.Equal(lat, result.Lat);
            Assert.Equal(lon, result.Lon);
        }
    }
}
