using System;
using System.Linq;

namespace Lab_3._1_Horbach_633p
{
    public class Methouds
    {
        // Видалення домінованих стратегій
        public static (double[,], bool[]) RemoveDominatedStrategies(double[,] A, int rows, int cols)
        {
            bool[] activeStrategiesB = new bool[cols];
            for (int i = 0; i < cols; i++)
                activeStrategiesB[i] = true;

            bool removed;
            do
            {
                removed = false;
                for (int j = 0; j < cols; j++)
                {
                    if (!activeStrategiesB[j]) continue;
                    for (int k = 0; k < cols; k++)
                    {
                        if (j != k && activeStrategiesB[k])
                        {
                            bool dominated = true;
                            for (int i = 0; i < rows; i++)
                            {
                                if (A[i, k] > A[i, j])
                                {
                                    dominated = false;
                                    break;
                                }
                            }
                            if (dominated)
                            {
                                activeStrategiesB[j] = false;
                                removed = true;
                                break;
                            }
                        }
                    }
                    if (removed) break;
                }
            } while (removed);

            int newCols = activeStrategiesB.Count(a => a);
            double[,] newA = new double[rows, newCols];
            int colIndex = 0;
            for (int j = 0; j < cols; j++)
            {
                if (activeStrategiesB[j])
                {
                    for (int i = 0; i < rows; i++)
                        newA[i, colIndex] = A[i, j];
                    colIndex++;
                }
            }

            return (newA, activeStrategiesB);
        }

        // Симплекс-метод
        public static Tuple<double[], double, double[]> SolveGame(double[,] A, int rows, int cols)
        {
            double[,] simplex = new double[rows + 1, cols + rows + 1];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                    simplex[i, j] = A[i, j];
                simplex[i, cols + i] = 1;
                simplex[i, cols + rows] = 1;
            }

            for (int j = 0; j < cols; j++)
                simplex[rows, j] = -1;

            string[] varNames = new string[cols + rows];
            for (int i = 0; i < cols; i++) varNames[i] = $"q{i + 1}";
            for (int i = 0; i < rows; i++) varNames[cols + i] = $"r{i + 1}";

            string[] basis = new string[rows];
            for (int i = 0; i < rows; i++) basis[i] = $"r{i + 1}";

            while (true)
            {
                int pivotCol = -1;
                double minVal = 0;
                for (int j = 0; j < cols + rows; j++)
                {
                    if (simplex[rows, j] < minVal)
                    {
                        minVal = simplex[rows, j];
                        pivotCol = j;
                    }
                }
                if (pivotCol == -1) break;

                int pivotRow = -1;
                double minRatio = double.MaxValue;
                for (int i = 0; i < rows; i++)
                {
                    if (simplex[i, pivotCol] > 1e-10)
                    {
                        double ratio = simplex[i, cols + rows] / simplex[i, pivotCol];
                        if (ratio < minRatio)
                        {
                            minRatio = ratio;
                            pivotRow = i;
                        }
                    }
                }

                if (pivotRow == -1)
                    return null;

                double pivot = simplex[pivotRow, pivotCol];
                for (int j = 0; j <= cols + rows; j++)
                    simplex[pivotRow, j] /= pivot;

                for (int i = 0; i <= rows; i++)
                {
                    if (i == pivotRow) continue;
                    double factor = simplex[i, pivotCol];
                    for (int j = 0; j <= cols + rows; j++)
                        simplex[i, j] -= factor * simplex[pivotRow, j];
                }

                basis[pivotRow] = varNames[pivotCol];
            }

            double[] q = new double[cols];
            for (int i = 0; i < cols; i++) q[i] = 0;

            for (int i = 0; i < rows; i++)
            {
                if (basis[i].StartsWith("q"))
                {
                    int varIndex = int.Parse(basis[i].Substring(1)) - 1;
                    q[varIndex] = simplex[i, cols + rows];
                }
            }

            double sumQ = q.Sum();
            if (sumQ == 0)
                return null;

            double[] mixedQ = q.Select(x => x / sumQ).ToArray();
            double gameValue = 1.0 / sumQ;

            double[] p = new double[rows];
            for (int i = 0; i < rows; i++)
                p[i] = simplex[rows, cols + i];

            for (int i = 0; i < rows; i++)
                if (p[i] < 0) p[i] = 0;

            double sumP = p.Sum();
            if (sumP > 0)
                for (int i = 0; i < rows; i++)
                    p[i] /= sumP;

            return Tuple.Create(mixedQ, gameValue, p);
        }

    }
}
       
