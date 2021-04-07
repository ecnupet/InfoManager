using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InfoManager.Model
{
    [Table("charge_project")]
    public class ChargeProject
    {
        [Key]
        [Column("id")]
        public int ID { get; set; }
        [Column("projectname")]
        public string ProjectName { get; set; }
        [Column("projectdescription")]
        public string ProjectDescription { get; set; }
        [Column("projectcharge")]
        public string ProjectCharge { get; set; }
    }
}
