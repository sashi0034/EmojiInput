#nullable enable

using System.Collections.Generic;

namespace EmojiInput_Model;

public class EmojiFilteredList
{
    private List<EmojiData> _list = new();
    public IReadOnlyList<EmojiData> List => _list;

    public void Refresh(List<EmojiData> data)
    {
        _list = data;
    }
}