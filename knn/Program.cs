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
        int k = 3;
        Metryka e = Euklidesowa;
        Metryka m = Manhatan;
        Metryka c = Czybyszew;
        Metryka z = Zlogarytmem;
      
        List<double[]> Znormalizowane = NormalizujKolumnowo(probki);

        double[] wynikZadania = new double[Znormalizowane.Count];
        for (int i = 0; i < Znormalizowane.Count; i++)
        {
            var zadanie = Indeksynajmniejszych(Znormalizowane, Znormalizowane[i], z, k + 1);
            var filtr = zadanie.Where(index => index != i).Take(k).ToArray();
            wynikZadania[i] = Klasyfikuj(ostatniaKolumna, filtr);
        }

        for (int i = 0; i < wynikZadania.Length; i++)
        {
            Console.WriteLine($"Wynik klasyfikacji  {i + 1}: {wynikZadania[i]} Oczekiwana klasa: "+ string.Join(",", ostatniaKolumna[i]));
        }
        double wynikprzedprocentami = 0;
        for(int i = 0; i<Znormalizowane.Count ; i++)
        {
            if (wynikZadania[i] != 0 && wynikZadania[i] == ostatniaKolumna[i][0])

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
        for (int i = 0; i < A.Length; i++)
        {
            wynik += (A[i] - B[i]) * (A[i] - B[i]);
        }
        return Math.Sqrt(wynik);
    }

    static double Manhatan(double[] A, double[] B)
    {
        double wynik = 0;
        for (int i = 0; i < A.Length; i++)
        {
            wynik += Math.Abs((A[i] - B[i]));
        }
        return wynik;
    }

    static double Czybyszew(double[]A, double[]B)
    {
        double[] wynik = new double[A.Length];
        for(int i=0; i < A.Length; i++)
        {
            wynik[i] = Math.Abs(A[i] - B[i]);
        }
        double Max=wynik.Max();
        return Max;
    }

    
    static double Zlogarytmem(double[] A, double[] B)
    {
        double wynik = 0;
        for (int i = 0; i < A.Length ; i++)
        {
            wynik += Math.Abs(Math.Log10(A[i]) - Math.Log10(B[i]));
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

       
        int[] najblizszeIndeksy = new int[ile];
        for (int i = 0; i < ile; i++)
        {
            najblizszeIndeksy[i] = indeksy[i];
        }
        return najblizszeIndeksy;
    }
    static double Klasyfikuj(List<double[]> klasy, int[] indeksyNajblizszych)
    {
        Dictionary<int, int> licznik = new Dictionary<int, int>();

        foreach (int indeks in indeksyNajblizszych)
        {
            int klasa = (int)klasy[indeks][0];
            if (!licznik.ContainsKey(klasa))
                licznik[klasa] = 1;
            else
                licznik[klasa]++;
        }

        int max = licznik.Values.Max();
        var najczestsze = licznik.Where(kvp => kvp.Value == max).Select(kvp => kvp.Key).ToList();

        if (najczestsze.Count > 1)
        {
            return 0; 
        }

        return najczestsze[0];
    }


    static List<double[]> NormalizujKolumnowo(List<double[]> dane)
    {
        int kolumny = dane[0].Length;
        int wiersze = dane.Count;

        double[] min = new double[kolumny];
        double[] max = new double[kolumny];

        for (int i = 0; i < kolumny; i++)
        {
            min[i] = double.MaxValue;
            max[i] = double.MinValue;
        }

        foreach (var wiersz in dane)
        {
            for (int i = 0; i < kolumny; i++)
            {
                if (wiersz[i] < min[i]) min[i] = wiersz[i];
                if (wiersz[i] > max[i]) max[i] = wiersz[i];
            }
        }

        
        List<double[]> wynik = new List<double[]>();
        foreach (var wiersz in dane)
        {
            double[] nowyWiersz = new double[kolumny];
            for (int i = 0; i < kolumny; i++)
            {
                if (max[i] == min[i])
                {
                    nowyWiersz[i] = 0.0; 
                }
                else
                {
                    nowyWiersz[i] = (wiersz[i] - min[i]) / (max[i] - min[i]);
                }
            }
            wynik.Add(nowyWiersz);
        }

        return wynik;
    }




}



