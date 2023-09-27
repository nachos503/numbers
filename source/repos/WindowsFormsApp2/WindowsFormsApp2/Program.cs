using System;
using System.Windows.Forms;
using WindowsFormsApp2;



    class Program
    {
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            GameForm gameForm = new GameForm();
            gameForm.Initialize();// Запускает инициализацию методов

            Application.Run(gameForm.Form); // Запуск игры 

            //Application.Restart();
        }
    }


