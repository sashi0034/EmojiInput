#nullable enable

using System.Collections.Generic;
using EmojiInput_Model;

namespace EmojiInput.Main.Forward;

public class EmojiFilteredList
{
    private List<EmojiData> _list = new();
    public IReadOnlyList<EmojiData> List => _list;

    public void Refresh(List<EmojiData> data)
    {
        _list = data;
    }
}