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
    public class CPU
    {
        byte[] memory = new byte[4096];
        byte[] V = new byte[16];
        private byte I;
        private short pc;
        private string opcode;
        byte[] stack = new byte[16];
        private short sp;
        
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
            
            //TODO: Load fontset
        }

        public void LoadRom(string name)
        {
            var romBytes = File.ReadAllBytes($"{Environment.CurrentDirectory}/roms/{name}.ch8");
            for (var i = 0; i < romBytes.Length; i++)
            {
                //i + 0x200 per mem map
                memory[i + 512] = romBytes[i];
            }

        }

        public void ProgramLoop()
        {
            opcode = memory[pc].ToString("X2") + memory[pc + 1].ToString("X2");

            pc = (short) (pc + 2);
        }
    }
}