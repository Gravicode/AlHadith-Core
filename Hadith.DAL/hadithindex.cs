using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadith.DAL
{
    [Table(name: "HadithIndex")]
    public class hadithindex
    {
        [Key]
        [Column(name: "IndexID")]
        public int IndexID { get; set; }

        [Column(name: "HadithID")]
        public int HadithID { get; set; }

        [Column(name: "No")]
        public int No { get; set; }

        [Column(name: "Name")]
        public string? Name { get; set; }

        [Column(name: "ArabicName")]
        public string? ArabicName { get; set; }

        [Column(name: "IndexFrom")]
        public int? IndexFrom { get; set; }

        [Column(name: "IndexTo")]
        public int? IndexTo { get; set; }


    }
}
