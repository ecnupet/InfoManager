using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InfoManager.Model
{
    [Table("drug")]
    public class Drug
    {
        [Key]
        [Column("id")]
        public int ID { get; set; }
        [Column("drug_name")]
        public string DrugName { get; set; }
        [Column("drug_price")]
        public decimal DrugPrice { get; set; }
        [Column("drug_usage")]
        public string DrugUsage { get; set; }
        [Column("drug_save")]
        public string DrugSave { get; set; }
    }
}
