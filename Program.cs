using System;
using System.Text;
using System.Threading;
namespace UnwelcomeCipher
{
    class Program
    {
        private static byte[] Generate(byte[] key)
        {
            byte[] outp = new byte[256];
            byte tmp = 0;
            uint a, b, c, d, e, f;
            for (int i = 0; i < 256; i++)
            {
                outp[i] = (byte)i;
            }
            for (uint i = 0; i < 256; i++)
            {
                a = ((uint)(tmp + key[i % key.Length]) + i) % 256;
                b = (a + 128) % 256;

                c = (uint)(((outp[b] << 1) | (outp[b] >> 7)) & 255);
                d = (uint)(((outp[a] >> 2) | (outp[a] << 6)) & 255);

                e = c ^ outp[a];
                f = (d * outp[b]) % 256;

                outp[i] = (byte)(outp[(a + b) % 256] ^ outp[(a + b + 128) % 256]);
                outp[a] = (byte)e;
                outp[b] = (byte)f;

                tmp = (byte)((a + (256 - i)) % 256);
            }
            return outp;
        }
        public static byte[] Encrypt(byte[] Data, byte[] Key)
        {
            byte[] RoundKey = Generate(Key);
            uint a, b, c;
            byte tmp;
            a = (uint)((RoundKey[0]) & 255);
            b = (uint)((RoundKey[^1] + 128) % 256);

            a = ((a << 1) | (a >> 7)) % 256;
            b = ((b >> 2) | (b << 6)) % 256;

            c = RoundKey[(a ^ b) % 256];

            Array.Copy(RoundKey, 0, RoundKey, 1, RoundKey.Length - 1);

            RoundKey[0] = (byte)((a * b) + c);
            for (uint i = 0; i < Data.Length; i++)
            {
                tmp = Data[i];
                a = (RoundKey[0] + i + 128) & 255;
                b = (RoundKey[^1] + i) % 256;

                a = ((a << 1) | (a >> 7)) % 256;
                b = ((b >> 2) | (b << 6)) % 256;

                c = RoundKey[(a ^ b) % 256];

                Data[i] ^= (byte)((RoundKey[0] * RoundKey[128]) & 255);

                Array.Copy(RoundKey, 0, RoundKey, 1, RoundKey.Length - 1);

                RoundKey[0] = (byte)((a * b) + c + tmp);
            }
            return Data;
        }
        public static byte[] Decrypt(byte[] Data, byte[] Key)
        {
            byte[] RoundKey = Generate(Key);
            uint a, b, c;
            a = (uint)((RoundKey[0]) & 255);
            b = (uint)((RoundKey[^1] + 128) % 256);

            a = ((a << 1) | (a >> 7)) % 256;
            b = ((b >> 2) | (b << 6)) % 256;

            c = RoundKey[(a ^ b) % 256];

            Array.Copy(RoundKey, 0, RoundKey, 1, RoundKey.Length - 1);

            RoundKey[0] = (byte)((a * b) + c);
            for (uint i = 0; i < Data.Length; i++)
            {

                a = (RoundKey[0] + i + 128) & 255;
                b = (RoundKey[^1] + i) % 256;

                a = ((a << 1) | (a >> 7)) % 256;
                b = ((b >> 2) | (b << 6)) % 256;

                c = RoundKey[(a ^ b) % 256];

                Data[i] ^= (byte)((RoundKey[0] * RoundKey[128]) & 255);

                Array.Copy(RoundKey, 0, RoundKey, 1, RoundKey.Length - 1);

                RoundKey[0] = (byte)((a * b) + c + Data[i]);
            }
            return Data;
        }
        public static byte[] FromHex(string hex)
        {
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return raw;
        }
        static void Main(string[] args)
        {
            Console.WriteLine("$(cmds: 'exit', 'clear')");

            string separator = "";
            while (true)
            {
                Console.WriteLine();
                Console.Write("(1) encrypt // (2) decrypt >> ");
                string eord = Console.ReadLine();

                if (eord.ToLower().StartsWith("1"))
                {
                    Console.Write("Text >> "); byte[] text = Encoding.ASCII.GetBytes(Console.ReadLine());
                    Console.Write("Key >> "); string THEKEY = Console.ReadLine(); byte[] key = Encoding.ASCII.GetBytes(THEKEY);

                    Console.Write("$Encrypted text >> ");

                    string alr = BitConverter.ToString(Encrypt(text, key)).Replace("-", separator);
                    int alrLEN = alr.Length / 2;
                    string one = alr.Substring(0, alrLEN);
                    string two = alr.Substring(alrLEN);
                    string final = two + one;

                    string FINALREAL = THEKEY + " --+-- " + "UNW." + final;

                    Console.WriteLine("UNW." + final);

                }
                if (eord.ToLower().StartsWith("2"))
                {
                    Console.Write("Encrypted Text >> "); string jej = Console.ReadLine();
                    int index = jej.IndexOf("UNW.");
                    string encodedFR = jej.Substring(index + 4);
                    int ps1LEN = encodedFR.Length / 2;
                    string ps2 = encodedFR.Substring(0, ps1LEN);
                    string ps2_2 = encodedFR.Substring(ps1LEN);
                    string textin = ps2_2 + ps2;

                    
                    Console.Write("Key >> "); byte[] key = Encoding.ASCII.GetBytes(Console.ReadLine());


                    if (separator != "")
                    {
                        textin = textin.Replace(separator, "");
                    }
                    byte[] decrypted = Decrypt(FromHex(textin), key);

                    Console.Write("$Decrypted text >> ");
                    Console.WriteLine(Encoding.ASCII.GetString(decrypted));

                }
                if (eord.ToLower().StartsWith("exit"))
                {
                    Console.Clear();
                    Console.WriteLine("$exiting now...");
                    Thread.Sleep(250); Environment.Exit(0);


                }
                if (eord.ToLower().StartsWith("clear"))
                {

                    Console.Clear();
                    Console.WriteLine("$(cmds: 'exit', 'clear')");
                }
            }
        }
    }
}
