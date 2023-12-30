using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EmojiInput.Main.Detail;

public partial class IconCollection : UserControl
{
    private const int columnSize = 10;
    private readonly List<Grid> girdList = new();
    private Grid tailGrid => girdList.Last();

    public IconCollection()
    {
        InitializeComponent();
        createNewGrid(stackPanel, girdList);
    }

    public void Add(UIElement uiElement)
    {
        if (tailGrid.Children.Count >= columnSize)
        {
            // 行が足りないから増やす
            createNewGrid(stackPanel, girdList);
        }

        // 末尾の行の後ろの列に追加
        Grid.SetColumn(uiElement, tailGrid.Children.Count % columnSize);
        tailGrid.Children.Add(uiElement);
    }

    private static void createNewGrid(StackPanel parent, List<Grid> cachedList)
    {
        var grid = new Grid();
        for (int i = 0; i < columnSize; i++)
        {
            var column = new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            };
            grid.ColumnDefinitions.Add(column);
        }

        parent.Children.Add(grid);
        cachedList.Add(grid);
    }
}