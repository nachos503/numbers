using System;
using System.Windows.Forms;
using System.Drawing;

namespace WindowsFormsApp2
{
    class GameForm
    {
        public Form Form { get; private set; }
        private TableLayoutPanel tableLayoutPanel;
        private int[,] matrix;

        public void Initialize()
        {
            InitializeMatrix();
            InitializeForm();
            InitializeTableLayoutPanel();
            PopulateTableLayoutPanel();
        }

        private void InitializeMatrix()
        {
            Random random = new Random();
            matrix = new int[4, 7];

            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    matrix[row, col] = random.Next(1, 10);
                }
            }
        }

        private void InitializeForm()
        {
            Form = new Form();
            Form.Size = new System.Drawing.Size(400, 300);
            Form.BackColor = System.Drawing.Color.White;
            Form.Text = "Циферки онлайн без смс и регистрации";
        }

        private void InitializeTableLayoutPanel()
        {
            tableLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel.Dock = DockStyle.Fill;
            Form.Controls.Add(tableLayoutPanel);

            tableLayoutPanel.RowCount = 4;
            tableLayoutPanel.ColumnCount = 7;

            // Устанавливаем минимальную ширину для каждого столбца
            for (int col = 0; col < tableLayoutPanel.ColumnCount; col++)
            {
                tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30));
            }
        }

        private void PopulateTableLayoutPanel()
        {
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    int digit = matrix[row, col];

                    Label label = new Label();
                    label.Text = digit.ToString();
                    SetLabelColor(label);

                    label.AutoSize = true;
                    label.TextAlign = ContentAlignment.MiddleCenter;
                    label.Font = new Font("Arial", 16, FontStyle.Bold);

                    tableLayoutPanel.Controls.Add(label, col, row);

                    label.Click += new EventHandler(label_Click);
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
        private Point previous_Click;//класс для работы с координатами
        private int sum = 0;
        private void label_Click(object sender, EventArgs e)
        {
            Label label = (Label)sender;
            MouseEventArgs mouseArgs = e as MouseEventArgs;//преобразуем объект e типа EventArgs к типу MouseEventArgs с помощью оператора as
            if (firstClickedLabel == null || firstClickedLabel.Visible == false)
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
                    return;
                }
                previous_Click = mouseArgs.Location; // запоминаем координату клика 

                if (firstClickedLabel.Text == secondClickedLabel.Text)
                {
                    Console.WriteLine("Оба числа совпали");
                    // Скрытие двух Label
                    firstClickedLabel.Visible = false;
                    secondClickedLabel.Visible = false;

                    // Проверка и удаление строки, если все ячейки в строке "  "
                    // Получаем родительский контейнер (TableLayoutPanel) для firstClickedLabel
                    TableLayoutPanel tableLayoutPanel = firstClickedLabel.Parent as TableLayoutPanel;

                    CheckAndHideEmptyRows();
                }
                else if (firstClickedLabel.Text != secondClickedLabel.Text)
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
                           firstClickedLabel.Visible =false;
                           secondClickedLabel.Visible =false;   

                        }
                        else
                        {
                            Console.WriteLine("Числа не совпали");
                        }
                        firstClickedLabel = null;
                        secondClickedLabel = null;
                    }
                }
            }
        }

        private void CheckAndHideEmptyRows()
        {
            for (int row = 0; row < tableLayoutPanel.RowCount; row++)
            {
                bool rowIsEmpty = true;
                for (int col = 0; col < tableLayoutPanel.ColumnCount; col++)
                {
                    Control control = tableLayoutPanel.GetControlFromPosition(col, row);
                    if (control != null && control.Visible)
                    {
                        rowIsEmpty = false;
                        break;
                    }
                }

                if (rowIsEmpty)
                {
                    HideRowElements(row);
                }
            }
        }

        private void HideRowElements(int rowIndex)
        {
            for (int col = 0; col < tableLayoutPanel.ColumnCount; col++)
            {
                Control control = tableLayoutPanel.GetControlFromPosition(col, rowIndex);
                if (control != null)
                {
                    control.Visible = false;
                }
            }
        }

    }
}

