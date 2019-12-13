using DiscordBot.Models;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

/* Create our class and extend from IModule */
public class BasicCommandsModule : IModule
{
    /* Commands in DSharpPlus.CommandsNext are identified by supplying a Command attribute to a method in any class you've loaded into it. */
    /* The description is just a string supplied when you use the help command included in CommandsNext. */
    [Command("alive")]
    [Description("Simple command to test if the bot is running!")]
    public async Task Alive(CommandContext ctx)
    {
        /* Trigger the Typing... in discord */
        await ctx.TriggerTypingAsync();
        System.Console.WriteLine("It works");
        /* Send the message "I'm Alive!" to the channel the message was recieved from */
        await ctx.RespondAsync("I'm alive!");
    }
    [Command("interact")]
    [Description("Simple command to test interaction!")]
    public async Task Interact(CommandContext ctx)
    {
        /* Trigger the Typing... in discord */
        await ctx.TriggerTypingAsync();

        /* Send the message "I'm Alive!" to the channel the message was recieved from */
        await ctx.RespondAsync("How are you today?");

        var intr = ctx.Client.GetInteractivityModule(); // Grab the interactivity module
        var reminderContent = await intr.WaitForMessageAsync(
            c => c.Author.Id == ctx.Message.Author.Id, // Make sure the response is from the same person who sent the command
            TimeSpan.FromSeconds(10) // Wait 60 seconds for a response instead of the default 30 we set earlier!
        );
        // You can also check for a specific message by doing something like
        // c => c.Content == "something"

        // Null if the user didn't respond before the timeout
        if (reminderContent == null)
        {
            await ctx.RespondAsync("Sorry, I didn't get a response!");
            return;
        }

        // Homework: have this change depending on if they say "good" or "bad", etc.
        if (reminderContent.Message.Content.ToLower() == "good")
        {
            await ctx.RespondAsync("Thats good to hear!");

        }
        else if (reminderContent.Message.Content.ToLower() == "bad")
        {

            await ctx.RespondAsync("Do you want to talk about it?");
            if (reminderContent.Message.Content.ToLower() == "yes")
            {
                await ctx.RespondAsync("Oh... I hope things are cool but... I'm busy");
            }
            else if (reminderContent.Message.Content.ToLower() == "no")
            {
                await ctx.RespondAsync("Aite have a good day fam");
            }
        }

    }
    [Command("waitforcode"), Description("Waits for a response containing a generated code.")]
    public async Task WaitForCode(CommandContext ctx)
    {
        // first retrieve the interactivity module from the client
        var interactivity = ctx.Client.GetInteractivityModule();

        // generate a code
        var codebytes = new byte[8];
        using (var rng = RandomNumberGenerator.Create())
            rng.GetBytes(codebytes);

        var code = BitConverter.ToString(codebytes).ToLower().Replace("-", "");

        // announce the code
        await ctx.RespondAsync($"The first one to type the following code gets a reward: `{code}`");

        // wait for anyone who types it
        var msg = await interactivity.WaitForMessageAsync(xm => xm.Content.Contains(code), TimeSpan.FromSeconds(60));
        if (msg != null)
        {
            // announce the winner
            await ctx.RespondAsync($"And the winner is: {msg.Message.Author.Mention}");
        }
        else
        {
            await ctx.RespondAsync("Nobody? Really?");
        }
    }
    [Command("poll"), Description("Run a poll with reactions.")]
    public async Task Poll(CommandContext ctx, [Description("How long should the poll last.")] TimeSpan duration, [Description("What options should people have.")] params DiscordEmoji[] options)
    {
        // first retrieve the interactivity module from the client
        var interactivity = ctx.Client.GetInteractivityModule();
        var poll_options = options.Select(xe => xe.ToString());

        // then let's present the poll
        var embed = new DiscordEmbedBuilder
        {
            Title = "Poll time!",
            Description = string.Join(" ", poll_options)
        };
        var msg = await ctx.RespondAsync(embed: embed);

        // add the options as reactions
        for (var i = 0; i < options.Length; i++)
            await msg.CreateReactionAsync(options[i]);

        // collect and filter responses
        var poll_result = await interactivity.CollectReactionsAsync(msg, duration);
        var results = poll_result.Reactions.Where(xkvp => options.Contains(xkvp.Key))
            .Select(xkvp => $"{xkvp.Key}: {xkvp.Value}");

        // and finally post the results
        await ctx.RespondAsync(string.Join("\n", results));
    }
    [Command("quote"), Description("gives a random quote")]


    public async Task Quote(CommandContext ctx)
    {
        /* Trigger the Typing... in discord */
        await ctx.TriggerTypingAsync();
        Random rand;
        string[] quotes;
        quotes = new string[]
        {
            "The best thing about a boolean is even if you are wrong, you are only off by a bit.",
            "Without requirements or design, programming is the art of adding bugs to an empty text file. ",
            "Before software can be reusable it first has to be usable.",
            "The best method for accelerating a computer is the one that boosts it by 9.8 m/s2.",
            "I think Microsoft named .Net so it wouldn’t show up in a Unix directory listing.",
            "If builders built buildings the way programmers wrote programs, then the first woodpecker that came along would destroy civilization.",
            "There are two ways to write error-free programs; only the third one works.",
            "Ready, fire, aim: the fast approach to software development. Ready, aim, aim, aim, aim: the slow approach to software development. ",
            "It’s not a bug – it’s an undocumented feature. ",
            "One man’s crappy software is another man’s full-time job.",
            "A good programmer is someone who always looks both ways before crossing a one-way street. ",
            "Always code as if the guy who ends up maintaining your code will be a violent psychopath who knows where you live. ",
        };
        rand = new Random();
        int randomQuote = rand.Next(quotes.Length);
        string quoteToPost = quotes[randomQuote];
        await ctx.RespondAsync(quoteToPost);
    }
    [Command("roshambo"), Description("Rock Paper Scissor")]
    public async Task Roshambo(CommandContext ctx, DiscordMember a, DiscordMember b)
    {
        System.Console.WriteLine("you made it here");
        var interactivity = ctx.Client.GetInteractivityModule();
        var members = new Dictionary<DiscordMember, string>();
        members.Add(a, "test");
        members.Add(b, "test");


        var results = await Task.WhenAll(members.Select(async member =>
        {
            System.Console.WriteLine("messaged");
            var message = await member.Key
            .SendMessageAsync($"{ctx.Member.Username} has declared a Rock Paper Scissors match on you. Please select your choice. `rock`, `paper`, or `scissors`");
            return interactivity
            .WaitForMessageAsync(x => x.Author.Id == member.Key.Id
            && (x.Content.ToLower().Equals("rock") || x.Content.ToLower().Equals("scissors") || x.Content.ToLower().Equals("paper")));
        }));

        var builder = new StringBuilder();
        List<MessageContext> players = new List<MessageContext>();
        foreach (var context in results.Select(task => task.Result))
        {
            builder.AppendFormat("```{0} played {1}\n```", context.User.Username, context.Message.Content);
            System.Console.WriteLine(context.Message.Content);

            players.Add(context);
        }
        await ctx.RespondAsync(builder.ToString());

        if (players[0].Message.Content.ToLower() == "rock" && players[1].Message.Content.ToLower() ==
        "rock" || players[0].Message.Content.ToLower() == "scissors" && players[1].Message.Content.ToLower() ==
        "scissors" || players[0].Message.Content.ToLower() == "paper" && players[1].Message.Content.ToLower() ==
        "paper")
        {
            await ctx.RespondAsync("```Tie```");
        }
        else if (players[0].Message.Content.ToLower() == "rock" && players[1].Message.Content.ToLower() ==
        "scissors" || players[0].Message.Content.ToLower() == "scissors" && players[1].Message.Content.ToLower() ==
        "paper" || players[0].Message.Content.ToLower() == "paper" && players[1].Message.Content.ToLower() ==
        "rock")
        {
            await ctx.RespondAsync($"```{players[0].User.Username} won!```");
        }
        else
        {
            await ctx.RespondAsync($"```{players[1].User.Username} won!```");

        }
    }



    [Command("dojodachi"), Description("lets you play dojodachi on discord")]
    public async Task Dojodachi(CommandContext ctx)
    {
        // first retrieve the interactivity module from the client
        var interactivity = ctx.Client.GetInteractivityModule();

        DojoDachi myDachi = new DojoDachi();
        await ctx.RespondAsync("Your DojoDachi is Created!");

        var dachiCommand = await interactivity.WaitForMessageAsync(
            c => c.Author.Id == ctx.Message.Author.Id &&
            c.Content.Equals("sleep") ||
            c.Content.Equals("feed") ||
            c.Content.Equals("play") ||
            c.Content.Equals("work"), // Make sure the response is from the same person who sent the command
            TimeSpan.FromSeconds(10) // Wait 60 seconds for a response instead of the default 30 we set earlier!
        );
        // Null if the user didn't respond before the timeout
        if (dachiCommand == null)
        {
            await ctx.RespondAsync("Sorry, I didn't get a response goodbye!");
            return;
        }


    }
}