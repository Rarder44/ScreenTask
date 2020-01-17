using ExtendCSharp.ExtendedClass;
using ExtendCSharp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ScreenTask.Classes
{
    class HTTP_Downloader
    {
        HttpListener serv;
        private bool isWorking = false;

        public void Start()
        {
            try
            {
                serv = new HttpListener();
                serv.IgnoreWriteExceptions = true;
                isWorking = true;
                Task.Factory.StartNew(() => WebServer()).Wait();
            }
            catch (ObjectDisposedException disObj)
            {
                serv = new HttpListener();
                serv.IgnoreWriteExceptions = true;
            }
            catch (Exception ex)
            {
                Common.Log("Error! : " + ex.Message);
            }
        }
        public void Stop()
        {
            
            if (serv != null)
                serv.Close();
        }

        private async Task WebServer(int portWebServer = 80)
        {
            serv.Prefixes.Clear();
            string Address = "http://*:" + portWebServer + "/";
            serv.Prefixes.Add(Address);
            Common.Log("Download address " + Address);
            serv.Start();



            while (isWorking)
            {
                var ctx = await serv.GetContextAsync();


                //qualsiasi richiesta la ridireziono sul download del file
                FilePlus client = ServicesManager.Get<ResourcesService>().GetObject<FilePlus>("Client.exe");
                client.Extension = ".exe";
                client.Name = "Client";


                ctx.Response.ContentType = "application/octet-stream";
                ctx.Response.AddHeader("Content-disposition", "attachment; filename=Client.exe");

                ctx.Response.StatusCode = 200;
                try
                {
                    //TODO: mi sa che non devo fare await ( altrimenti puo scaricare solo una persona per volta ) 
                    await ctx.Response.OutputStream.WriteAsync(client.data, 0, client.data.Length);
                }
                catch (Exception ex)
                {

                    //Do Nothing !!! this is the Only Effective Solution for this Exception : the specified network name is no longer available
                }

                ctx.Response.Close();
            }

            serv.Stop();
        }



    }
}
