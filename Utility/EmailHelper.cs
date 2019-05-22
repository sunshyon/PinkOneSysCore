using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Utility
{
    public class EmailHelper
    {

        public static void SendEmail(string mailParamJson,string logDic)
        {           
            try
            {
                Dictionary<string, object> dicParam = JsonHelper.JsonToT<Dictionary<string, object>>(mailParamJson);
                string smtpServer = dicParam["SMTPServer"].ToString();
                string mailFrom = dicParam["MailFrom"].ToString();
                string userPwd = dicParam["FromUserPwd"].ToString();//SMTP授权密码
                string mailTo = dicParam["MailTo"].ToString();
                string mailCCs = dicParam["MailCCs"].ToString();
                string mailSubject = dicParam["MailSubject"].ToString();
                string mailBody = dicParam["MailBody"].ToString();
                //char[] ccList = mailCC.ToArray<string>();
                // 邮件服务设置
                SmtpClient smtpClient = new SmtpClient();
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;//指定电子邮件发送方式
                smtpClient.Host = smtpServer; //指定SMTP服务器
                smtpClient.Port = 25;
                smtpClient.Credentials = new System.Net.NetworkCredential(mailFrom, userPwd);//用户名和密码

                // 发送邮件设置       
                MailMessage mailMessage = new MailMessage();// (mailFrom, mailTo); // 发送人和收件人
                mailMessage.From = new MailAddress(mailFrom);
                mailMessage.To.Add(mailTo);
                if (mailCCs.Contains("@"))
                {
                    if (mailCCs.Contains(","))
                    {
                        string[] ccArr = mailCCs.Split(',');
                        foreach (var item in ccArr)
                        {
                            if (item.Contains("@"))
                                mailMessage.CC.Add(item);
                        }
                    }
                    else
                        mailMessage.CC.Add(mailCCs);
                }
                mailMessage.Subject = mailSubject;//主题
                mailMessage.Body = mailBody;//内容
                mailMessage.BodyEncoding = Encoding.UTF8;//正文编码
                if (mailBody.Contains("</"))
                    mailMessage.IsBodyHtml = true;//设置为HTML格式
                else
                    mailMessage.IsBodyHtml = false;
                mailMessage.Priority = MailPriority.Normal;//优先级

                ////如果发送失败，SMTP 服务器将发送 失败邮件告诉我  
                //mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                smtpClient.Send(mailMessage); // 发送邮件
            }
            catch (Exception e)
            {
                LogHelper.Error("发送邮件错误："+e.Message);
            }
        }
    }
}
