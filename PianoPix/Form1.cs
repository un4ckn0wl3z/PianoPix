using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PianoPix
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        static extern void mouse_event(int a, int b, int c, int d, int swed);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);



        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("User32.Dll")]
        public static extern long SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(Keys vkey);


        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        int x;
        int y;
        bool on = false;
        const int leftDown = 0x02;
        const int leftUp = 0x04;
        Point pos = new Point(0, 0);




        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Thread AT = new Thread(AutoTile) { IsBackground = true };
            Thread HT = new Thread(Hotkey) { IsBackground = true };
            AT.Start();
            HT.Start();

        }

        void Hotkey()
        {
            while (true)
            {
                if (GetAsyncKeyState(Keys.P) < 0)
                {
                    on = true;

                }
                if (GetAsyncKeyState(Keys.O) < 0)
                {
                    on = false;
                }
            }
        }

        void AutoTile()
        {
            var hWND = FindWindowByCaption(IntPtr.Zero,"Don't Tap The White Tile 4 ( Piano Tiles 2 )");
            RECT rect = new RECT();
            if (hWND == IntPtr.Zero)
            {
                MessageBox.Show("NOT FOUND!");
            }

            GetWindowRect(hWND, ref rect);


            x = rect.Left;
            y = rect.Bottom;

            while (true)
            {
                while (on)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        int color = GP(x + 64 + i * 131, y - 527);
                        if (color == 0x000000)
                        {
                            SetCursorPos(x + 64 + i * 131, y - 327);
                            Click();
                        }
                    }
                }

                Thread.Sleep(2);
            }



        }

        int GP(int x, int y)
        {
            pos.X = x;
            pos.Y = y;

            using (var bitmap = new Bitmap(1, 1))
            {
                using (var graphics = Graphics.FromImage(bitmap) )
                {
                    graphics.CopyFromScreen(pos, new Point(0,0), new Size(1,1) );
                }

                Color f1 = bitmap.GetPixel(0, 0);
                return int.Parse(f1.R.ToString("X2") +
                                 f1.G.ToString("X2") +
                                 f1.B.ToString("X2"),
                                 System.Globalization.NumberStyles.HexNumber
                                 );
            }


        }

        void Click()
        {
            mouse_event(leftDown,0,0,0,0);
            mouse_event(leftUp, 0, 0, 0, 0);
        }
    }
}
