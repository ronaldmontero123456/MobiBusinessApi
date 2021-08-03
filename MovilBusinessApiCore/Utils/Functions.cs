using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Web;

namespace MovilBusinessApiCore.Utils
{
    public class Functions
    {
        private Functions() { }


        public static string LogDate
        {
            get { return DateTime.Now.ToString("dd-MM-"); }
        }

        public static string LogTime
        {
            get { return DateTime.Now.ToString("hh:mm:ss"); }
        }

        private static string UrlLog = "Logs";

        public static void WriteLog(string RpCodigo, string mensaje, bool lr = false)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo filVrsionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                string version = filVrsionInfo.ProductVersion;

                string Path = UrlLog + "/" + LogDate;

                string filePath = string.Concat(Path + "/", DateTime.Now.ToString("dd-MM-") + "(" + RpCodigo + (lr?"-CambiosConsumidos":"")+ ").txt");
             
                Directory.CreateDirectory(Path);

                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, true))
                {
                    writer.WriteLine(string.Concat(mensaje, $" versión {version} en Fecha: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")) + "\r\n\r\n");
                    writer.Flush();
                    writer.Close();
                }


            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
        }

        public static string StringToMd5(string value)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(value);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                string sb = "";
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb += (hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}