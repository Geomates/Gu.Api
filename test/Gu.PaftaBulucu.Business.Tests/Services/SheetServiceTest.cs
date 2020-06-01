using Amazon.S3;
using Gu.PaftaBulucu.Business.Services;
using Gu.PaftaBulucu.Data.Repositories;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Linq;
using Xunit;

namespace Gu.PaftaBulucu.Business.Tests.Services
{
    public class SheetServiceTest
    {
        private readonly ISheetRepository _sheetRepository; 
        private readonly Mock<IConfiguration> _configuration;

        public SheetServiceTest()
        {
            _configuration = new Mock<IConfiguration>();
            _configuration.SetupGet(x => x[It.Is<string>(s => s == "BucketName")]).Returns("pafta.bulucu.dev");
            _sheetRepository = new SheetRepository(new AmazonS3Client(), _configuration.Object);
        }

        [Theory]
        [InlineData(37.0000028, 41.5000028, 100, "Mardin-N47", 1)]
        [InlineData(37, 41.5, 100, "Mardin-N46,Mardin-N47,Hasec-O47,Hasec-O46", 4)]
        [InlineData(37, 41.5000028, 100, "Mardin-N47,Hasec-O47", 2)]
        [InlineData(37.0000028, 41.5, 100, "Mardin-N46,Mardin-N47", 2)]
        [InlineData(37.2500028, 41.7500028, 50, "Mardin-N47-b", 1)]
        [InlineData(37.25, 41.75, 50, "Mardin-N47-a,Mardin-N47-b,Mardin-N47-c,Mardin-N47-d", 4)]
        [InlineData(37.25, 41.7500028, 50, "Mardin-N47-b,Mardin-N47-c", 2)]
        [InlineData(37.2500028, 41.75, 50, "Mardin-N47-a,Mardin-N47-b", 2)]
        [InlineData(37.3000028, 41.8000028, 10, "Mardin-N47-b-17", 1)]
        [InlineData(37.30, 41.80, 10, "Mardin-N47-b-16,Mardin-N47-b-17,Mardin-N47-b-22,Mardin-N47-b-21", 4)]
        [InlineData(37.30, 41.8000028, 10, "Mardin-N47-b-17,Mardin-N47-b-22", 2)]
        [InlineData(37.3000028, 41.80, 10, "Mardin-N47-b-16,Mardin-N47-b-17", 2)]
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
        [InlineData("Mardin-N47", 100, 37, 41.5)]
        [InlineData("Mardin-N47-b", 50, 37.25, 41.75)]
        [InlineData("Mardin-N47-b1", 25, 37.375, 41.75)]
        [InlineData("Mardin-N47-b-17", 10, 37.3, 41.8)]
        [InlineData("Mardin-N47-b-10", 10, 37.40, 41.95)]
        [InlineData("Mardin-N47-b-17-b", 5, 37.325, 41.825)]
        [InlineData("Mardin-N47-b-17-b-1", 2, 37.3375, 41.825)]
        [InlineData("Mardin-N47-b-17-b-1-c", 1, 37.3375, 41.83125)]
        public void GetSheetByName(string sheetName, int scale, double lat, double lon)
        {
            var sheetService = new SheetService(_sheetRepository);
            var result = sheetService.GetSheetByName(sheetName, scale);

            Assert.Equal(lat, result.Lat);
            Assert.Equal(lon, result.Lon);
        }

        [Theory]
        [InlineData("Mardin-N47", 100)]
        [InlineData("Mardin-N47-b", 50)]
        [InlineData("Mardin-N47-b1", 25)]
        [InlineData("Mardin-N47-b-17", 10)]
        [InlineData("Mardin-N47-b-10", 10)]
        [InlineData("Mardin-N47-b-17-b", 5)]
        [InlineData("Mardin-N47-b-17-b-1", 2)]
        [InlineData("Mardin-N47-b-17-b-1-c", 1)]
        public void GetSheetParts(string sheetName, int scale)
        {
            var sheetService = new SheetService(_sheetRepository);
            var result = sheetService.GetSheetParts(sheetName);
            var lastScale = result.Keys.ToList().Last();

            Assert.Equal(scale, lastScale);
        }
    }
}
