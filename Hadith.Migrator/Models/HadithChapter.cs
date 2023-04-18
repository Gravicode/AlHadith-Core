using System;
using System.Collections.Generic;

namespace Hadith.Migrator.Models;

public partial class HadithChapter
{
    public int ChapterId { get; set; }

    public int? ChapterNo { get; set; }

    public string? Title { get; set; }

    public string? TitleArabic { get; set; }

    public string? Intro { get; set; }

    public int? PageNo { get; set; }

    public int? HadithId { get; set; }

    public string? ChapterNoStr { get; set; }
}
