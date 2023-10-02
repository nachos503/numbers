using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

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

        private Label firstClickedLabel = null;
        private Label secondClickedLabel = null;
        private int sum = 0;
        private Point previous_Click;//класс для работы с координатами
        private void label_Click(object sender, EventArgs e)
        {

            // Обработчик события двойного клика на Label
            Label label = (Label)sender;
            MouseEventArgs mouseArgs = e as MouseEventArgs;//преобразуем объект e типа EventArgs к типу MouseEventArgs с помощью оператора as
            if (firstClickedLabel == null || firstClickedLabel.Text == "  ")
            {
                firstClickedLabel = label;
                Console.WriteLine($"Первый клик сделан по {label.Text}");
            }
            else if (firstClickedLabel != label)
            {
                secondClickedLabel = label;
                Console.WriteLine($"Второй клик сделан по {label.Text}");
                if (firstClickedLabel.Text == secondClickedLabel.Text && mouseArgs.Location == previous_Click && mouseArgs != null && previous_Click != null) ///проверка на одинаковые числа
                {

                    clickCount = 0;
                    return;
                }
                previous_Click = mouseArgs.Location; // запоминаем координату клика 

                if (firstClickedLabel.Text == secondClickedLabel.Text)
                {
                    Console.WriteLine("Оба числа совпали");
                    // Удаление двух Label
                    firstClickedLabel.Text = "  ";
                    secondClickedLabel.Text = "  ";

                    // Проверка и удаление строки, если все ячейки в строке "  "
                    // Получаем родительский контейнер (TableLayoutPanel) для firstClickedLabel
                    TableLayoutPanel tableLayoutPanel = firstClickedLabel.Parent as TableLayoutPanel;

                    // Определяем индекс строки, в которой находится firstClickedLabel
                    int rowIndex = tableLayoutPanel.GetRow(firstClickedLabel);

                    // Устанавливаем флаг, предполагая, что все ячейки в строке пусты
                    bool allEmpty = true;

                    // Перебираем все элементы в Controls контейнера tableLayoutPanel
                    foreach (Control control in tableLayoutPanel.Controls)
                    {
                        // Проверяем, находится ли control в той же строке, что и firstClickedLabel, и не содержит ли control текст, отличный от "  "
                        if (tableLayoutPanel.GetRow(control) == rowIndex && control.Text != "  ")
                        {
                            // Если хотя бы одна ячейка не пуста, устанавливаем флаг в false и выходим из цикла
                            allEmpty = false;
                            break;
                        }
                    }

                    // Если флаг allEmpty остался true, это значит, что все ячейки в строке были пустыми
                    if (allEmpty)
                    {
                        // Перебираем все столбцы в строке
                        for (int columnIndex = 0; columnIndex < tableLayoutPanel.ColumnCount; columnIndex++)
                        {
                            // Получаем элемент в текущей ячейке
                            Control control = tableLayoutPanel.GetControlFromPosition(columnIndex, rowIndex);

                            // Проверяем, существует ли элемент в ячейке
                            if (control != null)
                            {
                                // Удаляем элемент из контейнера tableLayoutPanel
                                tableLayoutPanel.Controls.Remove(control);

                                // Уничтожаем элемент
                                control.Dispose();
                                firstClickedLabel = null;
                                secondClickedLabel = null;
                                break;
                            }
                        }
                    }
                   
                }
                if (firstClickedLabel.Text != secondClickedLabel.Text)
                {
                    if (firstClickedLabel.Text != " " && secondClickedLabel.Text != " ")
                    {
                        sum = 0;
                        int num1 = int.Parse(firstClickedLabel.Text);
                        int num2 = int.Parse(secondClickedLabel.Text);
                        sum += num1 + num2;
                        if (sum == 10)//провеярет числа на суму 10
                        {
                            Console.WriteLine("Сумма чисел равна 10");
                            clickCount = 0;
                            //firstClickText = "";
                            // secondClickText = "";

                        }
                        else
                        {
                            Console.WriteLine("Числа не совпали");
                        }

                        // Сброс состояния
                        firstClickedLabel = null;
                        secondClickedLabel = null;
                    }
                }
            }
        }

        private static int clickCount = 0;
        private static string firstClickText = "";
        private static string secondClickText = "";
    }
}
