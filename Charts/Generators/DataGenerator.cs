using System;
using System.IO;

namespace Charts.Generators;

public class DataGenerator
{
    public static void GenEnumerate(string filePath, int n)
    {
        Random random = new Random();

        for (int i = 0; i < n; i++)
        {
            File.AppendAllText(filePath, $"{i + 1} {random.Next(100)}\n");
        }
    }

    public static void GenDateInf(string filePath, int n)
    {
        Random random = new Random();

        for (int i = 0; i < n; i++)
        {
            File.AppendAllText(filePath, $"{DateTime.Now.AddDays(i)} {random.Next(100)}\n");
        }
    }
}