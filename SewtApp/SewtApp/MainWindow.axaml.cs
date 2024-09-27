using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using System.Threading.Tasks;

namespace SewtApp
{
    public partial class MainWindow : Window
    {
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

                // Charger l'image depuis le flux
                var bitmap = new Bitmap(stream);

                // Trouver le contrôle Image
                var imageControl = this.FindControl<Image>("DisplayedImage")!;

                // Afficher l'image
                imageControl.Source = bitmap;
            }
        }

        private void SaveButton_Click(object? sender, RoutedEventArgs e)
        {
            // Code pour sauvegarder l'image
        }

        private void ExportButton_Click(object? sender, RoutedEventArgs e)
        {
            // Code pour exporter l'image
        }
    }
}
