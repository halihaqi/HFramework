namespace HFramework
{
    public static class DataUtils
    {
        //数据密钥
        private const byte KEY = 233;
        
        public static void EncryptData(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] ^= KEY;
        }

        public static void DecryptData(byte[] bytes, int index, int count)
        {
            for (int i = index; i < index + count; i++)
                bytes[i] ^= KEY;
        }
    }
}