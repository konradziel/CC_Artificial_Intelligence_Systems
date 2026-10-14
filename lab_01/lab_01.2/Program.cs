using System.Globalization;

var ci = CultureInfo.InvariantCulture;

string dataFile = Path.Combine(AppContext.BaseDirectory, "spiralka.csv");
string[] lines = File.ReadAllLines(dataFile);
int M = lines.Length;
double[,] samples = new double[M, 2];
for (int s = 0; s < M; s++)
{
    string[] parts = lines[s].Split(',');
    samples[s, 0] = double.Parse(parts[0], ci);
    samples[s, 1] = double.Parse(parts[1], ci);
}

int m = 4;
int iters = 10;
int[] reportAt = { 4, 10 };
string name = "lab_01.2";

double Distance(double[] a, double[] b) =>
    Math.Abs(a[0] - b[0]);

Random rnd = new Random(123);
int attrs = 2;
double[,] V = new double[m, attrs];
bool[] used = new bool[M];
for (int j = 0; j < m; j++)
{
    int s;
    do { s = rnd.Next(M); } while (used[s]);
    used[s] = true;
    for (int i = 0; i < attrs; i++) V[j, i] = samples[s, i];
}

int[] u = new int[M];

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

void Report(int it)
{
    Console.WriteLine($"--- {name}: after {it} iterations ---");
    for (int j = 0; j < m; j++)
    {
        int count = 0;
        double min1 = double.MaxValue, max1 = double.MinValue;
        double min2 = double.MaxValue, max2 = double.MinValue;
        for (int s = 0; s < M; s++)
        {
            if (u[s] != j) continue;
            count++;
            min1 = Math.Min(min1, samples[s, 0]); max1 = Math.Max(max1, samples[s, 0]);
            min2 = Math.Min(min2, samples[s, 1]); max2 = Math.Max(max2, samples[s, 1]);
        }
        string range = count == 0
            ? "empty group"
            : $"x1 [{min1.ToString("F4", ci)}; {max1.ToString("F4", ci)}], x2 [{min2.ToString("F4", ci)}; {max2.ToString("F4", ci)}]";
        Console.WriteLine($"Group {j + 1}: center ({V[j, 0].ToString("F4", ci)}; {V[j, 1].ToString("F4", ci)}), samples: {count}, {range}");
    }

    string baseName = $"{name}_iter{it}";
    string[] rows = new string[M + m + 1];
    rows[0] = "x1;x2;group";
    for (int s = 0; s < M; s++)
        rows[s + 1] = samples[s, 0].ToString(ci).Replace(".", ",") + ";" + samples[s, 1].ToString(ci).Replace(".", ",") + ";" + (u[s] + 1);
    for (int j = 0; j < m; j++)
        rows[M + j + 1] = V[j, 0].ToString(ci).Replace(".", ",") + ";" + V[j, 1].ToString(ci).Replace(".", ",") + ";center" + (j + 1);
    File.WriteAllLines(baseName + ".csv", rows);
    Console.WriteLine($"Saved {baseName}.csv");
    Console.WriteLine();
}
