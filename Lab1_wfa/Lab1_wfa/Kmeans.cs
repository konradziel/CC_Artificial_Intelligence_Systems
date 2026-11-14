namespace Lab1_wfa;

public class KMeansRound
{
    public int Iteration { get; set; }
    public double[,] Center { get; set; } = new double[0, 0];
    public int[] Assignments { get; set; } = Array.Empty<int>();
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

    public static string Result(double[,] samples, KMeansRound round)
    {
        string text = $"Po {round.Iteration} iteracjach\r\n";

        for (int i = 0; i < round.Center.GetLength(0); i++)
        {
            int count = 0;
            double min1 = double.MaxValue, max1 = double.MinValue;
            double min2 = double.MaxValue, max2 = double.MaxValue;

            for (int j = 0; j < samples.GetLength(0); j++)
            {
                min1 = Math.Min(min1, samples[j, 0]);
                max1 = Math.Max(max1, samples[j, 0]);
                min2 = Math.Min(min2, samples[j, 1]);
                max2 = Math.Max(max2, samples[j, 1]);
            }

            text += $"Grupa {i + 1}: środek ({round.Center[i, 0]:F4}; {round.Center[i, 1]:F4}), próbek: {count}";

            if (count > 0)
            {
                text += $", x1 [{min1:F4}; {max1:F4}], x2 [{min2:F4}; {max2:F4}]";
            }

            text += "\r\n";
        }
        return text;
    }
}