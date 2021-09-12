﻿using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Trero.ClientBase.EntityBase;
using Trero.Modules;

namespace Trero.ClientBase.UIBase
{
    public partial class Overlay : Form
    {
        public Overlay()
        {
            InitializeComponent();
            handle = this;

            new Thread(() => {
                Thread.Sleep(100);
                Invoke((MethodInvoker)delegate {
                    Focus();
                });
                while (!Program.quit)
                {
                    Thread.Sleep(1);
                    // Thread.Sleep(1);
                    try
                    {
                        Invoke((MethodInvoker)delegate {
                            MCM.RECT rect = MCM.getMinecraftRect();

                            Placement e = new Placement();
                            GetWindowPlacement(MCM.mcWinHandle, ref e); // Change window size if fullscreen to match extra offsets
                            int vE = 0;
                            int vA = 0;
                            int vB = 0;
                            int vC = 0;
                            if (e.showCmd == 3) // Perfect window offsets
                            {
                                vE = 8;
                                vA = 2;

                                vB = 9; // these have extra because of the windows shadow effect (Not exactly required but oh well)
                                vC = 3;
                            }

                            Location = new Point(rect.Left + 9 + vA, rect.Top + 35 + vE); // Title bar is 32 pixels high
                            Size = new Size(rect.Right - rect.Left - 18 - vC, rect.Bottom - rect.Top - 44 - vB);
                        });
                    }
                    catch { }
                }
                Application.Exit();
            }).Start();
            TopMost = true;
        }

        [DllImport("user32.dll")] static extern IntPtr GetForegroundWindow();
        [DllImportAttribute("User32.dll")] static extern IntPtr SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")] static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] static extern bool GetWindowPlacement(IntPtr hWnd, ref Placement lpwndpl);

        private struct Placement
        {
            public int length;
            public int flags;
            public int showCmd;
            public Point ptMinPosition;
            public Point ptMaxPosition;
            public Rectangle rcNormalPosition;
        }

        public static Overlay handle;

        private void timer1_Tick(object sender, EventArgs e)
        {
            try // fixed
            {
                if (MCM.isMinecraftFocused() && TopMost == false)
                    TopMost = true;
                if (!MCM.isMinecraftFocused() && TopMost == true)
                {
                    if (ActiveForm != this)
                    {
                        Opacity = 1;
                        TopMost = false;
                        SetWindowPos(Handle, new IntPtr(1), 0, 0, 0, 0, 2 | 1 | 10);
                    }
                }
            }
            catch { }

            string list = "";

            try
            {
                var vList = Game.getPlayers();
                list = "Players : " + vList.Count + "\r\n";
                foreach (Actor plr in vList)
                {
                    list += (int)Game.position.Distance(plr.position) + "b " + plr.username + "\r\n";
                }
            }
            catch { }

            if (list == "")
                list = "No players";

            SizeF calclist = TextRenderer.MeasureText(list, playerList.Font);

            if (panel5.Size != new Size((int)calclist.Width, (int)calclist.Height + 20))
            {
                panel1.Size = new Size((int)calclist.Width + 20, (int)calclist.Height + 4);
                panel5.Size = new Size((int)calclist.Width + 20, (int)calclist.Height);
            }

            playerList.Text = list;

            try
            {
                Actor ent = Game.getClosestPlayer();

                if (ent == null)
                {
                    label2.Text = "None";
                    label2.Text = "";
                    label3.Text = "";
                    return;
                }

                var vec = Base.Vec3((int)ent.position.x, (int)ent.position.y, (int)ent.position.z);

                label1.Text = ent.username;
                label2.Text = vec.ToString();
                label3.Text = Game.position.Distance(vec) + "b";
            }
            catch { }
        }

        Font df = new Font(FontFamily.GenericSansSerif, 12f);

        private void Overlay_Paint(object sender, PaintEventArgs e)
        {
            /*
            
            // e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(255, 22, 22, 44)), new Rectangle(0, Size.Height - 2 - (6 * (int)df.Size + 4 * DrawingUtils.screenSize), (int)e.Graphics.MeasureString("ClientInstance: " + Game.clientInstance.ToString("X"), df).Width + 4, Size.Height - (4 * (int)df.Size + 4 * DrawingUtils.screenSize)));

            //e.Graphics.DrawString("Trero Template", df, Brushes.Orange, new PointF(0, 0));

            if (Game.screenData.StartsWith("start_screen"))
            {
                e.Graphics.DrawString("Tretard Edition", new Font(FontFamily.GenericSansSerif, 10f * DrawingUtils.screenSize), Brushes.Orange, // Size.Width / 24f
                    new PointF(DrawingUtils.screenCenter.x - (int)(e.Graphics.MeasureString("Tretard Edition", new Font(FontFamily.GenericSansSerif, 10f * DrawingUtils.screenSize)).Width / 2), DrawingUtils.LogoVCenter.y));
            }

            e.Graphics.DrawString("screenCenter: " + DrawingUtils.screenCenter, df, Brushes.Orange, new PointF(0, Size.Height - 6 - (6 * 14 * DrawingUtils.screenSize)));
            e.Graphics.DrawString("screenSize: " + DrawingUtils.screenSize, df, Brushes.Orange, new PointF(0, Size.Height - 6 - (5 * 14 * DrawingUtils.screenSize)));
            e.Graphics.DrawString("ClientInstance: " + Game.clientInstance.ToString("X"), df, Brushes.Orange, new PointF(0, Size.Height - 6 - (4 * 14 * DrawingUtils.screenSize)));
            e.Graphics.DrawString("Pos: " + Game.position, df, Brushes.Orange, new PointF(0, Size.Height - 6 - (3 * 14 * DrawingUtils.screenSize)));
            e.Graphics.DrawString("Players: " + Game.getPlayers().Count, df, Brushes.Orange, new PointF(0, Size.Height - 6 - (2 * 14 * DrawingUtils.screenSize)));
            e.Graphics.DrawString("Entities: " + Game.getEntites().Count, df, Brushes.Orange, new PointF(0, Size.Height - 6 - (1 * 14 * DrawingUtils.screenSize)));

            */
        }

        private Point MouseDownLocation;
        private void panel2_MouseDown_1(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MouseDownLocation = e.Location;
                panel2.BringToFront();
            }
        }

        private void panel2_MouseMove_1(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                panel2.Left = e.X + panel2.Left - MouseDownLocation.X;
                panel2.Top = e.Y + panel2.Top - MouseDownLocation.Y;
            }
        }

        private void Overlay_Load(object sender, EventArgs e)
        {
            foreach (Module mod in Program.modules)
            {
                Button moduleButton = ClonableButton.Clone();
                moduleButton.Visible = true;
                moduleButton.Name = mod.name;
                moduleButton.Text = mod.name;
                if (mod.keybind != 0x07)
                    moduleButton.Text += " (" + (Keys)mod.keybind + ")";
                moduleButton.Click += moduleActivated;
                moduleButton.MouseDown += keybindActivated;
                moduleButton.FlatAppearance.BorderSize = 0;
                moduleButton.FlatAppearance.BorderColor = TestCategory.BackColor;
                if (mod.category == "Flies")
                {
                    panel7.Controls.Add(moduleButton);
                    panel7.Size = new Size(0, panel7.Controls.Count * ClonableButton.Size.Height);
                    panel6.Size = new Size(panel6.Size.Width, panel7.Controls.Count * ClonableButton.Size.Height + 24);
                }
                if (mod.category == "Visual")
                {
                    panel15.Controls.Add(moduleButton);
                    panel15.Size = new Size(0, panel15.Controls.Count * ClonableButton.Size.Height);
                    panel14.Size = new Size(panel14.Size.Width, panel15.Controls.Count * ClonableButton.Size.Height + 24);
                }
                if (mod.category == "Exploits")
                {
                    panel13.Controls.Add(moduleButton);
                    panel13.Size = new Size(0, panel13.Controls.Count * ClonableButton.Size.Height);
                    panel12.Size = new Size(panel12.Size.Width, panel13.Controls.Count * ClonableButton.Size.Height + 24);
                }
                if (mod.category == "World")
                {
                    panel9.Controls.Add(moduleButton);
                    panel9.Size = new Size(0, panel9.Controls.Count * ClonableButton.Size.Height);
                    panel8.Size = new Size(panel8.Size.Width, panel9.Controls.Count * ClonableButton.Size.Height + 24);
                }
                if (mod.category == "Combat")
                {
                    panel11.Controls.Add(moduleButton);
                    panel11.Size = new Size(0, panel11.Controls.Count * ClonableButton.Size.Height);
                    panel10.Size = new Size(panel10.Size.Width, panel11.Controls.Count * ClonableButton.Size.Height + 24);
                }
                if (mod.category == "Player")
                {
                    panel17.Controls.Add(moduleButton);
                    panel17.Size = new Size(0, panel17.Controls.Count * ClonableButton.Size.Height);
                    panel16.Size = new Size(panel16.Size.Width, panel17.Controls.Count * ClonableButton.Size.Height + 24);
                }
                if (mod.category == "Other")
                {
                    TestCategory.Controls.Add(moduleButton);
                    TestCategory.Size = new Size(0, (TestCategory.Controls.Count - 1) * ClonableButton.Size.Height);
                    panel2.Size = new Size(panel2.Size.Width, (TestCategory.Controls.Count - 1) * ClonableButton.Size.Height + 24);
                }
            }
            foreach (Module mod in Program.modules)
                if (mod.name == "Antibot" || mod.name == "ClickGUI")
                    mod.onEnable();
        }

        private void keybindActivated(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                Button btn = (Button)sender;
                if (btn == null) return;

                btn.Text = btn.Name + " (...)";

                vMod = btn;

                btn.KeyDown += catchKeybind;
                btn.Select();
            }
        }

        private void catchKeybind(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Delete)
            {
                vMod.KeyDown -= catchKeybind;
                vMod.Text = vMod.Name;
                foreach (Module mod in Program.modules)
                {
                    if (mod.name == vMod.Name)
                    {
                        mod.keybind = (char)0x07;
                    }
                }
                return;
            }

            foreach (Module mod in Program.modules)
            {
                if (mod.name == vMod.Name)
                {
                    mod.keybind = (char)(int)(e.KeyCode);
                }
            }

            vMod.Text = vMod.Name + " (" + e.KeyCode + ")";

            vMod.KeyDown -= catchKeybind;
        }

        public Button vMod = null;

        private void moduleActivated(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (btn == null) return;

            foreach (Module mod in Program.modules)
            {
                if (mod.name == btn.Name)
                {
                    if (mod.enabled)
                    {
                        mod.onDisable();
                        btn.BackColor = Color.FromArgb(255, 44, 44, 44);
                    }
                    else
                    {
                        mod.onEnable();
                        btn.BackColor = Color.FromArgb(255, 39, 39, 39);
                    }
                }
            }
        }

        private void panel3_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MouseDownLocation = e.Location;
                panel3.BringToFront();
            }
        }

        private void panel3_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                panel3.Left = e.X + panel3.Left - MouseDownLocation.X;
                panel3.Top = e.Y + panel3.Top - MouseDownLocation.Y;
            }
        }

        private void ClonableButton_Click(object sender, EventArgs e) { }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MouseDownLocation = e.Location;
                panel1.BringToFront();
            }
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                panel1.Left = e.X + panel1.Left - MouseDownLocation.X;
                panel1.Top = e.Y + panel1.Top - MouseDownLocation.Y;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void panel6_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MouseDownLocation = e.Location;
                panel6.BringToFront();
            }
        }

        private void panel6_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                panel6.Left = e.X + panel6.Left - MouseDownLocation.X;
                panel6.Top = e.Y + panel6.Top - MouseDownLocation.Y;
            }
        }

        private void panel14_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MouseDownLocation = e.Location;
                panel14.BringToFront();
            }
        }

        private void panel14_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                panel14.Left = e.X + panel14.Left - MouseDownLocation.X;
                panel14.Top = e.Y + panel14.Top - MouseDownLocation.Y;
            }
        }

        private void panel8_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MouseDownLocation = e.Location;
                panel8.BringToFront();
            }
        }

        private void panel8_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                panel8.Left = e.X + panel8.Left - MouseDownLocation.X;
                panel8.Top = e.Y + panel8.Top - MouseDownLocation.Y;
            }
        }

        private void panel10_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MouseDownLocation = e.Location;
                panel10.BringToFront();
            }
        }

        private void panel10_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                panel10.Left = e.X + panel10.Left - MouseDownLocation.X;
                panel10.Top = e.Y + panel10.Top - MouseDownLocation.Y;
            }
        }

        private void panel12_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MouseDownLocation = e.Location;
                panel12.BringToFront();
            }
        }

        private void panel12_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                panel12.Left = e.X + panel12.Left - MouseDownLocation.X;
                panel12.Top = e.Y + panel12.Top - MouseDownLocation.Y;
            }
        }

        private void panel16_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MouseDownLocation = e.Location;
                panel16.BringToFront();
            }
        }

        private void panel16_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                panel16.Left = e.X + panel16.Left - MouseDownLocation.X;
                panel16.Top = e.Y + panel16.Top - MouseDownLocation.Y;
            }
        }
    }
}
