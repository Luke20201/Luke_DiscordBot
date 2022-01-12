using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Interactions;
using System.Linq;
using MySql.Data.MySqlClient;

namespace Bot.Admins
{
    public class AdminCommands : InteractionModuleBase<IInteractionContext>
    {
        public string sqlQuery;
        public string SQLReadQuery;
        private IMessage stickyMess;
        private string StickyContent;
        private string connStr = "Server=echstreme.de;Port=3306;Database=c1Look;Uid=c1Look;Pwd=gkmcpLNNF6Y_5;SSL Mode =None";
        public async void SQLRead()
        {
            await Context.Guild.DownloadUsersAsync();
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(SQLReadQuery, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                if (!rdr.HasRows)
                {
                    await SendEmbedMessage("__Top5Gaming Warning Manger__", "No warnings found for this user");
                }

                while (rdr.Read())
                {
                    await SendEmbedMessage("__Top5Gaming Warning Manager__", "**Warn ID** | " + rdr[0] + "\n **User** | <@" + rdr[1] + ">\n **Reason** | " + rdr[2] + "\n **Issued by** | <@" + rdr[3] + ">");    
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            conn.Close();
            Console.WriteLine("Done.");
        }
        
        public async void SQLInsert()
        { 
            MySqlConnection conn = new MySqlConnection(connStr);
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

        [SlashCommand("ban", "Ban a user for a serious violation of Server Rules", false, RunMode.Async)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task Ban(IGuildUser user = null,  string reason = null)
        {
            try
            {
                await Context.Guild.DownloadUsersAsync();

                var admin = Context.User; // Admin = The person who executed the command

                if (user == admin) //If the User and Person executing the command is the same
                {
                    await ReplyAsync("You cannot Ban yourself!");
                    return;
                }

                if (user == null) //If no user was mentioned
                {
                    await SendErrorMessage("Ban Request Failed", "Could not find a UserId in the Ban request. Please try again or contact <@883019151327764531>");
                    return;
                }

                if (user.GuildPermissions.ManageMessages) //If the user is an admin
                {
                    await ReplyAsync("You cannot Ban an Admin!");
                    return;
                }

                if (reason == null) //If no reason was supplied and reasons are required
                {
                    await SendErrorMessage("Ban Request Failed", "Could not find a Reason in your Ban. Please try again or contact <@883019151327764531>");
                    return;
                }
                await user.BanAsync(7, reason + " || Banned by " + admin);
                await SendEmbedMessage("__Top5Gaming Ban Manager__", user + " has been banned by " + admin + "\n \n For " + reason);
                await SendLog("__Top5Gaming Ban Manager__", "**Member**        | " + user.Mention + "\n**Offense**        | " + reason + "\n**Length**        | Permanent\n **Punishment**       | BAN" + "\n **Punished by**          | " + admin.Mention, user, true);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }        
	}

        [SlashCommand("kick", "Kick a user for a serious violation of Server Rules", false, RunMode.Async)]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task Kick(IGuildUser user = null,  string reason = null)
        {
            await Context.Guild.DownloadUsersAsync();

            var admin = Context.User;  //In this Context, admin is the person executing the command.

            // Error Handling
            // This is where we check for things that should prevent the kick
            if (user == admin) //If the User and admin are the same
            {
                await ReplyAsync("You cannot Kick yourself!");
                return;
            }
            if (user == null) //If no UserID is found
            {
                await SendErrorMessage("Kick Request Failed", "Could not find a UserId in the Kick request. Please try again or contact <@883019151327764531>");
                return;
            }
            if (reason == null) //If no reason is found
            {
                await SendErrorMessage("Kick Attempt", "Could not Find a Reason in your Kick. Please try again or contact <@883019151327764531>");
                return;
            }

            if (user.GuildPermissions.ManageMessages) //If the user has admin perms
            {
                await ReplyAsync("You cannot Kick an Admin!");
                return;
            }

            await user.KickAsync(reason);
            await SendEmbedMessage("Top5Gaming Kick Manager", user + " has been Kicked by " + admin + "\n \n" + reason + " Kicked by " + admin);
            await SendLog("__Top5Gaming Kick Manager__", "**Member**        | " + user.Mention + "\n**Offense**        | " + reason + "\n**Length**        | Inapplicable\n **Punishment**       | KICK" + "\n **Punished by**          | " + admin.Mention, user);
        }
        
        [SlashCommand("purge", "Delete the specfied number of message's from the channel", false, RunMode.Async)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task DelMesAsync(int delnum = 0)
        {
            if (delnum == 0) //If the number of messages is 0
            {
                await SendErrorMessage("Error", "You did not Specfify a Number of Messages to Delete");
                return;
            }

            var admin = Context.User;

            var messages = Context.Channel.GetMessagesAsync(delnum).Flatten();
            foreach (var h in await messages.ToArrayAsync())
            {
                await this.Context.Channel.DeleteMessageAsync(h);
            }
            await DeferAsync();
            await SendEmbedMessageAndLog("Top5Gaming", "`" + admin + "` Has purged " + Context.Channel);

        }

        [SlashCommand("warn","Issue a warning to the user", false, RunMode.Async)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task warn(IGuildUser user = null,  string reason = null)
        {
            await Context.Guild.DownloadUsersAsync();

            var admin = Context.User;

            if (user == null) //If no user was found
            {
                await SendErrorMessage("Error", "Could not find a UserId in the Warn request. Please try again or contact <@883019151327764531>");
                return;
            }

            if (reason == null) //If no reason was found
            {
                await SendErrorMessage("Error", "Please Speficfy a reason for the the warning");
                return;
            }

            if (user == admin) //If user and admin are the same person
            {
                await ReplyAsync("You cannot warn Yourself!");
                return;
            }
           
            sqlQuery = "INSERT INTO `warnings` (`UserID`, `warnReason`, `AdminID`) VALUES ('" + user.Id + "', '" +reason+ "', '"+ admin.Id+"')";
            SQLInsert();
            await SendEmbedMessage("Top5Gaming Warn Manager", user + " Has been warned by " + admin + " for " +reason);
            await SendLog("__Top5Gaming Warn Manager__", "**Member**        | " + user.Mention + "\n**Offense**        | " + reason + "\n**Length**        | Permanent\n **Punishment**       | WARNING" + "\n **Punished by**          | " + admin.Mention, user);
        }

        [SlashCommand("warnings","Display all warning's issued to the user" , false, RunMode.Async)]
        public async Task warnings(IGuildUser user = null)
        {
            await Context.Guild.DownloadUsersAsync();

            var commandUser = Context.User as IGuildUser;
            if (user == null)
            {
               await SendErrorMessage("Error", "Could not find a UserId in the Warn View request. Please try again or contact <@883019151327764531>");
                return;
            }

            if (!commandUser.GuildPermissions.ManageMessages && commandUser != user)
            {
                await ReplyAsync("You can only check your own warning history!");
                return;
            }
            await RespondAsync("Action Completed");
            SQLReadQuery = "SELECT * FROM `warnings` WHERE UserID = '" + user.Id + "'";
            SQLRead();
        }

        [SlashCommand("mute","Put a user into timeout mode for a set time period", false, RunMode.Async)]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task mute(IGuildUser user = null, int time = 0, string timePeriod = "m",  string reason = null)
        {
            await Context.Guild.DownloadUsersAsync();
            var admin = Context.User;

            if (user == null) //If user is null
            {
                await SendErrorMessage("Error", "Could not find a UserId in the Mute request. Please try again or contact <@883019151327764531>");
                return;
            }

            if (time == 0) // If no time was specified
            {
                await SendErrorMessage("Error", "Please Specfify a Time for the Mute");
                return;
            }

            if (reason == null) // If no reason supplied
            {
                await SendErrorMessage("Error", "Please Specfify a reason for the the mute");
                return;
            }

            if (timePeriod == "m")
            {
                await SendEmbedMessage("__Top5Gaming Mute Manager__", user.Mention + " has been muted for " + time + timePeriod + " for " + reason);
                await user.SetTimeOutAsync(TimeSpan.FromMinutes(time));
                await SendLog("__Top5Gaming Mute Manager__", "**Member**        | " + user.Mention + "\n**Offense**        | " + reason + "\n**Length**        | " + time + " minutes \n **Punishment**       | MUTE" + "\n **Punished by**          | " + admin.Mention, user);
            }
            else
            {
                await SendEmbedMessage("__Top5Gaming Mute Manager__", user.Mention + " has been muted for " + time + timePeriod + " for " + reason);
                await user.SetTimeOutAsync(TimeSpan.FromHours(time));
                await SendLog("__Top5Gaming Mute Manager__", "**Member**        | " + user.Mention + "\n**Offense**        | " + reason + "\n**Length**        | " + time + " hours \n **Punishment**       | MUTE" + "\n **Punished by**          | " + admin.Mention, user);
            }
        }

        [SlashCommand("unmute","Remove a user's timeout before the alloted period has ended", false, RunMode.Async)]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task unmute(IGuildUser user = null, ulong userb = 0)
        {
            await Context.Guild.DownloadUsersAsync();

            if (user == null) //If no user is supplied
            {
                await SendErrorMessage("Error", "Could not find a UserId in the Unmute request. Please try again or contact <@883019151327764531>");
                return;
            }

            if (user.TimedOutUntil == null)
            {
                await SendErrorMessage("Error", "This user is already in Timeout Mode");
                return;
            }
            var admin = Context.User;
            await user.RemoveTimeOutAsync();
            await SendEmbedMessage("Top5Gaming Mute Manager", user + " has been unmuted by " + admin);
            await SendLog("__Top5Gaming Mute Manager__", "**Member**        | " + user.Mention + "\n **Punishment**       | UNMUTE" + "\n **Punished by**          | " + admin.Mention, user);
        }

        [SlashCommand("say", "Make the bot respond with your message!")]
        [RequireUserPermission(GuildPermission.MentionEveryone)]
        public async Task Say( string text = null)
        {
                if (text == null) //Make the bot reply with nothing as a troll move. ||||||| Also avoids errors
                {
                text = "** **";
                }
            await RespondAsync(text);
        }

        [SlashCommand("nickname", "Change the nickname a user", false, RunMode.Async)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task nickname(IGuildUser user = null,  string newname = null)
        {
            await Context.Guild.DownloadUsersAsync();
            
            if (user == null)
            {
                await SendErrorMessage("Error", "Could not find a UserId in the Nickname request. Please try again or contact <@883019151327764531>");
                return;
            }

            if (newname == null)
            {
                await SendErrorMessage("Error", "Could not find a New Name in the Nickname request. Please try again or contact <@883019151327764531>");
                return;
            }

            await user.ModifyAsync(x =>
            {
                x.Nickname = newname;
            });
            await RespondAsync("Nickname changed!");

        }

        [SlashCommand("deletewarning", "Delete the specified Warn ID from the database")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task DeleteWarning(int WarnID = 0)
        {
            if (WarnID == 0)
            {
              await  ReplyAsync("Could not find a warn id.");
                return;
            }
            await SendLog("Top5Gaming Warn Manager",  Context.User.Mention + " has deleted warning " + WarnID);
            await RespondAsync("Action Completed");
            sqlQuery = "DELETE FROM `warnings` WHERE `WarnID` = " + WarnID + ";";
            SQLInsert();
        }

        [SlashCommand("deletewarnings", "Delete all warnings attached to a user", false, RunMode.Async)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task DeleteWarnings(IGuildUser user = null)
        {
            await Context.Guild.DownloadUsersAsync();

            if (user == null)
            {
                await ReplyAsync("Could not find a User id.");
                return;
            }
            await RespondAsync("All warnings for this user have been deleted");
            await SendLog("Top5Gaming Warn Manager", Context.User.Mention + " has deleted all warnings for " + user.Mention);
            sqlQuery = "DELETE FROM `warnings` WHERE `UserID` = " + user.Id + ";";
            SQLInsert();
        }

        [SlashCommand("helpadmin", "List all Commands's related to Administrators in Admin Chat")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task HelpA()
        {
            await SendMessageToModChat("Top5Gaming Admin Commands", "`!ban UserID reason` \n Permanently ban a Member from the Server \n \n `!kick UserID reason` \n Kick a Member from the Server  \n \n `!warn UserID reason` \n Give a formal warning to a Member \n \n `!mute UserID time m/h reason` \n Prevent a Member from talking for a set time period \n \n `!unmute UserID` \n Remove a Mute from a User \n \n `!purge number of message` \n Delete a defined number of messages from the chat \n \n `!nickname userID newname` \n Change a user's nickname via this command \n \n `!warnings UserID` \n View all User Warnings \n \n  `!deletewarning / delwarn/clearwarn warnID` \n Remove a Specified warning ID from the Database \n\n `!deletwarnings UserID` \n Delete all warnings for the specified User \n \n `!sticky content` \n Stick a message to the bottom of the channel \n \n `!say Message` \n Have the Bot repeat your Message \n \n `!ping` \n Get the response time for the bot ");
            await RespondAsync("This message has been sent to #ModChat");
        }

        [SlashCommand("ping", "Respond with the time it take's for the bot to communicate with Discord")]
        public async Task GetPing()
        {
            await RespondAsync("Current Ping: "+ "ms");
        }

        [SlashCommand("sticky", "Pin a message to the bottom of this channel **DISABLED**")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task sticky( string message = null)
        {
            if (message == null)
            {
                await ReplyAsync("You need to add a message to Stick!");
            }

            await SendEmbedMessageWithImage2("Sticked Message", "Message from the Administrative Team \n \n " + message , "");

            StickyContent = message;
            await RespondAsync("Action Completed");
        }

        [SlashCommand("getwarnings", "Count the number of warning's issued by each Administrator", false, RunMode.Async)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task GetAllWarnings(IGuildUser user = null)
        {
            await Context.Guild.DownloadUsersAsync();
            List<string> num = new List<string>();
            string query = "SELECT * FROM `warnings` WHERE `adminid` = " + user.Id;
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    num.Add(rdr[0].ToString());
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            await RespondAsync(user.Mention + " has issued " + num.Count() + " warnings");
        }

        public async Task SendSticky(SocketMessage message)
        {
            IMessage mes = message as IMessage;
            if (mes.Author.IsBot) return;
            await stickyMess.DeleteAsync();
            await SendEmbedMessageWithImage2("Sticked Message", "Message from the Administrative Team \n \n" + StickyContent , "");
        }

        [SlashCommand("rolesmenu", "Spawn the role's menu in this channel!")]
        public async Task Roles()
        { 
            
            var menuBuilder = new SelectMenuBuilder()
        .WithPlaceholder("Select a role")
        .WithCustomId("role-menu")
        .WithMinValues(1)
        .WithMaxValues(10)
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
        .AddOption("North America", "opt-n")
        .AddOption("South America", "opt-n")
        .AddOption("Europe", "opt-n")
        .AddOption("Africa", "opt-n")
        .AddOption("Asia", "opt-n")
        .AddOption("Oceania", "opt-n");

            var builder = new ComponentBuilder()
                .WithSelectMenu(menuBuilder);

            await RespondAsync("Get your role's here! We have notifications, pronouns, continents, and console's!", components: builder.Build());
        }

        public async Task SendErrorMessage(string title, string des)
        {
            var EmbedBuilder = new EmbedBuilder()
            .WithTitle(title)
            .WithDescription(des);
            Embed embed = EmbedBuilder.Build();
            await RespondAsync(embed: embed);
        }

        public async Task SendEmbedMessage(string title, string des)
        {
            var EmbedBuilder = new EmbedBuilder()
            .WithTitle(title)
            .WithThumbnailUrl("https://yt3.ggpht.com/ytc/AKedOLTQMXQnzj1y5uOfob1G0RFWacXDt9lnI8CRZ3JrNA=s900-c-k-c0x00ffffff-no-rj")
            .WithDescription(des);
            Embed embed = EmbedBuilder.Build();
            await RespondAsync(embed: embed);
        }

        public async Task SendEmbedMessageAndLog(string title, string des)
        {
            ITextChannel modlog = Context.Client.GetChannelAsync(745398099522093107) as ITextChannel;
            var EmbedBuilder = new EmbedBuilder()
            .WithTitle(title)
            .WithColor(255, 0, 0)
            .WithThumbnailUrl("https://yt3.ggpht.com/ytc/AKedOLTQMXQnzj1y5uOfob1G0RFWacXDt9lnI8CRZ3JrNA=s900-c-k-c0x00ffffff-no-rj")
            .WithDescription(des);
            Embed embed = EmbedBuilder.Build();
            await RespondAsync(embed: embed);
            Embed embedLog = EmbedBuilder.Build();
            await modlog.SendMessageAsync(embed: embedLog);
        }

        public async Task SendLog(string title, string des, IGuildUser user = null, bool isBan = false)
        {
            ITextChannel modlog = Context.Client.GetChannelAsync(745398099522093107) as ITextChannel;
            var EmbedBuilder = new EmbedBuilder()
            .WithTitle(title)
            .WithColor(255,0,0)
            .WithThumbnailUrl("https://yt3.ggpht.com/ytc/AKedOLTQMXQnzj1y5uOfob1G0RFWacXDt9lnI8CRZ3JrNA=s900-c-k-c0x00ffffff-no-rj")
            .WithDescription(des);
            Embed embed = EmbedBuilder.Build();
            await modlog.SendMessageAsync(embed: embed);
            if (user != null)
            {
                await user.SendMessageAsync(embed: embed);
                if (isBan)
                {
                    await user.SendMessageAsync("__Ban Appeal Form__ \n \n https://forms.gle/xYu3waoXtdiq6wMR6");
                }
            }
        }

        public async Task SendMessageToModChat(string title, string des)
        {
            ITextChannel adminchat = Context.Client.GetChannelAsync(585827591513178142) as ITextChannel;
            var EmbedBuilder = new EmbedBuilder()
            .WithTitle(title)
            .WithDescription(des);
            Embed embed = EmbedBuilder.Build();
            Embed embedLog = EmbedBuilder.Build();
            await adminchat.SendMessageAsync(embed: embedLog);
        }

        public async Task SendEmbedMessageWithImage2(string title, string des, string imageurl)
        {
            var EmbedBuilder = new EmbedBuilder()
            .WithTitle(title)
            .WithThumbnailUrl("https://yt3.ggpht.com/ytc/AKedOLTQMXQnzj1y5uOfob1G0RFWacXDt9lnI8CRZ3JrNA=s900-c-k-c0x00ffffff-no-rj")
            .WithColor(255, 0, 0)
            .WithImageUrl(imageurl)
            .WithDescription(des);
            Embed embed = EmbedBuilder.Build();
            stickyMess = await ReplyAsync(embed: embed);
        }
    }
}