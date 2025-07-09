// See https://aka.ms/new-console-template for more information
using Microsoft.Data.Analysis;
using Microsoft.ML;
using System.Data.Common;

Console.WriteLine("Hello, World!");

PrimitiveDataFrameColumn<int> age = new PrimitiveDataFrameColumn<int>("age", new int[] { 1, 2, 3 });
PrimitiveDataFrameColumn<double> height = new PrimitiveDataFrameColumn<double>("height", new double[] { 1.6, 1.55, 1.8 });
PrimitiveDataFrameColumn<double> weight = new PrimitiveDataFrameColumn<double>("weight", new double[] { 55.5, 80, 60 });
StringDataFrameColumn names = new StringDataFrameColumn("name", new string[] { "lan", "ngoc",null });

DataFrame dataFrame = new(names,age,height,weight);

for (int i = 0; i < dataFrame.Rows.Count; i++)
{
    Console.WriteLine();
    Console.Write(dataFrame.Rows[i]["name"] ?? "empty");
    Console.Write(dataFrame.Rows[i]["age"] ?? "0");
    Console.Write(dataFrame.Rows[i]["height"] ?? "0.0");
   
}
var a = dataFrame.Columns[0].RightShift(0);
var gaps = age - weight;
//dataFrame.Columns.Add(gaps);


Console.ReadLine();