// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataTable.cs" company="Obscureware Solutions">
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
//   Defines the DataTable class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Obscureware.Console.Operations.Tables
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DataTable<T>
    {
        private readonly Dictionary<T, string[]> _data = new Dictionary<T, string[]>();
        private readonly Dictionary<string, ColumnInfo> _columns;

        public DataTable(params ColumnInfo[] columns)
        {
            this._columns = columns.ToDictionary(c => c.Header, c => c);
        }

        public Dictionary<string, ColumnInfo> Columns
        {
            get
            {
                return this._columns;
            }
        }

        public void AddRow(T src, string[] rowValues)
        {
            this._data.Add(src, rowValues);
        }

        public IEnumerable<string[]> GetRows()
        {
            return this._data.Values;
        }

        /// <summary>
        /// Finds first value that is identified by value stored in the first column or NULL
        /// </summary>
        /// <param name="aIdentifier"></param>
        /// <returns></returns>
        public T GetUnderlyingValue(string aIdentifier)
        {
            return this._data.FirstOrDefault(pair => pair.Value.First().Equals(aIdentifier, StringComparison.InvariantCultureIgnoreCase)).Key;
        }

        /// <summary>
        /// Finds first value that matches given predicate filtering function or NULL
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="matchingFunc"></param>
        /// <returns></returns>
        public T FindValueWhere(string identifier, Func<T, string, bool> matchingFunc)
        {
            return this._data.FirstOrDefault(pair => matchingFunc(pair.Key, identifier)).Key;
        }
    }
}
