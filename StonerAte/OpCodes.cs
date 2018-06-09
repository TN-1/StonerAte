/*
StonerAte - A Chip 8 Emulator
Copyright (C) 2018 Hamish West, hamish@hamishwest.xyz, github.com/TN-1

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace StonerAte
{
    public partial class Cpu
    {
        Random rnd = new Random();

        /// <summary>
        /// Jump to location in memory
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void JP_0NNN()
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Clears the graphics display
        /// </summary>
        private void CLS_00E0()
        {
            for (var x = 0; x < Gfx.GetLength(0); x++)
            {
                for (var y = 0; y < Gfx.GetLength(1); y++)
                {
                    Gfx[x, y] = 0;
                }
            }
            
        }

        /// <summary>
        /// Return to previous memory location from a subroutine
        /// </summary>
        public void RET_00EE()
        {
            if(Sp != 0)
                Sp--;
            Pc = Stack[Sp];            
        }

        /// <summary>
        /// Jumps to a different location in memory
        /// </summary>
        /// <param name="n">Memory address to jump too</param>
        public void JP_1nnn(string n)
        {
            Pc = Convert.ToInt16(n, 16); 
        }

        /// <summary>
        /// Calls a subroutine, storing current mem address in stack for return
        /// </summary>
        /// <param name="n">Memory address of subroutine to call</param>
        public void CALL_2nnn(string n)
        {
            Sp++;

            if (Sp > Stack.Length)
            {
                Sp--;
                throw new Exception("SP exceeds length of stack");
            }

            Stack[Sp] = Pc;
            Pc = short.Parse(n, NumberStyles.HexNumber);
        }

        /// <summary>
        /// Skip next instruction if Vx = k
        /// </summary>
        /// <param name="x">Register to source</param>
        /// <param name="k">Value to compare too</param>
        public void SE_3xkk(string x, string k)
        {
            if (V[int.Parse(x, NumberStyles.HexNumber)] == byte.Parse(k, NumberStyles.HexNumber))
                Pc = (short)(Pc + 2);
            
        }

        /// <summary>
        /// Skip next instruction if Vk != k
        /// </summary>
        /// <param name="x">Register to source</param>
        /// <param name="k">Value to compare too</param>
        public void SNE_4xkk(string x, string k)
        {
            if (V[int.Parse(x, NumberStyles.HexNumber)] != Convert.ToByte(k))
                Pc = (short)(Pc + 2);
            
        }

        /// <summary>
        /// Skip next instruction if Vx = Vy
        /// </summary>
        /// <param name="x">Register to source</param>
        /// <param name="y">Register to compare too</param>
        public void SE_5xy0(string x, string y)
        {
            if (V[int.Parse(x, NumberStyles.HexNumber)] == V[int.Parse(y, NumberStyles.HexNumber)])
                Pc = (short)(Pc + 2);
            
        }

        /// <summary>
        /// Loads a byte into Vx
        /// </summary>
        /// <param name="x">Register to address</param>
        /// <param name="k">Byte to load</param>
        public void LD_6xkk(string x, byte k)
        {
            V[int.Parse(x, NumberStyles.HexNumber)] = k;
            
        }

        /// <summary>
        /// Adds a byte to Vx
        /// </summary>
        /// <param name="x">Register to address</param>
        /// <param name="k">Byte to add</param>
        public void ADD_7xkk(string x, byte k)
        {
            V[int.Parse(x, NumberStyles.HexNumber)] += k;
            
        }

        /// <summary>
        /// Copies a byte from Vx to Vy
        /// </summary>
        /// <param name="x">Vx</param>
        /// <param name="y">Vy</param>
        public void LD_8xy0(string x, string y)
        {
            V[int.Parse(y, NumberStyles.HexNumber)] =
                V[int.Parse(x, NumberStyles.HexNumber)];
            
        }

        /// <summary>
        /// Performs an OR operation on Vx and Vy then stores the result in Vx
        /// </summary>
        /// <param name="x">Vx</param>
        /// <param name="y">Vy</param>
        public void OR_8xy1(string x, string y)
        {
            V[int.Parse(x, NumberStyles.HexNumber)] =
                (byte) (V[int.Parse(x, NumberStyles.HexNumber)] |
                        V[int.Parse(y, NumberStyles.HexNumber)]);
            
        }
        
        /// <summary>
        /// Performs an AND operation on Vx and Vy then stores the result in Vx
        /// </summary>
        /// <param name="x">Vx</param>
        /// <param name="y">Vy</param>
        public void AND_8xy2(string x, string y)
        {
            V[int.Parse(x, NumberStyles.HexNumber)] =
                (byte) (V[int.Parse(x, NumberStyles.HexNumber)] &
                        V[int.Parse(y, NumberStyles.HexNumber)]);
            
        }

        /// <summary>
        /// Performs on exclsive OR operation on Vx and Vy then stores the result in Vx
        /// </summary>
        /// <param name="x">Vx</param>
        /// <param name="y">Vy</param>
        public void XOR_8xy3(string x, string y)
        {
            V[int.Parse(x, NumberStyles.HexNumber)] =
                (byte) (V[int.Parse(x, NumberStyles.HexNumber)] ^
                        V[int.Parse(y, NumberStyles.HexNumber)]);   
            
        }

        /// <summary>
        /// Adds values of Vx and Vy, stores result in Vx. Vf is set if result is greator than 8 bits(255)
        /// </summary>
        /// <param name="x">Vx</param>
        /// <param name="y">Vy</param>
        public void ADD_8xy4(string x, string y)
        {
            var result = V[int.Parse(x, NumberStyles.HexNumber)] +
                         V[int.Parse(y, NumberStyles.HexNumber)];
            if (result <= 255)
            {
                V[int.Parse(x, NumberStyles.HexNumber)] = (byte)result;
            }
            else
            {
                V[15] = 1;
                V[int.Parse(x, NumberStyles.HexNumber)] = (byte)result;
            }
            
        }

        /// <summary>
        /// Subtracts Vy from Vx, sets VF to 1 if Vy less than Vx
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SUB_8xy5(string x, string y)
        {
            if (V[int.Parse(x, NumberStyles.HexNumber)] >
                V[int.Parse(y, NumberStyles.HexNumber)])
            {
                V[15] = 1;
            }

            V[int.Parse(x, NumberStyles.HexNumber)] =
                (byte)(V[int.Parse(x, NumberStyles.HexNumber)] -
                V[int.Parse(y, NumberStyles.HexNumber)]);
            
        }
        
        /// <summary>
        /// Shifts Vy right one bit and stores in Vx, Stores least significant bit in Vf
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SHR_8xy6(string x, string y)
        {
            V[15] = (byte) (V[int.Parse(y, NumberStyles.HexNumber)] & 0x00F);
            V[int.Parse(x, NumberStyles.HexNumber)] =
                (byte) (V[int.Parse(y, NumberStyles.HexNumber)] >> 1);
            

        }
        
        /// <summary>
        /// Subtracts Vx from Vy, sets VF to 1 if Vy > Vx
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SUBN_8xy7(string x, string y)
        {
            if (V[int.Parse(y, NumberStyles.HexNumber)] >
                V[int.Parse(x, NumberStyles.HexNumber)])
            {
                V[15] = 1;
            }

            V[int.Parse(x, NumberStyles.HexNumber)] =
                (byte)(V[int.Parse(y, NumberStyles.HexNumber)] -
                       V[int.Parse(x, NumberStyles.HexNumber)]);
            
        }
        
        /// <summary>
        /// Shifts Vy left one bit and stores in Vx, Stores least significant bit in Vf
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SHL_8xyE(string x, string y)
        {
            V[15] = (byte) (V[int.Parse(y, NumberStyles.HexNumber)] & 0x00F);
            V[int.Parse(x, NumberStyles.HexNumber)] =
                (byte) (V[int.Parse(y, NumberStyles.HexNumber)] << 1);
            
        }

        /// <summary>
        /// Skip next instruction if Vx != Vy
        /// </summary>
        /// <param name="x">Vx</param>
        /// <param name="y">Vy</param>
        public void SNE_9xy0(string x, string y)
        {
            if (V[int.Parse(x, NumberStyles.HexNumber)] != V[int.Parse(y, NumberStyles.HexNumber)])
                Pc = (short)(Pc + 2);   
            
        }

        /// <summary>
        /// Set register I to n
        /// </summary>
        /// <param name="n"></param>
        public void LD_Annn(short n)
        {
            I = n;
            
        }

        /// <summary>
        /// Set PC to n + V0
        /// </summary>
        /// <param name="n"></param>
        public void JP_Bnnn(short n)
        {
            Pc = (short)(n + V[0]);
            
        }

        /// <summary>
        /// Sets Vx to a rnd number + k
        /// </summary>
        /// <param name="x">Vx</param>
        /// <param name="k">Value to add to number</param>
        public void RND_Cxkk(string x, byte k)
        {
            V[int.Parse(x, NumberStyles.HexNumber)] = (byte) (rnd.Next(0, 255) + k);
            
        }

        /// <summary>
        /// Draw n-byte sprite at location Vx, Vy. Set VF if collision.
        /// </summary>
        /// <param name="x">Vx</param>
        /// <param name="y">Vy</param>
        /// <param name="n">Length of sprite</param>
        public void DRW_Dxyn(string x, string y, byte n)
        {
            //TODO: Need to add collision detection
            var sprite = new List<byte>();
            var xLoc = V[int.Parse(x, NumberStyles.HexNumber)];
            var yLoc = V[int.Parse(y, NumberStyles.HexNumber)];
            
            for (int i = I; i < I + n; i++)
            {
                sprite.Add(Memory[i]);
            }

            for (var i = 0; i < sprite.Count; i++)
            {
                var bits = new BitArray(BitConverter.GetBytes(sprite[i]).ToArray());
                for (var k = 4; k < 8; k++)
                {
                    //TODO: This sucks, I need to rework it
                    if (xLoc + k > 63)
                    {
                        var value = Convert.ToInt32(bits[k]) ^ Gfx[xLoc + k - 63, yLoc + i];
                        Gfx[xLoc + k - 63, yLoc + i] = value;
                    }
                    else
                    {
                        var value = Convert.ToInt32(bits[k]) ^ Gfx[xLoc + k, yLoc + i];
                        Gfx[xLoc + k, yLoc + i] = value; 
                    }
                }
            }

            DrawFlag = true;
        }

        /// <summary>
        /// Skip next instruction if key with Vx is currently pressed
        /// </summary>
        /// <param name="x">Vx</param>
        public void SKP_Ex9E(string x)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Skip next instruction if key with Vx is not being pressed
        /// </summary>
        /// <param name="x">Vx</param>
        public void SKNP_ExA1(string x)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Load value of Delay Timer into Vx
        /// </summary>
        /// <param name="x">Vx</param>
        public void LD_Fx07(string x)
        {
            V[int.Parse(x, NumberStyles.HexNumber)] = Dt;
        }

        /// <summary>
        /// Wait for key press, then store value in Vx
        /// </summary>
        /// <param name="x">Vx</param>
        public void LD_Fx0A(string x)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Load value of Vx into Delay Timer 
        /// </summary>
        /// <param name="x">Vx</param>
        public void LD_Fx15(string x)
        {
            Dt = V[int.Parse(x, NumberStyles.HexNumber)];
        }
        
        /// <summary>
        /// Load value of Vx into Sound Timer
        /// </summary>
        /// <param name="x">Vx</param>
        public void LD_Fx18(string x)
        {
            St = V[int.Parse(x, NumberStyles.HexNumber)];
        }

        /// <summary>
        /// Set I as I + Vx
        /// </summary>
        /// <param name="x">Vx</param>
        public void ADD_Fx1E(string x)
        {
            I += V[int.Parse(x, NumberStyles.HexNumber)];
        }

        /// <summary>
        /// Load location of sprite for Vx into I
        /// </summary>
        /// <param name="x">Vx</param>
        public void LD_Fx29(string x)
        {
            I = (short)(5 * V[int.Parse(x, NumberStyles.HexNumber)]);
        }

        /// <summary>
        /// Takes Vx, stores Hundreds in I, tens into I+1, ones into I+2
        /// </summary>
        /// <param name="x">Vx</param>
        public void LD_Fx33(string x)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stores V0 through Vx into memory starting at location I
        /// I = I + Vx + 1 after operation
        /// </summary>
        /// <param name="x">Vx</param>
        public void LD_Fx55(string x)
        {
            for (var i = 0; i < int.Parse(x, NumberStyles.HexNumber); i++)
            {
                Memory[I + i] = V[i];
            }
        }

        /// <summary>
        /// Loads from memory starting at location I into V0 through Vx
        /// I = I + Vx + 1 after operation
        /// </summary>
        /// <param name="x"></param>
        public void LD_Fx65(string x)
        {
            for (var i = 0; i < int.Parse(x, NumberStyles.HexNumber); i++)
            {
                V[i] = Memory[I + i];
            }
        }
    }
}