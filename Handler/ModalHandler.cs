using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped.Handler
{
    internal class ModalHandler
    {
        public async Task HandleModal(SocketModal modal)
        {
            var modalId = modal.Data.CustomId;

            if (modalId == "waifu-name-input")
            {
                var waifuName = modal.Data.Components.First().Value;

                var unclaimedWaifu = Program.bot.Users.Users[modal.User.Id].UnclaimedWaifus.First();
                Program.bot.Users.Users[modal.User.Id].UnclaimedWaifus.RemoveAt(0);

                Waifu waifu = new Waifu(unclaimedWaifu, waifuName);
                Program.bot.Users.Users[modal.User.Id].Waifus.Add(waifu);
                Program.bot.Users.Save(modal.User.Id);

                Program.bot.Users.Users[modal.User.Id].RestFollowupMessage.ModifyAsync(x =>
                {
                    var embed = Program.bot.Users.Users[modal.User.Id].RestFollowupMessage.Embeds.First();
                    var fields = embed.Fields.ToList();
                    fields.Insert(0, new EmbedFieldBuilder()
                        .WithName(waifuName)
                        .WithValue("#" + waifu.Id)
                        .Build());

                    EmbedBuilder embedBuilder = new EmbedBuilder();
                    embedBuilder.Title = embed.Title;
                    embedBuilder.Description = $"{modal.User.Mention} named this Waifu:";
                    embedBuilder.ImageUrl = embed.Image?.Url;
                    embedBuilder.Color = embed.Color;
                    embedBuilder.Footer = new EmbedFooterBuilder()
                    {
                        Text = embed.Footer?.Text
                    };

                    foreach (var field in fields)
                    {
                        embedBuilder.AddField(new EmbedFieldBuilder()
                            .WithName(field.Name)
                            .WithValue(field.Value)
                            .WithIsInline(field.Inline)
                        );
                    }

                    x.Embed = embedBuilder.Build();
                    x.Components = null;
                });

                await modal.RespondAsync($"{modal.User.Mention}, your Waifu's name is {waifuName}!");
            }
            else
            {
                await modal.RespondAsync("Oops, something went wrong. (modalId == null)");
                await Program.Log(LogSeverity.Error, "ModalHandler", "Modal not found: " + modalId);
                return;
            }
        }
    }
}
