using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using System.Threading.Tasks;
using System.IO;

// Alias pour les classes Image
using AvaloniaImage = Avalonia.Controls.Image;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Bmp;


namespace SewtApp
{
    public partial class MainWindow : Window
    {
        // Variable pour stocker l'image chargée
        private SixLabors.ImageSharp.Image<Rgba32>? _currentImage;

        public MainWindow()
        {
            InitializeComponent();

            // Trouver les boutons par leur nom
            var openButton = this.FindControl<Button>("OpenButton")!;
            var saveButton = this.FindControl<Button>("SaveButton")!;
            var exportButton = this.FindControl<Button>("ExportButton")!;

            // Associer les événements
            openButton.Click += OpenButton_Click;
            saveButton.Click += SaveButton_Click;
            exportButton.Click += ExportButton_Click;
        }

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

                // Charger l'image en utilisant ImageSharp
                _currentImage = await SixLabors.ImageSharp.Image.LoadAsync<Rgba32>(stream);

                // Convertir l'image en Bitmap pour l'affichage
                stream.Seek(0, SeekOrigin.Begin); // Réinitialiser le flux pour le Bitmap
                var bitmap = new Bitmap(stream);

                // Trouver le contrôle Image
                var imageControl = this.FindControl<AvaloniaImage>("DisplayedImage")!;

                // Afficher l'image
                imageControl.Source = bitmap;
            }
        }

        private async void SaveButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_currentImage == null)
            {
                // Aucune image à sauvegarder
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

                // Ouvrir un flux pour écrire l'image
                await using var stream = await result.OpenWriteAsync();

                // Sauvegarder l'image en fonction du format choisi
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

        private void ExportButton_Click(object? sender, RoutedEventArgs e)
        {
            // Code pour exporter l'image
        }

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
