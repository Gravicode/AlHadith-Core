using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadith.DAL
{
    [Table(name: "Bookmark")]
    public class Bookmark
    {
        [Key]
        [Column(name: "idx")]
        public int idx { get; set; }

        [Column(name: "Title")]
        public string? Title { get; set; }

        [Column(name: "HadithId")]
        public int HadithId { get; set; }

        [Column(name: "PageNo")]
        public int PageNo { get; set; }

        [Column(name: "ChapterNo")]
        public int ChapterNo { get; set; }

        [Column(name: "HadithNo")]
        public int HadithNo { get; set; }


    }
}
