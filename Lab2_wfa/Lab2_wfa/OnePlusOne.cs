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

    public static List<IterationResult> GetIterandrationResults(
        double spread, double increaseSpeed, int amountIterations,
        double rangeMin, double rangeMax)
    {
        var result = new List<IterationResult>();

        double x = RandomInRange(rangeMin, rangeMax);
        double y = Function(x);
        result.Add(new IterationResult(0, x, y, spread));

        for (int i = 1; i <= amountIterations; i++)
        {
            var xNext = x + RandomInRange(-rangeMin, rangeMax);

            while (xNext < -rangeMin || xNext > rangeMax)
            {
                xNext += RandomInRange(-rangeMin/2, rangeMax/2);
            }
            
            var yNext = Function(xNext);

            if (yNext >= y)
            {
                x = xNext;
                y = yNext;
                spread *= increaseSpeed;
            }
            else
            {
                spread /= increaseSpeed;
            }
            
            result.Add(new IterationResult(i, x, y, spread));
        }

        return result;
    }
}