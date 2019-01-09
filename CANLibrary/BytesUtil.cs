using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CANLibrary
{
    public class BytesUtil
    {
        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="src">源字符串</param>
        /// <param name="startIndexInclusive">开始下标（包括）</param>
        /// <param name="endIndexExclusive">结束下标（不包括）</param>
        /// <returns></returns>
        public static byte[] subArray(byte[] src, int startIndexInclusive, int endIndexExclusive)
        {
            int len = endIndexExclusive - startIndexInclusive;
            byte[] b = new byte[len];
            Array.Copy(src, startIndexInclusive, b, 0, len);
            return b;
        }

        /// <summary>
        /// 将Int数字转换成byte数组（数组从位置0到n按数据高位到低位进行表示）
        /// </summary>
        /// <param name="num">待转换的数字</param>
        /// <param name="len">要转换的字节数组长度</param>
        /// <returns></returns>
        public static byte[] int2Bytes(uint num, int len)
        {
            byte[] b = new byte[len];
            //for (int i = 0; i < len; i++)
            //{
            //    b[i] = ((byte)(num >> (len - 1 - i) * 8));
            //}
            byte[] d = BitConverter.GetBytes(num);
            for (int i = 0; i < d.Length; i++)
            {
                b[len - 1 - i] = d[i];
            }
            return b;
        }

        /// <summary>
        /// int转成16进制数组字符串
        /// </summary>
        /// <param name="num">待转的数</param>
        /// <param name="len">数组长度</param>
        /// <returns></returns>
        public static string int2HexString(uint num, int len)
        {
            byte[] b = int2Bytes(num, len);
            return Bytes2HexString(b);
        }

        /// <summary>
        /// 将byte数组转换成数字（数组从位置0到n按数据高位到低位进行表示）
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static int bytes2Int(byte[] bytes)
        {
            int n = 0;
            if ((bytes == null) || (bytes.Length == 0))
            {
                return n;
            }
            for (int i = 0; i < bytes.Length; i++)
            {
                n += ((bytes[i] & 0xFF) << (bytes.Length - i - 1) * 8);
            }
            return n;
        }



        /// <summary>
        /// 判断byte数组内容是否为全0
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static bool ByteIsNotAll0X00(byte[] bytes)
        {
            bool flag = false;
            for (int i = 0; i < bytes.Length; i++)
            {
                if ((bytes[i] & 0xFF) != 0)
                {
                    flag = true;
                }
            }
            return flag;
        }

        /// <summary>
        /// 将byte数组转换成数字
        /// （数组从位置0到n按数据高位到低位进行表示）
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        //public static int Bytes2Int(byte[] bytes)
        //{
        //    int n = 0;
        //    if ((bytes == null) || (bytes.Length == 0))
        //    {
        //        return n;
        //    }
        //    for (int i = 0; i < bytes.Length; i++)
        //    {
        //        n += ((bytes[i] & 0xFF) << (bytes.Length - i - 1) * 8);
        //    }
        //    return n;
        //}

        /// <summary>
        /// 将byte数组转换成十六进制格式的字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static String Bytes2HexString(byte[] bytes)
        {

            StringBuilder sb = new StringBuilder(bytes.Length);
            for (int i = 0; i < bytes.Length; i++)
            {
                String sTemp = bytes[i].ToString("X2");
                if (sTemp.Length < 2)
                {
                    sb.Append("0");
                }
                sb.Append(sTemp.ToUpper()).Append(" ");
            }
            return sb.ToString();
        }
        public static String Bytes2HexString(byte bytes)
        {
            return Bytes2HexString(new byte[] { bytes });
        }

        /// <summary>
        /// 将byte数组转换成十六进制格式的字符串，如“0F 3E”
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static String Bytes2HexString_old(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                int h = ((bytes[i] & 0xFF) >> 4);
                h = h > 9 ? h + 55 : h + 48;
                sb.Append((char)h);
                int l = (bytes[i] & 0xF);
                l = l > 9 ? l + 55 : l + 48;
                sb.Append((char)l);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 十六进制形式字符串转Byte数组
        /// </summary>
        /// <param name="hexStr"></param>
        /// <returns></returns>
        public static byte[] HexString2Bytes(String hexStr)
        {
            try
            {
                hexStr = hexStr.Trim().Replace(" ", "");
                int len = hexStr.Length;
                if ((len % 2) != 0)
                {
                    hexStr = "0" + hexStr;
                    len = hexStr.Length;
                }
                len /= 2;
                byte[] bts = new byte[hexStr.Length / 2];
                for (int i = 0; i < bts.Length; i++)
                {
                    bts[i] = Convert.ToByte(hexStr.Substring(i * 2, 2), 16);
                }
                return bts;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 把脉冲数加入X/Y命令中,n大于0正方向移动，x小于0负方向移动
        /// </summary>
        /// <param name="data">被插入的数组</param>
        /// <param name="n">待插入的数</param>
        /// <returns></returns>
        public static byte[] IntInsertToByteArr(byte[] cmd, int n)
        {
            if (n > 0) { cmd[2] = 1; }
            else if (n < 0)
            {
                cmd[2] = 2;
                n = -n;
            }
            byte[] data = BytesUtil.int2Bytes(Convert.ToUInt32(n), 5);
            Array.Copy(data, 0, cmd, 3, data.Length);
            return cmd;
        }

    }
}
