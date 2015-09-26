using System;
using System.Diagnostics;
using System.Drawing;
using CUE.NET.Devices.Keyboard.Brushes;

namespace AudioAnalyzer
{
    public class FFTSpectrumBrush : IBrush
    {
        #region Properties & Fields

        public Color Color { get; set; } = Color.White;

        private byte[] _fftData;

        #endregion

        #region Methods

        public void Update(byte[] fftData)
        {
            _fftData = fftData;
        }

        public Color GetColorAtPoint(RectangleF rectangle, PointF point)
        {
            if (_fftData == null) return Color.Transparent;

            int index = ((int)((point.X / rectangle.Width) * 32)) * 8;
            if (index < 0) index = 0;
            else if (index > _fftData.Length - 1) index = _fftData.Length - 1;

            float heightIndex = ((rectangle.Height - point.Y) / rectangle.Height) * byte.MaxValue;

            Debug.WriteLine(index + " - " + heightIndex);

            return _fftData[index] > heightIndex ? Color : Color.Transparent;
        }

        #endregion
    }
}
