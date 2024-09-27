using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Layout;
using Avalonia.VisualTree;
using Avalonia;

namespace GetStartedApp.Views
{
    public partial class MainWindow : Window
    {
        private double _scaleFactor = 1.0; // Default scale factor

        public MainWindow()
        {
            InitializeComponent();
        }

        public async void PictureSelectionFunction(object sender, RoutedEventArgs args)
        {
            var dialog = new OpenFileDialog()
            {
                AllowMultiple = false,
                Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter
                    {
                        Name = "Images",
                        Extensions = { "jpg", "jpeg", "png" }
                    }
                }
            };

            var result = await dialog.ShowAsync(this);
            if (result != null && result.Length > 0)
            {
                var imagePath = result[0];

                if (File.Exists(imagePath))
                {
                    var bitmap = new Bitmap(imagePath);
                    SelectedImage.Source = bitmap;

                    // Reset zoom on new image load
                    _scaleFactor = 1.0;
                    UpdateImageScale();
                }
            }
        }

        private void OnPointerWheelChangedMethod(object sender, PointerWheelEventArgs e)
        {
            // Check if Ctrl key is pressed for zooming
            if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
            {
                // Zoom in or out based on the scroll delta
                if (e.Delta.Y > 0)
                {
                    _scaleFactor *= 1.1; // Zoom in
                }
                else
                {
                    _scaleFactor /= 1.1; // Zoom out
                }

                // Update the image scale
                UpdateImageScale();

                // Prevent vertical scrolling
                e.Handled = true;
            }
            else
            {
                // Allow vertical scrolling if Ctrl is not pressed
                var scrollViewer = this.FindControl<ScrollViewer>("ImageScrollViewer");
                if (scrollViewer != null)
                {
                    double newOffsetY = scrollViewer.Offset.Y - e.Delta.Y; // Adjust based on scroll delta
                    scrollViewer.Offset = new Point(scrollViewer.Offset.X, newOffsetY);
                }
            }
        }

        private void UpdateImageScale()
        {
            if (SelectedImage.Source is Bitmap)
            {
                // Apply the scale transform
                SelectedImage.RenderTransform = new ScaleTransform(_scaleFactor, _scaleFactor);

                // Optional: Set the image's alignment correctly
                SelectedImage.HorizontalAlignment = HorizontalAlignment.Center;
                SelectedImage.VerticalAlignment = VerticalAlignment.Center;
            }
        }


    }
}
