using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Common.Cryptography
{
    /// <summary>
    /// Performs AES 256 symmetric-key encryption.
    /// </summary>
    public class Aes256ByteEncoder : IByteEncoder
    {
        private static string defaultSalt = "CodeLibraryCryptography_abc123^%$";

        public byte[] DecryptBytes(byte[] bytes, string password)
        {
            return this.DecryptBytes(bytes, password, defaultSalt);
        }

        public byte[] EncryptBytes(byte[] bytes, string password)
        {
            return this.EncryptBytes(bytes, password, defaultSalt);
        }

        public byte[] DecryptBytes(byte[] bytes, string password, string salt)
        {
            // Using the default key size.
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, Encoding.ASCII.GetBytes(salt));

            RijndaelManaged aesAlgorithm = new RijndaelManaged();
            aesAlgorithm.Key = key.GetBytes(aesAlgorithm.KeySize / 8);

            using (MemoryStream inputStream = new MemoryStream(bytes))
            {
                // Read in the array size and the cryptographic data.
                byte[] ivLengthArray = new byte[sizeof(Int32)];
                inputStream.Read(ivLengthArray, 0, ivLengthArray.Length);

                int ivLength = BitConverter.ToInt32(ivLengthArray, 0);
                inputStream.Read(aesAlgorithm.IV, 0, ivLength);

                // Read the rest of the stream to decrypt the data.
                using (ICryptoTransform decryptor = aesAlgorithm.CreateDecryptor(aesAlgorithm.Key, aesAlgorithm.IV))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(inputStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (MemoryStream outputStream = new MemoryStream())
                        {
                            cryptoStream.CopyTo(outputStream);

                            byte[] dataWithIv = outputStream.ToArray();
                            byte[] cleanedData = new byte[dataWithIv.Length - aesAlgorithm.IV.Length];
                            Array.Copy(dataWithIv, aesAlgorithm.IV.Length, cleanedData, 0, cleanedData.Length);

                            return cleanedData;
                        }
                    }
                }
            }
        }

        public byte[] EncryptBytes(byte[] bytes, string password, string salt)
        {
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, Encoding.ASCII.GetBytes(salt));

            RijndaelManaged aesAlgorithm = new RijndaelManaged();
            aesAlgorithm.Key = key.GetBytes(aesAlgorithm.KeySize / 8);

            using (ICryptoTransform encryptor = aesAlgorithm.CreateEncryptor(aesAlgorithm.Key, aesAlgorithm.IV))
            {
                using (MemoryStream outputStream = new MemoryStream())
                {
                    // Writeout the IV size and the IV itself.
                    outputStream.Write(BitConverter.GetBytes(aesAlgorithm.IV.Length), 0, sizeof(Int32));
                    outputStream.Write(aesAlgorithm.IV, 0, aesAlgorithm.IV.Length);
                    using (CryptoStream cryptoStream = new CryptoStream(outputStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(new byte[aesAlgorithm.IV.Length], 0, aesAlgorithm.IV.Length);
                        cryptoStream.Write(bytes, 0, bytes.Length);
                    }
                    return outputStream.ToArray();
                }
            }
        }
    }
}
