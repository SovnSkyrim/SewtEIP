using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;
using System.IO;
using Avalonia.Media.Imaging;
using System.Threading.Tasks;

namespace SewtApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        // Méthode pour ouvrir une image
        private async void Open_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                AllowMultiple = false,
                // Les filtres d'extension sont maintenant gérés différemment
                Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter() { Name = "Images", Extensions = { "jpg", "jpeg", "png", "bmp" } }
                }
            };

            var result = await dialog.ShowAsync(this);
            if (result != null && result.Length > 0)
            {
                var imagePath = result[0];
                // Charger l'image dans le contrôle Image
                var bitmap = new Bitmap(imagePath);
                MainImage.Source = bitmap;
            }
        }

        // Méthode pour sauvegarder une image
        private async void Save_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog()
            {
                // Les filtres d'extension sont maintenant gérés différemment
                Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter() { Name = "PNG", Extensions = { "png" } },
                    new FileDialogFilter() { Name = "JPEG", Extensions = { "jpg", "jpeg" } },
                    new FileDialogFilter() { Name = "BMP", Extensions = { "bmp" } }
                }
            };

            var result = await dialog.ShowAsync(this);
            if (!string.IsNullOrEmpty(result))
            {
                // Sauvegarde l'image ici (implémente cette partie)
            }
        }

        // Méthode pour quitter l'application
        private void Exit_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close();
        }
    }
}
