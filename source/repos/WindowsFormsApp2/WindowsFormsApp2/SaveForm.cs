using System;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    class User
    {
        public string Username { get; set; }

        public User(string username)
        {
            Username = username;
        }
    }

    class SaveForm : Form
    {
        private TextBox playerNameTextBox;
        private Button continueButton;
        private databaseManager dbManager;
        private User user; // Добавляем экземпляр класса User для хранения Username
        private GameForm gameForm; // Добавляем экземпляр GameForm
        private bool ContinueClicked = false;

        public SaveForm(databaseManager dbManager, GameForm gameForm)
        {
            this.dbManager = dbManager;
            this.gameForm = gameForm; // Сохраняем экземпляр GameForm

            this.Text = "Имя пользователя";
            this.Size = new System.Drawing.Size(1920, 1080);

            playerNameTextBox = new TextBox();
            playerNameTextBox.Location = new System.Drawing.Point(500, 500);
            playerNameTextBox.Size = new System.Drawing.Size(200, 25);

            continueButton = new Button();
            continueButton.Text = "Продолжить";
            continueButton.Location = new System.Drawing.Point(500, 530);
            continueButton.AutoSize = true;
            continueButton.Click += ContinueButton_Click;

            this.Controls.Add(playerNameTextBox);
            this.Controls.Add(continueButton);

            this.FormClosing += SaveForm_FormClosing;
        }

        private void SaveForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ContinueClicked){ }
            else
            Application.Exit();
        }

        private void ContinueButton_Click(object sender, EventArgs e)
        {
            string playerName = playerNameTextBox.Text;
            if (string.IsNullOrWhiteSpace(playerName))
            {
                MessageBox.Show("Введите имя");
            }
            else if (!dbManager.IsPlayerNameAvailable(playerName))
            {
                ContinueClicked = true;
                user = new User(playerName);
                
                // Вызываем метод из GameForm, передавая имя игрока
                gameForm.LoadGameDataFromDatabaseByUsername(playerName);

                this.Close();
            }
            else if (dbManager.IsPlayerNameAvailable(playerName))
            {
                ContinueClicked = true;
                user = new User(playerName);
                this.Close();
            }
        }

        // Добавляем метод, чтобы получить экземпляр User с Username
        public User GetUser()
        {
            return user;
        }
    }
}
