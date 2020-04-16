using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Util;
using RT.Util.Drawing;

namespace Geocos
{
    partial class Program
    {
        static Bitmap Render(GeometricObject[] objects, Dictionary<Variable, double> values, double factor = 10)
        {
            var x1 = objects.Min(obj => obj.GetX1(values));
            var y1 = objects.Min(obj => obj.GetY1(values));
            var x2 = objects.Min(obj => obj.GetX2(values));
            var y2 = objects.Min(obj => obj.GetY2(values));

            var w = (x2 - x1) * factor;
            var h = (y2 - y1) * factor;
            var toBitmapX = Ut.Lambda((double x) => (float) ((x - x1) * factor));
            var toBitmapY = Ut.Lambda((double y) => (float) ((y - y1) * factor));

            return GraphicsUtil.DrawBitmap((int) Math.Ceiling(w), (int) Math.Ceiling(h), g =>
            {
                g.Clear(Color.White);
                foreach (var obj in objects)
                    obj.Render(g, x1, y1, x2, y2, factor, toBitmapX, toBitmapY, values);
            });
        }
    }
}
