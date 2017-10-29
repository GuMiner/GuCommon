namespace Common.Cryptography
{
    /// <summary>
    /// Defines an interface for text encryption
    /// </summary>
    public interface ITextEncoder
    {
        /// <summary>
        /// Encrypts the following string using the specified password and default salt.
        /// </summary>
        string EncryptText(string input, string password);

        /// <summary>
        /// Encrypts the following string using the specified password and salt.
        /// </summary>
        string EncryptText(string input, string password, string salt);

        /// <summary>
        /// Decrypts the following string using the specified password and default salt.
        /// </summary>
        string DecryptText(string input, string password);

        /// <summary>
        /// Decrypts the following string using the specified password and salt.
        /// </summary>
        string DecryptText(string input, string password, string salt);
    }
}
