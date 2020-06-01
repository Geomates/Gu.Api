using Gu.PaftaBulucu.Business.Dtos;
using Gu.PaftaBulucu.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Gu.PaftaBulucu.Business.Services
{
    public class SheetService : ISheetService
    {
        private readonly ISheetRepository _sheetRepository;
        private readonly Dictionary<int, (int lat, int lon)> _scaleRanges = new Dictionary<int, (int lat, int lon)>
        {
            {250, (36000000, 54000000)},
            {100, (18000000, 18000000)},
            {50, (9000000, 9000000)},
            {25, (4500000, 4500000)},
            {10, (1800000, 1800000)},
            {5, (900000, 900000)},
            {2, (450000, 450000)},
            {1, (225000, 225000)}
        };

        public SheetService(ISheetRepository sheetRepository)
        {
            _sheetRepository = sheetRepository;
        }

        public IEnumerable<SheetDto> GetSheetsByCoordinate(double latitude, double longitude, int scale)
        {
            int lat = (int)(latitude * 36000000);
            int lon = (int)(longitude * 36000000);

            //Coordinates are in the middle of four sheets
            if (lat % _scaleRanges[scale].lat == 0 && lon % _scaleRanges[scale].lon == 0)
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
            if (lat % _scaleRanges[scale].lat > 0 && lon % _scaleRanges[scale].lon == 0)
            {
                return new[]
                    {
                        AggregateSheets(lat, lon - 1000, scale),
                        AggregateSheets(lat, lon + 1000, scale)
                    };
            }

            //Coordinates are somewhere in the middle of two sheets in a vertical order
            if (lat % _scaleRanges[scale].lat == 0 && lon % _scaleRanges[scale].lon > 0)
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

        public SheetDto GetSheetByName(string name, int scale)
        {
            var sheetParts = GetSheetParts(name);
            if (sheetParts.Keys.Last() != scale)
            {
                throw new ArgumentException("Sheet name is not valid format", nameof(name));
            }

            var originSheetName = sheetParts[250];

            if (scale < 250)
                originSheetName += $"-{sheetParts[100]}";

            var originSheet = _sheetRepository.FindByNameAndScale(originSheetName, scale <= 100 ? 100 : 250);

            if (originSheet == null)
            {
                throw new KeyNotFoundException("Sheet not found!");
            }

            var result = new SheetDto
            {
                Lat = originSheet.Lat,
                Lon = originSheet.Lon,
                Name = name,
                Scale = scale
            };

            foreach (var sheetPart in sheetParts)
            {
                switch (sheetPart.Key)
                {
                    case 50: //1:50.000
                    case 25 when scale == 25: //1:25.000
                    case 5: //1:5.000
                    case 2: //1:2.000
                    case 1: //1:1.000
                        if (!int.TryParse(sheetPart.Value, out int index))
                        {
                            index = sheetPart.Value[0] - 96;
                        }
                        if (index < 3)
                        {
                            result.Lat += _scaleRanges[sheetPart.Key].lat;
                        }
                        if (index == 2 || index == 3)
                        {
                            result.Lon += _scaleRanges[sheetPart.Key].lon;
                        }
                        break;
                    case 10: //1:10.000
                        int.TryParse(sheetPart.Value, out int index10);
                        var x = index10 % 5 == 0 ? 4 : index10 % 5 - 1;
                        var y = 4 - index10 / 5 + (index10 % 5 == 0 ? 1 : 0);
                        result.Lat += y * _scaleRanges[sheetPart.Key].lat;
                        result.Lon += x * _scaleRanges[sheetPart.Key].lon;
                        break;
                }
            }

            result.Lat /= 36000000;
            result.Lon /= 36000000;
            return result;
        }


        private SheetDto AggregateSheets(int lat, int lon, int scale)
        {
            var result = new SheetDto
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

        public Dictionary<int, string> GetSheetParts(string sheetName)
        {
            var sheetNameRegex =
                new Regex(@"^([a-zA-ZsçÇöÖşŞıİğĞüÜ]*)?\-?([A-Z][0-9]{2})\-?([abcd])?([1-4])?\-?([0-9]{1,2})?\-?([abcd])?\-?([1-4])?\-?([abcd])?");
            var matchedSheetParts = sheetNameRegex.Match(sheetName);

            var result = new Dictionary<int, string>();
            var scales = _scaleRanges.Keys.ToArray();
            var matchedGroups = matchedSheetParts.Groups.Values.ToArray();

            for (var i = 1; i < matchedGroups.Length; i++)
            {
                if (matchedGroups[i].Value.Length > 0)
                {
                    result.Add(scales[i - 1], matchedGroups[i].Value);
                }
            }

            return result;
        }
    }
}

