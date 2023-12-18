using System.Collections.Generic;
using UnityEngine;

namespace AKIRA.Manager {
    /// <summary>
    /// 报告配置文件
    /// </summary>
    internal class ReportConfig : ScriptableObject {
        [CNName("开启 stmp 授权码")]
        public string authorizationCode;
        [CNName("授权端 QQ,163,网易等")]
        public string smtpClient;
        [CNName("端口（可要可不要）")]
        public string port;
        // [CNName("邮件主题")]
        // public string subject;
        // [CNName("邮件内容")]
        // public string body;
        // [CNName("附件")]
        // public List<string> attachments = new List<string>();
        [CNName("收件人")]
        public List<string> tos = new List<string>();

        public bool IsUseable() {
            if (string.IsNullOrWhiteSpace(authorizationCode)) {
                $"ReportConfig: 授权码为空".Log();
                return false;
            }

            if (string.IsNullOrWhiteSpace(smtpClient)) {
                $"ReportConfig: 授权端为空".Log();
                return false;
            }

            if (tos.Count == 0) {
                $"ReportConfig: 收件人为空".Log();
                return false;
            }

            return true;
        }
    }
}