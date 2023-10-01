using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp2
{
    class GameForm
    {
        public Form Form { get; private set; }
        private TableLayoutPanel tableLayoutPanel;//Для расположения каждого элемента из матрицы в виде ячеек таблицы
        private int[,] matrix; // создаем матрицу в виде двойного массива 

        public void Initialize()
        {
            InitializeMatrix(); // Создаем матрицу, заполняем её рандомными цифрами от 1 до 9 
            InitializeForm(); // Создаем окно для самой игры, прописываем его размер и название
            InitializeTableLayoutPanel();// Создаем таблицу 7х4
            PopulateTableLayoutPanel(); // Добавляем элементы управления в таблицу, даем возможность кликать на цифры в таблице
        }

        private void InitializeMatrix()
        {
            Random random = new Random();
            matrix = new int[4, 7];

            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    matrix[row, col] = random.Next(1, 9);
                }
            }
        }

        private void InitializeForm()
        {
            Form = new Form();
            Form.Size = new System.Drawing.Size(400, 300);
            Form.BackColor = System.Drawing.Color.White;
            Form.Text = "Цифреки онлайн без смс и регистрации";
        }
        /// <summary>
        /// Создает таблицу 7х4
        /// </summary>
        private void InitializeTableLayoutPanel()
        {
            tableLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel.Dock = DockStyle.Top;
            Form.Controls.Add(tableLayoutPanel);

            tableLayoutPanel.RowCount = 4;
            tableLayoutPanel.ColumnCount = 7;
        }

        private void PopulateTableLayoutPanel()
        {
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    int digit = matrix[row, col];

                    Label label = new Label();
                    label.Text = digit.ToString(); // Переносим матрицу в текстовых формат для label.Text для дальнейших манипуляций 
                    SetLabelColor(label); // Устанавливаем разные цвета для цифр

                    label.AutoSize = true;
                    tableLayoutPanel.Controls.Add(label, col, row); // Делаем цифры кликабельными с помощью элементов управления label

                    label.Click += new EventHandler(label_Click); // "Переопределяем" event Click, чтобы он позволял кликать на 2 разных цифры в таблице 
                }
            }
        }

        private void SetLabelColor(Label label)
        {
            switch (label.Text)
            {
                case "1":
                    label.ForeColor = System.Drawing.Color.Red;
                    break;
                case "2":
                    label.ForeColor = System.Drawing.Color.Blue;
                    break;
                case "3":
                    label.ForeColor = System.Drawing.Color.Green;
                    break;
                case "4":
                    label.ForeColor = System.Drawing.Color.Yellow;
                    break;
                case "5":
                    label.ForeColor = System.Drawing.Color.Pink;
                    break;
                case "6":
                    label.ForeColor = System.Drawing.Color.DeepPink;
                    break;
                case "7":
                    label.ForeColor = System.Drawing.Color.LightBlue;
                    break;
                case "8":
                    label.ForeColor = System.Drawing.Color.LightCoral;
                    break;
                case "9":
                    label.ForeColor = System.Drawing.Color.GreenYellow;
                    break;
            }
        }

        private void label_Click(object sender, EventArgs e)
        {
            // Обработчик события двойного клика на Label
            Label label = (Label)sender;
            clickCount++;

            if (clickCount == 1)
            {
                firstClickText = label.Text;
                Console.WriteLine($"Первый клик сделан по {label}");
            }
            else if (clickCount == 2)
            {
                secondClickText = label.Text;
                Console.WriteLine($"Второй клик сделан по {label}");
            }

            if (firstClickText == secondClickText)
            {
                Console.WriteLine("Оба числа совпали");
                // Обнуление
                clickCount = 0;
                firstClickText = "";
                secondClickText = "";
            }
            else if (firstClickText != secondClickText && firstClickText != "" && secondClickText != "")
            {
                Console.WriteLine("Числа не совпали");
                // Обнуление.
                clickCount = 0;
                firstClickText = "";
                secondClickText = "";
            }
        }

        private static int clickCount = 0;
        private static string firstClickText = "";
        private static string secondClickText = "";
    }
}
