using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using EmojiInput.Main;

namespace EmojiInput
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly MainWindow _mainWindow = new();

        public App()
        {
            InitializeComponent();

            _mainWindow.ShowInTaskbar = false;
            _mainWindow.ShowActivated = false;
        }
    }
}