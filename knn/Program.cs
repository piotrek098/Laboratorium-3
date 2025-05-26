using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;


class Program
{
    static Random random = new Random();
    public static void Main(string[] args)
    {
        double[]A=new double[10];
        double[]B=new double[10];
        for(int i = 0; i < 10; i++)
        {
            A[i]=random.NextDouble();
        }
        for (int i = 0; i < 10; i++)
        {
            B[i] = random.NextDouble();
        }

        Metryka m = Euklidesowa;
        double wynik=m(A,B);
        Console.WriteLine(wynik);


        Metryka n = Manhatan;
        double wynik1 = n(A, B);
        Console.WriteLine(wynik1);

        string plik = @"C:\Users\Lenovo\Desktop\WS\zadanie 2\knn\dane.txt"; 
        var dane = new List<double[]>();

        foreach (var line in File.ReadLines(plik))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var row = new double[parts.Length];

            for (int i = 0; i < parts.Length; i++)
                row[i] = double.Parse(parts[i], CultureInfo.InvariantCulture);

            dane.Add(row);
        }
       

        Console.WriteLine("Pierwszy wiersz:");
        Console.WriteLine(string.Join(", ", dane[0]));

        Console.WriteLine($"\nWczytano {dane.Count} wierszy.");

       

        List<double[]> probki = new List<double[]>();

        foreach(var wiersz in dane)
        {
            probki.Add(new double[] { wiersz[0], wiersz[1], wiersz[2], wiersz[3] });
        }
        for (int i = 0; i < probki.Count; i++)
        {
            Console.WriteLine($"Numer próbki {i+1}:");
            for (int j = 0; j < probki[i].Length; j++)
            {
                Console.Write($"{probki[i][j]} ");
            }
            Console.WriteLine();
        }
        
    }


    static double Euklidesowa(double[] A, double[] B)
    {
        double wynik = 0;
        for (int i = 0; i < A.Length - 1; i++)
        {
            wynik += (A[i] - B[i]) * (A[i] - B[i]);
        }
        return Math.Sqrt(wynik);
    }

    static double Manhatan(double[] A, double[] B)
    {
        double wynik = 0;
        for (int i = 0; i < A.Length - 1; i++)
        {
            wynik += Math.Abs((A[i] - B[i]));
        }
        return wynik;
    }

    static double Czybyszew(double[]A, double[]B)
    {
        double[] wynik = new double[A.Length];
        for(int i=0; i < A.Length -1; i++)
        {
            wynik[i] = Math.Abs(A[i] - B[i]);
        }
        double Max=wynik.Max();
        return Max;
    }
    static double Zalgorytmem(double[] A, double[] B)
    {
        double wynik = 0;
        for (int i = 0; i < A.Length - 1; i++)
        {
            wynik += Math.Abs(Math.Log(A[i]) - Math.Log(B[i]));
        }
        
        return wynik;
    }

    delegate double Metryka(double[] A, double[] B);

}