/*Chip 8 mem map
 *
 * 0x000 Start of memory
 * reserved for interp (inc fontset from 0 to something)
 * 0x200 start of ROM
 * 0x600 start of ETI600 ROM
 * 0xFFF end of memory
 * 
 */

using System;
using System.Globalization;
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

        private byte[] fontset =
        {
            0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
            0x20, 0x60, 0x20, 0x20, 0x70, // 1
            0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
            0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
            0x90, 0x90, 0xF0, 0x10, 0x10, // 4
            0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
            0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
            0xF0, 0x10, 0x20, 0x40, 0x40, // 7
            0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
            0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
            0xF0, 0x90, 0xF0, 0x90, 0x90, // A
            0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
            0xF0, 0x80, 0x80, 0x80, 0xF0, // C
            0xE0, 0x90, 0x90, 0x90, 0xE0, // D
            0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
            0xF0, 0x80, 0xF0, 0x80, 0x80  // F
        };
        
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

            for (int i = 0; i < fontset.Length; i++)
            {
                memory[i] = fontset[i];
            }
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
                        CLS_00E00();
                        break;
                    case "00EE":
                        RET_00EE();
                        break;
                    default:
                        //YAY for nesting! LOL JKS
                        switch (opcode.Substring(0, 1))
                        {
                            case "1":
                                JP_1nnn(opcode.Substring(1,3));
                                break;
                            case "2":
                                CALL_2nnn(opcode.Substring(1,3));
                                break;
                            case "3":
                                SE_3xkk(opcode.Substring(1,1), opcode.Substring(2,2));
                                break;
                            case "4":
                                SNE_4xkk(opcode.Substring(1,1), opcode.Substring(2,2));
                                break;
                            case "5":
                                SE_5xy0(opcode.Substring(1,1), opcode.Substring(2,2));
                                break;
                            case "6":
                                LD_6xkk(opcode.Substring(1,1), Byte.Parse(opcode.Substring(2,2), NumberStyles.HexNumber));
                                break;
                            case "7":
                                ADD_7xkk(opcode.Substring(1,1), Byte.Parse(opcode.Substring(2,2), NumberStyles.HexNumber));
                                break;
                            case "8":
                                switch (opcode.Substring(3, 1))
                                {
                                    case "0":
                                        LD_8xy0(opcode.Substring(1,1), opcode.Substring(2,1));
                                        break;
                                    case "1":
                                        OR_8xy1(opcode.Substring(1,1), opcode.Substring(2,1));
                                        break;
                                    case "2":
                                        AND_8xy2(opcode.Substring(1,1), opcode.Substring(2,1));
                                        break;
                                    case "3":
                                        XOR_8xy3(opcode.Substring(1,1), opcode.Substring(2,1));
                                        break;
                                    case "4":
                                        ADD_8xy4(opcode.Substring(1,1), opcode.Substring(2,1));
                                        break;
                                    case "5":
                                        SUB_8xy5(opcode.Substring(1,1), opcode.Substring(2,1));
                                        break;
                                    case "6":
                                        SHR_8xy6(opcode.Substring(1,1), opcode.Substring(2,1));
                                        break;
                                    case "7":
                                        SUB_8xy7(opcode.Substring(1,1), opcode.Substring(2,1));
                                        break;
                                    case "E":
                                        SHL_8xyE(opcode.Substring(1,1), opcode.Substring(2,1));
                                        break;
                                    default:
                                        Console.WriteLine("Invalid opcode " + opcode);
                                        break;
                                }
                                break;
                            case "9":
                                SNE_9xy0(opcode.Substring(1,1), opcode.Substring(2,1));
                                break;
                            case "A":
                                LD_Annn(Convert.ToByte(opcode.Substring(1,3)));
                                break;
                            case "B":
                                JP_Bnnn(Convert.ToInt16(opcode.Substring(1,3)));
                                break;
                            case "C":
                                RND_Cxkk(opcode.Substring(1,1), Byte.Parse(opcode.Substring(2,2), NumberStyles.HexNumber));
                                break;
                            case "D":
                                throw new NotImplementedException();
                                break;
                            case "E":
                                switch (opcode.Substring(2, 2))
                                {
                                    case "9E":
                                        Console.WriteLine($"SKP V{opcode.Substring(1,1)}");
                                        throw new NotImplementedException();
                                        break;
                                    case "A1":
                                        Console.WriteLine($"SKNP V{opcode.Substring(1,1)}");
                                        throw new NotImplementedException();
                                        break;
                                    default:
                                        Console.WriteLine("Invalid opcode " + opcode);
                                        throw new NotImplementedException();
                                        break;
                                }
                                break;
                            case "F":
                                switch (opcode.Substring(2, 2))
                                {
                                    case "07":
                                        Console.WriteLine($"LD V{opcode.Substring(1,1)}, DT");
                                        throw new NotImplementedException();
                                        break;
                                    case "0A":
                                        Console.WriteLine($"LD V{opcode.Substring(1,1)}, K");
                                        throw new NotImplementedException();
                                        break;
                                    case "15":
                                        Console.WriteLine($"LD DT, V{opcode.Substring(1,1)}");
                                        throw new NotImplementedException();
                                        break;
                                    case "18":
                                        Console.WriteLine($"LD ST, V{opcode.Substring(1, 1)}");
                                        throw new NotImplementedException();
                                        break;
                                    case "1E":
                                        Console.WriteLine($"ADD I, V{opcode.Substring(1,1)}");
                                        throw new NotImplementedException();
                                        break;
                                    case "29":
                                        Console.WriteLine($"LD F, V{opcode.Substring(1,1)}");
                                        throw new NotImplementedException();
                                        break;
                                    case "33":
                                        Console.WriteLine($"LD B, V{opcode.Substring(1,1)}");
                                        throw new NotImplementedException();
                                        break;
                                    case "55":
                                        Console.WriteLine($"LD [I], V{opcode.Substring(1,1)}");
                                        throw new NotImplementedException();
                                        break;
                                    case "65":
                                        Console.WriteLine($"LD V{opcode.Substring(1,1)}, [I]");
                                        throw new NotImplementedException();
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
            if(opcode.Substring(0, 1) != 1.ToString() && opcode != "00EE" && opcode.Substring(0,1) != 2.ToString() && opcode.Substring(0,1) != "B")
                pc = (short) (pc + 2);
            
            Console.Clear();
            Console.WriteLine($"PC: {pc}, I: {I}, OpCode: {opcode}");
            for (int i = 0; i < 16; i++)
            {
                Console.WriteLine($"V[{i}] = {V[i]}");
            }
            
            System.Threading.Thread.Sleep(1000);
        }
    }
}