using System;
using System.Windows.Forms;
using WindowsFormsApp2;


namespace WindowsFormsApp2
{
    class Program
    {
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            GameForm gameForm1 = new GameForm(); // Создайте GameForm, который инициализирует DatabaseManager внутри конструктора
            gameForm1.Initialize();

            // Получите DatabaseManager из GameForm
            databaseManager dbManager = gameForm1.GetDatabaseManager();

            SaveForm saveForm = new SaveForm(dbManager, gameForm1); // Передайте DatabaseManager в конструктор SaveForm
            
            saveForm.ShowDialog(); // Откройте SaveForm

            
            // После закрытия SaveForm, получите User и передайте его в GameForm
            User user = saveForm.GetUser();
            gameForm1.SetUser(user); // Передайте пользователя в GameForm


          
            Application.Run(gameForm1.gameForm);

        }
    }
}

