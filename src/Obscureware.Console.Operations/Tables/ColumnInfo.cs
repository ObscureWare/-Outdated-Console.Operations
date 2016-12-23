namespace Obscureware.Console.Operations.Tables
{
    using System;

    public class ColumnInfo
    {
        public ColumnInfo(string header, ColumnAlignment alignment = ColumnAlignment.Left)
        {
            if (string.IsNullOrWhiteSpace(header))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(header));
            }

            this.Header = header;
            this.Alignment = alignment;
            this.CurrentLength = header.Length;
        }

        public string Header { get; private set; }

        public int MinLength { get; set; }

        public int CurrentLength { get; internal set; }

        public ColumnAlignment Alignment { get; set; }
    }
}