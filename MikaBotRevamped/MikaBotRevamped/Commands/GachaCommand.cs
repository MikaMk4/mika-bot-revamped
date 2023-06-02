using Discord;
using Discord.WebSocket;
using MikaBotRevamped.Handler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped.Commands
{
    internal class GachaCommand : ISlashCommand
    {
        public string CommandName => "gacha";

        public bool AsyncMode => false;
        private IWaifuProvider waifuProvider;
        private IGachaProvider gachaProvider;
        private IGameItemProvider gameItemProvider;

        public async Task Respond(SocketSlashCommand command)
        {
            var subCommand = command.Data.Options.First().Name;

            if (subCommand == "roll")
            {
                var preText = string.Empty;

                if (!Program.bot.Users.Users.ContainsKey(command.User.Id))
                {
                    preText += "Welcome! I see you just rolled your first Waifu. I'll register you real quick and give you 20 Rolls for the beginning!";
                    Program.bot.RegisterUser(command.User.Id);
                }

                Embed embed;

                try
                {
                    embed = gachaProvider.RollAndBuildEmbed(waifuProvider, gameItemProvider, command);
                } catch (Exception e)
                {
                    await command.RespondAsync(e.Message, ephemeral:true);
                    return;
                }

                var buttonBuilder = new ButtonBuilder();
                buttonBuilder.WithCustomId("give-waifu-name-button");
                buttonBuilder.WithLabel("Name your Waifu!");
                buttonBuilder.WithStyle(ButtonStyle.Primary);
                ComponentBuilder componentBuilder = new ComponentBuilder();
                componentBuilder.WithButton(buttonBuilder);

                await command.DeferAsync();
                Program.bot.Users.Users[command.User.Id].RestFollowupMessage = await command.FollowupAsync(preText, embed: embed, components: componentBuilder.Build());
            }
            else if (subCommand == "qualities")
            {
                List<Embed> embeds = new List<Embed>();

                foreach (var (value, chance) in WaifuQualityMethods.GetEnumValueProbabilities())
                {
                    EmbedBuilder embedBuilder = new EmbedBuilder();
                    embedBuilder.Title = value.ToString();
                    embedBuilder.Description = $"Chance for a Waifu of quality {value} to drop is:";
                    embedBuilder.AddField("Chance:", $"{chance / 100:#0.00}%", inline:true);
                    var color = WaifuQualityMethods.GetColor(value);
                    embedBuilder.Color = new Color(color.R, color.G, color.B);
                    embeds.Add(embedBuilder.Build());
                }

                await command.RespondAsync(embeds: embeds.ToArray());
            } else if (subCommand == "list")
            {
                var user = command.User;
                int page = 1;

                if (command.Data.Options.First().Options.Any())
                {
                    foreach (var param in command.Data.Options.First().Options)
                    {
                        if (param.Name == "user")
                        {
                            user = param.Value as SocketUser;
                        } else if (param.Name == "page")
                        {
                            var _page = param.Value;

                            if (_page != null)
                            {
                                page = Convert.ToInt32(_page);
                            }
                            else
                            {
                                page = 1;
                            }
                        }
                    }
                }

                if (page <= 0)
                {
                    page = 1;
                }

                if (user == null)
                {
                    await command.RespondAsync("Oops, something went wrong. (user == null)");
                    Program.Log(LogSeverity.Error, "GachaCommand", "user was null");
                    return;
                }

                if (!Program.bot.Users.Users.TryGetValue(user.Id, out _))
                {
                    await command.RespondAsync($"{user.Username} doesn't have any Waifus yet!");
                    return;
                }

                EmbedBuilder embedBuilder = new EmbedBuilder();
                embedBuilder.Title = $"Waifus (Page {page})";
                embedBuilder.Description = $"{user.Mention}'s Waifus:";
                embedBuilder.Color = new Color(255, 0, 0);

                var waifus = Program.bot.Users.Users[user.Id].Waifus;
                var waifusPerPage = 10;
                var tenWaifus = GetPageElements(waifus, page, waifusPerPage);

                tenWaifus.ForEach(waifu => embedBuilder.AddField(waifu.Name, $"{waifu.Quality} | #{waifu.Id}"));

                command.RespondAsync(embed: embedBuilder.Build());
            } else if (subCommand == "view")
            {
                var waifuIdLong = command.Data.Options.First().Options.First().Value as long?;
                var waifuId = (int)waifuIdLong;

                if (waifuId == null)
                {
                    command.RespondAsync("Oops, something went wrong. (waifuId == null)");
                    Program.Log(LogSeverity.Error, "GachaCommand", "waifuId was null");
                    return;
                }

                var (user, waifu) = await Program.bot.SearchUserAndWaifuByWaifuId((ulong?)waifuId ?? 0);

                if (user == null || waifu == null)
                {
                    command.RespondAsync("Oops, something went wrong. (user == null || waifu == null)");
                    Program.Log(LogSeverity.Error, "GachaCommand", "user or waifu was null");
                    return;
                }

                EmbedBuilder embedBuilder = new EmbedBuilder();
                embedBuilder.Title = waifu.Name;
                embedBuilder.Description = $"<@!{user.Uid}>'s Waifu:";
                var color = WaifuQualityMethods.GetColor(waifu.Quality);
                embedBuilder.Color = new Color(color.R, color.G, color.B);
                embedBuilder.AddField("Quality:", waifu.Quality);
                embedBuilder.AddField("Id:", waifu.Id);
                embedBuilder.ImageUrl = waifu.ImageUrl;

                command.RespondAsync(embed: embedBuilder.Build());
            } else if (subCommand == "pity")
            {
                var user = command.User as SocketUser;

                if (!Program.bot.Users.Users.TryGetValue(user.Id, out _))
                {
                    await command.RespondAsync($"{user.Username} didn't play the gacha yet.");
                    return;
                }

                var pity = Program.bot.Users.Users[user.Id].PityCount;

                command.RespondAsync($"Your Pity Count: {pity}");
            }
        }

        private List<T> GetPageElements<T>(List<T> list, int page, int elementsPerPage)
        {
            int startIndex = (page - 1) * elementsPerPage;
            int endIndex = Math.Min(startIndex + elementsPerPage, list.Count);

            if (startIndex >= list.Count)
            {
                // If the start index is beyond the list length, return an empty list
                return new List<T>();
            }
            else
            {
                return list.GetRange(startIndex, endIndex - startIndex);
            }
        }

        public void SetDependencies(IDependencyProvider dependencyProvider)
        {
            waifuProvider = dependencyProvider.GetDependency<IWaifuProvider>();
            gachaProvider = dependencyProvider.GetDependency<IGachaProvider>();
            gameItemProvider = dependencyProvider.GetDependency<IGameItemProvider>();
        }

        public SlashCommandProperties GetCommandProperties()
        {
            return new SlashCommandBuilder()
                .WithName(CommandName)
                .WithDescription("Gacha commands")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("roll")
                    .WithDescription("Roll 1 time.")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                )
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("qualities")
                    .WithDescription("Display the chances for qualities.")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                )
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("list")
                    .WithDescription("List all your Waifus.")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("user")
                        .WithDescription("The user to show the list of")
                        .WithType(ApplicationCommandOptionType.User)
                        .WithRequired(false)
                    )
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("page")
                        .WithDescription("The page to show")
                        .WithType(ApplicationCommandOptionType.Integer)
                        .WithRequired(false)
                    )
                )
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("view")
                    .WithDescription("View a Waifu.")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("id")
                        .WithDescription("The ID of the Waifu.")
                        .WithType(ApplicationCommandOptionType.Integer)
                        .WithRequired(true)
                    )
                )
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("pity")
                    .WithDescription("View your pity.")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                )
                .Build();
        }
    }
}
