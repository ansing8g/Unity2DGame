
using System;

namespace ChattingServer
{
    public class Program
    {
        public static void Main()
        {
            string? strPort = "12121";
            //if (string.IsNullOrEmpty(strPort) == true)
            //{
            //    Console.WriteLine($"Input Port={strPort}");
            //    return;
            //}

            int port = 0;
            try
            {
                port = int.Parse(strPort);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Input Port={strPort}, Error={e.Message}");
                return;
            }

            if (false == TestServer.Instance.Start(port))
            {
                Console.WriteLine("Start Fail");
                return;
            }

            while (true)
            {
                string? input = Console.ReadLine();
                if (true == string.IsNullOrEmpty(input))
                {
                    continue;
                }


            }
        }
    }
}
