using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
namespace d
{
    class NoiseGen
    {
        public double noiseval;
        public double[,] noisegrid = new double[256, 256];
        public int oct = 0, freq = 1;
        public Random random = new Random();
        public Bitmap out1, out2, out3;
        public double[,] Noisegridgen()
        {
            while (freq <= noisegrid.GetLength(0)&&freq <= noisegrid.GetLength(1))
            {
                for (int i = 0; i < noisegrid.GetLength(0)/freq; i++)
                {
                    for (int j = 0; j < noisegrid.GetLength(1)/freq; j++)
                    {
                        noiseval = random.NextDouble();
                        for (int k = 0; k < freq; k++)
                        {
                            for (int l = 0; l < freq; l++)
                            {
                                noisegrid[(i * freq) + k, (j * freq) + l] += noiseval;
                            }
                        }
                    }
                }
                freq *= 2;
                oct++;
            }
            for (int a = 0; a < noisegrid.GetLength(0); a++)
            {
                for (int b = 0; b < noisegrid.GetLength(1); b++)
                {
                    noisegrid[a, b] /= oct;
                    if (noisegrid[a,b] > 1 || noisegrid[a,b]<0)
                    {
                        noisegrid[a, b] = Convert.ToDouble(Convert.ToInt32(noisegrid[a, b]));
                    }
                    //Console.WriteLine("noisegrid[{0},{1}] = {2}", a, b, noisegrid[a, b]);
                }
            }//generated noise grid, now save as png file after making a bitmap
            return noisegrid;
        }
        public double[,] Linear(double[,] a)
        {
            double[,] b = new double[a.GetLength(0), a.GetLength(1)]; double[,] diff = new double[a.GetLength(0), a.GetLength(1)];
            double c1, c2, c3, c4, c5,c6,c7,c8,c9;
            for (int i=1;i<a.GetLength(0)-1;i++)
            {
                for (int j=1;j<a.GetLength(1)-1;j++)
                {
                    c2 = a[i, j + 1]; c3 = a[i + 1, j]; c4 = a[i, j - 1]; c5 = a[i - 1, j];c6 = a[i + 1, j + 1];c7 = a[i - 1, j + 1];c8 = a[i - 1, j - 1];c9 = a[i + 1, j - 1];
                    c1 = (c2 + c3 + c4 + c5 + c6 + c7 + c8 + c9) / 8;
                    b[i, j] = c1;
                    diff[i, j] = a[i, j] - b[i, j];
                    //Console.WriteLine("a[{0},{1}] = {2} . b[{0},{1}] = {3} . diff = {4}", i, j, a[i, j], b[i, j], diff[i, j]);
                }
            }
            return b;
        }
        public double[,] CenterDistFactor(double[,] a)
        {
            double[,] b = new double[a.GetLength(0), a.GetLength(1)];
            double centerdist, xdist, ydist, centerX, centerY,multi;
            centerX = (a.GetLength(0) / 2) - 1;
            centerY = (a.GetLength(1) / 2) - 1;
            for (int x=0;x<a.GetLength(0);x++)
            {
                for (int y=0;y<a.GetLength(1);y++)
                {
                    xdist = x - centerX;
                    ydist = y - centerY;
                    centerdist = Math.Sqrt((Math.Pow(xdist, 2)) + (Math.Pow(ydist, 2)));
                    multi = Math.Sqrt(Math.Sqrt(a.Length) / centerdist);
                    b[x, y] = /*Math.Sqrt*/(a[x, y] * multi);
                    if (b[x,y] > 1)
                    {
                        b[x, y] = 1;
                    }
                    else if (b[x, y] < 0)
                    {
                        b[x, y] = 0;
                    }
                    //Console.WriteLine("Centerdist of [{0},{1}]= {2} . Multi = {3} . b[{0},{1}] = {4}", x, y, centerdist, multi, b[x, y]);
                }
            }
            return b;
        }
        public double[,] FractalNoiseGen(double[,] a, double[,] b)
        {
            double[,] fractal = new double[a.GetLength(0), a.GetLength(1)];
            for (int x=0;x<fractal.GetLength(0);x++)
            {
                for (int y=0;y<fractal.GetLength(1);y++)
                {
                    fractal[x, y] = (a[x, y] + b[x, y]) / 2;
                }
            }
            PictureBox pictureBox = new PictureBox
            {
                Height = 4096,
                Width = 4096
            };
            Bitmap bitmap = new Bitmap(pictureBox.Width, pictureBox.Height, pictureBox.CreateGraphics());
            int boxsizex = bitmap.Width / a.GetLength(0), boxsizey = bitmap.Height / a.GetLength(1);
            int color;
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    color = Convert.ToInt32(255 * a[i, j]);
                    for (int k = 0; k < boxsizex; k++)
                    {
                        for (int l = 0; l < boxsizey; l++)
                        {
                            bitmap.SetPixel((i * boxsizex) + k, (j * boxsizey) + l, Color.FromArgb(255, color, color, color));
                        }
                    }
                }
            }
            out3 = bitmap;
            bitmap.Save((Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)) + "\\d_fractal.png", System.Drawing.Imaging.ImageFormat.Png);
            return fractal;
        }
        public Bitmap GetBitmap(double[,] a)
        {
            PictureBox pictureBox = new PictureBox
            {
                Height = 4096,
                Width = 4096
            };
            Bitmap bitmap = new Bitmap(pictureBox.Width, pictureBox.Height, pictureBox.CreateGraphics());
            int boxsizex = bitmap.Width / a.GetLength(0), boxsizey = bitmap.Height / a.GetLength(1);
            int color;
            for (int i=0;i<a.GetLength(0);i++)
            {
                for (int j=0;j<a.GetLength(1);j++)
                {
                    color = Convert.ToInt32(255 * a[i, j]); 
                    for (int k=0;k<boxsizex;k++)
                    {
                        for (int l=0;l<boxsizey;l++)
                        {
                            bitmap.SetPixel((i * boxsizex) + k, (j * boxsizey) + l, Color.FromArgb(255, color, color, color));
                        }
                    }
                }
            }
            bitmap.Save((Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)) + "\\d.png", System.Drawing.Imaging.ImageFormat.Png);
            out1 = bitmap;
            return bitmap;
        }
        public Bitmap InterBitmap(Bitmap a)
        {
            PictureBox pictureBox = new PictureBox
            {
                Height = 4096,
                Width = 4096
            };
            Bitmap bitmap = new Bitmap(a.Width, a.Height, pictureBox.CreateGraphics());
            double n1, n2, n3, n4,n5,n6,n7,n8;
            int color;
            for (int x=1;x<bitmap.Width-1;x++)
            {
                for (int y=1;y<bitmap.Height-1;y++)
                {
                    n1 = a.GetPixel(x - 1, y).R;
                    n2 = a.GetPixel(x, y - 1).R;
                    n3 = a.GetPixel(x + 1, y).R;
                    n4 = a.GetPixel(x, y + 1).R;
                    n5 = a.GetPixel(x - 1, y - 1).R;
                    n6 = a.GetPixel(x - 1, y + 1).R;
                    n7 = a.GetPixel(x + 1, y - 1).R;
                    n8 = a.GetPixel(x + 1, y + 1).R;
                    color = Convert.ToInt32((n1 + n2 + n3 + n4 + n5 + n6 + n7 + n8) / 8);
                    bitmap.SetPixel(x, y, Color.FromArgb(255, color, color, color));
                }
            }
            Color c;
            c = a.GetPixel(0, 0); bitmap.SetPixel(0, 0, c); c = a.GetPixel(0, a.Height - 1); bitmap.SetPixel(0, a.Height - 1, c); c = a.GetPixel(a.Width - 1, 0); bitmap.SetPixel(a.Width - 1, 0, c); c = a.GetPixel(a.Width - 1, a.Height - 1); bitmap.SetPixel(a.Width - 1, a.Height - 1, c);
            bitmap.Save((Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)) + "\\inter_d.png", System.Drawing.Imaging.ImageFormat.Png);
            out2 = bitmap;
            return bitmap;
        }
        public Bitmap DiffChecker(Bitmap a, Bitmap b)
        {
            PictureBox pictureBox = new PictureBox
            {
                Height = 4096,
                Width = 4096
            };
            Bitmap bitmap = new Bitmap(a.Width, a.Height, pictureBox.CreateGraphics());
            int diff;
            for (int x=0;x<a.Width;x++)
            {
                for (int y=0;y<a.Height;y++)
                {
                    diff = Math.Abs( b.GetPixel(x, y).R - a.GetPixel(x, y).R); 
                }
            }
            bitmap.Save((Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)) + "\\d_diffchecker.png", System.Drawing.Imaging.ImageFormat.Png);
            return bitmap;
        }
        public Bitmap ColorBitmap(Bitmap a)
        {
            PictureBox pictureBox = new PictureBox
            {
                Height = a.Height,
                Width = a.Width
            };
            Bitmap bitmap = new Bitmap(a.Width, a.Height, pictureBox.CreateGraphics());
            int color;
            for (int x=0;x<a.Width;x++)
            {
                for (int y=0;y<a.Height;y++)
                {
                    color = a.GetPixel(x, y).R;
                    bitmap.SetPixel(x, y, Color.FromArgb(255, Convert.ToInt32(0.2 * color), color, Convert.ToInt32(0.2 * color)));
                }
            }
            bitmap.Save((Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)) + "\\d_green.png", System.Drawing.Imaging.ImageFormat.Png);
            return bitmap;
        }
        public Bitmap TerrainMapper(Bitmap a)
        {
            PictureBox pictureBox = new PictureBox
            {
                Height = 4096,
                Width = 4096
            };
            Bitmap bitmap = new Bitmap(a.Width, a.Height, pictureBox.CreateGraphics());
            int color;
            for (int x=0;x<bitmap.Width;x++)
            {
                for (int y=0;y<bitmap.Height;y++)
                {
                    color = a.GetPixel(x, y).R;
                    color = Convert.ToInt32(255 * (Math.Pow(Math.E, Math.Log((color / 255)))));
                    if (color > 210)
                    {
                        bitmap.SetPixel(x, y, Color.Snow);
                    }
                    else if (color > 150)
                    {
                        bitmap.SetPixel(x, y, Color.LightGray);
                    }
                    else if (color > 100)
                    {
                        bitmap.SetPixel(x, y, Color.YellowGreen);
                    }
                    else if (color > 60)
                    {
                        bitmap.SetPixel(x, y, Color.LawnGreen);
                    }
                    else if (color > 30)
                    {
                        bitmap.SetPixel(x, y, Color.SandyBrown);
                    }
                    else
                    {
                        bitmap.SetPixel(x, y, Color.Navy);
                    }
                    
                }
            }
            bitmap.Save((Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)) + "\\d_terrain.png", System.Drawing.Imaging.ImageFormat.Png);
            return bitmap;
        }
    }
    class Caller
    {
        static void Main(string[] args)
        {
            NoiseGen p = new NoiseGen();
            p.Noisegridgen();
            PictureBox pictureBox = new PictureBox
            {
                Height = 4096,
                Width = 4096
            }; Bitmap a = new Bitmap(pictureBox.Width, pictureBox.Height, pictureBox.CreateGraphics());
            Bitmap b = new Bitmap(pictureBox.Width, pictureBox.Height, pictureBox.CreateGraphics());
            double[,] i = new double[p.noisegrid.GetLength(0), p.noisegrid.GetLength(1)], j = new double[p.noisegrid.GetLength(0), p.noisegrid.GetLength(1)];
            i = p.CenterDistFactor(p.Linear(p.Noisegridgen()));
            a = p.InterBitmap(p.GetBitmap(i));
            j = p.CenterDistFactor(p.Linear(p.Noisegridgen()));
            b = p.InterBitmap(p.GetBitmap(j));
            a.Save((Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)) + "\\d_a.png", System.Drawing.Imaging.ImageFormat.Png);
            b.Save((Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)) + "\\d_b.png", System.Drawing.Imaging.ImageFormat.Png);
            double[,] c = new double[i.GetLength(0), i.GetLength(1)];
            p.FractalNoiseGen(i, j);
            a.Dispose(); b.Dispose();
            p.TerrainMapper(p.out3);
            Console.ReadLine();
        }
    }
    class DataSaveLoad
    {

    }
}
