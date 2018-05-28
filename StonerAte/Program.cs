using System;

namespace StonerAte
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CPU cpu = new CPU();
            GPU gpu = new GPU();
            
            cpu.initialize(gpu);
            cpu.LoadRom("Chip8 Picture");
            Console.WriteLine("Init complete");
            var test = true;
            while (test)
            {
                GPU.init();
            }
        }
    }
}