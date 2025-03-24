using House.Model;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace House.CBL
{
    public static class SecurityUtil
    {
        private static RNGCryptoServiceProvider rngp = new RNGCryptoServiceProvider();
        private static byte[] rb = new byte[4];
        private static string publicKeyPath = "PublicKey.xml";
        private static string privateKeyPath = "PrivateKey.xml";

        private static string publicKeyPemPath = "PublicKey.pem";
        private static string privateKeyPemPath = "PrivateKey.pem";
        private static string _password = "mm$$20240101@@";
        private static string _salt = "MapMarker$";
        #region AES

        public static string AesEncrypt(this string plainText)
        {
            if (plainText == null)
                return null;

            var aes = new AesManaged();
            var rfc2898 = new Rfc2898DeriveBytes(_password, Encoding.UTF8.GetBytes(_salt));
            aes.Key = rfc2898.GetBytes(aes.KeySize / 8);
            aes.IV = rfc2898.GetBytes(aes.BlockSize / 8);

            var source = Encoding.UTF8.GetBytes(plainText);
            var encData = aes.CreateEncryptor().TransformFinalBlock(source, 0, source.Length);

            var cipherText = Uri.EscapeDataString(Convert.ToBase64String(encData.ToArray()));
            return cipherText;
        }

        public static string AesDecrypt(this string cipherText)
        {
            if (cipherText == null)
                return null;

            var rfc2898 = new Rfc2898DeriveBytes(_password, Encoding.UTF8.GetBytes(_salt));
            var aes = new AesManaged();
            aes.Key = rfc2898.GetBytes(aes.KeySize / 8);
            aes.IV = rfc2898.GetBytes(aes.BlockSize / 8);

            using (var stream = new MemoryStream())
            {
                using (var stream2 = new CryptoStream(stream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    var buffer = Convert.FromBase64String(Uri.UnescapeDataString(cipherText));
                    stream2.Write(buffer, 0, buffer.Length);
                    stream2.FlushFinalBlock();

                    var buffer2 = stream.ToArray();

                    var plainText = Encoding.UTF8.GetString(buffer2, 0, buffer2.Length);

                    return plainText;
                }
            }
        }

        #endregion

        #region SHA
        /// <summary>
        /// sha256
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Sha256(this string value)
        {
            SHA256 sha256 = new SHA256CryptoServiceProvider();//建立一個SHA256
            byte[] source = Encoding.UTF8.GetBytes(value);//將字串轉為Byte[]
            byte[] crypto = sha256.ComputeHash(source);//進行SHA256加密

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < crypto.Length; i++)
            {
                sb.Append(crypto[i].ToString("x2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 對接 .NET Core 特殊字元編碼%後英文為小寫問題 & 空格轉換為+號問題
        /// </summary>
        /// <param name="FrameworkEncodeString"></param>
        /// <returns></returns>
        public static string Sha256_DotNetCore(string FrameworkEncodeString)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char cher_FrameworkEncodeString in FrameworkEncodeString)
            {
                if (WebUtility.UrlEncode(cher_FrameworkEncodeString.ToString()).Length > 1)
                {
                    builder.Append(WebUtility.UrlEncode(cher_FrameworkEncodeString.ToString()).ToLower());
                }
                else
                {
                    // .Net Core 空白會用+號代替
                    if (cher_FrameworkEncodeString.ToString() == " ")
                    {
                        builder.Append("+");
                    }
                    else
                    {
                        builder.Append(cher_FrameworkEncodeString);
                    }
                }
            }
            return Sha256(builder.ToString());
        }



        /// <summary>
        /// sha512
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Sha512(this string value)
        {
            SHA512 sha512 = new SHA512CryptoServiceProvider();//建立一個SHA
            byte[] source = Encoding.UTF8.GetBytes(value);//將字串轉為Byte[]
            byte[] crypto = sha512.ComputeHash(source);//進行SHA加密
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < crypto.Length; i++)
            {
                sb.Append(crypto[i].ToString("x2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// AppPWD_密碼SHA用
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string AppPWDSha256(this string value, string value2)
        {
          return  SecurityUtil.Sha256_Big5(value+SecurityUtil.Sha256_Big5(value2));
        }

        /// <summary>
        /// sha256
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Sha256_Big5(this string value)
        {
            SHA256 sha256 = new SHA256CryptoServiceProvider();//建立一個SHA256
            byte[] source = Encoding.GetEncoding("Big5").GetBytes(value);//將字串轉為Byte[]
            byte[] crypto = sha256.ComputeHash(source);//進行SHA256加密

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < crypto.Length; i++)
            {
                sb.Append(crypto[i].ToString("x2"));
            }
            return sb.ToString().ToLower();
        }
        #endregion

        #region Base-64

        public static string C85(string strInput)
        {
            var strut8 = Convert.FromBase64String(strInput);
            var strbig5 = Encoding.Convert(Encoding.UTF8, Encoding.Default, strut8);

            return Encoding.Default.GetString(strbig5);
        }

        public static string ToBase64(this string value)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
        }

        public static byte[] FromBase64(this string value)
        {
            return Convert.FromBase64String(value);
        }

        #endregion

        #region MD5
        /// <summary>
        /// 對 URL 參數加簽章。
        /// </summary>
        /// <param name="urlParams"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        public static string UrlSign(this StringBuilder urlParams, string sign, string signType = "MD5") {
            var paramStr = HttpUtility.UrlDecode(urlParams.ToString());
            var signStr = Md5(paramStr + sign);

            urlParams.Append("&sign=" + signStr);
            urlParams.Append("&sign_type=" + "MD5");

            return signStr;
        }

        public static string Md5(this string value, string salt)
        {
            return Md5(value + salt);
        }
        /// <summary>
        /// 精進7 避免Heap Inspection
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Md5(this string value)
        {
            var original = Encoding.UTF8.GetBytes(value);
            var md5 = MD5.Create(); //使用MD5 
            var hashValue = md5.ComputeHash(original);//進行加密 
            var sb = new StringBuilder();

            foreach (byte t in hashValue)
            {
                sb.Append(t.ToString("x2"));
            }

            return sb.ToString();
        }

        #endregion

        #region Mask

        /// <summary>
        /// 遮罩中間一半的文字
        /// </summary>
        /// <param name="originalText">原文</param>
        /// <param name="maskChar">遮罩字元</param>
        /// <param name="maxMaskCount">允許遮罩的最大字元數</param>
        /// <returns>遮罩後的文字</returns>
        public static string MaskHalf(this string originalText, char maskChar = '*', int? maxMaskCount = null)
        {
            if (string.IsNullOrEmpty(originalText))
                return string.Empty;
            else if (originalText.Length <= 2)
                return originalText.Substring(0, 1) + "*";
            else if (originalText.Length == 3)
                return originalText.Substring(0, 1) + "*" + originalText.Substring(2, 1);
            else
            {
                var maskNums = originalText.Length/2;

                if (maxMaskCount.HasValue)
                    maskNums = Math.Min(maskNums, maxMaskCount.Value);

                var maskIndex = Convert.ToInt32(Math.Ceiling(originalText.Length/4d));

                return originalText.Substring(0, maskIndex - 1) +
                       new string(maskChar, maskNums) +
                       originalText.Substring(maskIndex + maskNums);
            }
        }

        /// <summary>
        /// 遮罩中間的文字。當只有一個字時不遮罩，只有兩個字時遮罩第二個字，三個字以上時只有頭尾不遮罩。
        /// </summary>
        /// <param name="originalText">原文</param>
        /// <param name="maskChar">遮罩字元</param>
        /// <returns>遮罩後的文字</returns>
        public static string MaskMiddle(this string originalText, char maskChar = '*')
        {
            if (string.IsNullOrEmpty(originalText))
                return string.Empty;
            else if (originalText.Length == 1)
                return originalText;
            else if (originalText.Length == 2)
                return originalText.Substring(0, 1) + maskChar;
            else
                return originalText.First() + new string(maskChar, originalText.Length - 2) + originalText.Last();
        }

        /// <summary>
        /// 遮罩偶數字元
        /// </summary>
        /// <param name="originalText">原文</param>
        /// <param name="maskChar">遮罩字元</param>
        /// <returns>遮罩後的文字</returns>
        public static string MaskEven(this string originalText, char maskChar = '*')
        {
            if (string.IsNullOrEmpty(originalText))
                return originalText;
            else
            {
                var maskText = new StringBuilder();
                var position = 1;
                foreach (var chr in originalText)
                {
                    maskText.Append(position%2 == 1 ? chr : maskChar);
                    position++;
                }

                return maskText.ToString();
            }
        }

        /// <summary>
        /// 從指定的索引值開始遮罩
        /// </summary>
        /// <param name="originalText">原文</param>
        /// <param name="startMaskIndex">開始遮罩的索引值</param>
        /// <param name="maskNumbers">遮罩字數</param>
        /// <param name="maskChar">遮罩字元</param>
        /// <returns>遮罩後的文字</returns>
        public static string Mask(this string originalText, int startMaskIndex, int maskNumbers, char maskChar = '*')
        {
            return originalText.Substring(0, startMaskIndex) +
                   new string(maskChar, maskNumbers) +
                   originalText.Substring(startMaskIndex + maskNumbers);
        }

        public static string Mask(this DateTime plainDateTime, char maskChar = 'X')
        {
            return new string(maskChar, 4) + plainDateTime.ToString("/MM/dd");
        }

        #endregion

        #region Random
        /// <summary>
        /// 產生一個非負數的亂數
        /// </summary>
        public static int RandomNext()
        {
            rngp.GetBytes(rb);
            int value = BitConverter.ToInt32(rb, 0);
            if (value < 0) value = -value;
            return value;
        }
        /// <summary>
        /// 產生一個非負數且最大值 max 以下的亂數
        /// </summary>
        /// <param name="max">最大值</param>
        public static int RandomNext(int max)
        {
            rngp.GetBytes(rb);
            int value = BitConverter.ToInt32(rb, 0);
            value = value % (max + 1);
            if (value < 0) value = -value;
            return value;
        }
        /// <summary>
        /// 產生一個非負數且最小值在 min 以上最大值在 max 以下的亂數
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        public static int RandomNext(int min, int max)
        {
            int value = RandomNext(max - min) + min;
            return value;
        }

        public static string GenRandom(int length = 8, string allowedChars = "abcdefghijklmnopqrstuvwxyz1234567890")
        {
            char[] chars = new char[length];

            for (int retryTimes = 3; retryTimes > 0; retryTimes--)
            {
                for (int i = 0; i < length; i++)
                {
                    //allowedChars -> 這個String ，隨機取得一個字，丟給chars[i]
                    chars[i] = allowedChars[SecurityUtil.RandomNext(length)];
                }
            }
            return new string(chars);
        }
        #endregion

        #region RSA
        /// <summary>
        /// 獲取加密所使用的key，RSA算法是一種非對稱密碼算法，所謂非對稱，就是指該算法需要一對密鑰，使用其中一個加密，則需要用另一個才能解密。
        /// </summary>
        public static void GenKey()
        {
            RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();

            string publicKey = rSACryptoServiceProvider.ToXmlString(false); // 獲取公匙，用於加密
            string privateKey = rSACryptoServiceProvider.ToXmlString(true); // 獲取公匙和私匙，用於解密

            // 密匙中含有公匙，公匙是根據密匙進行計算得來的。
            using (StreamWriter streamWriter = new StreamWriter(publicKeyPath))
            {
                streamWriter.Write(publicKey);// 將公匙保存到運行目錄下的PublicKey
            }
            using (StreamWriter streamWriter = new StreamWriter(privateKeyPath))
            {
                streamWriter.Write(privateKey); // 將公匙&私匙保存到運行目錄下的PrivateKey
            }
        }

        public static string RSAEncrypto(this string str)
        {
            RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
            using (StreamReader streamReader = new StreamReader(publicKeyPath)) // 讀取運行目錄下的PublicKey.xml
            {
                rSACryptoServiceProvider.FromXmlString(streamReader.ReadToEnd()); // 將公匙載入進RSA實例中
            }
            byte[] buffer = Encoding.UTF8.GetBytes(str); // 將明文轉換為byte[]

            // 加密後的數據就是一個byte[] 數組,可以以 文件的形式保存 或 別的形式(網上很多教程,使用Base64進行編碼化保存)
            byte[] EncryptBuffer = rSACryptoServiceProvider.Encrypt(buffer, false); // 進行加密

            string EncryptBase64 = Convert.ToBase64String(EncryptBuffer); // 如果使用base64進行明文化，在解密時 需要再次將base64 轉換為byte[]
            //Console.WriteLine(EncryptBase64);
            return EncryptBase64;
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string RSADecrypto(this string base64String)
        {
            byte[] buffer = Convert.FromBase64String(base64String);
            RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
            using (StreamReader streamReader = new StreamReader(privateKeyPath)) // 讀取運行目錄下的PrivateKey.xml
            {
                rSACryptoServiceProvider.FromXmlString(streamReader.ReadToEnd()); // 將私匙載入進RSA實例中
            }
            // 解密後得到一個byte[] 數組
            byte[] DecryptBuffer = rSACryptoServiceProvider.Decrypt(buffer, false); // 進行解密
            string str = Encoding.UTF8.GetString(DecryptBuffer); // 將byte[]轉換為明文

            return str;
        }

        public static void GenPemKey()
        {
            RsaKeyPairGenerator rsaGenerator = new RsaKeyPairGenerator();
            rsaGenerator.Init(new KeyGenerationParameters(new SecureRandom(), 2048));
            var keyPair = rsaGenerator.GenerateKeyPair();


            using (TextWriter privateKeyTextWriter = new StringWriter())
            {

                PemWriter pemWriter = new PemWriter(privateKeyTextWriter);
                pemWriter.WriteObject(keyPair.Private);
                pemWriter.Writer.Flush();
                File.WriteAllText(privateKeyPemPath, privateKeyTextWriter.ToString());
            }


            using (TextWriter publicKeyTextWriter = new StringWriter())
            {

                PemWriter pemWriter = new PemWriter(publicKeyTextWriter);
                pemWriter.WriteObject(keyPair.Public);
                pemWriter.Writer.Flush();

                File.WriteAllText(publicKeyPemPath, publicKeyTextWriter.ToString());
            }
        }

        public static string RSAPemEncrypto(this string str)
        {
            RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
            using (TextReader publicKeyTextReader = new StringReader(File.ReadAllText(publicKeyPemPath)))
            {
                RsaKeyParameters publicKeyParam = (RsaKeyParameters)new PemReader(publicKeyTextReader).ReadObject();

                RSAParameters parms = new RSAParameters();

                parms.Modulus = publicKeyParam.Modulus.ToByteArrayUnsigned();
                parms.Exponent = publicKeyParam.Exponent.ToByteArrayUnsigned();

                rSACryptoServiceProvider.ImportParameters(parms);
            }

            byte[] buffer = Encoding.UTF8.GetBytes(str); // 將明文轉換為byte[]

            // 加密後的數據就是一個byte[] 數組,可以以 文件的形式保存 或 別的形式(網上很多教程,使用Base64進行編碼化保存)
            byte[] EncryptBuffer = rSACryptoServiceProvider.Encrypt(buffer, false); // 進行加密

            string EncryptBase64 = Convert.ToBase64String(EncryptBuffer); // 如果使用base64進行明文化，在解密時 需要再次將base64 轉換為byte[]
            //Console.WriteLine(EncryptBase64);
            return EncryptBase64;
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string RSAPemDecrypto(this string base64String)
        {
            RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
            using (TextReader privateKeyTextReader = new StringReader(File.ReadAllText(privateKeyPemPath)))
            {
                AsymmetricCipherKeyPair readKeyPair = (AsymmetricCipherKeyPair)new PemReader(privateKeyTextReader).ReadObject();


                RsaPrivateCrtKeyParameters privateKeyParams = ((RsaPrivateCrtKeyParameters)readKeyPair.Private);

                RSAParameters parms = new RSAParameters();

                parms.Modulus = privateKeyParams.Modulus.ToByteArrayUnsigned();
                parms.P = privateKeyParams.P.ToByteArrayUnsigned();
                parms.Q = privateKeyParams.Q.ToByteArrayUnsigned();
                parms.DP = privateKeyParams.DP.ToByteArrayUnsigned();
                parms.DQ = privateKeyParams.DQ.ToByteArrayUnsigned();
                parms.InverseQ = privateKeyParams.QInv.ToByteArrayUnsigned();
                parms.D = privateKeyParams.Exponent.ToByteArrayUnsigned();
                parms.Exponent = privateKeyParams.PublicExponent.ToByteArrayUnsigned();

                rSACryptoServiceProvider.ImportParameters(parms);

            }

            byte[] buffer = Convert.FromBase64String(base64String);

            // 解密後得到一個byte[] 數組
            byte[] DecryptBuffer = rSACryptoServiceProvider.Decrypt(buffer, false); // 進行解密
            string str = Encoding.UTF8.GetString(DecryptBuffer); // 將byte[]轉換為明文

            return str;
        }
        #endregion

        public static string HashPassword(this string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] saltBytes = Encoding.UTF8.GetBytes(_salt);

                byte[] combinedBytes = new byte[passwordBytes.Length + saltBytes.Length];
                Buffer.BlockCopy(passwordBytes, 0, combinedBytes, 0, passwordBytes.Length);
                Buffer.BlockCopy(saltBytes, 0, combinedBytes, passwordBytes.Length, saltBytes.Length);

                byte[] hashedBytes = sha256.ComputeHash(combinedBytes);
                return Convert.ToBase64String(hashedBytes);
            }
        }

        /// <summary>
        /// 產生JWT
        /// </summary>
        /// <param name="memberID">會員編號</param>
        /// <returns></returns>
        public static string GenToken(
            object payload,
            string privateKey
            )
        {
            using (var rsa = ImportPrivateKey(privateKey)) //讀取私鑰
            {
                var token = Jose.JWT.Encode(payload, rsa, Jose.JwsAlgorithm.RS256);
                return token;
            }
        }

        /// <summary>
        /// 轉換Unit time
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        private static long GetEpochDateTimeAsInt(DateTime datetime)
        {
            DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);
            return (long)(datetime - UnixEpoch).TotalSeconds;
        }

        /// <summary>
        /// RSA private key 轉換
        /// </summary>
        /// <returns></returns>
        private static RSACryptoServiceProvider ImportPrivateKey(string privateKey)
        {
            PemReader pr = new PemReader(new StringReader(privateKey));
            AsymmetricCipherKeyPair KeyPair = (AsymmetricCipherKeyPair)pr.ReadObject();
            RSAParameters rsaParams = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)KeyPair.Private);
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(rsaParams);
            return rsa;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pem"></param>
        /// <returns></returns>
        public static RSACryptoServiceProvider ImportPublicKey(string publicKey)
        {
            PemReader pr = new PemReader(new StringReader(publicKey));
            AsymmetricKeyParameter publicKeyObj = (AsymmetricKeyParameter)pr.ReadObject();
            RSAParameters rsaParams = DotNetUtilities.ToRSAParameters((RsaKeyParameters)publicKeyObj);

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(rsaParams);
            return rsa;
        }

        public static RsaSecurityKey GetPublicKey(string publicKey)
        {
            var rsaProvider = ImportPublicKey(publicKey);
            return new RsaSecurityKey(rsaProvider);
        }
    }
}
