using System;
using System.Collections.Generic;

namespace Hadith.Migrator.Models;

public partial class HadithPage
{
    public int PageId { get; set; }

    public string Title { get; set; }

    public string TitleArabic { get; set; }

    public int? PageNo { get; set; }

    public int? HadithId { get; set; }
}
