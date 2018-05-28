using System;

namespace StonerAte
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CPU cpu = new CPU();
            
            cpu.initialize();
            cpu.LoadRom("Chip8 Picture");
            Console.WriteLine("Init complete");
        }
    }
}