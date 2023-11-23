using System.Security.Cryptography;
using System.Text;
using DeadDrop.Services;

namespace DeadDrop.UnitTests;

public class AesEncryptionTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test, Description("Ensure strings can be encrypted and decrypted correctly.")]
    public async Task EncryptAndDecryptStrings()
    {
        const string PlainText = """
            “It is possible to commit no mistakes and still 
            lose. That is not weakness, that is life.”
            """;

        var key = RandomNumberGenerator.GetBytes(32);
        var encryptedBytes = await AesEncryption.Encrypt(PlainText, key);
        var decryptedBytes = await AesEncryption.Decrypt(encryptedBytes, key);
        var decryptedText = Encoding.UTF8.GetString(decryptedBytes);

        Assert.That(decryptedText, Is.EqualTo(PlainText));
    }

    [Test, Description("Ensure strings can be encrypted and decrypted correctly with a generated key.")]
    public async Task EncryptAndDecryptStringsWithoutKey()
    {
        const string PlainText = """
            “Buried deep within you, beneath all the years
            of pain and anger, there is something that has
            never been nurtured: The potential to be a 
            better man.” 
            """;
        
        var (encryptedBytes, key) = await AesEncryption.Encrypt(PlainText);
        var decryptedBytes = await AesEncryption.Decrypt(encryptedBytes, key);
        var decryptedText = Encoding.UTF8.GetString(decryptedBytes);

        Assert.That(decryptedText, Is.EqualTo(PlainText));
    }


    [Test, Description("Ensure byte arrays can be encrypted and decrypted correctly.")]
    public async Task EncryptAndDecryptBytes()
    {
        var PlainText = """
            “Someone once told me that time was a predator that stalked 
            us all our lives. I rather believe that time is a companion 
            who goes with us on the journey and reminds us to cherish 
            every moment, because it will never come again.”
            """;

        var plainTextBytes = Encoding.UTF8.GetBytes(PlainText);
        var key = RandomNumberGenerator.GetBytes(32);
        var encryptedBytes = await AesEncryption.Encrypt(plainTextBytes, key);
        var decryptedBytes = await AesEncryption.Decrypt(encryptedBytes, key);

        Assert.That(decryptedBytes, Is.EqualTo(plainTextBytes));
    }

    [Test, Description("Ensure streams can be encrypted and decrypted correctly.")]
    public async Task EncryptAndDecryptStreams()
    {
        const string PlainText = """
            “We have powerful tools: Openness, optimism and the spirit of curiosity.”
            """;
        
        var plainTextBytes = Encoding.UTF8.GetBytes(PlainText);
        using var plainTextStream = new MemoryStream(plainTextBytes);
        using var cipherStream = new MemoryStream();

        var key = RandomNumberGenerator.GetBytes(32);
        await AesEncryption.Encrypt(plainTextStream, cipherStream, key);

        using var decryptedStream = new MemoryStream();
        cipherStream.Position = 0;
        await AesEncryption.Decrypt(cipherStream, decryptedStream, key);

        Assert.That(decryptedStream.ToArray(), Is.EqualTo(plainTextBytes));
    }

    [Test, Description("Ensure streams can be encrypted and decrypted correctly with a generated key.")]
    public async Task EncryptAndDecryptStreamsWithoutKey()
    {
        const string PlainText = """
            “Make it so.”
            """;

        var plainTextBytes = Encoding.UTF8.GetBytes(PlainText);
        using var plainTextStream = new MemoryStream(plainTextBytes);
        using var cipherStream = new MemoryStream();

        var key = await AesEncryption.Encrypt(plainTextStream, cipherStream);

        using var decryptedStream = new MemoryStream();
        cipherStream.Position = 0;
        await AesEncryption.Decrypt(cipherStream, decryptedStream, key);

        Assert.That(decryptedStream.ToArray(), Is.EqualTo(plainTextBytes));
    }
}