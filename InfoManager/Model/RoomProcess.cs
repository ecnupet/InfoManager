using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InfoManager.Model
{
    [Table("room_process")]
    public class RoomProcess
    {
        [Key]
        [Column("id")]
        public int ID { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("process")]
        public string Process { get; set; }
        [Column("father_id")]
        public int FatherID { get; set; }
        [Column("video")]
        public string Video { get; set; }
        [Column("image")]
        public string Image { get; set; }
    }
}
