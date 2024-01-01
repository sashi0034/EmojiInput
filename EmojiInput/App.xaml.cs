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
    }
}