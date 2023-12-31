using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using EmojiInput_Model;
using EmojiInput_Utils;
using EmojiInput.Main.Control;
using EmojiInput.Main.Forward;
using EmojiInput.Main.Process;
using EmojiInput.Utils;

namespace EmojiInput.Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly WindowInteropHelper _interop;
        private readonly CancellationTokenSource _cancellation = new();
        private readonly EmojiDatabase _emojiDatabase;
        private readonly EmojiViewList _emojiViewList;
        private readonly EmojiFlushProcess _emojiFlushProcess;

        private int _emojiCursor;

        public MainWindow()
        {
            InitializeComponent();
            _interop = new WindowInteropHelper(this);
            _emojiDatabase = new EmojiDatabase("Resource/emoji.json");
            _emojiViewList = new EmojiViewList(_emojiDatabase.Count);

            _emojiFlushProcess = new EmojiFlushProcess(iconCollection, _emojiViewList);

            new EmojiLoadProcess(_emojiDatabase, _emojiViewList)
                .StartAsync(_cancellation.Token)
                .RunTaskHandlingError();

            iconCollection.Reserve(_emojiDatabase.Count);

            flushEmoji(_emojiDatabase);

            registerHotKeys();

            startAsync(_cancellation.Token).RunTaskHandlingError();
        }

        private void flushEmoji(IReadOnlyList<EmojiData> filteredData)
        {
            _emojiFlushProcess.StartAsync(filteredData.ToList(), _cancellation.Token).RunTaskHandlingError();
        }

        private void registerHotKeys()
        {
            int modifier = ModifierKeys.Control.ToInt() + ModifierKeys.Alt.ToInt();
            var key = KeyInterop.VirtualKeyFromKey(Key.OemSemicolon);
            int result = Win32.RegisterHotKey(_interop.Handle, Consts.HotKeyId_1, modifier, key);
            if (result == 0) Console.Error.WriteLine("Failed to register hot keys.");
            ComponentDispatcher.ThreadPreprocessMessage += onReceivedMessage;
        }

        private void onReceivedMessage(ref MSG msg, ref bool handled)
        {
            if (msg.message != Consts.MessageHotKey_0x0312) return;

            switch (msg.wParam.ToInt32())
            {
            case Consts.HotKeyId_1:
                startAsync(_cancellation.Token).RunTaskHandlingError();
                break;
            }
        }

        private async Task startAsync(CancellationToken cancel)
        {
            if (IsActive) return;
            Show();
            popupOnActiveWindow();
            iconCollection.LocateCursor(0);
            searchTextBox.Focus();
        }

        private void popupOnActiveWindow()
        {
            var rect = WinUtil.GetActiveWindowRect();
            var center = new Vector2(rect.Left + rect.Right, rect.Top + rect.Bottom) / 2;
            var tl = center / (float)ControlUtil.GetWindowScaling(this);
            float activeScaling = WinUtil.GetActiveWindowScaling();
            if (activeScaling == 0) return;

            Left = tl.X - (Width / activeScaling) / 2;
            Top = tl.Y - (Height / activeScaling) / 2;
            Topmost = true;
        }

        private void searchTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            iconCollection.LocateCursor(0);
            var searchText = searchTextBox.Text.TrimStart();
            var filtered = _emojiDatabase
                .Where(emoji =>
                    emoji.Description.Contains(searchText) || emoji.Aliases.Any(a => a.Contains(searchText)))
                .ToList();
            if (filtered.Count > 0)
            {
                selectedAlias.Text = filtered[0].Description;
            }

            flushEmoji(filtered);
        }

        private void onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                focusSearchTextBox();
                return;
            }

            if (iconCollection.CurrentSize == 0) return;

            switch (e.Key)
            {
            case Key.Left:
                if (iconCollection.IsFocused == false) return;
                _emojiCursor--;
                break;
            case Key.Right:
                if (iconCollection.IsFocused == false) return;
                _emojiCursor++;
                break;
            case Key.Up:
                _emojiCursor -= EmojiIconCollection.ColumnSize;
                break;
            case Key.Down:
                _emojiCursor += EmojiIconCollection.ColumnSize;
                break;
            default:
                return;
            }

            // カーソル移動
            // TODO: 上下移動の操作でフォーカスが切り替わるように修正
            while (_emojiCursor < 0)
            {
                _emojiCursor += EmojiIconCollection.ColumnSize;
            }

            while (_emojiCursor >= iconCollection.CurrentSize)
            {
                _emojiCursor -= EmojiIconCollection.ColumnSize;
            }

            iconCollection.LocateCursor(_emojiCursor);
            iconCollection.Focus();
        }

        private void onPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length == 0) return;
            if (char.IsLetterOrDigit(e.Text[0]) || e.Text[0] == '_')
            {
                // 英数字かアンダースコアが入力されたら検索欄にフォーカス
                focusSearchTextBox();
            }
        }

        private void focusSearchTextBox()
        {
            _emojiCursor = 0;
            searchTextBox.Focus();
        }
    }
}