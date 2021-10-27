namespace INFOIBV
{
    public static class Utils
    {
        /// <summary>
        /// Remap the value to the new range
        /// </summary>
        /// <param name="from1"> The low side of the current range</param>
        /// <param name="to1"> The high side of the current range</param>
        /// <param name="from2"> The low side of the new range</param>
        /// <param name="to2"> The high side of the new range</param>
        /// <returns></returns>
        public static float Remap (this float value, float from1, float to1, float from2, float to2) {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
        
        /// <summary>
        /// Remap the value to the new range
        /// </summary>
        /// <param name="from1"> The low side of the current range</param>
        /// <param name="to1"> The high side of the current range</param>
        /// <param name="from2"> The low side of the new range</param>
        /// <param name="to2"> The high side of the new range</param>
        /// <returns></returns>
        public static int Remap (this int value, int from1, int to1, int from2, int to2) {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
        
        /// <summary>
        /// Remap the value to the new range
        /// </summary>
        /// <param name="from1"> The low side of the current range</param>
        /// <param name="to1"> The high side of the current range</param>
        /// <param name="from2"> The low side of the new range</param>
        /// <param name="to2"> The high side of the new range</param>
        /// <returns></returns>
        public static byte Remap (this byte value, byte from1, byte to1, byte from2, byte to2) {
            return (byte) ((value - from1) / (to1 - from1) * (to2 - from2) + from2);
        }
        
        /// <summary>
        /// Remap the value to the new range
        /// </summary>
        /// <param name="from1"> The low side of the current range</param>
        /// <param name="to1"> The high side of the current range</param>
        /// <param name="from2"> The low side of the new range</param>
        /// <param name="to2"> The high side of the new range</param>
        /// <returns></returns>
        public static double Remap (this double value, double from1, double to1, double from2, double to2) {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
    }
}