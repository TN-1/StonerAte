using System;
using Gtk;

namespace StonerAte
{
    public class GPU
    {
        public static void init()
        {
            Application.Init();
 
            //Create the Window
            Window main = new Window("StonerAte");
            main.DefaultSize = new Gdk.Size(600,600);
            main.Resizable = false;
             
            Table mainTable = new Table(2, 2, false);
            Table statusTable = new Table(36, 2, false);
            DrawingArea drawingArea = new DrawingArea();
            ScrolledWindow scrolledWindow = new ScrolledWindow();
            TextView textView = new TextView();
            scrolledWindow.Add(textView);
            TextView[] TV = new TextView[36];
            ScrolledWindow[] window = new ScrolledWindow[36];
            for (int i = 0; i < 36; i++)
            {
                TV[i] = new TextView();
                window[i] = new ScrolledWindow();
                window[i].Add(TV[i]);
                statusTable.Attach(window[i], 1, 2 , Convert.ToUInt32(i), 1 + Convert.ToUInt32(i));
            }
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

            mainTable.Attach(statusTable, 1, 2, 0, 2);
            mainTable.Attach(drawingArea, 0, 1, 0, 1);
            mainTable.Attach(scrolledWindow, 0, 1, 1, 2);
            
            main.Add(mainTable);
            
            //Show Everything
            main.ShowAll();
 
            Application.Run();
        }
    }
}