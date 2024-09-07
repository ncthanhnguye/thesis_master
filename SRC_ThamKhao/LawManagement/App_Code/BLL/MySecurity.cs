using System;
using System.Security.Cryptography;
using System.Configuration;
using System.Text;
using System.IO;
using MOABSearch.Common;
namespace MOABSearch.BLL
{
    /// <summary>
    /// Summary description for MySecurity
    /// </summary>
    public class MySecurity
    {
        public MySecurity()
        {
            
        }
        
        

        public static string Encrypt(string str)
        {
            string EncryptKey ="";
            try
            {
                EncryptKey = Globals.GetConfig("EncryptKey");
            }
            catch
            {

            }
            return Encrypt(str, EncryptKey);//chi de test
        }
        public static string Decrypt(string str)
        {
            string EncryptKey = "";
            try
            {
                EncryptKey = Globals.GetConfig("EncryptKey");
            }
            catch
            {

            }
            return Decrypt(str,EncryptKey);//chi de test
        }
        public static string Encrypt(string strEncryptString, string strKey)
        {
            try
            {
                if (strEncryptString == "")
                    return strEncryptString;
                if (strKey.Length > 8) strKey = strKey.Substring(0, 8);

                DESCryptoServiceProvider cryptProvider = new DESCryptoServiceProvider();

                byte[] inputByteArray = Encoding.UTF8.GetBytes(strEncryptString);

                cryptProvider.Key = ASCIIEncoding.ASCII.GetBytes(strKey);
                cryptProvider.IV = ASCIIEncoding.ASCII.GetBytes(strKey);

                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, cryptProvider.CreateEncryptor(), CryptoStreamMode.Write);

                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();

                StringBuilder strRetVal = new StringBuilder();


                foreach (byte b in ms.ToArray())
                    strRetVal.AppendFormat("{0:X2}", b);

                return strRetVal.ToString();
            }
            catch (Exception ex)
            {
                string i = ex.Message;
                return "";
            }
        }
        public static string Decrypt(string strDecriptString, string strKey)
        {
            try
            {
                if (strDecriptString == "") return strDecriptString;
                if (strKey.Length > 8) strKey = strKey.Substring(0, 8);

                DESCryptoServiceProvider cryptProvider = new DESCryptoServiceProvider();

                cryptProvider.Key = ASCIIEncoding.ASCII.GetBytes(strKey);
                cryptProvider.IV = ASCIIEncoding.ASCII.GetBytes(strKey);

                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, cryptProvider.CreateDecryptor(), CryptoStreamMode.Write);
                StringBuilder strRetVal = new StringBuilder();

               

                byte[] inputByteArray = new byte[strDecriptString.Length / 2];
                int x;

                for (int i = 0; i < strDecriptString.Length / 2; i++)
                {
                    x = Convert.ToInt32(strDecriptString.Substring(i * 2, 2), 16);
                    inputByteArray[i] = (byte)x;
                }

                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();

                byte[] ttt = ms.ToArray();
                Encoding encode = Encoding.UTF8;
                string sss = encode.GetString(ttt);

                return sss;
            }
            catch (Exception ex)
            {
                string i = ex.Message;
                return "";
            }
        }
        public static string EncryptSHA512(string sInput)
        {
            int DATA_SIZE = sInput.Length;
            byte[] data = ASCIIEncoding.ASCII.GetBytes(sInput);

            SHA512 shaM = new SHA512Managed();
            string strHex = "";
            byte[] HashValue = shaM.ComputeHash(data);
            foreach (byte b in HashValue)
            {
                strHex += String.Format("{0:x2}", b);
            }
            return strHex;
        }


    }
}


