using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Bot.Admins;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Bot;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Bot
{
    public class Program
    {
       public string sqlQuery;
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _dclient;
        private CommandService _commandservice;
        private IServiceProvider _services;
        private bool hasSentLog = false;
        private string connstr = "THISCONNECTSTOMYSQL! THIS INFORMATION HAS BEEN EXCLUDED";
        private IMessageChannel modlog;
        public async Task RunBotAsync()
         {
            _dclient = new DiscordSocketClient();
            _commandservice = new CommandService();
            _services = new ServiceCollection()

                .AddSingleton(_dclient)
                .AddSingleton(_commandservice)
                .BuildServiceProvider();
 
            string token = "";

            await RegisterCommandsAsync();
            await _dclient.LoginAsync(TokenType.Bot, token);
            // Set custom status here
            await _dclient.SetGameAsync("Watching over Top5Gaming");
            await _dclient.StartAsync();
            _dclient.UserVoiceStateUpdated += UserInVC;
            _dclient.Ready += BotReady;
            _dclient.MessageReceived += OnMessageRecived;
            _dclient.MessageReceived += _dclient_MessageReceived;
            _dclient.MessageUpdated += _dclient_MessageUpdated; 
            _dclient.UserLeft += memberLeft;
            modlog = _dclient.GetChannel(745398099522093107) as IMessageChannel;
            await Task.Delay(-1);
        }

        private Task _dclient_MessageReceived(SocketMessage arg)
        {
            if (arg.Channel.Id == 780616856440012811)
            {
                var up = new Emoji("👍");
                arg.AddReactionAsync(up, RequestOptions.Default);
                var down = new Emoji("👎");
                arg.AddReactionAsync(down, RequestOptions.Default);
            }
            return Task.CompletedTask;
        }

        private async Task _dclient_MessageUpdated(Cacheable<IMessage, ulong> arg1, SocketMessage arg2, ISocketMessageChannel arg3)
        {
            await OnMessageRecived(arg2);
        }

        private Task memberLeft(SocketGuildUser user)
        {
            sqlQuery = "DELETE FROM `mutes` WHERE `UserID` = " + user.Id + ";";
            SQLInsert();
            return Task.CompletedTask;
        }

        private async Task UserInVC(SocketUser user, SocketVoiceState arg1, SocketVoiceState arg2)
        {
            SocketGuildUser user1 = user as SocketGuildUser;
            SocketGuildUser musicBot = _dclient.GetUser(282859044593598464) as SocketGuildUser;
            IVoiceChannel channel = user1.VoiceChannel;
            if (channel.Id == 741010210021179525)
            {
                Console.WriteLine("Someone is in Music VC");
                if (musicBot.VoiceChannel != _dclient.GetChannel(741010210021179525))
                {
                    Console.WriteLine("But the Music bot isn't");
                    await Task.Delay(60000);
                    if (musicBot.VoiceChannel.Id != 741010210021179525)
                    {
                        Console.WriteLine("But the Music bot still isn't");
                        ITextChannel chat = _dclient.GetChannel(543261550639579161) as ITextChannel;
                        await chat.SendMessageAsync(user.Mention + "Hey! I caught you AFK farming! Please do not do it again! Admin's have been notifed about this!");
                        await SendLog("AFK Farmer", user.Mention + "Has been removed from <#741010210021179525> for AFK Farming!");
                        IGuildUser user2 = user1 as IGuildUser;
                        await user2.ModifyAsync(x => x.Channel = null);
                    }
                }
            }
            return;
        }

        private Task BotReady()
        {
            Console.WriteLine("Ready");
            MuteUpdate();
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _dclient.MessageReceived += HandleCommandAsync;
            await _commandservice.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_dclient, message);

            int argPos = 0;
            if (message.HasStringPrefix("!", ref argPos))
            {
                var result = await _commandservice.ExecuteAsync(context, argPos, _services);
            }
        }
        string[] warnList = new string[]
        {
            "fuck",
            "shit",
            "Fuck",
            "Shit",
            "whore",
            "asshole",
            "Asshole",
            "bastard",
            "Bastard",
            "bitch",
            "Bitch",
            "bollocks",
            "Bollocks",
            "bullshit",
            "Bullshit",
            "cock",
            "Cock",
            "Cocksucker",
            "cocksucker",
            "cunt",
            "Cunt",
            "dick",
            "Dick",
            "holy shit",
            "motherfucker",
            "Motherfucker",
            "piss",
            "Piss",
            "prick",
            "Prick",
            "slut",
            "Slut",
            "penis",
            "Penis",
            "vagina",
            "Vagina",
            "fanny",
            "Fanny",
            "minge",
            "Minge",
            "pussy",
            "Pussy",
            "dickhead",
            "Dickhead",
            "wanker",
            "Wanker",
            "wank",
            "Wank",
            "Fucker",
            "fucker",
            "fucking",
            "Fucking"
        };
        string[] banList = new string[]
        {
            "Nigger",
            "Nigga",
            "fag",
            "Fag",
            "nigger",
            "nigga",
            "faggot",
            "Fag"
        };
        string[] t5gteam = new string[]
        {
            "<@160619327316295680>", // Shift
            "<@194594539883724800>", // Tommy
            "<@305583122659934208>", // Sammy
            "<@328060096443842562>", // Consequence
            "<@142787342111866881>"  // Dynamic
        };

        string[] scamLinks = new string[]
        {
            "https://dlscord.org/airdrop/nitro",
            "https://discocrd.gift/",
            "https://discord-cpp.com/",
        };
        public async Task OnMessageRecived(SocketMessage message)
        {
            if (message.Channel is IDMChannel)
            {
                SocketUser dms = _dclient.GetUser(543192194803302431) as SocketUser;
                await SendDM(message.Author, message.Content, dms);
            }
            if (message.Content.Split(" ").Intersect(warnList).Any())
            {
                var user = (message.Author as IGuildUser);
                if (message.Channel.Id == 585827591513178142)
                {
                    await message.Channel.SendMessageAsync("Language");
                    return;
                }
                await message.DeleteAsync();
                ISocketMessageChannel thisChanneel = message.Channel;
                await SendEmbedMessage("Chat Manager", message.Author.Mention + "'s message has been deleted by Chat Moderator as it contained a bad word.\n Warning issued to " + message.Author.Mention + " **for Bad Word Usage**", thisChanneel);
                await SendLog("__Top5Gaming Auto Moderator__", "**Member**      | " + user.Mention + "\n**Offense** | Violation of Banned Words list \n  **Legnth** | Permanent \n **Punishment** | WARNING \n **Punished By**  | AutoModerator");
                sqlQuery = "INSERT INTO `warnings` (`UserID`, `warnReason`, `AdminID`) VALUES ('" + user.Id + "', 'Violation of Bad Words list', '876197995547344896')";
                SQLInsert();
            }

            
            if (message.Content.Split(" ").Intersect(t5gteam).Any())
            {
                var user = (message.Author as IGuildUser);
                await message.DeleteAsync();
                ISocketMessageChannel thisChanneel = message.Channel;
                await SendEmbedMessage("Chat Manager", message.Author.Mention + "'s message has been deleted by Chat Moderator as it mentioned the Top5Gaming Team.\n Warning issued to " + message.Author.Mention + " **Pinging the Top5Gaming Team**", thisChanneel);
                await SendLog("__Top5Gaming Auto Moderator__", "**Member**      | " + user.Mention + "\n**Offense** | Pinging Top5Gaming Team \n  **Legnth** | Permanent \n **Punishment** | WARNING \n **Punished By**  | 876197995547344896");
                sqlQuery = "INSERT INTO `warnings` (`UserID`, `warnReason`, `AdminID`) VALUES ('" + user.Id + "', 'Pinging Top5Gaming Team', 'AutoModerator')";
                SQLInsert();
            }

            if (message.Content.Split(" ").Intersect(banList).Any())
            {
                var user = (message.Author as IGuildUser);
                await user.BanAsync(7, "Violated the Ban List");
                await message.DeleteAsync();
                ISocketMessageChannel thisChanneel = message.Channel;
                await SendEmbedMessage("Chat Manager", message.Author.Mention + "'s message has been deleted by Chat Moderator as it contained a extremely offensive words. \n Ban issued to " + message.Author.Mention + " **for Abusive Language**", thisChanneel);
                await SendLog("__Top5Gaming Auto Moderator__", "**Member**      | " + user.Mention + "\n**Offense** | Violation of Banned Words list \n  **Legnth** | Permanent \n **Punishment** | BAN \n **Punished By**  | AutoModerator");
            }

            /*if (message.Content.Contains("soccer") || message.Content.Contains("Soccer"))
            {
                await message.Channel.SendMessageAsync("It's football!");
            }*/

            if (message.Content.Split(" ").Intersect(scamLinks).Any())
            {
                await SendEmbedMessage("SCAM LINK DETECTED", message.Author.Mention + " has been banned for posting a link to a known scam site.", _dclient.GetChannel(message.Channel.Id) as ISocketMessageChannel);
                await (message.Author as IGuildUser).BanAsync(7, "Scam attempts");
            }

            AutoModd(message);
        }

        private async Task SendDM(SocketUser author, string content, SocketUser dms)
        {
            if (author.Id != 876197995547344896)
            {
                await dms.SendMessageAsync("**" + author.Mention + "** says\n \n " + content);
            }
        }

        public async void SQLInsert()
        {
            MySqlConnection conn = new MySqlConnection(connstr);
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sqlQuery, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task MuteUpdate()
        {
            await Task.Run(() =>
            {
                MySqlConnection conn = new MySqlConnection(connstr);
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("UPDATE `mutes` SET MuteUntil = MuteUntil - 1", conn);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                conn.Close();
                MuteRead();
                MuteUpdate();
            });
        }

        public async Task MuteRead()
        {
            await Task.Run(async () =>
            {
                MySqlConnection conn = new MySqlConnection(connstr);
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM `mutes` WHERE MuteUntil <= 0", conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        if (!hasSentLog)
                        {
                            hasSentLog = true;
                            SendLog("__Top5Gaming Mute Manager__", "**Member**        | <@" + rdr[1] + ">\n **Punishment**       | UNMUTE" + "\n **Punished by**          | <@" + rdr[4] + ">");
                        }
                        await _dclient.GetGuild(542876608143556623).DownloadUsersAsync();
                        ulong id = Convert.ToUInt64(rdr[1]);
                        await _dclient.GetGuild(542876608143556623).GetUser(id).RemoveRoleAsync(553340347606892544);
                        sqlQuery = "DELETE FROM `mutes` WHERE `UserID` = " + rdr[1] + ";";
                        SQLInsert();
                        hasSentLog = false;
                    }
                    rdr.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                conn.Close();
            });
        }


        //////////////////////////////////////////////////////////////
        ///                                                     //////
        ///                     Automod                         //////
        /////////////////////////////////////////////////////////////


        string[] seq1 =
        {
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
        private bool a = false;
        private bool b = false;
        private bool c = false;


        private bool stageOne = false;
        public async Task AutoModd(SocketMessage message)
        {
            await Task.Run(async () =>
            {
                try
                {
                    Console.WriteLine("Message Recieved");
                    if (!a && message.Content.Split(" ").Intersect(seq1).Any())
                    {
                        Console.WriteLine("Content Scanned");
                        a = true;
                        Console.WriteLine("A");
                        return;
                    }
                    Console.WriteLine("Checking after A");
                    if (a && message.Content.Split(" ").Intersect(seq1).Any() && !b)
                    {
                        Console.WriteLine("B Content Scanned");
                        b = true;
                        Console.WriteLine("B");
                        return;
                    }
                    if (a && message.Content.Split(" ").Intersect(seq1).Any() && !c)
                    {
                        Console.WriteLine("C Content Scanned");
                        c = true;
                        Console.WriteLine("C");
                        return;
                    }
                    Console.WriteLine("Checking after B");
                    if (c && message.Content.Split(" ").Intersect(seq1).Any())
                    {
                        Console.WriteLine("C");
                        await message.Channel.SendMessageAsync("The AutoModerator has detected a possible argument occuring in this channel. Please cease or next time we recognize a key-word Admin's will be notifed.");
                        stageOne = true;
                        return;
                    }
                    Console.WriteLine("Checking after C");
                    if (stageOne)
                    {
                        bool hasSentMessage = false;
                        if (hasSentMessage == false)
                        {
                            await SendEmbedMessage("Incident Tracked by AutoMod", "An incident in <#" + message.Channel.Id + "> has occured. https://discord.com/channels/542876608143556623/" + message.Channel.Id + "/" + message.Id, _dclient.GetChannel(585827591513178142) as ISocketMessageChannel);
                            hasSentMessage = true;
                        }
                            a = false;
                        b = false;
                        stageOne = false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            });
        }










        public async Task SendEmbedMessage(string title, string des, ISocketMessageChannel chan)
        {
            var EmbedBuilder = new EmbedBuilder()
            .WithTitle(title)
            .WithDescription(des);
            Embed embed = EmbedBuilder.Build();
            await chan.SendMessageAsync(embed: embed);
        }
        public async Task SendLog(string title, string des)
        {
            var EmbedBuilder = new EmbedBuilder()
            .WithTitle(title)
            .WithColor(255, 0, 0)
            .WithThumbnailUrl("https://yt3.ggpht.com/ytc/AKedOLTQMXQnzj1y5uOfob1G0RFWacXDt9lnI8CRZ3JrNA=s900-c-k-c0x00ffffff-no-rj")
            .WithDescription(des);
            Embed embed = EmbedBuilder.Build();
            await modlog.SendMessageAsync(embed: embed);
        }
    }
}
