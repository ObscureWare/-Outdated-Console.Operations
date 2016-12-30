﻿// --------------------------------------------------------------------------------------------------------------------
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
    using Shared;

    /// <summary>
    /// DataTable object to store object together with its tabelaric representation
    /// </summary>
    /// <typeparam name="T">Underlying object</typeparam>
    public class DataTable<T>
    {
        // TODO: probably shall implement INotifyChange on T and require rendering function callback for updates
        // For now - assuming that DataTable just stores static data

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
        /// <param name="aIdentifier">Artificial Idx identifier to be used for searching in the first column</param>
        /// <returns></returns>
        public T GetUnderlyingValue(string aIdentifier)
        {
            return this._data.FirstOrDefault(pair => pair.Value.First().Equals(aIdentifier, StringComparison.InvariantCultureIgnoreCase)).Key;
        }

        /// <summary>
        /// Finds first value that matches given predicate filtering function or NULL
        /// </summary>
        /// <param name="identifier">Identifier to be found in stored objects</param>
        /// <param name="matchingFunc">Matching function. When it returns true for given object, that row will be returned.</param>
        /// <returns>First found matching object or NULL</returns>
        public T FindValueWhere(string identifier, Func<T, string, bool> matchingFunc)
        {
            return this._data.FirstOrDefault(pair => matchingFunc(pair.Key, identifier)).Key;
        }


        /// <summary>
        /// Builds indexed table, that provides extra Idx column to be used with cached results
        /// </summary>
        /// <typeparam name="TKey">Type of data stored in the table</typeparam>
        /// <param name="header">Column headers</param>
        /// <param name="dataSource">Rows data source</param>
        /// <param name="dataGenerator">Row generating callback function</param>
        /// <returns>Artificial table object</returns>
        public static DataTable<TKey> BuildIndexedTable<TKey>(string[] header, IEnumerable<TKey> dataSource, Func<TKey, string[]> dataGenerator)
        {
            DataTable<TKey> table = new DataTable<TKey>(
                new[] { "Idx" }.Concat(header).Select(head => new ColumnInfo(head)).ToArray());

            uint i = 1;
            foreach (TKey src in dataSource)
            {
                // TODO: verify expected size of the array matches header count => #InvalidOperationException
                table.AddRow(src, new[] { i.ToAlphaEnum() + '.' }.Concat(dataGenerator.Invoke(src)).ToArray());
                i++;
            }

            return table;
        }
    }
}
