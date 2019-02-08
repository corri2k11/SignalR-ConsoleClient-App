using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SignalR .NET Chat Client");
            var hubCon = new HubConnection("http://localhost:4885");
            var theHub = hubCon.CreateHubProxy("chat");

            //opens connection to signalr hub.
            hubCon.Start()
                  .ContinueWith(task => {
                       if (task.IsFaulted) Console.WriteLine($"There was an error opening the connection to the server hub. {task.Exception.GetBaseException()}");
                       else Console.WriteLine("Connected to Hub...");
                  })
                  .Wait();  //alternatively use await keyword instead of the .Wait() method, for that async should be support in main method

            //get current date time from hub server
            theHub.Invoke<DateTime>("getServerDate")
                  .ContinueWith(task => {
                       if (task.IsFaulted) Console.WriteLine($"There was an error calling the GetServerDateTime method in the server Hub. {task.Exception.GetBaseException()}");
                       else Console.WriteLine(task.Result.ToLongDateString());
                  })
                  .Wait();

            //listen for incoming displayMessage signals/requests from the server hub. Updates the console screen with new incoming messages
            theHub.On("announce", msg => {
                Console.WriteLine(msg);
            });

            //chat messages loop, type quit to exit
            Console.WriteLine("Type in a message and press Enter to send, or type Quit to exit.");
            while (true) {
                var msg = Console.ReadLine();
                if (msg.ToLower().Equals("quit")) break;
                //send message to the server so hub can broadcast it to all connected clients
                theHub.Invoke("announceToEverybody", msg)
                      .ContinueWith(task => {
                           if (task.IsFaulted) Console.WriteLine($"There was an error calling the announceToEverybody method in the server Hub. {task.Exception.GetBaseException()}");
                      })
                      .Wait();
            }

            hubCon.Stop();
        }
    }
}