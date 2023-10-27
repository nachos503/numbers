using System;
using System.Windows.Forms;
using System.Drawing;
using System.Text;
using System.IO;

namespace WindowsFormsApp2
{
    /// <summary>
    /// Класс GameForm основное окно игры
    /// </summary>
    class GameForm : Form
    {
        public Form gameForm;
        protected TableLayoutPanel tableLayoutPanel;
        private int[,] matrix;
        protected Label scoreLabel; // Поле для Label с счетом
        protected User user; // Добавьте поле для хранения User
        protected databaseManager databaseManager;

        /// <summary>
        /// Конструктор иницилизации бд и привязка к пути файла
        /// </summary>
        public GameForm()
        {
            databaseManager = new databaseManager("Game.db");

            string dbFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Game.db");

            databaseManager = new databaseManager(dbFilePath);

        }

        /// <summary>
        /// Метод для установки пользователя 
        /// </summary>
        /// <param name="user"></param>
        public void SetUser(User user)
        {
            this.user = user;
        }

        /// <summary>
        /// Метод иницилизации матрицы
        /// </summary>
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

        /// <summary>
        /// Перечисляемый тип для перечисления цветов
        /// </summary>
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

        /// <summary>
        /// Метод установки цветов 
        /// </summary>
        /// <param name="label"></param>
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

        /// <summary>
        /// Метод иницилизации игрового поля 
        /// </summary>
        public void InitializeTableLayoutPanel()
        {
            tableLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel.Dock = DockStyle.Fill;
            gameForm.Controls.Add(tableLayoutPanel);

            tableLayoutPanel.RowCount = 4;
            tableLayoutPanel.ColumnCount = 7;

            for (int col = 0; col < tableLayoutPanel.ColumnCount; col++)// Устанавливаем минимальную ширину для каждого столбца
            {
                tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30));
            }
        }

        /// <summary>
        /// Метод заполнения игрового поля
        /// </summary>
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

        /// <summary>
        /// Метод обработки клика и обработка логики игры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label_Click(object sender, EventArgs e)
        {
            Label label = (Label)sender;// Приводим sender к типу label для дальнейших взаимодействий

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

                    firstClickedLabel.Visible = false;// Скрытие двух Label
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
                        firstClickedLabel = null;//Сбрасываем значения, чтобы не забивались от неправильных кликов
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

        /// <summary>
        /// Проверка на наличие цифр между нажатыми цифрами
        /// </summary>
        /// <returns></returns>
        private bool Checking_Intersections()
        {
            TableLayoutPanel tableLayoutPanel = firstClickedLabel.Parent as TableLayoutPanel;

            int rowIndex1 = tableLayoutPanel.GetRow(firstClickedLabel);// Определяем индекс строки и столбца
            int columnIndex1 = tableLayoutPanel.GetColumn(firstClickedLabel);
            int rowIndex2 = tableLayoutPanel.GetRow(secondClickedLabel);
            int columnIndex2 = tableLayoutPanel.GetColumn(secondClickedLabel);

            foreach (Control control in tableLayoutPanel.Controls)
            {
                // Проверяем, находится ли control в той же строке, что и firstClickedLabel, и не содержит ли control текст, отличный от "  "
                if ((rowIndex1 == rowIndex2 && (AreNumbersInBetweenByRow(firstClickedLabel, secondClickedLabel)))
                    || ((columnIndex1 == columnIndex2) && (AreNumbersInBetweenByColumn(firstClickedLabel, secondClickedLabel))))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Метод проверки нахлждения цифр в одном столбце
        /// </summary>
        /// <param name="firstlabelclick"></param>
        /// <param name="secondlabelclick"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Метод проверки нахождения цифр в одной строке
        /// </summary>
        /// <param name="firstlabelclick"></param>
        /// <param name="secondlabelclick"></param>
        /// <returns></returns>
        private bool AreNumbersInBetweenByRow(Label firstlabelclick, Label secondlabelclick)
        {
            
            TableLayoutPanel tableLayoutPanel = firstlabelclick.Parent as TableLayoutPanel;// Получаем ссылки на tableLayoutPanel для использования label в GetRow 
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

        /// <summary>
        /// Метод добавления новых элементов
        /// </summary>
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
                        Label newLabel = new Label();// Создаем новый Label с такими же параметрами
                        newLabel.Text = control.Text;
                        SetLabelColor(newLabel);
                        newLabel.AutoSize = true;
                        newLabel.TextAlign = ContentAlignment.MiddleCenter;
                        newLabel.Font = new Font("Arial", 16, FontStyle.Bold);
                
                        tableLayoutPanel.Controls.Add(newLabel, newCol, newRow);// Добавляем новый Label в конец таблицы в новой строке и текущем столбце

                        newCol++; // Переходим к следующему столбцу

                        if (newCol >= tableLayoutPanel.ColumnCount)// Если достигнут конец строки, переходим на следующую строку и сбрасываем счетчик столбцов
                        {
                            newRow++;
                            newCol = 0;
                        }
                        newLabel.Click += new EventHandler(label_Click);// Привязываем обработчик события label_Click к новому Label
                    }
                }
            }
            tableLayoutPanel.RowCount = newRow + 1; // Обновляем RowCount после добавления новых элементов
                                                    // нужно, чтобы не добавлялись цифры в старые label, только в новые 
        }

        /// <summary>
        /// Метод проверки конца игры
        /// </summary>
        /// <returns></returns>
        private bool IsGameFinished()
        {
            foreach (Control control in tableLayoutPanel.Controls)
            {
                if (control is Label label && label.Visible) // Если хоть один видимый Label найден, игра еще не закончена
                {
                    return false;
                }
            } 
            return true;// Если все цифры стали невидимыми, игра закончена
        }

        /// <summary>
        /// Метод для обновления счета и отображения его на форме
        /// </summary>
        /// <param name="pointsToAdd"></param>
        private void UpdateScore(int pointsToAdd)
        {            
            score += pointsToAdd;// Увеличиваем счет на указанное количество очков

            scoreLabel.Text = "Счёт: " + score.ToString();// Обновляем текст Label с текущим счетом
        }

        /// <summary>
        /// Метод для получения экземпляра класса бд
        /// </summary>
        /// <returns></returns>
        public databaseManager GetDatabaseManager()
        {
            return databaseManager;
        }

        /// <summary>
        /// Метод временного сохранения данных
        /// </summary>
        /// <param name="tableLayoutPanelData"></param>
        /// <param name="playerScore"></param>
        /// <param name="rowCount"></param>
        /// <param name="username"></param>
        public void SaveGameDataTempToDatabase(string tableLayoutPanelData, int playerScore, int rowCount, string username)
        {
            databaseManager.SaveGameDataTemp(tableLayoutPanelData, playerScore, rowCount, username);
        }

        /// <summary>
        /// Метод сохранения данных
        /// </summary>
        /// <param name="tableLayoutPanelData"></param>
        /// <param name="playerScore"></param>
        /// <param name="rowCount"></param>
        /// <param name="username"></param>
        public void SaveGameDataToDatabase(string tableLayoutPanelData, int playerScore, int rowCount, string username)
        {

            databaseManager.SaveGameData(tableLayoutPanelData, playerScore, rowCount, username);
        }

        /// <summary>
        /// Метод сохранения данных таблицы лидеров
        /// </summary>
        /// <param name="playerScore"></param>
        /// <param name="username"></param>
        public void SaveLeaderboardToDatabase(int playerScore, string username)
        {
            playerScore = score;
            username = user.Username.ToString();

            databaseManager.SaveLeaderboard(playerScore, username);
        }

        /// <summary>
        /// Метод для сериализации данных
        /// </summary>
        /// <returns></returns>
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
            if (sb.Length > 0)// Удалить последний разделитель, если он есть
            {
                sb.Length -= 1;
            }
            return sb.ToString();
        }

        /// <summary>
        /// Метод загрузки временных данных 
        /// </summary>
        /// <param name="playerName"></param>
        public void LoadGameDataFromDatabaseByUsernameTemp(string playerName)
        {
            var loadedData = databaseManager.LoadGameDataByUsernameTemp(playerName);

            if (loadedData != null)
            {
                string tableLayoutPanelData = loadedData.Item1;
                int playerScore = loadedData.Item2;
                int rowCount = loadedData.Item3;

                if (tableLayoutPanel != null)// Удаляем существующую таблицу, если она есть
                {
                    gameForm.Controls.Remove(tableLayoutPanel);
                    tableLayoutPanel.Dispose();
                }

                tableLayoutPanel = new TableLayoutPanel();// Создаём новую таблицу
                tableLayoutPanel.Dock = DockStyle.Fill;

                tableLayoutPanel.RowCount = rowCount;// Установливаем значение RowCount для новой таблицы
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

        /// <summary>
        /// Метод загрузки сохраненных данных
        /// </summary>
        /// <param name="playerName"></param>
        public void LoadGameDataFromDatabaseByUsername(string playerName)
        {
            var loadedData = databaseManager.LoadGameDataByUsername(playerName);// Вызываем метод в databaseManager для получения данных из базы данных

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

        /// <summary>
        /// Метод данных таблицы лидеров
        /// </summary>
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

        /// <summary>
        /// Метод десериализации
        /// </summary>
        /// <param name="data"></param>
        /// <param name="rowCount"></param>
        private void DeserializeTableLayoutPanelData(string data, int rowCount)
        {
            string[] elements = data.Split(',');
            int elementIndex = 0;

            TableLayoutPanel newTableLayoutPanel = new TableLayoutPanel(); // Создаем новый TableLayoutPanel с теми же параметрами
            newTableLayoutPanel.Dock = tableLayoutPanel.Dock;
            newTableLayoutPanel.RowCount = rowCount;
            newTableLayoutPanel.ColumnCount = tableLayoutPanel.ColumnCount;

            for (int col = 0; col < newTableLayoutPanel.ColumnCount; col++)
            {
                newTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30));// Устанавливаем стиль для всех столбцов, чтобы они не скрывались
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