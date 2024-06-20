using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace AKIRA.Security
{
    /// <summary>
    /// <para>加密解密</para>
    /// </summary>
    public static class SecurityTool
    {
        #region ========加密========
        public static byte[] Encrypt(byte[] encryptString, string encryptKey, string encryptIv)
        {
            byte[] cryptograph = null;
            Rijndael Aes = Rijndael.Create();
            using (MemoryStream Memory = new MemoryStream())
            {
                var transform = Aes.CreateEncryptor(AesKey(encryptKey), AesKey(encryptIv));
                using (CryptoStream Encryptor = new CryptoStream(Memory, transform, CryptoStreamMode.Write))
                {
                    Encryptor.Write(encryptString, 0, encryptString.Length);
                    Encryptor.FlushFinalBlock();
                    cryptograph = Memory.ToArray();
                }
                transform.Dispose();
            }
            return cryptograph;
        }
        #endregion

        #region ========解密========
        public static byte[] Decrypt(byte[] decryptString, string decryptKey, string encryptIv)
        {
            byte[] original = null;
            Rijndael Aes = Rijndael.Create();
            using (var Memory = new System.IO.MemoryStream(decryptString))
            {
                var transform = Aes.CreateDecryptor(AesKey(decryptKey), AesKey(encryptIv));
                using (CryptoStream Decryptor = new CryptoStream(Memory, transform, CryptoStreamMode.Read))
                {
                    using (var originalMemory = new System.IO.MemoryStream())
                    {
                        byte[] Buffer = new byte[1024];
                        int readBytes = 0;
                        while ((readBytes = Decryptor.Read(Buffer, 0, Buffer.Length)) > 0)
                        {
                            originalMemory.Write(Buffer, 0, readBytes);
                        }
                        original = originalMemory.ToArray();
                    }
                }
                transform.Dispose();
            }
            return original;
        }
        #endregion

        #region 密钥
        /// <summary>
        /// 限制16位密钥
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static byte[] AesKey(string key)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(key);
            byte[] keyBytes = new byte[16];
            for (int i = 0; i < bytes.Length; i++)
            {
                keyBytes[i % 16] = (byte)(keyBytes[i % 16] ^ bytes[i]);
            }
            return keyBytes;
        }
        #endregion
    }
}