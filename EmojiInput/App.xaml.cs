using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using EmojiInput_Model;
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

        public App()
        {
            InitializeComponent();

            _settingModel.Load();

            _mainWindow = new MainWindow(_settingModel)
            {
                ShowInTaskbar = false,
                ShowActivated = false
            };
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            setupNotifyIcon();
        }

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
    }
}