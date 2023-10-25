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

 
    class GameForm : Form
    {
        public Form gameForm { get; private set; }
        private TableLayoutPanel tableLayoutPanel;
        private int[,] matrix;
        private Label scoreLabel; // Поле для Label с счетом
        private User user; // Добавьте поле для хранения User
        private databaseManager databaseManager;
        public GameForm()
        {


            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;

            databaseManager = new databaseManager("Game.db");

            string dbFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Game.db");

            databaseManager = new databaseManager(dbFilePath);
        }




        // Метод, который будет вызываться для установки пользователя
        public void SetUser(User user)
        {
            this.user = user;
        }


        public databaseManager GetDatabaseManager()
        {
            return databaseManager;
        }

        public void SaveGameDataTempToDatabase(string tableLayoutPanelData, int playerScore, int RowCount, string username)
        {
            databaseManager.SaveGameDataTemp(tableLayoutPanelData, playerScore, RowCount, username);
        }

        public void SaveGameDataToDatabase(string tableLayoutPanelData, int playerScore, int RowCount, string username)
        {

            databaseManager.SaveGameData(tableLayoutPanelData, playerScore, RowCount, username);
        }

        public void SaveLeaderboardToDatabase(int playerScore, string username)
        {
            playerScore = score;
            username = user.Username.ToString();

            databaseManager.SaveLeaderboard(playerScore, username);
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

        public void LoadGameDataFromDatabaseByUsernameTemp(string playerName)
        {
            var loadedData = databaseManager.LoadGameDataByUsernameTemp(playerName);

            if (loadedData != null)
            {
                string tableLayoutPanelData = loadedData.Item1;
                int playerScore = loadedData.Item2;
                int rowCount = loadedData.Item3;

                // Удаляем существующую таблицу, если она есть
                if (tableLayoutPanel != null)
                {
                    gameForm.Controls.Remove(tableLayoutPanel);
                    tableLayoutPanel.Dispose();
                }

                // Создаём новую таблицу
                tableLayoutPanel = new TableLayoutPanel();
                tableLayoutPanel.Dock = DockStyle.Fill;

                // Установливаем значение RowCount для новой таблицы
                tableLayoutPanel.RowCount = rowCount;
                tableLayoutPanel.ColumnCount = 7; 


                gameForm.Controls.Add(tableLayoutPanel);

                DeserializeTableLayoutPanelData(tableLayoutPanelData, rowCount);

                

                score = playerScore;
                scoreLabel.Text = "Счёт: " + score.ToString();
            }
            else
            {
                MessageBox.Show("Нет сохраненных данных.");
            }
        }


        public void LoadGameDataFromDatabaseByUsername(string playerName)
        {
    // Вызываем метод в databaseManager для получения данных из базы данных
    var loadedData = databaseManager.LoadGameDataByUsername(playerName);

    if (loadedData != null)
    {
        string tableLayoutPanelData = loadedData.Item1;
        int playerScore = loadedData.Item2;
        int rowCount = loadedData.Item3;

        if(tableLayoutPanel != null)
        {
            gameForm.Controls.Remove((tableLayoutPanel));
            tableLayoutPanel.Dispose();
        }


        tableLayoutPanel = new TableLayoutPanel();
        tableLayoutPanel.Dock = DockStyle.Fill;
        tableLayoutPanel.RowCount = rowCount;
        tableLayoutPanel.ColumnCount = 7; 

        gameForm.Controls.Add(tableLayoutPanel);

        DeserializeTableLayoutPanelData(tableLayoutPanelData, rowCount);

        score = playerScore;
        scoreLabel.Text = "Счёт: " + score.ToString();
    }
    else
    {
        MessageBox.Show("Нет сохраненных данных.");
    }
        }

        public void LoadLeaderboardFromDatabase()
        {
            var loadedLeaderboard = databaseManager.LoadLeaderboard();

            if (loadedLeaderboard.Count > 0)
            {
                string leaderboardMessage = "Таблица лидеров:\n";

                foreach (var leader in loadedLeaderboard)
                {
                    string Username = leader.Username;
                    int playerScore = leader.playerScore;

                    leaderboardMessage += $"Имя: {Username}, Счёт: {playerScore}\n";
                }

                MessageBox.Show(leaderboardMessage);
            }
            else
            {
                MessageBox.Show("Таблица лидеров пуста.");
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
            gameForm.Controls.Remove(tableLayoutPanel);
            tableLayoutPanel.Dispose();
            tableLayoutPanel = newTableLayoutPanel;
            gameForm.Controls.Add(tableLayoutPanel);
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
            gameForm = new GameForm();
            gameForm.Size = new System.Drawing.Size(1920, 1080);
            gameForm.BackColor = System.Drawing.Color.White;
            gameForm.Text = "Циферки оффлайн с смс и регистрацией";
            gameForm.AutoScroll = true;

            CreateAddButton();
            CreateScoreBoard();
            CreateScoreBoardButton();
            CreateHelpButton();
            CreateLoadButton();
            CreateSaveButton();
            CreateExitButton();
            CreateRestartButton();
            CreateSaveTempButton();
            CreateLoadTempButton();
        }



        private void HelpButtonText(object sender, EventArgs e) //обработка клика и вывод текста
        {
            MessageBox.Show("-------------------------------| ПРАВИЛА ИГРЫ |-------------------------------" +
                "\r\n\r\nУдалять можно одинаковые числа (или их сумма равна 10), которые находятся на одной строке или столбце, при условии, что между ними нет других чисел " +
                "\r\n\r\nИгра заканчивается тогда, когда удалите все числа на поле");
        }
        private void CreateHelpButton()
        {
            Button HelpButton = new Button();
            HelpButton.Text = "Помощь";
            HelpButton.Location = new Point(250, 110);
            HelpButton.AutoSize = true;
            // Добавляем обработчик события нажатия на кнопку
            HelpButton.Click += new EventHandler(HelpButtonText);
            //Добавляем кнопку на форму
            gameForm.Controls.Add(HelpButton);
        }

        public void CreateRestartButton()
        {
            Button restartButton = new Button();
            restartButton.Text = "Перезапустить игру";
            restartButton.AutoSize = true;
            restartButton.Location = new Point(400, 80);
            restartButton.Click += RestartButton_Click;
            gameForm.Controls.Add(restartButton);
        }

        public void CreateExitButton()
        {
            Button exitButton = new Button();
            exitButton.Text = "Выйти из игры";
            exitButton.AutoSize = true;
            exitButton.Location = new Point(400, 170);
            exitButton.Click += ExitButton_Click;
            gameForm.Controls.Add(exitButton);
        }

        public void CreateSaveButton()
        {
            Button saveButton = new Button();
            saveButton.Text = "Сохранить и выйти";
            saveButton.AutoSize = true;
            saveButton.Location = new Point(400, 140);
            saveButton.Click += SaveButton_Click;
            gameForm.Controls.Add(saveButton);
        }

        public void CreateSaveTempButton()
        {
            Button saveTempButton = new Button();
            saveTempButton.Text = "Временное сохранение";
            saveTempButton.AutoSize = true;
            saveTempButton.Location = new Point(400, 110);
            saveTempButton.Click += SaveTempButton_Click;
            gameForm.Controls.Add(saveTempButton);
        }

        private void SaveTempButton_Click(object sender, EventArgs e)
        {
            string username = user.Username.ToString();
            SaveGameDataTempToDatabase(SerializeTableLayoutPanelData(), score, tableLayoutPanel.RowCount, username);
            
            MessageBox.Show("Игра сохранена.");
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            string username = user.Username.ToString();
            SaveGameDataToDatabase(SerializeTableLayoutPanelData(), score, tableLayoutPanel.RowCount, username);
            SaveLeaderboardToDatabase(score, username);
            MessageBox.Show("Игра сохранена.");
            Application.Exit();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            var dlgRes = MessageBox.Show("Вы хотите выйти без сохранения?", "", MessageBoxButtons.YesNo);
            if (dlgRes == DialogResult.Yes)
            {
                databaseManager.DeleteGameDataTemp(user.Username);
                Application.Exit();
            }
            if (dlgRes == DialogResult.No)
            {
                MessageBox.Show( $"Для сохранения нажмите <Сохранить и выйти>");
            }
        }

        private void RestartButton_Click(object sender, EventArgs e)
        {
            // Создаём новый экземпляр GameForm и инициализируйте его
            GameForm newGameForm = new GameForm();
            newGameForm.Initialize();

            // Получаем пользователя из текущего GameForm и передайте его в новый
    
            newGameForm.SetUser(user);

            // Закрываем текущий GameForm
            gameForm.Hide();

            // Отображаем новый GameForm
            newGameForm.gameForm.Show();
        }

        private void CreateAddButton()
        {
            Button addButton = new Button();
            addButton.Text = "Добавить цифры";
            addButton.Location = new Point(250, 50);
            addButton.AutoSize = true;
            addButton.Click += (sender, e) => AddVisibleElementsToTable();

            // Добавляем кнопку на форму
            gameForm.Controls.Add(addButton);
        }

        private void CreateLoadButton()
        {
            Button loadButton = new Button();
            loadButton.Text = "Загрузить сохранение";
            loadButton.Location = new Point(250, 140);
            loadButton.AutoSize = true;
            loadButton.Click += (sender, e) => LoadGameDataFromDatabaseByUsername(user.Username);
            gameForm.Controls.Add(loadButton);
        }

        private void CreateLoadTempButton()
        {
            Button loadTempButton = new Button();
            loadTempButton.Text = "Загрузить временное сохранение";
            loadTempButton.Location = new Point(540, 110);
            loadTempButton.AutoSize = true;
            loadTempButton.Click += (sender, e) => LoadGameDataFromDatabaseByUsernameTemp(user.Username);
            gameForm.Controls.Add(loadTempButton);
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
                gameForm.Controls.Add(scoreLabel);

            }
        }

        private void CreateScoreBoardButton()
        {
            Button ScoreBoardButton = new Button();
            ScoreBoardButton.Text = "Таблица лидеров";
            ScoreBoardButton.Location = new Point(250, 80);
            ScoreBoardButton.AutoSize = true;

            ScoreBoardButton.Click += ScoreBoardButton_Click;



            //  Добавляем кнопку на форму
            gameForm.Controls.Add(ScoreBoardButton);
        }

        private void ScoreBoardButton_Click(object sender, EventArgs e)
        {
            LoadLeaderboardFromDatabase();
        }

        public void InitializeTableLayoutPanel()
        {
            tableLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel.Dock = DockStyle.Fill;
            gameForm.Controls.Add(tableLayoutPanel);

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

        enum LabelColor
        {
            Black,
            Blue,
            Green,
            Yellow,
            Pink,
            DeepPink,
            LightBlue,
            LightCoral,
            GreenYellow,
            Red

        }

        private void SetLabelColor(Label label)
        {
            if (Enum.TryParse(label.Text, out LabelColor color))
            {
                 switch (color)
                 {
                case LabelColor.Red:
                 label.ForeColor = System.Drawing.Color.Red;
                    break;
                case LabelColor.Blue:
                label.ForeColor = System.Drawing.Color.Blue;
                    break;
                case LabelColor.Green:
                 label.ForeColor = System.Drawing.Color.Green;
                    break;
                case LabelColor.Yellow:
                 label.ForeColor = System.Drawing.Color.Yellow;
                    break;
                case LabelColor.Pink:
                 label.ForeColor = System.Drawing.Color.Pink;
                    break;
                case LabelColor.DeepPink:
                 label.ForeColor = System.Drawing.Color.DeepPink;
                    break;
                case LabelColor.LightBlue:
                 label.ForeColor = System.Drawing.Color.LightBlue;
                    break;
                case LabelColor.LightCoral:
                  label.ForeColor = System.Drawing.Color.LightCoral;
                  break;
                case LabelColor.GreenYellow:
                  label.ForeColor = System.Drawing.Color.GreenYellow;
                  break;
            }
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
                firstClickedLabel.Font = new Font("Arial", 16, FontStyle.Underline);
                Console.WriteLine($"Первый клик сделан по {label.Text}");
            }
            else if (firstClickedLabel != label)
            {
                secondClickedLabel = label;
                secondClickedLabel.Font = new Font("Arial", 16, FontStyle.Underline);
                Console.WriteLine($"Второй клик сделан по {label.Text}");
                if (firstClickedLabel.Text == secondClickedLabel.Text && mouseArgs.Location == previous_Click && mouseArgs != null && previous_Click != null) ///проверка на одинаковые числа
                {
                    label.Font = new Font("Arial", 16, FontStyle.Bold);
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

                    if (IsGameFinished())
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
                            firstClickedLabel.Font = new Font("Arial", 16, FontStyle.Bold);
                            secondClickedLabel.Font = new Font("Arial", 16, FontStyle.Bold);
                            firstClickedLabel = null;
                            secondClickedLabel = null;
                            
                        }

                        //Сбрасываем значения, чтобы не забивались от неправильных кликов
                       
                        firstClickedLabel = null;
                        secondClickedLabel = null;
                        
                    }
                }
                if (firstClickedLabel != null || secondClickedLabel != null)
                {
                    firstClickedLabel.Font = new Font("Arial", 16, FontStyle.Bold);
                    secondClickedLabel.Font = new Font("Arial", 16, FontStyle.Bold);
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
                if ((rowIndex1 == rowIndex2 && (AreNumbersInBetweenByRow(firstClickedLabel, secondClickedLabel)))
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
                for (int row = tableLayoutPanel1.GetRow(firstClickedLabel) + 1; row < tableLayoutPanel.GetRow(secondClickedLabel); row++) // идем от первого до второго клика
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
                for (int row = tableLayoutPanel1.GetRow(secondClickedLabel) + 1; row < tableLayoutPanel.GetRow(firstClickedLabel); row++) // идем от первого до второго клика
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
                for (int column = tableLayoutPanel.GetColumn(firstlabelclick) + 1; column < tableLayoutPanel1.GetColumn(secondlabelclick); column++)
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
                for (int column = tableLayoutPanel1.GetColumn(secondlabelclick) + 1; column < tableLayoutPanel.GetColumn(firstlabelclick); column++)
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

    public class databaseManager
    {
        private string connectionString;

        public databaseManager(string databaseFilePath)
        {
            connectionString = $"Data Source={databaseFilePath};Version=3;";
        }

        public void SaveGameDataTemp(string tableLayoutPanelData, int playerScore, int RowCount, string Username)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand cmd = new SQLiteCommand("INSERT OR REPLACE INTO TempData (TableLayoutPanelData, PlayerScore, RowCount, Username) VALUES (@TableLayoutPanelData, @PlayerScore, @RowCount, @Username)", connection))
                {
                    cmd.Parameters.AddWithValue("@TableLayoutPanelData", tableLayoutPanelData);
                    cmd.Parameters.AddWithValue("@PlayerScore", playerScore);
                    cmd.Parameters.AddWithValue("@RowCount", RowCount);
                    cmd.Parameters.AddWithValue("@Username", Username);

                    cmd.ExecuteNonQuery();
                }
            }

        }

       

                public void SaveLeaderboard(int playerScore, string Username)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand selectCommand = connection.CreateCommand())
                {
                    selectCommand.CommandText = "SELECT PlayerScore FROM LeaderboardData WHERE Username = @Username";
                    selectCommand.Parameters.AddWithValue("@Username", Username);

                    using (SQLiteDataReader reader = selectCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int currentScore = reader.GetInt32(0);

                            // Проверяем, если новый счет больше текущего
                            if (playerScore > currentScore)
                            {
                                using (SQLiteCommand updateCommand = connection.CreateCommand())
                                {
                                    updateCommand.CommandText = "UPDATE LeaderboardData SET PlayerScore = @PlayerScore WHERE Username = @Username";
                                    updateCommand.Parameters.AddWithValue("@PlayerScore", playerScore);
                                    updateCommand.Parameters.AddWithValue("@Username", Username);

                                    updateCommand.ExecuteNonQuery();
                                }
                            }   
                        }
                        else
                        {
                            // Если записи для данного пользователя нет, то создаем новую запись
                            using (SQLiteCommand insertCommand = connection.CreateCommand())
                            {
                                insertCommand.CommandText = "INSERT INTO LeaderboardData (PlayerScore, Username) VALUES ( @PlayerScore, @Username)";
                                insertCommand.Parameters.AddWithValue("@PlayerScore", playerScore);
                                insertCommand.Parameters.AddWithValue("@Username", Username);

                                insertCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
        }
        

        public void SaveGameData(string tableLayoutPanelData, int playerScore, int RowCount, string Username)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand selectCommand = connection.CreateCommand())
                {
                    selectCommand.CommandText = "SELECT PlayerScore FROM GameData WHERE Username = @Username";
                    selectCommand.Parameters.AddWithValue("@Username", Username);

                    using (SQLiteDataReader reader = selectCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                                using (SQLiteCommand updateCommand = connection.CreateCommand())
                                {
                                    updateCommand.CommandText = "UPDATE GameData SET PlayerScore = @PlayerScore, TableLayoutPanelData = @TableLayoutPanelData, RowCount = @RowCount WHERE Username = @Username";
                                    updateCommand.Parameters.AddWithValue("@TableLayoutPanelData", tableLayoutPanelData);
                                    updateCommand.Parameters.AddWithValue("@PlayerScore", playerScore);
                                    updateCommand.Parameters.AddWithValue("@RowCount", RowCount);
                                    updateCommand.Parameters.AddWithValue("@Username", Username);

                                    updateCommand.ExecuteNonQuery();
                                }
                        }
                        else
                        {
                            // Если записи для данного пользователя нет, то создаем новую запись
                            using (SQLiteCommand insertCommand = connection.CreateCommand())
                            {
                                insertCommand.CommandText = "INSERT INTO GameData (TableLayoutPanelData, PlayerScore, RowCount, Username) VALUES (@TableLayoutPanelData, @PlayerScore, @RowCount, @Username)";
                                insertCommand.Parameters.AddWithValue("@TableLayoutPanelData", tableLayoutPanelData);
                                insertCommand.Parameters.AddWithValue("@PlayerScore", playerScore);
                                insertCommand.Parameters.AddWithValue("@RowCount", RowCount);
                                insertCommand.Parameters.AddWithValue("@Username", Username);

                                insertCommand.ExecuteNonQuery();
                            }
                        }
                    }
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

        public (string Username,  string tableLayoutPanelData,  int PlayerScore,  int RowCount) LoadGameDataTemp()
        {
            string Username = null;
            string tableLayoutPanelData = null;
            int PlayerScore = 0;
            int RowCount = 0;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand cmd = new SQLiteCommand("SELECT TableLayoutPanelData, PlayerScore, RowCount FROM TempData WHERE Username = @Username", connection))
                {
                    cmd.Parameters.AddWithValue("@Username", Username);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            tableLayoutPanelData = reader["TableLayoutPanelData"].ToString();
                            PlayerScore = Convert.ToInt32(reader["PlayerScore"]);
                            RowCount = Convert.ToInt32(reader["RowCount"]);
                        }
                    }
                }

            }
            return (Username, tableLayoutPanelData, PlayerScore, RowCount);

        }
        public List<(string Username, int playerScore)> LoadLeaderboard()
        {
            List<(string Username, int playerScore)> leaderboard = new List<(string, int)>();

            string sql = "SELECT Username, playerScore FROM LeaderboardData ORDER BY playerScore DESC;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string username = reader["Username"].ToString();
                            int playerScore = Convert.ToInt32(reader["playerScore"]);
                            leaderboard.Add((username, playerScore));
                        }
                    }
                }
            }

            return leaderboard;
        }

        public bool IsPlayerNameAvailable(string playerName)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT COUNT(*) FROM GameData WHERE Username = @Username", connection))
                {
                    command.Parameters.AddWithValue("@Username", playerName);
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count == 0; // Если count равно 0, имя не существует
                }
            }
        }

        public Tuple<string, int, int> LoadGameDataByUsernameTemp(string playerName)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand("SELECT TableLayoutPanelData, PlayerScore, RowCount FROM TempData WHERE Username = @PlayerName", connection))
                {
                    command.Parameters.AddWithValue("@PlayerName", playerName);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string tableLayoutPanelData = reader["TableLayoutPanelData"].ToString();
                            int playerScore = Convert.ToInt32(reader["PlayerScore"]);
                            int rowCount = Convert.ToInt32(reader["RowCount"]);

                            return new Tuple<string, int, int>(tableLayoutPanelData, playerScore, rowCount);
                        }
                        else
                        {
                            // Вернуть null, если данные не найдены
                            return null;
                        }
                    }
                }
            }

        }

        public Tuple<string, int, int> LoadGameDataByUsername(string playerName)
        {
          using (SQLiteConnection connection = new SQLiteConnection(connectionString))
          {
            connection.Open();

            using (SQLiteCommand command = new SQLiteCommand("SELECT TableLayoutPanelData, PlayerScore, RowCount FROM GameData WHERE Username = @PlayerName", connection))
            {
                command.Parameters.AddWithValue("@PlayerName", playerName);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string tableLayoutPanelData = reader["TableLayoutPanelData"].ToString();
                        int playerScore = Convert.ToInt32(reader["PlayerScore"]);
                        int rowCount = Convert.ToInt32(reader["RowCount"]);

                        return new Tuple<string, int, int>(tableLayoutPanelData, playerScore, rowCount);
                    }
                    else
                    {
                        // Вернуть null, если данные не найдены
                        return null;
                    }
                }
            }
          }
        
       }

        public void DeleteGameDataTemp(string username)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand cmd = new SQLiteCommand("DELETE FROM TempData WHERE Username = @Username", connection))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
    
}