using System;
using Common.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.Tests
{
    [TestClass]
    public class CryptographyTests
    {
        [TestMethod]
        public void Aes256EncoderDecoderTest()
        {
            ITextEncoder textEncoder = new Aes256TextEncoder();
            string testText = "This text should be the same\r\n";
            string password = "PASSWORD WITH SPACES";

            string cypherText = textEncoder.EncryptText(testText, password);
            Assert.IsFalse(string.IsNullOrWhiteSpace(cypherText));

            string clearText = textEncoder.DecryptText(cypherText, password);
            Assert.IsFalse(string.IsNullOrWhiteSpace(clearText));

            Assert.AreEqual(testText, clearText);
            Assert.AreNotEqual(cypherText, clearText);
        }
    }
}
