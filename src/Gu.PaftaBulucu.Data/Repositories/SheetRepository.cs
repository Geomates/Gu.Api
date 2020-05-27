using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Amazon.S3;
using Amazon.S3.Model;
using Gu.PaftaBulucu.Data.Models;
using Newtonsoft.Json;

namespace Gu.PaftaBulucu.Data.Repositories
{
    public class SheetRepository : ISheetRepository
    {
        private readonly IAmazonS3 _amazonS3;

        private string _s3BucketName { get; set; }

        public SheetRepository(IAmazonS3 amazonS3)
        {
            _amazonS3 = amazonS3;
        }

        public ISheetRepository SetS3BucketName(string s3BucketName)
        {
            _s3BucketName = s3BucketName;
            return this;
        }

        public Sheet FindByName(string name) => FindInJsonByName(name);

        public Sheet FindByCoordinatesAndScale(int lat, int lon, int scale)
        {
            int range;
            int offset;

            switch (scale)
            {
                case 100:
                    range = 36000000;
                    offset = range / 2;
                    return FindInJsonByCoordinatesAndOffset(lat, lon, offset);
                case 50:
                    range = 18000000;
                    offset = range / 2;
                    lon = lon % 36000000 % range;
                    lat = lat % 36000000 % range;
                    return FindIn4Sheets(lat, lon, offset, false);
                case 25:
                    range = 9000000;
                    offset = range / 2;
                    lon = lon % 36000000 % range;
                    lat = lat % 36000000 % range;
                    return FindIn4Sheets(lat, lon, offset);
                case 10:
                    range = 9000000;
                    offset = range / 5;
                    lon = lon % 36000000 % range;
                    lat = lat % 36000000 % range;
                    return FindIn25Sheets(lat, lon, offset);
                case 5:
                    range = 1800000;
                    offset = range / 2;
                    lon = lon % 36000000 % range;
                    lat = lat % 36000000 % range;
                    return FindIn4Sheets(lat, lon, offset, false);
                case 2:
                    range = 900000;
                    offset = range / 2;
                    lon = lon % 36000000 % range;
                    lat = lat % 36000000 % range;
                    return FindIn4Sheets(lat, lon, offset);
                case 1:
                    range = 450000;
                    offset = range / 2;
                    lon = lon % 36000000 % range;
                    lat = lat % 36000000 % range;
                    return FindIn4Sheets(lat, lon, offset, false);
            }

            throw new ArgumentException("Scale is not known!", nameof(scale));
        }
        private Sheet FindIn4Sheets(int lat, int lon, int offset, bool isNumber = true)
        {
            if (lon < offset && lat < offset)
                return new Sheet
                {
                    Lat = 0,
                    Lon = 0,
                    Name = isNumber ? "4" : "d"
                };
            if (lon < offset && lat > offset)
                return new Sheet
                {
                    Lat = offset,
                    Lon = 0,
                    Name = isNumber ? "1" : "a"
                };
            if (lon > offset && lat > offset)
                return new Sheet
                {
                    Lat = offset,
                    Lon = offset,
                    Name = isNumber ? "2" : "b"
                };
            return new Sheet
            {
                Lat = 0,
                Lon = offset,
                Name = isNumber ? "3" : "c"
            };
        }

        private Sheet FindIn25Sheets(int lat, int lon, int offset)
        {
            int x = lon / offset;
            int y = lat / offset;

            return new Sheet
            {
                Lat = y * offset,
                Lon = x * offset,
                Name = ((x+1)+(4-y)*5).ToString().PadLeft(2, '0')
            };
        }

        private Sheet FindInJsonByCoordinatesAndOffset(int lat, int lon, int offset)
        {
            var getResponse = _amazonS3.GetObjectAsync(new GetObjectRequest
            {
                BucketName = _s3BucketName,
                Key = "sheets.json"
            }).Result;

            List<Sheet> sheets;

            var serializer = new JsonSerializer();

            using (var streamReader = new StreamReader(getResponse.ResponseStream))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                sheets = serializer.Deserialize<List<Sheet>>(jsonTextReader);
            }

            return sheets.FirstOrDefault(s => lat > s.Lat && lat < s.Lat + offset && lon > s.Lon && lon < s.Lon + offset);
        }

        private Sheet FindInJsonByName(string name)
        {
            var getResponse = _amazonS3.GetObjectAsync(new GetObjectRequest
            {
                BucketName = _s3BucketName,
                Key = "sheets.json"
            }).Result;

            List<Sheet> sheets;

            var serializer = new JsonSerializer();

            using (var streamReader = new StreamReader(getResponse.ResponseStream))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                sheets = serializer.Deserialize<List<Sheet>>(jsonTextReader);
            }

            return sheets.FirstOrDefault(s => s.Name == name);
        }
    }
}
