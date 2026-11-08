namespace Lab1_wfa;

public class KMeansRound
{
    public int Iteration { get; set; }
    public double[,] Center { get; set; } = new double[0, 0];
    public int[] Assignments { get; set; } = Array.Empty<int>();
}

public class GroupStats
{
    public int Group { get; set; }
    public double CenterX1 { get; set; }
    public double CenterX2 { get; set; }
    public int Count { get; set; }
    public double MinX1 { get; set; }
    public double MinX2 { get; set; }
    public double MaxX1 { get; set; }
    public double MaxX2 { get; set; }
}

public class Kmeans
{
    private static int seed = 123;
    public static double Distance(double[] a, double[] b)
    {
        return Math.Sqrt((a[0] - b[0]) * (a[0] - b[0]) + (a[1] - b[1]) * (a[1] - b[1]));
    }

    public static double CalculateX1(double[] a, double[] b)
    {
        return Math.Abs(a[0] - b[0]);
    }

    public static List<KMeansRound> KMeansRounds(
        double[,] samples, int m, int iters,
        Func<double[], double[], double> distance,
        int[] stateAt)
    {
        int M = samples.GetLength(0);
        int attrs = samples.GetLength(1);
        var rnd = new Random(seed);
        var iterations = new List<KMeansRound>();
        
        double[,] V = new double[m, attrs];
        bool[] used = new bool[M];

        for (int j = 0; j < m; j++)
        {
            int s;
            
            do
            {
                s = rnd.Next(M);
            }
            while (used[s]);
            
            used[s] = true;
            
            for (int i = 0; i < attrs; i++)
            {
                V[j, i] = samples[s, i];
            }
        }
        
        for (int it = 1; it <= iters; it++)
        {
            for (int s = 0; s < M; s++)
            {
                double[] p = { samples[s, 0], samples[s, 1] };
                double best = double.MaxValue;
                for (int j = 0; j < m; j++)
                {
                    double[] c = { V[j, 0], V[j, 1] };
                    double d = Distance(p, c);
                    if (d < best)
                    {
                        best = d;
                        u[s] = j;
                    }
                }
            }

            for (int j = 0; j < m; j++)
            {
                List<int> group = new List<int>();
                for (int s = 0; s < M; s++)
                    if (u[s] == j) group.Add(s);

                if (group.Count == 0) continue;

                for (int i = 0; i < attrs; i++)
                {
                    double sum = 0;
                    foreach (int s in group) sum += samples[s, i];
                    V[j, i] = sum / group.Count;
                }
            }

            if (Array.IndexOf(reportAt, it) >= 0) Report(it);
        }
        
        return iterations;
    }
}