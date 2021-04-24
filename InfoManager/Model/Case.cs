using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InfoManager.Model
{
    public enum CaseStages
    {
        /// <summary>
        /// 疾病介绍
        /// </summary>
        Introduce,
        /// <summary>
        /// 接诊
        /// </summary>
        ClinicalReception,
        /// <summary>
        /// 病例检查
        /// </summary>
        Check,
        /// <summary>
        /// 诊断结果
        /// </summary>
        Diagnosis,
        /// <summary>
        /// 治疗方案
        /// </summary>
        TherapeuticSchedule
    }
    [Table("case")]
    public class Case
    {
        [Key]
        [Column("id")]
        public int ID { get; set; }
        [Column("disease_id")]
        public int DiseaseID { get; set; }
        [Column("stage")]
        public CaseStages CaseStage { get; set; }
        [Column("description")]
        public string Description { get; set; }
        [Column("image")]
        public string Image { get; set; }
        [Column("video")]
        public string Video { get; set; }

    }
}
