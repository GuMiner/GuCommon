namespace Common.Cryptography
{
    /// <summary>
    /// Defines an interface for byte encryption
    /// </summary>
    public interface IByteEncoder
    {
        /// <summary>
        /// Encrypts the following bytes using the specified password and default salt.
        /// </summary>
        byte[] EncryptBytes(byte[] bytes, string password);

        /// <summary>
        /// Encrypts the following bytes using the specified password and salt.
        /// </summary>
        byte[] EncryptBytes(byte[] bytes, string password, string salt);

        /// <summary>
        /// Decrypts the following bytes using the specified password and default salt.
        /// </summary>
        byte[] DecryptBytes(byte[] bytes, string password);

        /// <summary>
        /// Decrypts the following bytes using the specified password and salt.
        /// </summary>
        byte[] DecryptBytes(byte[] bytes, string password, string salt);
    }
}
