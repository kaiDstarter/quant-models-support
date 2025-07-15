using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Analysis;
using QUANT.PATTERNS;
using QUANT.PATTERNS.Base;
using QUANT.PATTERNS.Models;

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
                //var atr = _pattern.ATR(df);
                //df.Columns.Add(atr);

                //var pivot_high = _pattern.PivotHigh(df);
                //df.Columns.Add(pivot_high);

                //var pivot_low = _pattern.PivotLow(df);
                //df.Columns.Add(pivot_low);

                _pattern.DetectMarketStructure(df);
                _pattern.DetectBosAndChoch(df);
                _pattern.DetectOrderBlocks(df);
                _pattern.DetectFVG(df);
                _pattern.DetectEQHL(df);
                _pattern.ComputePremiumDiscount(df);
                _pattern.LabelStrongWeak(df);
                _pattern.ApplyConfluenceFilter(df);
                _pattern.DetectMtfFvgOb(df,df.Clone());
                
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
