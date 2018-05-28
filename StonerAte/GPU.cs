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
            Table statusTable = new Table(36, 1, false);
            DrawingArea drawingArea = new DrawingArea();
            ScrolledWindow scrolledWindow = new ScrolledWindow();
            TextView textView = new TextView();
            scrolledWindow.Add(textView);
            Label[] label = new Label[36];
            for (int i = 0; i < 36; i++)
            {
                label[i] = new Label();
                label[i].Text = $"V{i}";
                statusTable.Attach(label[i], 0, 1 , Convert.ToUInt32(i), 1 + Convert.ToUInt32(i));
            }
            
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