#nullable enable

using System.Collections.Generic;
using System.Linq;

namespace EmojiInput_Model;

public class EmojiFilteredModel
{
    private List<EmojiData> _list = new();
    public IReadOnlyList<EmojiData> List => _list;

    private string _lastSearched = "";

    public void FilterRefresh(EmojiDatabase database, string searchText)
    {
        var source = searchText.Contains(_lastSearched) ? _list : database;
        _lastSearched = searchText;

        var filtered = source
            .Where(emoji =>
                emoji.Description.Contains(searchText) || emoji.Aliases.Any(a => a.Contains(searchText)))
            .ToList();

        _list = filtered;
    }

    public void Refresh(List<EmojiData> data)
    {
        _lastSearched = "";
        _list = data;
    }
}