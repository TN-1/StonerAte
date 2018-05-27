using NUnit.Framework;

namespace StonerAte.Tests
{
    [TestFixture]
    public class Tests
    {
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
        
        [Test]
        public void JP_1nnn()
        {
            short jump_to = 0x250;
            CPU cpu = new CPU();
            cpu.initialize();
            
            cpu.JP_1nnn(jump_to.ToString());
            
            Assert.AreEqual(jump_to, cpu.pc);
        }

        [Test]
        public void RET_00EE()
        {
            CPU cpu = new CPU();
            cpu.initialize();
            short sp_expect = 0;
            short pc_expect = 0x250;
            cpu.sp = 1;
            cpu.stack[1] = 0x250;
            
            Assert.AreEqual(0x200, cpu.pc);
            
            cpu.RET_00EE();
            
            Assert.AreEqual(sp_expect, cpu.sp);
            Assert.AreEqual(pc_expect, cpu.pc);
        }

        [Test]
        public void CALL_2nnn()
        {
            CPU cpu = new CPU();
            cpu.initialize();
            short address = 0x300;
            
            cpu.CALL_2nnn(address.ToString());
            
            Assert.AreEqual(1, cpu.sp);
            Assert.AreEqual(0x200, cpu.stack[0]);
            Assert.AreEqual(address, cpu.pc);
        }

        [Test]
        public void SE_3xkk_PASS()
        {
            CPU cpu = new CPU();
            cpu.initialize();
            cpu.V[4] = 0x010;
            
            cpu.SE_3xkk("4", "16");
            
            Assert.AreEqual(0x202, cpu.pc);
        }
        
        [Test]
        public void SE_3xkk_FAIL()
        {
            CPU cpu = new CPU();
            cpu.initialize();
            cpu.V[4] = 0x010;
            
            cpu.SE_3xkk("4", "11");
            
            Assert.AreEqual(0x200, cpu.pc);
        }
        
        [Test]
        public void SNE_4xkk_PASS()
        {
            CPU cpu = new CPU();
            cpu.initialize();
            cpu.V[4] = 0x010;
            
            cpu.SNE_4xkk("4", "11");
            
            Assert.AreEqual(0x202, cpu.pc);
        }
        
        [Test]
        public void SNE_4xkk_FAIL()
        {
            CPU cpu = new CPU();
            cpu.initialize();
            cpu.V[4] = 0x010;
            
            cpu.SNE_4xkk("4", "16");
            
            Assert.AreEqual(0x200, cpu.pc);
        }

        [Test]
        public void SE_5xy0_PASS()
        {
            CPU cpu = new CPU();
            cpu.initialize();
            cpu.V[4] = 0x010;
            cpu.V[14] = 0x010;
            
            cpu.SE_5xy0("4", "E");
            
            Assert.AreEqual(0x202, cpu.pc);
        }
        
        [Test]
        public void SE_5xy0_FAIL()
        {
            CPU cpu = new CPU();
            cpu.initialize();
            cpu.V[4] = 0x010;
            
            cpu.SE_5xy0("4", "6");
            
            Assert.AreEqual(0x200, cpu.pc);
        }

    }
}