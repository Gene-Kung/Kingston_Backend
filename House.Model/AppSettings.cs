using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace House.Model
{
    public partial class AppSettings
    {
        /// <summary>
        /// 資料庫連線設定
        /// </summary>
        public ConnectionStrings ConnectionStrings { get; set; }
        public ConnectionStrings MSConnectionStrings { get; set; }

        public string GoogleMapKey { get; set; }

        public string GoogleMapNearbysearchUrl { get; set; }

        public string GoogleMapFindPlaceUrl { get; set; }

        public Validating Validating { get; set; }

        public LineOAuth LineOAuth { get; set; }

        public FbOAuth FbOAuth { get; set; }

        public GoogleOAuth GoogleOAuth { get; set; }

        public string PrivateKey { get; set; }

        public string PublicKey { get; set; }

        public JWTDefault JWTDefault { get; set; }

        public Newebpay Newebpay { get; set; }

        public SMTP SMTP { get; set; }

        public int ValidEmailExpiredTime { get; set; }

        public Redis Redis { get; set; }

        //public TGos TGos { get; set; }

        public ServiceTokenAPI ServiceTokenAPI { get; set; }

        public TGosAPI TGosAPI { get; set; }

        public GoogleMapAPI GoogleMapAPI { get; set; }

        #region 開發階段暫時使用的Google Map Key
        public string gene03271991_key { get; set; }
        public string ytgroupvr8_key { get; set; }
        #endregion
    }

    public class ConnectionStrings
    {
        public string DefaultConnection { get; set; }
    }

    public class JWTDefault
    {
        public string iss { get; set; }
        public string aud { get; set; }
    }

    public class Validating
    {
        public bool MemberToken { get; set; }
        public bool WebToken { get; set; }
        public bool MerchantToken { get; set; }
        public bool CheckSum { get; set; }
        public bool Version { get; set; }
        public bool Platform { get; set; }
        public bool PropAttribute { get; set; }
    }

    public class LineOAuth
    {
        public string TokenIdUrl { get; set; }
        public string UserInfoUrl { get; set; }
        public string grant_type { get; set; }
        public string redirect_uri { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string scope { get; set; }
    }

    public class FbOAuth
    {
        public string UserInfoUrl { get; set; }
        public string scope { get; set; }
    }

    public class GoogleOAuth
    {
        public string AccessTokenUrl { get; set; }
        public string UserInfoUrl { get; set; }
        public string grant_type { get; set; }
        public string redirect_uri { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string scope { get; set; }
    }

    public class Newebpay
    {
        public string MerchantID { get; set; }
        public string HashKey { get; set; }
        public string HashIV { get; set; }
        public string QueryTradeInfoAPI { get; set; }
        public string CancelAPI { get; set; }
        public string CloseAPI { get; set; }
        public string TradeResultRedirectUrl { get; set; }
    }

    public class SMTP
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string DefaultSender { get; set; }
        public string DefaultSenderPWD { get; set; }
        public string DefaultReceiver { get; set; }
        public string AccountantReceiver { get; set; }
    }
    public class Redis 
    {
        public string ConnectionString { get; set; }
        public int DefaultTtl { get; set; }
        public int MemberInfoTtl { get; set; }
        public int PlaceTtl { get; set; }
    }
    //public class TGos 
    //{
    //    public string Url { get; set; }
    //    public string oSRS { get; set; }
    //    public string oFuzzyType { get; set; }
    //    public string oResultDataType { get; set; }
    //    public string oFuzzyBuffer { get; set; }
    //    public string oIsOnlyFullMatch { get; set; }
    //    public string oIsSupportPast { get; set; }
    //    public string oIsShowCodeBase { get; set; }
    //    public string oIsLockCounty { get; set; }
    //    public string oIsLockTown { get; set; }
    //    public string oIsLockVillage { get; set; }
    //    public string oIsLockRoadSection { get; set; }
    //    public string oIsLockLane { get; set; }
    //    public string oIsLockAlley { get; set; }
    //    public string oIsLockArea { get; set; }
    //    public string oIsSameNumber_SubNumber { get; set; }
    //    public string oCanIgnoreVillage { get; set; }
    //    public string oCanIgnoreNeighborhood { get; set; }
    //    public string oReturnMaxCount { get; set; }
    //    public string oAPPId { get; set; }
    //    public string oAPIKey { get; set; }
    //}

    public class ServiceTokenAPI 
    {
        public string CodeUrl { get; set; }
        public string TokenUrl { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
    }

    public class TGosAPI 
    {
        public string Url { get; set; }
        public string Token {  get; set; }
    }

    public class GoogleMapAPI
    {
        public string Url { get; set; }
        public string Token { get; set; }
    }
}
