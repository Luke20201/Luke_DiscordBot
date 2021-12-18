using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Bot.Admins
{
    public class AutoMod
    {
        string[] seq1 =
        {
            "stop",
            "Stop",
            "STOP",
            "shut up",
            "SHUT UP",
            "Shut up",
            "idiot",
            "a mod",
            "a admin",
            "stupid",
        };
        public bool a = false;
        string[] seq2 =
        {
            "stop",
            "Stop",
            "STOP",
            "shut up",
            "SHUT UP",
            "Shut up",
            "idiot",
            "a mod",
            "a admin",
            "stupid",
        };
        private bool b = false;
        string[] seq3 =
        {
            "stop",
            "Stop",
            "STOP",
            "shut up",
            "SHUT UP",
            "Shut up",
            "idiot",
            "a mod",
            "a admin",
            "stupid",
        };
        public async Task AutoModd(SocketMessage message)
        {
            try
            {
               Console.WriteLine("Message Recieved");
                if (!a && message.Content.Split(" ").Intersect(seq1).Any())
                {
                 //   Console.WriteLine("Content Scanned");
                    a = true;
                 //   Console.WriteLine("A");
                    return;
                }
              //  Console.WriteLine("Checking after A");
                 if (a && message.Content.Split(" ").Intersect(seq2).Any())
                 {
               //     Console.WriteLine("B Content Scanned");
                    b = true;
               //     Console.WriteLine("B");
                    return;
                 }
              //  Console.WriteLine("Checking after B");
                if (b && message.Content.Split(" ").Intersect(seq3).Any())
                {
                //    Console.WriteLine("C");
                    await message.Channel.SendMessageAsync("The AutoModerator has detected a possible arguement occuring in this channel. Please cease within or next time we recongize a key-word Admin's will be notifed.");
                    return;
                }
             //   Console.WriteLine("Checking after C");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
