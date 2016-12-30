// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpeflowStyleTablePrinter.cs" company="Obscureware Solutions">
// MIT License
//
// Copyright(c) 2016-2017 Sebastian Gruchacz
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// </copyright>
// <summary>
//   Defines the SpeflowStyleTablePrinter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Obscureware.Console.Operations.Tables
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ObscureWare.Console;
    using Styles;

    public class SpeflowStyleTablePrinter : DataTablePrinter
    {
        private readonly TableStyle _tableStyle;

        public SpeflowStyleTablePrinter(IConsole console, TableStyle tableStyle) : base(console)
        {
            this._tableStyle = tableStyle;
        }

        protected override int ExternalFrameThickness { get; } = 2;

        protected override void RenderTable(ColumnInfo[] columns, IEnumerable<string[]> rows)
        {
            int index = 0;
            string formatter = "|" + string.Join("|", columns.Select(col => $"{{{index++},{col.CurrentLength * (int)col.Alignment}}}")) + "|";

            this.Console.WriteLine(this._tableStyle.HeaderColor, string.Format(formatter, columns.Select(col => col.Header.Substring(0, Math.Min(col.Header.Length, col.CurrentLength))).ToArray()));
            // TODO: WriteText + this.Console.AdvanceLine();

            // TODO: add different coloring to the frame

            index = 0;
            foreach (string[] row in rows)
            {
                switch (this._tableStyle.OverflowBehaviour)
                {
                    case TableOverflowContentBehavior.Ellipsis:
                        {
                            string[] result = new string[columns.Length];
                            for (int i = 0; i < columns.Length; i++)
                            {
                                // taking care for asymmetric array, btw
                                if (row.Length > i)
                                {
                                    if (row[i].Length <= columns[i].CurrentLength)
                                    {
                                        result[i] = row[i];
                                    }
                                    else
                                    {
                                        result[i] = row[i].Substring(0, columns[i].CurrentLength);
                                    }
                                }
                            }

                            this.Console.WriteLine(
                                (index % 2 == 0 ) ? this._tableStyle.EvenRowColor : this._tableStyle.OddRowColor,
                                string.Format(formatter, result));
                            break;
                        }
                    case TableOverflowContentBehavior.Wrap:
                        {
                            throw new NotImplementedException();
                            break;
                        }
                    default:
                        {
                            throw new ArgumentOutOfRangeException(nameof(TableOverflowContentBehavior));
                        }
                }

                index++;
            }


            // TODO: implement and use Console.BatchPrint()! - atomic operation
        }
    }
}