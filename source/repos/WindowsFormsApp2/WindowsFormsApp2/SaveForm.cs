using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp2;

namespace WindowsFormsApp2
{
    class SaveForm : Form
    {
        private TextBox playerNameTextBox;
        private Button continueButton;
        private databaseManager dbManager;
        public SaveForm()
        {
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
        }
        private void ContinueButton_Click(object sender, EventArgs e)
        {
            string playerName = playerNameTextBox.Text;
            if (string.IsNullOrWhiteSpace(playerName))
            {
                MessageBox.Show("Введите имя");
            }
            else if (dbManager.IsPlayerNameAvailable(playerName))
            {
                this.Close();
                
            }
            else
            {
                MessageBox.Show("Это имя уже занято! Введите другое");
                playerNameTextBox.Text = string.Empty;
            }
        }
    }

}

