using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace DeadDrop.Services;

public static class AesEncryption
{
    /// <summary>
    /// Encrypt a string.
    /// </summary>
    /// <param name="plainText">String to encrypt</param>
    /// <param name="key">Encryption key used to encrypt the data</param>
    /// <returns>Returns encrypted data and a cryptographically random encryption key that was generated to encrypt the stream</returns>
    public static async Task<(byte[] EncryptedData, byte[] EncryptionKey)> Encrypt(string plainText)
    {
        var key = RandomNumberGenerator.GetBytes(32);
        var bytes = await Encrypt(Encoding.UTF8.GetBytes(plainText), key);
        return (bytes, key);
    }

    /// <summary>
    /// Encrypt a string.
    /// </summary>
    /// <param name="plainText">String to encrypt</param>
    /// <param name="key">Encryption key used to encrypt the data</param>
    /// <returns>Encrypted data</returns>
    public static Task<byte[]> Encrypt(string plainText, byte[] key) =>
        Encrypt(Encoding.UTF8.GetBytes(plainText), key);

    /// <summary>
    /// Encrypt a byte array.
    /// </summary>
    /// <param name="input">Byte array to be encrypted</param>
    /// <param name="key">Encryption key used to encrypt the data</param>
    /// <returns>Encrypted data</returns>
    public static async Task<byte[]> Encrypt(byte[] input, byte[] key)
    {
        using var inputStream = new MemoryStream(input);
        using var cipherStream = new MemoryStream();
        await Encrypt(inputStream, cipherStream, key);
        return cipherStream.ToArray();
    }

    /// <summary>
    /// Encrypt a stream of data while writing the encrypted data to another stream.
    /// </summary>
    /// <param name="inputStream">Stream to be encrypted</param>
    /// <param name="cipherStream">Encrypted data stream to be written</param>
    /// <param name="key">Encryption key used to encrypt the data</param>
    /// <returns></returns>
    public static async Task Encrypt(Stream inputStream, Stream cipherStream, byte[] key)
    {
        Trace.WriteLineIf(inputStream.Position > 0, "The input stream position is not set to 0.");
 
        using var aes = CreateAesAlgorithm(key);
        using var encryptor = aes.CreateEncryptor();
        using CryptoStream cryptoStream = new(cipherStream, encryptor, CryptoStreamMode.Write, true);
        await cipherStream.WriteAsync(aes.IV);
        await inputStream.CopyToAsync(cryptoStream);
        await cryptoStream.FlushFinalBlockAsync();
    }

    /// <summary>
    /// Encrypt a stream of data while writing the encrypted data to another stream.
    /// </summary>
    /// <param name="inputStream">Stream to be encrypted</param>
    /// <param name="cipherStream">Encrypted data stream to be written</param>
    /// <returns>Returns a cryptographically random encryption key that was generated to encrypt the stream</returns>
    public static async Task<byte[]> Encrypt(Stream inputStream, Stream cipherStream)
    {
        var key = RandomNumberGenerator.GetBytes(32);
        await Encrypt(inputStream, cipherStream, key);
        return key;
    }

    private static Aes CreateAesAlgorithm(byte[] key)
    {
        var aes = Aes.Create();
        aes.IV = RandomNumberGenerator.GetBytes(16);
        aes.Key = key;
        return aes;
    }

    /// <summary>
    /// Decrypts data that was encrypted with this service.
    /// </summary>
    /// <param name="cipherStream">Stream of encrypted data.</param>
    /// <param name="outputStream">Stream to write decrypted data to</param>
    /// <param name="key">Encryption Key</param>
    /// <param name="iv">Initialization Vector (IV)</param>
    /// <returns></returns>
    public static async Task Decrypt(Stream cipherStream, Stream outputStream, byte[] key)
    {
        using var aes = CreateAesAlgorithm(key);
        var iv = new byte[aes.IV.Length];
        cipherStream.ReadExactly(iv, 0, aes.IV.Length);
        aes.IV = iv;
        using CryptoStream cryptoStream = new(outputStream, aes.CreateDecryptor(), CryptoStreamMode.Write, true);
        await cipherStream.CopyToAsync(cryptoStream);
        await cryptoStream.FlushFinalBlockAsync();
    }

    /// <summary>
    /// Decrypts data that was encrypted with this service.
    /// </summary>
    /// <param name="input">Byte array of encrypted data.</param>
    /// <param name="key">Encryption Key</param>
    /// <param name="iv">Initialization Vector (IV)</param>
    /// <returns></returns>
    public static async Task<byte[]> Decrypt(byte[] input, byte[] key)
    {
        using var cipherStream = new MemoryStream(input);
        using var outputStream = new MemoryStream();
        await Decrypt(cipherStream, outputStream, key);
        return outputStream.ToArray();
    }
}
