using System;
using System.Windows.Forms;
using System.Drawing;
using System.Text;
using System.IO;

namespace WindowsFormsApp2
{
    internal class UserInterface : GameForm
    {
        /// <summary>
        /// Метод иницилизации формы и создание кнопок
        /// </summary>
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

        /// <summary>
        /// Метод иницилизации всех компонентов программы
        /// </summary>
        public void Initialize()
        {

            InitializeMatrix();
            InitializeForm();
            InitializeTableLayoutPanel();
            PopulateTableLayoutPanel();

        }

        /// <summary>
        /// Вывод текста кнопки "помощь"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HelpButtonText(object sender, EventArgs e) 
        {
            MessageBox.Show("-------------------------------| ПРАВИЛА ИГРЫ |-------------------------------" +
                "\r\n\r\nУдалять можно одинаковые числа (или их сумма равна 10), которые находятся на одной строке или столбце, при условии, что между ними нет других чисел " +
                "\r\n\r\nИгра заканчивается тогда, когда удалите все числа на поле");
        }

        /// <summary>
        /// Создание кнопки "помощь"
        /// </summary>
        private void CreateHelpButton()
        {
            Button HelpButton = new Button();
            HelpButton.Text = "Помощь";
            HelpButton.Location = new Point(250, 110);
            HelpButton.AutoSize = true;
            
            HelpButton.Click += new EventHandler(HelpButtonText);// Добавляем обработчик события нажатия на кнопку
     
            gameForm.Controls.Add(HelpButton);//Добавляем кнопку на форму
        }

        /// <summary>
        /// Создание кнопки "перезапустить игру"
        /// </summary>
        public void CreateRestartButton()
        {
            Button restartButton = new Button();
            restartButton.Text = "Перезапустить игру";
            restartButton.AutoSize = true;
            restartButton.Location = new Point(400, 80);
            restartButton.Click += RestartButton_Click;
            gameForm.Controls.Add(restartButton);
        }

        /// <summary>
        /// Создание кнопки "Выйти из игры"
        /// </summary>
        public void CreateExitButton()
        {
            Button exitButton = new Button();
            exitButton.Text = "Выйти из игры";
            exitButton.AutoSize = true;
            exitButton.Location = new Point(400, 170);
            exitButton.Click += ExitButton_Click;
            gameForm.Controls.Add(exitButton);
        }

        /// <summary>
        /// Создание кнопки "Сохранить и выйти"
        /// </summary>
        public void CreateSaveButton()
        {
            Button saveButton = new Button();
            saveButton.Text = "Сохранить и выйти";
            saveButton.AutoSize = true;
            saveButton.Location = new Point(400, 140);
            saveButton.Click += SaveButton_Click;
            gameForm.Controls.Add(saveButton);
        }

        /// <summary>
        /// Создание кнопки "Сохранить текущий ход"
        /// </summary>
        public void CreateSaveTempButton()
        {
            Button saveTempButton = new Button();
            saveTempButton.Text = "Сохранить текущий ход";
            saveTempButton.AutoSize = true;
            saveTempButton.Location = new Point(400, 110);
            saveTempButton.Click += SaveTempButton_Click;
            gameForm.Controls.Add(saveTempButton);
        }

        /// <summary>
        /// Логика сохранения текущего хода
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SaveTempButton_Click(object sender, EventArgs e)
        {
            string username = user.Username.ToString();
            SaveGameDataTempToDatabase(SerializeTableLayoutPanelData(), score, tableLayoutPanel.RowCount, username);

            MessageBox.Show("Игра сохранена.");
        }

        /// <summary>
        /// Логика сохранения всей игры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SaveButton_Click(object sender, EventArgs e)
        {
            string username = user.Username.ToString();
            SaveGameDataToDatabase(SerializeTableLayoutPanelData(), score, tableLayoutPanel.RowCount, username);
            SaveLeaderboardToDatabase(score, username);
            MessageBox.Show("Игра сохранена.");
            databaseManager.DeleteGameDataTemp(user.Username);
            Application.Exit();
        }

        /// <summary>
        /// Обработка выхода из игры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ExitButton_Click(object sender, EventArgs e)
        {
            var dlgRes = MessageBox.Show("Вы хотите выйти без сохранения?", "", MessageBoxButtons.YesNo);
            if (dlgRes == DialogResult.Yes)
            {
                databaseManager.DeleteGameDataTemp(user.Username);
                Application.Exit();
            }
            if (dlgRes == DialogResult.No)
            {
                MessageBox.Show($"Для сохранения нажмите <Сохранить и выйти>");
            }
        }

        /// <summary>
        /// Логика перезапуска игры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RestartButton_Click(object sender, EventArgs e)
        {
            UserInterface newUserInterface = new UserInterface();// Создаём новый экземпляр GameForm и инициализируйте его
            newUserInterface.Initialize();

            newUserInterface.SetUser(user);//Получаем пользователя из текущего GameForm и передайте его в новый

            gameForm.Hide();// Закрываем текущий GameForm
            
            newUserInterface.gameForm.Show();// Отображаем новый GameForm
        }

        /// <summary>
        /// Создание кнопки "Добавить цифры"
        /// </summary>
        protected void CreateAddButton()
        {
            Button addButton = new Button();
            addButton.Text = "Добавить цифры";
            addButton.Location = new Point(250, 50);
            addButton.AutoSize = true;
            addButton.Click += (sender, e) => AddVisibleElementsToTable();

            /** Добавляем кнопку на форму*/
            gameForm.Controls.Add(addButton);
        }

        /// <summary>
        /// Создание кнопки "Загрузка сохранения"
        /// </summary>
        protected void CreateLoadButton()
        {
            Button loadButton = new Button();
            loadButton.Text = "Загрузить сохранение";
            loadButton.Location = new Point(250, 140);
            loadButton.AutoSize = true;
            loadButton.Click += (sender, e) => LoadGameDataFromDatabaseByUsername(user.Username);
            gameForm.Controls.Add(loadButton);
        }

        /// <summary>
        /// Создание кнопки "Вернуться к сохраненному ходу"
        /// </summary>
        protected void CreateLoadTempButton()
        {
            Button loadTempButton = new Button();
            loadTempButton.Text = "Вернуться к сохраненному ходу";
            loadTempButton.Location = new Point(540, 110);
            loadTempButton.AutoSize = true;
            loadTempButton.Click += (sender, e) => LoadGameDataFromDatabaseByUsernameTemp(user.Username);
            gameForm.Controls.Add(loadTempButton);
        }

        /// <summary>
        /// Создание кнопки "Счет"
        /// </summary>
        protected void CreateScoreBoard()
        {
            if (scoreLabel == null)
            {
                scoreLabel = new Label();
                scoreLabel.ForeColor = Color.Black;
                scoreLabel.Font = new Font("Arial", 24, FontStyle.Bold);
                scoreLabel.AutoSize = true;
                scoreLabel.Location = new Point(280, 10);// Расположение Label с счетом
                scoreLabel.Text = "Счёт: " + score;

                gameForm.Controls.Add(scoreLabel);// Добавляем Label на форму

            }
        }

        /// <summary>
        /// Создание кнопки "Таблица лидеров"
        /// </summary>
        protected void CreateScoreBoardButton()
        {
            Button ScoreBoardButton = new Button();
            ScoreBoardButton.Text = "Таблица лидеров";
            ScoreBoardButton.Location = new Point(250, 80);
            ScoreBoardButton.AutoSize = true;

            ScoreBoardButton.Click += ScoreBoardButton_Click;

            gameForm.Controls.Add(ScoreBoardButton);//  Добавляем кнопку на форму
        }

        /// <summary>
        /// Обработка нажатие на кнопку "таблица лидеров"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ScoreBoardButton_Click(object sender, EventArgs e)
        {
            LoadLeaderboardFromDatabase();
        }
    }
}
