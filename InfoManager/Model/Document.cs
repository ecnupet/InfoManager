using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InfoManager.Model
{
    [Table("document")]
    public class Document
    {
        [Key]
        [Column("id")]
        public int ID { get; set; }

        [Column("documentname")]
        public string DocumentName { get; set; }
        [Column("description")]
        public string Description { get; set; }

    }
}
