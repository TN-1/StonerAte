using System;

namespace StonerAte
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CPU _cpu = new CPU();
            
            _cpu.initialize();
            _cpu.LoadRom("Chip8 Picture");
            Console.WriteLine("Init complete");
        }
    }
}