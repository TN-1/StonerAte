using System;
using System.Threading;

namespace StonerAte
{
    public class GPU
    {
        private static Label[] _tv = new Label[36];
        private static CPU cpu;
        private static Window main;
        private static TextBuffer text;
        private static DrawingArea drawingArea;
        
        public static void init(CPU _cpu)
        {
            Application.Init();
            cpu = _cpu;
            //Create the Window
            
            main = new Window("StonerAte");
            main.DefaultSize = new Gdk.Size(600,600);
            main.Resizable = false;
            
            //TODO: FIX THIS!!!
            main.DestroyEvent += delegate { Application.Quit(); };
             
            Table mainTable = new Table(2, 2, false);
            Table statusTable = new Table(36, 2, false);
            drawingArea = new DrawingArea();        
            drawingArea.SetSizeRequest(128,64);
            ;

            ScrolledWindow scrolledWindow = new ScrolledWindow();
            text = new TextBuffer(null);
            TextView textView = new TextView();
            textView.Buffer = text;
            scrolledWindow.Add(textView);
            for (int i = 0; i < 36; i++)
            {
                _tv[i] = new Label();
                statusTable.Attach(_tv[i], 1, 2 , Convert.ToUInt32(i), 1 + Convert.ToUInt32(i));
            }
            Label labelPC = new Label("PC: ");
            Label labelSP = new Label("SP: ");
            Label labelI = new Label("I: ");
            Label labelOpcode = new Label("Opcode: ");
            Label labelV0 = new Label("V0: ");
            Label labelV1 = new Label("V1: ");
            Label labelV2 = new Label("V2: ");
            Label labelV3 = new Label("V3: ");
            Label labelV4 = new Label("V4: ");
            Label labelV5 = new Label("V5: ");
            Label labelV6 = new Label("V6: ");
            Label labelV7 = new Label("V7: ");
            Label labelV8 = new Label("V8: ");
            Label labelV9 = new Label("V9: ");
            Label labelVA = new Label("VA: ");
            Label labelVB = new Label("VB: ");
            Label labelVC = new Label("VC: ");
            Label labelVD = new Label("VD: ");
            Label labelVE = new Label("VE: ");
            Label labelVF = new Label("VF: ");

            statusTable.Attach(labelV0, 0, 1, 0, 1);
            statusTable.Attach(labelV1, 0, 1, 1, 2);
            statusTable.Attach(labelV2, 0, 1, 2, 3);
            statusTable.Attach(labelV3, 0, 1, 3, 4);
            statusTable.Attach(labelV4, 0, 1, 4, 5);
            statusTable.Attach(labelV5, 0, 1, 5, 6);
            statusTable.Attach(labelV6, 0, 1, 6, 7);
            statusTable.Attach(labelV7, 0, 1, 7, 8);
            statusTable.Attach(labelV8, 0, 1, 8, 9);
            statusTable.Attach(labelV9, 0, 1, 9, 10);
            statusTable.Attach(labelVA, 0, 1, 10, 11);
            statusTable.Attach(labelVB, 0, 1, 11, 12);
            statusTable.Attach(labelVC, 0, 1, 12, 13);
            statusTable.Attach(labelVD, 0, 1, 13, 14);
            statusTable.Attach(labelVE, 0, 1, 14, 15);
            statusTable.Attach(labelVF, 0, 1, 15, 16);
            statusTable.Attach(labelPC, 0, 1, 17, 18);
            statusTable.Attach(labelSP, 0, 1, 18, 19);
            statusTable.Attach(labelI, 0, 1, 19, 20);
            statusTable.Attach(labelOpcode, 0, 1, 20, 21);

            mainTable.Attach(statusTable, 1, 2, 0, 2);
            mainTable.Attach(drawingArea, 0, 1, 0, 1);
            mainTable.Attach(scrolledWindow, 0, 1, 1, 2);
            
            main.Add(mainTable);
            
            //Show Everything
            main.ShowAll();

            Thread thread = new Thread(runCPU);
            thread.Start();
                Application.Run();
        }

        static public void runCPU()
        {
            var meh = true;
            while (meh)
            {
                try
                {
                    cpu.EmulateCycle();
                }
                catch(Exception e)
                {
                    meh = false;
                    text.Text = $"{text.Text}ERROR: {e.Message}\n";
                }
            }
        }

        public void AddText(string s)
        {
            text.Text = $"{text.Text}{s}\n";
        }
        
        public void Update(CPU cpu)
        {
            for (int i = 0; i < cpu.V.Length; i++)
            {
                _tv[i].Text = cpu.V[i].ToString("X4");
            }

            _tv[17].Text = cpu.pc.ToString("X4");
            _tv[18].Text = cpu.sp.ToString("X4");
            _tv[19].Text = cpu.I.ToString("X4");
            _tv[20].Text = cpu.opcode;
        }
    }
}