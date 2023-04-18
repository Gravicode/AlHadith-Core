using System;
using System.Collections.Generic;

namespace Hadith.Migrator.Models;

public partial class Crawled
{
    public int Id { get; set; }

    public string Url { get; set; }

    public string Description { get; set; }
}
