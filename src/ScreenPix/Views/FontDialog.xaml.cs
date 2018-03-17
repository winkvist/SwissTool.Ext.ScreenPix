// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FontDialog.xaml.cs" company="Fredrik Winkvist">
//   Copyright (c) Fredrik Winkvist. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for FontDialog.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SwissTool.Ext.ScreenPix.Views
{
    using System;
    using System.Windows;
    using System.Windows.Media.Imaging;

    using SwissTool.Ext.ScreenPix.ViewModels;
    using SwissTool.Framework.UI.Controls;
    using SwissTool.Framework.UI.Managers;

    /// <summary>
    /// Interaction logic for FontDialog
    /// </summary>
    public partial class FontDialog : MetroDialogWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FontDialog"/> class.
        /// </summary>
        public FontDialog()
        {
            this.InitializeComponent();

            var uiHint = WindowManager.CurrentTheme.UiHint;
            var uriString = $"/ScreenPix;component/Resources/Icons/{uiHint}/48x48/services.png";
            this.Image = new BitmapImage(new Uri(uriString, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Handles the Loaded event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            var dataContext = this.DataContext as FontDialogViewModel;

            if (dataContext != null)
            {
                this.FontSizeListBox.ScrollIntoView(dataContext.SelectedFontSize);
                this.FontFamilyListBox.ScrollIntoView(dataContext.SelectedFontFamily);
            }
        }
    }
}
