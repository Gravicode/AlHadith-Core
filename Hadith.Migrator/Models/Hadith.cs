using System;
using System.Collections.Generic;

namespace Hadith.Migrator.Models;

public partial class Hadith
{
    public int HadithId { get; set; }

    public string Name { get; set; }

    public string Title { get; set; }

    public string About { get; set; }

    public string Arabic { get; set; }
}
