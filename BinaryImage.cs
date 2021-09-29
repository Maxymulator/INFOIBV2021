using System;
using System.Drawing.Drawing2D;
using System.Linq;

namespace INFOIBV
{
    public class BinaryImage
    {
        // The stored binary image
        private byte[,] _image;
        
        // The x size of the binary image
        private readonly int _xSize;

        /// <summary>
        /// The x size of the binary image
        /// </summary>
        public int XSize => _xSize;

        // The y size of the binary image
        private readonly int _ySize;

        /// <summary>
        /// The y size of the binary image
        /// </summary>
        public int YSize => _ySize;

        /// <summary>
        /// Create a new empty binary image
        /// </summary>
        /// <param name="xSize">The x size of the binary image</param>
        /// <param name="ySize">The y size of the binary image</param>
        public BinaryImage(int xSize, int ySize)
        {
            _image = new byte[xSize, ySize];
            _xSize = xSize;
            _ySize = ySize;
        }

        /// <summary>
        /// Create a new filled binary image
        /// </summary>
        /// <param name="input">The input image, must be binary</param>
        public BinaryImage(byte[,] input)
        {
            if (!IsBinary(input))
                throw new ArgumentException("BinaryImage constructor was given an input image which is not binary");

            _xSize = input.GetLength(0);
            _ySize = input.GetLength(1);
            _image = input;
        }

        /// <summary>
        /// Get the byte[,] of the binary image
        /// </summary>
        /// <returns>A byte[,] with a binary image</returns>
        public byte[,] GetImage()
        {
            return _image;
        }

        /// <summary>
        /// Fill the given pixel with the given value in the binary image
        /// </summary>
        /// <param name="x">The x coord of the pixel</param>
        /// <param name="y">The y coord of the pixel</param>
        /// <param name="val">The value of the pixel, must be 0 or 255</param>
        public void Fill(int x, int y, byte val)
        {
            if (x >= XSize)
                throw new ArgumentException("BinaryImage.Fill was given a x out of range");
            
            if (y >= YSize)
                throw new ArgumentException("BinaryImage.Fill was given a y out of range");
            
            if (!(val == 0 || val == 255))
                throw new ArgumentException("BinaryImage.Fill was given a value which was not pure black or white");

            _image[x, y] = val;
        }

        /// <summary>
        /// Replace the stored binary image with the given image
        /// </summary>
        /// <param name="input">A byte[,] image, must be binary and match the x and y size of the BinaryImage</param>
        public void Fill(byte[,] input)
        {
            if (input.GetLength(0) != XSize)
                throw new ArgumentException("BinaryImage.Fill was given an image with mismatching x size");
            
            if (input.GetLength(1) != YSize)
                throw new ArgumentException("BinaryImage.Fill was given an image with mismatching y size");
            
            if (!IsBinary(input))
                throw new ArgumentException("BinaryImage.Fill was given an input image which is not binary");

            _image = input;
        }

        /// <summary>
        /// Checks whether a given byte[,] image is binary
        /// </summary>
        /// <returns>A bool denoting if the given image is binary</returns>
        private bool IsBinary(byte[,] input)
        {
            return input.Cast<byte>().All(val => val == 0 || val == 255);
        }
    }
}