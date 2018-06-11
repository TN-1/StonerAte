using System.Globalization;
using NUnit.Framework;

namespace StonerAte.Tests
{
    /// <summary>
    /// Tests responsible for ensuring that OpCode methods work as expected
    /// </summary>
    [TestFixture]
    public class TestsOpCodes
    {
        /// <summary>
        /// Verifies that we can jump to a different memory location
        /// </summary>
        [Test]
        public void JP_1nnn()
        {
            const string jumpTo = "250";
            var cpu = new Cpu();
            cpu.Initialize();
            
            cpu.JP_1nnn(jumpTo);
            cpu.Pc += 2;
            
            Assert.AreEqual(short.Parse(jumpTo, NumberStyles.HexNumber), cpu.Pc);
        }

        /// <summary>
        /// Verifies that we can return from a subroutine
        /// </summary>
        [Test]
        public void RET_00EE()
        {
            var cpu = new Cpu();
            cpu.Initialize();
            const short spExpect = 1;
            const short pcExpect = 0x250;
            cpu.Sp = 2;
            cpu.Stack[1] = 0x250;
            
            Assert.AreEqual(0x200, cpu.Pc);
            
            cpu.RET_00EE();

            Assert.AreEqual(spExpect, cpu.Sp);
            Assert.AreEqual(pcExpect, cpu.Pc);
        }

        /// <summary>
        /// Verifies that we can call a subroutine
        /// </summary>
        [Test]
        public void CALL_2nnn()
        {
            const string address = "300";
            var cpu = new Cpu();
            cpu.Initialize();
            
            cpu.CALL_2nnn(address);
            cpu.Pc += 2;
            
            Assert.AreEqual(1, cpu.Sp);
            Assert.AreEqual(0x200, cpu.Stack[1]);
            Assert.AreEqual(short.Parse(address, NumberStyles.HexNumber), cpu.Pc);
        }

        /// <summary>
        /// Verifies that we skip next instruction if Vx = k - Pass condition
        /// </summary>
        [Test]
        public void SE_3xkk_PASS()
        {
            var cpu = new Cpu();
            cpu.Initialize();
            cpu.V[4] = 0x010;
            
            cpu.SE_3xkk("4", "16");
            cpu.Pc += 2;

            Assert.AreEqual(0x202, cpu.Pc);
        }
        
        /// <summary>
        /// Verifies that we (dont) skip next instruction if Vx = k - Fail condition
        /// </summary>
        [Test]
        public void SE_3xkk_FAIL()
        {
            var cpu = new Cpu();
            cpu.Initialize();
            cpu.V[4] = 0x010;
            
            cpu.SE_3xkk("4", "11");
            
            Assert.AreEqual(0x200, cpu.Pc);
        }
        
        /// <summary>
        /// Verifies that we skip next instruction if Vx != k -  Pass condition
        /// </summary>
        [Test]
        public void SNE_4xkk_PASS()
        {
            var cpu = new Cpu();
            cpu.Initialize();
            cpu.V[4] = 0x010;
            
            cpu.SNE_4xkk("4", "020");
            
            Assert.AreEqual(0x202, cpu.Pc);
        }
        
        /// <summary>
        /// Verifies that we skip next instruction if Vx != k -  Fail condition
        /// </summary>
        [Test]
        public void SNE_4xkk_FAIL()
        {
            var cpu = new Cpu();
            cpu.Initialize();
            cpu.V[4] = 0x010;
            
            cpu.SNE_4xkk("4", "010");
            
            Assert.AreEqual(0x200, cpu.Pc);
        }

        /// <summary>
        /// Verifies that we skip next instruction if Vx = Vy - Pass condition
        /// </summary>
        [Test]
        public void SE_5xy0_PASS()
        {
            var cpu = new Cpu();
            cpu.Initialize();
            cpu.V[4] = 0x010;
            cpu.V[14] = 0x010;
            
            cpu.SE_5xy0("4", "E");
            
            Assert.AreEqual(0x202, cpu.Pc);
        }
        
        /// <summary>
        /// Verifies that we skip next instruction if Vx = Vy - Fail condition
        /// </summary>
        [Test]
        public void SE_5xy0_FAIL()
        {
            var cpu = new Cpu();
            cpu.Initialize();
            cpu.V[4] = 0x010;
            
            cpu.SE_5xy0("4", "6");
            
            Assert.AreEqual(0x200, cpu.Pc);
        }

        /// <summary>
        /// Verifies that we can load a byte into Vx
        /// </summary>
        [Test]
        public void LD_6xkk()
        {
            var cpu = new Cpu();
            cpu.Initialize();
            byte value = 0x069;
            
            cpu.LD_6xkk("A", value);
            
            Assert.AreEqual(value, cpu.V[10]);
        }

        /// <summary>
        /// Verifies that we can add a byte into Vx
        /// </summary>
        [Test]
        public void ADD_7xkk()
        {
            var cpu = new Cpu();
            cpu.Initialize();
            cpu.V[2] = 0x020;
            byte value = 0x010;
            
            cpu.ADD_7xkk("2", value);
            
            Assert.AreEqual(0x030, cpu.V[2]);
        }

        /// <summary>
        /// Verifies that we can copy a value from Vx to Vy
        /// </summary>
        [Test]
        public void LD_8xy0()
        {
            var cpu = new Cpu();
            cpu.Initialize();
            cpu.V[2] = 0x069;
            
            cpu.LD_8xy0("2", "D");
            
            Assert.AreEqual(0x069, cpu.V[2]);
            Assert.AreEqual(0x069, cpu.V[13]);
        }

        /// <summary>
        /// Verifies that we can perform an OR operation on two registers
        /// </summary>
        [Test]
        public void OR_8xy1()
        {
            var cpu = new Cpu();
            cpu.Initialize();
            cpu.V[2] = 0x001;
            cpu.V[4] = 0x010;
            
            cpu.OR_8xy1("2", "4");
            
            Assert.AreEqual(0x011, cpu.V[2]);
        }

        /// <summary>
        /// Verifies that we can perform an AND operation on two registers
        /// </summary>
        [Test]
        public void AND_8xy2_FAIL()
        {
            var cpu = new Cpu();
            cpu.Initialize();
            cpu.V[2] = 0x001;
            cpu.V[4] = 0x010;
            
            cpu.AND_8xy2("2", "4");
            
            Assert.AreEqual(0x000, cpu.V[2]);
        }

        [Test]
        public void AND_8xy2_PASS()
        {
            var cpu = new Cpu();
            cpu.Initialize();
            cpu.V[2] = 0x001;
            cpu.V[4] = 0x001;
            
            cpu.AND_8xy2("2", "4");
            
            Assert.AreEqual(0x001, cpu.V[2]);
        }
        
        /// <summary>
        /// Verifies that we can perform an AND operation on two registers
        /// </summary>
        [Test]
        public void XOR_8xy3()
        {
            var cpu = new Cpu();
            cpu.Initialize();
            cpu.V[2] = 0x001;
            cpu.V[4] = 0x010;
            
            cpu.XOR_8xy3("2", "4");
            
            Assert.AreEqual(0x011, cpu.V[2]);
        }

        /// <summary>
        /// Verifies that we can add the values of two registers together
        /// </summary>
        [Test]
        public void ADD_8xy4()
        {
            var cpu = new Cpu();
            cpu.Initialize();
            cpu.V[1] = 0x001;
            cpu.V[2] = 0x001;
            
            cpu.ADD_8xy4("1", "2");
            
            Assert.AreEqual(0x002, cpu.V[1]);
        }
        
        [Test]
        public void ADD_8xy4_CARRY()
        {
            var cpu = new Cpu();
            cpu.Initialize();
            cpu.V[1] = 0x0DD;
            cpu.V[2] = 0x0DD;
            
            cpu.ADD_8xy4("1", "2");
            
            Assert.AreEqual(0x0BA, cpu.V[1]);
            Assert.AreEqual(0x001, cpu.V[15]);
        }
        
        /// <summary>
        /// Verifies that we can subtract the values of two registers
        /// </summary>
        [Test]
        public void SUB_8xy5()
        {
            var cpu = new Cpu();
            cpu.Initialize();
            cpu.V[1] = 0x001;
            cpu.V[2] = 0x001;
            
            cpu.SUB_8xy5("1", "2");
            
            Assert.AreEqual(0x000, cpu.V[1]);
        }
        
        [Test]
        public void SUB_8xy5_BORROW()
        {
            var cpu = new Cpu();
            cpu.Initialize();
            cpu.V[1] = 0x002;
            cpu.V[2] = 0x001;
            
            cpu.SUB_8xy5("1", "2");
            
            Assert.AreEqual(0x001, cpu.V[1]);
            Assert.AreEqual(0x001, cpu.V[15]);
        }

        [Test]
        public void SUB_8xy7()
        {
            var cpu = new Cpu();
            cpu.Initialize();
            cpu.V[1] = 0x001;
            cpu.V[2] = 0x001;
            
            cpu.SUBN_8xy7("1", "2");
            
            Assert.AreEqual(0x000, cpu.V[1]);
        }
        
        [Test]
        public void SUB_8xy7_BORROW()
        {
            var cpu = new Cpu();
            cpu.Initialize();
            cpu.V[1] = 0x001;
            cpu.V[2] = 0x002;
            
            cpu.SUBN_8xy7("1", "2");
            
            Assert.AreEqual(0x001, cpu.V[1]);
            Assert.AreEqual(0x001, cpu.V[15]);
        }        
        
        /// <summary>
        /// Verifies that we can shift right
        /// </summary>
        [Test]
        public void SHR_8xy6()
        {
            var cpu = new Cpu();
            cpu.Initialize();
            cpu.V[1] = 0x011;
            
            cpu.SHR_8xy6("0", "1");
            
            Assert.AreEqual(0x008, cpu.V[0]);
            Assert.AreEqual(0x001, cpu.V[15]);
        }
        
        /// <summary>
        /// Verifies that we can shift left
        /// </summary>
        [Test]
        public void SHL_8xyE()
        {
            var cpu = new Cpu();
            cpu.Initialize();
            cpu.V[1] = 0x001;
            
            cpu.SHL_8xyE("0", "1");
            
            Assert.AreEqual(0x002, cpu.V[0]);
            Assert.AreEqual(0x001, cpu.V[15]);
        }

        /// <summary>
        /// Verifies that we can skip next instruction if two registers are not equal
        /// </summary>
        [Test]
        public void SNE_9xy0()
        {
            var cpu = new Cpu();
            cpu.Initialize();
            cpu.V[1] = 0x001;
            
            cpu.SNE_9xy0("0", "1");
            
            Assert.AreEqual(0x202, cpu.Pc);
        }

        /// <summary>
        /// Verfiies that we can load a value into register I
        /// </summary>
        [Test]
        public void LD_Annn()
        {
            var cpu = new Cpu();
            cpu.Initialize();
            byte value = 0x069;
            
            cpu.LD_Annn(value);
            
            Assert.AreEqual(0x069, cpu.I);
        }

        /// <summary>
        /// Verifies that we can jump to a certain address + register value
        /// </summary>
        [Test]
        public void JP_Bnnn()
        {
            var cpu = new Cpu();
            cpu.Initialize();
            cpu.V[0] = 0x020;
            short n = 0x300;
            
            cpu.JP_Bnnn(n);
            
            Assert.AreEqual(0x320, cpu.Pc);
        }
        
        
    }
}