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
    public class UserCommands : ModuleBase<SocketCommandContext>
    {
        private Emoji dice = new Emoji("🎲");
        private bool TopicCooldown = false;
        [Command("help"), Alias("help")]
        public async Task HelpMember()
        {
            await SendEmbedMessage("Top5Gaming User Commands", "`!explain memes/giveaways/t5g/promo/admin/xp` \n Respond with an explanation about the topic! \n \n `!banappeal` \n Display a link to our Ban Appeal form \n \n`!submit` \n Submit your clip to T5G! \n \n `!session` \n Can I play with T5g? \n \n `!invite` \n Get the Invite link for the Server \n \n `!team` \n Display the Team behind Top5Gaming \n \n !`joke` \n  Send a joke! \n \n `!fact` \n Send a Random Fact with zero context to surely confuse you! \n \n `!rps` \n Take me on in a game of Rock Paper and Scissors! \n\n `!1-10`\n I will pick a number between 1 and 10 and you have to guess it! \n \n `!dice`\n Role 2 dice \n \n `!yesorno` \n Make the bot respond with either yes or no to answer important decisions! \n \n `!whowins 'option a' 'option b' max of 5 options` \n Pick a random winner based on a minimum of two and max of 5 options");
        }

        [Command("submit"), Alias("Submit")]
        public async Task submit()
        {
            await SendEmbedMessageWithTitleLink("Submit A Clip!", "You can submit your clip to Top5Gaming through our clips website by clicking the Submit A Clip button!", "https://media.discordapp.net/attachments/740328591417409707/750470586807091230/unknown.png?width=1347&height=682");
        }

        [Command("session"), Alias("Session")]
        public async Task Session()
        {
            await SendEmbedMessage("Can I play with Top5Gaming?", "Top5Gaming does indeed play with users on the Discord sometimes, so make sure you stay active for when Tommy or anyone else joins the chat!");
        }

        [Command("invite"), Alias("Invite")]
        public async Task invite()
        {
            await SendEmbedMessage("Server Invite", "You can join the T5G Discord by using the link Discord.gg/T5G");
        }

        [Command("Team"), Alias("team")]
        public async Task team()
        {
            await SendEmbedMessage("Who Runs Top5Gaming?", "Lots of people work behind the scenes to give you the videos you know and love! \n \n • **Tommy** (CEO) \n • **Dynamic** (COO) \n • **Jukebox** (Director of Operations) \n • **Shifty** (CCO) \n • **Sammy** (Producer) \n • **Ben** (Head Writer & Social Media) \n • **Denni** (Head Graphic Artist) \n • **Consequence** (Video Production)\n • **Tyler** (Audio Production)");
        }

        [Command("rps")]
        public async Task rps()
        {
            await ReplyAsync("Let's play Rock Paper Scissors!");
            await ReplyAsync("I will close my eyes for 10 seconds, pick Rock, Paper, or Scissors before I open them again!");

           await Task.Delay(10000);

            string[] responselist = {"Rock!" , "Paper!", "Scissors!" };
            Random rnd = new Random();
            int index = rnd.Next(responselist.Length);
            await ReplyAsync(responselist[index]);
        }

        [Command("dice")]
        public async Task dicer()
        {
                await ReplyAsync("Rolling dice! " + dice + dice);
                await Task.Delay(5000);
                int[] responselist = { 1, 2, 3, 4, 5, 6 };
                Random rnd = new Random();
                int index = rnd.Next(responselist.Length);
                int index2 = rnd.Next(responselist.Length);
                await (ReplyAsync(dice.ToString() + dice + "You rolled a " + responselist[index] + " and a " + responselist[index2]));
        }

        [Command("1-10")]
        public async Task guess()
        {
            await ReplyAsync("Guess the Number I am thinking of between 1 and 10! ");
            await Task.Delay(10000);

            int[] numbers = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            Random rnd = new Random();
            int index = rnd.Next(numbers.Length);
            await ReplyAsync("I was think of the number " + numbers[index]);
        }

        [Command("yesorno")]
        public async Task yesorno()
        {
            string[] responses = { "Yes", "No" };
            Random rnd = new Random();
            int index = rnd.Next(responses.Length);
            await ReplyAsync("My answer is " + responses[index]);
        }

        [Command("secret"), Alias("secretrole", "easteregg")]
        public async Task secretRole()
        {
            await Context.Message.DeleteAsync();
            SocketGuildUser rolecheck = Context.User as SocketGuildUser;
            if (rolecheck.Roles.Any(i => i.Name == "Secret Role"))
            {
                await ReplyAsync(Context.User.Mention + " you already have the *Secret* role");
                return;
            }else
            {
                IGuildUser user = Context.User as IGuildUser;
                await ReplyAsync(user.Mention + " has found the *Secret* role!");
                await user.AddRoleAsync(807525985859797023);
            }
        }

        [Command("banappeal"), Alias("appeal")]
        public async Task banAppeal()
        {
            await ReplyAsync("__Top5Gaming Ban Appeal Form__ \n \n https://forms.gle/xYu3waoXtdiq6wMR6");
        }

        [Command("explain")]
        public async Task explain(string x = null)
        {
            if (x == null)
            {
                await ReplyAsync("How can I explains something if I don't know what to explain!");
            }
            else if (x == "memes")
            {
                await ReplyAsync("<#788551325913710592> is locked to members with the **Regular** role and above! This was implemented due to __inappropiate__ memes overloading Staff. \n You can get the Regular role by being active in chat!");
            }
            else if (x == "giveaways")
            {
                await ReplyAsync("Giveaways appear periodically, most usually during a Live Stream, or a public holiday such as Christmas! You can subscribe to the Giveaway Announcement role in <#742873333359706264>");
            }
            else if (x == "promo")
            {
                await SendEmbedMessage("Self Promotion", "Self promotion in any form is against the rules. Please see our rules channel for more information. \n \n Advertising in DMs will lead to an instant ban with no warnings.");
            }
            else if (x == "admin")
            {
                await SendEmbedMessage("Can I be an Admin?", "We are always looking for new Admins! However, we do not take applications but instead hand-pick Administrators. Those who show good skill and promise will be randomly chosen.");
            }
            else if (x == "t5g")
            {
                await SendEmbedMessage("Where are T5G?", "The T5G team are very busy making videos! They occasionally come into the server to chat, host giveaways or play games, so **stay active** to never miss it!");
            }
            else if (x == "xp")
            {
                await SendEmbedMessage("What can I get for gaining XP?", "**Newbie** (Level 1 Text) \n **Novice** (Level 5 Text) \n **Regular** (Level 10 Text) \n **Pro** (Level 20 Text) \n **Champion** (Level 30 Text & Level 1 Voice) \n **Warrior** (Level 50 Text & Level 5 Voice) \n **Leader** (Level 65 Text & Level 20 Voice) \n **Captain** (Level 75 Text & Level 40 Voice) \n **Chief** (Level 85 Text & Level 60 Voice) \n **Legend** (Level 95 Text & Level 65 Voice)\n **God** (Level 100 Text & Level 100 Voice) \n **The One True God** (Only Available for First Person to Get God Role)");
            }
        }

        [Command("topic"), Alias("Topic", "discuss")]
        public async Task RandomTopic()
        {
            string[] topics = { "What is your Favourite Fortnite season and why?", "What Sport do you guys like the most?", "What are you favourite hobbies/pastimes?", "Are indigo and violet colors of the rainbow or are they just purple?", "What is your favourite Fortnite skin?", "Which of Top5Gaming's Video's do you like the best?", "Random rnd = new Random();", "What is your favourite game mode?", "Other than Fortnite, what game do you like the most?", "Does the week start on Sunday, or Monday?", "What is the best thing about this Discord?", "What is the most iconic Christmas song?", "What is your favourite sport's team?" };
            Random rand = new Random();
            int index = rand.Next(topics.Length);
            await Task.Run(async () =>
            {
                int timer = 0;
                TopicCooldown = true;
                while (TopicCooldown == true)
                {
                    timer++;
                }
                if (timer >= 360000)
                {
                    TopicCooldown = false;
                    timer = 0;
                }
            });
        }

        [Command("whowins"), Alias("who", "winner")]
        public async Task WhoWins(string a = null, string b = null, string c = null, string d = null, string e = null)
        {
            if (b == null)
            {
                await ReplyAsync("I guess " + a + " wins because there isn't a second option");
            }

            string[] options =
            {
                a,
                b,
                c,
                d,
                e,
            };
            /*
            if (c != null)
            {
                options.Append(c);
            }
            if (d != null)
            {
                options.Append(d);
            }
            if (e != null)
            {
                options.Append(e);
            }
            await Task.Delay(5000);
            Console.WriteLine("The list of winners are " + options);
            */
            Random rnd = new Random();
            int index = rnd.Next(options.Length);
            await ReplyAsync("The winner is " + options[index]);
        }
       /* [Command("translate"), Alias("Translate")]
        public async Task TranslateTextDisplayed9(string target, string text)
        {
            TranslateClient client = new TranslateClient(text);
            string translatedtext  = client.Translate(text, "Language." + target, "Language.English");
           await ReplyAsync(translatedtext);
        }
        */
        

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
            .WithColor(255, 0, 0)
            .WithDescription(des);
            Embed embed = EmbedBuilder.Build();
            await ReplyAsync(embed: embed);
        }

        public async Task SendEmbedMessageWithTitleLink(string title, string des, string link)
        {
            var EmbedBuilder = new EmbedBuilder()
            .WithTitle(title)
            .WithUrl("https://top5clips.com/")
            .WithThumbnailUrl("https://yt3.ggpht.com/ytc/AKedOLTQMXQnzj1y5uOfob1G0RFWacXDt9lnI8CRZ3JrNA=s900-c-k-c0x00ffffff-no-rj")
            .WithColor(255, 0, 0)
            .WithImageUrl(link)
            .WithDescription(des);
            Embed embed = EmbedBuilder.Build();
            await ReplyAsync(embed: embed);
        }
        public async Task SendEmbedMessageWithImage(string title, string des, string link)
        {
            var EmbedBuilder = new EmbedBuilder()
            .WithTitle(title)
            .WithUrl("https://top5clips.com/")
            .WithThumbnailUrl("https://yt3.ggpht.com/ytc/AKedOLTQMXQnzj1y5uOfob1G0RFWacXDt9lnI8CRZ3JrNA=s900-c-k-c0x00ffffff-no-rj")
            .WithColor(255, 0, 0)
            .WithImageUrl(link)
            .WithDescription(des);
            Embed embed = EmbedBuilder.Build();
            await ReplyAsync(embed: embed);
        }
    }
}