using NUnit.Framework;

namespace StonerAte.Tests
{
    /// <summary>
    /// Tests to ensure that the CPU core works as designed
    /// </summary>
    [TestFixture]
    public class TestsCpu
    {
        /// <summary>
        /// Verifies that a rom is loaded into RAM correctly
        /// </summary>
        [Test]
        public void LoadRom()
        {
            Cpu cpu = new Cpu();
            cpu.Initialize();
            cpu.LoadRom("Chip-8 Pack/Chip-8 Programs/Chip8 Picture.ch8");

            //TODO: Account for fontset in this test
            for (var i = 100; i < 512; i++)
            {
                Assert.AreEqual(0x000, cpu.Memory[i]);
            }

            for (int i = 0; i < cpu.RomBytes.Length; i++)
            {                                                   
                Assert.AreEqual(cpu.RomBytes[i], cpu.Memory[i + 512]);
            }
        }

    }
}