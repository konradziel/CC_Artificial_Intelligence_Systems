namespace Lab2_wfa;

public record IterationResult(int Iteration, double x, double y, double Spread);


public class OnePlusOne
{
    private static Random random = new();

    static double RandomInRange(double min, double max)
    {
        return random.NextDouble() * (max - min) + min;
    }
    
    public static double Function(double x)
    {
        return Math.Sin(x / 10.0) * Math.Sin(x / 200);
    }

    public static List<IterationResult> GetIterationResults(
        double spread, double increaseSpeed, int amountIterations,
        double rangeMin, double rangeMax)
    {
        var result = new List<IterationResult>();



        return result;
    }
}