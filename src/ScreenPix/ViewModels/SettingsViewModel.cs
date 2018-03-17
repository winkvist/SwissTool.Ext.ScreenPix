// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsViewModel.cs" company="Fredrik Winkvist">
//   Copyright (c) Fredrik Winkvist. All rights reserved.
// </copyright>
// <summary>
//   The settings view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SwissTool.Ext.ScreenPix.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Input;

    using SwissTool.Ext.ScreenPix.Managers;
    using SwissTool.Ext.ScreenPix.Models;
    using SwissTool.Framework.UI.Commanding;
    using SwissTool.Framework.UI.Infrastructure;
    using SwissTool.Framework.Utilities.Serialization;

    /// <summary>
    /// The settings view model.
    /// </summary>
    public class SettingsViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
        /// </summary>
        public SettingsViewModel()
        {
            this.AcceptCommand = new RelayCommand(o => this.Accept());
            this.ResetToolbarLocationCommand = new RelayCommand(o => this.ResetToolbarLocation());

            this.Settings = ApplicationManager.Settings;
            this.SettingsCopy = JsonUtils.Clone(ApplicationManager.Settings);
            this.FileExtensions = ApplicationManager.SupportedImageTypes;
        }

        /// <summary>
        /// Occurs when [request apply settings].
        /// </summary>
        public event Action RequestApplySettings;

        /// <summary>
        /// Gets or sets the accept command.
        /// </summary>
        /// <value>The accept command.</value>
        public ICommand AcceptCommand { get; set; }

        /// <summary>
        /// Gets or sets the reset toolbar location command.
        /// </summary>
        /// <value>
        /// The reset toolbar location command.
        /// </value>
        public ICommand ResetToolbarLocationCommand { get; set; }

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>The settings.</value>
        public AppSettings Settings { get; set; }

        /// <summary>
        /// Gets or sets the settings copy.
        /// </summary>
        /// <value>The settings copy.</value>
        public AppSettings SettingsCopy { get; set; }

        /// <summary>
        /// Gets or sets the file extensions.
        /// </summary>
        /// <value>
        /// The file extensions.
        /// </value>
        public List<string> FileExtensions { get; set; }

        /// <summary>
        /// Gets or sets the default file extension.
        /// </summary>
        /// <value>
        /// The default file extension.
        /// </value>
        public string DefaultFileExtension
        {
            get
            {
                return this.SettingsCopy.DefaultFileExtension;
            }

            set
            {
                this.SettingsCopy.DefaultFileExtension = value;

                this.NotifyPropertyChanged(nameof(this.DefaultFileExtension));
                this.NotifyPropertyChanged(nameof(this.IsImageQualityVisible));
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is image quality visible.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is image quality visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsImageQualityVisible
        {
            get
            {
                var qualityExtensions = new List<string> { "JPG", "JPE", "JPEG", "JFIF" };
                return qualityExtensions.Contains(this.SettingsCopy.DefaultFileExtension);
            }
        }

        /// <summary>
        /// Gets the toolbar location label.
        /// </summary>
        /// <value>
        /// The toolbar location label.
        /// </value>
        public string ToolbarLocationLabel
        {
            get
            {
                if (this.SettingsCopy.ToolBarLocationX == -1 && this.SettingsCopy.ToolBarLocationY == -1)
                {
                    return "[ Center Screen ]";
                }

                return string.Format(
                    "[ {0} , {1} ]",
                    this.SettingsCopy.ToolBarLocationX,
                    this.SettingsCopy.ToolBarLocationY);
            }
        }

        /// <summary>
        /// Accepts this instance.
        /// </summary>
        private void Accept()
        {
            this.SaveChanges();
            this.Close();
        }

        /// <summary>
        /// Resets the toolbar location.
        /// </summary>
        private void ResetToolbarLocation()
        {
            this.SettingsCopy.ToolBarLocationX = -1;
            this.SettingsCopy.ToolBarLocationY = -1;

            this.NotifyPropertyChanged(nameof(this.ToolbarLocationLabel));
        }

        /// <summary>
        /// Saves the changes.
        /// </summary>
        private void SaveChanges()
        {
            ApplicationManager.Settings.ImageQuality = this.SettingsCopy.ImageQuality;
            ApplicationManager.Settings.DefaultFileExtension = this.SettingsCopy.DefaultFileExtension;
            ApplicationManager.Settings.ToolBarLocationX = this.SettingsCopy.ToolBarLocationX;
            ApplicationManager.Settings.ToolBarLocationY = this.SettingsCopy.ToolBarLocationY;

            ApplicationManager.SaveSettings();

            this.RequestApplySettings?.Invoke();
        }
    }
}
