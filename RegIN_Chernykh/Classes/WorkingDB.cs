using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegIN_Chernykh.Classes
{
    public class WorkingDB
    {
        ///<summary>
        /// Строка подключения к базе данных, указывается сервер, порт подключения, база данных, имя пользователя, пароль пользователя
        /// </summary>
        /// </summary>
        readonly static string connection = "server=localhost;port=3306;database=regin;user=root;pwd=root;";
        ///<summary>
        /// Создание и открытие подключения
        ///</summary>
        /// <returns>Открытое подключение или null</returns>
        public static MySqlConnection OpenConnection()
        {
            try
            {
                MySqlConnection mySqlConnection = new MySqlConnection(connection);
                mySqlConnection.Open();
                return mySqlConnection;
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.Message);
                return null;
            }
        }
        /// <summary>
        /// Функция выполнения SQL-запросов
        /// </summary>
        /// <returns></returns>
        public static MySqlDataReader Query(string Sql, MySqlConnection mySqlConnection)
        {
            MySqlCommand mySqlCommand = new MySqlCommand(Sql,mySqlConnection);
            return mySqlCommand.ExecuteReader();
        }
        ///<summary>
        /// Функция закрытия соединения с базой данных
        ///</summary>
        /// <param name="mySqlConnection">Открытое MySQL соединение</param>
        public static void CloseConnection(MySqlConnection mySqlConnection)
        {
            mySqlConnection.Close();
            MySqlConnection.ClearPool(mySqlConnection);
        }
        ///<summary>
        /// Функция проверки соединение на работоспособность
        /// </summary>
        /// <param name="mySqlConnection">MySQL соединения</param>
        /// <returns>Статус работоспособности соединения</returns>

        public static bool OpenConnection(MySqlConnection mySqlConnection)
        {
            return mySqlConnection != null && mySqlConnection.State == System.Data.Connectionatate.Open;

        }





    }
}
