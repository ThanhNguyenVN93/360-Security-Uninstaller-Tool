using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace frm360security
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            bool createdNew;
            using (new Mutex(true, "360SecurityUninstallerTool_SingleInstance", out createdNew))
            {
                if (!createdNew)
                {
                    MessageBox.Show(
                        "360 Uninstaller Tool is already running.",
                        "Already Running",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }
                Kill360Processes();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
        }

        internal static void Kill360Processes()
        {
            string[] procs = { "360Tray", "360Safe", "360sd", "360rps", "360rp",
                "QHSafeMain", "QHActiveDefense", "QHSafeTray", "QHWatchdog",
                "360leakfixer", "360TSLiveUpd8", "360TsLiveUpd",
                "LiveUpdate360", "PopWndLog" };
            foreach (string name in procs)
            {
                try
                {
                    foreach (Process p in Process.GetProcessesByName(name))
                    {
                        try { p.Kill(); } catch { }
                    }
                }
                catch { }
            }
        }
    }
}
