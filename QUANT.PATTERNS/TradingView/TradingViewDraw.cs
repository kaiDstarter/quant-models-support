using Microsoft.Data.Analysis;
using QUANT.PATTERNS.Models;
using QUANT.PATTERNS.Models.Responses.Shape;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QUANT.PATTERNS.TradingView
{
    public class TradingViewDraw : ITradingViewDraw
    {

        #region "Private function"
        private bool IsUpMarket(string structure)
        {
            return (structure == Constants.HIGH_HIGH || structure == Constants.LOW_HIGH);
        }

        private bool IsDownMarket(string structure)
        {
            return (structure == Constants.LOW_LOW || structure == Constants.HIGH_LOW);
        }
        #endregion

        #region "Public function"


        /// <summary>
        /// Rule:
        /// Nếu khoảng to có chứa nhiều khoảng nhỏ nhưng đều là bos thì lấy khoảng to, ngược lại thì lấy khoảng nhỏ
        /// </summary>
        /// <param name="df"></param>
        /// <returns></returns>
        public List<Shape> DrawBosAndChoCh(DataFrame df)
        {
            List<Shape> temps = new();
            string prevType = "";
            string prevTrend = "";
            decimal prevPrice = 0;
            foreach (var item in df.Rows)
            {
                var val = item["BosChoChPoint"];

                if (val == null || string.IsNullOrWhiteSpace(val.ToString()))
                    continue;
                string structure = item["Structure"].ToString() ?? "";
                if (string.IsNullOrWhiteSpace(structure)) continue;

                Shape shape = new Shape();
                shape.shapeType = Enumerations.ShapeType.MULTIPLE;
                decimal currentPrice = IsUpMarket(structure) ? (decimal)item["High"] : (decimal)item["Low"];
                string currentTrend = IsUpMarket(structure) ? Constants.UP : Constants.DOWN;
                ShapePoint start = new ShapePoint()
                {
                    time = (long)item["Time"],
                    price = currentPrice,
                };
                ShapePoint end = new ShapePoint()
                {
                    time = (long)item["BosChoChPoint"],
                    price = currentPrice,
                };
                shape.points = new List<ShapePoint>() { start, end };
                string text = (temps.Count == 0 ? Constants.BOS : item["Signal"].ToString()) ?? "Unknow";

                //nếu cùng xu hướng thì tạo tiếp BOS
                if (currentTrend == prevTrend)
                {
                    text = Constants.BOS;
                }

                //
                if (text == Constants.CHoCH)
                {
                    text += currentTrend;
                }

                ShapeOption option = new ShapeOption()
                {
                    shape = "trend_line",
                    text = text,// $"{text} ({item["Signal"].ToString()})",
                    overrides = new Dictionary<string, object>()
                    {
                        {"linecolor",IsUpMarket(structure) ? "green" : "red"},
                        {"textcolor",IsUpMarket(structure) ? "green" : "red"},
                        {"showLabel",true },
                        {"vertLabelsAlign", IsUpMarket(structure)?"bottom":"top"},
                        {"fontsize",12 }
                    }
                };
                shape.shapeOptions = option;
                temps.Add(shape);
                prevType = text;
                prevPrice = currentPrice;
                prevTrend = currentTrend;
            }
            List<Shape> shapes = new();
            List<int> ignoreHashCodes = new();
            foreach (var item in temps)
            {
                var lst = temps.Where(x => x.points[0].time > item.points[0].time && x.points[1].time <= item.points[1].time);
                var typesCount = lst.Select(x => x.shapeOptions.text).Distinct().Count();
                //nếu có 2 loại trong 1 range của shape thì ko hiển thị shape này
                if (typesCount > 1)
                {
                    continue;
                }
                ignoreHashCodes.AddRange(lst.Select(x => x.GetHashCode()));
                if (ignoreHashCodes.Contains(item.GetHashCode()))
                    continue;
                item.shapeOptions.text = item.shapeOptions.text.Replace(Constants.DOWN, "").Replace(Constants.UP, "");
                shapes.Add(item);
            }
            return shapes;
        }

        public List<Shape> DrawMarketStruct(DataFrame df)
        {
            List<Shape> shapes = new();
            foreach (var item in df.Rows)
            {
                string trend = item["Structure"].ToString() ?? "";
                if (string.IsNullOrWhiteSpace(trend)) continue;
                Shape shape = new Shape();
                shape.shapeType = Enumerations.ShapeType.MULTIPLE;
                decimal price = (trend == Constants.HIGH_HIGH || trend == Constants.LOW_HIGH) ? (decimal)item["High"] : (decimal)item["Low"];
                long time = (long)item["Time"];
                ShapePoint structPoint = new ShapePoint()
                {
                    time = (long)item["Time"],
                    price = price,
                };
                ShapePoint structPoint2 = new ShapePoint()
                {
                    time = (long)item["Time"],
                    price = price,
                };
                shape.points = new List<ShapePoint> { structPoint, structPoint2 };
                string color = (trend == Constants.HIGH_HIGH || trend == "HL") ? "green" : "red";
                ShapeOption option = new ShapeOption()
                {
                    shape = "callout",
                    text = (trend == Constants.HIGH_HIGH || trend == Constants.LOW_HIGH) ? trend + "\n\n\n" : "\n\n\n" + trend,
                    overrides = new Dictionary<string, object>() {
                            {"color",color},
                            {"backgroundColor","rgba(0, 0, 0, 0)"},
                            {"bordercolor","rgba(0, 0, 0, 0)"},
                        }
                };
                shape.shapeOptions = option;

                shapes.Add(shape);
            }
            return shapes;
        }
        #endregion
    }
}
