using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Gu.PaftaBulucu.Business.Models;
using Gu.PaftaBulucu.Data.Respositories;

namespace Gu.PaftaBulucu.Business.Services
{
    public class SheetService : ISheetService
    {
        private readonly ISheetRepository _sheetRepository;
        private readonly Dictionary<int, int> _scaleRanges = new Dictionary<int, int>
        {
            {100, 18000000},
            {50, 9000000},
            {25, 4500000},
            {10, 1800000},
            {5, 900000},
            {2, 450000},
            {1, 225000}

        };

        public SheetService(ISheetRepository sheetRepository)
        {
            _sheetRepository = sheetRepository.SetS3BucketName("pafta.bulucu");
        }

        public IEnumerable<Sheet> GetSheetsByCoordinate(double latitude, double longitude, int scale)
        {
            int lat = (int)(latitude * 36000000);
            int lon = (int)(longitude * 36000000);

            //Coordinates are in the middle of four sheets
            if (lat % _scaleRanges[scale] == 0 && lon % _scaleRanges[scale] == 0)
            {
                return new[]
                {
                    AggregateSheets(lat + 1000, lon - 1000, scale),
                    AggregateSheets(lat + 1000, lon + 1000, scale),
                    AggregateSheets(lat - 1000, lon + 1000, scale),
                    AggregateSheets(lat - 1000, lon - 1000, scale)
                };
            }

            //Coordinates are somewhere in the middle of two sheets in a horizontal order
            if (lat % _scaleRanges[scale] > 0 && lon % _scaleRanges[scale] == 0)
            {
                return new[]
                    {
                        AggregateSheets(lat, lon - 1000, scale),
                        AggregateSheets(lat, lon + 1000, scale)
                    };
            }

            //Coordinates are somewhere in the middle of two sheets in a vertical order
            if (lat % _scaleRanges[scale] == 0 && lon % _scaleRanges[scale] > 0)
            {

                return new[]
                    {
                        AggregateSheets(lat + 1000, lon, scale),
                        AggregateSheets(lat - 1000, lon, scale)
                    };
            }

            return new[]
            {
                AggregateSheets(lat, lon, scale)
            };
        }

        public Sheet GetSheetByName(string name, int scale)
        {
            var sheetNameRegex =
                new Regex(@"^([a-zA-ZsçÇöÖşŞıİğĞüÜ]*?\-?[A-Z]\s[0-9]{2})\-?([abcd])?([1-4])?\-?([0-9]{1,2})?\-?([abcd])?\-?([1-4])?\-?([abcd])?");
            var matchedSheetParts = sheetNameRegex.Match(name);
            if (!matchedSheetParts.Success || matchedSheetParts.Groups.Count == _scaleRanges.Keys.ToList().IndexOf(scale) + 1)
            {
                throw new ArgumentException("Sheet name is not valid format", nameof(name));
            }

            var sheet100 = _sheetRepository.FindByName(matchedSheetParts.Groups[1].Value);

            if (sheet100 == null)
            {
                throw new KeyNotFoundException("Sheet not found!");
            }

            var result = new Sheet
            {
                Lat = sheet100.Lat,
                Lon = sheet100.Lon,
                Name = name,
                Scale = scale
            };

            int scaleIndex = 0;

            foreach (var scaleRange in _scaleRanges.Where(s => s.Key < 100))
            {
                if (scaleRange.Key < scale)
                {
                    break;
                }

                var part = matchedSheetParts.Groups[scaleIndex++ + 2].Value;
                int index;

                switch (scaleRange.Key)
                {
                    case 50: //1:50.000
                    case 25 when scale == 25: //1:25.000
                    case 5: //1:5.000
                    case 2: //1:2.000
                    case 1: //1:1.000
                        if (!int.TryParse(part, out index))
                        {
                            index = part[0] - 96;
                        }
                        if (index < 3)
                        {
                            result.Lat += scaleRange.Value;
                        }
                        if (index == 2 || index == 3)
                        {
                            result.Lon += scaleRange.Value;
                        }
                        break;
                    case 10: //1:10.000
                        int.TryParse(part, out index);
                        var x = index % 5 == 0 ? 4 : index % 5 - 1;
                        var y = 4 - index / 5 + (index % 5 == 0 ? 1 : 0);
                        result.Lat += y * scaleRange.Value;
                        result.Lon += x * scaleRange.Value;
                        break;
                }
            }

            result.Lat /= 36000000;
            result.Lon /= 36000000;
            return result;
        }


        private Sheet AggregateSheets(int lat, int lon, int scale)
        {
            var result = new Sheet
            {
                Scale = scale
            };

            var nameBuilder = new StringBuilder();

            foreach (var s in new[] { 100, 50, 25, 10, 5, 2, 1 })
            {
                if (s == 25 && scale != s) continue; // skip 1:25.000 unless it is expected result 
                var sheet = _sheetRepository.FindByCoordinatesAndScale(lat, lon, s);
                nameBuilder.Append(s == 100 || s == 25 ? sheet.Name : "-" + sheet.Name);
                result.Lat += sheet.Lat;
                result.Lon += sheet.Lon;
                if (s == scale) break;
            }

            result.Name = nameBuilder.ToString();
            result.Lat /= 36000000;
            result.Lon /= 36000000;
            return result;
        }
    }
}

