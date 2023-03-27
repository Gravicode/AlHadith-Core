using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadith.DAL
{
    [Table(name: "HadithContent")]
    public class hadithcontent
    {
        [Key]
        [Column(name: "ContentID")]
        public int ContentID { get; set; }

        [Column(name: "HadithID")]
        public int HadithID { get; set; }

        [Column(name: "ChapterNo")]
        public int ChapterNo { get; set; }

        [Column(name: "PageNo")]
        public int PageNo { get; set; }

        [Column(name: "Narated")]
        public string? Narated { get; set; }

        [Column(name: "ContentEnglish")]
        public string? ContentEnglish { get; set; }

        [Column(name: "ContentUrdu")]
        public string? ContentUrdu { get; set; }

        [Column(name: "ContentIndonesia")]
        public string? ContentIndonesia { get; set; }

        [Column(name: "ContentArabic")]
        public string? ContentArabic { get; set; }

        [Column(name: "Grade")]
        public string? Grade { get; set; }

        [Column(name: "Reference")]
        public string? Reference { get; set; }

        [Column(name: "SanadTop")]
        public string? SanadTop { get; set; }

        [Column(name: "SanadBottom")]
        public string? SanadBottom { get; set; }

        [Column(name: "HadithOrder")]
        public int HadithOrder { get; set; }

        [Column(name: "BookRef")]
        public string? BookRef { get; set; }

        [Column(name: "USCRef")]
        public string? USCRef { get; set; }

        [Column(name: "OtherRef")]
        public string? OtherRef { get; set; }

        [Column(name: "UrlRef")]
        public string? UrlRef { get; set; }

    }
}
