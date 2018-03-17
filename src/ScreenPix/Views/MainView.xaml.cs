// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainView.xaml.cs" company="Fredrik Winkvist">
//   Copyright (c) Fredrik Winkvist. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for MainView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SwissTool.Ext.ScreenPix.Views
{
    using System;
    using System.Collections.Generic;
    using System.Drawing.Imaging;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Shapes;

    using DrawToolsLib;

    using SwissTool.Ext.ScreenPix.Managers;
    using SwissTool.Ext.ScreenPix.Utilities;
    using SwissTool.Ext.ScreenPix.ViewModels;
    using SwissTool.Framework.Infrastructure;
    using SwissTool.Framework.UI.Managers;

    using Color = System.Windows.Media.Color;
    using KeyEventArgs = System.Windows.Input.KeyEventArgs;
    using MessageBox = System.Windows.MessageBox;
    using MouseEventArgs = System.Windows.Input.MouseEventArgs;
    using Pen = System.Windows.Media.Pen;
    using Point = System.Windows.Point;
    using Rectangle = System.Windows.Shapes.Rectangle;
    using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
    using ToolBar = System.Windows.Controls.ToolBar;

    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        /// <summary>
        /// The point a drag action started at.
        /// </summary>
        private Point startDrag;

        /// <summary>
        /// Indicates whether the instance is loading.
        /// </summary>
        private bool loading;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainView"/> class.
        /// </summary>
        public MainView()
        {
            this.InitializeComponent();

            this.loading = false;
          
            var colorBrush = (Brush)(this.FindResource("BlackBrush") ?? new SolidColorBrush(Colors.Black));

            for (var i = 2; i <= 8; i++)
            {
                var pen = new Pen(colorBrush, i);
                var rect = new Ellipse { Stroke = pen.Brush, StrokeThickness = pen.Thickness };
                var item = new ComboBoxItem { Content = rect, Tag = i };
                this.comboPropertiesLineWidth.Items.Add(item);
            }

            var dashedPenLine = new Line
            {
                Stroke = colorBrush,
                StrokeThickness = 2,
                X1 = 0,
                Y1 = 0,
                X2 = 20,
                Y2 = 0
            };

            var dashes = new DoubleCollection { 2, 2 };
            dashedPenLine.StrokeDashArray = dashes;
            dashedPenLine.StrokeDashCap = PenLineCap.Round;

            var solidPenLine = new Line
            {
                Stroke = colorBrush,
                StrokeThickness = 2,
                X1 = 0,
                Y1 = 0,
                X2 = 20,
                Y2 = 0
            };

            var dashedPenItem = new ComboBoxItem { Content = dashedPenLine, Tag = LineStyleType.Dashed };
            var solidPenItem = new ComboBoxItem { Content = solidPenLine, Tag = LineStyleType.Solid };

            this.comboPropertiesLineStyle.Items.Add(solidPenItem);
            this.comboPropertiesLineStyle.Items.Add(dashedPenItem);

            var colors = new List<Color>
                                 {
                                     Colors.Black,
                                     Colors.White,
                                     Colors.Yellow,
                                     Colors.Lime,
                                     Colors.Purple,
                                     Colors.Orange,
                                     Colors.Blue,
                                     Colors.Gray,
                                     Colors.Red,
                                     Colors.Brown,
                                     Colors.Magenta
                                 };

            foreach (var color in colors)
            {
                var pen = new Pen(colorBrush, 1);
                var brush = new SolidColorBrush(color);
                var rect = new Rectangle
                {
                    Stroke = pen.Brush,
                    StrokeThickness = pen.Thickness,
                    Fill = brush,
                    Width = 30,
                    Height = 12
                };

                this.comboPropertiesLineColor.Items.Add(new ComboBoxItem
                {
                    Content = rect,
                    Tag = color
                });
            }
        }

        /// <summary>
        /// Gets the view model.
        /// </summary>
        public MainViewModel ViewModel
        {
            get { return this.DataContext as MainViewModel; }
        }

        /// <summary>
        /// Applies the settings.
        /// </summary>
        private void ApplySettings()
        {
            this.loading = true;
            
            try
            {
                this.drawingCanvas.Tool = ApplicationManager.Settings.Tool;
                this.RefreshDisplayMode();

                this.drawingCanvas.LineWidth = ApplicationManager.Settings.LineWidth;
                this.drawingCanvas.LineStyle = ApplicationManager.Settings.LineStyle;
                this.drawingCanvas.ObjectColor = ApplicationManager.Settings.LineColor;

                for (var i = 0; i < this.comboPropertiesLineWidth.Items.Count; i++)
                {
                    var lineWidth = double.Parse((this.comboPropertiesLineWidth.Items[i] as ComboBoxItem).Tag.ToString());

                    if (lineWidth == this.drawingCanvas.LineWidth)
                    {
                        this.comboPropertiesLineWidth.SelectedIndex = i;
                        break;
                    }
                }
                
                for (var i = 0; i < this.comboPropertiesLineStyle.Items.Count; i++)
                {
                    var lineStyle = (LineStyleType)(this.comboPropertiesLineStyle.Items[i] as ComboBoxItem).Tag;

                    if (lineStyle == this.drawingCanvas.LineStyle)
                    {
                        this.comboPropertiesLineStyle.SelectedIndex = i;
                        break;
                    }
                }

                for (var i = 0; i < this.comboPropertiesLineColor.Items.Count; i++)
                {
                    var color = (Color)(this.comboPropertiesLineColor.Items[i] as ComboBoxItem).Tag;

                    if (color == this.drawingCanvas.ObjectColor)
                    {
                        this.comboPropertiesLineColor.SelectedIndex = i;
                        break;
                    }
                }

                this.drawingCanvas.TextFontFamilyName = ApplicationManager.Settings.FontFamily.Source;
                this.drawingCanvas.TextFontSize = ApplicationManager.Settings.FontSize;
                this.drawingCanvas.TextFontStyle = ApplicationManager.Settings.FontStyle;
                this.drawingCanvas.TextFontWeight = ApplicationManager.Settings.FontWeight;
            }
            finally
            {
                this.loading = false;
            }
        }

        /// <summary>
        /// Buttons the tool preview mouse down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void ButtonToolPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.ChangeTool(((System.Windows.Controls.Primitives.ButtonBase)sender).Tag.ToString());

            e.Handled = true;
        }

        /// <summary>
        /// Buttons the properties font preview mouse down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void ButtonPropertiesFontPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var result = WindowManager.ShowDialog<FontDialog>(new FontDialogViewModel(), true, this);

            if (result.HasValue && result.Value)
            {
                this.ApplySettings();
            }
        }

        /// <summary>
        /// Changes the tool.
        /// </summary>
        /// <param name="toolKeyWord">The tool key word.</param>
        private void ChangeTool(string toolKeyWord)
        {
            var tool = (ToolType)Enum.Parse(typeof(ToolType), toolKeyWord);
            this.drawingCanvas.Tool = tool;

            ApplicationManager.Settings.Tool = tool;
            ApplicationManager.SaveSettings();

            this.RefreshDisplayMode();
        }

        /// <summary>
        /// Refreshes the display mode.
        /// </summary>
        private void RefreshDisplayMode()
        {
            switch (this.drawingCanvas.Tool)
            {
                case ToolType.Text:
                    this.ViewModel.DisplayDrawSettings = false;
                    this.ViewModel.DisplayFontSettings = true;
                    this.ViewModel.DisplayColorSettings = true;
                    break;
                case ToolType.Pointer:
                case ToolType.RectangularMarquee:
                    this.ViewModel.DisplayDrawSettings = false;
                    this.ViewModel.DisplayFontSettings = false;
                    this.ViewModel.DisplayColorSettings = false;
                    break;
                default:
                    this.ViewModel.DisplayDrawSettings = true;
                    this.ViewModel.DisplayFontSettings = false;
                    this.ViewModel.DisplayColorSettings = true;
                    break;
            }
        }

        /// <summary>
        /// Comboes the properties line width selection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void ComboPropertiesLineWidthSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.loading)
            {
                return;
            }

            var comboBoxItem = this.comboPropertiesLineWidth.SelectedValue as ComboBoxItem;
            if (comboBoxItem == null)
            {
                return;
            }

            var lineWidth = double.Parse(comboBoxItem.Tag.ToString());

            this.drawingCanvas.LineWidth = ApplicationManager.Settings.LineWidth = lineWidth;

            ApplicationManager.SaveSettings();
        }

        /// <summary>
        /// Comboes the properties line color selection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void ComboPropertiesLineColorSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.loading)
            {
                return;
            }

            var comboBoxItem = this.comboPropertiesLineColor.SelectedValue as ComboBoxItem;
            if (comboBoxItem == null)
            {
                return;
            }

            var color = (Color)comboBoxItem.Tag;

            this.drawingCanvas.ObjectColor = ApplicationManager.Settings.LineColor = color;

            ApplicationManager.SaveSettings();
        }

        /// <summary>
        /// Comboes the properties line style selection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void ComboPropertiesLineStyleSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.loading)
            {
                return;
            }

            var comboBoxItem = this.comboPropertiesLineStyle.SelectedValue as ComboBoxItem;
            if (comboBoxItem == null)
            {
                return;
            }

            var lineStyle = (LineStyleType)comboBoxItem.Tag;

            this.drawingCanvas.LineStyle = ApplicationManager.Settings.LineStyle = lineStyle;

            ApplicationManager.SaveSettings();
        }


        /// <summary>
        /// Handles the MouseMove event of the MainCanvas control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point currentPoint = e.GetPosition(this.MainCanvas);

            if (this.ToolbarMoveHandle.IsMouseCaptured)
            {
                Canvas.SetLeft(this.MainToolBar, currentPoint.X - this.startDrag.X);
                Canvas.SetTop(this.MainToolBar, currentPoint.Y - this.startDrag.Y);
            }
        }

        /// <summary>
        /// Handles the PreviewMouseLeftButtonUp event of the MainCanvas control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void MainCanvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.ToolbarMoveHandle.IsMouseCaptured)
            {
                this.ToolbarMoveHandle.ReleaseMouseCapture();
            }

            this.startDrag = new Point(-1, -1);
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the TextBlock control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.ToolbarMoveHandle.IsMouseCaptured)
            {
                this.ToolbarMoveHandle.CaptureMouse();
            }

            this.startDrag = new Point
                            {
                                X = e.GetPosition(this.MainToolBar).X,
                                Y = e.GetPosition(this.MainToolBar).Y
                            };
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the TextBlock control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.ToolbarMoveHandle.IsMouseCaptured)
            {
                this.ToolbarMoveHandle.ReleaseMouseCapture();
            }

            ApplicationManager.SaveSettings();
           
            this.startDrag = new Point(-1, -1);
        }

        /// <summary>
        /// Handles the Loaded event of the ToolBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            var toolBar = sender as ToolBar;
            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// News the button click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void NewButtonClick(object sender, RoutedEventArgs e)
        {
            this.ExecuteNewCommand();
        }

        /// <summary>
        /// Executes the new command.
        /// </summary>
        private void ExecuteNewCommand()
        {
            if (!this.drawingCanvas.IsDirty)
            {
                return;
            }

            var dlgRes = MessageBox.Show("Do you want to save changes?", "ScreenPix", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes);

            if (dlgRes == MessageBoxResult.Cancel)
            {
                return;
            }

            if (dlgRes == MessageBoxResult.Yes)
            {
                if (!this.ShowSaveFileDialog())
                {
                    return;
                }
            }

            this.drawingCanvas.Clear();
        }

        /// <summary>
        /// Saves the button click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            this.ShowSaveFileDialog();
        }
        
        /// <summary>
        /// Gets the image filter list.
        /// </summary>
        /// <param name="imageDecoders">The image decoders.</param>
        /// <returns>The image filter list.</returns>
        private List<string> GetImageFilterList(IEnumerable<ImageCodecInfo> imageDecoders)
        {
            return (from imageDecoder in imageDecoders
                    let extensions = imageDecoder.FilenameExtension.ToLower()
                    let extensionsDisplayString = extensions.Replace(";", "; ")
                    select $"{imageDecoder.FormatDescription} ({extensionsDisplayString})|{extensions}").ToList();   
        }

        /// <summary>
        /// Shows the save file dialog.
        /// </summary>
        /// <returns>A boolean value.</returns>
        private bool ShowSaveFileDialog()
        {
            var defaultExtension = ApplicationManager.Settings.DefaultFileExtension;

            var imageDecoders = ApplicationManager.GetSupportedImageDecoders();
            var filters = this.GetImageFilterList(imageDecoders);
            var extensionStr = defaultExtension.ToLower();
            var filterIndex = filters.FindIndex(f => f.IndexOf(extensionStr, StringComparison.OrdinalIgnoreCase) > -1);

            var dialog = new SaveFileDialog
            {
                Filter = string.Join("|", filters),
                DefaultExt = extensionStr,
                OverwritePrompt = true,
                AddExtension = true,
                FilterIndex = Math.Max(filterIndex + 1, 1)
            };

            var dlgRes = dialog.ShowDialog();

            if (!dlgRes.GetValueOrDefault() || string.IsNullOrEmpty(dialog.FileName))
            {
                return false;
            }

            var rect = this.drawingCanvas.CurrentSelection != null
                           ? this.drawingCanvas.CurrentSelection.Rectangle
                           : Rect.Empty;

            var quality = ApplicationManager.Settings.ImageQuality;

            ImageUtilities.SaveScreenImage(this.viewBoxContainer, 1, quality, dialog.FileName, rect.ToRectangle());

            return true;
        }

        /// <summary>
        /// Copies to clipboard button click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void CopyToClipboardButtonClick(object sender, RoutedEventArgs e)
        {
            this.ExecuteCopyCommand();
        }

        /// <summary>
        /// Executes the copy command.
        /// </summary>
        private void ExecuteCopyCommand()
        {
            var rect = this.drawingCanvas.CurrentSelection != null
                           ? this.drawingCanvas.CurrentSelection.Rectangle
                           : Rect.Empty;

            var dummyFilename = string.Format(@"{0}.{1}", "dummy", ApplicationManager.Settings.DefaultFileExtension);

            var encoder = ImageUtilities.GetBitmapEncoder(dummyFilename);
            var quality = ApplicationManager.Settings.ImageQuality;

            ImageUtilities.CopyScreenImageToClipboard(this.viewBoxContainer, 1, quality, rect.ToRectangle(), encoder);
        }

        /// <summary>
        /// Prints the button click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void PrintButtonClick(object sender, RoutedEventArgs e)
        {
            this.ExecutePrintCommand();
        }

        /// <summary>
        /// Executes the print command.
        /// </summary>
        private void ExecutePrintCommand()
        {
            using (new ApplicationWindowSession(this, true))
            {
                var rect = this.drawingCanvas.CurrentSelection != null
                           ? this.drawingCanvas.CurrentSelection.Rectangle
                           : Rect.Empty;

                ImageUtilities.PrintScreenImage(this.viewBoxContainer, 1, rect.ToRectangle());
            }
        }

        /// <summary>
        /// Exits the button click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Undoes the button click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void UndoButtonClick(object sender, RoutedEventArgs e)
        {
            this.drawingCanvas.Undo();
        }

        /// <summary>
        /// Redoes the button click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void RedoButtonClick(object sender, RoutedEventArgs e)
        {
            this.drawingCanvas.Redo();
        }

        /// <summary>
        /// Windows the key down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void WindowKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.N:
                        if (this.drawingCanvas.IsDirty)
                        {
                            this.ExecuteNewCommand();
                        }
                        break;
                    case Key.S:
                        this.ShowSaveFileDialog();
                        break;
                    case Key.C:
                        if (this.drawingCanvas.CanCopyToClipboard)
                        {
                            this.ExecuteCopyCommand();
                        }
                        break;
                    case Key.X:
                        if (this.drawingCanvas.CanCopyToClipboard)
                        {
                            this.ExecuteCopyCommand();
                            this.Close();
                        }
                        break;
                    case Key.Z:
                        if (this.drawingCanvas.CanUndo)
                        {
                            this.drawingCanvas.Undo();
                        }
                        break;
                    case Key.Y:
                        if (this.drawingCanvas.CanRedo)
                        {
                            this.drawingCanvas.Redo();
                        }
                        break;
                    case Key.P:
                        this.ExecutePrintCommand();
                        break;
                    case Key.A:
                        this.drawingCanvas.SelectFullScreen();
                        break;
                }
            }
            else
            {
                switch (e.Key)
                {
                    case Key.D1:
                        this.ChangeTool("Pointer");
                        break;
                    case Key.D2:
                        this.ChangeTool("RectangularMarquee");
                        break;
                    case Key.D3:
                        this.ChangeTool("Rectangle");
                        break;
                    case Key.D4:
                        this.ChangeTool("Ellipse");
                        break;
                    case Key.D5:
                        this.ChangeTool("Line");
                        break;
                    case Key.D6:
                        this.ChangeTool("PolyLine");
                        break;
                    case Key.D7:
                        this.ChangeTool("Text");
                        break;
                    case Key.Delete:
                        this.drawingCanvas.Delete();
                        break;
                    case Key.Escape:
                        if (this.drawingCanvas.CurrentSelection != null)
                        {
                            this.drawingCanvas.ClearSelection();
                        }
                        else if (this.drawingCanvas.HasSelection())
                        {
                            this.drawingCanvas.UnselectAll();
                        }
                        else
                        {
                            this.Close();
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Occurs when the window is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
#if DEBUG
            this.Topmost = false;
#endif
            this.ApplySettings();
        }
    }
}
