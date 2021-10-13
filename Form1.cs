using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;


namespace INFOIBV
{
    public partial class INFOIBV : Form
    {
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

            byte[,] workingImage = convertToGrayscale(Image); // convert image to grayscale
            countValues(workingImage);
            workingImage = thresholdImage(workingImage, 100);
            peakFinding(new BinaryImage(workingImage));
            workingImage = houghTranform(new BinaryImage(workingImage));
            workingImage = thresholdImage(workingImage, 10);

            workingImage = closeImage(workingImage, createStructuringElement(StructuringElementShape.Square, 3));
            
            //workingImage = thresholdImage(workingImage, 60);

            // ==================== END OF YOUR FUNCTION CALLS ====================
            // ====================================================================
            OutputImage = new Bitmap(workingImage.GetLength(0), workingImage.GetLength(1));
            // copy array to output Bitmap
            int count = 0;
            for (int x = 0; x < workingImage.GetLength(0); x++) // loop over columns
                for (int y = 0; y < workingImage.GetLength(1); y++) // loop over rows
                {
                    Color newColor = Color.FromArgb(workingImage[x, y], workingImage[x, y], workingImage[x, y]);
                    OutputImage.SetPixel(x, y, newColor); // set the pixel color at coordinate (x,y)
                }

            pictureBox2.Image = OutputImage; // display output image
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
                        (byte)((pixelColor.R + pixelColor.B + pixelColor.G) /
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
                    tempImage[x, y] = (byte)(255 - inputImage[x, y]);

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
                tempImage[inputX, inputY] = (byte)(255 - inputImage[inputX, inputY]);
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
            int amountIgnoredPixels = (int)(nPixels * percentageIgnoredValues);

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

            byte aHigh = (byte)(i + 1);

            // ignore the right amount of pixels and find the lowest
            ignoredPixels = 0;
            i = 0;
            while (ignoredPixels < amountIgnoredPixels)
            {
                ignoredPixels += histrogramValues[i];
                i++;
            }

            byte aLow = (byte)(i - 1);

            // calculate new values
            for (int y = 0; y < inputYSize; y++)
                for (int x = 0; x < inputXSize; x++)
                {
                    if (inputImage[x, y] > aHigh)
                        tempImage[x, y] = 255;
                    else if (inputImage[x, y] < aLow)
                        tempImage[x, y] = 0;
                    else
                        tempImage[x, y] = (byte)((inputImage[x, y] - aLow) * (255 / (aHigh - aLow)));
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
            int amountIgnoredPixels = (int)(nPixels * percentageIgnoredValues);

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

            byte aHigh = (byte)(i + 1);

            // ignore the right amount of pixels and find the lowest
            ignoredPixels = 0;
            i = 0;
            while (ignoredPixels < amountIgnoredPixels)
            {
                ignoredPixels += histrogramValues[i];
                i++;
            }

            byte aLow = (byte)(i - 1);

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
                    tempImage[inputX, inputY] = (byte)((inputImage[inputX, inputY] - aLow) * (255 / (aHigh - aLow)));
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
                    filter[x, y] = 1 / (2 * (float)Math.PI * (sigma * sigma)) *
                                   (float)Math.Pow(Math.E,
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
                        newPixel += (byte)(filter[kx, ky] *
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
                        newPixel += (byte)(filter[kx, ky] *
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
                    int medianIndex = (int)Math.Ceiling(((double)(size * size) / 2));
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
                int medianIndex = (int)Math.Ceiling(((double)(size * size) / 2));
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
                    tempImage[x, y] = (byte)answer;
                }

            return tempImage;

            // Apply the sobel kernel to the given image kernel
            double calcValue(double[,] sobelKernel, byte[,] kernel)
            {
                double total = 0;
                for (int y = 0; y < 3; y++)
                    for (int x = 0; x < 3; x++)
                        total += sobelKernel[x, y] * (double)kernel[x, y];

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
                tempImage[inputX, inputY] = (byte)answer;
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
                        total += sobelKernel[x, y] * (double)kernel[x, y];

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
                probability[i] = histrogramValues[i] / (double)totalPixels;

            //Calculate cumulative propability
            double[] cumulativeProbability = new double[256];
            cumulativeProbability[0] = probability[0];
            for (int i = 1; i <= 255; i++)
                cumulativeProbability[i] = cumulativeProbability[i - 1] + probability[i];

            //Multiply by 255
            byte[] newValues = new byte[256];
            for (int i = 1; i <= 255; i++)
                newValues[i] = (byte)(cumulativeProbability[i] * 255);

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
                    {0,   255, 0  },
                    {255, 255, 255},
                    {0,   255, 0  }
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
                            ? (byte)255 // current coordinate is part of the center 3x3 plus
                            : (byte)0;  // current coordinate is not part of the center 3x3 plus
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
                throw new ArgumentException("erodeImage got a control image which did not match the input image in size");

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

                        // Check if the reference coordinates are out of bounds and add 0 (background) to the valList if they are
                        if (refX < 0 || refX >= xSize)
                            valList.Add(0);
                        else if (refY < 0 || refY >= ySize)
                            valList.Add(0);
                        else
                            // Add the value to the valList with respect to the structuring element
                            valList.Add(structuringElement[seX, seY] == 255 ? input[refX, refY] : (byte)255);
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
                throw new ArgumentException("erodeImage got a control image which did not match the input image in size");

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

                        // Check if the reference coordinates are out of bounds and add 0 (background) to the valList if they are
                        if (refX < 0 || refX >= xSize)
                            valList.Add(0);
                        else if (refY < 0 || refY >= ySize)
                            valList.Add(0);
                        else
                            // Add the value to the valList with respect to the structuring element
                            valList.Add(structuringElement[seX, seY] == 255 ? input[refX, refY] : (byte)255);
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
                throw new ArgumentException("erodeImage got a control image which did not match the input image in size");

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
                            valList.Add(structuringElement[seX, seY] == 255 ? input[refX, refY] : (byte)0);
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
                throw new ArgumentException("erodeImage got a control image which did not match the input image in size");

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
                            valList.Add(structuringElement[seX, seY] == 255 ? input[refX, refY] : (byte)0);
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
                curIteration = erodeImageParallel(curIteration, createStructuringElement(StructuringElementShape.Square, erodeSize));
            }
            // After this loop, we know the previous iteration only holds a pixel that was contained in the largest object

            // Reset the variables
            curIteration = prevIteration;
            prevIteration = null;

            // Geodisic dilate the image until stable, where the largest object is back to its original size
            while (!checkIfSamePicture(curIteration, prevIteration))
            {
                prevIteration = curIteration;
                curIteration = dilateImageParallel(curIteration, createStructuringElement(StructuringElementShape.Square, dilateSize),
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
        /// builds a hough transform image out of a binary image
        /// </summary>
        /// <param name="inputImage">binary image</param>
        /// <returns>single-channel hough tranform (byte) image</returns>
        private byte[,] houghTranform(BinaryImage inputImage)
        {
            int maxDistance = (int)Math.Ceiling(Math.Sqrt(Math.Pow(inputImage.XSize, 2) + Math.Pow(inputImage.YSize, 2)));
            byte[,] paramSpaceArray = new byte[179, maxDistance * 2 + 1];
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
                for (int i = 0; i < 179; i += 1)
                {
                    double r = x * Math.Cos(Math.PI * i / 180) + y * Math.Sin(Math.PI * i / 180);
                    //paramSpaceArray[i, (int)r + maxDistance] += 1;
                    paramSpaceArray[i, (int)r + maxDistance] += (paramSpaceArray[i, (int)r + maxDistance] == (byte) 255) ? (byte)0 : (byte)1;
                }
            }
        }
        /// <summary>
        /// finds peaks in a hough transform image
        /// </summary>
        /// <param name="inputImage">binary image</param>
        /// <returns>tuple of r-theta pairs where peaks are found</returns>
        private List<Point> peakFinding(BinaryImage inputImage)
        {
            byte[,] imageByte = houghTranform(inputImage);
            //remove all unecessary data
            BinaryImage image = new BinaryImage(thresholdImage(imageByte, 10));
            //close image
            image = new BinaryImage(closeImage(image.GetImage(), createStructuringElement(StructuringElementShape.Square, 3)));

            //find regions with BFS
            bool[,] foundPixels = new bool[image.XSize, image.YSize];
            List<List<Point>> regions = new List<List<Point>>();
            for (int y = 0; y < image.YSize; y++)
            {
                for (int x = 0; x < image.XSize; x++)
                {
                    if (image.GetPixelBool(x,y) && !foundPixels[x,y])
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
                centers.Add(new Point((int)(xTotal / region.Count), (int)(yTotal / region.Count)));
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
                        if (neighbour.X >=0 && neighbour.X < image.XSize && neighbour.Y >= 0 && neighbour.Y < image.YSize &&
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
    }
}