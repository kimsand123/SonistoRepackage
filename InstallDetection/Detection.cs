using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Security.AccessControl;
using System.Security;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace SonistoRepackage.InstallDetection
{
    public class Detection
    {

        bool startRecord = false;
        List<String> eventList = new List<string>();


        Dictionary<int, WatchedElement> watchedList = new Dictionary<int, WatchedElement>();

        public Detection()
        {
            //ConsoleManager.Show();
        }
        public void InstanceMethod()
        {
            startRecord = true;
            var drives2 = DriveInfo.GetDrives();
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.InternalBufferSize = 65536;
            //Setting up the watcher for each fixed drive
            foreach (DriveInfo drive in drives2)
            {
                //Getting the fixed drives only. Not network drives
                if (drive.DriveType == DriveType.Fixed)
                {
                    // look for drive and directories, 
                    watcher.Path = drive.Name;
                    watcher.IncludeSubdirectories = true;
                    watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                    watcher.Filter = "*.*";

                    //Eventhandlers being added 
                    watcher.Changed += new FileSystemEventHandler(OnChanged);
                    watcher.Created += new FileSystemEventHandler(OnChanged);
                    watcher.Deleted += new FileSystemEventHandler(OnChanged);
                    watcher.Renamed += new RenamedEventHandler(OnRenamed);

                    //Start watching
                    watcher.EnableRaisingEvents = true;
                }
            }
            while (startRecord) { }
            watcher.EnableRaisingEvents = false;
        }

        public List<String> getEventList()
        {
            return eventList;
        }

        public void stop()
        {
            startRecord = false;
        }
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            //reference
            //https://stackoverflow.com/questions/40449973/how-to-modify-file-access-control-in-net-core


            //string user = Environment.UserName;
            // WindowsPrincipal myPrincipal = (WindowsPrincipal)Thread.CurrentPrincipal;
            WatchedElement watchedElement = new WatchedElement();
            //Test
            string owner = "";
            try
            {
                owner = File.GetAccessControl(e.FullPath).GetOwner(typeof(SecurityIdentifier)).Translate(typeof(NTAccount)).ToString();
                if (owner.Contains("BUILTIN\\Administratorer"))
                //if (owner.Contains("test"))
                {
                    eventList.Add(">" + e.FullPath + "<" + e.ChangeType + ":" + owner);
                }
            }
            catch (Exception ex)
            {
            }
            
        }
        private void OnRenamed(object source, RenamedEventArgs e)
        {
            //When renaming there will always be two entries in the dictionary
            //first the oldpath, after that the new path.

            string owner = "";
            try
            {
                owner = File.GetAccessControl(e.FullPath).GetOwner(typeof(SecurityIdentifier)).Translate(typeof(NTAccount)).ToString();
                if (owner.Contains("BUILTIN\\Administratorer"))
                //if (owner.Contains("test"))
                {
                    eventList.Add(">" + e.OldFullPath + "<" + e.ChangeType + ":" + owner);
                    eventList.Add(">" + e.FullPath + "<" + e.ChangeType + ":" + owner);
                }
            }
            catch (Exception ex)
            {
            }
        }
        public Dictionary<int, WatchedElement> getWatchedElements()
        {
            return watchedList;
        }



        //Console window for WPF clients
        [SuppressUnmanagedCodeSecurity]
        public static class ConsoleManager
        {
            private const string Kernel32_DllName = "kernel32.dll";

            [DllImport(Kernel32_DllName)]
            private static extern bool AllocConsole();

            [DllImport(Kernel32_DllName)]
            private static extern bool FreeConsole();

            [DllImport(Kernel32_DllName)]
            private static extern IntPtr GetConsoleWindow();

            [DllImport(Kernel32_DllName)]
            private static extern int GetConsoleOutputCP();

            public static bool HasConsole
            {
                get { return GetConsoleWindow() != IntPtr.Zero; }
            }

            /// <summary>
            /// Creates a new console instance if the process is not attached to a console already.
            /// </summary>
            public static void Show()
            {
                //#if DEBUG
                if (!HasConsole)
                {
                    AllocConsole();
                    InvalidateOutAndError();
                }
                //#endif
            }

            /// <summary>
            /// If the process has a console attached to it, it will be detached and no longer visible. Writing to the System.Console is still possible, but no output will be shown.
            /// </summary>
            public static void Hide()
            {
                //#if DEBUG
                if (HasConsole)
                {
                    SetOutAndErrorNull();
                    FreeConsole();
                }
                //#endif
            }

            public static void Toggle()
            {
                if (HasConsole)
                {
                    Hide();
                }
                else
                {
                    Show();
                }
            }

            static void InvalidateOutAndError()
            {
                Type type = typeof(System.Console);

                System.Reflection.FieldInfo _out = type.GetField("_out",
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

                System.Reflection.FieldInfo _error = type.GetField("_error",
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

                System.Reflection.MethodInfo _InitializeStdOutError = type.GetMethod("InitializeStdOutError",
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

                Debug.Assert(_out != null);
                Debug.Assert(_error != null);

                Debug.Assert(_InitializeStdOutError != null);

                _out.SetValue(null, null);
                _error.SetValue(null, null);

                _InitializeStdOutError.Invoke(null, new object[] { true });
            }

            static void SetOutAndErrorNull()
            {
                Console.SetOut(TextWriter.Null);
                Console.SetError(TextWriter.Null);
            }
        }


    }
}
