// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FontDialogViewModel.cs" company="Fredrik Winkvist">
//    Copyright (c) Fredrik Winkvist. All rights reserved.
// </copyright>
// <summary>
//   The font dialog view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SwissTool.Ext.ScreenPix.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;

    using SwissTool.Ext.ScreenPix.Managers;
    using SwissTool.Framework.Commanding;
    using SwissTool.Framework.UI.Infrastructure;

    /// <summary>
    /// The font dialog view model.
    /// </summary>
    public class FontDialogViewModel : ViewModelBase
    {
        /// <summary>
        /// The selected font family.
        /// </summary>
        private FontFamily selectedFontFamily;

        /// <summary>
        /// The selected font style.
        /// </summary>
        private FontStyle selectedFontStyle;

        /// <summary>
        /// The selected font weight.
        /// </summary>
        private FontWeight selectedFontWeight;

        /// <summary>
        /// The selected font style.
        /// </summary>
        private double selectedFontSize;

        /// <summary>
        /// Dialog result
        /// </summary>
        private bool? dialogResult;

        /// <summary>
        /// The selected weight and style combined.
        /// </summary>
        private object selectedWeightAndStyleCombined;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontDialogViewModel"/> class.
        /// </summary>
        public FontDialogViewModel()
        {
            this.SelectedFontFamily = ApplicationManager.Settings.FontFamily;
            this.SelectedFontSize = ApplicationManager.Settings.FontSize;
            this.SelectedFontWeight = ApplicationManager.Settings.FontWeight;
            this.SelectedFontStyle = ApplicationManager.Settings.FontStyle;

            var styleType = ApplicationManager.Settings.FontStyleType;

            if (styleType == "Style")
            {
                this.SelectedWeightAndStyleCombined = ApplicationManager.Settings.FontStyle;
            }
            else if (styleType == "Weight")
            {
                this.SelectedWeightAndStyleCombined = ApplicationManager.Settings.FontWeight;
            }
            else
            {
                this.SelectedWeightAndStyleCombined = ApplicationManager.Settings.FontStyle;
            }
            
            this.CancelCommand = new RelayCommand((o) => this.CloseWindow());
            this.SaveCommand = new RelayCommand((o) => this.SaveChanges());

            this.DialogResult = null;
        }

        /// <summary>
        /// Gets the cancel command.
        /// </summary>
        /// <value>The cancel command.</value>
        public ICommand CancelCommand { get; private set; }

        /// <summary>
        /// Gets or sets the save command.
        /// </summary>
        /// <value>The save command.</value>
        public ICommand SaveCommand { get; set; }

        /// <summary>
        /// Gets or sets the font family.
        /// </summary>
        /// <value>The font family.</value>
        public FontFamily SelectedFontFamily
        {
            get
            {
                return this.selectedFontFamily;
            }

            set
            {
                this.selectedFontFamily = value;

                this.NotifyPropertyChanged(nameof(this.FontWeightsAndStylesCombined));
                this.NotifyPropertyChanged(nameof(this.SelectedFontFamily));

                if (!this.FontWeightsAndStylesCombined.Contains(this.selectedWeightAndStyleCombined))
                {
                    this.SelectedWeightAndStyleCombined = this.FontWeightsAndStylesCombined.FirstOrDefault();
                }
            }
        }

        /// <summary>
        /// Gets the font weights and styles combined.
        /// </summary>
        /// <value>The font weights and styles combined.</value>
        public IEnumerable<object> FontWeightsAndStylesCombined
        {
            get
            {
                if (this.SelectedFontFamily == null)
                {
                    return new List<object>();
                }

                var objects = new List<object>();

                var allowedWeights = new[] { FontWeights.Bold };
                var allowedStyles = new[] { FontStyles.Italic, FontStyles.Normal };

                foreach (var typeFace in this.SelectedFontFamily.FamilyTypefaces)
                {
                    if (!objects.Contains(typeFace.Weight) && allowedWeights.Contains(typeFace.Weight))
                    {
                        objects.Add(typeFace.Weight);
                    }

                    if (!objects.Contains(typeFace.Style) && allowedStyles.Contains(typeFace.Style))
                    {
                        if (typeFace.Style == FontStyles.Normal)
                        {
                            objects.Insert(0, typeFace.Style);
                        }
                        else
                        {
                            objects.Add(typeFace.Style);
                        }
                    }
                }

                return objects;
            }
        }

        /// <summary>
        /// Gets the font families.
        /// </summary>
        /// <value>The font families.</value>
        public IEnumerable<FontFamily> FontFamilies
        {
            get
            {
                return Fonts.SystemFontFamilies.OrderBy(f => f.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the dialog result.
        /// </summary>
        /// <value>The dialog result.</value>
        public bool? DialogResult
        {
            get
            {
                return this.dialogResult;
            }

            set
            {
                this.dialogResult = value;
                this.NotifyPropertyChanged(nameof(this.DialogResult));
            }
        }

        /// <summary>
        /// Gets the font sizes.
        /// </summary>
        /// <value>The font sizes.</value>
        public IEnumerable<double> FontSizes
        {
            get
            {
                return new List<double> { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
            }
        }

        /// <summary>
        /// Gets or sets the font style.
        /// </summary>
        /// <value>The font style.</value>
        public FontStyle SelectedFontStyle
        {
            get
            {
                return this.selectedFontStyle;
            }

            set
            {
                this.selectedFontStyle = value;
                this.NotifyPropertyChanged(nameof(this.SelectedFontStyle));
            }
        }

        /// <summary>
        /// Gets or sets the font weight.
        /// </summary>
        /// <value>The font weight.</value>
        public FontWeight SelectedFontWeight
        {
            get
            {
                return this.selectedFontWeight;
            }

            set
            {
                this.selectedFontWeight = value;
                this.NotifyPropertyChanged(nameof(this.SelectedFontWeight));
            }
        }

        /// <summary>
        /// Gets the display size of the font.
        /// </summary>
        /// <value>The display size of the font.</value>
        public double DisplaySelectedFontSize
        {
            get
            {
                return this.SelectedFontSize / 0.75;
            }
        }

        /// <summary>
        /// Gets or sets the size of the selected font.
        /// </summary>
        /// <value>The size of the selected font.</value>
        public double SelectedFontSize
        {
            get
            {
                return this.selectedFontSize;
            }

            set
            {
                this.selectedFontSize = value;
                this.NotifyPropertyChanged(nameof(this.SelectedFontSize));
                this.NotifyPropertyChanged(nameof(this.DisplaySelectedFontSize));
            }
        }

        /// <summary>
        /// Gets or sets the selected weight and style combined.
        /// </summary>
        /// <value>The selected weight and style combined.</value>
        public object SelectedWeightAndStyleCombined
        {
            get
            {
                return this.selectedWeightAndStyleCombined;
            }

            set
            {
                if (value is FontWeight)
                {
                    this.SelectedFontWeight = (FontWeight)value;
                    this.SelectedFontStyle = FontStyles.Normal;
                }
                else if (value is FontStyle)
                {
                    this.SelectedFontStyle = (FontStyle)value;
                    this.SelectedFontWeight = FontWeights.Normal;
                }

                this.selectedWeightAndStyleCombined = value;
                this.NotifyPropertyChanged(nameof(this.SelectedWeightAndStyleCombined));
            }
        }

        /// <summary>
        /// Saves the changes and closes the dialog.
        /// </summary>
        public void SaveChanges()
        {
            ApplicationManager.Settings.FontFamily = this.SelectedFontFamily;
            ApplicationManager.Settings.FontSize = this.SelectedFontSize;
            ApplicationManager.Settings.FontWeight = this.SelectedFontWeight;
            ApplicationManager.Settings.FontStyle = this.SelectedFontStyle;

            if (this.SelectedWeightAndStyleCombined is FontStyle)
            {
                ApplicationManager.Settings.FontStyleType = "Style";
            }
            else if (this.SelectedWeightAndStyleCombined is FontWeight)
            {
                ApplicationManager.Settings.FontStyleType = "Weight";
            }
            else
            {
                ApplicationManager.Settings.FontStyleType = "Style";
            }

            ApplicationManager.SaveSettings();

            this.DialogResult = true;

            this.Close();
        }

        /// <summary>
        /// Closes the window.
        /// </summary>
        public void CloseWindow()
        {
            this.DialogResult = false;

            this.Close();
        }
    }
}
