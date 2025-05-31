using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection.Metadata.Ecma335;

class Program
{
    public static void Main(string[] args)
    {
        
        string plik = @"../../../../dane.txt";
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
        int k = 60;
        Metryka e = Euklidesowa;
        Metryka m = Manhatan;
        Metryka c = Czybyszew;
        Metryka z = Zalgorytmem;
        var wynik = Indeksynajmniejszych(dane, nowy, m, k);

        double wynikKlasyfikacji = Klasyfikuj(ostatniaKolumna, wynik);
        Console.WriteLine($"Przewidziana klasa: {wynikKlasyfikacji}");



/*
        double[] wektor = { 1.0, 2.0, 3.0, 4.0 };
        double[] znormalizowany = Normalizacja(wektor);

        Console.WriteLine("Znormalizowany wektor:");
        for (int i = 0; i < znormalizowany.Length; i++)
        {
            Console.WriteLine(znormalizowany[i]);
        }
*/
        List<double[]> Znormalizowane = new List<double[]>();

        foreach (var wiersz in probki)
        {
            Znormalizowane.Add(Normalizacja(wiersz));
        }
        double[] wynikZadania = new double[Znormalizowane.Count];
        for(int i = 0; i<Znormalizowane.Count; i++)
        {
            var zadanie = Indeksynajmniejszych(Znormalizowane, Znormalizowane[i], m, k);
            wynikZadania[i] = Klasyfikuj(ostatniaKolumna, zadanie);

        }
        for (int i = 0; i < wynikZadania.Length; i++)
        {
            Console.WriteLine($"Wynik klasyfikacji  {i + 1}: {wynikZadania[i]} Oczekiwana klasa: "+ string.Join(",", ostatniaKolumna[i]));
        }
        double wynikprzedprocentami = 0;
        for(int i = 0; i<Znormalizowane.Count ; i++)
        {
            if (wynikZadania[i] == ostatniaKolumna[i][0])
            {
                wynikprzedprocentami++;
            }
        }

        double wynikWProcentach = wynikprzedprocentami/ostatniaKolumna.Count * 100;
        Console.WriteLine($"Wynik w procentach: {wynikWProcentach}");
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

       // for (int i = 0; i < ile; i++)
       // {
      //      Console.WriteLine($"Indeks: {indeksy[i]}, Odległość: {odleglosci[i]}");
       // }
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


    static double[] Normalizacja(double[] A)
    {
        double min = A.Min();
        double max = A.Max();
        double[] wynik = new double[A.Length];
        if (max == min)
        {
            Console.WriteLine("Wszystkie wartości są takie same — nie można normalizować.");
            return A.Select(x => 0.0).ToArray(); 
        }


        for (int i = 0; i < A.Length; i++)
        {
            wynik[i] = (A[i] - min) / (max - min);
        }

        return wynik;
    }
    


}