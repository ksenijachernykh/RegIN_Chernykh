using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace RegIN_Chernykh.Classes
{
    public class SendMail
    {
        ///<summary>
        ///Функция отправки сообщения
        ///</summary>
        ///<param name="Message">Сообщение которое необходимо отправить</param>
        ///<param name="To">Почта на которую отправляется сообщение</param>
        public static void SendMessage(string Message, string To)
        {
            var smtpClient = new SmtpClient("smtp.yandex.ru")
            {
                Port 587,
                Credentials = new NetworkCredential("yandexdyandex.ru", "password"),
                EnableSs1 = true,

            };
            smtpClient.Send("landaxer@yandex.ru", То, "Проект RegIn", Message);
        }

    }
}
