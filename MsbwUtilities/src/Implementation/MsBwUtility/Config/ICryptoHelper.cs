namespace MsBw.MsBwUtility.Config
{
    public interface ICryptoHelper
    {
        string Encrypt(byte[] redBytes);
        int? DecryptNullableInt(string base64);
        string Encrypt(int? value);
        string Decrypt(string base64);
        string Encrypt(string value);
        byte[] DecryptAsBytes(string blackBase64);
    }
}