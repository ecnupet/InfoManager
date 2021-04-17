using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InfoManager.Model
{
    public enum DType
    {
        InfectiousDisease,
        ParasiticDisease

    }


    [Table("disease")]
    public class Disease
    {
        [Key]
        [Column("id")]
        public int ID { get; set; }
        [Column("disease_name")]
        public string DiseaseName { get; set; }
        [Column("disease_type")]
        public DType DiseaseType { get; set; }

    }
}
