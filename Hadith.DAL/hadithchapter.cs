using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadith.DAL
{
    [Table(name: "HadithChapter")]
    public class hadithchapter
    {
        [Key]
        [Column(name: "ChapterID")]
        public int ChapterID { get; set; }

        [Column(name: "ChapterNo")]
        public int ChapterNo { get; set; }

        [Column(name: "Title")]
        public string? Title { get; set; }

        [Column(name: "TitleArabic")]
        public string? TitleArabic { get; set; }

        [Column(name: "Intro")]
        public string? Intro { get; set; }

        [Column(name: "PageNo")]
        public int PageNo { get; set; }

        [Column(name: "HadithID")]
        public int HadithID { get; set; }

        [Column(name: "ChapterNoStr")]
        public string? ChapterNoStr { get; set; }

     
    }
}
