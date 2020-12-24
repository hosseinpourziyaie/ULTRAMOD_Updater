/************************************************************************************
____________________________ULTRAMOD UPDATER Project__________________________******
***********************************************************************************
**
** - Name        : App.xaml Code
** - Description : Main Startup Point
** - Author      : Hosseinpourziyaie
** - Note        : -----------------
** - Started on  : N/A        | Ended on : N/A
**
** [Copyright © ULTRAMOD/Hosseinpourziyaie 2019] <hosseinpourziyaie@gmail.com>
**
************************************************************************************/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;

namespace ULTRAMOD_Updater
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [STAThread]
        public static void Main(string[] args)
        {
            //if (!File.Exists(@"main\iw_00.iwd"))//binkw32.dll , mss32.dll
            //{
            //    //MessageBox.Show("Some game library files could not be found. It could be you didn't put the updater files in your game folder. Also, you still need the game if you want to run ULTRAMOD.", "ULTRAMOD Updater", MessageBoxButton.OK, MessageBoxImage.Error);
            //    //return;
            //}

            var application = new App();
            application.InitializeComponent();
            application.Run();

            //var progFile = "iw5mp.exe";

            //if (!File.Exists(Environment.CurrentDirectory + @"\" + progFile))
            //{
            //    //MessageBox.Show(progFile + " does not exist.");
            //    return;
            //}

            //Process.Start(progFile);
        }

        private static void Killprocess(string p)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                Arguments = "/F /IM " + p,
                FileName = "taskkill",
                WindowStyle = ProcessWindowStyle.Hidden
            };
            Process.Start(startInfo);
        }
    }
}
