using CGM.Communication.Extensions;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CGM.Communication.Extensions
{
    public static class ByteExtension
    {

        public static byte OneByteSum(this byte[] bytes)
        {
            byte sum = 0;
            foreach (byte b in bytes)
            {
                sum += (byte)((short)b);
            }
            return sum;
        }


        public static byte[] Sha256Digest(this byte[] bytes)
        {

            Org.BouncyCastle.Crypto.Digests.Sha256Digest myHash = new Org.BouncyCastle.Crypto.Digests.Sha256Digest();
            myHash.BlockUpdate(bytes, 0, bytes.Length);
            byte[] compArr = new byte[myHash.GetDigestSize()];
            myHash.DoFinal(compArr, 0);
            return compArr;
        }

        public static byte[] Sha1Digest(this byte[] bytes)
        {
            Org.BouncyCastle.Crypto.Digests.Sha1Digest myHash = new Org.BouncyCastle.Crypto.Digests.Sha1Digest();
            myHash.BlockUpdate(bytes, 0, bytes.Length);
            byte[] compArr = new byte[myHash.GetDigestSize()];
            myHash.DoFinal(compArr, 0);
            return compArr;

        }

        //public static byte[] Decrypt2(this byte[] cmessage,string password, byte[] salt)
        //{
        //    int KEY_SIZE = 256;
        //    int iterations = 100;
        //    //byte[] salt = new byte[KEY_SIZE >> 3];

        //    IBufferedCipher decCipher = BuildDecryptionCipher(password, iterations, salt);
        //    return DecryptTemp(decCipher, cmessage);
        //}

        private static IBufferedCipher BuildDecryptionCipher(string password, int iterations, byte[] salt)
        {
            string DECRYPTION_ALGORITHM = "PBEWithSHA256And256BitAES-CBC-BC";
            // get the password bytes
            char[] passwordChars = password.ToCharArray();

            IBufferedCipher cipher = CipherUtilities.GetCipher(DECRYPTION_ALGORITHM);

            Org.BouncyCastle.Asn1.Asn1Encodable algParams = PbeUtilities.GenerateAlgorithmParameters(DECRYPTION_ALGORITHM, salt, iterations);
            ICipherParameters cipherParams = PbeUtilities.GenerateCipherParameters(DECRYPTION_ALGORITHM, passwordChars, algParams);
            cipher.Init(false, cipherParams);

            return cipher;
        }

        //private static byte[] DecryptTemp(IBufferedCipher cipher, byte[] encrypted)
        //{
        //    byte[] plainText;

        //    using (MemoryStream writeStr = new MemoryStream())
        //    {
        //        using (MemoryStream readStr = new MemoryStream(encrypted))
        //        using (CipherStream cstr = new CipherStream(readStr, cipher, null))
        //        {
        //            byte[] data = new byte[256];
        //            int got;

        //            while ((got = cstr.Read(data, 0, data.Length)) > 0)
        //            {
        //                writeStr.Write(data, 0, got);
        //            }
        //            writeStr.Flush();
        //        }

        //        plainText = writeStr.ToArray();
        //    }

        //    return plainText; //Encoding.UTF8.GetString(plainText);
        //}

        //private static IBufferedCipher BuildEncryptionCipher(string password, int iterations,byte[] salt)
        //{
        //    // get the password bytes
        //    char[] passwordChars = password.ToCharArray();
        //    byte[] passwordBytes = PbeParametersGenerator.Pkcs12PasswordToBytes(passwordChars);


        //    string ENCRYPTION_ALGORITHM = "AES";
        //    int KEY_SIZE = 256;
            

        //    // select the digest algorithm.
        //    // if you change the digest algorithm, you must change DECRYPTION_ALGORITHM
        //    IDigest digest = new Org.BouncyCastle.Crypto.Digests.Sha256Digest();


        //    PbeParametersGenerator pbeParamGen = new Org.BouncyCastle.Crypto.Generators.Pkcs12ParametersGenerator(digest);
        //    pbeParamGen.Init(passwordBytes, salt, iterations);

        //    // 128-bit initialization vector
        //    ParametersWithIV parameters = (ParametersWithIV)pbeParamGen.GenerateDerivedParameters(ENCRYPTION_ALGORITHM, KEY_SIZE, 128);

        //    KeyParameter encKey = (KeyParameter)parameters.Parameters;

        //    // we’ll use CBC and PKCS7Padding
        //    IBufferedCipher cipher = CipherUtilities.GetCipher(ENCRYPTION_ALGORITHM + "/CBC/PKCS7Padding");

        //    cipher.Init(true, parameters);

        //    return cipher;
        //}


        public static byte[] Decrypt(this byte[] cmessage, byte[] m_Key, byte[] iVector)
        {
            return cmessage.CryptMessage(false, m_Key, iVector);
        }

        public static byte[] Encrypt(this byte[] cmessage, byte[] m_Key, byte[] iVector)
        {
            return cmessage.CryptMessage(true, m_Key, iVector);
        }

        public static byte[] CryptMessage(this byte[] cmessage, bool forEncryption, byte[] m_Key, byte[] iVector)
        {

            var m_IV = new byte[16];
            Array.Copy(iVector, 0, m_IV, 0, 16);

            KeyParameter aesKeyParam = ParameterUtilities.CreateKeyParameter("AES", m_Key);
            ParametersWithIV aesIVKeyParam = new ParametersWithIV(aesKeyParam, m_IV);

            IBufferedCipher cipher = CipherUtilities.GetCipher("AES/CFB/NoPadding");
            cipher.Init(forEncryption, aesIVKeyParam);
            return cipher.DoFinal(cmessage);
        }

        public static DateTime? GetDateTimeBigE(this byte[] bytes, int startIndex)
        {
            if ((bytes.Length - startIndex) >= 8)
            {
                var rtc = bytes.GetInt32BigE(startIndex);
                var offset = bytes.GetInt32BigE(startIndex + 4);
                return DateTimeExtension.GetDateTime(rtc, offset);
            }
            return null;

        }

        public static DateTime? GetDateTime(this byte[] bytes, int startIndex)
        {
            if ((bytes.Length - startIndex) >= 8)
            {
                var rtc = bytes.GetInt32(startIndex);
                var offset = bytes.GetInt32(startIndex + 4);
                return DateTimeExtension.GetDateTime(rtc, offset);
            }
            return null;

        }
        public static DateTime? GetDateTime(this byte[] bytes)
        {
            return bytes.GetDateTime(0);

        }

        public static ushort GetCrc16citt(this byte[] bytes)
        {
            const ushort poly = 4129;
            ushort[] table = new ushort[256];
            ushort initialValue = 0xffff;
            ushort temp, a;
            ushort crc = initialValue;
            for (int i = 0; i < table.Length; ++i)
            {
                temp = 0;
                a = (ushort)(i << 8);
                for (int j = 0; j < 8; ++j)
                {
                    if (((temp ^ a) & 0x8000) != 0)
                        temp = (ushort)((temp << 1) ^ poly);
                    else
                        temp <<= 1;
                    a <<= 1;
                }
                table[i] = temp;
            }
            for (int i = 0; i < bytes.Length; ++i)
            {
                crc = (ushort)((crc << 8) ^ table[((crc >> 8) ^ (0xff & bytes[i]))]);
            }
            return crc;

        }

    }
}
