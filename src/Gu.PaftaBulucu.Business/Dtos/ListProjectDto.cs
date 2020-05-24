using System.Collections.Generic;

namespace Gu.PaftaBulucu.Business.Dtos
{
    public class ListProjectDto
    {
        public int ProjectId { get; set; }

        public string Name { get; set; }

        public List<SheetEntryDto> Entries { get; set; }

        public int Created { get; set; }
    }
}
