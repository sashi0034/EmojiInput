using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace EmojiInput.Main.Detail;

public partial class IconCollection : UserControl
{
    private const int columnSize = 10;

    private readonly List<Grid> _girdList = new();
    private Grid tailGrid => _girdList.Last();

    private readonly List<Image> _reservedImaged = new();

    public IconCollection()
    {
        InitializeComponent();
        createNewGrid(stackPanel, _girdList);
    }

    public void Reserve(int length)
    {
        while (_reservedImaged.Count < length)
        {
            var newImage = new Image()
            {
                Width = 64
            };
            addElement(newImage);
        }
    }

    public void Resize(int length)
    {
        throw new NotImplementedException();
    }

    public void ChangeSource(int index, BitmapImage image)
    {
        _reservedImaged[index].Source = image;
    }

    private void addElement(Image uiElement)
    {
        if (tailGrid.Children.Count >= columnSize)
        {
            // 行が足りないから増やす
            createNewGrid(stackPanel, _girdList);
        }

        // 末尾の行の後ろの列に追加
        Grid.SetColumn(uiElement, tailGrid.Children.Count % columnSize);
        tailGrid.Children.Add(uiElement);

        // キャッシュにも格納
        _reservedImaged.Add(uiElement);
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