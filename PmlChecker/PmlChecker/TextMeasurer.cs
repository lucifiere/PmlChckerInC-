using System.Drawing;

namespace PmlChecker.PmlChecker
{
    class TextMeasurer
    {
        static Image _fakeImage;

        public static SizeF MeasureString(string text, Font font)
        {
            if (_fakeImage == null)
            {
                _fakeImage = new Bitmap(1, 1);
            }

            using (var g = Graphics.FromImage(_fakeImage))
            {
                return g.MeasureString(text, font);
            }
        }
    }
}
