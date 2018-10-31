using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace NSCSC.Shared.Utilities
{
    public static class Security
    {
        private const String SECRET_KEY = "30990fbe490342ca9c1044c3e953fce05dba3eca6a664105a915e07c9c7a17cd1e22dd7240fc4bce96b6621f270e379e82bfa4fef329415889235f4837300c9c529cb47357af400087ee6df6c65b3dcf30a218c9174c4507a8693b7916bc4a76690cc5fefd4d45d3aa79fdb8fb422857943202cadc7b4da786a0356f2cc4b6dc";
        public static string KeyCyberSouce = "b38f1960-1db2-4cac-876f-334020cad2de";
        public static string KeyeNETS = "b59038b93993";
        public static String sign(IDictionary<string, string> paramsArray)
        {
            return sign(buildDataToSign(paramsArray), SECRET_KEY);
        }

        private static String sign(String data, String secretKey)
        {
            UTF8Encoding encoding = new System.Text.UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(secretKey);

            HMACSHA256 hmacsha256 = new HMACSHA256(keyByte);
            byte[] messageBytes = encoding.GetBytes(data);
            return Convert.ToBase64String(hmacsha256.ComputeHash(messageBytes));
        }

        private static String buildDataToSign(IDictionary<string, string> paramsArray)
        {
            String[] signedFieldNames = paramsArray["signed_field_names"].Split(',');
            IList<string> dataToSign = new List<string>();

            foreach (String signedFieldName in signedFieldNames)
            {
                dataToSign.Add(signedFieldName + "=" + paramsArray[signedFieldName]);
            }

            return commaSeparate(dataToSign);
        }

        private static String commaSeparate(IList<string> dataToSign)
        {
            return String.Join(",", dataToSign);
        }
    }

    public class ResponseeNETS
    {
        public string ss { get; set; }
        public ResponseeNETSInfo msg { get; set; }
    }

    public class ResponseeNETSInfo
    {
        public string netsMid { get; set; }
        public string merchantTxnRef { get; set; }
        public string merchantTxnDtm { get; set; }
        public string paymentType { get; set; }
        public string currencyCode { get; set; }
        public string netsTxnRef { get; set; }
        public string netsTxnDtm { get; set; }
        public string paymentMode { get; set; }
        public string merchantTimeZone { get; set; }
        public string netsTxnStatus { get; set; }
        public string netsTxnMsg { get; set; }
        public string netsAmountDeducted { get; set; }
        public string maskPan { get; set; }
        public string stageRespCode { get; set; }
        public string txnRand { get; set; }
        public string actionCode { get; set; }
        public string netsMidIndicator { get; set; }
    }

    public class ENetsInfo
    {
        public string txnReq { get; set; }
        public string hMac { get; set; }
        public Int64 amount { get; set; }
        public string currency { get; set; }
    }
}
