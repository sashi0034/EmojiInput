#nullable enable

using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace EmojiInput_Model;

public enum EmojiIconSizeKind
{
    Small,
    Large,
}

public struct EmojiSettingPrimitiveData
{
    [JsonProperty("skin_key")] public string SkinKey;
    [JsonProperty("icon_size")] public EmojiIconSizeKind IconSize;

    public EmojiSettingPrimitiveData()
    {
        SkinKey = "";
        IconSize = EmojiIconSizeKind.Small;
    }

    [JsonIgnore] public (string, EmojiIconSizeKind) AsTuple => (SkinKey, IconSize);
}

public class EmojiSettingModel
{
    public const string DefaultFilepath = "config.json";

    private EmojiSettingPrimitiveData _data = new();
    private EmojiSettingPrimitiveData _lastSavedData = new();

    public string SkinKey
    {
        get => _data.SkinKey;
        set => _data.SkinKey = value;
    }

    public EmojiIconSizeKind IconSize
    {
        get => _data.IconSize;
        set => _data.IconSize = value;
    }

    public void RefreshSave(string filepath = DefaultFilepath)
    {
        if (_data.AsTuple == _lastSavedData.AsTuple) return;
        try
        {
            using var sw = new StreamWriter(filepath, false, Encoding.UTF8);
            sw.Write(JsonConvert.SerializeObject(_data));
            _lastSavedData = _data;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
            throw;
        }
    }

    public void Load(string filepath = DefaultFilepath)
    {
        try
        {
            using var sr = new StreamReader(filepath, Encoding.UTF8);
            _data = JsonConvert.DeserializeObject<EmojiSettingPrimitiveData>(sr.ReadToEnd());
            _lastSavedData = _data;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}