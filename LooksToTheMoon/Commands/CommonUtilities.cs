using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace LooksToTheMoon.Commands {
    
    public class CommonUtilities : ModuleBase {
        
        public static string AvatarURL = Core.Client.CurrentUser.GetAvatarUrl();
        public static Color EmbedColour = new Color(44, 192, 212);


        [Command("you good?")]
        [Alias("how you doin", "what's your status", "whats your status")]
        public async Task Ping() {

            string[] replies = {"I'm doing good, what about you?", "Doing a-okay!", "Ready and operational!", "Couldn't be better!"};
            
            await ReplyAsync(replies[new Random().Next(0, replies.Length)]);
            TimeSpan uptimeTS = DateTime.Now - Core.Startup;

            EmbedBuilder embedBuilder = new EmbedBuilder()
                .WithAuthor("Looks to the Moon Status and Uptime", AvatarURL)
                .WithDescription($"**Latency:** **{Core.Client.Latency}** ms\n**Uptime:** {uptimeTS.FormatTimeSpan()}")
                .WithColor(EmbedColour)
                .WithFooter($"Command issued by {Context.User.Username}#{Context.User.DiscriminatorValue}", Context.User.GetAvatarUrl());
            
            await ReplyAsync(embed: embedBuilder.Build());
            
        }
        
    }
}