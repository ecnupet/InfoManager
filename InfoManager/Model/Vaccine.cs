using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InfoManager.Model
{
    [Table("vaccine")]
    public class Vaccine
    {
        
        [Key]
        [Column("id")]
        public int ID { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("description")]
        public string Description { get; set; }
        [Column("image")]
        public string Image { get; set; }
        [Column("video")]
        public string Video { get; set; }

    }
}
