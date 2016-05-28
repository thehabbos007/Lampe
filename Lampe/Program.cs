using System;
using Phoenix;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace Lampe
{
    public class Program : Form
    {

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [STAThread]
        public static void Main()
        {
            Application.Run(new Program());
        }

        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;

        private static ws Socket = new ws();

        public Program()
        {
            // Create a simple tray menu with only one item.
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Exit", OnExit);
            trayMenu.MenuItems.Add("Send essage", SocketMessage);

            // Create a tray icon. In this example we use a
            // standard system icon for simplicity, but you
            // can of course use your own custom icon too.
            trayIcon = new NotifyIcon();
            trayIcon.Text = "Lampe";
            trayIcon.Icon = new Icon(SystemIcons.Application, 40, 40);

            // Add menu to tray icon and show it.
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar.

            base.OnLoad(e);
        }

        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void SocketMessage (object sender, EventArgs e)
        {
            bool createdNew;
            var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, "A29DE0E8-104E-4330-84D5-1A1C2422DD74", out createdNew);
            var signaled = false;

            if (!createdNew)
            {
                waitHandle.Set();
                return;
            }

            var timer = new System.Threading.Timer(OnTimerElapsed, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));

            // Wait if someone tells us to die or do every five seconds something else.
            do
            {
                signaled = waitHandle.WaitOne(TimeSpan.FromSeconds(5));
                Socket.ReadLoop(Socket.Channel, GetActiveWindowTitle());
                // ToDo: Something else if desired.
            } while (!signaled);
        }

        private void OnTimerElapsed(object state)
        {
            Console.WriteLine("Time elapsed");
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                // Release the icon resource.
                trayIcon.Dispose();
            }

            base.Dispose(isDisposing);
        }

        private string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }
    }
}