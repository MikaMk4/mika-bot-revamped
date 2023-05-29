using Discord;
using Discord.WebSocket;
using MikaBotRevamped.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped.Commands
{
    internal class WaifuCommand : ISlashCommand
    {
        public string CommandName => "waifu";

        public bool AsyncMode => false;

        private IWaifuProvider waifuProvider;

        public async Task Respond(SocketSlashCommand command)
        {
            var subCommand = command.Data.Options.First().Name;

            if (subCommand == "image")
            {
                var subSubCommand = command.Data.Options.First().Options.First().Name;
                if (subSubCommand == "random")
                {
                    EmbedBuilder embedBuilder = new EmbedBuilder();
                    embedBuilder.Title = "Here, have a waifu.";
                    embedBuilder.ImageUrl = await waifuProvider.GetRandomWaifuImageUrl();
                    embedBuilder.Color = Color.Green;
                    await command.RespondAsync(embed: embedBuilder.Build());
                }
                else if (subSubCommand == "tag")
                {
                    var tag = command.Data.Options.First().Options.First().Options.First().Value as string;
                    if (tag == null)
                    {
                        await command.RespondAsync("Oops, something went wrong. (tag)");
                        await Program.Log(LogSeverity.Error, "WaifuCommand", "tag is null");
                        return;
                    }
                    EmbedBuilder embedBuilder = new EmbedBuilder();
                    embedBuilder.Title = "Here, have a waifu.";
                    embedBuilder.ImageUrl = await waifuProvider.GetWaifuImageUrlByTag(tag);
                    embedBuilder.Color = Color.Green;
                    await command.RespondAsync(embed: embedBuilder.Build());
                }
            }
        }

        public void SetDependencies(IDependencyProvider dependencyProvider)
        {
            waifuProvider = dependencyProvider.GetDependency<IWaifuProvider>();
        }

        public SlashCommandProperties GetCommandProperties()
        {
            return new SlashCommandBuilder()
                .WithName(CommandName)
                .WithDescription("Waifu commands")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("image")
                    .WithDescription("Get a waifu image")
                    .WithType(ApplicationCommandOptionType.SubCommandGroup)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("random")
                        .WithDescription("Get a random waifu image")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                    )
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("tag")
                        .WithDescription("Get a waifu image by tag")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                        .AddOption(new SlashCommandOptionBuilder()
                            .WithName("tag")
                            .WithDescription("The tag from which to get a waifu")
                            .WithType(ApplicationCommandOptionType.String)
                            .WithRequired(true)
                            .AddChoice("waifu", "waifu")
                            .AddChoice("neko", "neko")
                            .AddChoice("shinobu", "shinobu")
                            .AddChoice("megumin", "megumin")
                            .AddChoice("bully", "bully")
                            .AddChoice("cuddle", "cuddle")
                            .AddChoice("cry", "cry")
                            .AddChoice("kiss", "kiss")
                            .AddChoice("lick", "lick")
                            .AddChoice("pat", "pat")
                            .AddChoice("smug", "smug")
                            .AddChoice("bonk", "bonk")
                            .AddChoice("yeet", "yeet")
                            .AddChoice("blush", "blush")
                            .AddChoice("smile", "smile")
                            .AddChoice("wave", "wave")
                            .AddChoice("handhold", "handhold")
                            .AddChoice("nom", "nom")
                            .AddChoice("bite", "bite")
                            .AddChoice("slap", "slap")
                            .AddChoice("happy", "happy")
                            .AddChoice("wink", "wink")
                            .AddChoice("poke", "poke")
                            .AddChoice("dance", "dance")
                            .AddChoice("cringe", "cringe")
                        )
                    )
                )
                .Build();
        }
    }
}
