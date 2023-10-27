using System;
using System.Windows.Forms;
using System.Drawing;
using System.Text;
using System.IO;

namespace WindowsFormsApp2
{
    class GameForm : Form
    {
        public Form gameForm;
        protected TableLayoutPanel tableLayoutPanel;
        private int[,] matrix;
        protected Label scoreLabel; // Поле для Label с счетом
        protected User user; // Добавьте поле для хранения User
        protected databaseManager databaseManager;
        public GameForm()
        {
            databaseManager = new databaseManager("Game.db");

            string dbFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Game.db");

            databaseManager = new databaseManager(dbFilePath);

        }

        // Метод, который будет вызываться для установки пользователя
        public void SetUser(User user)
        {
            this.user = user;
        }

        protected void InitializeMatrix()
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

        protected void PopulateTableLayoutPanel()
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
                    label.Font = new Font("Arial", 16);

                    tableLayoutPanel.Controls.Add(label, col, row);

                    label.Click += new EventHandler(label_Click);

                }
            }
        }

        private Label firstClickedLabel;
        private Label secondClickedLabel;
        protected int score = 0; // Поле для хранения счета


        private void label_Click(object sender, EventArgs e)
        {
            // Приводим sender к типу label для дальнейших взаимодействий
            Label label = (Label)sender;

            if (firstClickedLabel == null)
            {
                firstClickedLabel = label;
                firstClickedLabel.Font = new Font("Arial", 16, FontStyle.Bold);
            }
            else if (firstClickedLabel != label)
            {
                secondClickedLabel = label;
                secondClickedLabel.Font = new Font("Arial", 16, FontStyle.Bold);

                if (firstClickedLabel.Text == secondClickedLabel.Text && Checking_Intersections())
                {

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

                        if ((Convert.ToInt32(firstClickedLabel.Text) + Convert.ToInt32(secondClickedLabel.Text) == 10) && Checking_Intersections())//провеярет числа на суму 10
                        {
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
                            firstClickedLabel.Font = new Font("Arial", 16);
                            secondClickedLabel.Font = new Font("Arial", 16);

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
                    firstClickedLabel.Font = new Font("Arial", 16);
                    secondClickedLabel.Font = new Font("Arial", 16);
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
            // Получаем ссылки на tableLayoutPanel для использования label в GetRow итд
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

        protected void AddVisibleElementsToTable()
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

        public databaseManager GetDatabaseManager()
        {
            return databaseManager;
        }

        public void SaveGameDataTempToDatabase(string tableLayoutPanelData, int playerScore, int rowCount, string username)
        {
            databaseManager.SaveGameDataTemp(tableLayoutPanelData, playerScore, rowCount, username);
        }

        public void SaveGameDataToDatabase(string tableLayoutPanelData, int playerScore, int rowCount, string username)
        {

            databaseManager.SaveGameData(tableLayoutPanelData, playerScore, rowCount, username);
        }

        public void SaveLeaderboardToDatabase(int playerScore, string username)
        {
            playerScore = score;
            username = user.Username.ToString();

            databaseManager.SaveLeaderboard(playerScore, username);
        }

        protected string SerializeTableLayoutPanelData()
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

                if (tableLayoutPanel != null)
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
                    string Username = leader.username;
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
                        label.Font = new Font("Arial", 16);

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
    }
}