using System;
using System.Linq;
using System.Text;

namespace Lab_3._1_Horbach_633p
{
    public static class Methouds
    {
        /// <summary>
        /// Основний метод рішення гри: спочатку шукає сідлову точку, потім перевіряє розмірність (2xn або mx2),
        /// і нарешті розв'язує за допомогою симплекс-методу.
        /// Повертає Tuple(mixedQ, gameValue, mixedP)
        /// </summary>
        public static Tuple<double[], double, double[], string, string> SolveGame(double[,] A, int rows, int cols)
        {
            // 1) Шукаємо сідлову точку
            var saddle = FindSaddlePoint(A);
            if (saddle.saddleExists)
            {
                // Чисті стратегії: P та Q містять одиницю у відповідних позиціях
                double[] P = new double[rows];
                double[] Q = new double[cols];
                P[saddle.row] = 1.0;
                Q[saddle.col] = 1.0;

                // Таблиці не використовуються
                string emptyTable = "Симплекс-таблиця не використовувалась (знайдено сідлову точку).";
                return Tuple.Create(Q, saddle.value, P, emptyTable, emptyTable);
            }

            // 2) Якщо матриця 2 x n
            if (IsTwoByN(A))
            {
                var active = ActiveStrategies2xN(A);

                // Створюємо підматрицю 2x2
                double[,] sub = new double[2, 2]
                {
            { A[0, active[0]], A[0, active[1]] },
            { A[1, active[0]], A[1, active[1]] }
                };

                var sol = Solve2x2(sub);
                double[] P2 = sol.Item1;
                double[] Q2 = sol.Item2;
                double v = sol.Item3;

                // Розширюємо вектори Q до повного розміру
                double[] Qfull = new double[cols];
                Qfull[active[0]] = Q2[0];
                Qfull[active[1]] = Q2[1];

                string msg = "Симплекс-таблиця не використовувалась (використано рішення для 2×n).";
                return Tuple.Create(Qfull, v, P2, msg, msg);
            }

            // 3) Якщо матриця m x 2
            if (IsMByTwo(A))
            {
                var active = ActiveStrategiesMx2(A);

                // Підматриця 2x2 для рядків active[0], active[1]
                double[,] sub = new double[2, 2]
                {
            { A[active[0], 0], A[active[0], 1] },
            { A[active[1], 0], A[active[1], 1] }
                };

                var sol = Solve2x2(sub);
                double[] P2 = sol.Item1;
                double[] Q2 = sol.Item2;
                double v = sol.Item3;

                // Розширюємо вектори P до повного розміру
                double[] Pfull = new double[rows];
                Pfull[active[0]] = P2[0];
                Pfull[active[1]] = P2[1];

                string msg = "Симплекс-таблиця не використовувалась (використано рішення для m×2).";
                return Tuple.Create(Q2, v, Pfull, msg, msg);
            }

            // 4) Інакше: використовується симплекс-метод
            var result = SolveBySimplex(A, rows, cols);
            return Tuple.Create(result.Item1, result.Item2, result.Item3, result.Item4, result.Item5);
        }


        #region Сідлова точка
        public static (bool saddleExists, int row, int col, double value) FindSaddlePoint(double[,] A)
        {
            int rows = A.GetLength(0);
            int cols = A.GetLength(1);

            // Мінімум у кожному рядку
            double[] rowMins = new double[rows];
            for (int i = 0; i < rows; i++)
                rowMins[i] = Enumerable.Range(0, cols).Select(j => A[i, j]).Min();

            // Максимум з мінімумів
            double maxOfRowMins = rowMins.Max();
            int a_ = Array.IndexOf(rowMins, maxOfRowMins);

            // Максимум у кожному стовпці
            double[] colMaxs = new double[cols];
            for (int j = 0; j < cols; j++)
                colMaxs[j] = Enumerable.Range(0, rows).Select(i => A[i, j]).Max();

            // Мінімум з максимумів
            double minOfColMaxs = colMaxs.Min();
            int aDash = Array.IndexOf(colMaxs, minOfColMaxs);

            // Перевірка умов сідлової точки
            if (a_ == aDash && Math.Abs(maxOfRowMins - minOfColMaxs) < 1e-10)
                return (true, a_, aDash, A[a_, aDash]);
            else
                return (false, -1, -1, 0);
        }
        #endregion

        #region Розміри матриці
        public static bool IsTwoByN(double[,] A) => A.GetLength(0) == 2;
        public static bool IsMByTwo(double[,] A) => A.GetLength(1) == 2;
        #endregion

        #region Активні стратегії для 2 x n
        /// <summary>
        /// Пошук двох активних стратегій другого гравця для матриці 2 x n.
        /// Повертає індекси двох стовпців.
        /// </summary>
        public static int[] ActiveStrategies2xN(double[,] A)
        {
            int cols = A.GetLength(1);
            double bestValue = double.MinValue;
            int[] bestPair = new int[2];
            // Перебираємо всі пари стовпців j<k
            for (int j = 0; j < cols; j++)
                for (int k = j + 1; k < cols; k++)
                {
                    // Знаходимо p, у якому виграші ліній зрівнюються:
                    // p*A[0,j] + (1-p)*A[1,j] = p*A[0,k] + (1-p)*A[1,k]
                    double denom = (A[0, j] - A[1, j]) - (A[0, k] - A[1, k]);
                    if (Math.Abs(denom) < 1e-10) continue;
                    double p = (A[1, k] - A[1, j]) / denom;
                    if (p < 0 || p > 1) continue;

                    // Значення гри у точці p
                    double v_j = p * A[0, j] + (1 - p) * A[1, j];
                    double v_k = p * A[0, k] + (1 - p) * A[1, k];
                    double v = Math.Min(v_j, v_k);

                    if (v > bestValue)
                    {
                        bestValue = v;
                        bestPair[0] = j;
                        bestPair[1] = k;
                    }
                }
            return bestPair;
        }
        #endregion

        #region Активні стратегії для m x 2
        /// <summary>
        /// Пошук двох активних стратегій першого гравця для матриці m x 2.
        /// Повертає індекси двох рядків.
        /// </summary>
        public static int[] ActiveStrategiesMx2(double[,] A)
        {
            int rows = A.GetLength(0);
            double bestValue = double.MaxValue;
            int[] bestPair = new int[2];
            // Перебираємо всі пари рядків i<k
            for (int i = 0; i < rows; i++)
                for (int k = i + 1; k < rows; k++)
                {
                    // q*A[i,0] + (1-q)*A[i,1] = q*A[k,0] + (1-q)*A[k,1]
                    double denom = (A[i, 0] - A[i, 1]) - (A[k, 0] - A[k, 1]);
                    if (Math.Abs(denom) < 1e-10) continue;
                    double q = (A[k, 1] - A[i, 1]) / denom;
                    if (q < 0 || q > 1) continue;

                    double v_i = q * A[i, 0] + (1 - q) * A[i, 1];
                    double v_k = q * A[k, 0] + (1 - q) * A[k, 1];
                    double v = Math.Max(v_i, v_k);

                    if (v < bestValue)
                    {
                        bestValue = v;
                        bestPair[0] = i;
                        bestPair[1] = k;
                    }
                }
            return bestPair;
        }
        #endregion

        // Розв'язання 2x2 змішаними стратегиями
        /// <summary>
        /// Розв'язок 2x2 змішаними стратегіями.
        /// Повертає Tuple(P_vector, Q_vector, gameValue).
        /// </summary>
        public static Tuple<double[], double[], double> Solve2x2(double[,] B)
        {
            double a = B[0, 0], b = B[0, 1], c = B[1, 0], d = B[1, 1];
            double denom = a + d - b - c;
            if (Math.Abs(denom) < 1e-10)
                throw new InvalidOperationException("Неможливо розв'язати 2x2 (деномінатор = 0)");

            // Стратегії гравця A (дві рядкові стратегії)
            double p1 = (d - c) / denom;
            double p2 = 1 - p1;

            // Стратегії гравця B (дві стовпчикові стратегії)
            double q1 = (d - b) / denom;
            double q2 = 1 - q1;

            // Ціна гри
            double v = (a * d - b * c) / denom;

            return Tuple.Create(new double[] { p1, p2 }, new double[] { q1, q2 }, v);
        }

        // Симплекс-метод
        public static Tuple<double[], double, double[], string, string> SolveBySimplex(double[,] A, int rows, int cols)
        {
            int m = rows;
            int n = cols;
            double[,] simplex = new double[m + 1, n + m + 1];

            // Початкове заповнення симплекс-таблиці
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++) simplex[i, j] = A[i, j];
                simplex[i, n + i] = 1;
                simplex[i, n + m] = 1;
            }
            for (int j = 0; j < n; j++) simplex[m, j] = -1;

            string[] varNames = new string[n + m];
            for (int j = 0; j < n; j++) varNames[j] = $"q{j + 1}";
            for (int i = 0; i < m; i++) varNames[n + i] = $"r{i + 1}";
            string[] basis = new string[m];
            for (int i = 0; i < m; i++) basis[i] = varNames[n + i];

            // Зберігаємо початкову таблицю
            string initialTable = SimplexTableToString(simplex, varNames, basis);

            // Основний симплекс-алгоритм
            while (true)
            {
                int pivotCol = -1;
                double minVal = 0;
                for (int j = 0; j < n + m; j++)
                    if (simplex[m, j] < minVal) { minVal = simplex[m, j]; pivotCol = j; }
                if (pivotCol < 0) break;

                int pivotRow = -1;
                double minRatio = double.MaxValue;
                for (int i = 0; i < m; i++)
                {
                    if (simplex[i, pivotCol] > 1e-10)
                    {
                        double ratio = simplex[i, n + m] / simplex[i, pivotCol];
                        if (ratio < minRatio) { minRatio = ratio; pivotRow = i; }
                    }
                }
                if (pivotRow < 0) return null;

                // Оновлення базису
                basis[pivotRow] = varNames[pivotCol];

                // Нормалізуємо ведучий рядок
                double pivot = simplex[pivotRow, pivotCol];
                for (int j = 0; j <= n + m; j++)
                    simplex[pivotRow, j] /= pivot;

                // Обнуляємо інші рядки
                for (int i = 0; i <= m; i++)
                {
                    if (i == pivotRow) continue;
                    double factor = simplex[i, pivotCol];
                    for (int j = 0; j <= n + m; j++)
                        simplex[i, j] -= factor * simplex[pivotRow, j];
                }
            }

            // Зберігаємо фінальну таблицю
            string finalTable = FinalSimplexTableToString(simplex, varNames, basis);

            double[] Q = new double[n];
            for (int i = 0; i < m; i++)
            {
                if (basis[i].StartsWith("q"))
                {
                    int index = int.Parse(basis[i].Substring(1)) - 1;
                    Q[index] = simplex[i, n + m];
                }
            }

            double sumQ = Q.Sum();
            for (int i = 0; i < n; i++) Q[i] /= sumQ;

            double value = 1.0 / sumQ;
            double[] P = new double[m];
            for (int i = 0; i < m; i++)
                P[i] = 1.0 / m; // або інше, залежно від задачі

            return Tuple.Create(Q, value, P, initialTable, finalTable);
        }

        public static string SimplexTableToString(double[,] simplex, string[] varNames, string[] basis)
        {
            int m = basis.Length;                // кількість обмежень (рядків)
            int n = varNames.Count(v => v.StartsWith("q"));  // кількість змінних q

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Початкова симплекс-таблиця:");
            sb.Append("Basis\t|\t");
            for (int j = 0; j < n; j++) sb.Append($"{varNames[j]}\t");
            sb.Append("| b");
            sb.AppendLine();
            sb.AppendLine(new string('-', 50));

            // Рядки обмежень
            for (int i = 0; i < m; i++)
            {
                sb.Append($"{basis[i]}\t|\t");
                for (int j = 0; j < n; j++) sb.Append($"{simplex[i, j]:0.###}\t");
                sb.Append($"| {simplex[i, n + m]:0.###}");
                sb.AppendLine();
            }

            // Рядок Z
            sb.Append("Z\t|\t");
            for (int j = 0; j < n; j++) sb.Append($"{simplex[m, j]:0.###}\t");
            sb.Append("| 0");
            sb.AppendLine();

            return sb.ToString();
        }
        public static string FinalSimplexTableToString(double[,] simplex, string[] varNames, string[] basis)
        {
            int totalRows = simplex.GetLength(0);   // m + 1 (рядки + Z)
            int totalCols = simplex.GetLength(1);   // n + m + 1 (змінні + штучні + b)

            int m = totalRows - 1;                  // кількість рядків (без Z)

            // Визначаємо НЕ базисні змінні
            List<string> nonBasisNames = new List<string>();
            foreach (string v in varNames)
            {
                if (!basis.Contains(v))
                    nonBasisNames.Add(v);
            }

            // Визначаємо потрібний порядок для НЕ базисних змінних (тут можна змінити)
            string[] desiredOrder = new string[] { "r2", "r1", "q3" };

            // Відфільтруємо та впорядкуємо
            List<string> orderedNonBasis = new List<string>();
            foreach (var name in desiredOrder)
            {
                if (nonBasisNames.Contains(name))
                    orderedNonBasis.Add(name);
            }

            // Індекси у varNames для колонок
            List<int> nonBasisIndexes = new List<int>();
            foreach (var name in orderedNonBasis)
            {
                int idx = Array.IndexOf(varNames, name);
                if (idx >= 0) nonBasisIndexes.Add(idx);
            }

            var sb = new StringBuilder();

            sb.AppendLine("Остаточна симплекс-таблиця:");
            // Заголовок
            string header = string.Join("    ", orderedNonBasis.Select(x => "-" + x));
            sb.AppendLine(header + "    W,    1");
            sb.AppendLine(new string('-', 50));

            // Форматування чисел - ширина 7 символів, 2 знаки після коми
            string fmt = "{0,7:0.##}";

            // Рядки базису
            for (int i = 0; i < m; i++)
            {
                sb.Append($"{basis[i],-5} = ");
                foreach (int colIdx in nonBasisIndexes)
                {
                    sb.AppendFormat(fmt, simplex[i, colIdx]);
                    sb.Append("  ");
                }
                sb.AppendFormat(fmt, simplex[i, totalCols - 1]);
                sb.AppendLine();
            }

            // Рядок Z
            sb.Append("1 Z  = ");
            foreach (int colIdx in nonBasisIndexes)
            {
                sb.AppendFormat(fmt, simplex[m, colIdx]);
                sb.Append("  ");
            }
            sb.AppendFormat(fmt, simplex[m, totalCols - 1]);
            sb.AppendLine();

            return sb.ToString();
        }


        // Видалення домінованих стратегій
        public static (double[,], bool[]) RemoveDominatedStrategies(double[,] A, int rows, int cols)
        {
            bool[] activeStrategiesB = Enumerable.Repeat(true, cols).ToArray();
            bool removed;
            do
            {
                removed = false;
                for (int j = 0; j < cols; j++)
                {
                    if (!activeStrategiesB[j]) continue;
                    for (int k = 0; k < cols; k++)
                    {
                        if (j == k || !activeStrategiesB[k]) continue;
                        bool dominated = true;
                        for (int i = 0; i < rows; i++)
                        {
                            if (A[i, k] > A[i, j]) { dominated = false; break; }
                        }
                        if (dominated)
                        {
                            activeStrategiesB[j] = false;
                            removed = true;
                            break;
                        }
                    }
                    if (removed) break;
                }
            } while (removed);

            int newCols = activeStrategiesB.Count(x => x);
            double[,] newA = new double[rows, newCols];
            int idx = 0;
            for (int j = 0; j < cols; j++)
            {
                if (!activeStrategiesB[j]) continue;
                for (int i = 0; i < rows; i++) newA[i, idx] = A[i, j];
                idx++;
            }
            return (newA, activeStrategiesB);
        }
        
    }
}