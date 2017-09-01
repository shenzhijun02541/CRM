using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace LIB
{
    public static class MD5
    {
        /*
         * MD5CryptoServiceProvider 类 计算MD5哈希值输入的数据使用加密服务提供程序 (CSP) 提供的实现。 
         * 计算MD5哈希值输入的数据使用加密服务提供程序 (CSP) 提供的实现。 此类不能被继承。
         */
        public static string GetMD5(string sDataIn)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bytValue, bytHash;
            bytValue = System.Text.Encoding.UTF8.GetBytes(sDataIn);
            bytHash = md5.ComputeHash(bytValue);
            md5.Clear();
            string sTemp = "";
            for (int i = 0; i < bytHash.Length; i++)
            {
                sTemp += bytHash[i].ToString("X").PadLeft(2, '0');
            }
            return sTemp.ToLower();

        }


    }
}

