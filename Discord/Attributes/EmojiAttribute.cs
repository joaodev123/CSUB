
using System;
using DSharpPlus.Entities;

namespace Discord.Attributes
{
    public class EmojiAttribute : Attribute
    {
        public DiscordEmoji Emoji;

        public EmojiAttribute(DiscordEmoji emoji)
        {
            this.Emoji = emoji;
        }

        public EmojiAttribute(string unicode)
        {
            var client = Bot.Instance().client;
            this.Emoji = DiscordEmoji.FromUnicode(Bot.Instance().client, unicode) ?? DiscordEmoji.FromName(Bot.Instance().client, unicode);
        }

        public EmojiAttribute(ulong id)
        {
            this.Emoji = DiscordEmoji.FromGuildEmote(Bot.Instance().client, id);
        }
    }
}
