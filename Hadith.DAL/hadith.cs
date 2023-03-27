using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadith.DAL
{
    [Table(name: "Hadith")]
    public class hadith
    {
        [Key]
        [Column(name: "HadithID")]
        public int HadithID { get; set; }

        [Column(name: "Name")]
        public string? Name { get; set; }

        [Column(name: "Title")]
        public string? Title { get; set; }

        [Column(name: "About")]
        public string? About { get; set; }

        [Column(name: "Arabic")]
        public string? Arabic { get; set; }

        [Column(name: "TotalHadith")]
        public int? TotalHadith { get; set; }

        [Column(name: "TotalPage")]
        public int? TotalPage { get; set; }
    }
}
