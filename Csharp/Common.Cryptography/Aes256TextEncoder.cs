using System;
using System.Text;

namespace Common.Cryptography
{
    /// <summary>
    /// Performs AES 256 symmetric-key encryption.
    /// </summary>
    public class Aes256TextEncoder : ITextEncoder
    {
        private Aes256ByteEncoder backingEncoder;

        public Aes256TextEncoder()
        {
            this.backingEncoder = new Aes256ByteEncoder();
        }

        public string DecryptText(string input, string password)
        {
            return Encoding.UTF8.GetString(this.backingEncoder.DecryptBytes(Convert.FromBase64String(input), password));
        }

        public string DecryptText(string input, string password, string salt)
        {
            return Encoding.UTF8.GetString(this.backingEncoder.DecryptBytes(Convert.FromBase64String(input), password, salt));
        }

        public string EncryptText(string input, string password)
        {
            return Convert.ToBase64String(this.backingEncoder.EncryptBytes(Encoding.UTF8.GetBytes(input), password));
        }

        public string EncryptText(string input, string password, string salt)
        {
            return Convert.ToBase64String(this.backingEncoder.EncryptBytes(Encoding.UTF8.GetBytes(input), password, salt));
        }
    }
}
