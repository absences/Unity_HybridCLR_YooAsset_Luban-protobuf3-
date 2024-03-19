using System.IO;

namespace Telepathy
{
    public static class Utils
    {
        // IntToBytes version that doesn't allocate a new byte[4] each time.
        // -> important for MMO scale networking performance.
        public static void IntToBytesBigEndianNonAlloc(int value, byte[] bytes, int offset = 0)
        {
            bytes[offset + 0] = (byte)(value >> 24);
            bytes[offset + 1] = (byte)(value >> 16);
            bytes[offset + 2] = (byte)(value >> 8);
            bytes[offset + 3] = (byte)value;
        }
        public static void WriteIntBigEndian(int value, Stream stream)
        {
            stream.WriteByte((byte)(value >> 24));
            stream.WriteByte((byte)(value >> 16));
            stream.WriteByte((byte)(value >> 8));
            stream.WriteByte((byte)value);
        }
        public static int BytesToIntBigEndian(byte[] bytes, int offset = 0)
        {
            return (bytes[0 + offset] << 24) |
                   (bytes[1 + offset] << 16) |
                   (bytes[2 + offset] << 8) |
                    bytes[3 + offset];
        }
    }
}