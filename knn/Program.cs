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