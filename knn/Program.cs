using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

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

        List<double[]> probki = new List<double[]>();
        foreach (var wiersz in dane)
            probki.Add(new double[] { wiersz[0], wiersz[1], wiersz[2], wiersz[3] });

        List<double[]> klasy = new List<double[]>();
        foreach (var wiersz in dane)
            klasy.Add(new double[] { wiersz[4] });

        List<double[]> Znormalizowane = NormalizujKolumnowo(probki);

        int k = 99;

        Metryka[] metryki = {
        Euklidesowa,
        Manhatan,
        Czybyszew,
        Zlogarytmem,
        Minkowski
    };

        string[] nazwyMetryk = {
        "Euklidesowa",
        "Manhatan",
        "Czybyszew",
        "Zlogarytmem",
        "Minkowski (p=3)"
    };

        List<string> wynikiDoWyswietlenia = new List<string>();

        for (int m = 0; m < metryki.Length; m++)
        {
            double dokladnosc = JedenKontraReszta(Znormalizowane, klasy, k, metryki[m], nazwyMetryk[m]);
            wynikiDoWyswietlenia.Add($"Metryka: {nazwyMetryk[m]}, k = {k}, Dokładność: {dokladnosc:F2}%");
        }

        Console.WriteLine("\nWYNIKI KLASYFIKACJI:");
        foreach (var wiersz in wynikiDoWyswietlenia)
            Console.WriteLine(wiersz);
    }

    static double JedenKontraReszta(List<double[]> dane, List<double[]> klasy, int k, Metryka metryka, string nazwaMetryki)
    {
        int poprawne = 0;
        int total = dane.Count;

        for (int i = 0; i < total; i++)
        {
            
            var testowy = dane[i];

            
            var uczacy = new List<double[]>();
            var klasyUczace = new List<double[]>();
            for (int j = 0; j < total; j++)
            {
                if (j == i) continue;
                uczacy.Add(dane[j]);
                klasyUczace.Add(klasy[j]);
            }

            
            int[] najblizszeIndeksy = Indeksynajmniejszych(uczacy, testowy, metryka, k);

            double wynikKlasyfikacji = Klasyfikuj(klasyUczace, najblizszeIndeksy);

            
            if (wynikKlasyfikacji != 0 && wynikKlasyfikacji == (int)klasy[i][0])
                poprawne++;
        }

        double procent = (double)poprawne / total * 100.0;
        return procent;
    }


    static double Euklidesowa(double[] A, double[] B)
    {
        double wynik = 0;
        for (int i = 0; i < A.Length; i++)
            wynik += (A[i] - B[i]) * (A[i] - B[i]);

        return Math.Sqrt(wynik);
    }

    static double Manhatan(double[] A, double[] B)
    {
        double wynik = 0;
        for (int i = 0; i < A.Length; i++)
            wynik += Math.Abs(A[i] - B[i]);

        return wynik;
    }

    static double Czybyszew(double[] A, double[] B)
    {
        double max = 0;
        for (int i = 0; i < A.Length; i++)
        {
            double roznica = Math.Abs(A[i] - B[i]);
            if (roznica > max) max = roznica;
        }
        return max;
    }

    static double Zlogarytmem(double[] A, double[] B)
    {
        double wynik = 0;
        for (int i = 0; i < A.Length; i++)
            wynik += Math.Abs(Math.Log10(A[i]) - Math.Log10(B[i]));

        return wynik;
    }
    static double Minkowski(double[] A, double[] B)
    {
        double p = 3.0; 
        double wynik = 0;
        for (int i = 0; i < A.Length; i++)
        {
            wynik += Math.Pow(Math.Abs(A[i] - B[i]), p);
        }
        return Math.Pow(wynik, 1.0 / p);
    }


    delegate double Metryka(double[] A, double[] B);

    static int[] Indeksynajmniejszych(List<double[]> dane, double[] wektorTestowy, Metryka metryka, int ile)
    {
        double[] odleglosci = new double[dane.Count];
        int[] indeksy = new int[dane.Count];

        for (int i = 0; i < dane.Count; i++)
        {
            odleglosci[i] = metryka(dane[i], wektorTestowy);
            indeksy[i] = i;
        }

        Array.Sort(odleglosci, indeksy);
        return indeksy.Take(ile).ToArray();
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
        var najczestsze = licznik.Where(x => x.Value == max).Select(x => x.Key).ToList();

        if (najczestsze.Count > 1)
            return 0;

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
                nowyWiersz[i] = (max[i] == min[i]) ? 0.0 : (wiersz[i] - min[i]) / (max[i] - min[i]);
            }
            wynik.Add(nowyWiersz);
        }

        return wynik;
    }
}
