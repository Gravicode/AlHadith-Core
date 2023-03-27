using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadith.DAL
{
    [Table(name: "HadithPage")]
    public class hadithpage
    {
        [Key]
        [Column(name: "PageID")]
        public int PageID { get; set; }

        [Column(name: "Title")]
        public string? Title { get; set; }

        [Column(name: "TitleArabic")]
        public string? TitleArabic { get; set; }

        [Column(name: "PageNo")]
        public int PageNo { get; set; }

        [Column(name: "HadithID")]
        public int HadithID { get; set; }

       


    }
}
