#nullable enable

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace EmojiInput_Model;

internal class EmojiSkinJson
{
    internal class EmojisKeys
    {
        [JsonProperty("emojis")] public string Emojis { get; set; }
        [JsonProperty("key")] public string Key;
    }

    [JsonProperty("map")] public string Map { get; set; }
    [JsonProperty("skins")] public List<EmojisKeys> Skins { get; set; }
}

public class EmojiSkinData
{
    private readonly EmojiSkinJson _json;

    private readonly List<string> _keys;
    public IReadOnlyList<string> Keys => _keys;

    public EmojiSkinData(string jsonPath)
    {
        _json = JsonConvert.DeserializeObject<EmojiSkinJson>(File.ReadAllText(jsonPath))
                ?? throw new JsonException();

        _keys = _json.Map.Split(' ').ToList();
    }

    public string GetSkinEmoji(string aliasesKey, string skinKey)
    {
        int skinIndex = _keys.IndexOf(skinKey);
        if (skinIndex == -1) return "";
        var skins = _json.Skins.FirstOrDefault(s => s.Key == aliasesKey);
        if (skins == null) return "";
        return skins.Emojis.Split(' ')[skinIndex];
    }
}