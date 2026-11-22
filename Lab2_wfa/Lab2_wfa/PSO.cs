
namespace Lab2_wfa;

public record IterationResultPSO(int Iteration, double[][] Individuals, 
    double[] xGlob, double[] XGlob, double FGlob);

public class PSO
{
    private static Random random = new();

    public static List<IterationResultPSO> GetIterationResults(
        int N, double rIn, double rGlob, double rLok,
        double[] xMin, double[] xMax, int amountIterations)
    {
        var result = new List<IterationResultPSO>();

        double[][] nIndividuals = new double[N][];
        double[][] nXLok = new double[N][];

        for (int i = 0; i < N; i++)
        {
            nIndividuals[i] = new double[xMin.Length];
            nXLok[i] = new double[xMax.Length];
        }

        for (int j = 0; j < N; j++)
        {
            for (int i = 0; i < xMin.Length; i++)
            {
                var r = RandomInRange(xMin[i], xMax[i]);
                nIndividuals[j][i] = r;
                nXLok[j][i] = r;
            }
        }

        return result;
    }
    
    static double RandomInRange(double min, double max)
    {
        return random.NextDouble() * (max - min) + min;
    }
}