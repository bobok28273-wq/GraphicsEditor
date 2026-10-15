using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphicsEditor
{
    public partial class Form1 : Form
    {
        private Bitmap canvas;          // холст, на котором рисуем
        private Graphics g;             // графика холста
        private bool isDrawing = false; // зажата ли мышь
        private Point lastPoint;        // предыдущая точка
        private Color currentColor = Color.Black;

        public Form1()
        {
            InitializeComponent();
            SetupCanvas();
        }

        private void SetupCanvas()
        {
            // создаём bitmap размером с pictureBox
            canvas = new Bitmap(pictureBox.Width, pictureBox.Height);
            g = Graphics.FromImage(canvas);
            g.Clear(Color.White);           // белый фон
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            pictureBox.Image = canvas;
        }

        // Начало рисования
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDrawing = true;
                lastPoint = e.Location;
            }
        }

        // Процесс рисования
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDrawing) return;

            using (Pen pen = new Pen(currentColor, trackBarSize.Value))
            {
                pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                g.DrawLine(pen, lastPoint, e.Location);
            }
            lastPoint = e.Location;
            pictureBox.Invalidate(); // перерисовать
        }

        // Конец рисования
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isDrawing = false;
        }

        // Выбор цвета
        private void btnColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog cd = new ColorDialog())
            {
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    currentColor = cd.Color;
                    btnColor.BackColor = currentColor;
                }
            }
        }

        // Очистка
        private void btnClear_Click(object sender, EventArgs e)
        {
            g.Clear(Color.White);
            pictureBox.Invalidate();
        }

        // Изменение толщины
        private void trackBarSize_Scroll(object sender, EventArgs e)
        {
            label1.Text = "Толщина: " + trackBarSize.Value;
        }

        // При изменении размера формы — пересоздать холст
        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            if (pictureBox.Width <= 0 || pictureBox.Height <= 0) return;
            if (canvas == null) return;

            Bitmap newCanvas = new Bitmap(pictureBox.Width, pictureBox.Height);
            using (Graphics ng = Graphics.FromImage(newCanvas))
            {
                ng.Clear(Color.White);
                ng.DrawImage(canvas, 0, 0);
            }
            canvas = newCanvas;
            g = Graphics.FromImage(canvas);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            pictureBox.Image = canvas;
        }
    }
}