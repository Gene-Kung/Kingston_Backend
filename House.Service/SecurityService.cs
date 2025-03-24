using House.Model;
using Microsoft.Extensions.Options;
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

namespace House.Service
{
    public class SecurityService
    {
        private readonly AppSettings _appSettings;

        public SecurityService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        //public static string _salt = "MapMarker$";

        //public static string HashPassword(this string password)
        //{
        //    using (SHA256 sha256 = SHA256.Create())
        //    {
        //        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
        //        byte[] saltBytes = Encoding.UTF8.GetBytes(_salt);

        //        byte[] combinedBytes = new byte[passwordBytes.Length + saltBytes.Length];
        //        Buffer.BlockCopy(passwordBytes, 0, combinedBytes, 0, passwordBytes.Length);
        //        Buffer.BlockCopy(saltBytes, 0, combinedBytes, passwordBytes.Length, saltBytes.Length);

        //        byte[] hashedBytes = sha256.ComputeHash(combinedBytes);
        //        return Convert.ToBase64String(hashedBytes);
        //    }
        //}

        /// <summary>
        /// 產生JWT
        /// </summary>
        /// <param name="memberID">會員編號</param>
        /// <returns></returns>
        public string GenToken(long memberID, List<string> roleIDs)
        {
            var utcNow = DateTime.UtcNow;
            var payload = new JWTPayload
            {
                iss = _appSettings.JWTDefault.iss,
                sub = memberID,
                role = roleIDs,
                aud = _appSettings.JWTDefault.aud,
                exp = GetEpochDateTimeAsInt(utcNow) + 86400,
                nbf = GetEpochDateTimeAsInt(utcNow) - 180,
                iat = GetEpochDateTimeAsInt(utcNow) - 180,
                jti = Guid.NewGuid().ToString(),
            };

            using (var rsa = ImportPrivateKey()) //讀取私鑰
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
        private RSACryptoServiceProvider ImportPrivateKey()
        {
            string privateKey = _appSettings.PrivateKey;
            //string privateKey = File.ReadAllText("./PrivateKey.pem");

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
        public RSACryptoServiceProvider ImportPublicKey()
        {
            string pubKey = _appSettings.PublicKey;

            PemReader pr = new PemReader(new StringReader(pubKey));
            AsymmetricKeyParameter publicKey = (AsymmetricKeyParameter)pr.ReadObject();
            RSAParameters rsaParams = DotNetUtilities.ToRSAParameters((RsaKeyParameters)publicKey);

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(rsaParams);
            return rsa;
        }

        public RsaSecurityKey GetPublicKey()
        {
            var rsaProvider = ImportPublicKey();
            return new RsaSecurityKey(rsaProvider);
        }
    }
}
