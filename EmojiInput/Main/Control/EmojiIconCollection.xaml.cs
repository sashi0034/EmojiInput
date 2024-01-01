using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using EmojiInput_Model;
using EmojiInput_Utils;
using UserControl = System.Windows.Controls.UserControl;

namespace EmojiInput.Main.Control;

public partial class EmojiIconCollection : UserControl
{
    public int ColumnSize { get; private set; }
    private static IReadOnlyList<int> columnSizes = new List<int> { 20, 10 };

    public int ImageSize { get; private set; }
    private static IReadOnlyList<int> imageSizes = new List<int> { 32, 64 };

    private readonly List<Image> _reservedImages = new();
    public IReadOnlyList<Image> ReservedImages => _reservedImages;

    public int CurrentSize { get; private set; }

    public int Cursor { get; private set; }

    public EmojiIconCollection()
    {
        InitializeComponent();

        pushGridRow(rootGrid);
        focusCursor(false);
    }

    public void ChangeIconSize(EmojiIconSizeKind kind)
    {
        ColumnSize = columnSizes[kind.ToInt()];
        ImageSize = imageSizes[kind.ToInt()];

        cursorBorder.Width = ImageSize;
        cursorBorder.Height = ImageSize;

        rootGrid.ColumnDefinitions.Clear();
        for (int i = 0; i < ColumnSize; ++i)
        {
            rootGrid.ColumnDefinitions.Add(
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        }

        for (int i = 0; i < _reservedImages.Count(); i++)
        {
            _reservedImages[i].Width = ImageSize;
            _reservedImages[i].Height = ImageSize;
            locateImageOnGrid(i, _reservedImages[i]);
        }

        rootGrid.UpdateLayout();
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
            rootGrid.Children.Add(newImage);
            _reservedImages.Add(newImage);
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

    private void locateImageOnGrid(int index, Image uiElement)
    {
        int c = index % ColumnSize;
        int r = index / ColumnSize;

        while (r >= rootGrid.RowDefinitions.Count)
        {
            // 行が足りないから増やす
            pushGridRow(rootGrid);
        }

        Grid.SetColumn(uiElement, c);
        Grid.SetRow(uiElement, r);
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