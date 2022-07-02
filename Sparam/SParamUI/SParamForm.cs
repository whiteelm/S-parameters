using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using SParameters;


namespace SParamUI
{
    public partial class SParamForm : Form
    {
        /// <summary>
        /// Значения по умолчанию
        /// </summary>
        private SParams _sParameters = new SParams(
            500,
            0,
            20,
            0.005,
            0,
            0,
            0.412,
            176,
            34.2,
            68.4
            );
        
        public SParamForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Нажатие на кнопку и расчёт S-параметров.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawButton_Click(object sender, EventArgs e)
        {
            try
            {
                var mode = 0;
                if (mode0.Checked)
                {
                    mode = 0;
                }
                if (mode1.Checked)
                {
                    mode = 1;
                    _sParameters.Cp1 = Convert.ToDouble(CL1TextBox.Text);
                    _sParameters.Cp2 = Convert.ToDouble(CL2TextBox.Text);
                }
                if (mode2.Checked)
                {
                    mode = 2;
                    _sParameters.Cs1 = Convert.ToDouble(CL1TextBox.Text);
                    _sParameters.Cs2 = Convert.ToDouble(CL2TextBox.Text);
                }
                if (mode3.Checked)
                {
                    mode = 3;
                    _sParameters.Lp1 = Convert.ToDouble(CL1TextBox.Text);
                    _sParameters.Lp2 = Convert.ToDouble(CL2TextBox.Text);
                }
                _sParameters = new SParams(
                    Convert.ToInt32(NfTextBox.Text),
                    Convert.ToInt32(FminTextBox.Text),
                    Convert.ToInt32(FmaxTextBox.Text),
                    Convert.ToDouble(LenTextBox.Text),
                    Convert.ToDouble(R1TextBox.Text),
                    Convert.ToDouble(G1TextBox.Text),
                    Convert.ToDouble(L1TextBox.Text),
                    Convert.ToDouble(C1TextBox.Text),
                    Convert.ToDouble(zinTextBox.Text),
                    Convert.ToDouble(zoutTextBox.Text)
                );
                if (mode1.Checked)
                {
                    _sParameters.Cp1 = Convert.ToDouble(CL1TextBox.Text);
                    _sParameters.Cp2 = Convert.ToDouble(CL2TextBox.Text);
                }
                if (mode2.Checked)
                {
                    _sParameters.Cs1 = Convert.ToDouble(CL1TextBox.Text);
                    _sParameters.Cs2 = Convert.ToDouble(CL2TextBox.Text);
                }
                if (mode3.Checked)
                {
                    _sParameters.Lp1 = Convert.ToDouble(CL1TextBox.Text);
                    _sParameters.Lp2 = Convert.ToDouble(CL2TextBox.Text);
                }
                _sParameters.CalculateSParameters(mode);
                radioButton1.Enabled = true;
                radioButton2.Enabled = true;
                S1Label.Enabled = true;
                S2Label.Enabled = true;
                S3Label.Enabled = true;
                checkBox1.Enabled = true;
                checkBox2.Enabled = true;
                checkBox3.Enabled = true;
                DrawGraphics(_sParameters);
            }
            catch
            {
                MessageBox.Show(@"Проверьте данные.", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        /// <summary>
        /// Рисование графиков.
        /// </summary>
        /// <param name="sParameters"></param>
        private void DrawGraphics(SParams sParameters)
        {
            for (var i = 0; i < 6; i++)
            {
                chart1.Series[i].Points.Clear();
            }
            var interval = Convert.ToDouble(
                (Convert.ToDouble(sParameters.Fmax) - 
                 Convert.ToDouble(sParameters.Fmin)) /
                 Convert.ToDouble(sParameters.Nf));
            for (var i = 2; i < sParameters.Nf; i++)
            {
                chart1.Series[0].Points.AddXY(sParameters.F[i], sParameters.S[0][i]);
                chart1.Series[1].Points.AddXY(sParameters.F[i], sParameters.S[1][i]);
                chart1.Series[2].Points.AddXY(sParameters.F[i], sParameters.S[2][i]);
                chart1.Series[3].Points.AddXY(sParameters.F[i], sParameters.Fi[0][i]);
                chart1.Series[4].Points.AddXY(sParameters.F[i], sParameters.Fi[1][i]);
                chart1.Series[5].Points.AddXY(sParameters.F[i], sParameters.Fi[2][i]);
            }
            chart1.ChartAreas[0].AxisX.Minimum = sParameters.Fmin;
            chart1.ChartAreas[0].AxisX.Maximum = sParameters.Fmax;
        }

        /// <summary>
        /// Построение ФЧХ.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton2_CheckedChanged(object sender, EventArgs e)
        {
            chart1.ResetAutoValues();
            if (!radioButton2.Checked) return;
            chart1.Series[0].Enabled = false;
            chart1.Series[1].Enabled = false;
            chart1.Series[2].Enabled = false;
            if(checkBox1.Checked)
            {
                chart1.Series[3].Enabled = true;
            }
            if (checkBox2.Checked)
            {
                chart1.Series[4].Enabled = true;
            }
            if (checkBox3.Checked)
            {
                chart1.Series[5].Enabled = true;
            }
            S1Label.Text = @"φ11";
            S2Label.Text = @"φ12";
            S3Label.Text = @"φ22";
        }

        /// <summary>
        /// Построение АЧХ.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            chart1.ResetAutoValues();
            if (!radioButton1.Checked) return;
            if (checkBox1.Checked)
            {
                chart1.Series[0].Enabled = true;
            }
            if (checkBox2.Checked)
            {
                chart1.Series[1].Enabled = true;
            }
            if (checkBox3.Checked)
            {
                chart1.Series[2].Enabled = true;
            }
            chart1.Series[3].Enabled = false;
            chart1.Series[4].Enabled = false;
            chart1.Series[5].Enabled = false;
            S1Label.Text = @"S11";
            S2Label.Text = @"S12";
            S3Label.Text = @"S22";
        }

        /// <summary>
        /// Событие ввода с клавиатуры в текстбокс.
        /// </summary>
        private void ValidateDoubleTextBoxes_KeyPress(object sender,
            KeyPressEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.KeyChar.ToString(),
                @"[\d\b,]");
        }

        /// <summary>
        /// Показ графиков при изменении чекбоксов.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (sender.Equals(checkBox1) && radioButton1.Checked && checkBox1.Checked)
            {
                chart1.Series[0].Enabled = true;
            }
            if (sender.Equals(checkBox2) && radioButton1.Checked && checkBox2.Checked)
            {
                chart1.Series[1].Enabled = true;
            }
            if (sender.Equals(checkBox3) && radioButton1.Checked && checkBox3.Checked)
            {
                chart1.Series[2].Enabled = true;
            }
            if (sender.Equals(checkBox1) && radioButton2.Checked && checkBox1.Checked)
            {
                chart1.Series[3].Enabled = true;
            }
            if (sender.Equals(checkBox2) && radioButton2.Checked && checkBox2.Checked)
            {
                chart1.Series[4].Enabled = true;
            }
            if (sender.Equals(checkBox3) && radioButton2.Checked && checkBox3.Checked)
            {
                chart1.Series[5].Enabled = true;
            }
            if (sender.Equals(checkBox1) && radioButton1.Checked && !checkBox1.Checked)
            {
                chart1.Series[0].Enabled = false;
            }
            if (sender.Equals(checkBox2) && radioButton1.Checked && !checkBox2.Checked)
            {
                chart1.Series[1].Enabled = false;
            }
            if (sender.Equals(checkBox3) && radioButton1.Checked && !checkBox3.Checked)
            {
                chart1.Series[2].Enabled = false;
            }
            if (sender.Equals(checkBox1) && radioButton2.Checked && !checkBox1.Checked)
            {
                chart1.Series[3].Enabled = false;
            }
            if (sender.Equals(checkBox2) && radioButton2.Checked && !checkBox2.Checked)
            {
                chart1.Series[4].Enabled = false;
            }
            if (sender.Equals(checkBox3) && radioButton2.Checked && !checkBox3.Checked)
            {
                chart1.Series[5].Enabled = false;
            }
        }

        /// <summary>
        /// Изменение чекбоксов при нажатии на текст.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SLabel_Click(object sender, EventArgs e)
        {
            if (sender.Equals(S1Label))
            {
                checkBox1.Checked = !checkBox1.Checked;
            }
            if (sender.Equals(S2Label))
            {
                checkBox2.Checked = !checkBox2.Checked;
            }
            if (sender.Equals(S3Label))
            {
                checkBox3.Checked = !checkBox3.Checked;
            }
        }

        private void Mode_CheckedChanged(object sender, EventArgs e)
        {
            if (sender == mode0)
            {
                CL1TextBox.Enabled = false;
                CL2TextBox.Enabled = false;
                CL1Label.Enabled = false;
                CL2Label.Enabled = false;
                CL1Label.Text = @"CL1";
                CL2Label.Text = @"CL2";
                CL1TextBox.Text = "";
                CL2TextBox.Text = "";
            }
            if (sender == mode1)
            {
                CL1TextBox.Enabled = true;
                CL2TextBox.Enabled = true;
                CL1Label.Enabled = true;
                CL2Label.Enabled = true;
                CL1Label.Text = @"C1, pF";
                CL2Label.Text = @"C2, pF";
                CL1TextBox.Text = @"0,2";
                CL2TextBox.Text = @"0,2";
            }
            if (sender == mode2)
            {
                CL1TextBox.Enabled = true;
                CL2TextBox.Enabled = true;
                CL1Label.Enabled = true;
                CL2Label.Enabled = true;
                CL1Label.Text = @"C1, pF";
                CL2Label.Text = @"C2, pF";
                CL1TextBox.Text = @"0,05";
                CL2TextBox.Text = @"0,05";
            }
            if (sender == mode3)
            {
                CL1TextBox.Enabled = true;
                CL2TextBox.Enabled = true;
                CL1Label.Enabled = true;
                CL2Label.Enabled = true;
                CL1Label.Text = @"L1, µH";
                CL2Label.Text = @"L2, µH";
                CL1TextBox.Text = @"0,00012";
                CL2TextBox.Text = @"0,00012";
            }
        }
    }
}
