using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysX_test2
{
    /// <summary>
    /// Специальный класс для настройки цветов интерфейса. При создании контролов использовать этот класс!!! 
    /// </summary>
    public static class GColors
    {
        public static Color CHighLightText = Color.FromNonPremultiplied(100,100,255,120);
        public static Color CError = Color.Yellow;
        public static Color CControl = Color.FromNonPremultiplied(100, 100, 100, 100);
        public static Color CTextBack = Color.FromNonPremultiplied(0, 0, 0, 100); // желательно, чтоб был прозрачный
        public static Color CText = Color.White;
        public static Color CForeColor = Color.Black;
        public static Color CAlarm = Color.Red;
        public static Color CZero = Color.FromNonPremultiplied(0,0,0,0);
    }
}
