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
    public static double Distance(double[,] a, int i, double[,] b, int j, bool onlyX1)
    {
        double dx = a[i, 0] - b[j, 0];
        double dy = a[i, 1] - b[j, 1];
        return onlyX1 ? Math.Abs(dx) : Math.Sqrt(dx * dx + dy * dy);
    }

    public static List<KMeansRound> KMeansRounds(
        double[,] samples, int m, int iters,
        bool onlyX1, int[] stateAt)
    {
        int M = samples.GetLength(0);
        var rnd = new Random(seed);
        var iterations = new List<KMeansRound>();

        double[,] V = new double[m, 2];
        bool[] used = new bool[M];

        for (int j = 0; j < m; j++)
        {
            int s;

            do
            {
                s = rnd.Next(M);
            } while (used[s]);

            used[s] = true;

            V[j, 0] = samples[s, 0];
            V[j, 1] = samples[s, 1];
        }

        int[] u = new int[M];

        for (int it = 1; it <= iters; it++)
        {
            for (int s = 0; s < M; s++)
            {
                double best = double.MaxValue;
                for (int j = 0; j < m; j++)
                {
                    double d = Distance(samples, s, V, j, onlyX1);
                    if (d < best)
                    {
                        best = d;
                        u[s] = j;
                    }
                }
            }

            for (int j = 0; j < m; j++)
            {
                double sum0 = 0;
                double sum1 = 0;
                int count = 0;

                for (int i = 0; i < M; i++)
                {
                    sum0 += samples[i, 0];
                    sum1 += samples[i, 1];
                    count++;
                }

                V[j, 0] = sum0 / count;
                V[j, 1] = sum1 / count;
            }

            if (stateAt.Contains(it))
            {
                iterations.Add(new KMeansRound
                {
                    Iteration = it,
                    Center = (double[,])V.Clone(),
                    Assignments = (int[])u.Clone()
                });
            }
        }

        return iterations;
    }
}