using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using System.Collections.Generic;
using System.IO;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia;

namespace GetStartedApp.Views
{
    public partial class MainWindow : Window
    {
        private double _scaleFactor = 1.0; // Default scale factor
        private double _rotationAngle = 0.0; // Default rotation angle
        private Point _startPanPoint; // For tracking panning
        private TranslateTransform _translateTransform = new TranslateTransform(); // For panning

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
                    _scaleFactor = 1.0;
                    _rotationAngle = 0.0;
                    _translateTransform = new TranslateTransform();
                    UpdateImageTransform();
                }
            }
        }

        private void OnPointerWheelChangedMethod(object sender, PointerWheelEventArgs e)
        {
            var scrollViewer = this.FindControl<ScrollViewer>("ImageScrollViewer");
            if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
            {
                var position = e.GetPosition(SelectedImage);
                _scaleFactor *= (e.Delta.Y > 0) ? 1.1 : 0.9;
                SelectedImage.RenderTransformOrigin = new RelativePoint(position, RelativeUnit.Absolute);
                UpdateImageTransform();
                e.Handled = true;
            }
            else
            {
                // Allow vertical scrolling when Ctrl is not pressed
                if (scrollViewer != null)
                {
                    // Adjust based on scroll delta
                    double newOffsetY = scrollViewer.Offset.Y - e.Delta.Y * 50; // Increase delta sensitivity as needed
                    scrollViewer.Offset = new Point(scrollViewer.Offset.X, newOffsetY);
                    e.Handled = true;
                }
            }
        }

        private void OnPointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                _startPanPoint = e.GetPosition(this);
                SelectedImage.PointerMoved += OnPointerMoved;
                SelectedImage.PointerReleased += OnPointerReleased;
            }
        }

        private void OnPointerMoved(object sender, PointerEventArgs e)
        {
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
            SelectedImage.PointerMoved -= OnPointerMoved;
            SelectedImage.PointerReleased -= OnPointerReleased;
        }

        private void UpdateImageTransform()
        {
            // Combine translation, rotation, and scaling into a TransformGroup
            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(_translateTransform); // Panning
            transformGroup.Children.Add(new RotateTransform(_rotationAngle)); // Rotation
            transformGroup.Children.Add(new ScaleTransform(_scaleFactor, _scaleFactor)); // Zoom

            SelectedImage.RenderTransform = transformGroup;
        }

        // Call this method when you want to rotate the image (e.g., via a button click)
        private void RotateImage(double angle)
        {
            _rotationAngle += angle;
            UpdateImageTransform();
        }
    }
}
