﻿using CommonLib;
using CommonLib.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenTask
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string [] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            if (args != null)
            {
                if (args.Length > 0)
                {
                    if (args[0] == "\\?" || args[0] == "/?")
                    {
                        Console.WriteLine("ScreenTask.exe [-SP:SENDING_PROTOCOL] [-P:PORT]");
                        Console.WriteLine("SENDING_PROTOCOL:\r\nTCP\r\nMulticast\r\n");
                        //Console.WriteLine("PORT: numero intero per definire quale porta usare.\r\nTCP -> Porta di ascolto\r\nMulticast -> Porta di invio");
                        Console.WriteLine("PORT: NON IMPLEMENTATA");
                        return;
                    }
                    foreach(string a in args)
                    {
                        if(a.StartsWith("-SP:"))
                        {
                            string tmp=a.Substring(4).ToUpper();
                            switch(tmp)
                            {
                                case "TCP":
                                    CommonSetting.sendingProtocol = SendingProtocol.TCP;
                                    break;
                                case "MULTICAST":
                                    CommonSetting.sendingProtocol = SendingProtocol.Multicast;
                                    break;
                            }
                        }
                        if (a.StartsWith("-P:"))
                        {
                            int port = int.Parse(a.Substring(3));
                            //CommonSetting.ProtocolPort = port;
                        }
                    }
                }
                
            }


            Application.Run(new FormServer());
            
        }



        // This function is not called if the Assembly is already previously loaded into memory.
        // This function is not called if the Assembly is already in the same folder as the app.
        //
        private static Assembly OnResolveAssembly(object sender, ResolveEventArgs e)
        {
            var thisAssembly = Assembly.GetExecutingAssembly();

            // Get the Name of the AssemblyFile
            var assemblyName = new AssemblyName(e.Name);
            var dllName = assemblyName.Name + ".dll";

            // Load from Embedded Resources
            var resources = thisAssembly.GetManifestResourceNames().Where(s => s.EndsWith(dllName));
            if (resources.Any())
            {
                // 99% of cases will only have one matching item, but if you don't,
                // you will have to change the logic to handle those cases.
                var resourceName = resources.First();
                using (var stream = thisAssembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null) return null;
                    var block = new byte[stream.Length];

                    // Safely try to load the assembly.
                    try
                    {
                        stream.Read(block, 0, block.Length);
                        return Assembly.Load(block);
                    }
                    catch (IOException)
                    {
                        return null;
                    }
                    catch (BadImageFormatException)
                    {
                        return null;
                    }
                }
            }

            // in the case the resource doesn't exist, return null.
            return null;
        }
    }
}
