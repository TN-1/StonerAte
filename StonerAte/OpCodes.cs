using System;

namespace StonerAte
{
    public partial class CPU
    {
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

        public void RET_00EE()
        {
            pc = stack[sp];
            if(sp != 0)
                sp--;
        }

        public void JP_1nnn(string n)
        {
            pc = Convert.ToInt16(n);
        }

        public void CALL_2nnn(string n)
        {
            stack[sp] = pc;
            pc = Convert.ToInt16(n);
            sp++;
        }

        public void SE_3xkk(string x, string k)
        {
            if (V[Int32.Parse(x, System.Globalization.NumberStyles.HexNumber)] == Convert.ToByte(k))
                pc = (short)(pc + 2);
        }

        public void SNE_4xkk(string x, string k)
        {
            if (V[Int32.Parse(x, System.Globalization.NumberStyles.HexNumber)] != Convert.ToByte(k))
                pc = (short)(pc + 2);
        }

        public void SE_5xy0(string x, string y)
        {
            if (V[Int32.Parse(x, System.Globalization.NumberStyles.HexNumber)] == V[Int32.Parse(y, System.Globalization.NumberStyles.HexNumber)])
                pc = (short)(pc + 2);
        }
    }
}