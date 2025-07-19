using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Analysis;
using QUANT.PATTERNS;
using QUANT.PATTERNS.Base;
using QUANT.PATTERNS.Models;
using QUANT.PATTERNS.Models.Responses.Shape;
using QUANT.PATTERNS.TradingView;
using System.Reflection.Metadata;

namespace QUANT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatternController : ControllerBase
    {
        private readonly IPatternBase _pattern;
        private readonly ITradingViewDraw _tradingViewTool;
        public PatternController(IPatternBase pattern, ITradingViewDraw tradingViewTool)
        {
            _pattern = pattern;
            _tradingViewTool = tradingViewTool;
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
                //string path = Path.Combine(Environment.CurrentDirectory, "uploads", "data.csv");
                //DataFrame.SaveCsv(df, path);
                List<Shape> shapes = new();
                shapes.AddRange(_tradingViewTool.DrawMarketStruct(df));
                shapes.AddRange(_tradingViewTool.DrawBosAndChoCh(df));
                foreach (var item in df.Rows)
                {
                    //var val = item["BosChoChPoint"];

                    //if (val == null || string.IsNullOrWhiteSpace(val.ToString()))
                    //    continue;
                    //string trend = item["Structure"].ToString() ?? "";
                    //if (string.IsNullOrWhiteSpace(trend)) continue;
                    //Shape shapeStruct = new Shape();
                    //shapeStruct.shapeType = Enumerations.ShapeType.MULTIPLE;
                    //decimal price = (trend == "HH" || trend == "LH") ? (decimal)item["High"] : (decimal)item["Low"];
                    //long time = (long)item["Time"];
                    //ShapePoint structPoint = new ShapePoint()
                    //{
                    //    time = (long)item["Time"],
                    //    price = price,
                    //};
                    //ShapePoint structPoint2 = new ShapePoint()
                    //{
                    //    time = (long)item["Time"],
                    //    price = price,
                    //};
                    //shapeStruct.points = new List<ShapePoint> { structPoint, structPoint2 };
                    //string color = (trend == "HH" || trend == "HL") ? "green" : "red";
                    //ShapeOption Structoption = new ShapeOption()
                    //{
                    //    shape = "callout",
                    //    text = (trend == "HH" || trend == "LH") ? trend + "\n\n\n" : "\n\n\n" + trend,
                    //    overrides = new Dictionary<string, object>() {
                    //        {"color",color},
                    //        {"backgroundColor","rgba(0, 0, 0, 0)"},
                    //        {"bordercolor","rgba(0, 0, 0, 0)"},
                    //    }
                    //};
                    //shapeStruct.shapeOptions = Structoption;

                    //shapes.Add(shapeStruct);

                    //Shape shape = new Shape();
                    //shape.shapeType = Enumerations.ShapeType.MULTIPLE;
                    //ShapePoint start = new ShapePoint()
                    //{
                    //    time = (long)item["Time"],
                    //    price = (trend == "HH" || trend == "LH") ? (decimal)item["High"] : (decimal)item["Low"],
                    //};
                    //ShapePoint end = new ShapePoint()
                    //{
                    //    time = (long)item["BosChoChPoint"],
                    //    price = (trend == "HH" || trend == "LH") ? (decimal)item["High"] : (decimal)item["Low"],
                    //};
                    //shape.points = new List<ShapePoint>() { start, end };
                    //ShapeOption option = new ShapeOption()
                    //{
                    //    shape = "trend_line",
                    //    text = item["Signal"].ToString()
                    //};
                    //shape.shapeOptions = option;
                    //shapes.Add(shape);
                }
                //foreach (var item in shapes)
                //{
                //    if (item.points[0].)
                //}
                return Ok(shapes);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
