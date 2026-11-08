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
    public static double Distance(double[] a, double[] b)
    {
        return Math.Sqrt((a[0] - b[0]) * (a[0] - b[0]) + (a[1] - b[1]) * (a[1] - b[1]));
    }

    public static double CalculateX1(double[] a, double[] b)
    {
        return Math.Abs(a[0] - b[0]);
    }
}