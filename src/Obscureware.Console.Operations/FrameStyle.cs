namespace Obscureware.Console.Operations
{
    using ObscureWare.Console;

    /// <summary>
    /// Definition of on-screen frame
    /// </summary>
    public class FrameStyle
    {
        internal enum FramePiece : byte
        {
            TopLeft = 0,
            Top,
            TopRight,
            Left,
            Right,
            BottomLeft,
            Bottom,
            BottomRight
        }

        private readonly char[] _frameChars;

        public FrameStyle(ConsoleFontColor frameColor, ConsoleFontColor textColor, string frameChars, char backgroundFiller)
        {
            this.FrameColor = frameColor;
            this.TextColor = textColor;
            this.BackgroundFiller = backgroundFiller;
            this._frameChars = frameChars.ToCharArray();
        }

        public ConsoleFontColor FrameColor { get; private set; }

        public ConsoleFontColor TextColor { get; private set; }

        public char BackgroundFiller { get; private set; }

        public char TopLeft => this._frameChars[(byte)FramePiece.TopLeft];

        public char Top => this._frameChars[(byte)FramePiece.Top];

        public char TopRight => this._frameChars[(byte)FramePiece.TopRight];

        public char Left => this._frameChars[(byte)FramePiece.Left];

        public char Right => this._frameChars[(byte)FramePiece.Right];

        public char BottomLeft => this._frameChars[(byte)FramePiece.BottomLeft];

        public char Bottom => this._frameChars[(byte)FramePiece.Bottom];

        public char BottomRight => this._frameChars[(byte)FramePiece.BottomRight];
    }
}