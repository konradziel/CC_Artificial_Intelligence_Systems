using System.Security.Cryptography.X509Certificates;

namespace Lab2_wfa;

public record IterationResultPSO(int Iteration, double[][] Individuals, 
    double[] xGlob, double[] XGlob, double FGlob);

public class PSO
{
    private static Random random = new();

    public static List<IterationResultPSO> GetIterationResults(
        int N, double rIn, double rGlob, double rLok,
        double[] xMin, double[] xMac, int amountIterations)
    {
        var result = new List<IterationResultPSO>();


        return result;
    }
}