using System;
using System.IO;
using System.ServiceProcess;
using System.Diagnostics;

namespace CsharpFolderWatcher
{
    public class CSharpFolderWatcher : ServiceBase
    {
        public const string MyServiceName = "CsharpFolderWatcher";
        private FileSystemWatcher watcher = null;
        private string FileDropLocation = "C:\\temp\\watch_directory";
        private string FileToBeCopiedLocation = @"C:\Users\ngopisetty\Documents\temp\archive";
        protected void FileCreated(object sender, FileSystemEventArgs e)
        {
            if (Directory.Exists(e.FullPath))
            {
                if(e.ChangeType == WatcherChangeTypes.Changed)
                {

                }
                else if(e.ChangeType == WatcherChangeTypes.Created)
                {

                }
            }
            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                // if (Directory.Exists(e.FullPath))
                // a directory                            
                // else
                // a file               
            }
        }

        protected override void OnStart(string[] args)
        {
            this.ServiceName = MyServiceName;

            // Create a new FileSystemWatcher with the path
            //and text file filter
            FileSystemWatcher watcher = new FileSystemWatcher(FileDropLocation);

            //Watch for changes in LastAccess and LastWrite times, and
            //the renaming of files or directories.
            watcher.NotifyFilter = NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.FileName
                                 | NotifyFilters.DirectoryName;

            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);

            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }

        protected override void OnStop()
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();

            LogEvent("Monitoring Stopped");
        }

        void OnChanged(object sender, FileSystemEventArgs e)
        {
            string msg = string.Format("File {0} | {1}",
                                       e.FullPath, e.ChangeType);
            File.Copy(e.FullPath, FileToBeCopiedLocation);
            LogEvent(msg);
        }
        //This method is called when a file is created, changed, or deleted.
        //private static void OnChanged(object source, FileSystemEventArgs e)
        //{
        //    //Show that a file has been created, changed, or deleted.
        //    WatcherChangeTypes wct = e.ChangeType;
        //    Console.WriteLine("File {0} {1}", e.FullPath, wct.ToString());
        //}
        void OnRenamed(object sender, RenamedEventArgs e)
        {
            string log = string.Format("{0} | Renamed from {1}",
                                       e.FullPath, e.OldName);
            
            LogEvent(log);
        }
        private void LogEvent(string message)
        {
            string eventSource = "File Monitor Service";
            DateTime dt = new DateTime();
            dt = System.DateTime.UtcNow;
            message = dt.ToLocalTime() + ": " + message;

            EventLog.WriteEntry(eventSource, message);
        }
    }
}