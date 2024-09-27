using Google.Cloud.Vision.V1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProtoSewt
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @"C:\Users\Odyce\Desktop\Work\prototype-sewt-436908-d52bd6896960.json");
        }

        private void imgUpldBtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Bitmap image = new Bitmap(openFileDialog.FileName);

                    imgUplddDisp.Width = image.Width;
                    imgUplddDisp.Height = image.Height;
                    imgUplddDisp.Image = image;
                    imgUplddDisp.Left = (this.ClientSize.Width - imgUplddDisp.Width) / 2;
                    txtXtrBtn.Left = (imgUplddDisp.Width + imgUplddDisp.Left) + 10;
                }
            }
        }

        private void txtXtrBtn_Click(object sender, EventArgs e)
        {
            if (imgUplddDisp.Image == null)
            {
                MessageBox.Show("Please load an image first.");
                return;
            }

            // Convert the PictureBox image to a byte array
            var imageFilePath = ConvertPictureBoxImageToFile();

            // Call Google Vision API to extract text from the image
            ExtractTextFromImage(imageFilePath);
        }

        // Function to convert PictureBox image to a file and return the file path
        private string ConvertPictureBoxImageToFile()
        {
            string tempImagePath = Path.Combine(Path.GetTempPath(), "temp_image.png");

            // Save PictureBox image to a temporary file
            imgUplddDisp.Image.Save(tempImagePath, System.Drawing.Imaging.ImageFormat.Png);

            return tempImagePath;
        }

        // Function to call Google Vision API to extract text
        private void ExtractTextFromImage(string imageFilePath)
        {
            try
            {
                // Create a Vision API client
                var client = ImageAnnotatorClient.Create();

                // Load the image from the file path
                var image = Google.Cloud.Vision.V1.Image.FromFile(imageFilePath);

                // Call the Vision API to detect text
                var response = client.DetectText(image);

                // Display the extracted text
                string extractedText = string.Empty;
                foreach (var annotation in response)
                {
                    if (annotation.Description != null)
                    {
                        extractedText += annotation.Description + "\n";
                    }
                }

                // Show the extracted text in a message box
                MessageBox.Show(extractedText, "Extracted Text");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        }
    }
}