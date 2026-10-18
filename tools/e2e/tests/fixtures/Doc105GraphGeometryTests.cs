using System;

class Doc105GraphGeometryTests
{
    static void Equal(double actual, double expected)
    {
        if (Math.Abs(actual - expected) > 0.00001) throw new Exception("Geometry mismatch");
    }
    static void Main()
    {
        int count = 0;
        foreach (double zoom in new[] { 0.5, 1.0, 1.25, 2.0 })
        foreach (int rotation in new[] { 0, 90, 180, 270 })
        foreach (double origin in new[] { 0.0, 20.0, -30.0 })
        {
            double cw = (rotation % 180 == 0 ? 600 : 800) * zoom;
            double ch = (rotation % 180 == 0 ? 800 : 600) * zoom;
            double[] m = GeometriaGrafoPdf.Matriz(origin, origin, origin + 600, origin + 800,
                rotation, cw, ch, 80 * zoom, ch - 120 * zoom, 100 * zoom, 50 * zoom);
            // Comprobar las cuatro esquinas en el espacio visual, no solo el ancla.
            foreach (int ix in new[] { 0, 1 })
            foreach (int iy in new[] { 0, 1 })
            {
                double x = m[0] * ix + m[2] * iy + m[4] - origin;
                double y = m[1] * ix + m[3] * iy + m[5] - origin;
                double vx, vy;
                switch (rotation)
                {
                    case 90: vx = y; vy = x; break;
                    case 180: vx = 600 - x; vy = y; break;
                    case 270: vx = 800 - y; vy = 600 - x; break;
                    default: vx = x; vy = 800 - y; break;
                }
                Equal(vx, 80 + ix * 100);
                Equal(vy, 120 + (1 - iy) * 50);
            }
            count++;
        }
        foreach (double bad in new[] { 0.0, -1.0, double.NaN, double.PositiveInfinity })
        {
            bool rejected = false;
            try { GeometriaGrafoPdf.Matriz(0, 0, 600, 800, 0, bad, 800, 10, 700, 100, 50); }
            catch (ArgumentException) { rejected = true; }
            if (!rejected) throw new Exception("Invalid dimensions accepted");
        }
        Console.WriteLine(count + " cases passed");
    }
}
