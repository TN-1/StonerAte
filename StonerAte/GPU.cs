/*
StonerAte - A Chip 8 Emulator
Copyright (C) 2018 Hamish West, hamish@hamishwest.xyz, github.com/TN-1

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Threading;
using Eto.Forms;
using Eto.Drawing;

namespace StonerAte
{
    /// <summary>
    /// Represents the main form and the methods responsible for graphics work
    /// </summary>
    public class MainForm : Form
    {
        private Drawable _drawable = new Drawable();
        private TextArea _textArea = new TextArea();
        private int _taLines = 0;
        private Cpu _cpu;
        private Label[] _regLabels = new Label[16];
        private Label[] _basicsLabels = new Label[5];
        private TextBox[] _basicsBoxs = new TextBox[5];
        private TextBox[] _regBoxs = new TextBox[16];

        public MainForm(Cpu cpu, int scale)
        {
            _cpu = cpu;
            var scaleFactor = scale;
            
            Title = "StonerAte";
            ClientSize = new Size(1000, 1000);

            _drawable.Size = new Size(64 * scaleFactor, 32 * scaleFactor);
            _drawable.Paint += (sender, e) =>
            {
                e.Graphics.Clear(new Color(255, 255, 255));
                
                for (var x = 0; x < _cpu.Gfx.GetLength(0); x++)
                {
                    for (var y = 0; y < _cpu.Gfx.GetLength(1); y++)
                    {
                        if (_cpu.Gfx[x, y] == 1)
                        {    
                            e.Graphics.FillRectangle(new Color(0,0,0), x * scaleFactor, y * scaleFactor, scaleFactor,
                                scaleFactor);
                        }
                    }
                }
            };

            _textArea.ReadOnly = true;
            _textArea.Size = new Size(1000 - ((64 * scaleFactor) + 30), 32 * scaleFactor);
            
            var basicsLayout = new DynamicLayout();
            basicsLayout.BeginVertical();
            _basicsLabels[0] = new Label {Text = "Opcode:"};
            _basicsBoxs[0] = new TextBox {ReadOnly = true};
            _basicsLabels[1] = new Label {Text = "I:"};
            _basicsBoxs[1] = new TextBox {ReadOnly = true};
            _basicsLabels[2] = new Label {Text = "PC:"};
            _basicsBoxs[2] = new TextBox {ReadOnly = true};
            _basicsLabels[3] = new Label {Text = "SP:"};
            _basicsBoxs[3] = new TextBox {ReadOnly = true};
            _basicsLabels[4] = new Label {Text = "DrawFlag:"};
            _basicsBoxs[4] = new TextBox {ReadOnly = true};
            for (int i = 0; i < 5; i++)
            {
                basicsLayout.BeginHorizontal();
                basicsLayout.Add(_basicsLabels[i]);
                basicsLayout.Add(_basicsBoxs[i]);
                basicsLayout.EndHorizontal();
            }

            basicsLayout.EndVertical();
            var basicsBox = new GroupBox
            {
                Text = "Basics",
                Content = basicsLayout
            };

            var regLayout = new DynamicLayout();
            regLayout.BeginVertical();
            for (int i = 0; i < _regLabels.Length; i++)
            {
                _regBoxs[i] = new TextBox {ReadOnly = true};
                _regLabels[i] = new Label {Text = $"V[{i:X}]"};
                regLayout.BeginHorizontal();
                regLayout.Add(_regLabels[i]);
                regLayout.Add(_regBoxs[i]);
                regLayout.EndBeginHorizontal();
            }

            regLayout.EndVertical();
            var registerBox = new GroupBox
            {
                Text = "Registers",
                Content = regLayout
            };

            var layout = new PixelLayout();
            layout.Add(_drawable, 10, 10);
            layout.Add(_textArea, (64 * scaleFactor) + 20, 10);
            layout.Add(basicsBox, 10, (32 * scaleFactor) + 20);
            layout.Add(registerBox, 250, (32 * scaleFactor) + 20);

            Content = layout;
            
            LoadComplete += (sender, args) =>
            {
                var thread = new Thread(RunCpu);
                thread.Start();
            };
        }

        private void RunCpu()
        {
            var isRunning = true;
            while (isRunning)
            {                
                try
                {
                    _cpu.EmulateCycle(this);
                    Update();
                    if (!_cpu.DrawFlag)
                        continue;
                    _drawable.Invalidate();
                    _cpu.DrawFlag = false;
                }
                catch (Exception e)
                {
                    isRunning = false;
                    AddText(e.ToString());
                }
            }
        }

        public void AddText(string s)
        {
            if (_taLines == 100)
            {
                _textArea.Text = "";
                _taLines = 0;
            }
            _textArea.Append($"{s}\n");
            _taLines++;
        }

        private void Update()
        {
            _basicsBoxs[0].Text = _cpu.Opcode;
            _basicsBoxs[1].Text = $"{_cpu.I:X4}";
            _basicsBoxs[2].Text = $"{_cpu.Pc:X4}";
            _basicsBoxs[3].Text = $"{_cpu.Sp:X4}";
            _basicsBoxs[4].Text = $"{_cpu.DrawFlag}";

            for (var i = 0; i < _regBoxs.Length; i++)
            {
                _regBoxs[i].Text = $"{_cpu.V[i]:X4}";
            }
        }
    }
}