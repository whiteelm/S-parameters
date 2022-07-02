using System;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace SParam
{
    internal class Program
    {
        private static void Main()
        {
            var zi = new Complex32(0, 1);
            const int nf = 500;
            const int fmin = 0;
            const int fmax = 20;
            const double len = 0.005;
            const int r1 = 0;
            const double g1 = 0.0;
            const double l1 = 0.412 * 1e-6;
            const double c1 = 176 * 1e-12;
            var Z0 = Math.Sqrt(l1/c1);
            var Z1 = new Complex32[nf];
            var Y1 = new Complex32[nf];
            var f = Vector<double>.Build.Dense(nf);
            var w = Vector<double>.Build.Dense(nf);
            var Zo = Vector<Complex32>.Build.Dense(nf);
            var gamma = Vector<Complex32>.Build.Dense(nf);
            var theta = Vector<Complex32>.Build.Dense(nf);
            var y = Matrix<Complex32>.Build.Dense(2, 2);
            var Zin = Z0 * 1 / Math.Sqrt(2);
            var Zout = Z0 * Math.Sqrt(2);
            var E = Matrix<Complex32>.Build.Dense(2,2);
            E[0, 0] = 1;
            E[1, 1] = 1;
            var zt = Matrix<Complex32>.Build.Dense(2, 2);
            zt[0, 0] = Complex32.Sqrt((Complex32)Zin);
            zt[1, 1] = Complex32.Sqrt((Complex32)Zout);
            var s11 = new double[nf];
            var s12 = new double[nf];
            var s22 = new double[nf];
            var fi11 = new double[nf];
            var fi12 = new double[nf];
            var fi22 = new double[nf];
            for (var i = 0; i < nf; i++)
            {
                f[i] = fmin + ((double) fmax - fmin) / 
                    ((double) nf - 1) * i;
                w[i] = 2 * Math.PI * f[i] * 1e9;
                Z1[i] = r1 + zi * (Complex32) w[i] * (Complex32) l1;
                Y1[i] = (Complex32) g1 + zi * (Complex32) c1 * 
                    (Complex32) w[i];
                Zo[i] = Complex32.Sqrt(Z1[i] / Y1[i]);
                gamma[i] = Complex32.Sqrt(Z1[i] * Y1[i]);
                theta[i] = gamma[i] * (Complex32)len;
                y[0, 0] = 1 / (Zo[i] * Complex32.Tanh(theta[i]));
                y[0, 1] = -1 / (Zo[i] * Complex32.Sinh(theta[i]));
                y[1, 0] = -1 / (Zo[i] * Complex32.Sinh(theta[i]));
                y[1, 1] = 1 / (Zo[i] * Complex32.Tanh(theta[i]));
                var Y = zt * y * zt;
                var ss = 2 * (E + Y).Inverse() - E;
                s11[i] = 20 * Math.Log10(Complex32.Abs(ss[0, 0]));
                s12[i] = 20 * Math.Log10(Complex32.Abs(ss[0, 1]));
                s22[i] = 20 * Math.Log10(Complex32.Abs(ss[1, 1]));
                fi11[i] = ss[0, 0].Phase * 57.3;
                fi12[i] = ss[0, 1].Phase * 57.3;
                fi22[i] = ss[1, 1].Phase * 57.3;
            }
        }
    }
}
