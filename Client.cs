using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Client
{
    public class Program
    {    

        static string Encrypt(string pass)
        {
            try
            {                
                string ToReturn = "";
                string publickey = "98653215";
                string secretkey = "68532014";
                byte[] secretkeyByte = { };
                secretkeyByte = System.Text.Encoding.UTF8.GetBytes(secretkey);
                byte[] publickeybyte = { };
                publickeybyte = System.Text.Encoding.UTF8.GetBytes(publickey);
                MemoryStream ms = null;
                CryptoStream cs = null;
                byte[] inputbyteArray = System.Text.Encoding.UTF8.GetBytes(pass);
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateEncryptor(publickeybyte, secretkeyByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    ToReturn = Convert.ToBase64String(ms.ToArray());
                }
                return ToReturn;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public static void Main()
        {
            try {
                TcpClient tcpclnt = new TcpClient();
                Console.WriteLine("Connecting.....");
                tcpclnt.Connect("192.168.0.178", 8001);
                Console.WriteLine("Connected");
                Console.Write("Enter Username : ");
                String username = Console.ReadLine();
                Console.Write("Enter Password : ");
                String password = Console.ReadLine();

                //Enkripsi
                string enkripsiPassword = Encrypt(password);
                
                var Packet = new Packet
                {
                    user = username,
                    pass = enkripsiPassword
                };
                string str = JsonSerializer.Serialize(Packet);
                Console.WriteLine();
                Console.WriteLine("Data yang dikirim Ke Server Setelah Di Enkripsi : ");
                Console.WriteLine(str);
                Console.WriteLine();
                Stream stm = tcpclnt.GetStream();
                ASCIIEncoding asen = new ASCIIEncoding();
                byte[] ba = asen.GetBytes(str);
                Console.WriteLine("Transmitting.....");
                stm.Write(ba, 0, ba.Length);
                byte[] bb = new byte[100];
                int k = stm.Read(bb, 0, 100);
                for (int i = 0; i < k; i++)
                    Console.Write(Convert.ToChar(bb[i]));
                tcpclnt.Close();
            }
            catch (Exception ex) {
                Console.WriteLine("Error..... " + ex.StackTrace);
            }
            Console.ReadKey();
    }
        public class Packet
        {
            public string user { get; set; }
            public string pass { get; set; }
        }
    }
}
    
