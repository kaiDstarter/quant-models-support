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
            foreach (var item in df.Rows)
            {
                var val = item["BosChoChPoint"];

                if (val == null || string.IsNullOrWhiteSpace(val.ToString()))
                    continue;
                string structure = item["Structure"].ToString() ?? "";
                if (string.IsNullOrWhiteSpace(structure)) continue;

                Shape shape = new Shape();
                shape.shapeType = Enumerations.ShapeType.MULTIPLE;
                decimal currentPrice = Utils.IsUpMarket(structure) ? (decimal)item["High"] : (decimal)item["Low"];
                string currentTrend = Utils.IsUpMarket(structure) ? Constants.UP : Constants.DOWN;
                shape.price = currentPrice;
                shape.trend = currentTrend;
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
                text += currentTrend;


                ShapeOption option = new ShapeOption()
                {
                    shape = "trend_line",
                    text = text,
                    overrides = new Dictionary<string, object>()
                    {
                        {"linecolor",Utils.IsUpMarket(structure) ? "green" : "red"},
                        {"textcolor",Utils.IsUpMarket(structure) ? "green" : "red"},
                        {"showLabel",true },
                        {"vertLabelsAlign", Utils.IsUpMarket(structure)?"bottom":"top"},
                        {"fontsize",10 }
                    }
                };
                shape.shapeOptions = option;
                temps.Add(shape);
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

                // nếu là item đầu tiên thì add luôn không cần xử lý
                if (shapes.Count == 0)
                {
                    item.shapeOptions.text = Constants.BOS;
                    shapes.Add(item);
                    continue;
                }
                //nếu trend trc là up và currentPrice >= prevPrice thì tạo tiếp 1 BOS
                var prevItem = shapes.LastOrDefault();
                if (prevItem.trend == Constants.UP && item.price >= prevItem.price)
                {
                    item.shapeOptions.text = Constants.BOS;
                }
                else if (prevItem.trend == Constants.DOWN && item.price <= prevItem.price)
                {
                    //nếu trend trc là down và currentPrice <= prevPrice thì tạo tiếp 1 BOS
                    item.shapeOptions.text = Constants.BOS;
                }
                else
                {
                    item.shapeOptions.text = Constants.CHoCH;
                }
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
                string line = "\n\n\n\n\n";
                ShapeOption option = new ShapeOption()
                {
                    shape = "callout",

                    text = (trend == Constants.HIGH_HIGH || trend == Constants.LOW_HIGH) ? trend + line : line + trend,
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
