using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommandSender
{
    public partial class Form1 : Form
    {
        Process[] Plist = Process.GetProcesses();

        List<Process> PEqual = new List<Process>(); 

        Process SelectedProcess;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for(int i = 0; i < Plist.Length; i++)
            {
                listBox1.Items.Add(Plist[i].ProcessName);
                PEqual.Add(Plist[i]);
            }
        }



        public void click()
        {
            
        }

        [DllImport("user32.dll")]
        static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

        [DllImport("user32.dll")]
        internal static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);
        
        internal struct INPUT
        {
            public UInt32 Type;
            public MOUSEKEYBDHARDWAREINPUT Data;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct MOUSEKEYBDHARDWAREINPUT
        {
            [FieldOffset(0)]
            public MOUSEINPUT Mouse;
        }

        internal struct MOUSEINPUT
        {
            public Int32 X;
            public Int32 Y;
            public UInt32 MouseData;
            public UInt32 Flags;
            public UInt32 Time;
            public IntPtr ExtraInfo;
        }

        public static void ClickOnPoint(IntPtr wndHandle, Point clientPoint)
        {
            var oldPos = Cursor.Position;

            /// get screen coordinates
            ClientToScreen(wndHandle, ref clientPoint);

            /// set cursor on coords, and press mouse
            Cursor.Position = new Point(clientPoint.X, clientPoint.Y);

            var inputMouseDown = new INPUT();
            inputMouseDown.Type = 0; /// input type mouse
            inputMouseDown.Data.Mouse.Flags = 0x0002; /// left button down

            var inputMouseUp = new INPUT();
            inputMouseUp.Type = 0; /// input type mouse
            inputMouseUp.Data.Mouse.Flags = 0x0004; /// left button up

            var inputs = new INPUT[] { inputMouseDown, inputMouseUp };
            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));

            /// return mouse 
            Cursor.Position = oldPos;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                Process item = PEqual[listBox1.SelectedIndex];
                richTextBox1.Text = $"Name: {item.ProcessName}\nID: {item.Id}\nMem: {item.VirtualMemorySize64}\nHandle: {item.MainWindowHandle}";
                SelectedProcess = item;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if(textBox1.Text.Length>0)
            {
                listBox1.Items.Clear();
                PEqual.Clear();
                for (int i = 0; i < Plist.Length; i++)
                {
                    if(Plist[i].ProcessName.ToLower().Contains(textBox1.Text.ToLower()))
                    {
                        listBox1.Items.Add(Plist[i].ProcessName);
                        PEqual.Add(Plist[i]);
                    }
                }
            } else
            {
                listBox1.Items.Clear();
                PEqual.Clear();
                for (int i = 0; i < Plist.Length; i++)
                {
                    listBox1.Items.Add(Plist[i].ProcessName);
                    PEqual.Add(Plist[i]);
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for(int i = 0; i< 1000; i++)
            {
                CenterClick(SelectedProcess.MainWindowHandle);
                Thread.Sleep(10);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 100000; i++)
            {
                CenterClick(SelectedProcess.MainWindowHandle);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CenterClick(SelectedProcess.MainWindowHandle);
            CenterClick(SelectedProcess.MainWindowHandle);
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void CenterClick(IntPtr hWnd)
        {
            Size s = GetControlSize(hWnd);
            ClickOnPoint(hWnd, new Point((int)(s.Width / 2.0), (int)(s.Height / 2.0)));
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        public static Size GetControlSize(IntPtr hWnd)
        {
            RECT pRect;
            Size cSize = new Size();
            // get coordinates relative to window
            GetWindowRect(hWnd, out pRect);

            cSize.Width = pRect.Right - pRect.Left;
            cSize.Height = pRect.Bottom - pRect.Top;

            return cSize;
        }
    }
}
