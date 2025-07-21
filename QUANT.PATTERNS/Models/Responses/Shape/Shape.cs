using QUANT.PATTERNS.Models.Responses.Shape;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static QUANT.PATTERNS.Enumerations;

namespace QUANT.PATTERNS.Models;

public class Shape
{
    /// <summary>
    /// 0: Single, 1: Multiple, 2: Execution action
    /// </summary>
    public ShapeType shapeType { get; set; }
    public List<ShapePoint>? points { get; set; }
    public ShapeOption? shapeOptions { get; set; }
    public string trend { get; set; } = "";
    public decimal price { get; set; }
}
