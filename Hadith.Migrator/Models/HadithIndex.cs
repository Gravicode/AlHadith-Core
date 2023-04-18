using System;
using System.Collections.Generic;

namespace Hadith.Migrator.Models;

public partial class HadithIndex
{
    public int IndexId { get; set; }

    public int? HadithId { get; set; }

    public int? No { get; set; }

    public string Name { get; set; }

    public string ArabicName { get; set; }

    public int? IndexFrom { get; set; }

    public int? IndexTo { get; set; }
}
