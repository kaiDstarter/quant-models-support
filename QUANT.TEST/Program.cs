// See https://aka.ms/new-console-template for more information
using Microsoft.Data.Analysis;
using Microsoft.ML;
using System.Data.Common;

Console.WriteLine("Hello, World!");
int index = 0;
for (int i = 0; i < 10; i = index)
{
    Console.WriteLine(i);
    if (i == 1 || i == 5) index = i + 3;
    else index = i + 1;

}

Console.ReadLine();