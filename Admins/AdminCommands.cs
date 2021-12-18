using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.API;
using System.Linq;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Globalization;
using Bot;
using System.Numerics;

namespace Bot.Admins
{
    public class AdminCommands : ModuleBase<SocketCommandContext> 
    {
        public string sqlQuery;
        public string SQLReadQuery;
        public string SQLMUteRemove = "UPDATE `mutes` SET MuteUntil = MuteUntil - 1";
        private bool hasSentLog = false;
        private IMessage stickyMess;
        private string StickyContent;
        private IGuildUser filler;
        private bool deleteCommandsOnUse = true; // Set This to false to disable Admin Commands deleting after being used
        private bool reasonsRequired = true; //Set this to false to disable reasons being required on Punishments
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

                while (rdr.Read())
                {
                    if (rdr.HasRows)
                    {
                        await SendEmbedMessage("__Top5Gaming Warning Manager__", "**Warn ID** | " + rdr[0] + "\n **User** | <@" + rdr[1] + ">\n **Reason** | " + rdr[2] + "\n **Issued by** | <@" + rdr[3] + ">");
                    }else
                    {
                        await SendEmbedMessage("__Top5Gaming Warning Manger__", "No warnings found for this user");
                    }
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
               // await SendEmbedMessage("MySQL", "Attempting to connect to database");
                conn.Open();
               // await SendEmbedMessage("MySQL", "Connection to Database Successful");
                MySqlCommand cmd = new MySqlCommand(sqlQuery, conn);

                cmd.ExecuteNonQuery();

                hasSentLog = false;
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

    [Command("Ban", RunMode = RunMode.Async), Alias("ban")]        
	[RequireUserPermission(GuildPermission.BanMembers)]
        public async Task Ban(IGuildUser user = null, [Remainder] string reason = null)
        {
            try
            {
                Context.Guild.DownloadUsersAsync();
                if (!Context.Guild.HasAllMembers)
                {
                    await SendErrorMessage("Critical ERROR:", "Unable to download the User's list. ");
                }
                if (deleteCommandsOnUse)
                {
                    await Context.Message.DeleteAsync();
                }
                var admin = Context.User; // Admin = The person who executed the command
                                          // Error Handling
                                          // This is where we check for things that should prevent the ban

                if (user == admin) //If the User and Person executing the command is the same
                {
                    await ReplyAsync("You cannot Ban yourself!");
                    return;
                }

                if (user == null) //If no user was mentioned
                {
                    await SendErrorMessage("Ban Request Failed", "Could not find a UserId in the Ban request. Please try again or contact <@543192194803302431>");
                    return;
                }

                if (user.GuildPermissions.ManageMessages) //If the user is an admin
                {
                    await ReplyAsync("You cannot Ban an Admin!");
                    return;
                }

                if (reason == null && reasonsRequired) //If no reason was supplied and reasons are required
                {
                    await SendErrorMessage("Ban Request Failed", "Could not find a Reason in your Ban. Please try again or contact <@543192194803302431>");
                    return;
                }

                if (reason == null && !reasonsRequired)
                {
                    reason = "Not Specified";
                }
                await user.BanAsync(7, reason + " banned by " + admin);
                await SendEmbedMessage("__Top5Gaming Ban Manager__", user + " has been banned by " + admin + "\n \n For " + reason);
                await SendLog("__Top5Gaming Ban Manager__", "**Member**        | " + user.Mention + "\n**Offense**        | " + reason + "\n**Length**        | Permanent\n **Punishment**       | BAN" + "\n **Punished by**          | " + admin.Mention, user, true);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }        
	}

        [Command("Kick", RunMode = RunMode.Async), Alias("kick")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task Kick(IGuildUser user = null, [Remainder] string reason = null)
        {
            await Context.Guild.DownloadUsersAsync();
            if (deleteCommandsOnUse)
            {
                await Context.Message.DeleteAsync();
            }
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
                await SendErrorMessage("Kick Request Failed", "Could not find a UserId in the Kick request. Please try again or contact <@543192194803302431>");
                return;
            }
            if (reason == null && reasonsRequired) //If no reason is found
            {
                await SendErrorMessage("Kick Attempt", "Could not Find a Reason in your Kick. Please try again or contact <@543192194803302431>");
                return;
            }

            if (user.GuildPermissions.ManageMessages) //If the user has admin perms
            {
                await ReplyAsync("You cannot Kick an Admin!");
                return;
            }

            if (reason == null && !reasonsRequired)
            {
                reason = "Not Specified";
            }
            await user.KickAsync(reason);

            await SendEmbedMessage("Top5Gaming Kick Manager", user + " has been Kicked by " + admin + "\n \n" + reason + " Kicked by " + admin);
            await SendLog("__Top5Gaming Kick Manager__", "**Member**        | " + user.Mention + "\n**Offense**        | " + reason + "\n**Length**        | Inapplicable\n **Punishment**       | KICK" + "\n **Punished by**          | " + admin.Mention, user);
        }
        

        [Command("purge", RunMode = RunMode.Async), Alias("clear", "remove", "delete")]
        [RequireUserPermission(GuildPermission.ManageMessages, ErrorMessage = "You do not have the required permission `Manage Messages`.")]
        public async Task DelMesAsync(int delnum = 0)
        {
            if (delnum == 0) //If the number of messages is 0
            {
                await SendErrorMessage("Error", "You did not Specfify a Number of Messages to Delete");
                return;
            }

            var admin = Context.User;
            await Context.Message.DeleteAsync();

            var messages = Context.Channel.GetMessagesAsync(delnum).Flatten();
            foreach (var h in await messages.ToArrayAsync())
            {
                await this.Context.Channel.DeleteMessageAsync(h);
            }

            await SendEmbedMessageAndLog("Top5Gaming", "`" + admin + "` Has purged " + Context.Channel);
        }

        [Command("Warn" , RunMode = RunMode.Async), Alias("warn")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task warn(IGuildUser user = null, [Remainder] string reason = null)
        {
            await Context.Guild.DownloadUsersAsync();

            if (deleteCommandsOnUse)
            {
                await Context.Message.DeleteAsync();
            }
            var admin = Context.User;

            if (user == null) //If no user was found
            {
                await SendErrorMessage("Error", "Could not find a UserId in the Warn request. Please try again or contact <@543192194803302431>");
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

        [Command("warnings", RunMode = RunMode.Async), Alias("warns")]
        public async Task warnings(IGuildUser user = null)
        {
            await Context.Guild.DownloadUsersAsync();

            var commandUser = Context.User;
            if (user == null)
            {
               await SendErrorMessage("Error", "Could not find a UserId in the Warn View request. Please try again or contact <@543192194803302431>");
                return;
            }
            SocketGuildUser rolecheck = commandUser as SocketGuildUser;
            if (commandUser != user && !rolecheck.Roles.Any(i => i.Name == "Administrator"))
            {
                await ReplyAsync("You can only check your own warning history!");
                return;
            }
            SQLReadQuery = "SELECT * FROM `warnings` WHERE UserID = '" + user.Id + "'";
            SQLRead();

        }
        [Command("mute", RunMode = RunMode.Async), Alias("Mute")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task mute(IGuildUser user = null, int time = 0, [Remainder] string reason = null)
        {
            await Context.Guild.DownloadUsersAsync();

            if (deleteCommandsOnUse)
            {
                await Context.Message.DeleteAsync();
            }
            var admin = Context.User;

            if (user == null) //If user is null
            {
                await SendErrorMessage("Error", "Could not find a UserId in the Mute request. Please try again or contact <@543192194803302431>");
                return;
            }

            if (time == 0) // If no time was specified
            {
                await SendErrorMessage("Error", "Please Specfify a Time for the Mute");
                return;
            }

            if (reason == null && reasonsRequired) // If no reason supplied
            {
                await SendErrorMessage("Error", "Please Specfify a reason for the the mute");
                return;
            }

            if (user == admin) // If the user is the admin
            {
                await ReplyAsync("You cannot mute Yourself!");
                return;
            }

            if (reason == null && !reasonsRequired)
            {
                reason = "Not Specified";
            }

            //Run database code
            if (reason.StartsWith("m"))
            {
                sqlQuery = "INSERT INTO `mutes` (`UserID`, `MuteUntil`, `Reason`, `AdminID`) VALUES ('" + user.Id + "', '" + time * 60000 + "', '" + reason + "', '" + admin.Id + "')";
                SQLInsert();
                await SendEmbedMessage("Top5Gaming Mute Manager", "Muted " + user + " for " + time + " minutes for" + reason + " \n Muted by" + admin);
                await SendLog("__Top5Gaming Mute Manager__", "**Member**        | " + user.Mention + "\n**Offense**        | " + reason + "\n**Length**        | " + time + " minutes \n **Punishment**       | MUTE" + "\n **Punished by**          | " + admin.Mention, user);
            }
            if (reason.StartsWith("h"))
            {
                sqlQuery = "INSERT INTO `mutes` (`UserID`, `MuteUntil`, `Reason`, `AdminID`) VALUES ('" + user.Id + "', '" + time * 600000 + "', '" + reason + "', '" + admin.Id + "')";
                SQLInsert();
                await SendEmbedMessage("Top5Gaming Mute Manager", "Muted " + user + " for " + time + " hours for" + reason + " \n Muted by" + admin);
                await SendLog("__Top5Gaming Mute Manager__", "**Member**        | " + user.Mention + "\n**Offense**        | " + reason + "\n**Length**        | " + time + " hours \n **Punishment**       | MUTE" + "\n **Punished by**          | " + admin.Mention, user);
            }
            sqlQuery = "INSERT INTO `warnings` (`UserID`, `warnReason`, `AdminID`) VALUES ('" + user.Id + "', 'Muted for " + reason + "', '" + admin.Id + "')";
            SQLInsert();
            await user.AddRoleAsync(553340347606892544);
        }

        [Command("unmute", RunMode = RunMode.Async), Alias("Unmute")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task unmute(IGuildUser user = null, ulong userb = 0)
        {
            await Context.Guild.DownloadUsersAsync();
            if (deleteCommandsOnUse)
            {
                await Context.Message.DeleteAsync();
            }

            if (user == null) //If no user is supplied
            {
                await SendErrorMessage("Error", "Could not find a UserId in the Unmute request. Please try again or contact <@543192194803302431>");
                return;
            }

            if (userb != 0)
            {
                user = Context.Guild.GetUser(userb);
            }

            try
            {
                SocketGuildUser rolecheck = user as SocketGuildUser;
                if (!rolecheck.Roles.Any(i => i.Name == "Muted"))
                {
                    await SendErrorMessage("Error", "This user does not have the role 'Muted'.");
                    return;
                }
                var admin = Context.User;
                await user.RemoveRoleAsync(553340347606892544);
                sqlQuery = "DELETE FROM `mutes` WHERE `UserID` = " + user.Id + ";";
                SQLInsert();
                await SendEmbedMessage("Top5Gaming Mute Manager", user + " has been unmuted by " + admin);
                await SendLog("__Top5Gaming Mute Manager__", "**Member**        | " + user.Mention + "\n **Punishment**       | UNMUTE" + "\n **Punished by**          | " + admin.Mention, user);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error in the unmute \n \n \n " + ex.ToString());
                await SendMessageToModChat("ERROR", "Unmute request failed. Full details logged in Console.");
            }
        }

        [Command("embed"), Alias("Embed")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task SendEmbedMessageWithImage(string title, string des , string imageurl)
        {
            if (deleteCommandsOnUse)
            {
                await Context.Message.DeleteAsync();
            }
            var EmbedBuilder = new EmbedBuilder()
            .WithTitle(title)
            .WithThumbnailUrl("https://yt3.ggpht.com/ytc/AKedOLTQMXQnzj1y5uOfob1G0RFWacXDt9lnI8CRZ3JrNA=s900-c-k-c0x00ffffff-no-rj")
            .WithColor(255, 0, 0)
            .WithImageUrl(imageurl)
            .WithDescription(des);
            Embed embed = EmbedBuilder.Build();
            await ReplyAsync(embed: embed);
        }


        [Command("Say"), Alias("say")]
        [RequireUserPermission(GuildPermission.MentionEveryone)]
        public async Task Say([Remainder] string text = null)
        {
                await Context.Message.DeleteAsync(); 
                if (text == null) //Make the bot reply with nothing as a troll move. ||||||| Also avoids errors
                {
                text = "** **";
                }
                await ReplyAsync(text);
        }

        [Command("nickname", RunMode = RunMode.Async), Alias("nick")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task nickname(IGuildUser user = null, [Remainder] string newname = null)
        {
            await Context.Guild.DownloadUsersAsync();
            
            if (deleteCommandsOnUse)
            {
               await Context.Message.DeleteAsync();
            }
            if (user == null)
            {
                await SendErrorMessage("Error", "Could not find a UserId in the Nickname request. Please try again or contact <@543192194803302431>");
                return;
            }

            if (newname == null)
            {
                await SendErrorMessage("Error", "Could not find a New Name in the Nickname request. Please try again or contact <@543192194803302431>");
                return;
            }

           
            await user.ModifyAsync(x =>
            {
                x.Nickname = newname;
            });
        }

        [Command("deletewarning") , Alias("delwarn", "clearwarn", "clearwarning")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task DeleteWarning(int WarnID = 0)
        {
            if (deleteCommandsOnUse)
            {
                await Context.Message.DeleteAsync();
            }

            if (WarnID == 0)
            {
              await  ReplyAsync("Could not find a warn id.");
                return;
            }
            await SendLog("Top5Gaming Warn Manager",  Context.User.Mention + " has deleted warning " + WarnID);
            sqlQuery = "DELETE FROM `warnings` WHERE `WarnID` = " + WarnID + ";";
            SQLInsert();
        }

        [Command("deletewarnings", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task DeleteWarnings(IGuildUser user = null)
        {
            await Context.Guild.DownloadUsersAsync();

            if (deleteCommandsOnUse)
            {
                await Context.Message.DeleteAsync();
            }

            if (user == null)
            {
                await ReplyAsync("Could not find a User id.");
                return;
            }
            await SendLog("Top5Gaming Warn Manager", Context.User.Mention + " has deleted all warnings for " + user.Mention);
            sqlQuery = "DELETE FROM `warnings` WHERE `UserID` = " + user.Id + ";";
            SQLInsert();
        }
        [Command("fixdelay")]
        public async Task FixBot()
        {
            sqlQuery = "DELETE FROM `mutes` WHERE `MuteUntil` > 0";
            SQLInsert();
            await ReplyAsync("Cleaned all bugs!");
        }

        [Command("HelpAdmin"), Alias("helpadmin")]
        [RequireUserPermission(GuildPermission.ManageMessages, ErrorMessage = "You are not an Admin!")]
        public async Task HelpA()
        {
            if (deleteCommandsOnUse)
            {
                await Context.Message.DeleteAsync();
            }

            await SendMessageToModChat("Top5Gaming Admin Commands", "`!ban UserID reason` \n Permanently ban a Member from the Server \n \n `!kick UserID reason` \n Kick a Member from the Server  \n \n `!warn UserID reason` \n Give a formal warning to a Member \n \n `!mute UserID time m/h reason` \n Prevent a Member from talking for a set time period \n \n `!unmute UserID` \n Remove a Mute from a User \n \n `!purge number of message` \n Delete a defined number of messages from the chat \n \n `!nickname userID newname` \n Change a user's nickname via this command \n \n `!warnings UserID` \n View all User Warnings \n \n  `!deletewarning / delwarn/clearwarn warnID` \n Remove a Specified warning ID from the Database \n\n `!deletwarnings UserID` \n Delete all warnings for the specified User \n \n `!sticky content` \n Stick a message to the bottom of the channel \n \n `!say Message` \n Have the Bot repeat your Message \n \n `!ping` \n Get the response time for the bot ");
        }

        [Command("ping")]
        public async Task GetPing()
        {
            await ReplyAsync("Current Ping:" + Context.Client.Latency + "ms");
        }

        [Command("sticky")]
        public async Task sticky([Remainder] string message = null)
        {
            if (deleteCommandsOnUse)
            {
                await Context.Message.DeleteAsync();
            }

            if (message == null)
            {
                await ReplyAsync("You need to add a message to Stick!");
            }

            await SendEmbedMessageWithImage2("Sticked Message", "Message from the Administrative Team \n \n " + message , "");

            StickyContent = message;

            Context.Client.MessageReceived += SendSticky;

        }

        [Command("getwarnings")]
        public async Task GetAllWarnings(IGuildUser user)
        {
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
            await ReplyAsync(user.Mention + " has issued " + num.Count() + " warnings");
        }
        /*
         * 
         * 
         * This is where we store all of our Refernce methods for this script.
         * We have an error handling embed and a regular message embed
         * This is so we keep the code tidy and short, rather than build an embed message every few lines 
         * 
         * 
         * 
         */

            public async Task SendSticky(SocketMessage message)
            {
            IMessage mes = message as IMessage;
            if (mes.Author.IsBot) return;
 
            await stickyMess.DeleteAsync();
            await SendEmbedMessageWithImage2("Sticked Message", "Message from the Administrative Team \n \n" + StickyContent , "");
            
            //await ReplyAsync("Debug Bravo");
        }

        public async Task SendErrorMessage(string title, string des)
        {
            var EmbedBuilder = new EmbedBuilder()
            .WithTitle(title)
            .WithDescription(des);
            Embed embed = EmbedBuilder.Build();
            await ReplyAsync(embed: embed);
        }

        public async Task SendEmbedMessage(string title, string des)
        {
            var EmbedBuilder = new EmbedBuilder()
            .WithTitle(title)
            .WithThumbnailUrl("https://yt3.ggpht.com/ytc/AKedOLTQMXQnzj1y5uOfob1G0RFWacXDt9lnI8CRZ3JrNA=s900-c-k-c0x00ffffff-no-rj")
            .WithDescription(des);
            Embed embed = EmbedBuilder.Build();
            await ReplyAsync(embed: embed);
        }

        public async Task SendModEmbed(string title, string reason)
        {
            var EmbedBuilder = new EmbedBuilder()
            .WithTitle(title)
            .WithThumbnailUrl("https://yt3.ggpht.com/ytc/AKedOLTQMXQnzj1y5uOfob1G0RFWacXDt9lnI8CRZ3JrNA=s900-c-k-c0x00ffffff-no-rj")
            .WithColor(255, 0, 0)
            .WithDescription(reason);
            Embed embed = EmbedBuilder.Build();
            await ReplyAsync(embed: embed);
            ITextChannel modlog = Context.Client.GetChannel(745398099522093107) as ITextChannel;
            await modlog.SendMessageAsync(embed: embed);
        }

        public async Task SendEmbedMessageAndLog(string title, string des)
        {
            ITextChannel modlog = Context.Client.GetChannel(745398099522093107) as ITextChannel;
            var EmbedBuilder = new EmbedBuilder()
            .WithTitle(title)
            .WithColor(255, 0, 0)
            .WithThumbnailUrl("https://yt3.ggpht.com/ytc/AKedOLTQMXQnzj1y5uOfob1G0RFWacXDt9lnI8CRZ3JrNA=s900-c-k-c0x00ffffff-no-rj")
            .WithDescription(des);
            Embed embed = EmbedBuilder.Build();
            await ReplyAsync(embed: embed);
            Embed embedLog = EmbedBuilder.Build();
            await modlog.SendMessageAsync(embed: embedLog);
        }

        public async Task SendLog(string title, string des, IGuildUser user = null, bool isBan = false)
        {
            ITextChannel modlog = Context.Client.GetChannel(745398099522093107) as ITextChannel;
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
            ITextChannel adminchat = Context.Client.GetChannel(585827591513178142) as ITextChannel;
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