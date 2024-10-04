using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using System.Threading.Tasks;
using System.IO;
using Avalonia.Input;
using Avalonia.Media;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Bmp;
using AvaloniaImage = Avalonia.Controls.Image;

namespace SewtApp
{
    public partial class MainWindow : Window
    {
        // Image and Transform properties
        private SixLabors.ImageSharp.Image<Rgba32>? _currentImage;
        private double _scaleFactor = 1.0;
        private double _rotationAngle = 0.0;
        private Point _startPanPoint;
        private TranslateTransform _translateTransform = new TranslateTransform();

        public MainWindow()
        {
            InitializeComponent();

            // Initialize buttons
            var openButton = this.FindControl<Button>("OpenButton")!;
            var saveButton = this.FindControl<Button>("SaveButton")!;
            var exportButton = this.FindControl<Button>("ExportButton")!;
            
            // Assign button click events
            openButton.Click += OpenButton_Click;
            saveButton.Click += SaveButton_Click;
            exportButton.Click += ExportButton_Click;
        }

        // Open image and set up transforms
        private async void OpenButton_Click(object? sender, RoutedEventArgs e)
        {
            var filters = new FilePickerFileType[]
            {
                new FilePickerFileType("Images")
                {
                    Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.bmp" }
                }
            };

            var options = new FilePickerOpenOptions
            {
                Title = "Ouvrir une image",
                FileTypeFilter = filters,
                AllowMultiple = false
            };

            var result = await this.StorageProvider.OpenFilePickerAsync(options);

            if (result != null && result.Count > 0)
            {
                var file = result[0];
                var stream = await file.OpenReadAsync();

                // Load the image using ImageSharp
                _currentImage = await SixLabors.ImageSharp.Image.LoadAsync<Rgba32>(stream);

                // Convert to Bitmap and display it
                stream.Seek(0, SeekOrigin.Begin);
                var bitmap = new Bitmap(stream);

                var imageControl = this.FindControl<AvaloniaImage>("DisplayedImage");
                if (imageControl != null)
                {
                    imageControl.Source = bitmap;

                    // Reset transform properties
                    _scaleFactor = 1.0;
                    _rotationAngle = 0.0;
                    _translateTransform = new TranslateTransform();
                    UpdateImageTransform();
                }
            }
        }

        // Save the image to the selected format
        private async void SaveButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_currentImage == null)
            {
                await ShowMessageAsync("Aucune image à sauvegarder.", "Erreur");
                return;
            }

            var filters = new FilePickerFileType[]
            {
                new FilePickerFileType("PNG") { Patterns = new[] { "*.png" } },
                new FilePickerFileType("JPEG") { Patterns = new[] { "*.jpg", "*.jpeg" } },
                new FilePickerFileType("BMP") { Patterns = new[] { "*.bmp" } }
            };

            var options = new FilePickerSaveOptions
            {
                Title = "Sauvegarder l'image",
                FileTypeChoices = filters,
                DefaultExtension = "png",
                ShowOverwritePrompt = true
            };

            var result = await this.StorageProvider.SaveFilePickerAsync(options);

            if (result != null)
            {
                var filePath = result.Path.LocalPath;
                var extension = Path.GetExtension(filePath).ToLower();

                await using var stream = await result.OpenWriteAsync();

                switch (extension)
                {
                    case ".png":
                        await _currentImage.SaveAsync(stream, new PngEncoder());
                        break;
                    case ".jpg":
                    case ".jpeg":
                        await _currentImage.SaveAsync(stream, new JpegEncoder());
                        break;
                    case ".bmp":
                        await _currentImage.SaveAsync(stream, new BmpEncoder());
                        break;
                    default:
                        await ShowMessageAsync("Format de fichier non supporté.", "Erreur");
                        break;
                }
            }
        }
        
        private async void ExportButton_Click(object? sender, RoutedEventArgs e)
        {
            // Code pour exporter l'image
        }

        // Zoom functionality with mouse wheel
// Zoom functionality with mouse wheel and Ctrl
        private void OnPointerWheelChanged(object sender, PointerWheelEventArgs e)
        {
            var scrollViewer = this.FindControl<ScrollViewer>("ImageScrollViewer");
            if (scrollViewer == null) return;

            if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
            {
                // Zooming with Ctrl + Mouse Wheel
                var position = e.GetPosition(this.FindControl<AvaloniaImage>("DisplayedImage"));
        
                // Adjust scale factor based on the scroll direction
                _scaleFactor *= (e.Delta.Y > 0) ? 1.1 : 0.9;
        
                // Set the zoom origin to where the mouse pointer is
                this.FindControl<AvaloniaImage>("DisplayedImage").RenderTransformOrigin = new RelativePoint(position, RelativeUnit.Absolute);
        
                // Update the image with the new zoom level
                UpdateImageTransform();
        
                // Mark the event as handled (prevents further propagation)
                e.Handled = true;
            }
            else
            {
                // Scrolling without Ctrl key pressed - only scroll the image up and down
                double newOffsetY = scrollViewer.Offset.Y - e.Delta.Y * 50; // Adjust scrolling speed with factor (50)
        
                // Update the scroll position
                scrollViewer.Offset = new Point(scrollViewer.Offset.X, newOffsetY);
        
                // Mark the event as handled
                e.Handled = true;
            }
        }

        
        private void OnPointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                _startPanPoint = e.GetPosition(this);
                var displayedImage = this.FindControl<AvaloniaImage>("DisplayedImage");
                if (displayedImage != null)
                {
                    displayedImage.PointerMoved += OnPointerMoved;
                    displayedImage.PointerReleased += OnPointerReleased;
                }
            }
        }
        
        private void OnPointerMoved(object sender, PointerEventArgs e)
        {
            if (_currentImage == null) return;

            var currentPoint = e.GetPosition(this);
            var offsetX = currentPoint.X - _startPanPoint.X;
            var offsetY = currentPoint.Y - _startPanPoint.Y;

            _translateTransform.X += offsetX;
            _translateTransform.Y += offsetY;

            UpdateImageTransform();
            _startPanPoint = currentPoint;
        }

        private void OnPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            var displayedImage = this.FindControl<AvaloniaImage>("DisplayedImage");
            if (displayedImage != null)
            {
                displayedImage.PointerMoved -= OnPointerMoved;
                displayedImage.PointerReleased -= OnPointerReleased;
            }
        }

        // Update the image's transform (scaling, panning, and rotation)
        private void UpdateImageTransform()
        {
            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(_translateTransform); // Panning
            transformGroup.Children.Add(new RotateTransform(_rotationAngle)); // Rotation
            transformGroup.Children.Add(new ScaleTransform(_scaleFactor, _scaleFactor)); // Zoom

            var displayedImage = this.FindControl<AvaloniaImage>("DisplayedImage");
            if (displayedImage != null)
            {
                displayedImage.RenderTransform = transformGroup;
            }
        }

        // Rotate the image by a given angle
        private void RotateImage(double angle)
        {
            _rotationAngle += angle;
            UpdateImageTransform();
        }

        // Display a message dialog to the user
        private async Task ShowMessageAsync(string message, string title)
        {
            var okButton = new Button
            {
                Content = "OK",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 0),
            };

            var dialog = new Window
            {
                Title = title,
                Content = new StackPanel
                {
                    Children =
                    {
                        new TextBlock { Text = message, Margin = new Thickness(20) },
                        okButton
                    }
                },
                Width = 300,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            okButton.Click += (_, __) => dialog.Close();

            await dialog.ShowDialog(this);
        }
    }
}
