using NUnit.Framework;

namespace StonerAte.Tests
{
    [TestFixture]
    public class Tests_CPU
    {
        /// <summary>
        /// Verifies that a rom is loaded into RAM correctly
        /// </summary>
        [Test]
        public void LoadRom()
        {
            CPU cpu = new CPU();
            cpu.initialize();
            cpu.LoadRom("Chip8 Picture");

            for (var i = 0; i < 512; i++)
            {
                Assert.AreEqual(0x000, cpu.memory[i]);
            }

            for (int i = 0; i < cpu.romBytes.Length; i++)
            {                                                   
                Assert.AreEqual(cpu.romBytes[i], cpu.memory[i + 512]);
            }
        }

    }
}