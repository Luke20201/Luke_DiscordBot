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
using Discord.Interactions;

namespace Bot
{
    public class Program
    {
       public string sqlQuery;
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _dclient;
        private InteractionService _interactionService;
        private IServiceProvider _services;
        private string connstr = "";
        private IMessageChannel modlog;
        public async Task RunBotAsync()
         {
            var config = new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.All
            };

            _dclient = new DiscordSocketClient(config);
            _services = new ServiceCollection()

                .AddSingleton(_dclient)
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                .BuildServiceProvider();
 
            string token = "";

            await _dclient.LoginAsync(TokenType.Bot, token);
            // Set custom status here
            
            await _dclient.SetGameAsync("Watching over Top5Gaming");
            await _dclient.StartAsync();
            await _services.GetRequiredService<InteractionService>().AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            _dclient.Ready += BotReady;
            _dclient.MessageReceived += OnMessageRecived;
            _dclient.MessageReceived += _dclient_MessageReceived;
            _dclient.MessageUpdated += _dclient_MessageUpdated;
            _dclient.InteractionCreated += HandleInteraction;
            _dclient.SelectMenuExecuted += MyMenuHandler;
            _dclient.SlashCommandExecuted += SlashCommandHandler;

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

        // The following method is a sample Interaction menu. This menu give's roles.
        public async Task Roles(SocketSlashCommand ro)
        {
            Console.WriteLine("Debug Roles");
            var menuBuilder = new SelectMenuBuilder()
        .WithPlaceholder("Select a role")
        .WithCustomId("role-menu")
        .WithMinValues(1)
        .WithMaxValues(20)
        .AddOption("Video Notified", "opt-a")
        .AddOption("Giveaway Notified", "opt-b")
        .AddOption("Server Announcements", "opt-c")
        .AddOption("Item Shop Notified", "opt-d")
        .AddOption("He/Him", "opt-e")
        .AddOption("They/Them", "opt-f")
        .AddOption("She/Her", "opt-g")
        .AddOption("Ask Me", "opt-h")
        .AddOption("Any", "opt-i")
        .AddOption("PC", "opt-j")
        .AddOption("PS4", "opt-k")
        .AddOption("Xbox", "opt-l")
        .AddOption("Mobile", "opt-m")
        .AddOption("Switch", "opt-n")
        .AddOption("North America", "opt-o")
        .AddOption("South America", "opt-p")
        .AddOption("Europe", "opt-q")
        .AddOption("Africa", "opt-r")
        .AddOption("Asia", "opt-s")
        .AddOption("Oceania", "opt-t");
            Console.WriteLine("Building Roles");
            var builder = new ComponentBuilder()
                .WithSelectMenu(menuBuilder);

            Console.WriteLine("Roles Built!");
            await ro.DeferAsync();
            await ro.FollowupAsync("Get your role's here! We have notifications, pronouns, continents, and console's!", components: builder.Build());
        }
        public async Task MyMenuHandler(SocketMessageComponent arg)
        {
            string text = string.Join(", ", arg.Data.Values);
            var user = arg.User as IGuildUser;

            if (arg.Data.CustomId == "role-menu")
            {
                if (text.Contains("opt-a"))
                {
                    await user.AddRoleAsync(742874128700276926);
                }
                else if (text.Contains("opt-b"))
                {
                    await user.AddRoleAsync(742874693618499756);
                }
                else if (text.Contains("opt-c"))
                {
                    await user.AddRoleAsync(742874990403387393);
                }
                else if (text.Contains("opt-d"))
                {
                    await user.AddRoleAsync(821889255198294076);
                }
                else if (text.Contains("opt-e"))
                {
                    await user.AddRoleAsync(903370384479498261);
                }
                else if (text.Contains("opt-f"))
                {
                    await user.AddRoleAsync(903370383191846922);
                }
                else if (text.Contains("opt-g"))
                {
                    await user.AddRoleAsync(903370383770660884);
                }
                else if (text.Contains("opt-h"))
                {
                    await user.AddRoleAsync(903370382281682955);
                }
                else if (text.Contains("opt-i"))
                {
                    await user.AddRoleAsync(903370381816135702);
                }
            }
        }

        private async Task SlashCommandHandler(SocketSlashCommand command)
        {
            if (command.CommandName == "rolemenu") await Roles(command);
        }
        private async Task HandleInteraction(SocketInteraction arg)
        {
            try
            {
                // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules
                var ctx = new SocketInteractionContext(_dclient, arg);
                var commands = _services.GetRequiredService<InteractionService>();
                await commands.ExecuteCommandAsync(ctx, _services);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                if (arg.Type == InteractionType.ApplicationCommand)
                    await arg.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
        private async Task _dclient_MessageUpdated(Cacheable<IMessage, ulong> arg1, SocketMessage arg2, ISocketMessageChannel arg3)
        {
            await OnMessageRecived(arg2);
        }

        private async Task BotReady()
        {
            try
            {
                modlog = _dclient.GetChannel(745398099522093107) as ISocketMessageChannel;
                var commands = _services.GetRequiredService<InteractionService>();
                
                await commands.RegisterCommandsToGuildAsync(542876608143556623, true);

                var guild = _dclient.GetGuild(542876608143556623);
                var guildCommand = new SlashCommandBuilder();
                guildCommand.WithName("rolemenu");
                guildCommand.WithDescription("Spawn the role's menu");
                await guild.CreateApplicationCommandAsync(guildCommand.Build());
                Console.WriteLine("Ready");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
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
        string[] noping = new string[]
        {
            "<@>"
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
                sqlQuery = "INSERT INTO `warnings` (`UserID`, `warnReason`, `AdminID`) VALUES ('" + user.Id + "', 'Violation of Bad Words list', '876197995547344896')";
                SQLInsert();
                await SendLog("__Top5Gaming Auto Moderator__", "**Member**      | " + user.Mention + "\n**Offense** | Violation of Banned Words list \n  **Legnth** | Permanent \n **Punishment** | WARNING \n **Punished By**  | AutoModerator");
            }
            
            if (message.Content.Split(" ").Intersect(noping).Any())
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
            await AutoModd(message);
        }

        private async Task SendDM(SocketUser author, string content, SocketUser dms)
        {
            if (author.Id != 912754114402865204)
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
