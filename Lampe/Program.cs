using System;
using Phoenix;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Automation;
using System.Diagnostics;

namespace Lampe
{
    public class Lampe: Form
    {

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [STAThread]
        public static void Main()
        {
            Application.Run(new Lampe());
        }

        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;

        private static ws Socket = new ws();

        private AutomationFocusChangedEventHandler focusHandler = null;

        public Lampe()
        {
            // Create a simple tray menu with only one item.
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Exit", OnExit);
            trayMenu.MenuItems.Add("Send message 2", SocketMessage);


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

        private void SocketMessage(object sender, EventArgs e)
        {
            focusHandler = new AutomationFocusChangedEventHandler(OnFocusChange);
            Automation.AddAutomationFocusChangedEventHandler(focusHandler);
        }
        private void OnFocusChange(object src, AutomationFocusChangedEventArgs e)
        {
            // TODO Add event handling code.
            // The arguments tell you which elements have lost and received focus.
            AutomationElement element = src as AutomationElement;
            if (element != null)
            {

                int processId = element.Current.ProcessId;
                using (Process process = Process.GetProcessById(processId))
                {
                    if (db.Lookup(process.ProcessName))
                    {
                        //process.
                        Console.WriteLine(process.ProcessName);
                        Socket.ReadLoop(Socket.Channel, "yes");

                    }
                    else {
                        Socket.ReadLoop(Socket.Channel, "no");
                    }

                }
            }

        }

        public void UnsubscribeFocusChange()
        {
            if (focusHandler != null)
            {
                Automation.RemoveAutomationFocusChangedEventHandler(focusHandler);
            }
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

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                // Release the icon resource.
                trayIcon.Dispose();
            }

            base.Dispose(isDisposing);
        }

    }
}