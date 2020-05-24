using System;
using System.Collections.Generic;
using System.Text;

namespace Gu.PaftaBulucu.Business.Dtos
{
    public class SheetEntryDto
    {
        public double Lat { get; set; }

        public double Lng { get; set; }

        public string Name { get; set; }

        public int Scale { get; set; }
    }
}
