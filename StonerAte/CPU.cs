﻿/*Chip 8 mem map
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
using System.Threading;

// ReSharper disable FieldCanBeMadeReadOnly.Global

namespace StonerAte
{
    /// <summary>
    /// Represents the main class of the emulator
    /// </summary>
    public partial class Cpu
    {
        //4k bytes of RAM
        public byte[] Memory = new byte[4096];

        private readonly byte[] _fontset =
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
        public byte[] RomBytes;
        //16 length stack to store PC in when jumping to subroutines
        public short[] Stack = new short[16];
        //Index register
        public byte I;
        //PC - Memory location of current instruction
        public short Pc;
        //Current level of stack in use
        public short Sp;
        //Current opcode in operation
        public string Opcode;
        //Represents graphics screen
        public int [,] Gfx = new int[64,32];
        //Set clock speed of execution in hz
        public int Freq = 1;
                
        /// <summary>
        /// Initialize all memory to expcted defaults for begin of execution
        /// </summary>
        public void Initialize()
        {
            Pc = 0x200;
            I = 0;
            Sp = 0;
            Opcode = "0x000";
            
            for (var i = 0; i < Memory.Length; i++)
            {
                Memory[i] = 0x000;
            }
            
            for (var i = 0; i < V.Length; i++)
            {
                V[i] = 0x000;
            }
            
            for (var i = 0; i < Stack.Length; i++)
            {
                Stack[i] = 0x000;
            }

            for (var i = 0; i < _fontset.Length; i++)
            {
                Memory[i] = _fontset[i];
            }
            
            for (var x = 0; x < Gfx.GetLength(0); x++)
            {
                for (var y = 0; y < Gfx.GetLength(1); y++)
                {
                    if (x % 2 == 0)
                        Gfx[x, y] = 1;
                    else
                        Gfx[x, y] = 0;
                }
            }
        }

        /// <summary>
        /// Loads a specified ROM from /roms to memory ready for execution
        /// </summary>
        /// <param name="name">Name of rom minus the filetype(assumed .ch8)</param>
        public void LoadRom(string name)
        {
            RomBytes = File.ReadAllBytes($"{AppDomain.CurrentDomain.BaseDirectory}/roms/{name}.ch8");
            for (var i = 0; i < RomBytes.Length; i++)
            {
                //i + 0x200 per mem map
                Memory[i + 512] = RomBytes[i];
            }

        }
        
        /// <summary>
        /// Emulates a CPU cycle of fetch, decode and execute
        /// </summary>
        public void EmulateCycle(MainForm form)
        {
           
            //TODO: See if we can solve the SIGSEV when NotImpleExcep is thrown more than once. No rush though :)
            //Store current opcode in a format we like
            Opcode = Memory[Pc].ToString("X2") + Memory[Pc + 1].ToString("X2");

                switch (Opcode)
                {
                    case "00E0":
                        CLS_00E0();
                        form.AddText("CLS");
                        break;
                    case "00EE":
                        RET_00EE();
                        form.AddText("RET");
                        break;
                    default:
                        //YAY for nesting! LOL JKS
                        switch (Opcode.Substring(0, 1))
                        {
                            case "1":
                                JP_1nnn(Opcode.Substring(1,3));
                                form.AddText($"JP {Opcode.Substring(1,3)}");
                                break;
                            case "2":
                                CALL_2nnn(Opcode.Substring(1,3));
                                form.AddText($"CALL {Opcode.Substring(1,3)}");
                                break;
                            case "3":
                                SE_3xkk(Opcode.Substring(1,1), Opcode.Substring(2,2));
                                form.AddText($"SE {Opcode.Substring(1,1)}, {Opcode.Substring(2,2)}");
                                break;
                            case "4":
                                SNE_4xkk(Opcode.Substring(1,1), Opcode.Substring(2,2));
                                form.AddText($"SNE {Opcode.Substring(1,1)}, {Opcode.Substring(2,2)}");
                                break;
                            case "5":
                                SE_5xy0(Opcode.Substring(1,1), Opcode.Substring(2,2));
                                form.AddText($"SE {Opcode.Substring(1,1)}, {Opcode.Substring(2,2)}");
                                break;
                            case "6":
                                LD_6xkk(Opcode.Substring(1,1), byte.Parse(Opcode.Substring(2,2), NumberStyles.HexNumber));
                                form.AddText($"LD {Opcode.Substring(1,1)}, {string.Format(Opcode.Substring(2,2), NumberStyles.HexNumber)}");
                                break;
                            case "7":
                                ADD_7xkk(Opcode.Substring(1,1), byte.Parse(Opcode.Substring(2,2), NumberStyles.HexNumber));
                                form.AddText($"ADD {Opcode.Substring(1,1)}, {string.Format(Opcode.Substring(2,2), NumberStyles.HexNumber)}");
                                break;
                            case "8":
                                switch (Opcode.Substring(3, 1))
                                {
                                    case "0":
                                        LD_8xy0(Opcode.Substring(1,1), Opcode.Substring(2,1));
                                        form.AddText($"LD {Opcode.Substring(1,1)}, {Opcode.Substring(2,1)}");
                                        break;
                                    case "1":
                                        OR_8xy1(Opcode.Substring(1,1), Opcode.Substring(2,1));
                                        form.AddText($"OR {Opcode.Substring(1,1)}, {Opcode.Substring(2,1)}");
                                        break;
                                    case "2":
                                        AND_8xy2(Opcode.Substring(1,1), Opcode.Substring(2,1));
                                        form.AddText($"AND {Opcode.Substring(1,1)}, {Opcode.Substring(2,1)}");
                                        break;
                                    case "3":
                                        XOR_8xy3(Opcode.Substring(1,1), Opcode.Substring(2,1));
                                        form.AddText($"XOR {Opcode.Substring(1,1)}, {Opcode.Substring(2,1)}");
                                        break;
                                    case "4":
                                        ADD_8xy4(Opcode.Substring(1,1), Opcode.Substring(2,1));
                                        form.AddText($"ADD {Opcode.Substring(1,1)}, {Opcode.Substring(2,1)}");
                                        break;
                                    case "5":
                                        SUB_8xy5(Opcode.Substring(1,1), Opcode.Substring(2,1));
                                        form.AddText($"SUB {Opcode.Substring(1,1)}, {Opcode.Substring(2,1)}");
                                        break;
                                    case "6":
                                        SHR_8xy6(Opcode.Substring(1,1), Opcode.Substring(2,1));
                                        form.AddText($"SHR {Opcode.Substring(1,1)}, {Opcode.Substring(2,1)}");
                                        break;
                                    case "7":
                                        SUBN_8xy7(Opcode.Substring(1,1), Opcode.Substring(2,1));
                                        form.AddText($"SHL {Opcode.Substring(1,1)}, {Opcode.Substring(2,1)}");
                                        break;
                                    case "E":
                                        SHL_8xyE(Opcode.Substring(1,1), Opcode.Substring(2,1));
                                        form.AddText($"SHL {Opcode.Substring(1,1)}, {Opcode.Substring(2,1)}");
                                        break;
                                    default:
                                        Console.WriteLine("Invalid opcode " + Opcode);
                                        form.AddText($"INVALID {Opcode}");
                                        break;
                                }
                                break;
                            case "9":
                                SNE_9xy0(Opcode.Substring(1,1), Opcode.Substring(2,1));
                                form.AddText($"SNE {Opcode.Substring(1,1)}, {Opcode.Substring(2,1)}");
                                break;
                            case "A":
                                LD_Annn(Convert.ToByte(Opcode.Substring(1,3)));
                                form.AddText($"LD {Opcode.Substring(1,3)}");
                                break;
                            case "B":
                                JP_Bnnn(Convert.ToInt16(Opcode.Substring(1,3)));
                                form.AddText($"JP {Opcode.Substring(1,3)}");
                                break;
                            case "C":
                                RND_Cxkk(Opcode.Substring(1,1), byte.Parse(Opcode.Substring(2,2), NumberStyles.HexNumber));
                                form.AddText($"RND {Opcode.Substring(1,1)}, {string.Format(Opcode.Substring(2,2), NumberStyles.HexNumber)}");
                                break;
                            case "D":
                                DRW_Dxyn(Opcode.Substring(1,1), Opcode.Substring(2,1), Convert.ToByte(Opcode.Substring(3,1)));
                                form.AddText($"DRW V{Opcode.Substring(1,1)}, V{Opcode.Substring(2,1)}, {Opcode.Substring(3,1)}");
                                break;
                            case "E":
                                switch (Opcode.Substring(2, 2))
                                {
                                    case "9E":
                                        SKP_Ex9E(Opcode.Substring(1,1));
                                        form.AddText($"SKP V{Opcode.Substring(1,1)}");
                                        break;
                                    case "A1":
                                        SKNP_ExA1(Opcode.Substring(1,1));
                                        form.AddText($"SKNP V{Opcode.Substring(1,1)}");
                                        break;
                                    default:
                                        Console.WriteLine("Invalid opcode " + Opcode);
                                        form.AddText($"INVALID {Opcode}");
                                        break;
                                }
                                break;
                            case "F":
                                switch (Opcode.Substring(2, 2))
                                {
                                    case "07":
                                        LD_Fx07(Opcode.Substring(1,1));
                                        form.AddText($"LD V{Opcode.Substring(1,1)}, DT");
                                        break;
                                    case "0A":
                                        LD_Fx0A(Opcode.Substring(1,1));
                                        form.AddText($"LD V{Opcode.Substring(1,1)}, K");
                                        break;
                                    case "15":
                                        LD_Fx15(Opcode.Substring(1,1));
                                        form.AddText($"LD DT, V{Opcode.Substring(1,1)}");
                                        break;
                                    case "18":
                                        LD_Fx18(Opcode.Substring(1,1));
                                        form.AddText($"LD ST, V{Opcode.Substring(1, 1)}");
                                        break;
                                    case "1E":
                                        ADD_Fx1E(Opcode.Substring(1,1));
                                        form.AddText($"ADD I, V{Opcode.Substring(1,1)}");
                                        break;
                                    case "29":
                                        LD_Fx29(Opcode.Substring(1,1));
                                        form.AddText($"LD F, V{Opcode.Substring(1,1)}");
                                        break;
                                    case "33":
                                        LD_Fx33(Opcode.Substring(1,1));
                                        form.AddText($"LD B, V{Opcode.Substring(1,1)}");
                                        break;
                                    case "55":
                                        LD_Fx55(Opcode.Substring(1,1));
                                        form.AddText($"LD [I], V{Opcode.Substring(1,1)}");
                                        break;
                                    case "65":
                                        LD_Fx65(Opcode.Substring(1,1));
                                        form.AddText($"LD V{Opcode.Substring(1,1)}, [I]");
                                        break;
                                    default:
                                        Console.WriteLine("Invalid opcode " + Opcode);
                                        form.AddText($"INVALID {Opcode}");
                                        break;
                                }
                                break;
                            default:
                                Console.WriteLine("Invalid opcode " + Opcode);
                                form.AddText($"INVALID {Opcode}");
                                break;
                        }
                        break;
                }
            
            //Inc PC to next instruction
            if(Opcode.Substring(0, 1) != 1.ToString() && Opcode != "00EE" && Opcode.Substring(0,1) != 2.ToString() && Opcode.Substring(0,1) != "B")
                Pc = (short) (Pc + 2);

            Console.Clear();
            Console.WriteLine($"PC: {Pc}, I: {I}, OpCode: {Opcode}");
            for (var i = 0; i < 16; i++)
            {
                Console.WriteLine($"V[{i}] = {V[i]}");
            }

            Thread.Sleep((1/Freq) * 1000);
        }
    }
}