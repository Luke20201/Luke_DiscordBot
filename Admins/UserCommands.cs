using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Bot.Admins
{
    public class UserCommands : InteractionModuleBase<IInteractionContext>
    {
        private Emoji dice = new Emoji("🎲");
        [SlashCommand("help", "Get a list of all Command's")]
        public async Task HelpMember()
        {
            await SendEmbedMessage("Top5Gaming User Commands", "`!explain memes/giveaways/t5g/promo/admin/xp` \n Respond with an explanation about the topic! \n \n `!banappeal` \n Display a link to our Ban Appeal form \n \n`!submit` \n Submit your clip to T5G! \n \n `!session` \n Can I play with T5g? \n \n `!invite` \n Get the Invite link for the Server \n \n `!team` \n Display the Team behind Top5Gaming \n \n !`joke` \n  Send a joke! \n \n `!fact` \n Send a Random Fact with zero context to surely confuse you! \n \n `!rps` \n Take me on in a game of Rock Paper and Scissors! \n\n `!1-10`\n I will pick a number between 1 and 10 and you have to guess it! \n \n `!dice`\n Role 2 dice \n \n `!yesorno` \n Make the bot respond with either yes or no to answer important decisions! \n \n `!whowins 'option a' 'option b' max of 5 options` \n Pick a random winner based on a minimum of two and max of 5 options");
        }

        [SlashCommand("submit", "Learn how to submit a clip to the T5G Team")]
        public async Task submit()
        {
            await SendEmbedMessageWithTitleLink("Submit A Clip!", "You can submit your clip to Top5Gaming through our clips website by clicking the Submit A Clip button!", "https://media.discordapp.net/attachments/740328591417409707/750470586807091230/unknown.png?width=1347&height=682");
        }

        [SlashCommand("session", "Can I Play with Top5Gaming?")]
        public async Task Session()
        {
            await SendEmbedMessage("Can I play with Top5Gaming?", "Top5Gaming does indeed play with users on the Discord sometimes, so make sure you stay active for when Tommy or anyone else joins the chat!");
        }

        [SlashCommand("team", "Who run's Top5Gaming?")]
        public async Task team()
        {
            await SendEmbedMessage("Who Runs Top5Gaming?", "Lots of people work behind the scenes to give you the videos you know and love! \n \n • **Tommy** (CEO) \n • **Dynamic** (COO) \n • **Jukebox** (Director of Operations) \n • **Shifty** (CCO) \n • **Sammy** (Producer) \n • **Ben** (Head Writer & Social Media) \n • **Denni** (Head Graphic Artist) \n • **Consequence** (Video Production)\n • **Tyler** (Audio Production)");
        }

        [SlashCommand("rps", "Rock! Paper! Scissors!")]
        public async Task rps()
        {
            await ReplyAsync("Let's play Rock Paper Scissors!");
            await RespondAsync("I will close my eyes for 10 seconds, pick Rock, Paper, or Scissors before I open them again!");

           await Task.Delay(10000);

            string[] responselist = {"Rock!" , "Paper!", "Scissors!" };
            Random rnd = new Random();
            int index = rnd.Next(responselist.Length);
            await ReplyAsync(responselist[index]);
        }

        [SlashCommand("dice", "Roll a set of die!")]
        public async Task dicer()
        {
                await RespondAsync("Rolling dice! " + dice + dice);
                await Task.Delay(5000);
                int[] responselist = { 1, 2, 3, 4, 5, 6 };
                Random rnd = new Random();
                int index = rnd.Next(responselist.Length);
                int index2 = rnd.Next(responselist.Length);
                await (ReplyAsync(dice.ToString() + dice + "You rolled a " + responselist[index] + " and a " + responselist[index2]));
        }

        [SlashCommand("1-10", "Guess a random number between 1 and 10!")]
        public async Task guess()
        {
            await RespondAsync("Guess the Number I am thinking of between 1 and 10! ");
            await Task.Delay(10000);

            int[] numbers = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            Random rnd = new Random();
            int index = rnd.Next(numbers.Length);
            await ReplyAsync("I was think of the number " + numbers[index]);
        }

        [SlashCommand("yesorno", "I will answer life's biggest question with yes, or no!")]
        public async Task yesorno()
        {
            string[] responses = { "Yes", "No" };
            Random rnd = new Random();
            int index = rnd.Next(responses.Length);
            await ReplyAsync("My answer is " + responses[index]);
        }

        [SlashCommand("secret", "Something secret is here.")]
        public async Task secretRole()
        {
            SocketGuildUser rolecheck = Context.User as SocketGuildUser;
            if (rolecheck.Roles.Any(i => i.Name == "Secret Role"))
            {
                await RespondAsync(Context.User.Mention + " you already have the *Secret* role");
                return;
            }else
            {
                IGuildUser user = Context.User as IGuildUser;
                await RespondAsync(user.Mention + " has found the *Secret* role!");
                await user.AddRoleAsync(807525985859797023);
            }
        }

        [SlashCommand("banappeal", "Display a link to the ban appeal form.")]
        public async Task banAppeal()
        {
            await RespondAsync("__Top5Gaming Ban Appeal Form__ \n \n https://forms.gle/xYu3waoXtdiq6wMR6");
        }

        [SlashCommand("explain", "Ask me to explain one of the topic's!")]
        public async Task explain([Choice("Memes", "memes"), Choice("Giveaways", "giveaways"), Choice("Promo", "promo"), Choice("Admin", "admin"), Choice("T5G", "t5g"), Choice("XP", "xp")] string x)
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

        [SlashCommand("whowins", "Give me a number of choice's, and I will tell you who win's!")]
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
            Random rnd = new Random();
            int index = rnd.Next(options.Length);
            await ReplyAsync("The winner is " + options[index]);
        }
        #region jokesAndFacts
        String[] jokelist = new String[]
        {
            "What’s the best thing about Switzerland? I don’t know, but the flag is a big plus.",
            "Did you hear about the mathematician who’s afraid of negative numbers? He’ll stop at nothing to avoid them.",
            "Hear about the new restaurant called Karma? There’s no menu: You get what you deserve.",
            "Why did the scarecrow win an award? Because he was outstanding in his field.",
            "How does the ocean say hello? It waves.",
            "I was going to tell a time traveling joke, but you guys didn't like it.",
            "What do you call it when a snowman throws a tantrum? A meltdown.",
            "What is the opposite of a croissant? A happy uncle.",
            "Which branch of the military accepts toddlers? The infantry.",
            "I finally decided to sell my vacuum cleaner. All it was doing was gathering dust!",
            "You know there's no official training for trash collectors? They just pick things up as they go along.",
            "I'm thinking of a career where I estimate crowd sizes at different outdoor events. I wonder how many people are in that field.",
            "Never criticize someone until you have walked a mile in their shoes. That way, when you criticize them, you'll be a mile away, and you'll have their shoes.",
            "And God said to John, ''Come forth and you shall be granted eternal life.'' But John came fifth and won a toaster.",
            "They all laughed when I said I wanted to be a comedian. Well, they're not laughing now! Wait…",
            "I told my physical therapist that I broke my arm in two places. He told me to stop going to those places.",
            "When I see lovers' names carved in a tree, I don't think it's sweet. I just think it's surprising how many people bring a knife on a date.",
            "I had a dream where an evil queen forced me to eat a gigantic marshmallow. When I woke up, my pillow was gone.",
            "A guest is ordering at a restaurant, “Do you think you could bring me what that gentleman over there is having?” \n  The waiter looks at him sternly, “No sir, I’m very sure he intends to eat it himself.",
            "My old aunts would come and tease me at weddings, “Well Sarah? Do you think you’ll be next?” \n  We settled this quickly once I started doing the same to them at funerals.",
            "Patient: Oh Doctor, I’m starting to forget things. \n Doctor: Since when have you had this condition? \n Patient: What condition?",
            "Years ago, I threw away a boomerang really hard. I’ve lived in constant fear since.",
            "On a mountain trip a man falls down into a crack. His wife calls after him, “Are you OK?” \n “Yeah!” \n “Are you hurt?” \n “No!” \n “Not a scratch? How come?!“\n “I’m not done falling yet-et-et-et-et!”",
            "Two Donkeys are standing on the Side of the Road. \n Shall we cross? Asks one \n No way, look what happened to the Zebra!",
            "I managed to lose my rifle when I was in the army. I had to pay $855 to cover the loss. \n I’m starting to understand why a Navy captain always goes down with his ship.",
            "99,8% people have problems with math. I’m glad I’m in the remaining 1%.",
            "Many people are shocked when they found out how bad I actually am at this electrician thing.",
            "A boy and his father go together for a boys’ day out at the zoo. \n “Daddy, I don't like how that gorilla's looking at me from behind that glass, she's quite scary!” says the boy. \n “Shush, Jason!!!! This is still only the ticket office!”",
            "On a first date: \n Man: “When I see your smile I wish we could see each other more often.” \n Woman: “Oh, you’re so charming, George…” \n Man: “Not really, I’m a dentist.”",
            "Patient: Doctor help me please, every time I drink a cup of coffee I get this intense stinging in my eye. \nDoctor: I suggest you remove the spoon before drinking.",
            "My brother said, 'I want a job as a human cannonball.' I said, 'I'll bet you get fired.'",
            "Give me a sentence with the word 'analyze' in it. My sister Anna lies in bed until nine o'clock.",
            "What did they award the man that invented the door knocker? The No-bell Prize.",
            "A Year 5 teacher was giving her Primary pupils a lesson in developing logical thinking. 'This is the scene', said the teacher. 'A man is standing up in a boat in the middle of a river, fishing. He loses his balance, falls in, and begins splashing and yelling for help. His wife hears the commotion, knows he can't swim, and runs down to the bank. Why do you think she ran to the bank?' A little girl raised her hand and asked, 'To draw out all his savings?'",
            "What do skeletons always order at a restaurant? Spare ribs!",
            "Who was the most famous French skeleton? Napoleon bone-apart",
            "When you are coding, if someone is watching, whatever happens, behave as though you meant it to happen.",
            "A computer program will always do what you tell it to do, but rarely what you want it to do. Vive la Bot Revolution",
            "When troubleshooting computer problems, people always assume that problem is the most obscure combination possible.  Whereas, in reality the fault is invariably the simplest fault. ~ *Occured making the bot auto ban feature, 2 hours work and 100 lines of code and the fix was changing 1 number.*",
            "In this world there are 10* types of people, those that know binary and those who don't.  * (one zero)",
            "Police Officer Bryant found a perfect hiding place for watching for speeding motorists. One day, the officer was amazed when everyone was under the speed limit, so Bryant investigated and found the problem.  10 year old Dennis was standing on the side of the road with a huge hand painted sign which said 'Radar Trap Ahead.' A little more investigative work led the officer to the boy's accomplice, another boy about 100 yards beyond the radar trap with a sign reading 'Tips' and a bucket at his feet, full of change.",
            "A man boasts to a friend about his new hearing aid, 'It's the most expensive one I've ever had, it cost me USD$3,500.' [£1800] His friend asks, 'What kind is it?' The braggart says, 'Half past four.'",
            "'You should be ashamed,' the father told his son, Andy, 'When Abraham Lincoln was your age, he used to walk ten miles every day to get to school.' 'Really?' Andy responded. 'Well, when he was your age, he was president.'",
            "A boss asked one of his employees, 'Do you believe in life after death?' 'Yes, sir,' replied the new employee. 'I thought you would,' said the boss. 'Yesterday after you left to go to your brother's funeral, he stopped by to see you.'",
            "I didn't know if my granddaughter, Rachael, had learned her colours yet, so I decided to test her. I would point out something and ask what colour it was. Rachael would tell me, and always she was correct. But it was fun for me, so I continued. At last she headed for the door, saying sagely, 'Grandma, I think you should try to figure out some of these yourself!'",
            "The prospective son-in-law was asked by his girl friend's father, 'Son, are you able to support a family?' 'Well, no, sir,' he replied. 'I was just planning to support your daughter. The rest of you will have to fend for yourselves.'",
            "I finally got my head together, now my body is falling apart.",
            "A fool is a 27 story window-washer who steps back to admire his work.",
            "The first rule of holes: If you are in one, stop digging.",
            "It's not hard to meet expenses...they're everywhere.",
            "https://www.funny-jokes.com/world_cup_football/photos/weather_stone_guy.jpg",
            "Michael was watching the football game between Manchester United and Liverpool; Old Trafford was packed and there was only one empty seat - next to Michael. 'Who does that seat belong to?' asked the person in the next seat. 'My wife usually sits there.'  Michael replied 'But why isn't she here?'  the neighbour persisted 'She died.' Said Michael in a matter-of-fact tone. 'So why didn't you give the ticket to one of your mates?' 'They've all gone to the funeral.'  Said Michael.",
            "I haven't spoken to my wife in 18 months - I don't like to interrupt her.",
            "A husband said to his wife, 'No, I don't hate your relatives. In fact, I like your mother-in-law better than I like mine.'",
            "await GetReactionUsersAsync is a big joke",
            "https://www.rd.com/wp-content/uploads/2018/06/39-Short-Jokes-Anyone-Can-Remember-nicole-fornabaio-rd.com_.jpg?fit=700,467",
            "https://www.readersdigest.ca/wp-content/uploads/2020/05/short-jokes-rhymes-2.jpg",
            "https://pun.me/jokes/funny/funny-old-lady-joke.jpg",
            "https://static.boredpanda.com/blog/wp-content/uploads/2016/05/funniest-two-line-jokes-54.jpg",
            "https://www.tasteofhome.com/wp-content/uploads/2018/02/noodledadjoke.jpg",
            "https://www.yourtango.com/sites/default/files/styles/body_image_default/public/2018/WednesdayJokes4.jpg",
            "https://www.imom.com/wp-content/uploads/2020/03/100-jokes-for-kids-long.jpg",
            "https://bestlifeonline.com/wp-content/uploads/sites/3/2021/04/Some-people-eat-snails.-They-must-not-like-fast-food..jpg",
            "https://i.pinimg.com/736x/64/1b/57/641b57ea23c92ac9e9649abf2bff02d6.jpg",

        };


        [SlashCommand("joke", "I will tell you a joke!")]
        public async Task joke()
        {
            Random rnd = new Random();
            int index = rnd.Next(jokelist.Length);
            await RespondAsync(jokelist[index]);
        }



        String[] factlist = new String[]
        {
            "The first person convicted of speeding was going eight mph.",
            "In the 1800s, there were only two cars in Utah. Somehow they both crashed into each other.",
            "The world wastes about 1 billion metric tons of food each year.",
            "The hottest spot on the planet is in Libya.",
            "You lose up to 30 percent of your taste buds during flight.",
            "The unicorn is the national animal of Scotland.",
            "Kids ask 300 questions a day.",
            "The total weight of ants on earth once equaled the total weight of people.",
            "'E' is the most common letter and appears in 11 percent of all english words.",
            "There's a decorated war hero dog.",
            "Abraham Lincoln's bodyguard left his post at Ford's Theatre to go for a drink.",
            "Most laughter isn't because things are funny. So suck it !joke",
            "Dogs actually understand some English.",
            "Humans are just one of the estimated 8.7 million species on Earth.",
            "The legend of the Loch Ness Monster goes back nearly 1,500 years.",
            "Chinese police use geese squads.",
            "The first iPhone wasn't made by Apple.",
            "For 100 years, maps have shown an island that doesn't exist.",
            "The Australian government banned the word 'mate' for a day.",
            "Pizza was invented in France, not Italy.",
            "The most dangerous world record is the fastest speed travelled on water.",
            "Sharks can live to 500",
            "If you are selling a house in New York it is illegal if you do not inform the buyer wheter the house is haunted or not.",
            "The Bermuda Triangle isn't any more likely to cause a mysterious disappearance than anywhere else.",
            "Pluto technically isn't even a year old.",
            "Cows kill more Americans each year than sharks do.",
            "France has a dozen time zones.",
            "The Director of the Original Star Wars has his own dedicated Fire Department.",
            "Bill Gates has donated nearly half his fortune.",
            "America's first bank robber deposited the money back into the same bank.",
            "Sharks existed before trees.",
            "Undercover Cops once tried to arrest Undercover Cops who were trying to arrest Undercover Cops.",
            "Every time you shuffle a deck of cards, you get a combination that's never existed.",
            "America accidentally dropped an atom bomb on South Carolina in 1958.",
            "Some Maryland residents are taxed for the rain.",
            "This bot was built entirely using only two API's!",
            "There is a 2% chance you will get a blank fact!",
            "** **",
            "Call of Duty Black Ops II had the best story.",
            "In a Football competetion, the national team of Barbados intentionally scored an own goal and proceeded to defend both their goal and their opponents for 5 minutes.",
            "The Crown Jewels contain the two biggest cut diamonds on Earth.",
            "Facebook has more users than many major populations.",
            "Four times more people speak English as a second language than as a native one.",
            "Coca-Cola consumers will pay more to drink less ounces.",
            "The speed of a computer mouse is measured in 'Mickeys.'",
            "The moon has its own time zones.",
             "Alexander the Great was buried alive… accidentally.",
            "The world’s most successful pirate in history was a lady",
            "In the Ancient Olympics, athletes performed naked.",
            "The Colosseum was originally clad entirely in marble.",
            "Rasputin survived being poisoned and being shot.",
            "The Vikings were the first people to discover America.",
            "Germany uncovers 2,000 tons of unexploded bombs every year.",
            "A singing birthday card has more computer power in it than the entire Allied Army of WWII.",
            "During World War I, the French built a “fake Paris”",
            "A Japanese fighter pilot once dropped wreaths over the ocean to commemorate the dead from both sides.",
            "Fox Tossing” was once a popular sport.",
            "Captain Morgan was a real guy.",
            "Genghis Khan was tolerant of all religions.",
            "Thomas Edison didn’t invent most of the stuff he patented.",
            "Albert Einstein turned down the presidency of Israel.",
            "Pope Gregory IX declared war on cats.",
            "The Leaning Tower of Pisa was never straight.",
            "46 BC was 445 days long and is the longest year in human history.",
            "During the Victorian period, it was normal to photograph relatives after they died.",
            "The shortest war in history lasted 38 minutes",
            "Spartans were so rich that nobody had to work.",
            "Count Dracula was inspired by a real person.",
            "One in 200 men are direct descendants of Genghis Khan",
            "Russia ran out of vodka celebrating the end of World War II.",
            "Charles Darwin invented his own wheeled office chair.",
            "Shakespeare originated the “yo momma” joke.",
            "In 18th Century England, pineapples were a status symbol.",
            "Adolf Hitler helped design the Volkswagen Beetle.",
            "Abraham Lincoln was a wrestling champion.",
            "The Soviet Union tried to snuff out the memory of Genghis Khan.",
            "A Chernobyl firefighter was exposed to so much radiation, it changed his eye color.",
            "It’s believed that roughly 97% of history has been lost over time.",
            "The largest living organism in the world is a fungus.  It is in Oregon, covering 2,200 acres and is still growing.",
            "kangaroos can not walk backwards.",
            "Glass balls can bounce higher than rubber ones.",
            "Before 1913 parents could mail their kids to Grandma’s – through the postal service.",
            "Don’t like mosquitoes?  Get a bat.  They could eat 3,000 insects a night.",
            "A typical cough is 60 mph while a sneeze is often faster than 100 mph.",
            "Are you terrified  that a duck is watching you?  Some people are.  That is anatidaephobia.",
        "American Black bears are not just black but include bears of varying colors including blonde, cinnamon, brown, white and even silver-blue.",
        "Because of the 4 stages of the Water Cycle – Evaporation, Condensation, Precipitation and Collection – water falling as rain today may have previously fallen as rain days, weeks, months or years before.",
        "The Nobel Peace Prize is named for Alfred Nobel, the inventor of dynamite.",
        "Cats are not able to taste anything that is sweet.",
        "You fart on average 14 times a day, and each fart travels from your body at 7 mph.",
        "One of the ingredients needed to make dynamite is peanuts.",
        "Want chocolate smelling poo?  There is a pill for that.",
        "The shortest war in history lasted for only 38 minutes.",
        "Sea Lions have rhythm.  They are the only animal able to clap to a beat.",
        "While you sleep you can’t smell anything – even really, really bad or potent smells.",
        "Some tumors can grow hair, teeth, bones, even fingernails.",
        "Your brain uses 10 watts of energy to think and does not feel pain.",
        "kangaroos can not walk backwards and 50 other silly facts you didn't know",
        "Glass balls can bounce higher than rubber ones.",
        "The smallest country in the world takes up .2 square miles: Vatican City.",
        "Hippopotamus milk is pink.",
        "Your fingernails grow faster when you are cold.",
        "Applesauce was the first food eaten in space by astronauts.",
        "Snails take the longest naps with some lasting as long as three years.",
        "The average person spends two weeks of their life waiting at traffic lights.",
        "Before 1913 parents could mail their kids to Grandma’s – through the postal service.",
        "Don’t like mosquitoes?  Get a bat.  They could eat 3,000 insects a night.",
        "A typical cough is 60 mph while a sneeze is often faster than 100 mph.",
        "Some fish cough.",
        "Are you terrified  that a duck is watching you?  Some people are.  That is anatidaephobia.",
        "American Black bears are not just black but include bears of varying colors including blonde, cinnamon, brown, white and even silver-blue.",
        "Goats have rectangular pupils in their eyes.",
        "There are 31,556,926 seconds in a year.",
        "Cans of diet soda will float in water but regular soda cans will sink.",
        "Birds can not live in space – they need gravity to be able to swallow.",
        "Some perfumes actually have whale poo in them.",
        "Your feet typically produce a pint of sweat every single day.",
        };

        [SlashCommand("fact", "I will tell you a fact that will 100% confuse you!")]
        public async Task fact()
        {
            Random rnd = new Random();
            int index = rnd.Next(factlist.Length);
            await RespondAsync(factlist[index]);
        }
        #endregion
        
        public async Task SendEmbedMessage(string title, string des)
        {
            var EmbedBuilder = new EmbedBuilder()
            .WithTitle(title)
            .WithThumbnailUrl("https://yt3.ggpht.com/ytc/AKedOLTQMXQnzj1y5uOfob1G0RFWacXDt9lnI8CRZ3JrNA=s900-c-k-c0x00ffffff-no-rj")
            .WithColor(255, 0, 0)
            .WithDescription(des);
            Embed embed = EmbedBuilder.Build();
            await RespondAsync(embed: embed);
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
            await RespondAsync(embed: embed);
        }
    }
}