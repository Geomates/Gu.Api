using System.Collections.Generic;

namespace Gu.PaftaBulucu.Business.Dtos
{
    public class SaveProjectDto
    {
        public string Email { get; set; }

        public string Name { get; set; }

        public List<SheetEntryDto> Entries { get; set; }
    }
}
