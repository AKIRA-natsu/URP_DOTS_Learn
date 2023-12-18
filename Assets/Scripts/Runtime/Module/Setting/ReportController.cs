using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using UnityEngine;

namespace AKIRA.Manager {
    /// <summary>
    /// <para>崩溃报告</para>
    /// <para>来源：https://blog.csdn.net/Xz616/article/details/128897190?utm_medium=distribute.pc_feed_404.none-task-blog-2~default~BlogCommendFromBaidu~Rate-3-128897190-blog-null.262^v1^pc_404_mixedpudn&depth_1-utm_source=distribute.pc_feed_404.none-task-blog-2~default~BlogCommendFromBaidu~Rate-3-128897190-blog-null.262^v1^pc_404_mixedpud</para>
    /// </summary>
    internal class ReportController : IController {
        private ReportConfig config;

        public async Task Initialize() {
            await Task.Yield();
            config = GameConfig.Instance.GetConfig<ReportConfig>();
            EventSystem.Instance.AddEventListener(GameData.Event.OnTestReportEmail, TestReportEmail);
        }

        private void TestReportEmail(object obj) {
            SendEmail(obj.ToString(), "Test", "this is a test email");
        }

        /// <summary>
        /// 发送邮箱
        /// </summary>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <returns></returns>
        private bool SendEmail(string email, string subject, string body) {
            if (config == null) {
                $"ReportController: 报告发送配置文件无法在GameConfig找到".Log(GameData.Log.Error);
                return false;
            }

            if (!config.IsUseable())
                return false;

            // 创建邮件，设置发件人
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(email);
            foreach (var to in config.tos)
                mail.To.Add(to);

            // 创建文件夹保存日志和截图
            var dir = Directory.CreateDirectory(Path.Combine(Application.dataPath, "Report", DateTime.Now.ToString().Replace("/", ".").Replace(":", ".")));

            // 设置邮件主题和内容
            mail.Subject = subject;
            mail.Body = body;

            // 设置截图附件
            // 截图不会立刻存在，暂时不处理
            // var screenShotPath = Path.Combine(dir.FullName, "ScreenShot.png");
            // ScreenCapture.CaptureScreenshot(screenShotPath);
            // mail.Attachments.Add(new Attachment(screenShotPath));

            // 绑定开通 smtp 服务的邮箱 credentials登录smtp服务器的身份验证
            SmtpClient smtpClient = new SmtpClient(config.smtpClient);
            smtpClient.Credentials = new NetworkCredential(email, config.authorizationCode) as ICredentialsByHost;
            smtpClient.EnableSsl = true;

            ServicePointManager.ServerCertificateValidationCallback = 
                delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
                    return true;
                };

            // 发送邮件
            smtpClient.Send(mail);
            $"发送邮件".Log(GameData.Log.Success);

            return true;
        }
    }

    
}