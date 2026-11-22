
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
        var amountOfAtributes = xMin.Length;

        double[][] nIndividuals = CreateTable(N, amountOfAtributes);
        double[][] nXLok = CreateTable(N, amountOfAtributes);
        double[][] speed = CreateTable(N, amountOfAtributes);

        for (int j = 0; j < N; j++)
        {
            for (int i = 0; i < amountOfAtributes; i++)
            {
                var r = RandomInRange(xMin[i], xMax[i]);
                nIndividuals[j][i] = r;
                nXLok[j][i] = r;
            }
        }

        for (int i = 0; i < amountIterations; i++)
        {
            double[] scores = new double[N];

            for (int j = 0; j < N; j++)
            {
                scores[j] = F(nIndividuals[j]);
            }
        }
        
        return result;
    }
    
    static double RandomInRange(double min, double max)
    {
        return random.NextDouble() * (max - min) + min;
    }

    static double[][] CreateTable(int rows, int cols)
    {
        var table = new double[rows][];
        for (int i = 0; i < rows; i++)
        {
            table[i] = new double[cols];
        }

        return table;
    }

    static double F(double[] individual)
    {
        double x1 = individual[0];
        double x2 = individual[1];
        
        return Math.Sin(x1 * 0.05) + Math.Sin(x2 * 0.05) +
            0.4 * Math.Sin(x1 * 0.15) * Math.Sin(x2 * 0.15);
    }
}