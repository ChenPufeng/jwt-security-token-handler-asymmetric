using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;

namespace jwt_security_token_handler_asymmetric.Helpers
{
    public static class RsaProviderHelper
    {
        public static RSACryptoServiceProvider CreateRsaProviderFromPublicKey(string publicKey)
        {
            var x509Key = Convert.FromBase64String(publicKey);

            using (var memoryStream = new MemoryStream(x509Key))
            using (var binReader = new BinaryReader(memoryStream))
            {
                ReadBytesRsa(binReader, out var exponent, out var modulus);

                var rsa = new RSACryptoServiceProvider();
                rsa.ImportParameters(new RSAParameters { Modulus = modulus, Exponent = exponent });
                return rsa;
            }
        }

        private static void ReadBytesRsa(BinaryReader binary, out byte[] exponent, out byte[] modulus)
        {
            ValidIod(binary);
            ValidSmallByte(binary);
            ValidByte(binary);

            modulus = ExtractedModulus(binary);
            exponent = ExtractedExponent(binary);
        }

        /// <summary>
        /// Encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <exception cref="ArgumentException"></exception>
        private static void ValidIod(BinaryReader binaryReader)
        {
            byte[] seqOid = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };

            ValidByte(binaryReader);

            var seq = binaryReader.ReadBytes(15);
            if (!CompareByteArrays(seq, seqOid))
                throw new ArgumentException("Sequence OID incorret.");
        }
        
        private static bool CompareByteArrays(IReadOnlyCollection<byte> a, IReadOnlyList<byte> b)
        {
            if (a.Count != b.Count)
                return false;
            var i = 0;
            foreach (var c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }
            return true;
        }

        /// <summary>
        /// data read as little endian order (actual data order for Sequence is 30 81)
        /// </summary>
        /// <param name="bynary"></param>
        /// <exception cref="ArgumentException"></exception>
        private static void ValidByte(BinaryReader bynary)
        {
            var bytes = bynary.ReadUInt16();
            if (bytes == 0x8130) //advance 1 byte
                bynary.ReadByte();
            else if (bytes == 0x8230) //advance 2 byte
                bynary.ReadInt16();
            else
                throw new ArgumentException("Bytes invalid.");
        }

        private static void ValidSmallByte(BinaryReader bynary)
        {
            var bytes = bynary.ReadUInt16();
            if (bytes == 0x8103)
                bynary.ReadByte();
            else if (bytes == 0x8203)
                bynary.ReadInt16();
            else
                throw new ArgumentException("Bytes invalid.");

            var bt = bynary.ReadByte();
            if (bt != 0x00)
                throw new ArgumentException("Bytes invalid.");
        }

        /// <summary>
        /// Validatated and returns low e high from byte of <see cref="BinaryReader"/>
        /// </summary>
        /// <param name="binary"></param>
        /// <param name="lowByte"></param>
        /// <param name="highByte"></param>
        private static void ReadBytesRanges(BinaryReader binary, out byte lowByte, out byte highByte)
        {
            lowByte = 0x00;
            highByte = 0x00;
            var bytes = binary.ReadUInt16();
            if (bytes == 0x8102)
                lowByte = binary.ReadByte();
            else if (bytes == 0x8202)
            {
                highByte = binary.ReadByte();
                lowByte = binary.ReadByte();
            }
            else
                throw new ArgumentException("Bytes invalid.");
        }

        private static byte[] ExtractedModulus(BinaryReader binary)
        {
            ReadBytesRanges(binary, out var lowByte, out var highByte);
            byte[] modInt = {lowByte, highByte, 0x00, 0x00};
            var modSize = BitConverter.ToInt32(modInt, 0);
            var firstByte = binary.PeekChar();
            
            if (firstByte != 0x00) 
                return binary.ReadBytes(modSize);
            
            binary.ReadByte();
            modSize -= 1;

            return binary.ReadBytes(modSize); 
        }

        private static byte[] ExtractedExponent(BinaryReader binary)
        {
            if (binary.ReadByte() != 0x02)
                throw new ArgumentException("Bytes invalid.");
            var expBytes = (int) binary.ReadByte();
            
            return binary.ReadBytes(expBytes);
        }
    }
}