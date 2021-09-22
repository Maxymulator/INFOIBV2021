using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading.Tasks;

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
           if (openImageDialog.ShowDialog() == DialogResult.OK)             // open file dialog
            {
                string file = openImageDialog.FileName;                     // get the file name
                imageFileName.Text = file;                                  // show file name
                if (InputImage != null) InputImage.Dispose();               // reset image
                InputImage = new Bitmap(file);                              // create new Bitmap from file
                if (InputImage.Size.Height <= 0 || InputImage.Size.Width <= 0 ||
                    InputImage.Size.Height > 512 || InputImage.Size.Width > 512) // dimension check (may be removed or altered)
                    MessageBox.Show("Error in image dimensions (have to be > 0 and <= 512)");
                else
                    pictureBox1.Image = (Image) InputImage;                 // display input image
            }
        }


        /*
         * applyButton_Click: process when user clicks "Apply" button
         */
        private void applyButton_Click(object sender, EventArgs e)
        {
            if (InputImage == null) return;                                 // get out if no input image
            if (OutputImage != null) OutputImage.Dispose();                 // reset output image
            OutputImage = new Bitmap(InputImage.Size.Width, InputImage.Size.Height); // create new output image
            Color[,] Image = new Color[InputImage.Size.Width, InputImage.Size.Height]; // create array to speed-up operations (Bitmap functions are very slow)

            // copy input Bitmap to array            
            for (int x = 0; x < InputImage.Size.Width; x++)                 // loop over columns
                for (int y = 0; y < InputImage.Size.Height; y++)            // loop over rows
                    Image[x, y] = InputImage.GetPixel(x, y);                // set pixel color in array at (x,y)

            // ====================================================================
            // =================== YOUR FUNCTION CALLS GO HERE ====================
            // Alternatively you can create buttons to invoke certain functionality
            // ====================================================================

            Stopwatch stopwatch = Stopwatch.StartNew();
            byte[,] workingImage = convertToGrayscale(Image);          // convert image to grayscale
            //workingImage = invertImage(workingImage);
            
            stopwatch = Stopwatch.StartNew();
            //workingImage = convolveImageParallel(workingImage, createGaussianFilter(9, 10f));
            workingImage = histrogramEqualization(workingImage);

            stopwatch.Stop();
            Debug.WriteLine($@"Total time in milliseconds : {stopwatch.ElapsedMilliseconds}");


            // ==================== END OF YOUR FUNCTION CALLS ====================
            // ====================================================================

            // copy array to output Bitmap
            for (int x = 0; x < workingImage.GetLength(0); x++)             // loop over columns
                for (int y = 0; y < workingImage.GetLength(1); y++)         // loop over rows
                {
                    Color newColor = Color.FromArgb(workingImage[x, y], workingImage[x, y], workingImage[x, y]);
                    OutputImage.SetPixel(x, y, newColor);                  // set the pixel color at coordinate (x,y)
                }
            
            pictureBox2.Image = (Image)OutputImage;                         // display output image
        }


        /*
         * saveButton_Click: process when user clicks "Save" button
         */
        private void saveButton_Click(object sender, EventArgs e)
        {
            if (OutputImage == null) return;                                // get out if no output image
            if (saveImageDialog.ShowDialog() == DialogResult.OK)
                OutputImage.Save(saveImageDialog.FileName);                 // save the output image
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
            for (int x = 0; x < InputImage.Size.Width; x++)                 // loop over columns
                for (int y = 0; y < InputImage.Size.Height; y++)            // loop over rows
                {
                    Color pixelColor = inputImage[x, y];                    // get pixel color
                    byte average = (byte)((pixelColor.R + pixelColor.B + pixelColor.G) / 3); // calculate average over the three channels
                    tempImage[x, y] = average;                              // set the new pixel color at coordinate (x,y)
                    progressBar.PerformStep();                              // increment progress bar
                }

            progressBar.Visible = false;                                    // hide progress bar

            return tempImage;
        }


        // ====================================================================
        // ============= YOUR FUNCTIONS FOR ASSIGNMENT 1 GO HERE ==============
        // ====================================================================

        /*
         * invertImage: invert a single channel (grayscale) image
         * input:   inputImage          single-channel (byte) image
         * output:                      single-channel (byte) image
         */
        private byte[,] invertImage(byte[,] inputImage)
        {
            // create temporary grayscale image
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];

            for (int y = 0; (y <= (tempImage.GetLength(1) - 1)); y++)
            {
                for (int x = 0; (x <= (tempImage.GetLength(0) - 1)); x++)
                {
                    tempImage[x, y] = (byte)(255 - inputImage[x, y]);
                }
            }

            return tempImage;
        }


        /*
         * adjustContrast: create an image with the full range of intensity values used
         * input:   inputImage          single-channel (byte) image
         * output:                      single-channel (byte) image
         */
        private byte[,] adjustContrast(byte[,] inputImage)
        {
            // create temporary grayscale image
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];
            int[] histrogramValues = new int[256];
            int nPixels = inputImage.GetLength(0) * inputImage.GetLength(1);
            //0.1 = 10% 
            double percentageIgnoredValues = 0.1;
            int amountIgnoredPixels = (int)(nPixels * percentageIgnoredValues);
            //Count all the histrogram values
            for (int y = 0; (y < (inputImage.GetLength(1))); y++)
            {
                for (int x = 0; (x < (inputImage.GetLength(0))); x++)
                {
                    histrogramValues[inputImage[x, y]]++;
                }
            }
            //ignore the right amount of pixels and find the higest
            int ignoredPixels = 0;
            int i = 255;
            while (ignoredPixels < amountIgnoredPixels)
            {
                ignoredPixels += histrogramValues[i];
                i--;
            }
            byte aHigh = (byte)(i+1);
            //ignore the right amount of pixels and find the lowest
            ignoredPixels = 0;
            i = 0;
            while (ignoredPixels < amountIgnoredPixels)
            {
                ignoredPixels += histrogramValues[i];
                i++;
            }
            byte aLow = (byte)(i - 1);
            //calculate new values
            for (int y = 0; (y < (inputImage.GetLength(1))); y++)
            {
                for (int x = 0; (x < (inputImage.GetLength(0))); x++)
                {
                    if(inputImage[x, y] > aHigh)
                        tempImage[x, y] = 255;
                    else if (inputImage[x, y] < aLow)
                        tempImage[x, y] = 0;
                    else
                        tempImage[x, y] = (byte)((inputImage[x, y] - aLow) * (255 / (aHigh - aLow)));
                }
            }

            return tempImage;
        }

        //private const int filterRoundingDelta = 1000;

        /*
         * createGaussianFilter: create a Gaussian filter of specific square size and with a specified sigma
         * input:   size                length and width of the Gaussian filter (only odd sizes)
         *          sigma               standard deviation of the Gaussian distribution // larger -> more spread out
         * output:                      Gaussian filter
         */
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
        private int GetRefImageX(int x, int kx, int filterSizeDelta, int inputImageXLength)
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
        private int GetRefImageY(int y, int ky, int filterSizeDelta, int inputImageYLength)
        {
            int n = y + (ky - filterSizeDelta); // get the actual input image pixel
            if (n < 0) return Math.Abs(n); // off the top of the image
            if (n >= inputImageYLength) return inputImageYLength - ky; // off the bottom of the image
            return n; // in the image
        }

        /*
         * convolveImage: apply linear filtering of an input image
         * input:   inputImage          single-channel (byte) image
         *          filter              linear kernel
         * output:                      single-channel (byte) image
         */
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
                for(int kx = 0; kx < filterSize; kx++)
                for (int ky = 0; ky < filterSize; ky++)
                {
                    newPixel += (byte) (filter[kx, ky] * 
                                        inputImage[GetRefImageX(x, kx, filterSizeDelta, inputImage.GetLength(0))
                                            , GetRefImageY(y, ky, filterSizeDelta, inputImage.GetLength(1))]);
                }
                return newPixel;
            }
        }

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
                tempImage[inputX, inputY] = ApplyKernel(inputX, inputY); // thread-safe because no thread writes to the same place in the target array
            });

            if (!loopResult.IsCompleted)
                throw new Exception($"consolveImageParallel did not complete it's loop to completion, stopped at iteration {loopResult.LowestBreakIteration}");
            
            return tempImage;

            // apply the kernel to the given pixel
            byte ApplyKernel(int x, int y)
            {
                // create a new pixel
                byte newPixel = 0;
                
                // loop over the filter kernel, adding the values to newPixel during execution
                for(int kx = 0; kx < filterSize; kx++)
                for (int ky = 0; ky < filterSize; ky++)
                {
                    newPixel += (byte) (filter[kx, ky] * 
                                        inputImage[GetRefImageX(x, kx, filterSizeDelta, inputImage.GetLength(0))
                                            , GetRefImageY(y, ky, filterSizeDelta, inputImage.GetLength(1))]);
                }
                return newPixel;
            }
        }

        /*
         * medianFilter: apply median filtering on an input image with a kernel of specified size
         * input:   inputImage          single-channel (byte) image
         *          size                length/width of the median filter kernel
         * output:                      single-channel (byte) image
         */
        private byte[,] medianFilter(byte[,] inputImage, byte size)
        {
            // create temporary grayscale image
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];
            int boundryPixels = size / 2;

            for (int y = boundryPixels; y < (inputImage.GetLength(1) - boundryPixels); y++)
            {
                for (int x = boundryPixels; x < (inputImage.GetLength(0) - boundryPixels); x++)
                {
                    //Add all kernel Values to a list
                    List<byte> kernelValues = new List<byte>();
                    for (int yK = 0; yK < size; yK++)
                    {
                        for (int xK = 0; xK < size; xK++)
                        {

                            kernelValues.Add(inputImage[GetRefImageX(x, xK, boundryPixels, inputImage.GetLength(0)),
                                                        GetRefImageY(y, yK, boundryPixels, inputImage.GetLength(1))]);
                        }
                    }
                    //sort list
                    kernelValues.Sort();

                    int medianIndex = (int)Math.Ceiling(((double)(size * size) / 2));

                    tempImage[x, y] = kernelValues[medianIndex];

                }
            }

            return tempImage;
        }

        private byte[,] medianFilterParallel(byte[,] inputImage, byte size)
        {
            // create temporary grayscale image
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];


            // calculate the size delta
            int filterSizeDelta = size / 2;
            // get the x size of the input image
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
                        
                        kernelValues.Add(inputImage[GetRefImageX(inputX, xK, filterSizeDelta, inputImage.GetLength(0)), 
                                                    GetRefImageY(inputY, yK, filterSizeDelta, inputImage.GetLength(1))]);
                    }
                }
                //sort list
                kernelValues.Sort();

                int medianIndex = (int)Math.Ceiling(((double)(size * size) / 2));

                tempImage[inputX, inputY] = kernelValues[medianIndex];
            });

            if (!loopResult.IsCompleted)
                throw new Exception($"consolveImageParallel did not complete it's loop to completion, stopped at iteration {loopResult.LowestBreakIteration}");

            

            return tempImage;
        }


        /*
         * edgeMagnitude: calculate the image derivative of an input image and a provided edge kernel
         * input:   inputImage          single-channel (byte) image
         *          horizontalKernel    horizontal edge kernel
         *          virticalKernel      vertical edge kernel
         * output:                      single-channel (byte) image
         */
        private byte[,] edgeMagnitude(byte[,] inputImage, sbyte[,] horizontalKernel, sbyte[,] verticalKernel)
        {
            // create temporary grayscale image
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];

            // TODO: add your functionality and checks, think about border handling and type conversion (negative values!)

            return tempImage;
        }


        /*
         * thresholdImage: threshold a grayscale image
         * input:   inputImage          single-channel (byte) image
         * output:                      single-channel (byte) image with on/off values
         */
        private byte[,] thresholdImage(byte[,] inputImage, byte thresholdValue)
        {
            // create temporary grayscale image
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];

            // TODO: add your functionality and checks, think about how to represent the binary values
            for (int y = 0; (y < tempImage.GetLength(1)); y++)
            {
                for (int x = 0; (x < tempImage.GetLength(0)); x++)
                {
                    if (inputImage[x, y] > thresholdValue)
                        tempImage[x, y] = 255;
                    else
                        tempImage[x, y] = 0;
                }
            }
            return tempImage;
        }

        private byte[,] histrogramEqualization(byte[,] inputImage)
        {
            // create temporary grayscale image
            byte[,] tempImage = new byte[inputImage.GetLength(0), inputImage.GetLength(1)];
            int[] histrogramValues = new int[256];
            int totalPixels = inputImage.GetLength(0) * inputImage.GetLength(1);

            //Count all the histrogram values
            for (int y = 0; (y < (inputImage.GetLength(1))); y++)
            {
                for (int x = 0; (x < (inputImage.GetLength(0))); x++)
                {
                    histrogramValues[inputImage[x, y]]++;
                }
            }

            //Calculate probability 
            double[] probability = new double[256];
            for (int i = 0; i <= 255; i++)
            {
                probability[i] = histrogramValues[i] / (double)totalPixels;
            }

            //Calculate cumulative propability
            double[] cumulativeProbability = new double[256];
            cumulativeProbability[0] = probability[0];
            for (int i = 1; i <= 255; i++)
            {
                cumulativeProbability[i] = cumulativeProbability[i-1] + probability[i];
            }

            //Multiply by 255
            byte[] newValues = new byte[256];
            for (int i = 1; i <= 255; i++)
            {
                newValues[i] = (byte)(cumulativeProbability[i] * 255);
            }

            //add new values to new image
            for (int y = 0; (y < tempImage.GetLength(1)); y++)
            {
                for (int x = 0; (x < tempImage.GetLength(0)); x++)
                {
                    tempImage[x, y] = newValues[inputImage[x, y]];
                }
            }

            return tempImage;
        }
        
        // ====================================================================
        // ============= YOUR FUNCTIONS FOR ASSIGNMENT 2 GO HERE ==============
        // ====================================================================


        // ====================================================================
        // ============= YOUR FUNCTIONS FOR ASSIGNMENT 3 GO HERE ==============
        // ====================================================================

    }
}