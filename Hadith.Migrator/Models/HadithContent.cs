using System;
using System.Collections.Generic;

namespace Hadith.Migrator.Models;

public partial class HadithContent
{
    public int ContentId { get; set; }

    public int? HadithId { get; set; }

    public int? ChapterNo { get; set; }

    public int? PageNo { get; set; }

    public string? Narated { get; set; }

    public string? ContentEnglish { get; set; }

    public string? ContentUrdu { get; set; }

    public string? ContentIndonesia { get; set; }

    public string? ContentArabic { get; set; }

    public string? Grade { get; set; }

    public string? Reference { get; set; }

    public string? SanadTop { get; set; }

    public string? SanadBottom { get; set; }

    public int? HadithOrder { get; set; }

    public string? BookRef { get; set; }

    public string? Uscref { get; set; }

    public string? OtherRef { get; set; }

    public string? UrlRef { get; set; }
}
