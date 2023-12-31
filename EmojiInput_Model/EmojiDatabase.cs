#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using EmojiInput_Utils;
using Newtonsoft.Json;

namespace EmojiInput_Model;

public record EmojiData(
    int Index,
    string EmojiCharacter,
    string ImageFilename,
    string Description,
    List<string> Aliases
);

internal class EmojiJson
{
    [JsonProperty("emoji")] public string Emoji { get; set; }
    [JsonProperty("description")] public string Description { get; set; }
    [JsonProperty("aliases")] public List<string> Aliases { get; set; }
}

public class EmojiDatabase : List<EmojiData>
{
    public EmojiDatabase(string jsonPath)
    {
        var emojis = JsonConvert.DeserializeObject<List<EmojiJson>>(File.ReadAllText(jsonPath));
        if (emojis == null) return;

        for (var index = 0; index < emojis.Count; index++)
        {
            var e = emojis[index];
            var s = UnicodeUtil.CharacterToUtf32(e.Emoji);
            this.Add(new EmojiData(
                index,
                e.Emoji,
                $"emoji_{s}.png",
                e.Description,
                e.Aliases));
        }
    }
}