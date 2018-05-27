using System;

namespace StonerAte
{
    public partial class CPU
    {
        /// <summary>
        /// Clears the graphics display
        /// </summary>
        private void CLS_00E00()
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
        /// <param name="n"><Memory address of subroutine to call/param>
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
    }
}