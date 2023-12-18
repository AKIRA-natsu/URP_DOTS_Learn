#if !UNITY_ANDROID
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AKIRA.Manager {
    /// <summary>
    /// <para>崩溃报告</para>
    /// <para>来源：https://blog.csdn.net/Xz616/article/details/128897190?utm_medium=distribute.pc_feed_404.none-task-blog-2~default~BlogCommendFromBaidu~Rate-3-128897190-blog-null.262^v1^pc_404_mixedpudn&depth_1-utm_source=distribute.pc_feed_404.none-task-blog-2~default~BlogCommendFromBaidu~Rate-3-128897190-blog-null.262^v1^pc_404_mixedpud</para>
    /// </summary>
    internal class ReportController : IController, IUpdate {
        // 配置文件
        private ReportConfig config;
        // 检查时间 5min
        private const int CheckTime = 300;
        // 日志记录
        private StringBuilder logs = new StringBuilder();

        private bool sendMessage = false;

        public async Task Initialize() {
            await Task.Yield();
            config = GameConfig.Instance.GetConfig<ReportConfig>();
            EventSystem.Instance.AddEventListener(GameData.Event.OnTestReportEmail, TestReportEmail);

            Application.logMessageReceived += ReportCallback;
            this.Regist(CheckTime, GameData.Group.Other);
        }

        ~ReportController() {
            Application.logMessageReceived -= ReportCallback;
        }

        private void ReportCallback(string condition, string stackTrace, LogType type) {
            logs.Append($"{type}:{condition}\n{stackTrace}\n");

            if (type == LogType.Error && !sendMessage) {
                this.RemoveSpaceUpdate(GameData.Group.Other);
                SendEmail("Error Report -> {[email]}", GetDeviceInfo());
            }
        }

        private void TestReportEmail(object obj) {
            SendEmail($"Test Report -> {obj}", $"This is a test email\n\n{GetDeviceInfo()}");
        }

        /// <summary>
        /// 获得设备信息
        /// </summary>
        /// <returns></returns>
        private string GetDeviceInfo() {
            StringBuilder builder = new StringBuilder();
            builder.Append($"操作系统: {SystemInfo.operatingSystem}\n");
            builder.Append($"设备模型: {SystemInfo.deviceModel}\n");
            builder.Append($"设备名称: {SystemInfo.deviceName}\n");
            builder.Append($"设备类型: {SystemInfo.deviceType}\n");
            builder.Append($"系统内存: {SystemInfo.systemMemorySize} MB\n");
            builder.Append($"显卡名称: {SystemInfo.graphicsDeviceName}\n");
            builder.Append($"显卡类型: {SystemInfo.graphicsDeviceType}\n");
            builder.Append($"显存大小: {SystemInfo.graphicsMemorySize} MB\n");
            return builder.ToString();
        }

        /// <summary>
        /// 发送邮箱
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        private bool SendEmail(string subject, string body) {
            if (config == null) {
                $"ReportController: 报告发送配置文件无法在GameConfig找到".Log(GameData.Log.Error);
                return false;
            }

            if (!config.IsUseable())
                return false;

            sendMessage = true;

            // 创建邮件
            MailMessage mail = new MailMessage {
                // 设置发件人
                From = new MailAddress(config.from),
                // 设置邮件主题和内容
                Subject = subject,
                Body = body,
            };
            foreach (var to in config.tos)
                mail.To.Add(to);

            // 创建文件夹保存日志和截图
            var dir = Directory.CreateDirectory(Path.Combine(Application.dataPath, "Report", DateTime.Now.ToString().Replace("/", ".").Replace(":", ".")));

            // 设置截图附件
            var logPath = Path.Combine(dir.FullName, "Log.txt");
            File.WriteAllText(logPath, logs.ToString());
            mail.Attachments.Add(new Attachment(logPath));
            // 截图不会立刻存在，暂时不处理
            // var screenShotPath = Path.Combine(dir.FullName, "ScreenShot.png");
            // ScreenCapture.CaptureScreenshot(screenShotPath);
            // mail.Attachments.Add(new Attachment(screenShotPath));

            // 绑定开通 smtp 服务的邮箱 credentials登录smtp服务器的身份验证
            SmtpClient smtpClient = new SmtpClient(config.smtpClient);
            smtpClient.Credentials = new NetworkCredential(config.from, config.authorizationCode) as ICredentialsByHost;
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

        public void GameUpdate() {
            logs.Clear();
        }
    }
}
#endif