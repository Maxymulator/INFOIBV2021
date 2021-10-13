using System;
using System.Drawing.Drawing2D;
using System.Linq;

namespace INFOIBV
{
    public class BinaryImage
    {
        // The stored binary image
        private byte[,] _image;
        
        /// <summary>
        /// The x size of the binary image
        /// </summary>
        public int XSize { get; }

        /// <summary>
        /// The y size of the binary image
        /// </summary>
        public int YSize { get; }

        /// <summary>
        /// Create a new empty binary image
        /// </summary>
        /// <param name="xSize">The x size of the binary image</param>
        /// <param name="ySize">The y size of the binary image</param>
        public BinaryImage(int xSize, int ySize)
        {
            _image = new byte[xSize, ySize];
            XSize = xSize;
            YSize = ySize;
        }

        /// <summary>
        /// Create a new filled binary image
        /// </summary>
        /// <param name="input">The input image, must be binary</param>
        public BinaryImage(byte[,] input)
        {
            if (!IsBinary(input))
                throw new ArgumentException("BinaryImage constructor was given an input image which is not binary");

            XSize = input.GetLength(0);
            YSize = input.GetLength(1);
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
        /// Get the byte value of the given pixel in the binary image
        /// </summary>
        /// <param name="x">The x coord of the pixel</param>
        /// <param name="y">The y coord of the pixel</param>
        /// <returns>The byte value of the pixel</returns>
        public byte GetPixelByte(int x, int y)
        {
            if (x >= XSize)
                throw new ArgumentException("BinaryImage.GetPixelByte was given a x out of range");
            
            if (y >= YSize)
                throw new ArgumentException("BinaryImage.GetPixelByte was given a y out of range");
            
            return _image[x, y];
        }
        
        /// <summary>
        /// Get the bool value of the given pixel in the binary image
        /// Where byte value 255 returns true
        /// And byte value 0 returns false
        /// </summary>
        /// <param name="x">The x coord of the pixel</param>
        /// <param name="y">The y coord of the pixel</param>
        /// <returns>The byte value of the pixel</returns>
        public bool GetPixelBool(int x, int y)
        {
            if (x >= XSize)
                throw new ArgumentException("BinaryImage.GetPixelBool was given a x out of range");
            
            if (y >= YSize)
                throw new ArgumentException("BinaryImage.GetPixelBool was given a y out of range");
            
            return _image[x, y] == 255;
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
        /// Fill the given pixel with the given boolean value in the binary image
        /// </summary>
        /// <param name="x">The x coord of the pixel</param>
        /// <param name="y">The y coord of the pixel</param>
        /// <param name="val">The value of the pixel</param>
        public void Fill(int x, int y, bool val)
        {
            if (x >= XSize)
                throw new ArgumentException("BinaryImage.Fill was given a x out of range");
            
            if (y >= YSize)
                throw new ArgumentException("BinaryImage.Fill was given a y out of range");

            _image[x, y] = (byte) (val ? 255 : 0);
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

        /// <summary>
        /// Get the pixel-wise AND of two binary images
        /// </summary>
        /// <param name="rhs">The binary image on the right hand side of the AND operator, must match the size of the binary image on the left hand side</param>
        /// <returns>A BinaryImage, which is the result of a pixel-wise AND</returns>
        public BinaryImage AND(BinaryImage rhs)
        {
            if (rhs.XSize != XSize)
                throw new ArgumentException("BinaryImage.And was given an image with mismatching x size");
            
            if (rhs.YSize != YSize)
                throw new ArgumentException("BinaryImage.And was given an image with mismatching y size");

            BinaryImage output = new BinaryImage(XSize, YSize);

            for (int y = 0; y < YSize; y++)
            for (int x = 0; x < XSize; x++)
            {
                output.Fill(x, y, GetPixelByte(x, y) == rhs.GetPixelByte(x, y) ? (byte) 255 : (byte) 0);
            }

            return output;
        }
        
        /// <summary>
        /// Get the pixel-wise OR of two binary images
        /// </summary>
        /// <param name="rhs">The binary image on the right hand side of the OR operator, must match the size of the binary image on the left hand side</param>
        /// <returns>A BinaryImage, which is the result of a pixel-wise OR</returns>
        public BinaryImage OR(BinaryImage rhs)
        {
            if (rhs.XSize != XSize)
                throw new ArgumentException("BinaryImage.And was given an image with mismatching x size");
            
            if (rhs.YSize != YSize)
                throw new ArgumentException("BinaryImage.And was given an image with mismatching y size");

            BinaryImage output = new BinaryImage(XSize, YSize);

            for (int y = 0; y < YSize; y++)
            for (int x = 0; x < XSize; x++)
            {
                output.Fill(x, y, GetPixelBool(x, y) || rhs.GetPixelBool(x, y) ? (byte) 255 : (byte) 0);
            }

            return output;
        }
    }
}