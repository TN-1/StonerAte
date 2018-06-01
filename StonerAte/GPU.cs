using System.Threading;
using Eto.Forms;
using Eto.Drawing;

namespace StonerAte
{
    public class GPU
    {
        
    }

    public class MainForm : Form
    {
        private Drawable _drawable = new Drawable();
        private TextArea _textArea = new TextArea();
        private CPU _cpu;
        
        public MainForm(CPU cpu)
        {
            _cpu = cpu;
            
            Title = "StonerAte";
            ClientSize = new Size(1000,1000);
                        
            _drawable.Size = new Size(128, 64);
            _drawable.Paint += (sender, e) =>
            {
                e.Graphics.Clear(new Color(255,255,255)); 
            };

            _textArea.ReadOnly = true;
            
            /*Content = new TableLayout {
                Spacing = new Size(5, 5), // space between each cell
                Padding = new Padding(10, 10, 10, 10), // space around the table's sides
                Rows =
                {
                    new TableRow(
                        new TableCell(_drawable, true), 
                        new TableCell(_textArea, true)
                    ),
                    
                    new TableRow(
                        new Label { Text = "Some text" },
                        new TextBox { Text = "TextBox", ReadOnly = true},
                        new Label { Text = "Some text" },
                        new TextBox { Text = "TextBox", ReadOnly = true}
                    ),
                    
                    new TableRow(
                        new Label { Text = "Some text" },
                        new TextBox { Text = "TextBox", ReadOnly = true},
                        new Label { Text = "Some text" },
                        new TextBox { Text = "TextBox", ReadOnly = true}
                    ),
                    
                    // by default, the last row & column will get scaled. This adds a row at the end to take the extra space of the form.
                    // otherwise, the above row will get scaled and stretch the TextBox/ComboBox/CheckBox to fill the remaining height.
                    new TableRow { ScaleHeight = true }
                }
            }; */

            Content = new DynamicLayout
            {
                Rows =
                {
                    new DynamicRow(
                        _drawable,
                        _textArea
                    ),

                    new DynamicRow(
                        new Label {Text = "Some text"},
                        new TextBox {Text = "TextBox", ReadOnly = true},
                        new Label {Text = "Some text"},
                        new TextBox {Text = "TextBox", ReadOnly = true}
                    ),

                    new DynamicRow(
                        new Label {Text = "Some text"},
                        new TextBox {Text = "TextBox", ReadOnly = true},
                        new Label {Text = "Some text"},
                        new TextBox {Text = "TextBox", ReadOnly = true}
                    )
                }
            };
            
            Thread thread = new Thread(runCPU);
            thread.Start();
        }

        void runCPU()
        {
            var test = true;
            while(test)
                _cpu.EmulateCycle(this);
        }

        public void addText(string s)
        {
            _textArea.Append($"{s}\n");
        }
    }
}