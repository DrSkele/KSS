using System;
using System.Linq;
using System.Security.Cryptography;

public static class Encryption
{
    /*
    MessageDigest encrypt = MessageDigest.getInstance("SHA-256");
    encrypt.reset();
    encrypt.update(userID.getBytes());
    return new String(Base64.encodeBase64(encrypt.digest(pwd.getBytes())));
    */
    //Equivalent to java code above.
    public static string Encrypt(string userID, string pwd)
    {
        var sourceByte = System.Text.UTF8Encoding.UTF8.GetBytes(pwd);
        var saltByte = System.Text.UTF8Encoding.UTF8.GetBytes(userID);
        var salted = SaltHash(sourceByte, saltByte);

        string key = Convert.ToBase64String(salted, 0, salted.Length);
        return key;

        byte[] SaltHash(byte[] source, byte[] salt)
        {
            var sha256 = new SHA256CryptoServiceProvider();
            byte[] combined = salt.Concat(source).ToArray();
            byte[] hashed = sha256.ComputeHash(combined);
            return hashed;
        }
    }
}