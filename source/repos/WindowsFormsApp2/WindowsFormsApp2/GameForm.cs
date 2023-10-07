using System;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;

namespace WindowsFormsApp2
{
    class GameForm
    {
        public Form Form { get; private set; }
        private TableLayoutPanel tableLayoutPanel;
        private int[,] matrix;
      //  private List<int> visibleNumbers = new List<int>(); // Список для хранения видимых чисел
        private Label scoreLabel; // Поле для Label с счетом

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

        public void InitializeForm()
        {
            Form = new Form();
            Form.Size = new System.Drawing.Size(500, 500);
            Form.BackColor = System.Drawing.Color.White;
            Form.Text = "Циферки онлайн без смс и регистрации";

            CreateAddButton();
            CreateScoreBoard();
            CreateScoreBoardButton();
            CreateHelpButton();
        }

        private void CreateAddButton()
        {
            Button addButton = new Button();
            addButton.Text = "Добавить цифры";
            addButton.Location = new Point(250, 50);
            addButton.AutoSize = true;
            addButton.Click += (sender, e) => AddVisibleElementsToTable();

            // Добавляем кнопку на форму
            Form.Controls.Add(addButton);
        }

        private void CreateScoreBoard()
        {
                    // Создаем или обновляем Label для отображения счета
            if (scoreLabel == null)
            {
                scoreLabel = new Label();
                scoreLabel.ForeColor = Color.Black;
                scoreLabel.Font = new Font("Arial", 24, FontStyle.Bold);
                scoreLabel.AutoSize = true;
                scoreLabel.Location = new Point(280, 10); // Расположение Label с счетом
                scoreLabel.Text = "Счёт: " + score;

                // Добавляем Label на форму
                Form.Controls.Add(scoreLabel); 
                
            }
        }

        private void CreateScoreBoardButton()
        {
            Button ScoreBoardButton = new Button();
            ScoreBoardButton.Text = "Таблица лидеров";
            ScoreBoardButton.Location = new Point(250, 80);
            ScoreBoardButton.AutoSize = true;

            //  Добавляем кнопку на форму
            Form.Controls.Add(ScoreBoardButton);
        }

        private void CreateHelpButton()
        {
            Button HelpButton = new Button();
            HelpButton.Text = "Помощь";
            HelpButton.Location = new Point(250, 110);
            HelpButton.AutoSize = true;

            //Добавляем кнопку на форму
            Form.Controls.Add(HelpButton);
        }

        private void InitializeTableLayoutPanel()
        {
            tableLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel.Dock = DockStyle.Fill;
            Form.Controls.Add(tableLayoutPanel);

            tableLayoutPanel.RowCount = 128;
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

        private Label firstClickedLabel;
        private Label secondClickedLabel;
        private Point previous_Click;   //класс для работы с координатами
        private int sum = 0;
        private int score = 0; // Поле для хранения счета

        private void label_Click(object sender, EventArgs e)
        {
            // Приводим sender к типу label для дальнейших взаимодействий
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

                    UpdateScore(Convert.ToInt32(firstClickedLabel.Text) + Convert.ToInt32(secondClickedLabel.Text));

                    // Скрытие двух Label
                    firstClickedLabel.Visible = false;
                    secondClickedLabel.Visible = false;

                    if(IsGameFinished()){
                        MessageBox.Show($"Игра окончена!\n Счёт: {score}");
                    }

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
                            UpdateScore(Convert.ToInt32(firstClickedLabel.Text) + Convert.ToInt32(secondClickedLabel.Text));
                            firstClickedLabel.Visible = false;
                            secondClickedLabel.Visible = false;
                            if (IsGameFinished())
                            {
                                MessageBox.Show($"Игра окончена!\n Счёт: {score}");
                            }

                        }
                        else
                        {
                            Console.WriteLine("Числа не совпали");
                        }

                        //Сбрасываем значения, чтобы не забивались от неправильных кликов
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

        private void AddVisibleElementsToTable()
        {
            int newRow = tableLayoutPanel.RowCount; // Получить текущее количество строк
            int newCol = 0; // Начать добавление с первого столбца в новой строке

            for (int row = 0; row < tableLayoutPanel.RowCount; row++)
            {
                for (int col = 0; col < tableLayoutPanel.ColumnCount; col++)
                {
                    Control control = tableLayoutPanel.GetControlFromPosition(col, row);

                    if (control != null && control.Visible)
                    {
                        // Создаем новый Label с такими же параметрами
                        Label newLabel = new Label();
                        newLabel.Text = control.Text;
                        SetLabelColor(newLabel);
                        newLabel.AutoSize = true;
                        newLabel.TextAlign = ContentAlignment.MiddleCenter;
                        newLabel.Font = new Font("Arial", 16, FontStyle.Bold);

                        // Устанавливаем минимальную ширину для текущего столбца
                        tableLayoutPanel.ColumnStyles[col] = new ColumnStyle(SizeType.Absolute, 30); // Заменить 30 на минимальную ширину

                        // Добавляем новый Label в конец таблицы в новой строке и текущем столбце
                        tableLayoutPanel.Controls.Add(newLabel, newCol, newRow);

                        newCol++; // Переходим к следующему столбцу

                        if (newCol >= tableLayoutPanel.ColumnCount)
                        {
                            // Если достигнут конец строки, переходим на следующую строку и сбрасываем счетчик столбцов
                            newRow++;
                            newCol = 0;
                        }

                        // Привязываем обработчик события label_Click к новому Label
                        newLabel.Click += new EventHandler(label_Click);
                    }
                }
            }

            tableLayoutPanel.RowCount = newRow + 1; // Обновляем RowCount после добавления новых элементов
            // нужно, чтобы не добавлялись цифры в старые label, только в новые 
        }

        private bool IsGameFinished()
        {
            foreach (Control control in tableLayoutPanel.Controls)
            {
                if (control is Label label && label.Visible)
                {
                    // Если хоть один видимый Label найден, игра еще не закончена
                    return false;
                }
            }

            // Если все цифры стали невидимыми, игра закончена
            return true;
        }

        // Метод для обновления счета и отображения его на форме
        private void UpdateScore(int pointsToAdd)
        {
            // Увеличиваем счет на указанное количество очков
            score += pointsToAdd; 

            // Обновляем текст Label с текущим счетом
            scoreLabel.Text = "Счёт: " + score.ToString();
        }


    }

}