using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Analysis;
using QUANT.PATTERNS;
using QUANT.PATTERNS.Base;
using QUANT.PATTERNS.Models;
using QUANT.PATTERNS.Models.Responses.Shape;
using System.Reflection.Metadata;

namespace QUANT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatternController : ControllerBase
    {
        private readonly IPatternBase _pattern;
        public PatternController(IPatternBase pattern)
        {
            _pattern = pattern;
        }
        [HttpPost("draw-atr")]
        public IActionResult DrawATR(List<OHCLV> listohclv)
        {
            try
            {
                DataFrame df = new DataFrame();
                df = df.LoadFromOHCLVList(listohclv);
                DataFrame dfClone = df.Clone();
                var atr = _pattern.ATR(df);
                df.Columns.Add(atr);

                var pivot_high = _pattern.PivotHigh(df);
                df.Columns.Add(pivot_high);

                var pivot_low = _pattern.PivotLow(df);
                df.Columns.Add(pivot_low);

                _pattern.DetectMarketStructure(df);
                _pattern.DetectPointDrawBosChoCh(df);
                _pattern.DetectBosAndChoch(df);
                _pattern.DetectOrderBlocks(df);
                _pattern.DetectFVG(df);
                _pattern.DetectEQHL(df);
                _pattern.ComputePremiumDiscount(df);
                _pattern.LabelStrongWeak(df);
                _pattern.ApplyConfluenceFilter(df);
                _pattern.DetectMtfFvgOb(df, dfClone);
                string path = Path.Combine(Environment.CurrentDirectory, "uploads", "data.csv");
                DataFrame.SaveCsv(df, path);
                List<Shape> shapes = new();
                foreach (var item in df.Rows)
                {
                    var val = item["BosChoChPoint"];
                    if (val == null || string.IsNullOrWhiteSpace(val.ToString()))
                        continue;
                    Shape shape = new Shape();
                    shape.shapeType = Enumerations.ShapeType.MULTIPLE;
                    ShapePoint start = new ShapePoint()
                    {
                        time = (long)item["Time"],
                        price = (decimal)item["High"],
                    };
                    ShapePoint end = new ShapePoint()
                    {
                        time = (long)item["BosChoChPoint"],
                        price = (decimal)item["High"],
                    };
                    shape.points = new List<ShapePoint>() { start, end };
                    ShapeOption option = new ShapeOption()
                    {
                        shape = "trend_line",
                        text = item["Signal"].ToString()
                    };
                    shape.shapeOptions = option;
                    shapes.Add(shape);
                }
                return Ok(shapes);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
