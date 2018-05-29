using System;

namespace StonerAte
{
    public partial class CPU
    {
        Random rnd = new Random();
        
        /// <summary>
        /// Clears the graphics display
        /// </summary>
        private void CLS_00E0()
        {
            for (var x = 0; x < gfx.GetLength(0); x++)
            {
                for (var y = 0; y < gfx.GetLength(1); y++)
                {
                    gfx[x, y] = 0x000;
                }
            }
            
        }

        /// <summary>
        /// Return to previous memory location from a subroutine
        /// </summary>
        public void RET_00EE()
        {
            pc = stack[sp];
            if(sp != 0)
                sp--;
            
        }

        /// <summary>
        /// Jumps to a different location in memory
        /// </summary>
        /// <param name="n">Memory address to jump too</param>
        public void JP_1nnn(string n)
        {
            pc = Convert.ToInt16(n);
            
        }

        /// <summary>
        /// Calls a subroutine, storing current mem address in stack for return
        /// </summary>
        /// <param name="n">Memory address of subroutine to call</param>
        public void CALL_2nnn(string n)
        {
            stack[sp] = pc;
            pc = Convert.ToInt16(n);
            sp++;
            
        }

        /// <summary>
        /// Skip next instruction if Vx = k
        /// </summary>
        /// <param name="x">Register to source</param>
        /// <param name="k">Value to compare too</param>
        public void SE_3xkk(string x, string k)
        {
            if (V[Int32.Parse(x, System.Globalization.NumberStyles.HexNumber)] == Convert.ToByte(k))
                pc = (short)(pc + 2);
            
        }

        /// <summary>
        /// Skip next instruction if Vk != k
        /// </summary>
        /// <param name="x">Register to source</param>
        /// <param name="k">Value to compare too</param>
        public void SNE_4xkk(string x, string k)
        {
            if (V[Int32.Parse(x, System.Globalization.NumberStyles.HexNumber)] != Convert.ToByte(k))
                pc = (short)(pc + 2);
            
        }

        /// <summary>
        /// Skip next instruction if Vx = Vy
        /// </summary>
        /// <param name="x">Register to source</param>
        /// <param name="y">Register to compare too</param>
        public void SE_5xy0(string x, string y)
        {
            if (V[Int32.Parse(x, System.Globalization.NumberStyles.HexNumber)] == V[Int32.Parse(y, System.Globalization.NumberStyles.HexNumber)])
                pc = (short)(pc + 2);
            
        }

        /// <summary>
        /// Loads a byte into Vx
        /// </summary>
        /// <param name="x">Register to address</param>
        /// <param name="k">Byte to load</param>
        public void LD_6xkk(string x, byte k)
        {
            V[Int32.Parse(x, System.Globalization.NumberStyles.HexNumber)] = k;
            
        }

        /// <summary>
        /// Adds a byte to Vx
        /// </summary>
        /// <param name="x">Register to address</param>
        /// <param name="k">Byte to add</param>
        public void ADD_7xkk(string x, byte k)
        {
            V[Int32.Parse(x, System.Globalization.NumberStyles.HexNumber)] += k;
            
        }

        /// <summary>
        /// Copies a byte from Vx to Vy
        /// </summary>
        /// <param name="x">Vx</param>
        /// <param name="y">Vy</param>
        public void LD_8xy0(string x, string y)
        {
            V[Int32.Parse(y, System.Globalization.NumberStyles.HexNumber)] =
                V[Int32.Parse(x, System.Globalization.NumberStyles.HexNumber)];
            
        }

        /// <summary>
        /// Performs an OR operation on Vx and Vy then stores the result in Vx
        /// </summary>
        /// <param name="x">Vx</param>
        /// <param name="y">Vy</param>
        public void OR_8xy1(string x, string y)
        {
            V[Int32.Parse(x, System.Globalization.NumberStyles.HexNumber)] =
                (byte) (V[Int32.Parse(x, System.Globalization.NumberStyles.HexNumber)] |
                        V[Int32.Parse(y, System.Globalization.NumberStyles.HexNumber)]);
            
        }
        
        /// <summary>
        /// Performs an AND operation on Vx and Vy then stores the result in Vx
        /// </summary>
        /// <param name="x">Vx</param>
        /// <param name="y">Vy</param>
        public void AND_8xy2(string x, string y)
        {
            V[Int32.Parse(x, System.Globalization.NumberStyles.HexNumber)] =
                (byte) (V[Int32.Parse(x, System.Globalization.NumberStyles.HexNumber)] &
                        V[Int32.Parse(y, System.Globalization.NumberStyles.HexNumber)]);
            
        }

        /// <summary>
        /// Performs on exclsive OR operation on Vx and Vy then stores the result in Vx
        /// </summary>
        /// <param name="x">Vx</param>
        /// <param name="y">Vy</param>
        public void XOR_8xy3(string x, string y)
        {
            V[Int32.Parse(x, System.Globalization.NumberStyles.HexNumber)] =
                (byte) (V[Int32.Parse(x, System.Globalization.NumberStyles.HexNumber)] ^
                        V[Int32.Parse(y, System.Globalization.NumberStyles.HexNumber)]);   
            
        }

        /// <summary>
        /// Adds values of Vx and Vy, stores result in Vx. Vf is set if result is greator than 8 bits(255)
        /// </summary>
        /// <param name="x">Vx</param>
        /// <param name="y">Vy</param>
        public void ADD_8xy4(string x, string y)
        {
            var result = V[Int32.Parse(x, System.Globalization.NumberStyles.HexNumber)] +
                         V[Int32.Parse(y, System.Globalization.NumberStyles.HexNumber)];
            if (result <= 255)
            {
                V[Int32.Parse(x, System.Globalization.NumberStyles.HexNumber)] = (byte)result;
            }
            else
            {
                V[15] = 1;
                V[Int32.Parse(x, System.Globalization.NumberStyles.HexNumber)] = (byte)result;
            }
            
        }

        /// <summary>
        /// Subtracts Vy from Vx, sets VF to 1 if Vy < Vx
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SUB_8xy5(string x, string y)
        {
            if (V[Int32.Parse(x, System.Globalization.NumberStyles.HexNumber)] >
                V[Int32.Parse(y, System.Globalization.NumberStyles.HexNumber)])
            {
                V[15] = 1;
            }

            V[Int32.Parse(x, System.Globalization.NumberStyles.HexNumber)] =
                (byte)(V[Int32.Parse(x, System.Globalization.NumberStyles.HexNumber)] -
                V[Int32.Parse(y, System.Globalization.NumberStyles.HexNumber)]);
            
        }
        
        /// <summary>
        /// Shifts Vy right one bit and stores in Vx, Stores least significant bit in Vf
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SHR_8xy6(string x, string y)
        {
            V[15] = (byte) (V[Int32.Parse(y, System.Globalization.NumberStyles.HexNumber)] & 0x00F);
            V[Int32.Parse(x, System.Globalization.NumberStyles.HexNumber)] =
                (byte) (V[Int32.Parse(y, System.Globalization.NumberStyles.HexNumber)] >> 1);
            

        }
        
        /// <summary>
        /// Subtracts Vx from Vy, sets VF to 1 if Vy > Vx
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SUBN_8xy7(string x, string y)
        {
            if (V[Int32.Parse(y, System.Globalization.NumberStyles.HexNumber)] >
                V[Int32.Parse(x, System.Globalization.NumberStyles.HexNumber)])
            {
                V[15] = 1;
            }

            V[Int32.Parse(x, System.Globalization.NumberStyles.HexNumber)] =
                (byte)(V[Int32.Parse(y, System.Globalization.NumberStyles.HexNumber)] -
                       V[Int32.Parse(x, System.Globalization.NumberStyles.HexNumber)]);
            
        }
        
        /// <summary>
        /// Shifts Vy left one bit and stores in Vx, Stores least significant bit in Vf
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SHL_8xyE(string x, string y)
        {
            V[15] = (byte) (V[Int32.Parse(y, System.Globalization.NumberStyles.HexNumber)] & 0x00F);
            V[Int32.Parse(x, System.Globalization.NumberStyles.HexNumber)] =
                (byte) (V[Int32.Parse(y, System.Globalization.NumberStyles.HexNumber)] << 1);
            
        }

        /// <summary>
        /// Skip next instruction if Vx != Vy
        /// </summary>
        /// <param name="x">Vx</param>
        /// <param name="y">Vy</param>
        public void SNE_9xy0(string x, string y)
        {
            if (V[Int32.Parse(x, System.Globalization.NumberStyles.HexNumber)] != V[Int32.Parse(y, System.Globalization.NumberStyles.HexNumber)])
                pc = (short)(pc + 2);   
            
        }

        /// <summary>
        /// Set register I to n
        /// </summary>
        /// <param name="n"></param>
        public void LD_Annn(byte n)
        {
            I = n;
            
        }

        /// <summary>
        /// Set PC to n + V0
        /// </summary>
        /// <param name="n"></param>
        public void JP_Bnnn(short n)
        {
            pc = (short)(n + V[0]);
            
        }

        /// <summary>
        /// Sets Vx to a rnd number + k
        /// </summary>
        /// <param name="x">Vx</param>
        /// <param name="k">Value to add to number</param>
        public void RND_Cxkk(string x, byte k)
        {
            V[Int32.Parse(x, System.Globalization.NumberStyles.HexNumber)] = (byte) (rnd.Next(0, 255) + k);
            
        }
    }
}