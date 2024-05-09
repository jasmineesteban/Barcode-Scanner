using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Price_Checker.Services
{
    internal class ImagesManagerService
    {
        private Queue<string> imageQueue = new Queue<string>();
        private Dictionary<string, string> imagePaths = new Dictionary<string, string>();


        private System.Windows.Forms.Timer imageLoopTimer;
        private System.Windows.Forms.PictureBox pictureBox1;

        public ImagesManagerService(System.Windows.Forms.PictureBox pictureBox)
        {
            this.pictureBox1 = pictureBox;
        }

        public void ImageSlideshow()
        {
            imageLoopTimer = new System.Windows.Forms.Timer();
            imageLoopTimer.Interval = 5000; // 5 seconds
            imageLoopTimer.Tick += DisplayNextImage;
            imageLoopTimer.Start();

            // Initialize image file paths
            string commonPath = @"C:\ESTEBAN_JASMINE_PUPSMB\C#\Barcode-Scanner\PriceScannerV1\Price Checker\assets\Images";
            // Add image paths to the Dictionary



            imagePaths.Add("002_", Path.Combine(commonPath, "002_Adds.jpg"));
            imagePaths.Add("003_", Path.Combine(commonPath, "003_Adds.jpg"));
            imagePaths.Add("001_", Path.Combine(commonPath, "001_Adds.jpg"));

            // Populate the imageQueue with the image file paths
            var sortedImagePaths = imagePaths.OrderBy(item => item.Key).ToDictionary(item => item.Key, item => item.Value);

            // Populate the imageQueue with the sorted image file paths
            foreach (string imagePath in sortedImagePaths.Values)
            {
                imageQueue.Enqueue(imagePath);
            }
            // Display the first image
            DisplayNextImage(null, EventArgs.Empty);
        }

        public void DisplayNextImage(object sender, EventArgs e)
        {
            if (imageQueue.Count == 0)
            {
                // Repopulate the imageQueue with the image file paths
                imageQueue = new Queue<string>(imagePaths.Values);
            }

            if (imageQueue.Count > 0)
            {
                string imagePath = imageQueue.Dequeue();
                pictureBox1.Image = Image.FromFile(imagePath);
                imageQueue.Enqueue(imagePath); // Add the image back to the end of the queue
            }
        }
    }
}
