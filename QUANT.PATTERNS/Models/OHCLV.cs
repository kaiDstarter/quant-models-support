using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QUANT.PATTERNS.Models
{
    public class OHCLV
    {
        public decimal open { get; set; }
        public decimal close { get; set; }
        public decimal high { get; set; }
        public decimal low { get; set; }
        public decimal volume { get; set; }
        public long time { get; set; }
        public string? timeFrame { get; set; }
    }
}
