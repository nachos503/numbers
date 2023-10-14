using System;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.IO;

namespace WindowsFormsApp2
{
    class GameForm
    {
        public Form Form { get; private set; }
        private TableLayoutPanel tableLayoutPanel;
        private int[,] matrix;
        private Label scoreLabel; // Поле для Label с счетом

        private DatabaseManager databaseManager;

        public GameForm()
        {

            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;

            databaseManager = new DatabaseManager("Game.db");

            string dbFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Game.db");

            databaseManager = new DatabaseManager(dbFilePath);
        }

        public void SaveGameDataToDatabase(string tableLayoutPanelData, int playerScore, int RowCount)
        {
            // Вместо передачи RowCount в аргументах, просто получим его из TableLayoutPanel
            tableLayoutPanelData = SerializeTableLayoutPanelData();
            playerScore = score;

            // Также просто получим значение RowCount из TableLayoutPanel
            RowCount = tableLayoutPanel.RowCount;

            databaseManager.SaveGameData(tableLayoutPanelData, playerScore, RowCount);
        }

        private string SerializeTableLayoutPanelData()
        {
            StringBuilder sb = new StringBuilder();

            foreach (Control control in tableLayoutPanel.Controls)
            {
                if (control is Label label)
                {
                    if (label.Visible == false) { label.Text = "0"; }
                    sb.Append(label.Text);
                    sb.Append(","); // Разделитель между данными 
                }
            }

            // Удалить последний разделитель, если он есть
            if (sb.Length > 0)
            {
                sb.Length -= 1;
            }

            return sb.ToString();
        }
public void LoadGameDataFromDatabase()
{
    var loadedData = databaseManager.LoadGameData();

    if (loadedData != (null, null, null))
    {
        string tableLayoutPanelData = loadedData.Item1;
        int playerScore = loadedData.Item2;
        int rowCount = loadedData.Item3;

        // Удалите существующую таблицу, если она есть
        if (tableLayoutPanel != null)
        {
            Form.Controls.Remove(tableLayoutPanel);
            tableLayoutPanel.Dispose();
        }

        // Создайте новую таблицу
        tableLayoutPanel = new TableLayoutPanel();
        tableLayoutPanel.Dock = DockStyle.Fill;

        // Установите значение RowCount для новой таблицы
        tableLayoutPanel.RowCount = rowCount;
        tableLayoutPanel.ColumnCount = 7; // Замените на желаемое количество столбцов


        Form.Controls.Add(tableLayoutPanel);

        DeserializeTableLayoutPanelData(tableLayoutPanelData, rowCount);

        score = playerScore;
        scoreLabel.Text = "Счёт: " + score.ToString();
    }
    else
    {
        MessageBox.Show("Нет сохраненных данных.");
    }
}

private void DeserializeTableLayoutPanelData(string data, int rowCount)
{
    string[] elements = data.Split(',');
    int elementIndex = 0;

    // Создаем новый TableLayoutPanel с теми же параметрами
    TableLayoutPanel newTableLayoutPanel = new TableLayoutPanel();
    newTableLayoutPanel.Dock = tableLayoutPanel.Dock;
    newTableLayoutPanel.RowCount = rowCount;
    newTableLayoutPanel.ColumnCount = tableLayoutPanel.ColumnCount;

    // Устанавливаем стиль для всех столбцов, чтобы они не скрывались
    for (int col = 0; col < newTableLayoutPanel.ColumnCount; col++)
    {
        newTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30));
    }

    for (int row = 0; row < rowCount; row++)
    {
        for (int col = 0; col < tableLayoutPanel.ColumnCount; col++)
        {
            if (elementIndex < elements.Length)
            {
                Label label = new Label();
                label.Text = elements[elementIndex];
                SetLabelColor(label);
                label.AutoSize = true;
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Font = new Font("Arial", 16, FontStyle.Bold);

                if (label.Text == "0")
                {
                    label.Visible = false;
                }

                newTableLayoutPanel.Controls.Add(label, col, row);
                label.Click += new EventHandler(label_Click);
                elementIndex++;
            }
        }
    }

    // Удалить старый TableLayoutPanel и заменить его на новый
    Form.Controls.Remove(tableLayoutPanel);
    tableLayoutPanel.Dispose();
    tableLayoutPanel = newTableLayoutPanel;
    Form.Controls.Add(tableLayoutPanel);
}

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
            Form.Size = new System.Drawing.Size(1920, 1080);
            Form.BackColor = System.Drawing.Color.White;
            Form.Text = "Циферки онлайн без смс и регистрации";

            CreateAddButton();
            CreateScoreBoard();
            CreateScoreBoardButton();
            CreateHelpButton();
            CreateSaveButton();
            CreateLoadButton();
        }

        private void CreateAddButton()
        {
            Button addButton = new Button();
            addButton.Text = "Добавить цифры";
            addButton.Location = new Point(250, 50);
            addButton.AutoSize = true;
            addButton.Click += (sender, e) =>   AddVisibleElementsToTable();

            // Добавляем кнопку на форму
            Form.Controls.Add(addButton);
        }

        private void CreateLoadButton()
        {
            Button loadButton = new Button();
            loadButton.Text = "Загрузить данные";
            loadButton.Location = new Point(250, 170);
            loadButton.AutoSize = true;
            loadButton.Click += (sender, e) => LoadGameDataFromDatabase();
            Form.Controls.Add(loadButton);
        }

        private void CreateSaveButton()
        {
            Button saveButton = new Button();
            saveButton.Text = "Сохранить игру";
            saveButton.Location = new Point(250, 140); // Расположение кнопки
            saveButton.AutoSize = true;

            // Добавляем обработчик события Click для кнопки сохранения
            saveButton.Click += (sender, e) =>
            {
                string tableLayoutPanelData = SerializeTableLayoutPanelData(); 
                int playerScore = score; // Здесь используется текущий счет игрока
                SaveGameDataToDatabase(tableLayoutPanelData, playerScore, tableLayoutPanel.RowCount);
            };


            // Добавляем кнопку на форму
            Form.Controls.Add(saveButton);
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
            if (firstClickedLabel == null)
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

                if (firstClickedLabel.Text == secondClickedLabel.Text && Checking_Intersections())
                {
                    Console.WriteLine("Оба числа совпали");

                    UpdateScore(Convert.ToInt32(firstClickedLabel.Text) + Convert.ToInt32(secondClickedLabel.Text));

                    // Скрытие двух Label
                    firstClickedLabel.Visible = false;
                    secondClickedLabel.Visible = false;

                    if(IsGameFinished())
                    {
                        MessageBox.Show($"Игра окончена!\n Счёт: {score}");
                    }

                }
                else if (firstClickedLabel.Text != secondClickedLabel.Text)
                {
                    if (firstClickedLabel.Text != " " && secondClickedLabel.Text != " ")
                    {
                        sum = 0;
                        int num1 = int.Parse(firstClickedLabel.Text);
                        int num2 = int.Parse(secondClickedLabel.Text);
                        sum += num1 + num2;
                        if (sum == 10 && Checking_Intersections())//провеярет числа на суму 10
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
                            firstClickedLabel = null;
                            secondClickedLabel = null;
                        }

                        //Сбрасываем значения, чтобы не забивались от неправильных кликов
                        firstClickedLabel = null;
                        secondClickedLabel = null;
                    }
                }
                firstClickedLabel = null;
                secondClickedLabel = null;
            }
        }

        private bool Checking_Intersections()
        {
            TableLayoutPanel tableLayoutPanel = firstClickedLabel.Parent as TableLayoutPanel;

            // Определяем индекс строки и столбца
            int rowIndex1 = tableLayoutPanel.GetRow(firstClickedLabel);
            int columnIndex1 = tableLayoutPanel.GetColumn(firstClickedLabel);
            int rowIndex2 = tableLayoutPanel.GetRow(secondClickedLabel);
            int columnIndex2 = tableLayoutPanel.GetColumn(secondClickedLabel);

            foreach (Control control in tableLayoutPanel.Controls)
            {
                // Проверяем, находится ли control в той же строке, что и firstClickedLabel, и не содержит ли control текст, отличный от "  "
                if ((rowIndex1 == rowIndex2 && (AreNumbersInBetweenByRow(firstClickedLabel,secondClickedLabel)))
                    || ((columnIndex1 == columnIndex2) && (AreNumbersInBetweenByColumn(firstClickedLabel, secondClickedLabel))))
                {
                    //устанавливаем флаг в true и выходим из цикла
                    return true;
                }
            }
            // Если не найдено препятствий, возвращаем false
            return false;
        }

        private bool AreNumbersInBetweenByColumn(Label firstlabelclick, Label secondlabelclick) //провекра наличие препятсвий по столбцам
        {
            TableLayoutPanel tableLayoutPanel = firstClickedLabel.Parent as TableLayoutPanel;
            TableLayoutPanel tableLayoutPanel1 = secondClickedLabel.Parent as TableLayoutPanel;

            int columnNumber = tableLayoutPanel.GetColumn(firstClickedLabel);
            if (Math.Abs(tableLayoutPanel.GetRow(firstlabelclick) - tableLayoutPanel1.GetRow(secondlabelclick)) == 1)
            {
                return true;
            }
                if (tableLayoutPanel.GetRow(firstClickedLabel) < tableLayoutPanel1.GetRow(secondClickedLabel)) //сравниваем какой раньше встречается
            {
                for (int row = tableLayoutPanel1.GetRow(firstClickedLabel)+1; row < tableLayoutPanel.GetRow(secondClickedLabel); row++) // идем от первого до второго клика
                {
                    Label labelBetween = tableLayoutPanel.GetControlFromPosition(columnNumber, row) as Label; //переводим конкуретную позицию таблицу в тип Label 
                    if (labelBetween != null && labelBetween.Visible) // если не null и если отображается элемент управления (не удален)
                    {
                        return false;
                    }
                }
            }
            else if (tableLayoutPanel.GetRow(firstClickedLabel) > tableLayoutPanel1.GetRow(secondClickedLabel))
            {
                for (int row = tableLayoutPanel1.GetRow(secondClickedLabel)+1; row < tableLayoutPanel.GetRow(firstClickedLabel); row++) // идем от первого до второго клика
                {
                    Label labelBetween = tableLayoutPanel1.GetControlFromPosition(columnNumber, row) as Label; //переводим конкуретную позицию таблицу в тип Label 
                    if (labelBetween != null && labelBetween.Visible) // если не null и если виден
                    {
                        return false;
                    }
                }

            }
             return true;
        }

        private bool AreNumbersInBetweenByRow(Label firstlabelclick, Label secondlabelclick)
        {
            TableLayoutPanel tableLayoutPanel = firstlabelclick.Parent as TableLayoutPanel;
            TableLayoutPanel tableLayoutPanel1 = secondlabelclick.Parent as TableLayoutPanel;

            int rowNumber = tableLayoutPanel.GetRow(firstlabelclick);
            if (Math.Abs(tableLayoutPanel.GetColumn(firstlabelclick) - tableLayoutPanel1.GetColumn(secondlabelclick)) == 1)
            {
                return true;
            }
            if (tableLayoutPanel.GetColumn(firstlabelclick) < tableLayoutPanel1.GetColumn(secondlabelclick))
            {
                for (int column = tableLayoutPanel.GetColumn(firstlabelclick)+1; column < tableLayoutPanel1.GetColumn(secondlabelclick); column++)
                {
                    Label labelBetween = tableLayoutPanel.GetControlFromPosition(column, rowNumber) as Label;
                    if (labelBetween != null && labelBetween.Visible)
                    {
                        return false;
                    }
                }
            }
            else if (tableLayoutPanel.GetColumn(firstlabelclick) > tableLayoutPanel1.GetColumn(secondlabelclick))
            {
                for (int column = tableLayoutPanel1.GetColumn(secondlabelclick)+1; column < tableLayoutPanel.GetColumn(firstlabelclick); column++)
                {
                    Label labelBetween = tableLayoutPanel1.GetControlFromPosition(column, rowNumber) as Label;
                    if (labelBetween != null && labelBetween.Visible)
                    {
                        return false;
                    }
                }
            }

            return true; // Возвращаем true, если не найдено видимых Label'ов между ними по строкам
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

                    if (control != null && control.Visible) //если мы встречаем путое место, в которое можно вставить цифры
                    {
                        // Создаем новый Label с такими же параметрами
                        Label newLabel = new Label();
                        newLabel.Text = control.Text;
                        SetLabelColor(newLabel);
                        newLabel.AutoSize = true;
                        newLabel.TextAlign = ContentAlignment.MiddleCenter;
                        newLabel.Font = new Font("Arial", 16, FontStyle.Bold);

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
                if (control is Label label && label.Visible) //является ли control экземпляром класса
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





public class DatabaseManager
{
    private string connectionString;

    public DatabaseManager(string databaseFilePath)
    {
        connectionString = $"Data Source={databaseFilePath};Version=3;";
    }

    public void SaveGameData(string tableLayoutPanelData, int playerScore, int RowCount)
    {
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO GameData (TableLayoutPanelData, PlayerScore, RowCount) VALUES (@TableLayoutPanelData, @PlayerScore, @RowCount)";
                command.Parameters.AddWithValue("@TableLayoutPanelData", tableLayoutPanelData);
                command.Parameters.AddWithValue("@PlayerScore", playerScore);
                command.Parameters.AddWithValue("@RowCount", RowCount);

                command.ExecuteNonQuery();
            }
        }
    }

    public (string tableLayoutPanelData, int playerScore, int RowCount) LoadGameData()
        {
            string tableLayoutPanelData = null;
            int playerScore = 0;
            int RowCount = 0;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT TableLayoutPanelData, PlayerScore, RowCount FROM GameData ORDER BY Id DESC LIMIT 1";

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            tableLayoutPanelData = reader.GetString(0);
                            playerScore = reader.GetInt32(1);
                            RowCount = reader.GetInt32(2);
                        }
                    }
                }
            }

            return (tableLayoutPanelData, playerScore, RowCount);
        }
        }
        }