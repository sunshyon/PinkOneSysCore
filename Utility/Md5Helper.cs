using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    /// <summary>
    /// MD5加密帮助类
    /// </summary>
    public class Md5Helper
    {
        #region "MD5加密"

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="str">加密字符</param>
        /// <param name="code">加密位数16/32</param>
        /// <returns></returns>
        //public static string MD5(string str, int code)
        //{
        //    string strEncrypt = string.Empty;
        //    if (code == 16)
        //    {
        //        strEncrypt = FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5").Substring(8, 16);
        //    }

        //    if (code == 32)
        //    {
        //        strEncrypt = FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5");
        //    }

        //    return strEncrypt;
        //}

        #endregion

        #region 获取大写的MD5签名结果

        /// <summary>
        /// 获取大写的MD5签名结果
		/// </summary>
		/// <param name="encypStr"></param>
		/// <param name="charset"></param>
		/// <returns></returns>
		public static string GetMD5(string encypStr, string charset)
        {
            string retStr;
            MD5CryptoServiceProvider m5 = new MD5CryptoServiceProvider();
            // 创建md5对象
            byte[] inputBye;
            byte[] outputBye;
            // 使用GB2312编码方式把字符串转化为字节数组
            try
            {
                inputBye = Encoding.GetEncoding(charset).GetBytes(encypStr);
            }
            catch 
            {
                inputBye = Encoding.GetEncoding("GB2312").GetBytes(encypStr);
            }
            outputBye = m5.ComputeHash(inputBye);
            retStr = BitConverter.ToString(outputBye);
            retStr = retStr.Replace("-", "").ToUpper();
            return retStr;
        }

        #endregion
    }
}