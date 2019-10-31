namespace Cds.DroidManagement.Domain.DroidAggregate.Abstractions
{
    public interface IEncryptionService
    {
        string Encrypt(string clearText);

        string Decrypt(string cipherText);
    }
}
