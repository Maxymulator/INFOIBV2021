using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static INFOIBV.Utils;


namespace INFOIBV
{
    public partial class INFOIBV : Form
    {
        // Create the global constants for this operation
        // Added so all changes can be made in one place
        private const byte FilterSize = 3;
        private const byte GreyscaleThreshold = 160;
        private const byte HoughPeakThreshold = 65;
        private const int CrossingThreshold = 1;
        private const int MinLineLength = 15;
        private const int MaxLineGap = 5;
        private const int minimumIntesityThreshold = 100;
        private const double rMin = 20;
        private const double rMax = 40;
        private const int stepsPerR = 2;
        private const int margeCircles = 50;
        
        private static readonly Color CircleColor = Color.Blue;
        private static readonly Color FullLineColor = Color.Red;
        private static readonly Color LineSegmentColor = Color.Lime;
        private static readonly Color CrossingColor = Color.BlueViolet;
        private static readonly Color HpGlassesColor = Color.Yellow;
        
        private Bitmap InputImage;
        private Bitmap OutputImage;
        
        public INFOIBV()
        {
            InitializeComponent();
        }

        /*
         * loadButton_Click: process when user clicks "Load" button
         */
        private void loadImageButton_Click(object sender, EventArgs e)
        {
            if (openImageDialog.ShowDialog() == DialogResult.OK) // open file dialog
            {
                string file = openImageDialog.FileName; // get the file name
                imageFileName.Text = file; // show file name
                if (InputImage != null) InputImage.Dispose(); // reset image
                InputImage = new Bitmap(file); // create new Bitmap from file
                if (InputImage.Size.Height <= 0 || InputImage.Size.Width <= 0 ||
                    InputImage.Size.Height > 512 ||
                    InputImage.Size.Width > 512) // dimension check (may be removed or altered)
                    MessageBox.Show("Error in image dimensions (have to be > 0 and <= 512)");
                else
                    pictureBox1.Image = InputImage; // display input image
            }
        }

        /// <summary>
        /// process when user clicks the pipeline 1 button
        /// </summary>
        private void buttonPipeline1_Click(object sender, EventArgs e)
        {
            if (InputImage == null) return; // get out if no input image
            if (OutputImage != null) OutputImage.Dispose(); // reset output image
            OutputImage = new Bitmap(InputImage.Size.Width, InputImage.Size.Height); // create new output image
            Color[,]
                Image = new Color[InputImage.Size.Width,
                    InputImage.Size.Height]; // create array to speed-up operations (Bitmap functions are very slow)

            // copy input Bitmap to array            
            for (int x = 0; x < InputImage.Size.Width; x++) // loop over columns
            for (int y = 0; y < InputImage.Size.Height; y++) // loop over rows
                Image[x, y] = InputImage.GetPixel(x, y); // set pixel color in array at (x,y)

            // call pipeline 1
            // convert image to grayscale
            byte[,] workingImage = convertToGrayscale(Image);

            // adjust the contrast
            workingImage = adjustContrast(workingImage);

            // apply gaussian filter
            workingImage = convolveImageParallel(workingImage, createGaussianFilter(5, 10f));

            // apply edge detection
            workingImage = edgeMagnitude(workingImage);

            // apply a threshold
            workingImage = thresholdImageToBinaryParallel(workingImage, 80).GetImage();

            // copy array to output Bitmap
            for (int x = 0; x < workingImage.GetLength(0); x++) // loop over columns
            for (int y = 0; y < workingImage.GetLength(1); y++) // loop over rows
            {
                Color newColor = Color.FromArgb(workingImage[x, y], workingImage[x, y], workingImage[x, y]);
                OutputImage.SetPixel(x, y, newColor); // set the pixel color at coordinate (x,y)
            }

            pictureBox2.Image = OutputImage; // display output image
        }

        /// <summary>
        /// process when user clicks the pipeline 2 button
        /// </summary>
        private void buttonPipeline2_Click(object sender, EventArgs e)
        {
            if (InputImage == null) return; // get out if no input image
            if (OutputImage != null) OutputImage.Dispose(); // reset output image
            OutputImage = new Bitmap(InputImage.Size.Width, InputImage.Size.Height); // create new output image
            Color[,]
                Image = new Color[InputImage.Size.Width,
                    InputImage.Size.Height]; // create array to speed-up operations (Bitmap functions are very slow)

            // copy input Bitmap to array            
            for (int x = 0; x < InputImage.Size.Width; x++) // loop over columns
            for (int y = 0; y < InputImage.Size.Height; y++) // loop over rows
                Image[x, y] = InputImage.GetPixel(x, y); // set pixel color in array at (x,y)

            // call pipeline 2
            // convert image to grayscale
            byte[,] workingImage = convertToGrayscale(Image);

            // adjust the contrast
            workingImage = adjustContrast(workingImage);

            // apply median filter
            workingImage = medianFilterParallel(workingImage, 5);

            // apply edge detection
            workingImage = edgeMagnitude(workingImage);

            // apply a threshold
            workingImage = thresholdImageToBinaryParallel(workingImage, 80).GetImage();

            // copy array to output Bitmap
            for (int x = 0; x < workingImage.GetLength(0); x++) // loop over columns
            for (int y = 0; y < workingImage.GetLength(1); y++) // loop over rows
            {
                Color newColor = Color.FromArgb(workingImage[x, y], workingImage[x, y], workingImage[x, y]);
                OutputImage.SetPixel(x, y, newColor); // set the pixel color at coordinate (x,y)
            }

            pictureBox2.Image = OutputImage; // display output image
        }

        /*
         * applyButton_Click: process when user clicks "Apply" button
         */
        private void applyButton_Click(object sender, EventArgs e)
        {
            if (InputImage == null) return; // get out if no input image
            if (OutputImage != null) OutputImage.Dispose(); // reset output image
            OutputImage = new Bitmap(InputImage.Size.Width, InputImage.Size.Height); // create new output image
            Color[,]
                Image = new Color[InputImage.Size.Width,
                    InputImage.Size.Height]; // create array to speed-up operations (Bitmap functions are very slow)

            // copy input Bitmap to array            
            for (int x = 0; x < InputImage.Size.Width; x++) // loop over columns
            for (int y = 0; y < InputImage.Size.Height; y++) // loop over rows
                Image[x, y] = InputImage.GetPixel(x, y); // set pixel color in array at (x,y)

            // ====================================================================
            // =================== YOUR FUNCTION CALLS GO HERE ====================
            // Alternatively you can create buttons to invoke certain functionality
            // ====================================================================

            // convert image to grayscale
            byte[,] workingImage = convertToGrayscale(Image);

            // invert Image
            workingImage = invertImage(workingImage);

            // adjust the contrast
            workingImage = adjustContrast(workingImage);

            // apply median filter
            workingImage = medianFilterParallel(workingImage, 3);

            //apply closing
            workingImage = closeImage(workingImage, createStructuringElement(StructuringElementShape.Square, 3));

            // apply edge detection
            workingImage = edgeMagnitude(workingImage);


            // apply a threshold
            workingImage = thresholdImage(workingImage, 254);

            //apply closing
            workingImage = closeImage(workingImage,createStructuringElement(StructuringElementShape.Plus,5));



            //workingImage = closeImage(workingImage, createStructuringElement(StructuringElementShape.Square, 3));
            //workingImage = openImage(workingImage, createStructuringElement(StructuringElementShape.Square, 3));
            List<Circle> circles = peakFindingCircle(new BinaryImage(workingImage));

            //workingImage = thresholdImage(workingImage, 255);
            // apply the hough transform
            List<Point> centers = peakFinding(new BinaryImage(workingImage), HoughPeakThreshold);
            List<LineSegment> line = new List<LineSegment>();
            foreach (var center in centers)
            {
                line.AddRange(houghLineDetection(new BinaryImage(workingImage), center, MinLineLength, MaxLineGap));
            }

            circles = pruneCircleList(circles, 10, 10);
            List<HPGlasses> found2 = findConnectedCircles(circles, line, 7d);
            
            line = pruneLineSegments(line);

            // ==================== END OF YOUR FUNCTION CALLS ====================
            // ====================================================================
            // Create the output bitmap
            OutputImage = new Bitmap(workingImage.GetLength(0), workingImage.GetLength(1));

            // copy array to output Bitmap
            for (int x = 0; x < workingImage.GetLength(0); x++) // loop over columns
            for (int y = 0; y < workingImage.GetLength(1); y++) // loop over rows
            {
                Color newColor = Color.FromArgb(workingImage[x, y], workingImage[x, y], workingImage[x, y]);
                OutputImage.SetPixel(x, y, newColor); // set the pixel color at coordinate (x,y)
            }

            // Draw the overlays
            OutputImage = drawFoundCircles(OutputImage, circles, CircleColor);
            //OutputImage = drawFoundLines(OutputImage, centers, FullLineColor);
            OutputImage = visualiseHoughLineSegmentsColors(OutputImage, workingImage, line, LineSegmentColor);
            //OutputImage = visualiseCrossingsColor(OutputImage, CrossingThreshold, 3, centers, CrossingColor);
            OutputImage = visualiseHPGlassesColor(OutputImage, workingImage, found2, 20, HpGlassesColor);

            // display output image
            pictureBox2.Image = OutputImage;
        }

        /*
         * button_GetLargest_Click: process when user clicks Get Largest Object button
         */
        private void buttonGetLargest_Click_1(object sender, EventArgs e)
        {
            if (InputImage == null) return; // get out if no input image
            if (OutputImage != null) OutputImage.Dispose(); // reset output image
            OutputImage = new Bitmap(InputImage.Size.Width, InputImage.Size.Height); // create new output image
            Color[,]
                Image = new Color[InputImage.Size.Width,
                    InputImage.Size.Height]; // create array to speed-up operations (Bitmap functions are very slow)

            // copy input Bitmap to array            
            for (int x = 0; x < InputImage.Size.Width; x++) // loop over columns
            for (int y = 0; y < InputImage.Size.Height; y++) // loop over rows
                Image[x, y] = InputImage.GetPixel(x, y); // set pixel color in array at (x,y)

            // convert image to grayscale
            byte[,] workingImage = convertToGrayscale(Image);

            // Threshold the image
            workingImage = thresholdImage(workingImage, 10);

            // Get the largest object in the image
            workingImage = getLargestObject(new BinaryImage(workingImage), 3, 3).GetImage();

            // copy array to output Bitmap
            for (int x = 0; x < workingImage.GetLength(0); x++) // loop over columns
            for (int y = 0; y < workingImage.GetLength(1); y++) // loop over rows
            {
                Color newColor = Color.FromArgb(workingImage[x, y], workingImage[x, y], workingImage[x, y]);
                OutputImage.SetPixel(x, y, newColor); // set the pixel color at coordinate (x,y)
            }

            pictureBox2.Image = OutputImage; // display output image
        }

        /*
         * saveButton_Click: process when user clicks "Save" button
         */
        private void saveButton_Click(object sender, EventArgs e)
        {
            if (OutputImage == null) return; // get out if no output image
            if (saveImageDialog.ShowDialog() == DialogResult.OK)
                OutputImage.Save(saveImageDialog.FileName); // save the output image
        }


        /*
         * convertToGrayScale: convert a three-channel color image to a single channel grayscale image
         * input:   inputImage          three-channel (Color) image
         * output:                      single-channel (byte) image
         */
        private byte[,] convertToGrayscale(Color[,] inputImage)
        {
            // create temporary grayscale image of the same size as input, with a single channel
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];

            // setup progress bar
            progressBar.Visible = true;
            progressBar.Minimum = 1;
            progressBar.Maximum = InputImage.Size.Width * InputImage.Size.Height;
            progressBar.Value = 1;
            progressBar.Step = 1;

            // process all pixels in the image
            for (int x = 0; x < InputImage.Size.Width; x++) // loop over columns
            for (int y = 0; y < InputImage.Size.Height; y++) // loop over rows
            {
                Color pixelColor = inputImage[x, y]; // get pixel color
                byte average =
                    (byte) ((pixelColor.R + pixelColor.B + pixelColor.G) /
                            3); // calculate average over the three channels
                tempImage[x, y] = average; // set the new pixel color at coordinate (x,y)
                progressBar.PerformStep(); // increment progress bar
            }

            progressBar.Visible = false; // hide progress bar

            return tempImage;
        }


        // ====================================================================
        // ============= YOUR FUNCTIONS FOR ASSIGNMENT 1 GO HERE ==============
        // ====================================================================

        #region Assignment 1

        /// <summary>
        /// Invert a single channel (grayscale) image
        /// </summary>
        /// <param name="inputImage">single-channel (byte) image</param>
        /// <returns>single-channel (byte) image</returns>
        private byte[,] invertImage(byte[,] inputImage)
        {
            // create temporary grayscale image
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];

            // iterate over the pixels and invert them
            for (int y = 0; y < tempImage.GetLength(1); y++)
            for (int x = 0; x < tempImage.GetLength(0); x++)
                tempImage[x, y] = (byte) (255 - inputImage[x, y]);

            return tempImage;
        }

        /// <summary>
        /// Parallel
        /// Invert a single channel (grayscale) image
        /// </summary>
        /// <param name="inputImage">single-channel (byte) image</param>
        /// <returns>single-channel (byte) image</returns>
        private byte[,] invertImageParallel(byte[,] inputImage)
        {
            // create temporary grayscale image
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];

            // get the x size of the input image
            int inputXSize = tempImage.GetLength(0);

            // iterate over all pixels and invert them
            ParallelLoopResult loopResult = Parallel.For(0, inputImage.Length, index =>
            {
                int inputX = index % inputXSize; // gets the x coord from the loop index
                int inputY = index / inputXSize; // gets the y coord from the loop index
                tempImage[inputX, inputY] = (byte) (255 - inputImage[inputX, inputY]);
            });

            // throw an error if any thread failed
            if (!loopResult.IsCompleted)
                throw new Exception(
                    $"invertImageParallel did not complete it's loop to completion, stopped at iteration {loopResult.LowestBreakIteration}");

            return tempImage;
        }


        /// <summary>
        /// Create an image with the full range of intensity values used
        /// </summary>
        /// <param name="inputImage">single-channel (byte) image</param>
        /// <returns>single-channel (byte) image</returns>
        private byte[,] adjustContrast(byte[,] inputImage)
        {
            // create temporary grayscale image
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];
            int[] histrogramValues = new int[256];
            int nPixels = inputImage.GetLength(0) * inputImage.GetLength(1);
            // 0.1 = 10% 
            double percentageIgnoredValues = 0.1;
            int amountIgnoredPixels = (int) (nPixels * percentageIgnoredValues);

            // get the x and y size of the input image
            int inputXSize = tempImage.GetLength(0);
            int inputYSize = tempImage.GetLength(1);

            // count all the histogram values
            for (int y = 0; y < inputYSize; y++)
            for (int x = 0; x < inputXSize; x++)
            {
                histrogramValues[inputImage[x, y]]++;
            }

            // ignore the right amount of pixels and find the highest
            int ignoredPixels = 0;
            int i = 255;
            while (ignoredPixels < amountIgnoredPixels)
            {
                ignoredPixels += histrogramValues[i];
                i--;
            }

            byte aHigh = (byte) (i + 1);

            // ignore the right amount of pixels and find the lowest
            ignoredPixels = 0;
            i = 0;
            while (ignoredPixels < amountIgnoredPixels)
            {
                ignoredPixels += histrogramValues[i];
                i++;
            }

            byte aLow = (byte) (i - 1);

            // calculate new values
            for (int y = 0; y < inputYSize; y++)
            for (int x = 0; x < inputXSize; x++)
            {
                if (inputImage[x, y] > aHigh)
                    tempImage[x, y] = 255;
                else if (inputImage[x, y] < aLow)
                    tempImage[x, y] = 0;
                else
                    tempImage[x, y] = (byte) ((inputImage[x, y] - aLow) * (255 / (aHigh - aLow)));
            }

            return tempImage;
        }

        /// <summary>
        /// Parallel
        /// Create an image with the full range of intensity values used
        /// </summary>
        /// <param name="inputImage">single-channel (byte) image</param>
        /// <returns>single-channel (byte) image</returns>
        private byte[,] adjustContrastParallel(byte[,] inputImage)
        {
            // create temporary grayscale image
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];
            int[] histrogramValues = new int[256];
            int nPixels = inputImage.GetLength(0) * inputImage.GetLength(1);
            // 0.1 = 10% 
            double percentageIgnoredValues = 0.1;
            int amountIgnoredPixels = (int) (nPixels * percentageIgnoredValues);

            // get the x and y size of the input image
            int inputXSize = tempImage.GetLength(0);
            int inputYSize = tempImage.GetLength(1);

            // count all the histogram values
            for (int y = 0; y < inputYSize; y++)
            for (int x = 0; x < inputXSize; x++)
            {
                histrogramValues[inputImage[x, y]]++;
            }

            // ignore the right amount of pixels and find the highest
            int ignoredPixels = 0;
            int i = 255;
            while (ignoredPixels < amountIgnoredPixels)
            {
                ignoredPixels += histrogramValues[i];
                i--;
            }

            byte aHigh = (byte) (i + 1);

            // ignore the right amount of pixels and find the lowest
            ignoredPixels = 0;
            i = 0;
            while (ignoredPixels < amountIgnoredPixels)
            {
                ignoredPixels += histrogramValues[i];
                i++;
            }

            byte aLow = (byte) (i - 1);

            // calculate new values
            ParallelLoopResult loopResult = Parallel.For(0, inputImage.Length, index =>
            {
                int inputX = index % inputXSize; // gets the x coord from the loop index
                int inputY = index / inputXSize; // gets the y coord from the loop index
                if (inputImage[inputX, inputY] > aHigh)
                    tempImage[inputX, inputY] = 255;
                else if (inputImage[inputX, inputY] < aLow)
                    tempImage[inputX, inputY] = 0;
                else
                    tempImage[inputX, inputY] = (byte) ((inputImage[inputX, inputY] - aLow) * (255 / (aHigh - aLow)));
            });

            // throw an error if any thread failed
            if (!loopResult.IsCompleted)
                throw new Exception(
                    $"invertImageParallel did not complete it's loop to completion, stopped at iteration {loopResult.LowestBreakIteration}");

            return tempImage;
        }

        /// <summary>
        /// Create a Gaussian filter of specific square size and with a specified sigma
        /// </summary>
        /// <param name="size">Length and width of the Gaussian filter (only odd sizes)</param>
        /// <param name="sigma">Standard deviation of the Gaussian distribution (larger -> more spread out)</param>
        /// <returns>Gaussian filter kernel</returns>
        private float[,] createGaussianFilter(byte size, float sigma)
        {
            // check if the size is odd
            if (size % 2 == 0)
                throw new ArgumentException("createGaussianFilter, size was even");

            // calculate the size delta
            int sizeDelta = size / 2;

            // create the filter kernel
            float[,] filter = new float[size, size];

            // create the Gaussian filter
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                int kernelX = Math.Abs(x - sizeDelta);
                int kernelY = Math.Abs(y - sizeDelta);
                filter[x, y] = 1 / (2 * (float) Math.PI * (sigma * sigma)) *
                               (float) Math.Pow(Math.E,
                                   -((kernelX * kernelX + kernelY * kernelY) / (2 * (sigma * sigma))));
            }

            // calculate the normalizing multiplier of the kernel
            float kernelSum = 0.0f;
            foreach (var f in filter)
                kernelSum += f;
            float kernelMult = 1.0f / kernelSum;

            // normalize the kernel
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                filter[x, y] *= kernelMult;
            }

            return filter;
        }

        /// <summary>
        /// get the x value of the image for the kernel application, mirroring the image if the edge is exceeded
        /// </summary>
        /// <param name="x">The x value of the input image</param>
        /// <param name="kx">The x value of the kernel</param>
        /// <param name="filterSizeDelta">The offset from the kernels center. Floor(kernelSize / 2)</param>
        /// <param name="inputImageXLength">The amount of pixels in the input image's x direction</param>
        /// <returns>The x coordinate which should be used in the reference image</returns>
        private int GetRefImageXMirrored(int x, int kx, int filterSizeDelta, int inputImageXLength)
        {
            int n = x + (kx - filterSizeDelta); // get the actual input image pixel
            if (n < 0) return Math.Abs(n); // off the left side of the image
            if (n >= inputImageXLength) return inputImageXLength - kx; // off the right side of the image
            return n; // in the image
        }

        /// <summary>
        /// get the y value of the image for the kernel application, mirroring the image if the edge is exceeded
        /// </summary>
        /// <param name="x">The y value of the input image</param>
        /// <param name="kx">The y value of the kernel</param>
        /// <param name="filterSizeDelta">The offset from the kernels center. Floor(kernelSize / 2)</param>
        /// <param name="inputImageYLength">The amount of pixels in the input image's y direction</param>
        /// <returns>The y coordinate which should be used in the reference image</returns>
        private int GetRefImageYMirrored(int y, int ky, int filterSizeDelta, int inputImageYLength)
        {
            int n = y + (ky - filterSizeDelta); // get the actual input image pixel
            if (n < 0) return Math.Abs(n); // off the top of the image
            if (n >= inputImageYLength) return inputImageYLength - ky; // off the bottom of the image
            return n; // in the image
        }

        /// <summary>
        /// get the x value of the image for the kernel application, stretching the image if the edge is exceeded
        /// </summary>
        /// <param name="x">The x value of the input image</param>
        /// <param name="kx">The x value of the kernel</param>
        /// <param name="filterSizeDelta">The offset from the kernels center. Floor(kernelSize / 2)</param>
        /// <param name="inputImageXLength">The amount of pixels in the input image's x direction</param>
        /// <returns>The x coordinate which should be used in the reference image</returns>
        private int GetRefImageXStretched(int x, int kx, int filterSizeDelta, int inputImageXLength)
        {
            int n = x + (kx - filterSizeDelta); // get the actual input image pixel
            if (n < 0) return 0; // off the left side of the image
            if (n >= inputImageXLength) return inputImageXLength - 1; // off the right side of the image
            return n; // in the image
        }

        /// <summary>
        /// get the y value of the image for the kernel application, stretching the image if the edge is exceeded
        /// </summary>
        /// <param name="x">The y value of the input image</param>
        /// <param name="kx">The y value of the kernel</param>
        /// <param name="filterSizeDelta">The offset from the kernels center. Floor(kernelSize / 2)</param>
        /// <param name="inputImageYLength">The amount of pixels in the input image's y direction</param>
        /// <returns>The y coordinate which should be used in the reference image</returns>
        private int GetRefImageYStretched(int y, int ky, int filterSizeDelta, int inputImageYLength)
        {
            int n = y + (ky - filterSizeDelta); // get the actual input image pixel
            if (n < 0) return 0; // off the top of the image
            if (n >= inputImageYLength) return inputImageYLength - 1; // off the bottom of the image
            return n; // in the image
        }

        /// <summary>
        /// Apply linear filtering of an input image
        /// </summary>
        /// <param name="inputImage">Single-channel (byte) image</param>
        /// <param name="filter">Linear kernel</param>
        /// <returns>Single-channel (byte) image</returns>
        private byte[,] convolveImage(byte[,] inputImage, float[,] filter)
        {
            // create temporary grayscale image
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];

            // store the size of the filter
            int filterSize = filter.GetLength(0);
            // calculate the size delta
            int filterSizeDelta = filterSize / 2;

            // loop over the input image
            for (int y = 0; y < inputImage.GetLength(1); y++)
            for (int x = 0; x < inputImage.GetLength(0); x++)
            {
                tempImage[x, y] = ApplyKernel(x, y);
            }

            return tempImage;

            // apply the kernel to the given pixel
            byte ApplyKernel(int x, int y)
            {
                // create a new pixel
                byte newPixel = 0;

                // loop over the filter kernel, adding the values to newPixel during execution
                for (int kx = 0; kx < filterSize; kx++)
                for (int ky = 0; ky < filterSize; ky++)
                {
                    newPixel += (byte) (filter[kx, ky] *
                                        inputImage[GetRefImageXMirrored(x, kx, filterSizeDelta, inputImage.GetLength(0))
                                            , GetRefImageYMirrored(y, ky, filterSizeDelta, inputImage.GetLength(1))]);
                }

                return newPixel;
            }
        }

        /// <summary>
        /// Parallel
        /// Apply linear filtering of an input image
        /// </summary>
        /// <param name="inputImage">Single-channel (byte) image</param>
        /// <param name="filter">Linear kernel</param>
        /// <returns>Single-channel (byte) image</returns>
        private byte[,] convolveImageParallel(byte[,] inputImage, float[,] filter)
        {
            // create temporary grayscale image
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];

            // store the size of the filter
            int filterSize = filter.GetLength(0);
            // calculate the size delta
            int filterSizeDelta = filterSize / 2;
            // get the x size of the input image
            int inputXSize = inputImage.GetLength(0);

            // loop over the input image in parallel
            ParallelLoopResult loopResult = Parallel.For(0, inputImage.Length, index =>
            {
                int inputX = index % inputXSize; // gets the x coord from the loop index
                int inputY = index / inputXSize; // gets the y coord from the loop index
                tempImage[inputX, inputY] =
                    ApplyKernel(inputX,
                        inputY); // thread-safe because no thread writes to the same place in the target array
            });

            if (!loopResult.IsCompleted)
                throw new Exception(
                    $"consolveImageParallel did not complete it's loop to completion, stopped at iteration {loopResult.LowestBreakIteration}");

            return tempImage;

            // apply the kernel to the given pixel
            byte ApplyKernel(int x, int y)
            {
                // create a new pixel
                byte newPixel = 0;

                // loop over the filter kernel, adding the values to newPixel during execution
                for (int kx = 0; kx < filterSize; kx++)
                for (int ky = 0; ky < filterSize; ky++)
                {
                    newPixel += (byte) (filter[kx, ky] *
                                        inputImage[GetRefImageXMirrored(x, kx, filterSizeDelta, inputImage.GetLength(0))
                                            , GetRefImageYMirrored(y, ky, filterSizeDelta, inputImage.GetLength(1))]);
                }

                return newPixel;
            }
        }

        /// <summary>
        /// Apply median filtering on an input image with a kernel of specified size
        /// </summary>
        /// <param name="inputImage">Single-channel (byte) image</param>
        /// <param name="size">Length/width of the median filter kernel</param>
        /// <returns>Single-channel (byte) image</returns>
        private byte[,] medianFilter(byte[,] inputImage, byte size)
        {
            // create temporary grayscale image
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];
            int boundryPixels = size / 2;

            for (int y = boundryPixels; y < (inputImage.GetLength(1) - boundryPixels); y++)
            for (int x = boundryPixels; x < (inputImage.GetLength(0) - boundryPixels); x++)
            {
                //Add all kernel Values to a list
                List<byte> kernelValues = new List<byte>();
                for (int yK = 0; yK < size; yK++)
                for (int xK = 0; xK < size; xK++)
                    kernelValues.Add(inputImage[GetRefImageXMirrored(x, xK, boundryPixels, inputImage.GetLength(0)),
                        GetRefImageYMirrored(y, yK, boundryPixels, inputImage.GetLength(1))]);

                //sort list
                kernelValues.Sort();
                int medianIndex = (int) Math.Ceiling(((double) (size * size) / 2));
                tempImage[x, y] = kernelValues[medianIndex];
            }

            return tempImage;
        }

        /// <summary>
        /// Parallel
        /// Apply median filtering on an input image with a kernel of specified size
        /// </summary>
        /// <param name="inputImage">Single-channel (byte) image</param>
        /// <param name="size"></param>
        /// <returns></returns>
        private byte[,] medianFilterParallel(byte[,] inputImage, byte size)
        {
            // create temporary grayscale image
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];
            // calculate the size delta
            int filterSizeDelta = size / 2;
            int inputXSize = inputImage.GetLength(0);
            // loop over the input image in parallel
            ParallelLoopResult loopResult = Parallel.For(0, inputImage.Length, index =>
            {
                int inputX = index % inputXSize; // gets the x coord from the loop index
                int inputY = index / inputXSize; // gets the y coord from the loop index
                //Add all kernel Values to a list
                List<byte> kernelValues = new List<byte>();
                for (int yK = 0; yK < size; yK++)
                {
                    for (int xK = 0; xK < size; xK++)
                    {
                        kernelValues.Add(inputImage[
                            GetRefImageXMirrored(inputX, xK, filterSizeDelta, inputImage.GetLength(0)),
                            GetRefImageYMirrored(inputY, yK, filterSizeDelta, inputImage.GetLength(1))]);
                    }
                }

                //sort list and extract the median value
                kernelValues.Sort();
                int medianIndex = (int) Math.Ceiling(((double) (size * size) / 2));
                tempImage[inputX, inputY] = kernelValues[medianIndex];
            });

            if (!loopResult.IsCompleted)
                throw new Exception(
                    $"medianFilterParallel did not complete it's loop to completion, stopped at iteration {loopResult.LowestBreakIteration}");

            return tempImage;
        }

        /// <summary>
        /// Sobel filter in the horizontal direction
        /// </summary>
        double[,] sobelX = new double[,]
        {
            {-1, 0, 1},
            {-2, 0, 2},
            {-1, 0, 1}
        };

        /// <summary>
        /// Sobel filter in the vertical direction
        /// </summary>
        double[,] sobelY = new double[,]
        {
            {1, 2, 1},
            {0, 0, 0},
            {-1, -2, -1}
        };

        /// <summary>
        /// Calculate the image derivative of an input image and a provided edge kernel
        /// </summary>
        /// <param name="inputImage">Single-channel (byte) image</param>
        /// <returns></returns>
        private byte[,] edgeMagnitude(byte[,] inputImage)
        {
            // create temporary grayscale image
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];

            // loop over the image and calculate the image derivative
            for (int y = 0; (y < tempImage.GetLength(1)); y++)
            for (int x = 0; (x < tempImage.GetLength(0)); x++)
            {
                double hor = calcValue(sobelY, calcKernel(x, y));
                double ver = calcValue(sobelX, calcKernel(x, y));
                double answer = Math.Sqrt((hor * hor) + (ver * ver));
                if (answer > 255)
                    answer = 255;
                tempImage[x, y] = (byte) answer;
            }

            return tempImage;

            // Apply the sobel kernel to the given image kernel
            double calcValue(double[,] sobelKernel, byte[,] kernel)
            {
                double total = 0;
                for (int y = 0; y < 3; y++)
                for (int x = 0; x < 3; x++)
                    total += sobelKernel[x, y] * (double) kernel[x, y];

                return total;
            }

            // Get the image kernel corresponding to the given pixel
            byte[,] calcKernel(int x, int y)
            {
                byte[,] kernel = new byte[,]
                {
                    {
                        inputImage[GetRefImageXStretched(x, 0, 1, inputImage.GetLength(0)),
                            GetRefImageYStretched(y, 0, 1, inputImage.GetLength(1))],
                        inputImage[GetRefImageXStretched(x, 0, 1, inputImage.GetLength(0)),
                            GetRefImageYStretched(y, 1, 1, inputImage.GetLength(1))],
                        inputImage[GetRefImageXStretched(x, 0, 1, inputImage.GetLength(0)),
                            GetRefImageYStretched(y, 2, 1, inputImage.GetLength(1))]
                    },
                    {
                        inputImage[GetRefImageXStretched(x, 1, 1, inputImage.GetLength(0)),
                            GetRefImageYStretched(y, 0, 1, inputImage.GetLength(1))],
                        inputImage[GetRefImageXStretched(x, 1, 1, inputImage.GetLength(0)),
                            GetRefImageYStretched(y, 1, 1, inputImage.GetLength(1))],
                        inputImage[GetRefImageXStretched(x, 1, 1, inputImage.GetLength(0)),
                            GetRefImageYStretched(y, 2, 1, inputImage.GetLength(1))]
                    },
                    {
                        inputImage[GetRefImageXStretched(x, 2, 1, inputImage.GetLength(0)),
                            GetRefImageYStretched(y, 0, 1, inputImage.GetLength(1))],
                        inputImage[GetRefImageXStretched(x, 2, 1, inputImage.GetLength(0)),
                            GetRefImageYStretched(y, 1, 1, inputImage.GetLength(1))],
                        inputImage[GetRefImageXStretched(x, 2, 1, inputImage.GetLength(0)),
                            GetRefImageYStretched(y, 2, 1, inputImage.GetLength(1))]
                    }
                };
                return kernel;
            }
        }

        /// <summary>
        /// Parallel
        /// Calculate the image derivative of an input image and a provided edge kernel
        /// </summary>
        /// <param name="inputImage">Single-channel (byte) image</param>
        /// <returns></returns>
        private byte[,] edgeMagnitudeParallel(byte[,] inputImage)
        {
            // create temporary grayscale image
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];

            int inputXSize = inputImage.GetLength(0);

            ParallelLoopResult loopResult = Parallel.For(0, inputImage.Length, index =>
            {
                int inputX = index % inputXSize; // gets the x coord from the loop index
                int inputY = index / inputXSize; // gets the y coord from the loop index

                double hor = calcValue(sobelY, calcKernel(inputX, inputY));
                double ver = calcValue(sobelX, calcKernel(inputX, inputY));
                double answer = Math.Sqrt((hor * hor) + (ver * ver));
                if (answer > 255)
                    answer = 255;
                tempImage[inputX, inputY] = (byte) answer;
            });

            if (!loopResult.IsCompleted)
                throw new Exception(
                    $"medianFilterParallel did not complete it's loop to completion, stopped at iteration {loopResult.LowestBreakIteration}");

            return tempImage;

            //Apply the sobel kernel to the given image kernel
            double calcValue(double[,] sobelKernel, byte[,] kernel)
            {
                double total = 0;
                for (int y = 0; y < 3; y++)
                for (int x = 0; x < 3; x++)
                    total += sobelKernel[x, y] * (double) kernel[x, y];

                return total;
            }

            // Get the image kernel corresponding to the given pixel
            byte[,] calcKernel(int x, int y)
            {
                byte[,] kernel = new byte[,]
                {
                    {
                        inputImage[GetRefImageXStretched(x, 0, 1, inputImage.GetLength(0)),
                            GetRefImageYStretched(y, 0, 1, inputImage.GetLength(1))],
                        inputImage[GetRefImageXStretched(x, 0, 1, inputImage.GetLength(0)),
                            GetRefImageYStretched(y, 1, 1, inputImage.GetLength(1))],
                        inputImage[GetRefImageXStretched(x, 0, 1, inputImage.GetLength(0)),
                            GetRefImageYStretched(y, 2, 1, inputImage.GetLength(1))]
                    },
                    {
                        inputImage[GetRefImageXStretched(x, 1, 1, inputImage.GetLength(0)),
                            GetRefImageYStretched(y, 0, 1, inputImage.GetLength(1))],
                        inputImage[GetRefImageXStretched(x, 1, 1, inputImage.GetLength(0)),
                            GetRefImageYStretched(y, 1, 1, inputImage.GetLength(1))],
                        inputImage[GetRefImageXStretched(x, 1, 1, inputImage.GetLength(0)),
                            GetRefImageYStretched(y, 2, 1, inputImage.GetLength(1))]
                    },
                    {
                        inputImage[GetRefImageXStretched(x, 2, 1, inputImage.GetLength(0)),
                            GetRefImageYStretched(y, 0, 1, inputImage.GetLength(1))],
                        inputImage[GetRefImageXStretched(x, 2, 1, inputImage.GetLength(0)),
                            GetRefImageYStretched(y, 1, 1, inputImage.GetLength(1))],
                        inputImage[GetRefImageXStretched(x, 2, 1, inputImage.GetLength(0)),
                            GetRefImageYStretched(y, 2, 1, inputImage.GetLength(1))]
                    }
                };
                return kernel;
            }
        }

        /// <summary>
        /// Threshold the given image, setting every value above the given threshold value to white and every value below the given threshold value to black.
        /// </summary>
        /// <param name="inputImage"> single-channel (byte) image to threshold</param>
        /// <param name="thresholdValue"> threshold value </param>
        /// <returns>single-channel (byte) image</returns>
        private byte[,] thresholdImage(byte[,] inputImage, byte thresholdValue)
        {
            // create temporary grayscale image
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];

            // iterate over the image pixels and threshold them
            for (int y = 0; y < tempImage.GetLength(1); y++)
            for (int x = 0; x < tempImage.GetLength(0); x++)
            {
                if (inputImage[x, y] > thresholdValue)
                    tempImage[x, y] = 255;
                else
                    tempImage[x, y] = 0;
            }

            return tempImage;
        }
        
        /// <summary>
        /// Threshold the given image, setting every value above the given threshold value to white and every value below the given threshold value to black.
        /// </summary>
        /// <param name="inputImage"> single-channel (byte) image to threshold</param>
        /// <param name="thresholdValue"> threshold value </param>
        /// <returns>single-channel (byte) image</returns>
        private byte[,,] threshold3D_2(byte[,,] inputImage, byte thresholdValue)
        {
            // create temporary grayscale image
            byte[,,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1), inputImage.GetLength(2)];

            // iterate over the image pixels and threshold them
            for (int z = 0; z < tempImage.GetLength(2); z++)
            for (int y = 0; y < tempImage.GetLength(1); y++)
            for (int x = 0; x < tempImage.GetLength(0); x++)
            {
                if (inputImage[x, y, z] > thresholdValue)
                    tempImage[x, y, z] = 255;
                else
                    tempImage[x, y, z] = 0;
            }

            return tempImage;
        }
        
        /// <summary>
        /// Threshold the given image, setting every value above the given threshold value to white and every value below the given threshold value to black.
        /// </summary>
        /// <param name="inputImage"> single-channel (byte) image to threshold</param>
        /// <param name="thresholdValue"> threshold value </param>
        /// <returns>single-channel (byte) image</returns>
        private int[,,] threshold3D_2(int[,,] inputImage, int thresholdValue)
        { 
            // iterate over the image pixels and threshold them
            for (int z = 0; z < inputImage.GetLength(2); z++)
            for (int y = 0; y < inputImage.GetLength(1); y++)
            for (int x = 0; x < inputImage.GetLength(0); x++)
            {
                if (inputImage[x, y, z] > thresholdValue)
                    inputImage[x, y, z] = 255;
                else
                    inputImage[x, y, z] = 0;
            }

            return inputImage;
        }
        private void threshold3D(ref int[,,] inputImage, int thresholdValue)
        {
            // iterate over the image pixels and threshold them
            for (int z = 0; z < inputImage.GetLength(2); z++)
                for (int y = 0; y < inputImage.GetLength(1); y++)
                    for (int x = 0; x < inputImage.GetLength(0); x++)
                    {
                        if (inputImage[x, y, z] > thresholdValue)
                            inputImage[x, y, z] = 255;
                        else
                            inputImage[x, y, z] = 0;
                    }
        }

        /// <summary>
        /// Threshold the given image, setting every value above the given threshold value to white and every value below the given threshold value to black.
        /// </summary>
        /// <param name="inputImage"> single-channel (int) image to threshold</param>
        /// <param name="thresholdValue"> threshold value </param>
        /// <returns>single-channel (byte) image</returns>
        private byte[,] thresholdImage(int[,] inputImage, int thresholdValue)
        {
            // create temporary grayscale image
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];

            // iterate over the image pixels and threshold them
            for (int y = 0; y < tempImage.GetLength(1); y++)
                for (int x = 0; x < tempImage.GetLength(0); x++)
                {
                    if (inputImage[x, y] > thresholdValue)
                        tempImage[x, y] = 255;
                    else
                        tempImage[x, y] = 0;
                }

            return tempImage;
        }
        /// <summary>
        /// Parallel.
        /// Threshold the given image, setting every value above the given threshold value to white and every value below the given threshold value to black.
        /// </summary>
        /// <param name="inputImage"> single-channel (byte) image to threshold</param>
        /// <param name="thresholdValue"> threshold value </param>
        /// <returns>single-channel (byte) image</returns>
        private byte[,] thresholdImageParallel(byte[,] inputImage, byte thresholdValue)
        {
            // create temporary grayscale image
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];

            // get the x size of the input image
            int inputXSize = tempImage.GetLength(0);

            // iterate over the image pixels and threshold them
            ParallelLoopResult loopResult = Parallel.For(0, inputImage.Length, index =>
            {
                int inputX = index % inputXSize; // gets the x coord from the loop index
                int inputY = index / inputXSize; // gets the y coord from the loop index
                if (inputImage[inputX, inputY] > thresholdValue)
                    tempImage[inputY, inputY] = 255;
                else
                    tempImage[inputX, inputY] = 0;
            });

            // throw and exception if any thread did not finish
            if (!loopResult.IsCompleted)
                throw new Exception(
                    $"thresholdImageParallel did not complete it's loop to completion, stopped at iteration {loopResult.LowestBreakIteration}");

            return tempImage;
        }

        /// <summary>
        /// Threshold the given image, setting every value above the given threshold value to white and every value below the given threshold value to black.
        /// </summary>
        /// <param name="inputImage"> single-channel (byte) image to threshold</param>
        /// <param name="thresholdValue"> threshold value </param>
        /// <returns>single-channel (byte) image</returns>
        private BinaryImage thresholdImageToBinary(byte[,] inputImage, byte thresholdValue)
        {
            // create temporary grayscale image
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];

            // iterate over the image pixels and threshold them
            for (int y = 0; y < tempImage.GetLength(1); y++)
            for (int x = 0; x < tempImage.GetLength(0); x++)
            {
                if (inputImage[x, y] > thresholdValue)
                    tempImage[x, y] = 255;
                else
                    tempImage[x, y] = 0;
            }

            return new BinaryImage(tempImage);
        }

        /// <summary>
        /// Parallel.
        /// Threshold the given image, setting every value above the given threshold value to white and every value below the given threshold value to black.
        /// </summary>
        /// <param name="inputImage"> single-channel (byte) image to threshold</param>
        /// <param name="thresholdValue"> threshold value </param>
        /// <returns>single-channel (byte) image</returns>
        private BinaryImage thresholdImageToBinaryParallel(byte[,] inputImage, byte thresholdValue)
        {
            // create temporary grayscale image
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];

            // get the x size of the input image
            int inputXSize = tempImage.GetLength(0);

            // iterate over the image pixels and threshold them
            ParallelLoopResult loopResult = Parallel.For(0, inputImage.Length, index =>
            {
                int inputX = index % inputXSize; // gets the x coord from the loop index
                int inputY = index / inputXSize; // gets the y coord from the loop index
                if (inputImage[inputX, inputY] > thresholdValue)
                    tempImage[inputY, inputY] = 255;
                else
                    tempImage[inputX, inputY] = 0;
            });

            // throw and exception if any thread did not finish
            if (!loopResult.IsCompleted)
                throw new Exception(
                    $"thresholdImageParallel did not complete it's loop to completion, stopped at iteration {loopResult.LowestBreakIteration}");

            return new BinaryImage(tempImage);
        }

        /// <summary>
        /// Equalize the histogram of the given image
        /// </summary>
        /// <param name="inputImage">single-channel (byte) image</param>
        /// <returns>single-channel (byte) image</returns>
        private byte[,] histrogramEqualization(byte[,] inputImage)
        {
            // create temporary grayscale image
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];
            int[] histrogramValues = new int[256];
            int totalPixels = inputImage.GetLength(0) * inputImage.GetLength(1);

            //Count all the histrogram values
            for (int y = 0; y < inputImage.GetLength(1); y++)
            for (int x = 0; x < inputImage.GetLength(0); x++)
                histrogramValues[inputImage[x, y]]++;

            //Calculate probability 
            double[] probability = new double[256];
            for (int i = 0; i <= 255; i++)
                probability[i] = histrogramValues[i] / (double) totalPixels;

            //Calculate cumulative propability
            double[] cumulativeProbability = new double[256];
            cumulativeProbability[0] = probability[0];
            for (int i = 1; i <= 255; i++)
                cumulativeProbability[i] = cumulativeProbability[i - 1] + probability[i];

            //Multiply by 255
            byte[] newValues = new byte[256];
            for (int i = 1; i <= 255; i++)
                newValues[i] = (byte) (cumulativeProbability[i] * 255);

            //add new values to new image
            for (int y = 0; y < tempImage.GetLength(1); y++)
            for (int x = 0; x < tempImage.GetLength(0); x++)
                tempImage[x, y] = newValues[inputImage[x, y]];

            return tempImage;
        }

        #endregion

        // ====================================================================
        // ============= YOUR FUNCTIONS FOR ASSIGNMENT 2 GO HERE ==============
        // ====================================================================

        #region Assignment 2

        private enum StructuringElementShape
        {
            Square,
            Plus
        }

        /// <summary>
        /// Creates a structuring element
        /// </summary>
        /// <param name="shape">struct shape element</param>
        /// <param name="size">size of the structuringlement</param>
        /// <returns>single-channel (byte) image</returns>
        private byte[,] createStructuringElement(StructuringElementShape shape, int size)
        {
            if (size % 2 == 0 || size <= 1)
                throw new ArgumentException("createStructuringElement got an invalid size");

            byte[,] H = new byte[size, size];
            switch (shape)
            {
                case StructuringElementShape.Square:
                    CreateSquareSE();
                    break;
                case StructuringElementShape.Plus:
                    CreatePlusSE();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(shape), shape, null);
            }

            return H;

            // Create the square shaped structuring element
            void CreateSquareSE()
            {
                for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    H[x, y] = 255;
                }
            }

            // Create the Plus shaped structuring element
            void CreatePlusSE()
            {
                // Get the index of the center of the structuring element
                int plusIndex = size / 2;

                // Create a 3x3 plus shaped structuring element
                byte[,] plus3x3 = new byte[,]
                {
                    {0, 255, 0},
                    {255, 255, 255},
                    {0, 255, 0}
                };

                // If the size is 3x3, the further calculations are not needed and thus skipped for efficiency
                if (size == 3)
                {
                    H = plus3x3;
                    return;
                }

                // Create the empty structuring element with a 3x3 plus in the center
                for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    H[x, y] = x == plusIndex && y - plusIndex >= -1 && y - plusIndex <= 1 ||
                              y == plusIndex && x - plusIndex >= -1 && x - plusIndex <= 1
                        ? (byte) 255 // current coordinate is part of the center 3x3 plus
                        : (byte) 0; // current coordinate is not part of the center 3x3 plus
                }

                // Iteratively dilate the empty structuring element to get the required size structuring element
                for (int i = size; i > 3; i -= 2)
                {
                    H = dilateImage(H, plus3x3);
                }
            }
        }

        /// <summary>
        /// Erodes the given image with respect to the given structuring element
        /// </summary>
        /// <param name="input">The byte[,] image to erode</param>
        /// <param name="structuringElement">The byte[,] structuring element to reference</param>
        /// <param name="controlImage">The byte[,] control image / mask for geodesic operation</param>
        /// <returns>The eroded byte[,] image</returns>
        private byte[,] erodeImage(byte[,] input, byte[,] structuringElement, byte[,] controlImage = null)
        {
            // Store the size of the input
            int xSize = input.GetLength(0);
            int ySize = input.GetLength(1);

            // Store the size of the structuring element
            int seSize = structuringElement.GetLength(0);
            int seSizeDelta = seSize / 2;

            // Check the controlImage if needed
            if (controlImage is not null && (controlImage.GetLength(0) != xSize || controlImage.GetLength(1) != ySize))
                throw new ArgumentException(
                    "erodeImage got a control image which did not match the input image in size");

            // Create a temporary working image
            byte[,] tempImage = new byte[xSize, ySize];

            // Iterate over the input image, applying erosion with respect to the given structuring element
            for (int y = 0; y < ySize; y++)
            for (int x = 0; x < xSize; x++)
            {
                tempImage[x, y] = ApplyStructuringElement(x, y);
            }

            return tempImage;

            // Apply the given structuring element to the given pixel
            byte ApplyStructuringElement(int x, int y)
            {
                List<byte> valList = new List<byte>();
                for (int seY = 0; seY < seSize; seY++)
                for (int seX = 0; seX < seSize; seX++)
                {
                    // Get the actual coordinates to reference in the input image
                    int refX = x + (seX - seSizeDelta);
                    int refY = y + (seY - seSizeDelta);

                    // Check if the reference coordinates are out of bounds and add 255 (foreground) to the valList if they are
                    if (refX < 0 || refX >= xSize)
                        valList.Add(255);
                    else if (refY < 0 || refY >= ySize)
                        valList.Add(255);
                    else
                        // Add the value to the valList with respect to the structuring element
                        valList.Add(structuringElement[seX, seY] == 255 ? input[refX, refY] : (byte) 255);
                }

                // Get the lowest value of the neighborhood
                byte returnVal = valList.Min();

                // Apply the mask if necessary
                if (controlImage is not null)
                    returnVal = Math.Max(returnVal, controlImage[x, y]);

                // Return the lowest value to erode the image
                return returnVal;
            }
        }

        /// <summary>
        /// Parallel
        /// Erodes the given image with respect to the given structuring element
        /// </summary>
        /// <param name="input">The byte[,] image to erode</param>
        /// <param name="structuringElement">The byte[,] structuring element to reference</param>
        /// <param name="controlImage">The byte[,] control image / mask for geodesic operation</param>
        /// <returns>The eroded byte[,] image</returns>
        private byte[,] erodeImageParallel(byte[,] input, byte[,] structuringElement, byte[,] controlImage = null)
        {
            // Store the size of the input
            int xSize = input.GetLength(0);
            int ySize = input.GetLength(1);

            // Store the size of the structuring element
            int seSize = structuringElement.GetLength(0);
            int seSizeDelta = seSize / 2;

            // Check the controlImage if needed
            if (controlImage is not null && (controlImage.GetLength(0) != xSize || controlImage.GetLength(1) != ySize))
                throw new ArgumentException(
                    "erodeImage got a control image which did not match the input image in size");

            // Create a temporary working image
            byte[,] tempImage = new byte[xSize, ySize];

            // Iterate over the input image, applying erosion with respect to the given structuring element
            ParallelLoopResult loopResult = Parallel.For(0, input.Length, index =>
            {
                int inputX = index % xSize; // gets the x coord from the loop index
                int inputY = index / xSize; // gets the y coord from the loop index
                tempImage[inputX, inputY] = ApplyStructuringElement(inputX, inputY);
            });

            if (!loopResult.IsCompleted)
                throw new Exception(
                    $"erodeImageParallel did not complete it's loop to completion, stopped at iteration {loopResult.LowestBreakIteration}");

            return tempImage;

            // Apply the given structuring element to the given pixel
            byte ApplyStructuringElement(int x, int y)
            {
                List<byte> valList = new List<byte>();
                for (int seY = 0; seY < seSize; seY++)
                for (int seX = 0; seX < seSize; seX++)
                {
                    // Get the actual coordinates to reference in the input image
                    int refX = x + (seX - seSizeDelta);
                    int refY = y + (seY - seSizeDelta);

                    // Check if the reference coordinates are out of bounds and add 255 (foreground) to the valList if they are
                    if (refX < 0 || refX >= xSize)
                        valList.Add(255);
                    else if (refY < 0 || refY >= ySize)
                        valList.Add(255);
                    else
                        // Add the value to the valList with respect to the structuring element
                        valList.Add(structuringElement[seX, seY] == 255 ? input[refX, refY] : (byte) 255);
                }

                // Get the lowest value of the neighborhood
                byte returnVal = valList.Min();

                // Apply the mask if necessary
                if (controlImage is not null)
                    returnVal = Math.Max(returnVal, controlImage[x, y]);

                // Return the lowest value to erode the image
                return returnVal;
            }
        }

        /// <summary>
        /// Dilates the given image with respect to the given structuring element
        /// </summary>
        /// <param name="input">The byte[,] image to dilate</param>
        /// <param name="structuringElement">The byte[,] structuring element to reference</param>
        /// <param name="controlImage">The byte[,] control image / mask for geodesic operation</param>
        /// <returns>The dilated byte[,] image</returns>
        private byte[,] dilateImage(byte[,] input, byte[,] structuringElement, byte[,] controlImage = null)
        {
            // Store the size of the input
            int xSize = input.GetLength(0);
            int ySize = input.GetLength(1);

            // Store the size of the structuring element
            int seSize = structuringElement.GetLength(0);
            int seSizeDelta = seSize / 2;

            // Check the controlImage if needed
            if (controlImage is not null && (controlImage.GetLength(0) != xSize || controlImage.GetLength(1) != ySize))
                throw new ArgumentException(
                    "erodeImage got a control image which did not match the input image in size");

            // Create a temporary working image
            byte[,] tempImage = new byte[xSize, ySize];

            // Iterate over the input image, applying dilation with respect to the given structuring element
            for (int y = 0; y < ySize; y++)
            for (int x = 0; x < xSize; x++)
            {
                tempImage[x, y] = ApplyStructuringElement(x, y);
            }

            return tempImage;

            // Apply the given structuring element to the given pixel
            byte ApplyStructuringElement(int x, int y)
            {
                List<byte> valList = new List<byte>();
                for (int seY = 0; seY < seSize; seY++)
                for (int seX = 0; seX < seSize; seX++)
                {
                    // Get the actual coordinates to reference in the input image
                    int refX = x + (seX - seSizeDelta);
                    int refY = y + (seY - seSizeDelta);

                    // Check if the reference coordinates are out of bounds and add 0 (background) to the valList if they are
                    if (refX < 0 || refX >= xSize)
                        valList.Add(0);
                    else if (refY < 0 || refY >= ySize)
                        valList.Add(0);
                    else
                        // Add the value to the valList with respect to the structuring element
                        valList.Add(structuringElement[seX, seY] == 255 ? input[refX, refY] : (byte) 0);
                }

                // Get the lowest value of the neighborhood
                byte returnVal = valList.Max();

                // Apply the mask if necessary
                if (controlImage is not null)
                    returnVal = Math.Min(returnVal, controlImage[x, y]);

                // Return the lowest value to erode the image
                return returnVal;
            }
        }

        /// <summary>
        /// Parallel
        /// Dilates the given image with respect to the given structuring element
        /// </summary>
        /// <param name="input">The byte[,] image to dilate</param>
        /// <param name="structuringElement">The byte[,] structuring element to reference</param>
        /// <param name="controlImage">The byte[,] control image / mask for geodesic operation</param>
        /// <returns>The dilated byte[,] image</returns>
        private byte[,] dilateImageParallel(byte[,] input, byte[,] structuringElement, byte[,] controlImage = null)
        {
            // Store the size of the input
            int xSize = input.GetLength(0);
            int ySize = input.GetLength(1);

            // Store the size of the structuring element
            int seSize = structuringElement.GetLength(0);
            int seSizeDelta = seSize / 2;

            // Check the controlImage if needed
            if (controlImage is not null && (controlImage.GetLength(0) != xSize || controlImage.GetLength(1) != ySize))
                throw new ArgumentException(
                    "erodeImage got a control image which did not match the input image in size");

            // Create a temporary working image
            byte[,] tempImage = new byte[xSize, ySize];

            // Iterate over the input image, applying dilation with respect to the given structuring element
            ParallelLoopResult loopResult = Parallel.For(0, input.Length, index =>
            {
                int inputX = index % xSize; // gets the x coord from the loop index
                int inputY = index / xSize; // gets the y coord from the loop index
                tempImage[inputX, inputY] = ApplyStructuringElement(inputX, inputY);
            });

            if (!loopResult.IsCompleted)
                throw new Exception(
                    $"erodeImageParallel did not complete it's loop to completion, stopped at iteration {loopResult.LowestBreakIteration}");


            return tempImage;

            // Apply the given structuring element to the given pixel
            byte ApplyStructuringElement(int x, int y)
            {
                List<byte> valList = new List<byte>();
                for (int seY = 0; seY < seSize; seY++)
                for (int seX = 0; seX < seSize; seX++)
                {
                    // Get the actual coordinates to reference in the input image
                    int refX = x + (seX - seSizeDelta);
                    int refY = y + (seY - seSizeDelta);

                    // Check if the reference coordinates are out of bounds and add 0 (background) to the valList if they are
                    if (refX < 0 || refX >= xSize)
                        valList.Add(0);
                    else if (refY < 0 || refY >= ySize)
                        valList.Add(0);
                    else
                        // Add the value to the valList with respect to the structuring element
                        valList.Add(structuringElement[seX, seY] == 255 ? input[refX, refY] : (byte) 0);
                }

                // Get the lowest value of the neighborhood
                byte returnVal = valList.Max();

                // Apply the mask if necessary
                if (controlImage is not null)
                    returnVal = Math.Min(returnVal, controlImage[x, y]);

                // Return the lowest value to erode the image
                return returnVal;
            }
        }

        /// <summary>
        /// opens the image by doing an erosion followed by a dilation
        /// </summary>
        /// <param name="inputImage">single-channel (byte) image</param>
        /// <param name="structuringElement">structuring element</param>
        /// <returns>single-channel (byte) image</returns>
        private byte[,] openImage(byte[,] inputImage, byte[,] structuringElement)
        {
            //erosion followed by dilation
            return dilateImage(erodeImage(inputImage, structuringElement), structuringElement);
        }

        /// <summary>
        /// closes the image by doing a dilation followed by an erosion
        /// </summary>
        /// <param name="inputImage">single-channel (byte) image</param>
        /// <param name="structuringElement">structuring element</param>
        /// <returns>single-channel (byte) image</returns>
        private byte[,] closeImage(byte[,] inputImage, byte[,] structuringElement)
        {
            //dilation followed by erosion
            return erodeImage(dilateImage(inputImage, structuringElement), structuringElement);
        }

        /// <summary>
        /// closes the image by doing a dilation followed by an erosion
        /// </summary>
        /// <param name="inputImage">single-channel (byte) image</param>
        /// <returns>single-channel (byte) image</returns>
        private List<Point> contourTrace(BinaryImage inputImage)
        {
            List<Point> boundaryPixels = new List<Point>();
            Point lastPixel = findFirstPoint();

            boundaryPixels.Add(lastPixel);
            Point startPosRotation = new Point(0, 1);
            Point newPixel = findNextBoundaryPixel(lastPixel, startPosRotation);
            boundaryPixels.Add(newPixel);

            while (boundaryPixels[0] != boundaryPixels.Last())
            {
                startPosRotation = calcStartingPosRotation();
                lastPixel = newPixel;
                newPixel = findNextBoundaryPixel(newPixel, startPosRotation);
                boundaryPixels.Add(newPixel);
            }

            return boundaryPixels;

            Point findFirstPoint()
            {
                for (int y = 0; y < inputImage.YSize; y++)
                for (int x = 0; x < inputImage.XSize; x++)
                {
                    if (inputImage.GetPixelBool(x, y))
                    {
                        return new Point(x, y);
                    }
                }

                throw new ArgumentException("Empty Image!");
            }

            Point findNextBoundaryPixel(Point pixel, Point startingPos)
            {
                Point newPoint = addPoints(pixel, new Point(startingPos.X - 1, startingPos.Y - 1));
                while (!allowedValue(newPoint))
                {
                    startingPos = step(startingPos);
                    newPoint = addPoints(pixel, new Point(startingPos.X - 1, startingPos.Y - 1));
                }

                while (!inputImage.GetPixelBool(newPoint.X, newPoint.Y))
                {
                    startingPos = step(startingPos);
                    Point temp = addPoints(lastPixel, new Point(startingPos.X - 1, startingPos.Y - 1));
                    if (allowedValue(temp))
                        newPoint = addPoints(lastPixel, new Point(startingPos.X - 1, startingPos.Y - 1));
                }

                return newPoint;
            }

            bool allowedValue(Point i)
            {
                if (i.X >= 0 && i.X < inputImage.XSize && i.Y >= 0 && i.Y < inputImage.YSize)
                    return true;
                else
                    return false;
            }

            Point addPoints(Point a, Point b)
            {
                return new Point(a.X + b.X, a.Y + b.Y);
            }

            Point step(Point point)
            {
                if (point.X < 2 && point.Y == 2)
                    return new Point(++point.X, point.Y);
                if (point.X == 2 && point.Y > 0)
                    return new Point(point.X, --point.Y);
                if (point.X > 0 && point.Y == 0)
                    return new Point(--point.X, point.Y);
                if (point.X == 0 && point.Y < 2)
                    return new Point(point.X, ++point.Y);
                throw new ArgumentException("Something went wrong with the step!");
            }

            Point calcStartingPosRotation()
            {
                float x = lastPixel.X - newPixel.X;
                float y = lastPixel.Y - newPixel.Y;

                if (x == 1 && y == 1)
                    return new Point(2, 0);
                else if (x == 1 && y == 0)
                    return new Point(1, 0);
                else if (x == 1 && y == -1)
                    return new Point(0, 0);
                else if (x == 0 && y == -1)
                    return new Point(0, 1);
                else if (x == -1 && y == -1)
                    return new Point(0, 2);
                else if (x == -1 && y == 0)
                    return new Point(1, 2);
                else if (x == -1 && y == 1)
                    return new Point(2, 2);
                else if (x == 0 && y == 1)
                    return new Point(2, 1);
                throw new ArgumentException("Points are not nex to eachother!");
            }
        }

        /// <summary>
        /// Fills the histrogram in the UI and calculates the number of distinct values
        /// </summary>
        /// <param name="inputImage">single-channel (byte) image</param>
        /// <returns>single-channel (byte) image</returns>
        void countValues(byte[,] inputImage)
        {
            //Count all the histrogram values
            chart1.Series.Clear();
            chart1.Titles.Clear();
            int[] histrogramValues = new int[256];

            for (int y = 0; y < inputImage.GetLength(1); y++)
            for (int x = 0; x < inputImage.GetLength(0); x++)
                histrogramValues[inputImage[x, y]]++;

            Series series = chart1.Series.Add("Grey values");
            for (int i = 0; i < histrogramValues.Length; i++)
            {
                series.Points.AddXY(i, histrogramValues[i]);
            }

            int nDistinctValues = 0;
            for (int i = 0; i < histrogramValues.Length; i++)
            {
                if (histrogramValues[i] > 0)
                    nDistinctValues++;
            }

            chart1.Titles.Add("Number of distinct Values: " + nDistinctValues);
        }

        /// <summary>
        /// Counts the number of foreground values and displays it to the UI
        /// </summary>
        /// <param name="inputImage">single-channel (byte) image</param>
        void countForegroundValues(BinaryImage inputImage)
        {
            chart1.Titles.Clear();
            int fgValues = 0;
            for (int y = 0; y < inputImage.YSize; y++)
            for (int x = 0; x < inputImage.XSize; x++)
            {
                if (inputImage.GetPixelBool(x, y))
                    fgValues += 1;
            }

            chart1.Titles.Add($"Number of foreground values: {fgValues}");
        }

        byte[,] createBoundaryImage(int sizeX, int sizeY, List<Point> boundaryPixels)
        {
            byte[,] image = new byte[sizeX, sizeY];

            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    image[x, y] = 255;
                }
            }

            foreach (Point pixel in boundaryPixels)
            {
                image[pixel.X, pixel.Y] = 0;
            }

            return image;

        }

        /// <summary>
        /// Computes the pixel-wise AND operation on the given binary images
        /// </summary>
        /// <param name="lhs">The left hand side BinaryImage</param>
        /// <param name="rhs">The right hand side BinaryImage</param>
        /// <returns>a BinaryImage containing the pixel-wise AND of the inputs</returns>
        private BinaryImage andImage(BinaryImage lhs, BinaryImage rhs)
        {
            return lhs.AND(rhs);
        }

        /// <summary>
        /// Computes the pixel-wise OR operation on the given binary images
        /// </summary>
        /// <param name="lhs">The left hand side BinaryImage</param>
        /// <param name="rhs">The right hand side BinaryImage</param>
        /// <returns>a BinaryImage containing the pixel-wise OR of the inputs</returns>
        private BinaryImage orImage(BinaryImage lhs, BinaryImage rhs)
        {
            return lhs.OR(rhs);
        }

        private BinaryImage getLargestObject(BinaryImage input, int erodeSize, int dilateSize)
        {
            byte[,] curIteration = input.GetImage();
            byte[,] prevIteration = curIteration;

            // Erode the image until there are no foreground pixels remaining
            while (checkIfAtLeastOnePixelIsForeground(new BinaryImage(curIteration)))
            {
                prevIteration = curIteration;
                curIteration = erodeImageParallel(curIteration,
                    createStructuringElement(StructuringElementShape.Square, erodeSize));
            }
            // After this loop, we know the previous iteration only holds a pixel that was contained in the largest object

            // Reset the variables
            curIteration = prevIteration;
            prevIteration = null;

            // Geodisic dilate the image until stable, where the largest object is back to its original size
            while (!checkIfSamePicture(curIteration, prevIteration))
            {
                prevIteration = curIteration;
                curIteration = dilateImageParallel(curIteration,
                    createStructuringElement(StructuringElementShape.Square, dilateSize),
                    input.GetImage());
            }

            return new BinaryImage(curIteration);
        }

        /// <summary>
        /// Check if there is at least one foreground pixel remaining
        /// </summary>
        /// <param name="input">binary image</param>
        /// <returns>true if there is at least 1 pixel remaining</returns>
        private bool checkIfAtLeastOnePixelIsForeground(BinaryImage input)
        {
            foreach (byte pixel in input.GetImage())
            {
                if (pixel == 255)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Check if the given images are the same image
        /// </summary>
        /// <param name="lhs">byte[,] image</param>
        /// <param name="rhs">byte[,] image</param>
        /// <returns>true if the images are the same</returns>
        /// <exception cref="ArgumentException"></exception>
        private bool checkIfSamePicture(byte[,] lhs, byte[,] rhs)
        {
            if (lhs is null)
                return rhs is null;
            if (rhs is null)
                return false;

            if (lhs.GetLength(0) != rhs.GetLength(0) || lhs.GetLength(1) != rhs.GetLength(1))
                throw new ArgumentException("checkIfSamePicture got images which dont match in size");

            for (int y = 0; y < lhs.GetLength(1); y++)
            for (int x = 0; x < lhs.GetLength(0); x++)
            {
                if (lhs[x, y] != rhs[x, y])
                    return false;
            }

            return true;
        }

        #endregion

        // ====================================================================
        // ============= YOUR FUNCTIONS FOR ASSIGNMENT 3 GO HERE ==============
        // ====================================================================

        /// <summary>
        /// builds a hough transform image for line detection out of a binary image
        /// </summary>
        /// <param name="inputImage">binary image</param>
        /// <returns>single-channel hough tranform (byte) image</returns>
        private int[,] houghTranform(BinaryImage inputImage)
        {
            int maxDistance = (int)Math.Ceiling(Math.Sqrt(Math.Pow(inputImage.XSize, 2) + Math.Pow(inputImage.YSize, 2)));
            int[,] paramSpaceArray = new int[720, maxDistance * 2 + 1]; // 720 = 180 * 4, we use 4 steps per degree
            for (int y = 0; y < inputImage.YSize; y++)
            for (int x = 0; x < inputImage.XSize; x++)
            {
                if (inputImage.GetPixelBool(x, y))
                {
                    applyHough(x, y);

                }
            }

            return paramSpaceArray;

            void applyHough(int x, int y)
            {
                for (double i = 0; i < 180; i += 0.25d) // 0.25 = 1 / 4, we use 4 steps per degree
                {
                    double r = Math.Round(x * Math.Cos(Math.PI * i / 180d) + y * Math.Sin(Math.PI * i / 180d));
                    paramSpaceArray[(int)(i * 4d), (int)r + maxDistance] += 1;
                }
            }
        }

        /// <summary>
        /// Maps the given hough transform image to a byte[,] for visualisation
        /// DO NOT USE FOR CALCULATION PURPOSES!
        /// </summary>
        /// <param name="houghTransformImage"> The hough transform image</param>
        /// <returns> a byte[,] of the hough transform image for visualisation</returns>
        private byte[,] visualiseHoughImage(int[,] houghTransformImage)
        {
            byte[,] tempImage = new byte[houghTransformImage.GetLength(0), houghTransformImage.GetLength(1)];

            for (int y = 0; y < houghTransformImage.GetLength(1); y++)
            for (int x = 0; x < houghTransformImage.GetLength(0); x++)
            {
                tempImage[x, y] = (byte) houghTransformImage[x, y].Remap(0, int.MaxValue, 0, 255);
            }

            return tempImage;
        }

        /// <summary>
        /// builds a hough transform image for circle detection out of a binary image
        /// </summary>
        /// <param name="inputImage">binary image</param>
        /// <returns>single-channel hough tranform (byte) image</returns>
        private int[,,] houghTranformCircle(BinaryImage inputImage)
        {
            int[,,] paramSpaceArray = new int[inputImage.XSize, inputImage.YSize, (int)((rMax - rMin) * stepsPerR)];
            for (int z = 0; z < paramSpaceArray.GetLength(2); z++)
            for (int y = 0; y < paramSpaceArray.GetLength(1); y++)
            for (int x = 0; x < paramSpaceArray.GetLength(0); x++)
            {
                if (inputImage.GetPixelBool(x, y))
                {

                            applyHough(x, y,(z / 2d)+rMin);
                }
            }

            return paramSpaceArray;

            void applyHough(int a, int b, double r)
            {
                for (int t = 0; t < 360; t++)
                {
                    int x = (int)Math.Round(r * Math.Cos((Math.PI / 180) * t)) + a;
                    int y = (int)Math.Round(r * Math.Sin((Math.PI / 180) * t)) + b;
                    if (x > 0 && y > 0 && x < inputImage.XSize && y < inputImage.YSize)
                        paramSpaceArray[x, y, (int)((r - rMin) * 2)] += 1;
                }
            }

            int calcCircleAccumulator(double r)
            {
                double maxCircumference = 2 * Math.PI * (inputImage.XSize < inputImage.YSize ? inputImage.XSize / 2 : inputImage.YSize / 2);
                double circumference = 2 * r * Math.PI;
                double maxAcc = 100d;
                double output = maxAcc - circumference.Remap(1, maxCircumference, 1, maxAcc - 1);
                
                return (int)Math.Round(output);
            }
        }

        /// <summary>
        /// builds a hough transform image out of a binary image with a lower and upper angle boundary
        /// </summary>
        /// <param name="inputImage">binary image</param>
        /// <param name="lowerBoundary">byte minimum angle</param>
        /// <param name="upperBoundary">byte maximum angle</param>
        /// <returns>single-channel hough tranform (byte) image</returns>
        private int[,] houghTransformAngleLimits(BinaryImage inputImage, int lowerBoundary, int upperBoundary)
        {
            int maxDistance =
                (int) Math.Ceiling(Math.Sqrt(Math.Pow(inputImage.XSize, 2) + Math.Pow(inputImage.YSize, 2)));
            int[,] paramSpaceArray = new int[(upperBoundary - lowerBoundary) * 4, maxDistance * 2 + 1];
            for (int y = 0; y < inputImage.YSize; y++)
            for (int x = 0; x < inputImage.XSize; x++)
            {
                if (inputImage.GetPixelBool(x, y))
                {
                    applyHough(x, y);
                }
            }

            return paramSpaceArray;

            void applyHough(int x, int y)
            {
                for (double i = lowerBoundary; i < upperBoundary; i += 0.25d)
                {
                    double r = x * Math.Cos(Math.PI * i / 180d) + y * Math.Sin(Math.PI * i / 180d);
                    paramSpaceArray[(int)(i * 4d), (int)r + maxDistance] += 1;
                }
            }
        }
        /// <summary>
        /// finds peaks in a hough transform image
        /// </summary>
        /// <param name="inputImage">binary image</param>
        /// <returns>tuple of r-theta pairs where peaks are found</returns>
        private List<Circle> peakFindingCircle(BinaryImage inputImage)
        {
            int[,,] imageByte = houghTranformCircle(inputImage);
            imageByte = nonMaximumSuppression(imageByte);
            //remove all unecessary data
            int thresholdValue = 0;
            for (int x = 0; x < inputImage.XSize; x++)
            {
                for (int y = 0; y < inputImage.YSize; y++)
                {
                    for (int z = 0; z < imageByte.GetLength(2); z++)
                    {
                        if (imageByte[x, y, z] > thresholdValue)
                            thresholdValue = imageByte[x, y, z];
                    }
                    
                }
            }
            threshold3D(ref imageByte, thresholdValue-margeCircles);

            return findCenters(imageByte);

            List<Circle> findCenters(int[,,] image)
            {
                List<Circle> centers = new List<Circle>();

                for (int z = 0; z < image.GetLength(2); z++)
                for (int y = 0; y < image.GetLength(1); y++)
                for (int x = 0; x < image.GetLength(0); x++)
                {
                    if (image[x, y, z] == 255)
                    {
                        centers.Add(new Circle(new Point(x, y), (z / 2d) + rMin));
                    }
                }
                return centers;
            }
        }

        private int[,,] nonMaximumSuppression(int[,,] image)
        {
            for (int z = 0; z < image.GetLength(2); z++)
            for (int y = 0; y < image.GetLength(1); y++)
            for (int x = 0; x < image.GetLength(0); x++)
            {
                int maxValue = getMaxValues(x, y, z);
                if (maxValue > image[x, y, z])
                    image[x, y, z] = 0;
            }

            return image;

            int getMaxValues(int xCenter, int yCenter, int zCenter)
            {
                int maxValue = 0;
                for (int z = -1; z < 2; z++)
                for (int y = -1; y < 2; y++)
                for (int x = -1; x < 2; x++)
                {
                    Point3D point = new Point3D(xCenter + x, yCenter + y, zCenter + z);
                    if (point.X >= 0 && point.X < image.GetLength(0) && 
                        point.Y >= 0 && point.Y < image.GetLength(1) && 
                        point.Z >= 0 && point.Z < image.GetLength(2))
                    {
                        if (image[(int)point.X, (int)point.Y, (int)point.Z] > maxValue)
                            maxValue = image[(int)point.X, (int)point.Y, (int)point.Z];
                    }
                }
                return maxValue;
            }
        }
        /// <summary>
        /// finds peaks in a hough transform image
        /// </summary>
        /// <param name="inputImage">binary image</param>
        /// <returns>tuple of r-theta pairs where peaks are found</returns>
        private List<Point> peakFinding(BinaryImage inputImage, byte thresholdValue)
        {
            int maxDistance =
                (int) Math.Ceiling(Math.Sqrt(Math.Pow(inputImage.XSize, 2) + Math.Pow(inputImage.YSize, 2)));
            int[,] imageByte = houghTranform(inputImage);
            //remove all unecessary data
            BinaryImage image = new BinaryImage(thresholdImage(imageByte, thresholdValue));
            //close image
            image = new BinaryImage(closeImage(image.GetImage(),
                createStructuringElement(StructuringElementShape.Square, 3)));

            return findCenterPoints(image, maxDistance);

        }
        /// <summary>
        /// finds peaks in a hough transform image
        /// </summary>
        /// <param name="inputImage">binary image</param>
        /// <returns>tuple of r-theta pairs where peaks are found</returns>
        private List<Point> peakFinding(byte[,] inputImage, byte thresholdValue, byte greyScaleThreshold)
        {
            int maxDistance =
                (int)Math.Ceiling(Math.Sqrt(Math.Pow(inputImage.GetLength(0), 2) + Math.Pow(inputImage.GetLength(1), 2)));
            int[,] imageByte = houghTranform(new BinaryImage(thresholdImage(inputImage, greyScaleThreshold)));
            //remove all unecessary data
            BinaryImage image = new BinaryImage(thresholdImage(imageByte, thresholdValue));
            //close image
            image = new BinaryImage(closeImage(image.GetImage(),
                createStructuringElement(StructuringElementShape.Square, 3)));

            return findCenterPoints(image, maxDistance);

        }
        /// <summary>
        /// finds center points with bfs in a binary image
        /// </summary>
        /// <param name="inputImage">binary image</param>
        /// <returns>list of all the center points</returns>
        private List<Point> findCenterPoints(BinaryImage image, int maxDistance)
        {
            //find regions with BFS
            bool[,] foundPixels = new bool[image.XSize, image.YSize];
            List<List<Point>> regions = new List<List<Point>>();
            for (int y = 0; y < image.YSize; y++)
            {
                for (int x = 0; x < image.XSize; x++)
                {
                    if (image.GetPixelBool(x, y) && !foundPixels[x, y])
                    {
                        regions.Add(bfs(x, y));
                    }
                }
            }

            //find centers
            List<Point> centers = new List<Point>();
            foreach (var region in regions)
            {
                int xTotal = 0;
                int yTotal = 0;

                foreach (var point in region)
                {
                    xTotal += point.X;
                    yTotal += point.Y;
                }

                //add the avererage point
                centers.Add(new Point((xTotal / region.Count), (int) (yTotal / region.Count) - maxDistance));
            }

            return centers;

            List<Point> bfs(int x, int y)
            {
                Queue<Point> queue = new Queue<Point>();
                bool[,] Visited = new bool[image.XSize, image.YSize];
                List<Point> regionPixels = new List<Point>();
                //add first point
                queue.Enqueue(new Point(x, y));
                Visited[x, y] = true;

                while (queue.Count > 0)
                {
                    //get next point
                    Point point = queue.Dequeue();
                    //add point to list
                    regionPixels.Add(point);
                    //mark as added in foundPixels
                    foundPixels[point.X, point.Y] = true;
                    //find neigbouring points, add them to the queue and mark as visited
                    List<Point> neighbours = findNeighbours(point);
                    addNeighboursIfAllowed(neighbours);
                }

                return regionPixels;

                void addNeighboursIfAllowed(List<Point> neighbours)
                {
                    foreach (Point neighbour in neighbours)
                    {
                        //check if in image range and not visted and pixel is filled.
                        if (neighbour.X >= 0 && neighbour.X < image.XSize && neighbour.Y >= 0 &&
                            neighbour.Y < image.YSize &&
                            !Visited[neighbour.X, neighbour.Y] && image.GetPixelBool(neighbour.X, neighbour.Y))
                        {
                            queue.Enqueue(neighbour);
                            Visited[neighbour.X, neighbour.Y] = true;
                        }
                    }
                }

                List<Point> findNeighbours(Point center)
                {
                    List<Point> neighbours = new List<Point>();
                    Point index = new Point(-1, -1);
                    neighbours.Add(new Point(center.X + index.X, center.Y + index.Y));

                    for (int i = 0; i < 7; i++)
                    {
                        index = step(index);
                        neighbours.Add(new Point(center.X + index.X, center.Y + index.Y));
                    }

                    return neighbours;
                }

                Point step(Point point)
                {
                    if (point.X < 1 && point.Y == 1)
                        return new Point(++point.X, point.Y);
                    if (point.X == 1 && point.Y > -1)
                        return new Point(point.X, --point.Y);
                    if (point.X > -1 && point.Y == -1)
                        return new Point(--point.X, point.Y);
                    if (point.X == -1 && point.Y < 1)
                        return new Point(point.X, ++point.Y);
                    throw new ArgumentException("Something went wrong with the step!");
                }
            }
        }

        /// <summary>
        /// Calculates the X coordinate which belongs to the Y coordinate of the given line
        /// </summary>
        /// <param name="y"> The Y coordinate on the line</param>
        /// <param name="theta"> The theta value of the line</param>
        /// <param name="r"> The r value of the line</param>
        private double calcLineX(int y, double theta, double r)
        {
            theta = theta / 4d;
            double temp1 = r - ((double)y * Math.Sin(Math.PI * theta / 180d));
            double x = temp1 / Math.Cos(Math.PI * theta / 180d);
            return Math.Round(x);
        }

        /// <summary>
        /// Calculates the Y coordinate which belongs to the X coordinate of the given line
        /// </summary>
        /// <param name="x"> The X coordinate on the line</param>
        /// <param name="theta"> The theta value of the line</param>
        /// <param name="r"> The r value of the line</param>
        private double calcLineY(int x, double theta, double r)
        {
            theta = theta / 4d;
            double temp1 = r - ((double)x * Math.Cos(Math.PI * theta / 180d));
            double y = temp1 / Math.Sin(Math.PI * theta / 180d);
            return Math.Round(y);
        }

        /// <summary>
        /// Gets a list of line segments which run across the given r theta pair
        /// </summary>
        /// <param name="inputImage"> single channel (byte) image</param>
        /// <param name="rThetaPair"> the r theta pair depicting the line</param>
        /// <param name="minIntensityThreshold"> the minimum edge intensity for a pixel to be counted as on</param>
        /// <param name="minLineLenght"> the minimum length for a line to be considered a line segment</param>
        /// <param name="maxLineGap"> the maximum gap in the line</param>
        /// <returns>a list of start/end (x,y) coordinates</returns>
        private List<LineSegment> houghLineDetection(
            byte[,] inputImage,
            Point rThetaPair,
            byte minIntensityThreshold,
            int minLineLenght,
            int maxLineGap)
        {
            // Check input
            if (minLineLenght <= 1)
                throw new ArgumentException("houghLineDetection got an invalid minLineLength");

            // Create the list of line segments
            List<LineSegment> lineSegmentList = new List<LineSegment>();

            // Extract the r and theta values
            double inputR = rThetaPair.Y;
            double inputTheta = rThetaPair.X;

            // Create the variables for in the loop
            Point? startPoint = null;
            Point? endPoint = null;
            bool startNewLine = true;

            if (inputTheta / 4d < 80 || inputTheta / 4d > 100) // Line is mostly vertical
                lineDetectYAxis();
            else // Line is mostly horizontal
                lineDetectXAxis();

            return lineSegmentList;

            void lineDetectYAxis()
            {
                for (int y = 0; y < inputImage.GetLength(1); y++) // loop over rows
                {
                    // Get the x corresponding to this y on the given line
                    int x = (int) calcLineX(y, inputTheta, inputR);

                    // Discard the x and y if the x is outside the image
                    if (x < 0 || x >= inputImage.GetLength(0))
                        continue;

                    // Check if this current (x, y) coord is 'on'
                    if (yAxisInputImageAtCoordIsOn(x, y))
                        applyLineLogic(x, y);
                    else // There is no acceptable value at this xy coord
                        applyGapLogic(x, y);
                }
            }

            void lineDetectXAxis()
            {
                for (int x = 0; x < inputImage.GetLength(0); x++) // loop over columns
                {
                    // Get the x corresponding to this y on the given line
                    int y = (int) calcLineY(x, inputTheta, inputR);

                    // Discard the x and y if the x is outside the image
                    if (y < 0 || y >= inputImage.GetLength(1))
                        continue;

                    // Check if this current (x, y) coord is 'on'
                    if (xAxisInputImageAtCoordIsOn(x, y))
                        applyLineLogic(x, y);
                    else // There is no acceptable value at this xy coord
                        applyGapLogic(x, y);
                }
            }

            bool yAxisInputImageAtCoordIsOn(int x, int y)
            {
                return inputImage[x, y] >= minIntensityThreshold // check the current pixel
                       || inputImage[x, y != 0 ? y - 1 : y] >= minIntensityThreshold // check the pixel above
                       || inputImage[x, y != inputImage.GetLength(1) - 1 ? y + 1 : y] >=
                       minIntensityThreshold; // check the pixel below
            }

            bool xAxisInputImageAtCoordIsOn(int x, int y)
            {
                return inputImage[x, y] >= minIntensityThreshold // check the current pixel
                       || inputImage[x != 0 ? x - 1 : x, y] >= minIntensityThreshold // check the pixel to the left
                       || inputImage[x != inputImage.GetLength(0) - 1 ? x + 1 : x, y] >=
                       minIntensityThreshold; // check the pixel to the right
            }

            void applyLineLogic(int x, int y)
            {
                // Check if a new line needs to be started, and do so if true
                if (startNewLine)
                {
                    // Set this point to be the starting point of this line segment
                    startPoint = new Point(x, y);
                    endPoint = new Point(x, y);

                    // Let the program know a new line has been started
                    startNewLine = false;
                }
                else // The program is already working on a line
                {
                    // Store the current point as the endpoint of the line
                    endPoint = new Point(x, y);
                }
            }

            void applyGapLogic(int x, int y)
            {
                // Handle empty space in the image
                if (startNewLine) return;

                // Check if an error occured
                if (endPoint is null)
                    throw new Exception("houghLineDetection, something went wrong");

                // If the current gap exceeds the maximum line gap, check if a complete segment has been made
                if (calcLineLength((Point) endPoint, new Point(x, y)) > maxLineGap)
                {
                    // Check if the line is sufficient and add it to the list if needed
                    checkIfLineAndAddToList();

                    // Let the program know that a new line needs to be started
                    startNewLine = true;
                    startPoint = null;
                    endPoint = null;
                }
            }

            int calcLineLength(Point start, Point end)
            {
                return (int) Math.Round(Math.Sqrt(Math.Pow(end.X - start.X, 2) + Math.Pow(end.Y - start.Y, 2)));
            }

            void checkIfLineAndAddToList()
            {
                // Check if the starting point and end point have been defined
                if (startPoint is null || endPoint is null)
                    throw new Exception($"houghLineDetection, Something went wrong");

                Point startPointReal = (Point) startPoint;
                Point endPointReal = (Point) endPoint;

                // If the current line length is equal to or above the minimum length, store this segment in the list 
                if (calcLineLength(startPointReal, endPointReal) >= minLineLenght)
                {
                    // Add this line segment to the list
                    lineSegmentList.Add(new LineSegment(startPointReal, endPointReal, inputR, inputTheta));
                }
            }
        }

        /// <summary>
        /// Gets a list of line segments which run across the given r theta pair
        /// </summary>
        /// <param name="inputImage"> binary image</param>
        /// <param name="rThetaPair"> the r theta pair depicting the line</param>
        /// <param name="minLineLenght"> the minimum length for a line to be considered a line segment</param>
        /// <param name="maxLineGap"> the maximum gap in the line</param>
        /// <returns>a list of start/end (x,y) coordinates</returns>
        private List<LineSegment> houghLineDetection(
            BinaryImage inputImage,
            Point rThetaPair,
            int minLineLenght,
            int maxLineGap)
        {
            // Check input
            if (minLineLenght <= 1)
                throw new ArgumentException("houghLineDetection got an invalid minLineLength");

            // Create the list of line segments
            List<LineSegment> lineSegmentList = new List<LineSegment>();

            // Extract the r and theta values
            double inputR = rThetaPair.Y;
            double inputTheta = rThetaPair.X;

            // Create the variables for in the loop
            Point? startPoint = null;
            Point? endPoint = null;
            bool startNewLine = true;

            if (inputTheta / 4d < 80 || inputTheta / 4d > 100) // Line is mostly vertical
                lineDetectYAxis();
            else // Line is mostly horizontal
                lineDetectXAxis();

            return lineSegmentList;

            void lineDetectYAxis()
            {
                for (int y = 0; y < inputImage.YSize; y++) // loop over rows
                {
                    // Get the x corresponding to this y on the given line
                    int x = (int) calcLineX(y, inputTheta, inputR);

                    // Discard the x and y if the x is outside the image
                    if (x < 0 || x >= inputImage.XSize)
                        continue;

                    // Check if this current (x, y) coord is 'on'
                    if (yAxisInputImageAtCoordIsOn(x, y))
                        applyLineLogic(x, y);
                    else // There is no acceptable value at this xy coord
                        applyGapLogic(x, y);
                }

                if (!startNewLine)
                    checkIfLineAndAddToList();
            }

            void lineDetectXAxis()
            {
                for (int x = 0; x < inputImage.XSize; x++) // loop over columns
                {
                    // Get the x corresponding to this y on the given line
                    int y = (int) calcLineY(x, inputTheta, inputR);

                    // Discard the x and y if the x is outside the image
                    if (y < 0 || y >= inputImage.YSize)
                        continue;

                    // Check if this current (x, y) coord is 'on'
                    if (xAxisInputImageAtCoordIsOn(x, y))
                        applyLineLogic(x, y);
                    else // There is no acceptable value at this xy coord
                        applyGapLogic(x, y);
                }

                if (!startNewLine)
                    checkIfLineAndAddToList();
            }

            bool yAxisInputImageAtCoordIsOn(int x, int y)
            {
                return inputImage.GetPixelBool(x, y) // check the current pixel
                       || (inputImage.GetPixelBool(x, y != 0 ? y - 1 : y) && !startNewLine) // check the pixel above
                       || (inputImage.GetPixelBool(x, y != inputImage.YSize - 1 ? y + 1 : y) &&
                           !startNewLine); // check the pixel below
            }

            bool xAxisInputImageAtCoordIsOn(int x, int y)
            {
                return inputImage.GetPixelBool(x, y) // check the current pixel
                       || (inputImage.GetPixelBool(x != 0 ? x - 1 : x, y) &&
                           !startNewLine) // check the pixel to the left
                       || (inputImage.GetPixelBool(x != inputImage.XSize - 1 ? x + 1 : x, y) &&
                           !startNewLine); // check the pixel to the right
            }

            void applyLineLogic(int x, int y)
            {
                // Check if a new line needs to be started, and do so if true
                if (startNewLine)
                {
                    // Set this point to be the starting point of this line segment
                    startPoint = new Point(x, y);
                    endPoint = new Point(x, y);

                    // Let the program know a new line has been started
                    startNewLine = false;
                }
                else // The program is already working on a line
                {
                    // Store the current point as the endpoint of the line
                    endPoint = new Point(x, y);
                }
            }

            void applyGapLogic(int x, int y)
            {
                // Handle empty space in the image
                if (startNewLine) return;

                // Check if an error occured
                if (endPoint is null)
                    throw new Exception("houghLineDetection, something went wrong");

                // If the current gap exceeds the maximum line gap, check if a complete segment has been made
                if (calcLineLength((Point) endPoint, new Point(x, y)) > maxLineGap)
                {
                    // Check if the line is sufficient and add it to the list if needed
                    checkIfLineAndAddToList();

                    // Let the program know that a new line needs to be started
                    startNewLine = true;
                    startPoint = null;
                    endPoint = null;
                }
            }

            int calcLineLength(Point start, Point end)
            {
                return (int) Math.Round(Math.Sqrt(Math.Pow(end.X - start.X, 2) + Math.Pow(end.Y - start.Y, 2)));
            }

            void checkIfLineAndAddToList()
            {
                // Check if the starting point and end point have been defined
                if (startPoint is null || endPoint is null)
                    throw new Exception($"houghLineDetection, Something went wrong");

                Point startPointReal = (Point) startPoint;
                Point endPointReal = (Point) endPoint;

                // If the current line length is equal to or above the minimum length, store this segment in the list 
                if (calcLineLength(startPointReal, endPointReal) >= minLineLenght)
                {
                    // Add this line segment to the list
                    lineSegmentList.Add(new LineSegment(startPointReal, endPointReal, inputR, inputTheta));
                }
            }
        }

        /// <summary>
        /// Plots a line in the given binary image, using Bresenham's line algorithm
        /// </summary>
        /// <param name="inputImage"> The binary image in which the line should be plotted</param>
        /// <param name="startPoint">The start point of the line</param>
        /// <param name="endPoint">The end point of the line</param>
        /// <returns>A binary image with the line plotted in</returns>
        private BinaryImage plotLineBresenham(BinaryImage inputImage, Point startPoint, Point endPoint)
        {
            // Initiate the needed variables
            int xDelta = Math.Abs(endPoint.X - startPoint.X);
            int xSigma = startPoint.X < endPoint.X ? 1 : -1;
            int yDelta = -Math.Abs(endPoint.Y - startPoint.Y);
            int ySigma = startPoint.Y < endPoint.Y ? 1 : -1;
            int errorMargin = xDelta + yDelta;

            // Iterate until we reach the end point
            while (!(startPoint.X == endPoint.X && startPoint.Y == endPoint.Y))
            {
                // Turn the current pixel on in the input image
                inputImage.Fill(startPoint.X, startPoint.Y, true);

                // Calculate the next x,y coordinates which are on this line
                var localErrorMargin = 2 * errorMargin;
                if (localErrorMargin >= yDelta)
                {
                    errorMargin += yDelta;
                    startPoint.X += xSigma;
                }

                if (localErrorMargin <= xDelta)
                {
                    errorMargin += xDelta;
                    startPoint.Y += ySigma;
                }
            }

            return inputImage;
        }

        /// <summary>
        /// Superimpose the given line segments on the given input image
        /// </summary>
        /// <param name="inputImage">a binary image</param>
        /// <param name="lineSegmentList">the list of line segments</param>
        /// <returns>a binary image</returns>
        private BinaryImage visualiseHoughLineSegments(BinaryImage inputImage,
            List<LineSegment> lineSegmentList)
        {
            // Create a binary image to store all the lines
            BinaryImage lineImage = new BinaryImage(inputImage.XSize, inputImage.YSize);

            // Iterate over all the line segments
            foreach (var lineSegment in lineSegmentList)
            {
                // Plot the line in the line image
                lineImage = plotLineBresenham(lineImage, lineSegment.Point1, lineSegment.Point2);
            }

            // Take the OR of the input image and the line image, so the line image is superimposed on the input image
            return inputImage.OR(lineImage);
        }

        /// <summary>
        /// Superimpose the given line segments on the given input image
        /// </summary>
        /// <param name="inputImage">single chanel (byte) image </param>
        /// <param name="lineSegmentList">the list of line segments</param>
        /// <returns>single chanel (byte) image</returns>
        private byte[,] visualiseHoughLineSegments(byte[,] inputImage, List<LineSegment> lineSegmentList)
        {
            // Create a temporary output image, which is a copy of the input image
            byte[,] outputImage = inputImage;

            // Create a binary image to store all the lines
            BinaryImage lineImage = new BinaryImage(inputImage.GetLength(0), inputImage.GetLength(1));

            // Iterate over all the line segments
            foreach (var lineSegment in lineSegmentList)
            {
                // Plot the line in the line image
                lineImage = plotLineBresenham(lineImage, lineSegment.Point1, lineSegment.Point2);
            }

            // Iterate over the output image
            for (int y = 0; y < outputImage.GetLength(1); y++)
            for (int x = 0; x < outputImage.GetLength(0); x++)
            {
                // If the line image holds a value (and thus a pixel in a line) at the current coordinate, set the value to 255
                if (lineImage.GetPixelBool(x, y))
                {
                    outputImage[x, y] = 255;
                }
            }

            return outputImage;
        }

        /// <summary>
        /// Superimpose the given line segments on the given input image in the given color
        /// </summary>
        /// <param name="inputBitmap"> The input Bitmap</param>
        /// <param name="inputImage">single chanel (byte) image </param>
        /// <param name="lineSegmentList">the list of line segments</param>
        /// <param name="color"> The color to be used</param>
        /// <returns>single chanel (byte) image</returns>
        private Bitmap visualiseHoughLineSegmentsColors(Bitmap inputBitmap, byte[,] inputImage,
            List<LineSegment> lineSegmentList, Color color)
        {
            // Create a binary image to store all the lines
            BinaryImage lineImage = new BinaryImage(inputImage.GetLength(0), inputImage.GetLength(1));

            // Iterate over all the line segments
            foreach (var lineSegment in lineSegmentList)
            {
                // Plot the line in the line image
                lineImage = plotLineBresenham(lineImage, lineSegment.Point1, lineSegment.Point2);
            }

            // Iterate over the output image
            for (int y = 0; y < inputImage.GetLength(1); y++)
            for (int x = 0; x < inputImage.GetLength(0); x++)
            {
                // If the line image holds a value (and thus a pixel in a line) at the current coordinate, set the value to 255
                if (lineImage.GetPixelBool(x, y))
                {
                    inputBitmap.SetPixel(x, y, color); // set the pixel color at coordinate (x,y)
                }
            }

            return inputBitmap;
        }

        /// <summary>
        /// Draws the lines corresponding to the found r theta pairs to the image
        /// </summary>
        /// <param name="image"> The bitmap to draw in</param>
        /// <param name="centers"> The r theta pairs</param>
        /// <param name="color"> The color to draw the line in</param>
        private Bitmap drawFoundLines(Bitmap image, List<Point> centers, Color color)
        {
            foreach (var center in centers)
            {
                if (center.X / 4d > 100 || center.X / 4d < 80)
                {
                    for (int y = 0; y < image.Height; y++) // loop over columns
                    {
                        int x = (int) calcLineX(y, center.X, center.Y);

                        // Discard the x and y if the x is outside the image
                        if (x < 0 || x >= image.Width)
                            continue;

                        OutputImage.SetPixel(x, y, color);
                    }
                }
                else
                {
                    for (int x = 0; x < image.Width; x++) // loop over columns
                    {
                        int y = (int) calcLineY(x, center.X, center.Y);

                        // Discard the x and y if the x is outside the image
                        if (y < 0 || y >= image.Height)
                            continue;

                        OutputImage.SetPixel(x, y, color);
                    }
                }
            }

            return OutputImage;
        }
        /// <summary>
        /// Draws the circles corresponding to the found center point-radius pairs to the image
        /// </summary>
        /// <param name="image"> The bitmap to draw in</param>
        /// <param name="circles"> The circle center and radius</param>
        /// <param name="color"> The color to draw the line in</param>
        private Bitmap drawFoundCircles(Bitmap image, List<Circle> circles, Color color)
        {
            foreach (var circle in circles)
            {
                for (int t = 0; t < 360; t++)
                {
                    int x = (int)Math.Round(circle.Radius * Math.Cos((Math.PI / 180) * t)) + circle.Center.X;
                    int y = (int)Math.Round(circle.Radius * Math.Sin((Math.PI / 180) * t)) + circle.Center.Y;
                    if (x > 0 && y > 0 && x < image.Width && y < image.Height)
                        image.SetPixel(x, y, color);
                }
            }

            return image;
        }

        /// <summary>
        /// Visualise the crossings as a size x size square in the given color
        /// </summary>
        /// <param name="inputBitmap"> The bitmap to draw to</param>
        /// <param name="threshold"> The threshold for the amount of lines that cross</param>
        /// <param name="size"> The size of the drawn square</param>
        /// <param name="rThetaPairs"> The r theta pairs</param>
        /// <param name="color"> The color to draw the square in</param>
        private Bitmap visualiseCrossingsColor(Bitmap inputBitmap, byte threshold, int size, List<Point> rThetaPairs, Color color)
        {
            // Input checking
            if (size % 2 == 0)
                throw new ArgumentException("visualiseCrossingsColor got an even size");

            // Calculate all the crossings in the image
            List<Point> crossingList = findCrossings(inputBitmap, rThetaPairs, threshold, inputBitmap.Width, inputBitmap.Height);

            // Draw the crossings to the bitmap in a size x size square around the crossing
            foreach (var crossing in crossingList)
            {
                for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    int refX = crossing.X + (x - (size / 2));
                    int refY = crossing.Y + (y - (size / 2));
                    refX = Math.Min(Math.Max(refX, 0), inputBitmap.Width - 1);
                    refY = Math.Min(Math.Max(refY, 0), inputBitmap.Height - 1);

                    inputBitmap.SetPixel(refX, refY, color);
                }
            }

            return inputBitmap;
        }

        /// <summary>
        /// Finds all the points where at least 2 lines cross eachother
        /// </summary>
        /// <param name="inputBitmap"> The bitmap to draw to for reference</param>
        /// <param name="rThetaPairs"> The r theta pairs of the image</param>
        /// <param name="threshold"> The threshold for the amount of lines that cross</param>
        /// <param name="xSize"> The x size of the image</param>
        /// <param name="ySize"> The y size of the image</param>
        private List<Point> findCrossings(Bitmap inputBitmap, List<Point> rThetaPairs, byte threshold, int xSize, int ySize)
        {
            // Create the accumulator array
            int[,] accumulatorArray = new int[xSize, ySize];

            // Iterate over the r theta pairs and plot them in the accumulator array
            foreach (var rThetaPair in rThetaPairs)
            {
                // Extract the r and theta values
                double inputR = rThetaPair.Y;
                double inputTheta = rThetaPair.X;

                // Draw the line in the accumulator array
                drawLineInAccArray(inputR, inputTheta);
            }

            // Create a temporary binary image
            BinaryImage tempImage = new BinaryImage(thresholdImage(accumulatorArray, threshold));

            // Find the centers (and thus the crossings)
            List<Point> centers = new List<Point>();

            // Check if there are at least 'threshold' amount of lines with pixels set to 'on' at the crossing
            foreach (var crossing in findCenterPoints(tempImage, 0))
            {
                if(accumulatorArray[crossing.X, crossing.Y] >= threshold && inputBitmap.GetPixel(crossing.X, crossing.Y) != Color.Empty)
                    centers.Add(crossing);
            }
            
            return centers;

            // Draw the rThetaPair in the accumulator array
            void drawLineInAccArray(double r, double theta)
            {
                if (theta / 4d < 80 || theta / 4d > 100) // Line is mostly vertical
                    drawLineYAxis();
                else // Line is mostly horizontal
                    drawLineXAxis();

                void drawLineYAxis()
                {
                    for (int y = 0; y < ySize; y++) // loop over rows
                    {
                        // Get the x corresponding to this y on the given line
                        int x = (int) calcLineX(y, theta, r);

                        // Discard the x and y if the x is outside the image
                        if (x < 0 || x >= xSize)
                            continue;

                        // Write the coordinate to the accumulator array
                        accumulatorArray[x, y] += accumulatorArray[x, y] != int.MaxValue ? (byte) 1 : (byte) 0;
                    }
                }

                void drawLineXAxis()
                {
                    for (int x = 0; x < xSize; x++) // loop over columns
                    {
                        // Get the x corresponding to this y on the given line
                        int y = (int) calcLineY(x, theta, r);

                        // Discard the x and y if the x is outside the image
                        if (y < 0 || y >= ySize)
                            continue;

                        // Write the coordinate to the accumulator array
                        accumulatorArray[x, y] += accumulatorArray[x, y] != int.MaxValue ? (byte) 1 : (byte) 0;
                    }
                }
            }
        }

        private List<LineSegment> pruneLineSegments(List<LineSegment> lineSegments)
        {
            LineSegment[] lsArray = lineSegments.ToArray();
            for (int i = 0; i < lsArray.Length; i++) // Iterate over the line segments
            for (int j = 0; j < lsArray.Length; j++) // Iterate over the line segments
            {
                if (i == j) // no check needed if same line
                    continue;
                if (lsArray[i] is null || lsArray[j] is null) // no check needed if either one is null
                    continue;
                if (lsArray[i].IsSimilarTo(lsArray[j])) // check if the lines are similar
                {
                    // If another line is found which is similar to lsArray[i], set lsArray[i] to null and continue to next i
                    lsArray[i] = null;
                    break;
                }
            }

            List<LineSegment> output = lsArray.ToList();

            return output.Where(ls => ls is not null).ToList();
        }
        
        private List<Circle> pruneCircleList(List<Circle> circles, int centerMargin, int radiusMargin)
        {
            Circle[] lsArray = circles.ToArray();
            for (int i = 0; i < lsArray.Length; i++) // Iterate over the circles
            for (int j = 0; j < lsArray.Length; j++) // Iterate over the circles
            {
                if (i == j) // no check needed if same line
                    continue;
                if (lsArray[i] is null || lsArray[j] is null) // no check needed if either one is null
                    continue;
                if (CheckSimilar(lsArray[i], lsArray[j])) // check if the circles are similar
                {
                    // If another circle is found which is similar to lsArray[i], set lsArray[i] to null and continue to next i
                    lsArray[i] = null;
                    break;
                }
            }

            List<Circle> output = lsArray.ToList();

            return output.Where(ls => ls is not null).ToList();

            bool CheckSimilar(Circle c1, Circle c2)
            {
                if (Math.Abs(c1.Radius - c2.Radius) < radiusMargin)
                {
                    if (Math.Abs(c1.Center.X - c2.Center.X) < centerMargin &&
                        Math.Abs(c1.Center.Y - c2.Center.Y) < centerMargin)
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Find all line segments in the given list of which either one of the points are on the given circle.
        /// </summary>
        /// <param name="circle"> The circle to compare each line segment to</param>
        /// <param name="lineSegments"> The line segments to check</param>
        /// <param name="margin"> The distance that the line can be removed from the circle while still counting</param>
        /// <returns> A list of line segments which start or end on the given circle </returns>
        private List<LineSegment> findLineSegmentsThatStartOnCircle(Circle circle, List<LineSegment> lineSegments, double margin)
        {
            // Create a temporary list for all found line segments
            List<LineSegment> found = new List<LineSegment>();
            
            // Iterate over the given line segments
            foreach (var ls in lineSegments)
            {
                // Add this line segment to the found list if either of its points are on the circle within the given margin
                if(circle.isPointOnCircle(ls.Point1, margin) || circle.isPointOnCircle(ls.Point2, margin))
                    found.Add(ls);
            }

            return found;
        }

        private List<HPGlasses> findConnectedCircles(List<Circle> circles, List<LineSegment> lineSegments,
            double margin)
        {
            // This function is only valid when there are at least 1 circle
            if (circles.Count < 1) return null;
            
            // Catch edge case, no line segments and only 1 circle
            if (circles.Count == 1 && lineSegments.Count < 1)
                // Add the only circle as a HPGlasses
                return new List<HPGlasses>() { new HPGlasses(circles[0])};
            
            // Catch edge case, no line segments and 2 circles
            if (circles.Count == 2 && lineSegments.Count < 1)
                // Add the only 2 circles as a HPGlasses
                return new List<HPGlasses>() { new HPGlasses(circles[0], null, circles[1])};
            
            // Create a list of found HPGlasses
            List<HPGlasses> foundGlasses = new List<HPGlasses>();
            
            // Iterate over all circles
            for (int circleIndex = 0; circleIndex < circles.Count; circleIndex++)
            {
                // Initialise the variable for this HPGlasses
                Circle circle1 = circles[circleIndex];
                
                // Find all line segments that start or end on this circle
                List<LineSegment> lsOnCircle = findLineSegmentsThatStartOnCircle(circle1, lineSegments, margin);

                // If there are no line segments crossing this circle, add it to the list and continue
                if (lsOnCircle.Count == 0)
                {
                    foundGlasses.Add(new HPGlasses(circle1));
                    continue;
                }
                
                // Iterate over all circles to find other circles which might make a pair
                for (int internalCircleIndex = 0; internalCircleIndex < circles.Count; internalCircleIndex++)
                {
                    // Only check other circles
                    if(internalCircleIndex == circleIndex)
                        continue;
                    
                    // Get all line segments that connect this circle with the overarching circle
                    List<LineSegment> lsThatConnectTwoCircles = findLineSegmentsThatStartOnCircle(circles[internalCircleIndex], lsOnCircle, margin);
                    
                    // Prune the list of line segments to make sure all of them actually connect the circles
                    lsThatConnectTwoCircles = PruneLsThatConnectTwoCircles(circle1, circles[internalCircleIndex],
                        lsThatConnectTwoCircles);
                    
                    // Check if there are any line segments connecting these circles, continue to the next inner-loop circle if not
                    if (lsThatConnectTwoCircles.Count == 0) continue;

                    // Store this circle as circle2 of the HPGlasses
                    Circle circle2 = circles[internalCircleIndex];

                    // Get the best nose bridge candidate and store it
                    LineSegment noseBridge = GetBestNoseBridge(circle1, circle2, lsThatConnectTwoCircles);
                    
                    // Find ear pieces if they exist
                    LineSegment earPiece1 = FindEarPiece(circle1, noseBridge, lsOnCircle);
                    LineSegment earPiece2 = FindEarPiece(circle2, noseBridge,
                        findLineSegmentsThatStartOnCircle(circle2, lineSegments, margin));
                    
                    // Store the current HPGlasses
                    HPGlasses hpgTemp = new HPGlasses(circle1, noseBridge, circle2, earPiece1, earPiece2);
                    
                    // Check if hpgTemp has not been found before, if new, add it to the found list
                    if(GlassesAreNew(hpgTemp))
                        foundGlasses.Add(hpgTemp);
                }
            }
            
            // prune the list of found glasses to remove duplicates
            PruneFoundGlasses();

            return foundGlasses;

            // Returns a list of line segments which actually connect the two given circles
            List<LineSegment> PruneLsThatConnectTwoCircles(Circle c1, Circle c2, List<LineSegment> lsList)
            {
                List<LineSegment> outputList = new List<LineSegment>();
                
                // Prune the list to make sure the line segments actually connect the two circles
                foreach (var ls in lsList)
                {
                    // Check if the line segments does indeed connect both circles
                    if (c1.isPointOnCircle(ls.Point1, margin) &&
                        c2.isPointOnCircle(ls.Point2, margin)
                        || c1.isPointOnCircle(ls.Point2, margin) &&
                        c2.isPointOnCircle(ls.Point1, margin))
                    {
                        outputList.Add(ls);
                    }
                }

                return outputList;
            }
            
            // Returns the best nose bridge candidate from a list of line segments which connect two circles
            LineSegment GetBestNoseBridge(Circle c1, Circle c2, List<LineSegment> lsList)
            {
                // Check edge case of empty list
                if (lsList.Count == 0)
                    return null;
                
                // Create a var to store the index of the best candidate
                int bestIndex = 0;
                
                // Calculate the slope of the line between the centers of the circles
                double c1X = c1.Center.X;
                double c1Y = c1.Center.Y;
                double c2X = c2.Center.X;
                double c2Y = c2.Center.Y;
                double slopeCircleLine = (c2Y - c1Y) / (c2X - c1X);
                
                // Store the lowest slope difference
                double lowestSlopeDiff = CalcSlopeDiff(slopeCircleLine, lsList[0]);
                
                // Iterate over all the lines
                for (int i = 1; i < lsList.Count; i++) // Starting at 1 because 0 is already stored
                {
                    // Calculate the slope difference of this line
                    double iSlopeDiff = CalcSlopeDiff(slopeCircleLine, lsList[i]);

                    // Check if it is lower than the stored value
                    if (iSlopeDiff < lowestSlopeDiff)
                    {
                        bestIndex = i;
                        lowestSlopeDiff = iSlopeDiff;
                    }
                    // If the slopes are very similar, take the shortest line
                    else if (Math.Abs(iSlopeDiff - lowestSlopeDiff) < 0.1 &&
                             lsList[i].Length < lsList[bestIndex].Length)
                    {
                        bestIndex = i;
                        lowestSlopeDiff = iSlopeDiff;
                    }
                }

                // Return the best candidate
                return lsList[bestIndex];
            }
            
            // Calculate the slope difference between the given slope and line
            double CalcSlopeDiff(double slopeCircleLine, LineSegment ls)
            {
                double ls1X = ls.Point1.X;
                double ls1Y = ls.Point1.Y;
                double ls2X = ls.Point2.X;
                double ls2Y = ls.Point2.Y;
                double slopeNoseBridge = (ls2Y - ls1Y) / (ls2X - ls1X);

                if (slopeCircleLine > 0 && slopeNoseBridge > 0)
                    return Math.Abs(slopeCircleLine - slopeNoseBridge);
                if (slopeCircleLine < 0 && slopeNoseBridge < 0)
                    return Math.Abs(-slopeCircleLine + slopeNoseBridge);
                if (slopeCircleLine < 0 && slopeNoseBridge > 0 || slopeCircleLine > 0 && slopeNoseBridge < 0)
                    return Math.Abs(slopeCircleLine) + Math.Abs(slopeNoseBridge);
                return 100d;
            }

            // Check whether the given pair of HPGlasses have not yet been found
            bool GlassesAreNew(HPGlasses givenHpg)
            {
                if (foundGlasses.Count == 0) return true;

                // Iterate over all found glasses to check if the given one is new
                foreach (var hpg in foundGlasses)
                {
                    if ((hpg.Circle1 == givenHpg.Circle1 && hpg.Circle2 == givenHpg.Circle2 ||
                         hpg.Circle1 == givenHpg.Circle2 && hpg.Circle2 == givenHpg.Circle1) &&
                        hpg.NoseBridge == givenHpg.NoseBridge)
                    {
                        return false;
                    }
                }

                return true;
            }

            // Prune the list of found glasses to remove duplicates
            void PruneFoundGlasses()
            {
                // Create an array to store the found glasses in
                HPGlasses[] hpgArray = foundGlasses.ToArray();

                // Iterate over the array
                for (int i = 0; i < hpgArray.Length; i++)
                for (int j = 0; j < hpgArray.Length; j++) // Iterate over the other values
                {
                    // Only compare different HPGlasses
                    if (i == j) continue;
                    
                    // If either is null, continue
                    if(hpgArray[i] is null || hpgArray[j] is null) continue;
                    
                    // Check if these glasses match
                    if (hpgArray[i].Circle1 == hpgArray[j].Circle1 || hpgArray[i].Circle1 == hpgArray[j].Circle2)
                    {
                        // Delete this i if the certainty values match or if the certainty value of i is lower
                        if (hpgArray[i].GetCertainty() <= hpgArray[j].GetCertainty())
                            hpgArray[i] = null;
                    }
                }

                // Return the list, which is pruned by removing the null values
                foundGlasses = hpgArray.ToList().Where(ls => ls is not null).ToList();
            }
            
            // Try to find an earpiece on the given circle
            LineSegment FindEarPiece(Circle c, LineSegment noseBridge, List<LineSegment> lsOnCircle)
            {
                // Handle the edge cases
                if (lsOnCircle.Count <= 1) return null; // Either no lines or only the nose bridge
                if (noseBridge is null) return null; // No nose bridge
                
                // Find the point where the nose bridge crosses the circle
                Point noseBridgeCrossing = new Point();
                if (c.isPointOnCircle(noseBridge.Point1, margin))
                    noseBridgeCrossing = noseBridge.Point1;
                else if (c.isPointOnCircle(noseBridge.Point2, margin))
                    noseBridgeCrossing = noseBridge.Point2;
                
                // Find the point on the circle, opposite the point where the nose bridge touches the circle
                Point oppositeOfNoseBridge = new Point();
                int vx = c.Center.X - noseBridgeCrossing.X;
                int vy = c.Center.Y - noseBridgeCrossing.Y;
                double length = Math.Sqrt(vx * vx + vy * vy);
                oppositeOfNoseBridge.X = (int) Math.Round(vx / length * c.Radius + c.Center.X);
                oppositeOfNoseBridge.Y = (int) Math.Round(vy / length * c.Radius + c.Center.Y);

                // Store the maximum allowed distance
                int maxDist = (Math.Abs(oppositeOfNoseBridge.X - noseBridgeCrossing.X) + Math.Abs(oppositeOfNoseBridge.Y - noseBridgeCrossing.Y)) * 3 / 4;
                
                // Create the iteration variables depicting the best candidate
                int indexOfClosest = -1;
                int minXDist = int.MaxValue / 2;
                int minYDist = int.MaxValue / 2;
                
                // Iterate over the given line segments
                for (int i = 0; i < lsOnCircle.Count; i++)
                {
                    // Store this line segment temporarily
                    LineSegment ls = lsOnCircle[i];
                    int iXDist = 0;
                    int iYDist = 0;
                    
                    // Calculate the distance from the point of the line on the circle to the point opposite the crossing point of the nose bridge
                    if (c.isPointOnCircle(ls.Point1, margin)) // Point 1 is on the circle
                    {
                        iXDist = Math.Abs(oppositeOfNoseBridge.X - ls.Point1.X);
                        iYDist = Math.Abs(oppositeOfNoseBridge.Y - ls.Point1.Y);
                    }
                    else if (c.isPointOnCircle(ls.Point2, margin)) // Point 2 is on the circle
                    {
                        iXDist = Math.Abs(oppositeOfNoseBridge.X - ls.Point2.X);
                        iYDist = Math.Abs(oppositeOfNoseBridge.Y - ls.Point2.Y);
                    }
                    
                    // Check if the ear piece is on the opposite end of the circle as the nose bridge
                    if(iXDist + iYDist > maxDist) continue;

                    // Check if the distance is smaller than the currently stored distance
                    if (iXDist + iYDist < minXDist + minYDist)
                    {
                        // Store this ls as the closest candidate
                        indexOfClosest = i;
                        minXDist = iXDist;
                        minYDist = iYDist;
                    }
                }

                // If no ear piece is found, return null
                if (indexOfClosest == -1) return null;
                // Else return the best candidate
                return lsOnCircle[indexOfClosest];
            }
        }

        /// <summary>
        /// Superimpose the given line segments on the given input image in the given color
        /// </summary>
        /// <param name="inputBitmap"> The input Bitmap</param>
        /// <param name="inputImage">single chanel (byte) image </param>
        /// <param name="hpGlassesList"> The list of hpGlasses</param>
        /// <param name="minCertainty"> The minimum certainty needed for a HPGlasses to be displayed</param>
        /// <param name="color"> The color to be used</param>
        /// <returns>single chanel (byte) image</returns>
        private Bitmap visualiseHPGlassesColor(Bitmap inputBitmap, byte[,] inputImage,
            List<HPGlasses> hpGlassesList, int minCertainty, Color color)
        {
            // Create a binary image to store all the lines
            BinaryImage lineImage = new BinaryImage(inputImage.GetLength(0), inputImage.GetLength(1));
            
            // Create a graphics component
            Graphics g = Graphics.FromImage(inputBitmap);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            
            // Iterate over all the line segments
            foreach (var hpg in hpGlassesList)
            {
                // Make sure only HPGlasses with a minimum certainty are drawn
                if (hpg.GetCertainty() < minCertainty) continue;
                
                // Get the corner values of the bounding box
                Point topL = new Point(hpg.GetMinXValue(), hpg.GetMinYValue());
                Point botL = new Point(hpg.GetMinXValue(), hpg.GetMaxYValue());
                Point topR = new Point(hpg.GetMaxXValue(), hpg.GetMinYValue());
                Point botR = new Point(hpg.GetMaxXValue(), hpg.GetMaxYValue());
                
                // Plot the lines of the bounding box in the line image
                lineImage = plotLineBresenham(lineImage, topL, topR); // top side
                lineImage = plotLineBresenham(lineImage, topL, botL); // left side
                lineImage = plotLineBresenham(lineImage, botL, botR); // bottom side
                lineImage = plotLineBresenham(lineImage, botR, topR); // right side
                
                // Draw the certainty percentage to the image
                g.DrawString($"{hpg.GetCertainty()}% sure", new Font("Tahoma", 8), Brushes.Yellow, new PointF(hpg.GetMinXValue() + 2, hpg.GetMaxYValue() - 13));
            }

            // Iterate over the output image
            for (int y = 0; y < inputImage.GetLength(1); y++)
            for (int x = 0; x < inputImage.GetLength(0); x++)
            {
                // If the line image holds a value (and thus a pixel in a line) at the current coordinate, set the value to the given color
                if (lineImage.GetPixelBool(x, y))
                {
                    inputBitmap.SetPixel(x, y, color); // set the pixel color at coordinate (x,y)
                }
            }

            return inputBitmap;
        }
        
    }
}