// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScreenPixExt.cs" company="Fredrik Winkvist">
//   Copyright (c) Fredrik Winkvist. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for App.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SwissTool.Ext.ScreenPix
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media.Imaging;

    using SwissTool.Ext.ScreenPix.Managers;
    using SwissTool.Ext.ScreenPix.Models;
    using SwissTool.Ext.ScreenPix.Utilities;
    using SwissTool.Ext.ScreenPix.ViewModels;
    using SwissTool.Ext.ScreenPix.Views;
    using SwissTool.Framework.Commanding;
    using SwissTool.Framework.Enums;
    using SwissTool.Framework.Infrastructure;
    using SwissTool.Framework.UI.Managers;

    /// <summary>
    /// Interaction logic for App
    /// </summary>
    public class ScreenPixExt : ExtensionBase
    {
        /// <summary>
        /// A flag to keep track on running instances.
        /// </summary>
        private bool isInUse;

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>The settings.</value>
        public AppSettings Settings { get; private set; }
        
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public override void Initialize()
        {
            this.isInUse = false;

            this.InitiateMenuItems();
            this.InitiateActions();

            this.Icon = new BitmapImage(new Uri("/ScreenPix;component/Resources/Images/app.png", UriKind.RelativeOrAbsolute));

            // Load Settings
            this.Settings = this.LoadConfiguration<AppSettings>();
            
            ApplicationManager.Setup(this, this.Settings);
        }

        /// <summary>
        /// Captures the screen delayed.
        /// </summary>
        public void CaptureScreenDelayed()
        {
            Task.Run(() =>
                    {
                        Task.Delay(250).Wait(); // Wait for that context menu to close :)
                        Application.Current.Dispatcher.Invoke(this.CaptureScreen);
                    });
        }

        /// <summary>
        /// Captures the screen.
        /// </summary>
        public void CaptureScreen()
        {
            if (this.isInUse)
            {
                MessageBox.Show("Close the previous workspace before capturing a new screenshot", this.Name, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                this.isInUse = true;
                var screenshot = ImageUtilities.CaptureScreen(ScreenCaptureMode.All);

                WindowManager.ShowDialog<MainView>(new MainViewModel(screenshot));
            }
            finally
            {
                this.isInUse = false;
            }
        }

        /// <summary>
        /// Shows the about window.
        /// </summary>
        public void ShowAboutWindow()
        {
            var model = new AboutModel(Assembly.GetExecutingAssembly());

            WindowManager.ShowDialog<AboutView>(new AboutViewModel { Model = model });
        }

        /// <summary>
        /// Shows the settings window.
        /// </summary>
        public void ShowSettingsWindow()
        {
            var view = new SettingsView();
            var viewModel = new SettingsViewModel();
            viewModel.RequestClose += view.Close;
            view.DataContext = viewModel;
            view.ShowDialog();
            viewModel.RequestClose -= view.Close;
        }

        /// <summary>
        /// Initiates the menu items.
        /// </summary>
        private void InitiateMenuItems()
        {
            this.AddMenuItems(
                new ExtensionMenuItem
                    {
                        Name = "Take screenshot",
                        Description = "Captures the screen",
                        Icon = new BitmapImage(new Uri(this.GetIconPath("camera"), UriKind.RelativeOrAbsolute)),
                        Command = new RelayCommand(o => this.CaptureScreenDelayed())
                    },
                new ExtensionMenuItem
                    {
                        Name = "Settings",
                        Description = "Displays the settings window",
                        Icon = new BitmapImage(new Uri(this.GetIconPath("services"), UriKind.RelativeOrAbsolute)),
                        Command = new RelayCommand(o => this.ShowSettingsWindow())
                    },
                new ExtensionMenuItem
                    {
                        Name = "About",
                        Description = "Displays the about window",
                        Icon = new BitmapImage(new Uri(this.GetIconPath("help"), UriKind.RelativeOrAbsolute)),
                        Command = new RelayCommand(o => this.ShowAboutWindow())
                    });
        }

        /// <summary>
        /// Gets the icon path.
        /// </summary>
        /// <param name="iconName">Name of the icon.</param>
        /// <returns>The icon path.</returns>
        private string GetIconPath(string iconName)
        {
            var uiHint = WindowManager.CurrentTheme.UiHint;
            var path = $"/ScreenPix;component/Resources/Icons/{uiHint}/24x24/{iconName}.png";

            return path;
        }

        /// <summary>
        /// Initiates the actions.
        /// </summary>
        private void InitiateActions()
        {
            this.AddActions(
                new ExtensionHotKeyAction
                    {
                        Identifier = "CaptureScreenHotKey",
                        Name = "Take screenshot",
                        Description = "Captures the screen",
                        Icon = new BitmapImage(new Uri(this.GetIconPath("camera"), UriKind.RelativeOrAbsolute)),
                        Command = new RelayCommand(o => this.CaptureScreen()),
                        DefaultHotKey = new ActionHotKey(HotKeyModifier.Control, HotKeyModifier.None, HotKey.PrintScreen)
                    });
        }
    }
}
