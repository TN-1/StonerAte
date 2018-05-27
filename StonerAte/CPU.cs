/*Chip 8 mem map
 *
 * 0x000 Start of memory
 * reserved for interp
 * 0x200 start of ROM
 * 0x600 start of ETI600 ROM
 * 0xFFF end of memory
 * 
 */

using System;
using System.IO;
using System.Text;

namespace StonerAte
{
    /// <summary>
    /// Represents the main class of the emulator
    /// </summary>
    public partial class CPU
    {
        //4k bytes of RAM
        public byte[] memory = new byte[4096];
        //16 8 bit registers. V[F] not to be used for general use
        public byte[] V = new byte[16];
        //Temp storage for reading rom into RAM
        public byte[] romBytes;
        //Represents the graphics to draw
        public byte[,] gfx =  new byte[64,32];
        //16 length stack to store PC in when jumping to subroutines
        public short[] stack = new short[16];
        //Index register
        public byte I;
        //PC - Memory location of current instruction
        public short pc;
        //Current level of stack in use
        public short sp;
        //Current opcode in operation
        public string opcode;
        
        /// <summary>
        /// Initialize all memory to expcted defaults for begin of execution
        /// </summary>
        public void initialize()
        {
            pc = 0x200;
            I = 0;
            sp = 0;
            opcode = "0x000";
            
            for (var i = 0; i < memory.Length; i++)
            {
                memory[i] = 0x000;
            }
            
            for (var i = 0; i < V.Length; i++)
            {
                V[i] = 0x000;
            }
            
            for (var i = 0; i < stack.Length; i++)
            {
                stack[i] = 0x000;
            }

            for (var x = 0; x < gfx.GetLength(0); x++)
            {
                for (var y = 0; y < gfx.GetLength(1); y++)
                {
                    gfx[x, y] = 0x000;
                }
            }
            
            //TODO: Load fontset
        }

        /// <summary>
        /// Loads a specified ROM from /roms to memory ready for execution
        /// </summary>
        /// <param name="name">Name of rom minus the filetype(assumed .ch8)</param>
        public void LoadRom(string name)
        {
            romBytes = File.ReadAllBytes($"{AppDomain.CurrentDomain.BaseDirectory}/roms/{name}.ch8");
            for (var i = 0; i < romBytes.Length; i++)
            {
                //i + 0x200 per mem map
                memory[i + 512] = romBytes[i];
            }

        }
        
        /// <summary>
        /// Emulates a CPU cycle of fetch, decode and execute
        /// </summary>
        public void EmulateCycle()
        {
            //Store current opcode in a format we like
            opcode = memory[pc].ToString("X2") + memory[pc + 1].ToString("X2");

                switch (opcode)
                {
                    case "00E0":
                        Console.WriteLine("CLS");
                        CLS_00E00();
                        break;
                    case "00EE":
                        Console.WriteLine("RET");
                        RET_00EE();
                        break;
                    default:
                        //YAY for nesting! LOL JKS
                        switch (opcode.Substring(0, 1))
                        {
                            case "1":
                                Console.WriteLine($"JP {opcode.Substring(1,3)}");
                                JP_1nnn(opcode.Substring(1,3));
                                break;
                            case "2":
                                Console.WriteLine($"CALL {opcode.Substring(1,3)}");
                                CALL_2nnn(opcode.Substring(1,3));
                                break;
                            case "3":
                                Console.WriteLine($"SE V{opcode.Substring(1,1)}, {opcode.Substring(2,2)}");
                                SE_3xkk(opcode.Substring(1,1), opcode.Substring(2,2));
                                break;
                            case "4":
                                Console.WriteLine($"SNE V{opcode.Substring(1,1)}, {opcode.Substring(2,2)}");
                                SNE_4xkk(opcode.Substring(1,1), opcode.Substring(2,2));
                                break;
                            case "5":
                                Console.WriteLine($"SE V{opcode.Substring(1,1)}, V{opcode.Substring(2,1)}");
                                SE_5xy0(opcode.Substring(1,1), opcode.Substring(2,2));
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
            
            //Inc PC to next instruction
            if(opcode.Substring(0, 1) != 1.ToString())
                pc = (short) (pc + 2);
        }
    }
}