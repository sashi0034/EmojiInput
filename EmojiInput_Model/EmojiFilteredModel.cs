#nullable enable

using System.Collections.Generic;
using System.Linq;

namespace EmojiInput_Model;

public class EmojiFilteredModel
{
    private List<EmojiData> _list = new();
    public IReadOnlyList<EmojiData> List => _list;

    public void FilterRefresh(EmojiDatabase database, string searchText)
    {
        var filtered = database
            .Where(emoji =>
                emoji.Description.Contains(searchText) || emoji.Aliases.Any(a => a.Contains(searchText)))
            .ToList();
        _list = filtered;
    }

    public void Refresh(List<EmojiData> data)
    {
        _list = data;
    }
}