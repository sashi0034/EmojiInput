using System;
using System.Collections.Generic;
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

        public MainWindow()
        {
            InitializeComponent();
            _interop = new WindowInteropHelper(this);
            _emojiDatabase = new EmojiDatabase("Resource/emoji.json");
            setupImages(_cancellation.Token).RunTaskHandlingError();

            registerHotKeys();
            startAsync(_cancellation.Token).RunTaskHandlingError();
        }

        private async Task setupImages(CancellationToken cancel)
        {
            var bitmapImages = new List<BitmapImage>();

            foreach (var emoji in _emojiDatabase)
            {
                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    var iconPath = "Resource/emoji_icon/" + emoji.ImageFilename;
                    bitmap.UriSource = new Uri(iconPath, UriKind.Relative);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    bitmapImages.Add(bitmap);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                    continue;
                }

                if (bitmapImages.Count >= Consts.Large_100) await flushImagesAsync(bitmapImages, cancel);
            }

            await flushImagesAsync(bitmapImages, cancel);
        }

        private async Task flushImagesAsync(List<BitmapImage> bitmapImages, CancellationToken cancel)
        {
            bool ok = false;
            Dispatcher.Invoke(() =>
            {
                foreach (var src in bitmapImages)
                {
                    var image = new Image
                    {
                        Width = 64,
                        Source = src
                    };
                    iconStackPanel.Children.Add(image);
                }

                bitmapImages.Clear();
                ok = true;
            });
            while (true)
            {
                await Task.Delay(Consts.Large_100, cancel);
                if (ok) break;
            }
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

            selectedAlias.Text = UnicodeUtil.UnicodeToCharacter("2b50");
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
    }
}