using System;
using System.IO;

namespace StonerAte
{
    class Decoder
    {
        public static void Decode()
        {
            
            Console.WriteLine("Reading ROM into memory...");
            
            //Read all bytes from rom file and setup variables
            var romBytes = File.ReadAllBytes(Environment.CurrentDirectory + "/roms/Pong (alt).ch8");
            var rom = new string[romBytes.Length / 2];
            var j = 0;
            
            //Iterate every second entry in array, and add the bytes to form our 2 byte opcodes
            //This will probably need to be removed for the emulator, but for the purposes of decoding
            //it should be ok. Operative word being should.
            for (var i = 0; i < romBytes.Length; i = i + 2)
            {
                rom[j] = romBytes[i].ToString("X2") + romBytes[i + 1].ToString("X2");
                j++;
            }

            //Dont need this anymore, mark for GC
            // ReSharper disable once RedundantAssignment
            romBytes = null;

            Console.WriteLine("Decode opcodes in memory one by one");
            foreach (var opcode in rom)
            {
                switch (opcode)
                {
                    case "00E0":
                        Console.WriteLine("CLS");
                        break;
                    case "00EE":
                        Console.WriteLine("RET");
                        break;
                    default:
                        //YAY for nesting! LOL JKS
                        switch (opcode.Substring(0, 1))
                        {
                            case "1":
                                Console.WriteLine($"JP {opcode.Substring(1,3)}");
                                break;
                            case "2":
                                Console.WriteLine($"CALL {opcode.Substring(1,3)}");
                                break;
                            case "3":
                                Console.WriteLine($"SE V{opcode.Substring(1,1)}, {opcode.Substring(2,2)}");
                                break;
                            case "4":
                                Console.WriteLine($"SNE V{opcode.Substring(1,1)}, {opcode.Substring(2,2)}");
                                break;
                            case "5":
                                Console.WriteLine($"SE V{opcode.Substring(1,1)}, V{opcode.Substring(2,1)}");
                                break;
                            case "6":
                                Console.WriteLine($"LD V{opcode.Substring(1,1)}, {opcode.Substring(2,2)}");
                                break;
                            case "7":
                                Console.WriteLine($"ADD V{opcode.Substring(1,1)}, {opcode.Substring(2,2)}");
                                break;
                            case "8":
                                switch (opcode.Substring(3, 1))
                                {
                                    case "0":
                                        Console.WriteLine($"LD V{opcode.Substring(1,1)}, V{opcode.Substring(2,2)}");
                                        break;
                                    case "1":
                                        Console.WriteLine($"OR V{opcode.Substring(1,1)}, V{opcode.Substring(2,2)}");
                                        break;
                                    case "2":
                                        Console.WriteLine($"AND V{opcode.Substring(1,1)}, V{opcode.Substring(2,2)}");
                                        break;
                                    case "3":
                                        Console.WriteLine($"XOR V{opcode.Substring(1,1)}, V{opcode.Substring(2,2)}");
                                        break;
                                    case "4":
                                        Console.WriteLine($"ADD V{opcode.Substring(1,1)}, V{opcode.Substring(2,2)}");
                                        break;
                                    case "5":
                                        Console.WriteLine($"SUB V{opcode.Substring(1,1)}, V{opcode.Substring(2,2)}");
                                        break;
                                    case "6":
                                        Console.WriteLine($"SHR V{opcode.Substring(1,1)}, V{opcode.Substring(2,2)}");
                                        break;
                                    case "7":
                                        Console.WriteLine($"SUBN V{opcode.Substring(1,1)}, V{opcode.Substring(2,2)}");
                                        break;
                                    case "E":
                                        Console.WriteLine($"SHL V{opcode.Substring(1,1)}, V{opcode.Substring(2,2)}");
                                        break;
                                    default:
                                        Console.WriteLine("Invalid opcode " + opcode);
                                        break;
                                }
                                break;
                            case "9":
                                Console.WriteLine($"SNE V{opcode.Substring(1,1)}, V{opcode.Substring(2,1)}");
                                break;
                            case "A":
                                Console.WriteLine($"LD I, {opcode.Substring(1,3)}");
                                break;
                            case "B":
                                Console.WriteLine($"JP V0, {opcode.Substring(1,3)}");
                                break;
                            case "C":
                                Console.WriteLine($"RND V{opcode.Substring(1,1)}, {opcode.Substring(2,2)}");
                                break;
                            case "D":
                                Console.WriteLine($"DRW V{opcode.Substring(1,1)}, V{opcode.Substring(2,1)}, {opcode.Substring(3,1)}");
                                break;
                            case "E":
                                switch (opcode.Substring(2, 2))
                                {
                                    case "9E":
                                        Console.WriteLine($"SKP V{opcode.Substring(1,1)}");
                                        break;
                                    case "A1":
                                        Console.WriteLine($"SKNP V{opcode.Substring(1,1)}");
                                        break;
                                    default:
                                        Console.WriteLine("Invalid opcode " + opcode);
                                        break;
                                }
                                break;
                            case "F":
                                switch (opcode.Substring(2, 2))
                                {
                                    case "07":
                                        Console.WriteLine($"LD V{opcode.Substring(1,1)}, DT");
                                        break;
                                    case "0A":
                                        Console.WriteLine($"LD V{opcode.Substring(1,1)}, K");
                                        break;
                                    case "15":
                                        Console.WriteLine($"LD DT, V{opcode.Substring(1,1)}");
                                        break;
                                    case "18":
                                        Console.WriteLine($"LD ST, V{opcode.Substring(1,1)}");
                                        break;
                                    case "1E":
                                        Console.WriteLine($"ADD I, V{opcode.Substring(1,1)}");
                                        break;
                                    case "29":
                                        Console.WriteLine($"LD F, V{opcode.Substring(1,1)}");
                                        break;
                                    case "33":
                                        Console.WriteLine($"LD B, V{opcode.Substring(1,1)}");
                                        break;
                                    case "55":
                                        Console.WriteLine($"LD [I], V{opcode.Substring(1,1)}");
                                        break;
                                    case "65":
                                        Console.WriteLine($"LD V{opcode.Substring(1,1)}, [I]");
                                        break;
                                    default:
                                        Console.WriteLine("Invalid opcode " + opcode);
                                        break;
                                }
                                break;
                            default:
                                Console.WriteLine("Invalid opcode " + opcode);
                                break;
                        }

                        break;
                }
            }

            Console.WriteLine("Done?");
        }
    }
}