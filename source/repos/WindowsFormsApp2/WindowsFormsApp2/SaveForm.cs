using System;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    /// <summary>
    /// Класс пользователь для хранения имени, полученного от пользователя
    /// </summary>
    class User
    {
        public string Username { get; set; }

        /// <summary>
        /// Конструктор присвоение имени
        /// </summary>
        /// <param name="username"></param>
        public User(string username)
        {
            Username = username;
        }
    }

    /// <summary>
    /// Класс для записи имени пользователя
    /// </summary>
    class SaveForm : Form
    {
        private TextBox usernameTextBox; // Изменено на usernameTextBox
        private Button continueButton;
        private databaseManager dbManager;
        private User user; // Добавляем экземпляр класса User для хранения Username
        private GameForm gameForm; // Добавляем экземпляр GameForm
        private bool ContinueClicked = false;

        /// <summary>
        /// Конструктор создания формы и экземпляров 
        /// </summary>
        /// <param name="dbManager"></param>
        /// <param name="gameForm"></param>
        public SaveForm(databaseManager dbManager, GameForm gameForm)
        {
            this.dbManager = dbManager;
            this.gameForm = gameForm; // Сохраняем экземпляр GameForm

            this.Text = "Имя пользователя";
            this.Size = new System.Drawing.Size(1920, 1080);

            usernameTextBox = new TextBox(); // Изменено на usernameTextBox
            usernameTextBox.Location = new System.Drawing.Point(500, 500);
            usernameTextBox.Size = new System.Drawing.Size(200, 25);

            continueButton = new Button();
            continueButton.Text = "Продолжить";
            continueButton.Location = new System.Drawing.Point(500, 530);
            continueButton.AutoSize = true;
            continueButton.Click += ContinueButton_Click;

            this.Controls.Add(usernameTextBox); // Изменено на usernameTextBox
            this.Controls.Add(continueButton);

            this.FormClosing += SaveForm_FormClosing;
        }

        /// <summary>
        /// Обработка события FormClosing после нажатия на кнопку 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ContinueClicked) { }
            else
                Application.Exit();
        }

        /// <summary>
        /// Кнопка перехода на новую форму
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContinueButton_Click(object sender, EventArgs e)
        {
            string username = usernameTextBox.Text; // Изменено на username
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Введите имя");
            }
            else if (!dbManager.IsPlayerNameAvailable(username))
            {
                ContinueClicked = true;
                user = new User(username);

                // Вызываем метод из GameForm, передавая имя игрока
                gameForm.LoadGameDataFromDatabaseByUsername(username);

                this.Close();
            }
            else if (dbManager.IsPlayerNameAvailable(username))
            {
                ContinueClicked = true;
                user = new User(username);
                this.Close();
            }
        }

        
        /// <summary>
        /// Метод, чтобы получить экземпляр User с Username
        /// </summary>
        /// <returns></returns>
        public User GetUser()
        {
            return user;
        }
    }
}
