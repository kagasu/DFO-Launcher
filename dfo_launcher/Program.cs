using Codeplex.Data;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace dfo_launcher
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (var proxy in File.ReadAllLines("proxys.txt"))
            {
                try
                {
                    Console.WriteLine(string.Format("try login, using proxy [{0}]", proxy));
                    var wc = new WebClientEx();
                    wc.Proxy = new WebProxy(string.Format("http://{0}/", proxy));

                    var data = new NameValueCollection();

                    data.Add("email", File.ReadAllLines("userInfo.txt")[0]);
                    data.Add("password", File.ReadAllLines("userInfo.txt")[1]);

                    var res = wc.UploadValues("https://member.dfoneople.com/launcher/restapi/login", data);
                    var json = DynamicJson.Parse((new UTF8Encoding()).GetString(res));

                    Console.WriteLine(string.Format("message: {0}", json["message"]));

                    var str = wc.DownloadString(string.Format("https://member.dfoneople.com/{0}", json["nextUrl"]));

                    var reg = new Regex(@"src=""dfoglobal://([0-9a-zA-Z]{1,})/([0-9a-zA-Z]{1,})/([0-9a-zA-Z]{1,})""");

                    var result = new Process()
                    {
                        StartInfo = new ProcessStartInfo()
                        {
                            FileName = @"D:\Neople\DFO\DFO.exe",
                            Arguments = string.Format("9?52.0.226.21?7101?{0}?{1}?0?0?0?0?0?2?0?0?0?0?0?0?0", ((Match)reg.Match(str)).Groups[2].Value, ((Match)reg.Match(str)).Groups[3].Value),
                            WorkingDirectory = @"D:\Neople\DFO"
                        }
                    }.Start();

                    if (result)
                    {
                        Environment.Exit(0);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}