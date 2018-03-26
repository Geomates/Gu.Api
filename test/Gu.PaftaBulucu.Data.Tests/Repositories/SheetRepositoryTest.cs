using Amazon;
using Amazon.S3;
using Gu.PaftaBulucu.Data.Respositories;
using Xunit;

namespace Gu.PaftaBulucu.Data.Tests.Repositories
{
    public class SheetRepositoryTest
    {
        [Theory]
        [InlineData(1494000100, 1062000100, "İstanbul-E 23")]
        [InlineData(1296004000, 918000300, "Kalimnos-P 15")]
        public void FindSheet_100Scale(int lat, int lon, string sheetName)
        {

            var sheetRepository = new SheetRepository(new AmazonS3Client(new AmazonS3Config()
            {
                RegionEndpoint = RegionEndpoint.EUWest1
            })).SetS3BucketName("pafta.bulucu");

            var sheet = sheetRepository.FindByCoordinatesAndScale(lat, lon, 100);

            Assert.Equal(sheetName, sheet.Name);
        }

        [Theory]
        [InlineData("İstanbul-E 23", 1494000000, 1062000000)]
        [InlineData("Kalimnos-P 15", 1296000000, 918000000)]
        public void FindSheet_ByName(string sheetName, int lat, int lon)
        {

            var sheetRepository = new SheetRepository(new AmazonS3Client(new AmazonS3Config()
            {
                RegionEndpoint = RegionEndpoint.EUWest1
            })).SetS3BucketName("pafta.bulucu");

            var sheet = sheetRepository.FindByName(sheetName);

            Assert.Equal(sheetName, sheet.Name);
            Assert.Equal(lat, sheet.Lat);
            Assert.Equal(lon, sheet.Lon);
        }

        /// <summary>
        /// 1:50.000 Sheets
        /// Offset : 9000000
        /// </summary>
        [Theory]
        [InlineData(1503000100, 1062000100, "a")]
        [InlineData(1503000100, 1071000100, "b")]
        [InlineData(1494000100, 1071000100, "c")]
        [InlineData(1494000100, 1062000100, "d")]
        public void FindSheet_FindIn4Sheets(int lat, int lon, string sheetName)
        {

            var sheetRepository = new SheetRepository(null);

            var sheet = sheetRepository.FindByCoordinatesAndScale(lat, lon, 50);

            Assert.Equal(sheetName, sheet.Name);
        }

        /// <summary>
        /// 1:10.000 Sheets
        /// Offset : 1800000
        /// </summary>
        [Fact]
        public void FindSheet_FindIn25Sheets()
        {
            var lat = 1494000000;
            var lon = 1062000000;
            var offset = 1800000;

            var sheetRepository = new SheetRepository(null);
            int index = 1;

            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    var sheet = sheetRepository.FindByCoordinatesAndScale(lat + (4 - row) * offset + 100, lon+ col * offset + 100, 10);
                    Assert.Equal(index++.ToString().PadLeft(2, '0'), sheet.Name);
                }
            }
            
        }
    }
}
