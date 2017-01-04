// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICoreTableStyle.cs" company="Obscureware Solutions">
// MIT License
//
// Copyright(c) 2016 Sebastian Gruchacz
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
//   Defines the ICoreTableStyle interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ObscureWare.Console.Operations.Styles
{
    using ObscureWare.Console;
    
    public interface ICoreTableStyle
    {
        ConsoleFontColor HeaderColor { get; }

        ConsoleFontColor RowColor { get; }

        ConsoleFontColor EvenRowColor { get; }

        /// <summary>
        /// Gets whether table printer shall display column header(s)
        /// </summary>
        bool ShowHeader { get; }

        /// <summary>
        /// Gets whether table printing shall be done in atomic operation (might be considerably slower) or not.
        /// This might not be required in some scenarios, so better one have choice.
        /// </summary>
        bool AtomicPrinting { get; }

        /// <summary>
        /// Specifies how overflowing cell values shall be treated - clipped or splitted.
        /// </summary>
        TableOverflowContentBehavior OverflowBehaviour { get; }
    }
}