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

            UserInterface userInterface = new UserInterface();
            userInterface.Initialize();

            // Получаем DatabaseManager из GameForm
            databaseManager dbManager = userInterface.GetDatabaseManager();

            SaveForm saveForm = new SaveForm(dbManager, userInterface); // Передайте DatabaseManager в конструктор SaveForm
            
            saveForm.ShowDialog(); // Откройте SaveForm

            
            // После закрытия SaveForm, получаем User и передаём его в GameForm
            User user = saveForm.GetUser();
            userInterface.SetUser(user); // Передаём пользователя в GameForm


          
            Application.Run(userInterface.gameForm);

        }
    }
}

