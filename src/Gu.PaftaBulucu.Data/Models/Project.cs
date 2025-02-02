using Amazon.DynamoDBv2.DataModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gu.PaftaBulucu.Data.Models
{
    [DynamoDBTable("pafta-bulucu-projects")]
    public class Project
    {
        [DynamoDBHashKey]
        public int ProjectId{ get; set; }

        [DynamoDBProperty]
        public string Email { get; set; }

        [DynamoDBProperty]
        public string Name { get; set; }

        [DynamoDBProperty]
        public List<SheetEntry> Entries { get; set; }

        [DynamoDBProperty]
        public int Created { get; set; }
    }
}