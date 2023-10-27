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
    public class databaseManager
    {

        private string connectionString;

        public databaseManager(string databaseFilePath)
        {
            connectionString = $"Data Source={databaseFilePath};Version=3;";
        }

        public void SaveGameDataTemp(string tableLayoutPanelData, int playerScore, int rowCount, string username)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand cmd = new SQLiteCommand("INSERT OR REPLACE INTO TempData (TableLayoutPanelData, PlayerScore, RowCount, Username) VALUES (@TableLayoutPanelData, @PlayerScore, @RowCount, @Username)", connection))
                {
                    cmd.Parameters.AddWithValue("@TableLayoutPanelData", tableLayoutPanelData);
                    cmd.Parameters.AddWithValue("@PlayerScore", playerScore);
                    cmd.Parameters.AddWithValue("@RowCount", rowCount);
                    cmd.Parameters.AddWithValue("@Username", username);

                    cmd.ExecuteNonQuery();
                }
            }

        }



        public void SaveLeaderboard(int playerScore, string username)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand selectCommand = connection.CreateCommand())
                {
                    selectCommand.CommandText = "SELECT PlayerScore FROM LeaderboardData WHERE Username = @Username";
                    selectCommand.Parameters.AddWithValue("@Username", username);

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
                                    updateCommand.Parameters.AddWithValue("@Username", username);

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
                                insertCommand.Parameters.AddWithValue("@Username", username);

                                insertCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
        }


        public void SaveGameData(string tableLayoutPanelData, int playerScore, int rowCount, string username)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand selectCommand = connection.CreateCommand())
                {
                    selectCommand.CommandText = "SELECT PlayerScore FROM GameData WHERE Username = @Username";
                    selectCommand.Parameters.AddWithValue("@Username", username);

                    using (SQLiteDataReader reader = selectCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            using (SQLiteCommand updateCommand = connection.CreateCommand())
                            {
                                updateCommand.CommandText = "UPDATE GameData SET PlayerScore = @PlayerScore, TableLayoutPanelData = @TableLayoutPanelData, RowCount = @RowCount WHERE Username = @Username";
                                updateCommand.Parameters.AddWithValue("@TableLayoutPanelData", tableLayoutPanelData);
                                updateCommand.Parameters.AddWithValue("@PlayerScore", playerScore);
                                updateCommand.Parameters.AddWithValue("@RowCount", rowCount);
                                updateCommand.Parameters.AddWithValue("@Username", username);

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
                                insertCommand.Parameters.AddWithValue("@RowCount", rowCount);
                                insertCommand.Parameters.AddWithValue("@Username", username);

                                insertCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
        }




        public (string tableLayoutPanelData, int playerScore, int rowCount) LoadGameData()
        {
            string tableLayoutPanelData = null;
            int playerScore = 0;
            int rowCount = 0;

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
                            rowCount = reader.GetInt32(2);
                        }
                    }
                }
            }

            return (tableLayoutPanelData, playerScore, rowCount);
        }

        public (string Username, string tableLayoutPanelData, int PlayerScore, int rowCount) LoadGameDataTemp()
        {
            string Username = null;
            string tableLayoutPanelData = null;
            int PlayerScore = 0;
            int rowCount = 0;

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
                            rowCount = Convert.ToInt32(reader["RowCount"]);
                        }
                    }
                }

            }
            return (Username, tableLayoutPanelData, PlayerScore, rowCount);

        }
        public List<(string username, int playerScore)> LoadLeaderboard()
        {
            List<(string username, int playerScore)> leaderboard = new List<(string, int)>();

            string sql = "SELECT Username, playerScore FROM LeaderboardData WHERE playerScore >0 ORDER BY playerScore DESC;";

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
