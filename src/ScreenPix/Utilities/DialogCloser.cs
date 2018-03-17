// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DialogCloser.cs" company="Fredrik Winkvist">
//   Copyright (c) Fredrik Winkvist. All rights reserved.
// </copyright>
// <summary>
//   Defines the DialogCloser type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SwissTool.Ext.ScreenPix.Utilities
{
    using System.Windows;

    /// <summary>
    /// The dialog closer class.
    /// </summary>
    public static class DialogCloser
    {
        /// <summary>
        /// The dialog result property
        /// </summary>
        public static readonly DependencyProperty DialogResultProperty = 
            DependencyProperty.RegisterAttached(
                "DialogResult",
                typeof(bool?),
                typeof(DialogCloser),
                new PropertyMetadata(DialogResultChanged));

        /// <summary>
        /// Sets the dialog result.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        public static void SetDialogResult(Window target, bool? value)
        {
            target.SetValue(DialogResultProperty, value);
        }

        /// <summary>
        /// Dialogs the result changed.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void DialogResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as Window;
            if (window != null)
            {
                window.DialogResult = e.NewValue as bool?;
            }
        }
    }
}
