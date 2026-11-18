using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphicsEditor
{
    public partial class Form1 : Form
    {
        private Bitmap canvas;
        private Graphics g;
        private bool isDrawing = false;
        private Point lastPoint;
        private Color currentColor = Color.Black;

        public Form1()
        {
            InitializeComponent();
            SetupCanvas();
        }

        private void SetupCanvas()
        {
            canvas = new Bitmap(pictureBox.Width, pictureBox.Height);
            g = Graphics.FromImage(canvas);
            g.Clear(Color.White);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            pictureBox.Image = canvas;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDrawing = true;
                lastPoint = e.Location;
            }
        }

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
            pictureBox.Invalidate();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isDrawing = false;
        }

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

        private void btnClear_Click(object sender, EventArgs e)
        {
            g.Clear(Color.White);
            pictureBox.Invalidate();
        }

        private void trackBarSize_Scroll(object sender, EventArgs e)
        {
            label1.Text = "Толщина: " + trackBarSize.Value;
        }

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

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "Сохранить изображение";
                sfd.Filter = "PNG (*.png)|*.png|JPEG (*.jpg)|*.jpg|BMP (*.bmp)|*.bmp";
                sfd.DefaultExt = "png";
                sfd.FileName = "рисунок";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ImageFormat format = ImageFormat.Png;
                        string ext = Path.GetExtension(sfd.FileName).ToLower();

                        if (ext == ".jpg" || ext == ".jpeg") format = ImageFormat.Jpeg;
                        else if (ext == ".bmp") format = ImageFormat.Bmp;

                        canvas.Save(sfd.FileName, format);
                        MessageBox.Show("Изображение сохранено!", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка сохранения: " + ex.Message, "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Текущий рисунок будет заменён. Продолжить?",
            "Открыть файл", MessageBoxButtons.YesNo,
            MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Открыть изображение";
                ofd.Filter = "Изображения (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp|Все файлы (*.*)|*.*";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Bitmap loaded;
                        using (var temp = new Bitmap(ofd.FileName))
                        {
                            loaded = new Bitmap(temp);
                        }

                        g.Clear(Color.White);
                        g.DrawImage(loaded, 0, 0);
                        loaded.Dispose();

                        pictureBox.Invalidate();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка загрузки: " + ex.Message, "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}