﻿using System.Drawing;
using CUE.NET.Devices.Generic;
using CUE.NET.Devices.Keyboard.Enums;

namespace CUE.NET.Devices.Keyboard.Keys
{
    public class CorsairKey
    {
        #region Properties & Fields

        public CorsairKeyboardKeyId KeyId { get; }
        public CorsairLed Led { get; }
        public RectangleF KeyRectangle { get; }

        #endregion

        #region Constructors

        internal CorsairKey(CorsairKeyboardKeyId keyId, CorsairLed led, RectangleF keyRectangle)
        {
            this.KeyId = keyId;
            this.Led = led;
            this.KeyRectangle = keyRectangle;
        }

        #endregion

        #region Methods

        #endregion
    }
}
