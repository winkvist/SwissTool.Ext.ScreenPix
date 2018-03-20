// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Fredrik Winkvist">
//   Copyright (c) Fredrik Winkvist. All rights reserved.
// </copyright>
// <summary>
//   Defines the MainViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SwissTool.Ext.ScreenPix.ViewModels
{
    using System.Reflection;
    using System.Windows.Forms;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;

    using SwissTool.Ext.ScreenPix.Managers;
    using SwissTool.Ext.ScreenPix.Models;
    using SwissTool.Ext.ScreenPix.Utilities;
    using SwissTool.Ext.ScreenPix.Views;
    using SwissTool.Framework.Commanding;
    using SwissTool.Framework.UI.Infrastructure;
    using SwissTool.Framework.UI.Managers;

    /// <summary>
    /// The main view model.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// The captured screen shot
        /// </summary>
        private Screenshot screenShot;

        /// <summary>
        /// Display the font settings
        /// </summary>
        private bool displayFontSettings;

        /// <summary>
        /// Display the draw settings.
        /// </summary>
        private bool displayDrawSettings;

        /// <summary>
        /// Display the color settings.
        /// </summary>
        private bool displayColorSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <param name="screenshot">The screenshot.</param>
        public MainViewModel(Screenshot screenshot)
        {
            this.Screenshot = screenshot;

            this.ShowAboutWindowCommand = new RelayCommand(o => this.ShowAboutWindow());
            this.ShowSettingsWindowCommand = new RelayCommand(o => this.ShowSettingsWindow());

            this.OnRequestApplySettings();
        }

        /// <summary>
        /// Gets or sets the captured screenshot.
        /// </summary>
        /// <value>
        /// The screenshot.
        /// </value>
        public Screenshot Screenshot
        {
            get
            {
                return this.screenShot;
            }

            set
            { 
                this.screenShot = value;
                this.NotifyPropertyChanged(nameof(this.Screenshot));
                this.NotifyPropertyChanged(nameof(this.FullscreenImage));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [display font settings].
        /// </summary>
        /// <value><c>true</c> if [display font settings]; otherwise, <c>false</c>.</value>
        public bool DisplayFontSettings
        {
            get
            {
                return this.displayFontSettings;
            }

            set
            {
                this.displayFontSettings = value;
                this.NotifyPropertyChanged(nameof(this.DisplayFontSettings));
                this.NotifyPropertyChanged(nameof(this.ShowToolPropertyPanel));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [display draw settings].
        /// </summary>
        /// <value><c>true</c> if [display draw settings]; otherwise, <c>false</c>.</value>
        public bool DisplayDrawSettings
        {
            get
            {
                return this.displayDrawSettings;
            }

            set
            {
                this.displayDrawSettings = value;
                this.NotifyPropertyChanged(nameof(this.DisplayDrawSettings));
                this.NotifyPropertyChanged(nameof(this.ShowToolPropertyPanel));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [display color settings].
        /// </summary>
        /// <value>
        /// <c>true</c> if [display color settings]; otherwise, <c>false</c>.
        /// </value>
        public bool DisplayColorSettings
        {
            get
            {
                return this.displayColorSettings;
            }

            set
            {
                this.displayColorSettings = value;
                this.NotifyPropertyChanged(nameof(this.DisplayColorSettings));
                this.NotifyPropertyChanged(nameof(this.ShowToolPropertyPanel));
            }
        }

        /// <summary>
        /// Gets a value indicating whether [show tool property panel].
        /// </summary>
        /// <value>
        /// <c>true</c> if [show tool property panel]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowToolPropertyPanel
        {
            get
            {
                return this.DisplayFontSettings || this.DisplayDrawSettings || this.DisplayColorSettings;
            }
        }

        /// <summary>
        /// Gets the undo command.
        /// </summary>
        public ICommand UndoCommand { get; private set; }

        /// <summary>
        /// Gets or sets the show about window command.
        /// </summary>
        /// <value>
        /// The show about window command.
        /// </value>
        public ICommand ShowAboutWindowCommand { get; set; }

        /// <summary>
        /// Gets or sets the show settings window command.
        /// </summary>
        /// <value>
        /// The show settings window command.
        /// </value>
        public ICommand ShowSettingsWindowCommand { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show font settings].
        /// </summary>
        /// <value><c>true</c> if [show font settings]; otherwise, <c>false</c>.</value>
        public bool ShowFontSettings { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show draw settings].
        /// </summary>
        /// <value><c>true</c> if [show draw settings]; otherwise, <c>false</c>.</value>
        public bool ShowDrawSettings { get; set; }

        /// <summary>
        /// Gets the fullscreen image.
        /// </summary>
        public BitmapSource FullscreenImage
        {
            get
            {
                if (this.Screenshot?.Image == null)
                {
                    return null;
                }

                return ImageUtilities.BitmapToBitmapSource(this.Screenshot.Image);
            }
        }

        /// <summary>
        /// Gets or sets the tool bar location X.
        /// </summary>
        /// <value>The tool bar location X.</value>
        public int ToolBarLocationX
        {
            get
            {
                return ApplicationManager.Settings.ToolBarLocationX;
            }

            set
            {
                ApplicationManager.Settings.ToolBarLocationX = value;
                this.NotifyPropertyChanged(nameof(this.ToolBarLocationX));
                this.NotifyPropertyChanged(nameof(this.SubToolBarLocationX));
            }
        }

        /// <summary>
        /// Gets or sets the tool bar location Y.
        /// </summary>
        /// <value>The tool bar location Y.</value>
        public int ToolBarLocationY
        {
            get
            {
                return ApplicationManager.Settings.ToolBarLocationY;
            }

            set
            {
                ApplicationManager.Settings.ToolBarLocationY = value;
                this.NotifyPropertyChanged(nameof(this.ToolBarLocationY));
                this.NotifyPropertyChanged(nameof(this.SubToolBarLocationY));
            }
        }

        /// <summary>
        /// Gets the sub tool bar location X.
        /// </summary>
        /// <value>The sub tool bar location X.</value>
        public int SubToolBarLocationX
        {
            get
            {
                return ApplicationManager.Settings.ToolBarLocationX + 216;
            }
        }

        /// <summary>
        /// Gets the sub tool bar location Y.
        /// </summary>
        /// <value>The sub tool bar location Y.</value>
        public int SubToolBarLocationY
        {
            get
            {
                return ApplicationManager.Settings.ToolBarLocationY + 25;
            }
        }

        /// <summary>
        /// Gets the preview panel location X.
        /// </summary>
        /// <value>The preview panel location X.</value>
        public int PreviewPanelLocationX
        {
            get
            {
                var width = Screen.PrimaryScreen.Bounds.Width;
                return (width / 2) - 115;
            }
        }

        /// <summary>
        /// Gets the preview panel location Y.
        /// </summary>
        /// <value>The preview panel location Y.</value>
        public int PreviewPanelLocationY
        {
            get
            {
                var height = Screen.PrimaryScreen.Bounds.Height;
                return (height / 2) - 25;
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
            viewModel.RequestApplySettings += this.OnRequestApplySettings;
            view.DataContext = viewModel;
            view.ShowDialog();
            viewModel.RequestClose -= view.Close;
            viewModel.RequestApplySettings -= this.OnRequestApplySettings;
        }

        /// <summary>
        /// Called when settings is applied.
        /// </summary>
        private void OnRequestApplySettings()
        {
            if (ApplicationManager.Settings.ToolBarLocationX == -1 && ApplicationManager.Settings.ToolBarLocationY == -1)
            {
                var height = Screen.PrimaryScreen.Bounds.Height;
                var width = Screen.PrimaryScreen.Bounds.Width;

                this.ToolBarLocationX = (width - 400) / 2;
                this.ToolBarLocationY = (height / 2) + 50;
            }
        }
    }
}
