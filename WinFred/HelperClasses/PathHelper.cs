﻿using System;
using System.Linq;
using System.Runtime.InteropServices;
using Alphaleonis.Win32.Filesystem;

namespace James.HelperClasses
{
    public static class PathHelper
    {
        /// <summary>
        /// Fetches the file name of an providen path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFilename(string path)
        {
            for (int i = (path ?? "").Length - 2; i >= 0; i--)
            {
                if (path[i] == '\\')
                {
                    return path.Substring(i + 1);
                }
            }
            return path;
        }

        /// <summary>
        /// Returns online the path to the folder, of the given file path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFolderPath(string path)
        {
            for (int i = (path ?? "").Length - 2; i >= 0; i--)
            {
                if (path[i] == '\\')
                {
                    return path.Substring(0,i);
                }
            }
            return null;
        }

        /// <summary>
        /// Searchs through the Windows Paths location for an given executable to get the full path
        /// </summary>
        /// <param name="executableName"></param>
        /// <returns></returns>
        public static string GetFullPathOfExe(string executableName)
        {
            var path = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);
            return path.Split(';').Select(p => Path.Combine(p, executableName)).FirstOrDefault(File.Exists);
        }

        /// <summary>
        /// Opens the property window of the explorer.exe for a providen path
        /// </summary>
        /// <param name="path"></param>
        public static void OpenPathPropertyWindow(string path)
        {
            if (File.Exists(path) || Directory.Exists(path))
            {
                if (!ShowFileProperties(path))
                {
                    Console.WriteLine("Something went wrong...");
                }
            }
        }

        /// <summary>
        /// Gets the location of James
        /// </summary>
        /// <returns></returns>
        public static string GetLocationOfJames()
        {
            var folders = Directory.GetDirectories(Config.ConfigFolderLocation).Where(s => PathHelper.GetFilename(s).StartsWith("app-")).Select(s => new { path = s, version = new Version(PathHelper.GetFilename(s).Replace("app-", "")) });
            return folders.OrderByDescending(arg => arg.version).First().path;
        }

        #region P/Invoke for open explorer's file properties window
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO lpExecInfo);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHELLEXECUTEINFO
        {
            public int cbSize;
            public uint fMask;
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.LPTStr)] public string lpVerb;
            [MarshalAs(UnmanagedType.LPTStr)] public string lpFile;
            [MarshalAs(UnmanagedType.LPTStr)] public string lpParameters;
            [MarshalAs(UnmanagedType.LPTStr)] public string lpDirectory;
            public int nShow;
            public IntPtr hInstApp;
            public IntPtr lpIDList;
            [MarshalAs(UnmanagedType.LPTStr)] public string lpClass;
            public IntPtr hkeyClass;
            public uint dwHotKey;
            public IntPtr hIcon;
            public IntPtr hProcess;
        }

        private const int SW_SHOW = 5;
        private const uint SEE_MASK_INVOKEIDLIST = 12;

        private static bool ShowFileProperties(string Filename)
        {
            SHELLEXECUTEINFO info = new SHELLEXECUTEINFO();
            info.cbSize = Marshal.SizeOf(info);
            info.lpVerb = "properties";
            info.lpFile = Filename;
            info.nShow = SW_SHOW;
            info.fMask = SEE_MASK_INVOKEIDLIST;
            return ShellExecuteEx(ref info);
        }
        #endregion

        /// <summary>
        /// Calculates the fileExtension in an effective way
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetFileExtension(string filePath)
        {
            for (int i = (filePath??"").Length - 2; i >= 0 ; i--)
            {
                switch (filePath[i])
                {
                    case '.':
                        return filePath.Substring(i + 1);
                    case '\\':
                        return null;
                }
            }
            return null;
        }
    }
}