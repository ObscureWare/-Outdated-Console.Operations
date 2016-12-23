namespace Obscureware.Console.Operations
{
    using ObscureWare.Console;

    public class TableStyle
    {
        internal enum TablePiece : byte
        {
            TopLeft = 0,
            Top,
            TopRight,
            Left,
            Right,
            BottomLeft,
            Bottom,
            BottomRight,
            HeaderSeparatorWithFrame,
            HeaderSeparatorWithoutFrame,
            ColumnsSeparator,
            TopConnector,
            BottomConnector
        }

        private readonly char[] _frameChars;

        public TableStyle(ConsoleFontColor frameColor, ConsoleFontColor headerColor, ConsoleFontColor oddRowColor, ConsoleFontColor evenRowColor,
            string frameChars, char backgroundFiller, TableLargeRowContentBehavior behaviour)
        {
            this.FrameColor = frameColor;
            this.HeaderColor = headerColor;
            this.OddRowColor = oddRowColor;
            this.EvenRowColor = evenRowColor;
            this.BackgroundFiller = backgroundFiller;
            this.Behaviour = behaviour;
            this._frameChars = frameChars.ToCharArray();
        }

        public ConsoleFontColor FrameColor { get; private set; }
        public ConsoleFontColor HeaderColor { get; private set; }
        public ConsoleFontColor OddRowColor { get; private set; }
        public ConsoleFontColor EvenRowColor { get; private set; }

        public char BackgroundFiller { get; private set; }
        public TableLargeRowContentBehavior Behaviour { get; private set; }

        public char TopLeft => this._frameChars[(byte)TablePiece.TopLeft];

        public char Top => this._frameChars[(byte)TablePiece.Top];

        public char TopRight => this._frameChars[(byte)TablePiece.TopRight];

        public char Left => this._frameChars[(byte)TablePiece.Left];

        public char Right => this._frameChars[(byte)TablePiece.Right];

        public char BottomLeft => this._frameChars[(byte)TablePiece.BottomLeft];

        public char Bottom => this._frameChars[(byte)TablePiece.Bottom];

        public char BottomRight => this._frameChars[(byte)TablePiece.BottomRight];

        public char HeaderSeparatorFrame => this._frameChars[(byte)TablePiece.HeaderSeparatorWithFrame];

        public char HeaderSeparatorCell => this._frameChars[(byte)TablePiece.HeaderSeparatorWithoutFrame];

        public char ColumnSeparator => this._frameChars[(byte)TablePiece.ColumnsSeparator];

        public char TopConnector => this._frameChars[(byte)TablePiece.TopConnector];

        public char BottomConnector => this._frameChars[(byte)TablePiece.BottomConnector];
    }

    /// <summary>
    /// Specifies how large content of table rows will be treated
    /// </summary>
    public enum TableLargeRowContentBehavior
    {
        /// <summary>
        /// Cut to fit with ellipsis
        /// </summary>
        Ellipsis,

        /// <summary>
        /// Multi-lined
        /// </summary>
        Wrap // TODO:
    }
}