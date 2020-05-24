using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gu.PaftaBulucu.Data.Models
{
    public class Project
    {
        [Key]
        [Column("pid")]
        public int ProjectId{ get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        [Column("type")]
        public string ProjectType { get; set; }

        [Column(TypeName = "jsonb")]
        public List<SheetEntry> Entries { get; set; }

        public int Created { get; set; }
    }
}