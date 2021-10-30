using System;
using System.Collections.Generic;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace SParameters
{
    public class SParams
    {
        /// <summary>
        /// Количество точек.
        /// </summary>
        public int Nf { get; set; }

        /// <summary>
        /// Начало частотных точек.
        /// </summary>
        public int Fmin { get; set; }

        /// <summary>
        /// Конец частотных точек.
        /// </summary>
        public int Fmax { get; set; }

        /// <summary>
        /// Длина линии.
        /// </summary>
        public double Len { get; set; }

        /// <summary>
        /// R1
        /// </summary>
        public double R { get; set; }

        /// <summary>
        /// G1
        /// </summary>
        public double G { get; set; }

        /// <summary>
        /// Индуктивность.
        /// </summary>
        private double _l;

        /// <summary>
        /// Индуктивность.
        /// </summary>
        public double L
        {
            get => _l;
            set => _l = value * 1e-6;
        }

        /// <summary>
        /// Ёмкость.
        /// </summary>
        private double _c;

        /// <summary>
        /// Ёмкость.
        /// </summary>
        public double C
        {
            get => _c;
            set => _c = value * 1e-12;
        }

        /// <summary>
        /// Входная нагрузка.
        /// </summary>
        public double Zin { get; set; }

        /// <summary>
        /// Выходная загрузка.
        /// </summary>
        public double Zout { get; set; }

        /// <summary>
        /// Вх. конденсатор параллельный.
        /// </summary>
        private double _cp1;

        /// <summary>
        /// Вх. конденсатор параллельный.
        /// </summary>
        public double Cp1
        {
            get => _cp1;
            set => _cp1 = value * 1e-12;
        }

        /// <summary>
        /// Вых. конденсатор параллельный.
        /// </summary>
        private double _cp2;

        /// <summary>
        /// Вых. конденсатор параллельный.
        /// </summary>
        public double Cp2
        {
            get => _cp2;
            set => _cp2 = value * 1e-12;
        }

        /// <summary>
        /// Вх. индуктор параллельный.
        /// </summary>
        private double _lp1;

        /// <summary>
        /// Вх. индуктор параллельный.
        /// </summary>
        public double Lp1
        {
            get => _lp1;
            set => _lp1 = value * 1e-6;
        }

        /// <summary>
        /// Вых индуктор параллельный.
        /// </summary>
        private double _lp2;

        /// <summary>
        /// Вых индуктор параллельный.
        /// </summary>
        public double Lp2
        {
            get => _lp2;
            set => _lp2 = value * 1e-6;
        }

        /// <summary>
        /// Вх. конденсатор последовательный.
        /// </summary>
        private double _cs1;

        /// <summary>
        /// Вх. конденсатор последовательный.
        /// </summary>
        public double Cs1
        {
            get => _cs1;
            set => _cs1 = value * 1e-12;
        }

        /// <summary>
        /// Вых. конденсатор последовательный.
        /// </summary>
        private double _cs2;

        /// <summary>
        /// Вых. конденсатор последовательный.
        /// </summary>
        public double Cs2
        {
            get => _cs2;
            set => _cs2 = value * 1e-12;
        }

        ///// <summary>
        ///// S - параметры.
        ///// </summary>
        public List<double[]> S = new List<double[]>();

        /// <summary>
        /// Fi-параметры.
        /// </summary>
        public List<double[]> Fi = new List<double[]>();

        /// <summary>
        /// Мнимая единица.
        /// </summary>
        public Complex32 Zi = new Complex32(0, 1);

        /// <summary>
        /// Расчёт S-параметров.
        /// </summary>
        /// <param name="nf">Количество точек.</param>
        /// <param name="fmin">Начало частотных точек.</param>
        /// <param name="fmax">Конец частотных точек.</param>
        /// <param name="len">Длина линии.</param>
        /// <param name="r">R1.</param>
        /// <param name="g">G1.</param>
        /// <param name="l">Индуктивность.</param>
        /// <param name="c">Ёмкость.</param>
        /// <param name="zin">Входная нагрузка.</param>
        /// <param name="zout">Выходная нагрузка.</param>
        public SParams(int nf, int fmin, int fmax, double len,
            double r, double g, double l, double c, double zin,
            double zout)
        {
            Nf = nf;
            Fmin = fmin;
            Fmin = fmin;
            Fmax = fmax;
            Len = len;
            R = r;
            G = g;
            L = l;
            C = c;
            Zin = zin;
            Zout = zout;
            if (Fmin >= Fmax)
            {
                throw new ArgumentException(
                    "Minimum frequency can't be more than maximum frequency");
            }
            if (Zin == Zout)
            {
                throw new ArgumentException(
                    "Zin can't be equal to Zout");
            }
        }

        /// <summary>
        /// Расчёт S-параметров.
        /// <param name="mode">Режим:
        /// 0 - general;
        /// 1 - C-parallel;
        /// 2 - C-serial
        /// 3 - L-parallel</param>
        /// </summary>
        public void CalculateSParameters(int mode)
        {
            var z1 = new Complex32[Nf];
            var y1 = new Complex32[Nf];
            var f = Vector<double>.Build.Dense(Nf);
            var w = Vector<double>.Build.Dense(Nf);
            var zo = Vector<Complex32>.Build.Dense(Nf);
            var gamma = Vector<Complex32>.Build.Dense(Nf);
            var theta = Vector<Complex32>.Build.Dense(Nf);
            var y = Matrix<Complex32>.Build.Dense(2, 2);
            var e = Matrix<Complex32>.Build.Dense(2, 2);
            e[0, 0] = 1;
            e[1, 1] = 1;
            var zt = Matrix<Complex32>.Build.Dense(2, 2);
            zt[0, 0] = Complex32.Sqrt((Complex32)Zin);
            zt[1, 1] = Complex32.Sqrt((Complex32)Zout);
            if (mode == 2)
            {
                zt = zt.Inverse();
            }
            var ybMatrix = Matrix<Complex32>.Build.Dense(2, 2);
            var s11 = new double[Nf];
            var s12 = new double[Nf];
            var s22 = new double[Nf];
            var fi11 = new double[Nf];
            var fi12 = new double[Nf];
            var fi22 = new double[Nf];
            for (var i = 2; i < Nf; i++)
            {
                f[i] = Fmin + ((double)Fmax - Fmin) /
                    ((double)Nf - 1) * i;
                w[i] = 2 * Math.PI * f[i] * 1e9;
                z1[i] = (Complex32)R + Zi * (Complex32)w[i] * (Complex32)L;
                y1[i] = (Complex32)G + Zi * (Complex32)C *
                    (Complex32)w[i];
                zo[i] = Complex32.Sqrt(z1[i] / y1[i]);
                gamma[i] = Complex32.Sqrt(z1[i] * y1[i]);
                theta[i] = gamma[i] * (Complex32)Len;

                y[0, 0] = 1 / (zo[i] * Complex32.Tanh(theta[i]));
                y[0, 1] = -1 / (zo[i] * Complex32.Sinh(theta[i]));
                y[1, 0] = -1 / (zo[i] * Complex32.Sinh(theta[i]));
                y[1, 1] = 1 / (zo[i] * Complex32.Tanh(theta[i]));
                if (mode != 0)
                {
                    switch (mode)
                    {
                        case 1:
                            ybMatrix[0, 0] = Zi * (Complex32)w[i] * (Complex32)Cp1;
                            ybMatrix[1, 1] = Zi * (Complex32)w[i] * (Complex32)Cp2;
                            break;
                        case 2:
                            y[0, 0] = zo[i] / Complex32.Tanh(theta[i]);
                            y[0, 1] = zo[i] / Complex32.Sinh(theta[i]);
                            y[1, 0] = zo[i] / Complex32.Sinh(theta[i]);
                            y[1, 1] = zo[i] / Complex32.Tanh(theta[i]);
                            ybMatrix[0, 0] = 1 / (Zi * (Complex32)w[i] *
                                                  (Complex32)Cs1);
                            ybMatrix[1, 1] = 1 / (Zi * (Complex32)w[i] *
                                                  (Complex32)Cs2);
                            break;
                        case 3:
                            ybMatrix[0, 0] = 1 / (Zi * (Complex32)w[i] *
                                                  (Complex32)Lp1);
                            ybMatrix[1, 1] = 1 / (Zi * (Complex32)w[i] *
                                                  (Complex32)Lp2);
                            break;
                    }
                    y += ybMatrix;
                }

                var yMatrix = zt * y * zt;
                var ss = 2 * (e + yMatrix).Inverse() - e;
                if (mode == 2)
                {
                    ss = e - 2 * (e + yMatrix).Inverse();
                }
                s11[i] = 20 * Math.Log10(Complex32.Abs(ss[0, 0]));
                s12[i] = 20 * Math.Log10(Complex32.Abs(ss[0, 1]));
                s22[i] = 20 * Math.Log10(Complex32.Abs(ss[1, 1]));
                fi11[i] = ss[0, 0].Phase * 57.3;
                fi12[i] = ss[0, 1].Phase * 57.3;
                fi22[i] = ss[1, 1].Phase * 57.3;
            }
            S.Add(s11);
            S.Add(s12);
            S.Add(s22);
            Fi.Add(fi11);
            Fi.Add(fi12);
            Fi.Add(fi22);
        }
    }
}


