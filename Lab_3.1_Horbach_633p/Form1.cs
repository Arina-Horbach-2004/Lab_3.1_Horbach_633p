using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Lab_3._1_Horbach_633p
{
    public partial class Lab_3_1_Horbach_633p : Form
    {
        private double[] mixedP;    // Змішані стратегії 1-го гравця
        private double[] mixedQ;    // Змішані стратегії 2-го гравця
        private double gameValue;   // Ціна гри

        public Lab_3_1_Horbach_633p()
        {
            InitializeComponent();
        }

        private void button_result_Click(object sender, EventArgs e)
        {
            string input = textBox_matrix.Text.Trim();
            string[] rows = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            int rowCount = rows.Length;
            string[] firstRowElements = rows[0].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            int colCount = firstRowElements.Length;

            if (!int.TryParse(textBox_rounds.Text.Trim(), out int rounds))
            {
                MessageBox.Show("Введіть коректне число раундів!");
                return;
            }

            double[,] A = new double[rowCount, colCount];
            for (int i = 0; i < rowCount; i++)
            {
                string[] elements = rows[i].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < colCount; j++)
                    A[i, j] = double.Parse(elements[j]);
            }

            double adjustment = 0.0;
            double minValue = A.Cast<double>().Where(x => x < 0).DefaultIfEmpty(double.MaxValue).Min();
            if (minValue < 0)
            {
                adjustment = Math.Abs(minValue);
                for (int i = 0; i < rowCount; i++)
                    for (int j = 0; j < colCount; j++)
                        A[i, j] += adjustment;
            }

            bool[] activeStrategiesB = new bool[colCount];
            for (int i = 0; i < colCount; i++) activeStrategiesB[i] = true;

            var removeResult = Methouds.RemoveDominatedStrategies(A, rowCount, colCount);
            A = removeResult.Item1;
            activeStrategiesB = removeResult.Item2;
            colCount = A.GetLength(1);

            var result = Methouds.SolveGame(A, rowCount, colCount);
            if (result == null)
            {
                MessageBox.Show("Розв’язок неможливий.");
                return;
            }

            mixedQ = result.Item1;
            gameValue = result.Item2 - adjustment;
            mixedP = result.Item3;

            // Очищення текстових полів перед виводом
            textBox_A.Clear();
            textBox_B.Clear();
            textBox_cost.Clear();

            string pString = "(" + string.Join("; ", mixedP.Select(x => x.ToString("0.##").Replace('.', ','))) + ")";
            textBox_A.AppendText($"{pString}");

            double[] fullMixedQ = new double[activeStrategiesB.Length];
            int idxNew = 0;
            for (int i = 0; i < activeStrategiesB.Length; i++)
                fullMixedQ[i] = activeStrategiesB[i] ? mixedQ[idxNew++] : 0.0;

            string qString = "(" + string.Join("; ", fullMixedQ.Select(x => x.ToString("0.##").Replace('.', ','))) + ")";
            textBox_B.AppendText($"{qString}");

            textBox_cost.AppendText($"{gameValue:0.00}");
        }

        private void button_modeling_Click(object sender, EventArgs e)
        {
            if (mixedP == null || mixedQ == null)
            {
                MessageBox.Show("Спочатку натисніть кнопку 'Результат', щоб отримати змішані стратегії.");
                return;
            }

            int rounds;
            if (!int.TryParse(textBox_rounds.Text.Trim(), out rounds))
            {
                MessageBox.Show("Введіть коректне число раундів!");
                return;
            }

            Random rnd = new Random();
            double accumulatedWin = 0;
            StringBuilder sb = new StringBuilder();

            // Заголовок таблиці (колонки)
            sb.AppendLine("Номер партії\tВипадкове число гравця А\tСтратегія гравця А\tВипадкове число гравця В\tСтратегія гравця В\tВиграш А\tНакопичений виграш А\tСередній виграш А");

            double[,] matrix = ParseMatrixFromTextBox();

            for (int i = 1; i <= rounds; i++)
            {
                double randA = rnd.NextDouble();
                double randB = rnd.NextDouble();

                int stratA = GetStrategyFromMixed(mixedP, randA);
                int stratB = GetStrategyFromMixed(mixedQ, randB);

                double winA = matrix[stratA, stratB];

                accumulatedWin += winA;
                double avgWin = accumulatedWin / i;

                sb.AppendLine($"{i}\t{randA:0.0000}\t{stratA + 1}\t{randB:0.0000}\t{stratB + 1}\t{winA:0.00}\t{accumulatedWin:0.00}\t{avgWin:0.00}");
            }

            sb.AppendLine(); // Порожній рядок для розділення

            // Додаємо змішані стратегії та ціну гри в кінці файлу
            string pString = "(" + string.Join("; ", mixedP.Select(x => x.ToString("0.##").Replace('.', ','))) + ")";
            string qString = "(" + string.Join("; ", mixedQ.Select(x => x.ToString("0.##").Replace('.', ','))) + ")";
            string gameValueStr = gameValue.ToString("0.00").Replace('.', ',');

            sb.AppendLine($"Змішані стратегії 1-го гравця: {pString}");
            sb.AppendLine($"Змішані стратегії 2-го гравця: {qString}");
            sb.AppendLine($"Ціна гри: {gameValueStr}");

            string filePath = @"C:\Users\Arina Gorbach\Desktop\modeling_result.txt";
            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);

            MessageBox.Show($"Результати моделювання збережено у файл:\n{filePath}", "Готово");
        }

        private int GetStrategyFromMixed(double[] mixedStrategy, double randomValue)
        {
            double cumulative = 0;
            for (int i = 0; i < mixedStrategy.Length; i++)
            {
                cumulative += mixedStrategy[i];
                if (randomValue <= cumulative)
                    return i;
            }
            return mixedStrategy.Length - 1; // На випадок округлень
        }

        private double[,] ParseMatrixFromTextBox()
        {
            string input = textBox_matrix.Text.Trim();
            string[] rows = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            int rowCount = rows.Length;
            string[] firstRowElements = rows[0].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            int colCount = firstRowElements.Length;

            double[,] matrix = new double[rowCount, colCount];

            for (int i = 0; i < rowCount; i++)
            {
                string[] elements = rows[i].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < colCount; j++)
                    matrix[i, j] = double.Parse(elements[j]);
            }

            return matrix;
        }

        private void button_example1_Click(object sender, EventArgs e)
        {
            textBox_matrix.Text = "5 2 7\r\n1 4 3\r\n6 1 5";
            textBox_rounds.Text = "50";
        }

        private void button_example2_Click(object sender, EventArgs e)
        {
            textBox_matrix.Text = "2 -1 3 3\r\n-1 2 2 7\r\n1 1 1 2";
            textBox_rounds.Text = "50";
        }

        private void button_example3_Click(object sender, EventArgs e)
        {
            textBox_matrix.Text = "3 2 6 9\r\n10 8 1 3";
            textBox_rounds.Text = "50";
        }

        private void button_protocol_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            string input = textBox_matrix.Text.Trim();
            string[] rows = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            int rowCount = rows.Length;
            int colCount = rows[0].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;

            double[,] A = new double[rowCount, colCount];

            for (int i = 0; i < rowCount; i++)
            {
                var elements = rows[i].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < colCount; j++)
                    A[i, j] = double.Parse(elements[j]);
            }

           // ---Виведення матриці-- -
           sb.AppendLine("Згенерований протокол обчислення:");
            sb.AppendLine("Матриця A:");
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                    sb.Append($"{A[i, j]:0.##} ");
                sb.AppendLine();
            }

            //---Сідлова точка-- -
           double minInRows = double.MaxValue;
            int minRow = -1, minCol = -1;

            for (int i = 0; i < rowCount; i++)
            {
                double rowMin = A[i, 0];
                int colIndex = 0;
                for (int j = 1; j < colCount; j++)
                {
                    if (A[i, j] < rowMin)
                    {
                        rowMin = A[i, j];
                        colIndex = j;
                    }
                }

                bool isSaddlePoint = true;
                for (int k = 0; k < rowCount; k++)
                {
                    if (A[k, colIndex] > rowMin)
                    {
                        isSaddlePoint = false;
                        break;
                    }
                }

                if (isSaddlePoint)
                {
                    minRow = i;
                    minCol = colIndex;
                    minInRows = rowMin;
                    break;
                }
            }

            sb.AppendLine("Пошук сідлової точки:");
            if (minRow != -1)
            {
                sb.AppendLine($"Знайдено сідлову точку: A[{minRow + 1}, {minCol + 1}] = {A[minRow, minCol]}");
            }
            else
            {
                double lowerValue = Enumerable.Range(0, rowCount).Select(i => Enumerable.Range(0, colCount).Select(j => A[i, j]).Min()).Max();
                double upperValue = Enumerable.Range(0, colCount).Select(j => Enumerable.Range(0, rowCount).Select(i => A[i, j]).Max()).Min();

                sb.AppendLine($"Знайдено нижню ціну гри: {lowerValue}");
                sb.AppendLine($"Знайдено верхню ціну гри: {upperValue}");
                sb.AppendLine("Сідлову точку не знайдено...");
                sb.AppendLine("Розв’язання матричної гри симплекс-методом...");
            }

            //---Постановка задач-- -
           sb.AppendLine();
            sb.AppendLine("Постановка прямої задачі:");
            sb.Append("Z = ");
            for (int j = 0; j < colCount; j++)
            {
                sb.Append($"q{j + 1} ");
                if (j < colCount - 1) sb.Append("+ ");
            }
            sb.AppendLine("-> max");
            sb.AppendLine("при обмеженнях:");

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    sb.Append($"{A[i, j]:0.00} * q{j + 1}");
                    if (j < colCount - 1) sb.Append(" + ");
                }
                sb.AppendLine(" <= 1");
            }
            sb.AppendLine("q1, q2, ..., qn >= 0");

            sb.AppendLine();
            sb.AppendLine("Постановка двоїстої задачі:");
            sb.Append("W = ");
            for (int i = 0; i < rowCount; i++)
            {
                sb.Append($"p{i + 1} ");
                if (i < rowCount - 1) sb.Append("+ ");
            }
            sb.AppendLine("-> min");
            sb.AppendLine("при обмеженнях:");

            for (int j = 0; j < colCount; j++)
            {
                for (int i = 0; i < rowCount; i++)
                {
                    sb.Append($"{A[i, j]:0.00} * p{i + 1}");
                    if (i < rowCount - 1) sb.Append(" + ");
                }
                sb.AppendLine(" >= 1");
            }
            sb.AppendLine("p1, p2, ..., pm >= 0");

            //---Симплексний метод-- -
           var result = Methouds.SolveGame(A, rowCount, colCount);
            if (result == null)
            {
                sb.AppendLine("Неможливо розв’язати гру (помилка симплекс-методу).");
                MessageBox.Show(sb.ToString(), "Протокол");
                return;
            }

            mixedQ = result.Item1;
            gameValue = result.Item2;
            mixedP = result.Item3;

            sb.AppendLine();
            sb.AppendLine("Розрахунок змішаних стратегій...");

            sb.AppendLine("Стратегії 1-го гравця:");
            sb.AppendLine(string.Join("; ", mixedP.Select(x => x.ToString("0.00").Replace('.', ','))));

            sb.AppendLine("Стратегії 2-го гравця:");
            sb.AppendLine(string.Join("; ", mixedQ.Select(x => x.ToString("0.00").Replace('.', ','))));

            sb.AppendLine($"Остаточна ціна гри: {gameValue:0.00}".Replace('.', ','));
            //Збереження в файл(опційно):
            string filePath = @"C:\Users\Arina Gorbach\Desktop\Lab_3.1_Horbach_633p.txt";
            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
            MessageBox.Show($"Протокол обчислення збережено у файл:\n{filePath}", "Готово");
        }

    }
}
