using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Security.Cryptography;

//Diaz Abdul Matin

namespace TCP_Aplication
{
    public class Program
    {
        static string Decrypt(string textToDecrypt)
        {
            try
            {
                
                string ToReturn = "";
                string publickey = "98653215";
                string secretkey = "68532014";
                byte[] privatekeyByte = { };
                privatekeyByte = System.Text.Encoding.UTF8.GetBytes(secretkey);
                byte[] publickeybyte = { };
                publickeybyte = System.Text.Encoding.UTF8.GetBytes(publickey);
                MemoryStream ms = null;
                CryptoStream cs = null;
                byte[] inputbyteArray = new byte[textToDecrypt.Replace(" ", "+").Length];
                inputbyteArray = Convert.FromBase64String(textToDecrypt.Replace(" ", "+"));
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateDecryptor(publickeybyte, privatekeyByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    Encoding encoding = Encoding.UTF8;
                    ToReturn = encoding.GetString(ms.ToArray());
                }
                return ToReturn;
            }
            catch (Exception ae)
            {
                throw new Exception(ae.Message, ae.InnerException);
            }
        }

        public static void Main()
        {
            try
            {
                IPAddress ipAd = IPAddress.Parse("192.168.0.178");                
                TcpListener myList = new TcpListener(ipAd, 8001);               
                myList.Start();
                Console.WriteLine("The server is running at port 8001...");
                Console.WriteLine("The local End point is  :" +myList.LocalEndpoint);
                Console.WriteLine("Waiting for a connection.....");
                Socket s = myList.AcceptSocket();
                Console.WriteLine("Connection accepted from " + s.RemoteEndPoint);
                byte[] b = new byte[100];                
                int k = s.Receive(b);
                char z = Convert.ToChar(b[0]); 
                Console.WriteLine("Recieved...");
                Console.WriteLine();

                Console.WriteLine("Data sebelum di Enkripsi");
                string json;

                json = Encoding.Default.GetString(b);
                Console.WriteLine(json);
                Packet packet = JsonConvert.DeserializeObject<Packet>(json);
                Console.WriteLine();
                Console.WriteLine("=====================================");
                Console.WriteLine();
                Console.WriteLine("Data setelah di Enkripsi");
                Console.WriteLine();

                //Deskripsi
                string deskripsi = Decrypt(packet.pass);


                Console.WriteLine("Username : "+packet.user);
                Console.WriteLine("Password : " + deskripsi);
                ASCIIEncoding asen = new ASCIIEncoding();
                s.Send(asen.GetBytes("The string was recieved by the server."));          
                
                s.Close();
                myList.Stop();

            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }
            Console.ReadKey();
        }
    }
    public class Packet
    {
        public string user { get; set; }
        public string pass { get; set; }
    }
}
