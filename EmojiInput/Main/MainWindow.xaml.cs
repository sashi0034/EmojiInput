using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using EmojiInput_Model;
using EmojiInput_Utils;
using EmojiInput.Main.Forward;
using EmojiInput.Main.Process;
using ModernWpf.Controls;

namespace EmojiInput.Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly EmojiSettingModel _settingModel;
        private readonly WindowInteropHelper _interop;
        private readonly CancellationTokenSource _cancellation = new();
        private readonly EmojiDatabase _emojiDatabase;
        private readonly EmojiSkinData _skinData;
        private readonly EmojiViewList _emojiViewList;
        private readonly EmojiLoadProcess _emojiLoadProcess;
        private readonly EmojiFlushProcess _emojiFlushProcess;
        private readonly FocusCursorMover _focusCursorMover;
        private readonly EmojiFilteredModel _filteredModel = new();

        private IntPtr _lastForegroundWindow;
        private bool _isPinEnabled;

        public MainWindow(EmojiSettingModel settingModel)
        {
            InitializeComponent();

            // 各初期化
            _settingModel = settingModel;
            _interop = new WindowInteropHelper(this);
            _emojiDatabase = new EmojiDatabase("Resource/emoji.json");
            _skinData = new EmojiSkinData("Resource/emoji_skin.json");
            iconCollection.Reserve(_emojiDatabase.Count);
            _emojiViewList = new EmojiViewList(_emojiDatabase.Count);

            _emojiLoadProcess = new EmojiLoadProcess(_emojiDatabase, _settingModel, _emojiViewList);
            _emojiFlushProcess = new EmojiFlushProcess(iconCollection, _emojiViewList);
            _focusCursorMover = new FocusCursorMover(
                _filteredModel, selectedDescription, iconCollection, searchTextBox, scrollViewer);
            _focusCursorMover.Subscribe();

            // 絵文字を非同期読み込み
            _emojiLoadProcess.StartAsync(_cancellation.Token).RunErrorHandler();

            // 絵文字を非同期表示
            _filteredModel.Refresh(_emojiDatabase);
            flushEmojiIcons();

            // 設定項目初期化
            new MenuSetup(_skinData, _settingModel, skinMenu, selectedSkinImage, iconSizeMenu).Invoke();
            iconCollection.ChangeIconSize(_settingModel.IconSize);

            // アイコンクリック時の設定
            subscribeImageClicked();

            // ホットキー設定
            registerHotKeys();

#if DEBUG
            new PopupProcess(this, searchTextBox).StartAsync(_cancellation.Token).RunErrorHandler();
#endif
        }

        ~MainWindow()
        {
            _cancellation.Cancel();
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
            _emojiFlushProcess.StartAsync(_filteredModel.List, _cancellation.Token).RunErrorHandler();
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
                _lastForegroundWindow = Win32.GetForegroundWindow();
                new PopupProcess(this, searchTextBox).StartAsync(_cancellation.Token).RunErrorHandler();
                break;
            }
        }

        private void searchTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            iconCollection.LocateCursor(0);
            _filteredModel.FilterRefresh(_emojiDatabase, searchTextBox.Text.TrimStart());
            flushEmojiIcons();

            // 検索したらカーソルをリセット
            _focusCursorMover.MoveCursor(0);
        }

        private void sendEmojiAndClose()
        {
            int cursor = iconCollection.Cursor;
            if (cursor < 0 || _filteredModel.List.Count <= cursor) return;
            requestHideWindow();
            Win32.SetForegroundWindow(_lastForegroundWindow);
            var selectedEmoji = _filteredModel.List[cursor];
            System.Windows.Forms.SendKeys.SendWait(selectedEmoji.HasSkinTones && _settingModel.SkinKey != ""
                ? _skinData.GetSkinEmoji(selectedEmoji.Aliases[0], _settingModel.SkinKey)
                : selectedEmoji.EmojiCharacter);
        }

        private void onClosing(object? sender, CancelEventArgs e)
        {
            // ウィンドウを破棄せずに非表示にする
            e.Cancel = true;
            requestHideWindow();
        }

        private void requestHideWindow()
        {
            if (_isPinEnabled) return;
            Hide();
            _settingModel.RefreshSave();
        }

        private void iconSizeMenu_OnChecked(object sender, RoutedEventArgs e)
        {
            int index = iconSizeMenu.Items.IndexOf(e.Source);
            if (index == -1) return;
            _settingModel.IconSize = index.ToEnum<EmojiIconSizeKind>();
            iconCollection.ChangeIconSize(_settingModel.IconSize);
            _focusCursorMover.ScrollToCursor();
        }

        private void skinMenu_OnChecked(object sender, RoutedEventArgs e)
        {
            int index = skinMenu.Items.IndexOf(e.Source);
            if (index == -1) return;
            if (e.Source is RadioMenuItem { Header: Image image })
                selectedSkinImage.Source = image.Source;
            _settingModel.SkinKey = index == 0 ? "" : _skinData.Keys[index - 1];
            _emojiLoadProcess.StartAsync(_cancellation.Token).RunErrorHandler();
            flushEmojiIcons();
        }

        private void iconCollection_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) sendEmojiAndClose();
            _focusCursorMover.PreviewKeyDown(e);
        }

        private void searchTextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) sendEmojiAndClose();
        }

        private void searchTextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            _focusCursorMover.PreviewKeyDown(e);
        }

        private void iconCollection_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            _focusCursorMover.PreviewKeyDown(e);
        }

        private void iconCollection_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            _focusCursorMover.PreviewTextInput(e);
        }

        private void pinTitleBarButton_OnClick(object sender, RoutedEventArgs e)
        {
            _isPinEnabled = !_isPinEnabled;
            Topmost = _isPinEnabled;
            if (_isPinEnabled)
                pinGrid.SetResourceReference(Panel.BackgroundProperty, "AccentButtonBackground");
            else
                pinGrid.Background = Brushes.Transparent;
        }
    }
}