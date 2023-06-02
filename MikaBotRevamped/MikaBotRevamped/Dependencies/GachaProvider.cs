using Discord;
using Discord.WebSocket;
using MikaBotRevamped.Handler;
using MikaBotRevamped.Items;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped.Dependencies
{
    internal class GachaProvider : IGachaProvider
    {
        private readonly Random rng;

        private const int MYTHIC_HARD_PITY = 100;

        public GachaProvider()
        {
            rng = new Random();
        }

        private UnclaimedWaifu Roll(IWaifuProvider waifuProvider, int pityCount)
        {
            WaifuQuality quality = GetRandomElementFromEnum<WaifuQuality>();

            UnclaimedWaifu waifu = new UnclaimedWaifu(waifuProvider.GetRandomWaifuImageUrl().Result)
            {
                Quality = quality
            };

            if (pityCount >= MYTHIC_HARD_PITY-1)
            {
                waifu.Quality = WaifuQuality.Mythic;
            }

            return waifu;
        }

        public Embed RollAndBuildEmbed(IWaifuProvider waifuProvider, IGameItemProvider gameItemProvider, SocketSlashCommand command)
        {
            var user = Program.bot.Users.Users[command.User.Id];
            var isNearPity = user.PityCount >= MYTHIC_HARD_PITY - 11 && user.PityCount < MYTHIC_HARD_PITY - 1;

            if (user.GetItemCount(new Roll()) < 1)
            {
                throw new Exception("You do not have enough Rolls!");
            }

            UnclaimedWaifu unclaimedWaifu = Roll(waifuProvider, user.PityCount);

            user.UnclaimedWaifus.Clear();
            user.UnclaimedWaifus.Add(unclaimedWaifu);
            user.PityCount = user.PityCount >= MYTHIC_HARD_PITY-1 ? 0 : user.PityCount + 1;
            user.RemoveItem(new Roll(), 1);

            if (user.RestFollowupMessage != null)
            {
                user.RestFollowupMessage.ModifyAsync(x =>
                {
                    x.Components = null;
                });
            }

            Program.bot.Users.Save(user.Uid);

            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder.Title = "Waifu rolled";
            embedBuilder.Description = $"{command.User.Mention} rolled a waifu!";
            embedBuilder.ImageUrl = unclaimedWaifu.ImageUrl;
            embedBuilder.AddField("Quality:", unclaimedWaifu.Quality);
            var color = WaifuQualityMethods.GetColor(unclaimedWaifu.Quality);
            embedBuilder.Color = new Color(color.R, color.G, color.B);

            if (isNearPity)
            {
                embedBuilder.WithFooter($"You are {MYTHIC_HARD_PITY - user.PityCount} roll{(MYTHIC_HARD_PITY - user.PityCount == 1 ? string.Empty : "s")} away from a guaranteed Mythic!");
            }

            return embedBuilder.Build();
        }

        private T GetRandomElementFromEnum<T>()
        {
            Array values = Enum.GetValues(typeof(T));

            int totalProbability = 0;
            foreach (var value in values)
            {
                var fieldInfo = typeof(T).GetField(value.ToString());
                var probabilityAttribute = fieldInfo.GetCustomAttributes(typeof(EnumProbabilityAttribute), false).FirstOrDefault() as EnumProbabilityAttribute;
                int probability = probabilityAttribute?.Probability ?? 1;
                totalProbability += probability;
            }

            int randomValue = rng.Next(totalProbability);

            int cumulativeProbability = 0;
            foreach (var value in values)
            {
                var fieldInfo = typeof(T).GetField(value.ToString());
                var probabilityAttribute = fieldInfo.GetCustomAttributes(typeof(EnumProbabilityAttribute), false).FirstOrDefault() as EnumProbabilityAttribute;
                int probability = probabilityAttribute?.Probability ?? 1;

                cumulativeProbability += probability;
                if (randomValue < cumulativeProbability)
                    return (T)value;
            }

            return default;
        }
    }
}
