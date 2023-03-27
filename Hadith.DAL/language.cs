using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadith.DAL
{
    public class language
    {
        public string? lang { set; get; }
        [Key]
        public int langid { set; get; }
    }
}
