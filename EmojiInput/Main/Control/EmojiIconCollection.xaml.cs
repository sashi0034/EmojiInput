using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EmojiInput.Main.Control;

public partial class EmojiIconCollection : UserControl
{
    public const int ColumnSize = 20;
    public const int ImageSize = 32;

    private readonly List<Image> _reservedImages = new();
    public IReadOnlyList<Image> ReservedImages => _reservedImages;

    public int CurrentSize { get; private set; }

    public int Cursor { get; private set; }

    public EmojiIconCollection()
    {
        InitializeComponent();

        for (int i = 0; i < ColumnSize; ++i)
        {
            rootGrid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });
        }

        pushGridRow(rootGrid);
        focusCursor(false);

        cursorBorder.Width = ImageSize;
        cursorBorder.Height = ImageSize;
    }

    public void Reserve(int length)
    {
        while (_reservedImages.Count < length)
        {
            var newImage = new Image()
            {
                Width = ImageSize,
                Height = ImageSize,
            };
            appendElement(newImage);
        }
    }

    public void Resize(int size)
    {
        for (int i = 0; i < size; ++i)
        {
            _reservedImages[i].Visibility = Visibility.Hidden;
        }

        for (int i = size; i < _reservedImages.Count; ++i)
        {
            _reservedImages[i].Visibility = Visibility.Collapsed;
        }

        CurrentSize = size;
    }

    public void ChangeSource(int index, BitmapImage image)
    {
        _reservedImages[index].Visibility = Visibility.Visible;
        _reservedImages[index].Source = image;
    }

    public void LocateCursor(int index)
    {
        if (index < 0) return;
        Grid.SetColumn(cursorBorder, index % ColumnSize);
        Grid.SetRow(cursorBorder, index / ColumnSize);
        Cursor = index;
    }

    private void appendElement(Image uiElement)
    {
        int c = _reservedImages.Count % ColumnSize;
        int r = _reservedImages.Count / ColumnSize;

        if (r >= rootGrid.RowDefinitions.Count)
        {
            // 行が足りないから増やす
            pushGridRow(rootGrid);
        }

        Grid.SetColumn(uiElement, c);
        Grid.SetRow(uiElement, r);
        rootGrid.Children.Add(uiElement);

        // キャッシュにも格納
        _reservedImages.Add(uiElement);
    }

    private static void pushGridRow(Grid parent)
    {
        parent.RowDefinitions.Add(new RowDefinition()
        {
            Height = new GridLength(1, GridUnitType.Star)
        });
    }

    private void onGotFocus(object sender, RoutedEventArgs e)
    {
        focusCursor(true);
    }

    private void onLostFocus(object sender, RoutedEventArgs e)
    {
        focusCursor(false);
    }

    private void focusCursor(bool isFocused)
    {
        cursorBorder.BorderBrush = new SolidColorBrush(
            (Color)ColorConverter.ConvertFromString("#32bcef") * (isFocused ? 1.0f : 0.5f));
        cursorBorder.BorderThickness = new Thickness(isFocused ? 4 : 2);
    }
}