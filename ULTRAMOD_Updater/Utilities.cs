/************************************************************************************
____________________________ULTRAMOD UPDATER Project__________________________******
***********************************************************************************
**
** - Name        : Utilities.cs Code
** - Description : Extra functions
** - Author      : Hosseinpourziyaie
** - Note        : -----------------
** - Started on  : N/A        | Ended on : N/A
**
** [Copyright © ULTRAMOD/Hosseinpourziyaie 2019] <hosseinpourziyaie@gmail.com>
**
************************************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows;

namespace ULTRAMOD_Updater
{
    class Utilities
    {
        public static string ReadNickName()
        {
            if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "\\ULTRA_MW3.ini"))
            {
                try
                {
                    var MyIni = new IniFile(System.AppDomain.CurrentDomain.BaseDirectory + "\\ULTRA_MW3.ini");
                    string Nickname = MyIni.Read("Name", "Settings");
                    return Nickname /*+ " (" + Environment.UserName + ")"*/;
                }
                catch (Exception)
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        public static string Parse_HWID()
        {
            string vol = ExecuteCommand("vol");
            string dash = "-";
            int index = vol.IndexOf(dash);
            string hwid = vol.Substring(index - 4).Trim();
            return hwid;
        }
        public static string ExecuteCommand(string command)
        {
            ProcessStartInfo info = new ProcessStartInfo("cmd", "/c " + command)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            Process process = new Process
            {
                StartInfo = info
            };
            process.Start();
            return process.StandardOutput.ReadToEnd();
        }

        public static string FetchMacId()
        {
            string macAddresses = "";

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    macAddresses += nic.GetPhysicalAddress().ToString();
                    break;
                }
            }
            return macAddresses;
        }

        public static string ParseMacId(string RawMacID)
        {

            for (int i = 2; i <= RawMacID.Length; i += 2)
            {
                RawMacID = RawMacID.Insert(i, "-");
                i++;
            }
            return RawMacID;
        }
        public static string GetCPUID()
        {
            var mbs = new ManagementObjectSearcher("Select ProcessorId From Win32_processor");
            ManagementObjectCollection mbsList = mbs.Get();
            string id = "";
            foreach (ManagementObject mo in mbsList)
            {
                id = mo["ProcessorId"].ToString();
                break;
            }
            return id;
        }
        public static string GetWindows()
        {
            var name = (from x in new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem").Get().Cast<ManagementObject>()
                        select x.GetPropertyValue("Caption")).FirstOrDefault();
            return name != null ? name.ToString() : "Unknown";
        }
    }
}
