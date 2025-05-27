using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;


class Program
{
    public static void Main(string[] args)
    {
        
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

        foreach (var wiersz in dane)
        {
            probki.Add(new double[] { wiersz[0], wiersz[1], wiersz[2], wiersz[3] });
        }

        
        List<double[]> ostatniaKolumna = new List<double[]>();
        foreach(var wiersz in dane)
        {
            ostatniaKolumna.Add(new double[] { wiersz[4] });
        }
        double[]nowy= new double[] { 1.2, 1.5, 5, 2.3 };
        int k = 3;
        Metryka m = Euklidesowa;
        var wynik = Indeksynajmniejszych(probki, nowy, m, k);

        double wynikKlasyfikacji = Klasyfikuj(ostatniaKolumna, wynik);
        Console.WriteLine($"Przewidziana klasa: {wynikKlasyfikacji}");

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


    static int[] Indeksynajmniejszych(List<double[]> dane, double[] wektorTestowy, Metryka metryka,int ile)
    {
        double[] odleglosci = new double[dane.Count];

        int[] indeksy = new int[dane.Count];


        for (int i = 0; i < dane.Count; i++)
        {
            odleglosci[i] = metryka(dane[i], wektorTestowy);
            indeksy[i] = i;
        }
        Array.Sort(odleglosci, indeksy);

        for (int i = 0; i < ile; i++)
        {
            Console.WriteLine($"Indeks: {indeksy[i]}, Odległość: {odleglosci[i]}");
        }
        int[] najblizszeIndeksy = new int[ile];
        for (int i = 0; i < ile; i++)
        {
            najblizszeIndeksy[i] = indeksy[i];
        }
        return najblizszeIndeksy;
    }
    static double Klasyfikuj(List<double[]> klasy, int[] indeksyNajblizszych)
    {
        double[] klasyNajblizszych = new double[indeksyNajblizszych.Length];

        for (int i = 0; i < indeksyNajblizszych.Length; i++)
        {
            int indeks = indeksyNajblizszych[i];
            klasyNajblizszych[i] = klasy[indeks][0]; 
        }

        double najczestsza = klasyNajblizszych[0];
        int maxLicznik = 1;

        for (int i = 0; i < klasyNajblizszych.Length; i++)
        {
            double aktualna = klasyNajblizszych[i];
            int licznik = 0;

            for (int j = 0; j < klasyNajblizszych.Length; j++)
            {
                if (klasyNajblizszych[j] == aktualna)
                {
                    licznik++;
                }
            }

            if (licznik > maxLicznik)
            {
                maxLicznik = licznik;
                najczestsza = aktualna;
            }
        }

        return najczestsza;
    }


}