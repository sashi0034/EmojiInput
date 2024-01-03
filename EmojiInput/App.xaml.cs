using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using EmojiInput_Model;
using EmojiInput_Utils;
using EmojiInput.Main;

namespace EmojiInput
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly EmojiSettingModel _settingModel = new();
        private readonly MainWindow _mainWindow;
        private readonly System.Threading.Mutex _mutex = new(false, $"sashi0034/{Consts.AppProductName}");

        public App()
        {
            InitializeComponent();

            var targetDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (targetDir != null) Directory.SetCurrentDirectory(targetDir);

            checkInitialSetup();

            if (_mutex.WaitOne(0, false) == false)
            {
                // 多重起動防止
                _mutex.Close();
                Shutdown();
            }

            _settingModel.Load();

            _mainWindow = new MainWindow(_settingModel)
            {
                ShowInTaskbar = false,
                ShowActivated = false
            };

            if (_settingModel.Data.InstalledPath != Consts.GetCurrentExecutingPath())
            {
                MessageBox.Show(
                    $"You can input emoji: ctrl + shift + :",
                    "Info",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                askRebootAsAdmin();
                return;
            }
        }

        private void askRebootAsAdmin()
        {
            if (MessageBox.Show(
                    "Do you want to register the application as a startup application?",
                    "Setup",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                rebootAsAdmin();
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            setupNotifyIcon();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            _mutex.ReleaseMutex();
            _mutex.Close();
        }

        // タスクトレイにアイコンを出す
        private void setupNotifyIcon()
        {
            var icon = GetResourceStream(new Uri("Resource/app_icon.ico", UriKind.Relative))?.Stream;
            if (icon == null)
            {
                Console.Error.WriteLine("Missing application icon");
                return;
            }

            var menu = new System.Windows.Forms.ContextMenuStrip();
            menu.Items.Add("Shutdown Process", null, (_, _) => { Shutdown(); });
            var notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                Visible = true,
                Icon = new System.Drawing.Icon(icon),
                Text = "EmojiInput\nCtrl + Alt + :",
                ContextMenuStrip = menu
            };
            // notifyIcon.MouseClick += ((_, _) => { _mainWindow.StartPopup(); });
        }

        // 管理者権限があればレジストリに登録
        private void checkInitialSetup()
        {
            var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);
            if (principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
            {
                try
                {
                    var path = Consts.GetCurrentExecutingPath();
                    setupAsAdmin(path);
                    _settingModel.Data.InstalledPath = path;
                    _settingModel.RefreshSave();
                    MessageBox.Show($"Finished setup", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Filed to setup\n" + ex, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // 管理者権限で再起動
        private void rebootAsAdmin()
        {
            var startInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                WorkingDirectory = Environment.CurrentDirectory,
                FileName = Consts.AppExecutiveFileName,
                Verb = "runas"
            };

            try
            {
                Process.Start(startInfo);
                Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Filed to reboot\n" + ex, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// レジストリに実行ファイルパスを登録する
        private void setupAsAdmin(string path)
        {
            var registry =
                Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);

            if (registry == null) throw new NullReferenceException();

            registry.SetValue(Consts.AppProductName, path);

            registry.Close();
        }
    }
}