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
