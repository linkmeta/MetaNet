using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MetaNet.Interfaces
{
    internal class ProcessInterface
    {
        public Process process;
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AttachConsole(uint dwProcessId);

        delegate Boolean ConsoleCtrlDelegate(CtrlTypes type);
        enum CtrlTypes : uint
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GenerateConsoleCtrlEvent(CtrlTypes dwCtrlEvent, uint dwProcessGroupId);

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate handler, bool add);
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool FreeConsole();
        public void StopProcess()
        {
            // It's impossible to be attached to 2 consoles at the same time,
            // so release the current one.

            try
            {
                uint pid = (uint)process.Id;
                FreeConsole();

                // This does not require the console window to be visible.
                if (AttachConsole(pid))
                {
                    // Disable Ctrl-C handling for our program
                    SetConsoleCtrlHandler(null, true);
                    GenerateConsoleCtrlEvent(CtrlTypes.CTRL_C_EVENT, 0);

                    // Must wait here. If we don't and re-enable Ctrl-C
                    // handling below too fast, we might terminate ourselves.
                    Thread.Sleep(500);

                    FreeConsole();

                    // Re-enable Ctrl-C handling or any subsequently started
                    // programs will inherit the disabled state.
                    SetConsoleCtrlHandler(null, false);

                    process.Kill();
                }
            }
            catch (Exception)
            {

            }

        }

        public async void StartProcess( string exePath, string parameters, DataReceivedEventHandler outputhandler)
        {

            try
            {
                process = new System.Diagnostics.Process();
                process.StartInfo.FileName = exePath;
                process.StartInfo.Arguments = parameters;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.OutputDataReceived += new DataReceivedEventHandler(outputhandler);
                process.ErrorDataReceived += new DataReceivedEventHandler(outputhandler);
 
            }
            catch (Exception ex)
            {

            }
        }
    }
}
