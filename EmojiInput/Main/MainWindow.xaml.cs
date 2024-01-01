using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using EmojiInput_Model;
using EmojiInput_Utils;
using EmojiInput.Main.Forward;
using EmojiInput.Main.Process;
using EmojiInput.Utils;
using ModernWpf.Controls;

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
        private readonly FocusCursorMover _focusCursorMover;
        private readonly EmojiFilteredModel _filteredModel = new();
        private readonly EmojiIconSizeModel _iconSizeModel = new();

        public MainWindow()
        {
            InitializeComponent();

            // 各初期化
            _interop = new WindowInteropHelper(this);
            _emojiDatabase = new EmojiDatabase("Resource/emoji.json");
            iconCollection.Reserve(_emojiDatabase.Count);
            _emojiViewList = new EmojiViewList(_emojiDatabase.Count);

            _emojiFlushProcess = new EmojiFlushProcess(iconCollection, _emojiViewList);
            _focusCursorMover = new FocusCursorMover(
                _filteredModel, selectedDescription, iconCollection, searchTextBox, scrollViewer);
            _focusCursorMover.Subscribe();

            // 絵文字を非同期読み込み
            new EmojiLoadProcess(_emojiDatabase, _emojiViewList)
                .StartAsync(_cancellation.Token)
                .RunTaskHandlingError();

            // 絵文字を非同期表示
            _filteredModel.Refresh(_emojiDatabase);
            flushEmojiIcons();

            // アイコンサイズ設定
            if (iconSizeMenu.Items[_iconSizeModel.Kind.ToInt()] is RadioMenuItem checkingIconSize)
                checkingIconSize.IsChecked = true;
            iconCollection.ChangeIconSize(_iconSizeModel.Kind);

            // アイコンクリック時の設定
            subscribeImageClicked();

            // ホットキー設定
            registerHotKeys();

#if DEBUG
            startAsync(_cancellation.Token).RunTaskHandlingError();
#endif
        }

        private void subscribeImageClicked()
        {
            iconCollection.ReservedImages.ForEachIndexed(((image, index) =>
            {
                image.MouseLeftButtonUp += ((_, _) =>
                {
                    _focusCursorMover.MoveCursor(index);
                    sendEmojiAndClose();
                });
            }));
        }

        private void flushEmojiIcons()
        {
            _emojiFlushProcess.StartAsync(_filteredModel.List, _cancellation.Token).RunTaskHandlingError();
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
            Dispatcher.Invoke(() =>
            {
                searchTextBox.Focus();
                searchTextBox.SelectAll();
                // _focusCursorMover.MoveCursor(0);
            });
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
            // Topmost = true;
            Activate();
        }

        private void searchTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            iconCollection.LocateCursor(0);
            _filteredModel.FilterRefresh(_emojiDatabase, searchTextBox.Text.TrimStart());
            flushEmojiIcons();

            // 検索したらカーソルをリセット
            _focusCursorMover.MoveCursor(0);
        }

        private void onPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                sendEmojiAndClose();
                return;
            }

            _focusCursorMover.PreviewKeyDown(e);
        }

        private void sendEmojiAndClose()
        {
            int cursor = iconCollection.Cursor;
            if (cursor < 0 || _filteredModel.List.Count <= cursor) return;
            Hide();
            System.Windows.Forms.SendKeys.SendWait(_filteredModel.List[cursor].EmojiCharacter);
        }

        private void onPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            _focusCursorMover.PreviewTextInput(e);
        }

        private void scrollViewer_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key is Key.Left or Key.Right or Key.Down or Key.Up) e.Handled = true;
        }

        private void onClosing(object? sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void iconSizeMenu_OnChecked(object sender, RoutedEventArgs e)
        {
            int index = iconSizeMenu.Items.IndexOf(e.Source);
            if (index == -1) return;
            _iconSizeModel.Kind = index.ToEnum<EmojiIconSizeKind>();
            iconCollection.ChangeIconSize(_iconSizeModel.Kind);
            _focusCursorMover.ScrollToCursor();
        }
    }
}