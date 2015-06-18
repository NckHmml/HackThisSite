using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HTS
{
    public static class ReverseEncryption
    {
        private static byte[] PossibleChars { get; set; }
        private static MD5 Hash { get; set; }

        static ReverseEncryption()
        {
            PossibleChars = new char[] { 
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',
                'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
                'U', 'V', 'W', 'X', 'Y', 'Z'
            }
                .Select(x => (byte)x)
                .ToArray();

            Hash = MD5.Create();
        }

        public static string BruteForce(string input)
        {
            int[] encrypted = input
                .Split(' ')
                .Select(x => Int32.Parse(x))
                .ToArray();

            byte[] hash = new byte[32];
            byte[] decrypted = new byte[40];

            for (int i = 0; i < PossibleChars.Length; i++)
                for (byte p = 0; p < 0x10; p++)
                {
                    byte c = PossibleChars[i];
                    decrypted[0] = c;
                    hash[0] = p;
                    int curTotal = c - encrypted[0] + p;
                    if (BackTrack(encrypted, hash, decrypted, curTotal))
                        return hash
                            .Select(x => x.ToString("x"))
                            .Aggregate((x, y) => x + y);
                }

            return "";
        }

        public static string DecryptString(string input, string strPasswordMD5)
        {
            int[] encrypted = input
                .Split(' ')
                .Select(x => Int32.Parse(x))
                .ToArray();
            byte[] decrypted = new byte[encrypted.Length];
            int curTotal = strPasswordMD5.Sum(c => Convert.ToInt32(c.ToString(), 16));
            for (int i = 0; i < encrypted.Length; i++)
            {
                decrypted[i] = (byte)(encrypted[i] + curTotal - Convert.ToInt32(strPasswordMD5[i % 32].ToString(), 16));
                NewTotal(ref curTotal, decrypted.Take(i + 1));
            }

            return Encoding.ASCII.GetString(decrypted);
        }

        private static bool BackTrack(int[] encrypted, byte[] hash, byte[] decrypted, int curTotal, int position = 1)
        {
            if (position == 40)
                return true;

            NewTotal(ref curTotal, decrypted.Take(position));

            if (position < 32)
            {
                for (byte p = 0; p < 0x10; p++)
                {
                    int curChar = encrypted[position] + curTotal - p;
                    if (IsValidChar(curChar, position))
                    {
                        decrypted[position] = (byte)curChar;
                        hash[position] = p;
                        if (BackTrack(encrypted, hash, decrypted, curTotal, position + 1))
                            return true;
                    }
                }
                return false;
            }
            else
            {
                byte p = hash[position % 32];
                int curChar = encrypted[position] + curTotal - p;
                if (IsValidChar(curChar, position))
                {
                    decrypted[position] = (byte)curChar;
                    if (BackTrack(encrypted, hash, decrypted, curTotal, position + 1))
                        return true;
                }
                return false;
            }
        }

        private static void NewTotal(ref int curTotal, IEnumerable<byte> curDecrypt)
        {
            var left = Hash.ComputeHash(curDecrypt.ToArray())
                .SelectMany(x => x.ToString("x2"));
            var right = Hash.ComputeHash(Encoding.ASCII.GetBytes(curTotal.ToString()))
                .SelectMany(x => x.ToString("x2"));
            curTotal = left.Take(16)
                .Concat(right.Take(16))
                .Sum(c => Convert.ToInt32(c.ToString(), 16));
        }

        private static bool IsValidChar(int c, int position)
        {
            position++;
            if (c > 0xff || c < 0)
                return false;

            switch (position)
            {
                case 40:
                case 20:
                    return c == (byte)'\n';
                case 39:
                case 37:
                case 19:
                case 17:
                    return c == (byte)'1';
                case 38:
                case 18:
                    return c == (byte)'.';

                case 31:
                case 11:
                    return c == (byte)'M';
                case 30:
                case 10:
                    return c == (byte)'E';
                case 29:
                case 9:
                    return c == (byte)'O';
                default:
                    if (position % 4 == 0)
                        return c == (byte)'-';
                    return PossibleChars.Contains((byte)c);
            }
        }
    }
}
