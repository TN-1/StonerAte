using System;
using Eto.Forms;

namespace StonerAte
{
    public static class Program
    {
        /// <summary>
        /// Entry point for the emulator. Not much is done here expect pass off to smarter places
        /// </summary>
        public static void Main()
        {
            Cpu cpu = new Cpu();
            
            cpu.Initialize();
            cpu.LoadRom("TICTAC");
            Console.WriteLine("Init complete");
            new Application().Run(new MainForm(cpu, 10));
        }
    }
}