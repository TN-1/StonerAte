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
            var cpu = new Cpu();
            
            cpu.Initialize();
            Console.WriteLine("Init complete");
            new Application().Run(new MainForm(cpu, 10));
        }
    }
}