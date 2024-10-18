using Google.Cloud.Vision.V1;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProtoSewt
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @"C:\Users\Odyce\Documents\Work Doc\Sewt\prototype-sewt-436908-d52bd6896960.json");
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
                    txtXtrBtn.Left = (imgUplddDisp.Width + imgUplddDisp.Left) + 12;
                    listBoxFonts.Left = (imgUplddDisp.Width + imgUplddDisp.Left) + 12;
                    txtBoxRes.Left = (imgUplddDisp.Left - txtBoxRes.Width) - 12;
                    imgUpldBtn.Left = (this.ClientSize.Width - imgUpldBtn.Width) / 2;
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
            Task task = ExtractTextWithBoundingBoxesAsync(imageFilePath);
        }

        // Function to convert PictureBox image to a file and return the file path
        private string ConvertPictureBoxImageToFile()
        {
            string tempImagePath = Path.Combine(Path.GetTempPath(), "temp_image.png");

            // Save PictureBox image to a temporary file
            imgUplddDisp.Image.Save(tempImagePath, System.Drawing.Imaging.ImageFormat.Png);

            return tempImagePath;
        }

        // Function to call Google Vision API to extract text and bounding boxes
        private async Task ExtractTextWithBoundingBoxesAsync(string imageFilePath)
        {
            try
            {
                // Create a Vision API client
                var client = ImageAnnotatorClient.Create();

                // Load the image from the file path
                var image = Google.Cloud.Vision.V1.Image.FromFile(imageFilePath);

                // Call the Vision API to detect text
                var response = client.DetectText(image);

                // List to store text and bounding boxes
                var textAndBoundingBoxes = new List<(string Text, List<Point> BoundingBox)>();

                // Process each detected text annotation
                foreach (var annotation in response)
                {
                    if (!string.IsNullOrEmpty(annotation.Description))
                    {
                        // Extract the detected text
                        string detectedText = annotation.Description;

                        // Extract the bounding box vertices
                        var vertices = annotation.BoundingPoly.Vertices;
                        List<Point> boundingBox = new List<Point>();

                        foreach (var vertex in vertices)
                        {
                            boundingBox.Add(new Point(vertex.X, vertex.Y));
                        }

                        // Add text and its bounding box to the list
                        textAndBoundingBoxes.Add((detectedText, boundingBox));
                    }
                }

                // Display the extracted text and bounding boxes (for example, in a message box)
                string result = "";
                foreach (var item in textAndBoundingBoxes)
                {
                    result += $"Text: {item.Text}\r\n";
                    result += "Bounding Box: ";
                    foreach (var point in item.BoundingBox)
                    {
                        result += $"({point.X}, {point.Y}) ";
                    }
                    result += "\r\n\r\n";
                }

                txtBoxRes.Text = result;

                // List to store unique fonts detected
                var uniqueFonts = new HashSet<string>();

                // Clear the font list on UI
                listBoxFonts.Items.Clear();

                // Process each text bounding box
                foreach (var (detectedText, boundingBox) in textAndBoundingBoxes)
                {
                    // Crop the image based on bounding box
                    Bitmap croppedImage = CropImageByBoundingBox(imageFilePath, boundingBox);

                    // Call the Font Recognition API
                    string detectedFont = await DetectFontFromImage(croppedImage);

                    // If the font is detected and it's not already in the list, add it
                    if (!string.IsNullOrEmpty(detectedFont) && !uniqueFonts.Contains(detectedFont))
                    {
                        uniqueFonts.Add(detectedFont);
                        listBoxFonts.Items.Add(detectedFont); // Display font on UI
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        }

        // Function to crop image by bounding box coordinates
        private Bitmap CropImageByBoundingBox(string imageFilePath, List<Point> boundingBox)
        {
            using (var image = new Bitmap(imageFilePath))
            {
                // Get the bounding box rectangle
                int minX = boundingBox.Min(p => p.X);
                int minY = boundingBox.Min(p => p.Y);
                int width = boundingBox.Max(p => p.X) - minX;
                int height = boundingBox.Max(p => p.Y) - minY;

                // Create a cropped image
                Rectangle cropRect = new Rectangle(minX, minY, width, height);
                Bitmap croppedImage = new Bitmap(cropRect.Width, cropRect.Height);

                using (Graphics g = Graphics.FromImage(croppedImage))
                {
                    g.DrawImage(image, new Rectangle(0, 0, croppedImage.Width, croppedImage.Height), cropRect, GraphicsUnit.Pixel);
                }

                return croppedImage;
            }
        }

        // Hypothetical function to detect the font using a Font Recognition API
        private async Task<string> DetectFontFromImage(Bitmap croppedImage)
        {
            // Convert image to byte array
            byte[] imageBytes;
            using (MemoryStream ms = new MemoryStream())
            {
                croppedImage.Save(ms, ImageFormat.Png);
                imageBytes = ms.ToArray();
            }

            // Call your Font Recognition API (using a placeholder URL)
            using (HttpClient client = new HttpClient())
            {
                var content = new ByteArrayContent(imageBytes);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                // Replace with your Font Recognition API endpoint
                var response = await client.PostAsync("https://api.fontrecognition.com/detect", content);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    // Parse the response (assuming it returns a JSON with font information)
                    var fontResult = System.Text.Json.JsonSerializer.Deserialize<FontRecognitionResponse>(jsonResponse);
                    return fontResult?.FontName;
                }
            }

            return null; // Return null if the font couldn't be detected
        }

        // Hypothetical response structure for the Font Recognition API
        private class FontRecognitionResponse
        {
            public string FontName { get; set; }
        }
    }
}
