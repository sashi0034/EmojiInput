﻿#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EmojiInput_Utils;
using Newtonsoft.Json;

namespace EmojiInput_Model;

public enum EmojiIconSizeKind
{
    Small,
    Large,
}

public class EmojiHistoryArray
{
    public const int Size = 30;
    private readonly List<string> _array;
    public IReadOnlyList<string> Array => _array;

    public EmojiHistoryArray() : this(new List<string>())
    {
    }

    public EmojiHistoryArray(List<string> array)
    {
        _array = array;
        while (_array.Count < Size) _array.Add("");
    }

    public void RenewData(string data)
    {
        int index = _array.IndexOf(data);
        if (index == -1)
        {
            _array.RemoveAt(_array.Count - 1);
        }
        else
        {
            _array.RemoveAt(index);
        }

        _array.Insert(0, data);
    }
}

public class EmojiSettingPrimitiveData
{
    [JsonProperty("installed_path")] public string InstalledPath = "";
    [JsonProperty("skin_key")] public string SkinKey = "";
    [JsonProperty("icon_size")] public EmojiIconSizeKind IconSize = EmojiIconSizeKind.Small;
    [JsonProperty("history")] private List<string>? _history = new();

    [JsonIgnore] public EmojiHistoryArray History => new EmojiHistoryArray(_history ??= new List<string>());

    public EmojiSettingPrimitiveData Clone()
    {
        return new EmojiSettingPrimitiveData()
        {
            InstalledPath = InstalledPath,
            SkinKey = SkinKey,
            IconSize = IconSize,
            _history = History.Array.Clone()
        };
    }

    public override bool Equals(object? obj)
    {
        return obj is EmojiSettingPrimitiveData data
               && this.InstalledPath == data.InstalledPath
               && this.SkinKey == data.SkinKey
               && this.IconSize == data.IconSize
               && this.History.Array.SequenceEqual(data.History.Array);
    }

    public override int GetHashCode() => HashCode.Combine(this);

    public static bool operator ==(EmojiSettingPrimitiveData lhs, EmojiSettingPrimitiveData? rhs) => lhs.Equals(rhs);

    public static bool operator !=(EmojiSettingPrimitiveData lhs, EmojiSettingPrimitiveData? rhs) => !(lhs == rhs);
}

public class EmojiSettingModel
{
    public const string DefaultFilepath = "setting.json";

    private EmojiSettingPrimitiveData _data = new();
    private EmojiSettingPrimitiveData _lastSavedData = new();

    public EmojiSettingPrimitiveData Data => _data;

    public void RefreshSave(string filepath = DefaultFilepath)
    {
        if (_data == _lastSavedData) return;
        try
        {
            using var sw = new StreamWriter(filepath, false, Encoding.UTF8);
            sw.Write(JsonConvert.SerializeObject(_data, Formatting.Indented));
            _lastSavedData = _data.Clone();
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
            _data = JsonConvert.DeserializeObject<EmojiSettingPrimitiveData>(sr.ReadToEnd())
                    ?? new EmojiSettingPrimitiveData();
            _lastSavedData = _data.Clone();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}