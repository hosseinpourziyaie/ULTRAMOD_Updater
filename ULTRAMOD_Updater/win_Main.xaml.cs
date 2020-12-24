/************************************************************************************
____________________________ULTRAMOD UPDATER Project__________________________******
***********************************************************************************
**
** - Name        : win_Main.xaml Code
** - Description : Main Updater form
** - Author      : Hosseinpourziyaie
** - Note        : -----------------
** - Started on  : N/A        | Ended on : N/A
**
** [Copyright © ULTRAMOD/Hosseinpourziyaie 2019] <hosseinpourziyaie@gmail.com>
**
************************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

using MahApps.Metro.Controls;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Security.Cryptography;
using System.Reflection;
using SevenZip.Compression;

using SimpleJSON;

namespace ULTRAMOD_Updater
{
    /// <summary>
    /// Interaction logic for win_Main.xaml
    /// </summary>
    public partial class win_Main : MetroWindow
    {
        private string version = "1.4.0";
        private BackgroundWorker worker = new BackgroundWorker();
        private string currentData = "";
        private string currentStatus = "";
        //private List<string> filesNeedUpdate = new List<string>();//OLD IW4PLAY METHUD
        private List<HashFile> filesNeedUpdate = new List<HashFile>();//OLD IW4PLAY METHUD
        private List<HashFile> filesUpdate = new List<HashFile>();
        protected bool Auth_Result = true;
        private bool dlError = false;
        private string UpdateHashList;
        private string ClientDownloadDirectory;
        private string logfile = "ULTRAMOD_UPDATER.log";
        private string HeadquartersURL = "http://ultramod.eu/ultramod/updater/updater_hq.php";
        private string AuthorizationURL = "http://ultramod.eu/ultramod/updater/updater_auth.php";
        private string NewUpdater_Link;
        System.Windows.Threading.DispatcherTimer NoConnection_Timer;
        protected bool ci_Visible = false;
        protected bool flag_error = false;

        private int MaxTimeoutTime = 5;
        private int TimedoutTimes = 0;

        /// <summary>
        /// Window Entry Point
        /// </summary>
        public win_Main()
        {
            InitializeComponent();
            //////////////////////////////////////////////////////////////
            this.btn_refresh.Visibility = Visibility.Hidden;
            this.btn_update_link.Visibility = Visibility.Hidden;
            this.btn_continue.Visibility = Visibility.Hidden;
            this.btn_continue_update.Visibility = Visibility.Hidden;
            this.btn_submit.Visibility = Visibility.Hidden;
            this.btn_clear.Visibility = Visibility.Hidden;
            this.btn_open_logfile.Visibility = Visibility.Hidden;
            //////////////////////////////////////////////////////////////
            this.lbl_Message.Visibility = Visibility.Hidden;
            this.lbl_Body_Message.Visibility = Visibility.Hidden;
            //////////////////////////////////////////////////////////////
            this.img_timeout.Visibility = Visibility.Hidden;
            this.img_banned.Visibility = Visibility.Hidden;
            this.img_update.Visibility = Visibility.Hidden;
            this.img_error.Visibility = Visibility.Hidden;
            this.img_success.Visibility = Visibility.Hidden;
            this.img_conf.Visibility = Visibility.Hidden;
            //////////////////////////////////////////////////////////////
            this.progressbar_download.Visibility = Visibility.Hidden;

            //this.Title = "IRAN REVOLTION IW5 - UPDATING ...";

            this.lbl_version.Content = "ULTRAMOD Updater " + version;

            NoConnection_Timer = new System.Windows.Threading.DispatcherTimer();
            NoConnection_Timer.Tick += NoConnection_Timer_Tick;
            NoConnection_Timer.Interval = new TimeSpan(0, 0, 0, 1);
        }

        /// <summary>
        /// Window Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void win_Main_Load(object sender, RoutedEventArgs e)
        {
            //StackPanel_Body_Container.Children.Add(lbl_Body_Message);
            this.lbl_status.Content = "Getting Authorization from ULTRAMOD Headquarters  ...";

            Thread Auth = new Thread(new ThreadStart(WebJoinRequest));
            Auth.Start();

            //Thread RequestDownloadOptions = new Thread(RequestDownloadOptionList);
            //RequestDownloadOptions.Start();

            //Thread PreLoader = new Thread(StartPreLoader);
            //PreLoader.Start();

            //while (Interrogation_mode)
            //{
            //    //I'd like the wait here
            //}
            //StartPreLoader();

            //if (WebJoinRequest() == false)
            //System.Environment.Exit(1);
        }

        private void UpdateProcessCompleted()
        {
            if (flag_error)
                return;

            CleanScreen();

            this.img_success.Visibility = Visibility.Visible;
            this.lbl_Body_Message.Visibility = Visibility.Visible;
            this.lbl_Body_Message.Content = "Your Game Updated Successfully! You may now Head to the game.";
            this.btn_cancel.Content = "Close";
            this.btn_cancel.Visibility = Visibility.Visible;
            //if (dlError == true || Auth_Result == false)
            //    Environment.Exit(1);
            //this.lbl_status.Content = "Starting the Game ...";
            //this.Close();
        }

        /// <summary>
        /// Start Updating Client Files
        /// </summary>
        private void StartPreLoader()
        {
            CleanScreen();

            this.progressbar_Loading_Efftect.Visibility = Visibility.Visible;
            this.progressbar_total.Visibility = Visibility.Visible;
            this.progressbar_download.Visibility = Visibility.Visible;
            this.lbl_status.Visibility = Visibility.Visible;
            this.lbl_data.Visibility = Visibility.Visible;

            this.lbl_status.Content = "Checking Reiterative Processes ...";
            CloseExtraProcess();
            this.lbl_status.Content = "Cleaning Game Folder Extras ...";
            CleanGameFolder();

            this.lbl_status.Content = "Getting Information Needed to Start Updater ...";
            //ObtainInformation();
            //this.lbl_status.Content = "Preparing ScreenShot System ...";

            this.worker = new BackgroundWorker();
            this.worker.WorkerSupportsCancellation = true;
            this.worker.WorkerReportsProgress = true;

            this.worker.DoWork += new DoWorkEventHandler(this.wCU_DoWork);
            this.worker.ProgressChanged += new ProgressChangedEventHandler(this.wCU_ProgressChanged);
            this.worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.wCU_RunWorkerCompleted);
            this.lbl_data.Content = "Checking for updates...";
            this.lbl_status.Content = "Checking for the Updates ...";
            this.worker.RunWorkerAsync();
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(1);
        }
        private void btn_update_link_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(NewUpdater_Link);
            this.Close();
        }
        private void btn_refresh_Click(object sender, RoutedEventArgs e)
        {
            NoConnection_Timer.Stop();
            CleanScreen();

            this.progressbar_download.Visibility = Visibility.Visible;
            this.progressbar_Loading_Efftect.Visibility = Visibility.Visible;
            this.progressbar_total.Visibility = Visibility.Visible;
            this.lbl_status.Visibility = Visibility.Visible;
            this.lbl_data.Visibility = Visibility.Visible;
            this.btn_cancel.Visibility = Visibility.Visible;

            flag_error = false;
            TimedoutTimes = 0;

            Thread Auth = new Thread(new ThreadStart(WebJoinRequest));
            Auth.Start();
        }

        /// <summary>
        /// Function To Log Events had Happened
        /// </summary>
        /// <param name="tmpMsg"></param>
        private void LogMessage(string tmpMsg)
        {
            try
            {
                if (!System.IO.File.Exists(logfile))
                {
                    System.IO.File.WriteAllText(logfile, "");
                }
                System.IO.File.WriteAllText(logfile, System.IO.File.ReadAllText(logfile) + "[" + DateTime.Now.ToString() + "] " + tmpMsg + "\r\n");
            }
            catch (Exception)
            {
                //throw;
            }
        }

        /// <summary>
        /// Function To Download Files as LZMA and Decompress
        /// </summary>
        /// <param name="tmpMsg"></param>
        private void DownloadLZMA(string filelink, string savepath)
        {
            //https://stackoverflow.com/questions/7646328/how-to-use-the-7z-sdk-to-compress-and-decompress-a-file
            //https://www.7-zip.org/sdk.html

            string tempfile = savepath + ".lzma";

            try
            {
                this.currentData = "Downloading " + tempfile + " [LZMA Compressed]";
                LogMessage("INFO: Downloading " + tempfile + " [LZMA Compressed]");

                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(filelink, tempfile);
                }

                LogMessage("INFO: Successfully downloaded " + tempfile + " [LZMA Compressed]");
                this.currentData = "Successfully downloaded " + tempfile + " [LZMA Compressed]";
            }
            catch (Exception e)
            {
                LogMessage(String.Format("ERROR: Failed to download LZMA file. {0}", e.ToString()));
                this.currentData = "Failed to download " + tempfile + " [LZMA Compressed]";
                this.dlError = true;
            }
            try
            {
                this.currentData = "Decompressing " + tempfile + " [LZMA Compressed]";
                LogMessage("INFO: Decompressing " + tempfile + " [LZMA Compressed]");

                SevenZip.Compression.LZMA.Decoder coder = new SevenZip.Compression.LZMA.Decoder();
                FileStream input = new FileStream(tempfile, FileMode.Open);
                FileStream output = new FileStream(savepath, FileMode.Create);

                // Read the decoder properties
                byte[] properties = new byte[5];
                input.Read(properties, 0, 5);

                // Read in the decompress file size.
                byte[] fileLengthBytes = new byte[8];
                input.Read(fileLengthBytes, 0, 8);
                long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

                coder.SetDecoderProperties(properties);
                coder.Code(input, output, input.Length, fileLength, null);
                output.Flush();
                output.Close();

                input.Close();

                File.Delete(tempfile);

                LogMessage("INFO: Decompressed " + tempfile + " Successfully [LZMA Compressed]");
                this.currentData = "INFO: Decompressed " + tempfile + " Successfully [LZMA Compressed]";

                this.currentData = " " + tempfile + " [LZMA Compressed]";
                LogMessage("INFO: Decompressing " + tempfile + " [LZMA Compressed]");

            }
            catch (Exception e)
            {
                LogMessage(String.Format("ERROR: Failed to Decompress LZMA file {0} . {1}", tempfile, e.ToString()));
                this.currentData = "Failed to Decompress LZMA file " + tempfile + " [LZMA Compressed]";
                this.dlError = true;
            }
        }

        /// <summary>
        /// Step 1 of Updater
        /// Get Update HashList from Web BackGroundWorker
        ///  Gets Updater HashList from WebServer and Parse its to list of 
        ///   Files Inforamtion Contains Name , Directory , MD5
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wCU_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            if (worker.CancellationPending)
            {
                e.Cancel = true;
            }
            else
            {
                LogMessage("INFO: Parsing Downloaded HashList...");
                this.currentData = "Parsing Downloaded HashList...";
                worker.ReportProgress(0);
                try
                {
                    string[] strArray = Regex.Split(UpdateHashList, "\n");
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        var ContentExpanded = Regex.Replace(strArray[i], @"\s+", "").Split('*');

                        if (ContentExpanded.Length < 2)
                            continue;

                        bool isLzma = false;
                        string DiffrentDownloadLink = "";

                        if (ContentExpanded.Length > 2 && ContentExpanded[2].ToLower() == "lzma")
                            isLzma = true;

                        if (ContentExpanded.Length > 3 && !string.IsNullOrWhiteSpace(ContentExpanded[3]))
                            DiffrentDownloadLink = ContentExpanded[3];


                        HashFile item = new HashFile
                        {
                            md5Hash = ContentExpanded[0],
                            fileName = ContentExpanded[1],
                            LZMA = isLzma,
                            Link = DiffrentDownloadLink
                        };
                        this.filesUpdate.Add(item);

                        //if (strArray[i].Contains(" *"))
                        //{
                        //    string[] strArray2 = new string[2];
                        //    strArray2[1] = strArray[i].Substring(34);
                        //    strArray2[0] = strArray[i].Substring(0, 32);
                        //    HashFile item = new HashFile
                        //    {
                        //        md5Hash = strArray2[0],
                        //        fileName = strArray2[1].Trim()
                        //    };
                        //    this.filesUpdate.Add(item);
                        //}
                    }
                    LogMessage("INFO: Hashlist parsed successfully.");
                    this.currentData = "Hashlist Parsed Successfully";
                    //progressbar_download.Maximum = filesUpdate.Count;
                }
                catch (Exception ex)
                {
                    Thread FlagScriptError = new Thread(new ThreadStart(FlagAsScriptError));
                    FlagScriptError.Start();
                    Dispatcher.Invoke(new Action(() => { lbl_Body_Message.Content = "Error on Parsing HashList : " + ex.ToString(); }));
                }
            }
        }

        private void wCU_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.lbl_data.Content = this.currentData;
            //this.currentData = "";
            this.lbl_status.Content = this.currentStatus;
            //this.currentStatus = "";
        }

        private void wCU_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && (e.Error == null))
            {
                this.worker = new BackgroundWorker();
                this.worker.WorkerSupportsCancellation = true;
                this.worker.WorkerReportsProgress = true;
                this.worker.DoWork += new DoWorkEventHandler(this.wCH_DoWork);
                this.worker.ProgressChanged += new ProgressChangedEventHandler(this.wCH_ProgressChanged);
                this.worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.wCH_RunWorkerCompleted);
                if (!this.worker.IsBusy)
                {
                    this.worker.RunWorkerAsync();
                }
                this.lbl_data.Content = "Comparing the files...";
            }
        }

        /// <summary>
        /// Step 2 of Updater
        /// Check Hashes BackGroundWorker
        /// this Function Compares Updater DownloadList Files Hashes with
        ///   Files Already Exists to Decide wich Files We Need to Download 
        ///    also if file not Exsists it will too :D
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wCH_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            if (worker.CancellationPending)
            {
                e.Cancel = true;
            }
            else
            {
                string str = "";
                LogMessage("INFO: comparing hashes...");
                for (int i = 0; i < this.filesUpdate.Count; i++)
                {
                    HashFile file = this.filesUpdate.ElementAt<HashFile>(i);
                    this.filesNeedUpdate.Add(file);
                    //this.filesNeedUpdate.Add(file.fileName);//OLD IW4PLAY METHUD
                    this.currentData = "Comparing " + file.fileName;
                    worker.ReportProgress(0);
                    if (System.IO.File.Exists(file.fileName))
                    {
                        try
                        {
                            FileStream inputStream = System.IO.File.OpenRead(str + file.fileName);
                            //inputStream.Close(); //Not Shoud be here .Maybe Reflector Fault 
                            //Dispatcher.Invoke(new Action(() => { MessageBox.Show(file.md5Hash.ToLower() + " \nWeb MD5"); }));
                            //Dispatcher.Invoke(new Action(() => { MessageBox.Show(BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(inputStream)).Replace("-", "").ToLower() + "\nGet MD5"); }));
                            if (BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(inputStream)).Replace("-", "").ToLower() == file.md5Hash.ToLower())
                            {
                                this.filesNeedUpdate.Remove(file);
                                //this.filesNeedUpdate.Remove(file.fileName);//OLD IW4PLAY METHUD 
                                //Dispatcher.Invoke(new Action(() => { MessageBox.Show(file.fileName + " Deleted from list"); }));
                            }
                            inputStream.Close();
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                Dispatcher.Invoke(new Action(() => { progressbar_download.Maximum = filesNeedUpdate.Count + 1; }));
            }
        }

        private void wCH_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.lbl_data.Content = this.currentData;
            //this.currentData = "";
            this.lbl_status.Content = this.currentStatus;
            //this.currentStatus = "";
        }

        private void wCH_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && (e.Error == null))
            {
                this.worker = new BackgroundWorker();
                this.worker.WorkerSupportsCancellation = true;
                this.worker.WorkerReportsProgress = true;
                this.worker.DoWork += new DoWorkEventHandler(this.wUF_DoWork);
                this.worker.ProgressChanged += new ProgressChangedEventHandler(this.wUF_ProgressChanged);
                this.worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.wUF_RunWorkerCompleted);
                this.lbl_data.Content = "Updating the files...";
                if (!this.worker.IsBusy)
                {
                    this.worker.RunWorkerAsync();
                }
            }
        }

        /// <summary>
        /// Step 3 of Updater
        /// Start Updating Needed Files BackGroundWorker
        /// this Function Download Client Files from Web
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wUF_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            //###############################################################################################
            //###############################################################################################
            if (worker.CancellationPending)
            {
                e.Cancel = true;
            }
            else if (this.filesNeedUpdate.Count == 0)
            {
                LogMessage("INFO: Game is up to date");
                this.currentStatus = "Client Files is Already Latest Version.";
                this.currentData = "Game is up to date";
                worker.ReportProgress(0);
            }
            else
            {
                LogMessage("INFO: Game needs an update");
                this.currentStatus = "Updating the Game Files...";
                this.currentData = "The game needs an update";
                worker.ReportProgress(0);
                WebClient client2 = new WebClient();
                for (int i = 0; i < this.filesNeedUpdate.Count; i++)
                {
                    HashFile file = this.filesNeedUpdate.ElementAt<HashFile>(i);

                    string input = file.fileName;
                    string str2 = Regex.Replace(input, @"\\", "/");
                    if (file.LZMA)
                    {
                        this.currentData = "Downloading " + str2 + " [LZMA Compressed]";
                        LogMessage("INFO: Downloading " + str2 + " [LZMA Compressed]");
                    }
                    else
                    {
                        this.currentData = "Downloading " + str2;
                        LogMessage("INFO: Downloading " + str2);
                    }

                    this.currentStatus = i + "/" + filesNeedUpdate.Count + " of Files Downloaded.";
                    Dispatcher.Invoke(new Action(() => { progressbar_download.Value = i + 1; }));

                    worker.ReportProgress(0);
                    try
                    {
                        new FileInfo(Environment.CurrentDirectory + @"\\" + input).Directory.Create();

                        if (System.IO.File.Exists(input + ".lzma"))
                        {
                            System.IO.File.Delete(input + ".lzma");
                        }


                        if (System.IO.File.Exists(input))
                        {
                            System.IO.File.Delete(input);
                        }

                        Thread.Sleep(50);

                        //var input_parse = input.Split('.');
                        //string file_type = input_parse[input_parse.Length - 1];

                        //if (file_type == "lzma")
                        //{
                        //    DownloadLZMA(ClientDownloadDirectory + str2, input);
                        //}
                        //else
                        //    client2.DownloadFile(ClientDownloadDirectory + str2, input);

                        if (!string.IsNullOrWhiteSpace(file.Link))
                        {
                            if (file.LZMA)
                                DownloadLZMA(file.Link, input);
                            else
                                client2.DownloadFile(file.Link, input);
                        }
                        else
                        {
                            if (file.LZMA)
                                DownloadLZMA(ClientDownloadDirectory + str2 + ".lzma", input);
                            else
                                client2.DownloadFile(ClientDownloadDirectory + str2, input);
                        }

                        LogMessage("INFO: successfully downloaded " + str2);
                        //this.currentData = "Successfully downloaded " + str2;

                        worker.ReportProgress(0);
                    }
                    catch (Exception ex)
                    {
                        this.dlError = true;
                        LogMessage(String.Format("ERROR: Problem on downloading {0} process. {1}", str2, ex.ToString()));
                        this.currentData = "Failed downloading " + str2;
                        worker.ReportProgress(0);
                    }
                }
            }
        }

        private void wUF_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //MessageBox.Show(currentData);

            this.lbl_data.Content = this.currentData;
            //IDK why but it seems to be problem with multi-threading that label fades right after setContent
            //this.currentData = "";
            this.lbl_status.Content = this.currentStatus;
            //this.currentStatus = "";
        }

        private void wUF_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && (e.Error == null))
            {
                if (this.dlError)
                {
                    //this.lbl_update.Content = "Error on Updating the Game.";
                    //MessageBox.Show("There were problems updating your game. Please check your " + logfile, "Error", MessageBoxButton.OK, MessageBoxImage.Hand);

                    Thread FlagScriptError = new Thread(new ThreadStart(FlagAsScriptError));
                    FlagScriptError.Start();
                    this.lbl_Body_Message.Content = "There were problems updating your game.\n Please check your " + logfile;
                    //Dispatcher.Invoke(new Action(() => { this.btn_open_logfile.Visibility = Visibility.Visible; }));           

                    // Environment.Exit(1);
                }
                else
                {
                    this.lbl_data.Content = "Game is up to date.";
                    this.worker = null;
                    UpdateProcessCompleted();
                }
                //this.worker = null;
            }
        }

        /// <summary>
        /// Function that Cleans Game Root From some Trashes
        /// </summary>
        private void CleanGameFolder()
        {
            try
            {
                if (File.Exists(logfile))
                    File.Delete(logfile);
            }
            catch (Exception)
            {
            }
            try
            {
                var files = Directory.GetFiles(Environment.CurrentDirectory, "*.exe", SearchOption.TopDirectoryOnly);
                foreach (var file in files)
                {
                    if (Regex.Match(file.Split('.')[0], "([0-9A-F]{7,10})").Success)
                    {
                        File.Delete(file);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private bool processIsRunning(string process)
        {
            return (System.Diagnostics.Process.GetProcessesByName(process).Length != 0);
        }

        private void ProcessKill(string p)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                Arguments = "/F /IM " + p,
                FileName = "taskkill",
                WindowStyle = ProcessWindowStyle.Hidden
            };
            Process.Start(startInfo);
        }

        private void CloseExtraProcess()
        {

            /*if (processIsRunning("iw5mp"))
            {
                foreach (var process in Process.GetProcessesByName("iw5mp"))
                {
                    process.Kill();
                }
            }

             if (processIsRunning("iw4mp"))
             {
                 foreach (var process in Process.GetProcessesByName("iw4mp"))
                 {
                     process.Kill();
                 }
             }
             */
        }

        bool wait_for_me = false;
        /// <summary>
        /// Get Ticket from WebServer Authorization System
        /// </summary>
        private void WebJoinRequest()
        {
            try
            {
                string Encryption_Key = "k#n%dk222+62#nhsga8814y3hd=175s1";
                string Encryption_iv = "512453forhja32@ht^4sta127hlj4^2a";
                string UserName = Encryption.Encrypt(Environment.UserName, Encryption_Key, Encryption_iv);
                string PlayerNickName = Encryption.Encrypt(Utilities.ReadNickName(), Encryption_Key, Encryption_iv);
                string Windows = Encryption.Encrypt(Utilities.GetWindows(), Encryption_Key, Encryption_iv);
                string HIWD = Encryption.Encrypt(Utilities.Parse_HWID(), Encryption_Key, Encryption_iv);
                string MacID = Encryption.Encrypt(Utilities.ParseMacId(Utilities.FetchMacId()), Encryption_Key, Encryption_iv);
                string Version = Encryption.Encrypt(version, Encryption_Key, Encryption_iv);
                string UID = Encryption.Encrypt(FingerPrint.Value(), Encryption_Key, Encryption_iv);
                ASCIIEncoding encoding = new ASCIIEncoding();
                //string postData = "user=" + Environment.UserName + "&win=" + GetWindows() + "&cpuid=" + GetCPUID() + "&macid=" + ParseMacId(FetchMacId()) + "&guid=" + GetMachineGuid();
                string postData = "a=" + UserName + "&b=" + PlayerNickName + "&c=" + Windows + "&d=" + HIWD + "&e=" + UID + "&f=" + MacID + "&g=" + Version;
                byte[] data = encoding.GetBytes(postData);

                WebRequest Request = WebRequest.Create(AuthorizationURL/*"http://localhost/test_hq_req.php"*/);

                Request.Method = "POST";
                Request.ContentType = "application/x-www-form-urlencoded";
                Request.ContentLength = data.Length;

                Stream stream = Request.GetRequestStream();
                stream.Write(data, 0, data.Length);
                stream.Close();

                WebResponse Response = Request.GetResponse();
                stream = Response.GetResponseStream();

                StreamReader sr = new StreamReader(stream);
                string ResponseText = sr.ReadToEnd();

                sr.Close();
                stream.Close();

                var ResponseContent = ResponseText.Split(';');

                if (ResponseContent[0] == "banned")
                {
                    Auth_Result = false;
                    Dispatcher.Invoke(new Action(() =>
                    {
                        CleanScreen();
                        this.lbl_Body_Message.Content = "You Have been banned from ULTRAMOD \nReason: " + ResponseContent[1] + "\nBanID: " + ResponseContent[2] + "\nYou can Appeal to Unban in ULTRAMOD Discord Server";
                        this.lbl_Body_Message.Visibility = Visibility.Visible;
                        this.btn_cancel.Content = "Close";
                        this.btn_cancel.Visibility = Visibility.Visible;
                        this.img_banned.Visibility = Visibility.Visible;

                    }));
                }
                else if (ResponseContent[0] == "update")
                {
                    LogMessage("INFO: Launcher new optional update exists");

                    Auth_Result = true;
                    wait_for_me = true;

                    Dispatcher.Invoke(new Action(() =>
                    {
                        CleanScreen();
                        this.currentData = "The launcher needs an update";
                        this.lbl_Body_Message.Content = "ULTRAMOD Updater New Version ( " + ResponseContent[2] + " ) Available\nYou Can Download it Manualy from Link below. Also You can Continue with current version";
                        NewUpdater_Link = ResponseContent[1];
                        this.btn_update_link.Visibility = Visibility.Visible;
                        this.btn_continue_update.Visibility = Visibility.Visible;
                        this.lbl_Body_Message.Visibility = Visibility.Visible;
                        this.img_update.Visibility = Visibility.Visible;
                    }));
                }
                else if (ResponseContent[0] == "force_update")
                {
                    LogMessage("INFO: Launcher needs an update");

                    Dispatcher.Invoke(new Action(() =>
                    {
                        CleanScreen();
                        this.currentData = "The launcher needs an update";
                        this.lbl_Body_Message.Content = "ULTRAMOD Updater New Version ( " + ResponseContent[2] + " ) Required\nYou Can Download it Manualy With below Links";
                        NewUpdater_Link = ResponseContent[1];
                        this.btn_update_link.Visibility = Visibility.Visible;
                        this.lbl_Body_Message.Visibility = Visibility.Visible;
                        this.img_update.Visibility = Visibility.Visible;
                    }));
                    Auth_Result = false;
                }
                else if (ResponseContent[0] == "messagebox")
                {
                    MessageBox.Show(ResponseContent[1], "ULTRAMOD", MessageBoxButton.OK, MessageBoxImage.Information);
                    Auth_Result = true;
                }
                else if (ResponseContent[0] == "message")
                {
                    Auth_Result = true;
                    wait_for_me = true;
                    Dispatcher.Invoke(new Action(() =>
                    {
                        CleanScreen();
                        this.lbl_Message.Content = ResponseContent[1];
                        this.lbl_Message.Visibility = Visibility.Visible;
                        //this.btn_continue.Content = "Continue";
                        this.btn_continue.Visibility = Visibility.Visible;
                    }));
                }
                else if (ResponseContent[0] == "closemessage")
                {
                    //MessageBox.Show(ResponseContent[1], "ULTRAMW3", MessageBoxButton.OK, MessageBoxImage.Information);
                    //System.Environment.Exit(1);
                    Dispatcher.Invoke(new Action(() =>
                    {
                        Auth_Result = false;
                        CleanScreen();
                        this.lbl_Message.Content = ResponseContent[1];
                        this.lbl_Message.Visibility = Visibility.Visible;
                        this.btn_cancel.Content = "Close";
                        this.btn_cancel.Visibility = Visibility.Visible;

                    }));
                }
                else if (ResponseContent[0] == "advertisment")
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        win_Adv AdvertismentForm = new win_Adv(this, ResponseContent[1], Double.Parse(ResponseContent[2]));//Convert.ToInt16(ResponseContent[2])
                        this.Hide();
                        AdvertismentForm.Show();
                    }));

                    Auth_Result = true;
                }
                else if (ResponseContent[0] == "welcome" || ResponseContent[0] == "ok")
                {
                    Auth_Result = true;
                }
                else
                {
                    Auth_Result = false;
                    TimedoutTimes++;
                    if (TimedoutTimes <= MaxTimeoutTime)
                    {
                        Dispatcher.Invoke(new Action(() => { this.lbl_status.Content = "Re-Trying on connecting Authorization Server [" + TimedoutTimes + "/" + MaxTimeoutTime + "]  ..."; }));
                        Thread.Sleep(2000);
                        WebJoinRequest();
                    }
                    else
                    {
                        //MessageBox.Show(ex.ToString());
                        Thread FlagOffline = new Thread(new ThreadStart(FlagAsOffline));
                        FlagOffline.Start();
                    }
                }

                if (Auth_Result)
                {
                    while (wait_for_me)
                    {
                        //I'd like the wait here
                    }

                    Thread RequestDownloadOptions = new Thread(RequestDownloadOptionList);
                    RequestDownloadOptions.Start();
                }

            }
            catch (Exception ex)
            {
                TimedoutTimes++;
                if (TimedoutTimes <= MaxTimeoutTime)
                {
                    Dispatcher.Invoke(new Action(() => { this.lbl_status.Content = "Re-Trying on connecting Authorization Server [" + TimedoutTimes + "/" + MaxTimeoutTime + "]  ..."; }));
                    Thread.Sleep(2000);
                    WebJoinRequest();
                }
                else
                {
                    //MessageBox.Show(ex.ToString());
                    Thread FlagOffline = new Thread(new ThreadStart(FlagAsOffline));
                    FlagOffline.Start();
                }
            }
        }

        /// <summary>
        /// Defines No Connection UI
        /// </summary>
        private void FlagAsOffline()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                CleanScreen();

                this.img_timeout.Visibility = Visibility.Visible;
                ci_Visible = true;

                this.btn_refresh.Visibility = Visibility.Visible;
                this.lbl_Body_Message.Visibility = Visibility.Visible;
                this.lbl_Body_Message.Content = "Error : Can not Fetch Data from Headquarters Server at \"ultramod.eu\" ";

                NoConnection_Timer.Start();

            }));
        }
        /// <summary>
        /// Defines Error in Script
        /// </summary>
        private void FlagAsScriptError()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                NoConnection_Timer.Stop();
                CleanScreen();

                this.img_error.Visibility = Visibility.Visible;
                this.btn_open_logfile.Visibility = Visibility.Visible;
                this.btn_refresh.Visibility = Visibility.Visible;
                this.lbl_Body_Message.Visibility = Visibility.Visible;
                flag_error = true;
            }));
        }

        /// <summary>
        /// Clear All Static Controls from Form
        /// </summary>
        private void CleanScreen()
        {
            this.btn_update_link.Visibility = Visibility.Hidden;
            this.btn_cancel.Visibility = Visibility.Hidden;
            this.btn_submit.Visibility = Visibility.Hidden;
            this.btn_clear.Visibility = Visibility.Hidden;
            this.btn_continue.Visibility = Visibility.Hidden;
            this.btn_continue_update.Visibility = Visibility.Hidden;
            this.btn_refresh.Visibility = Visibility.Hidden;
            this.btn_open_logfile.Visibility = Visibility.Hidden;
            this.progressbar_download.Visibility = Visibility.Hidden;
            this.progressbar_Loading_Efftect.Visibility = Visibility.Hidden;
            this.progressbar_total.Visibility = Visibility.Hidden;
            this.lbl_status.Visibility = Visibility.Hidden;
            this.lbl_status_pos2.Visibility = Visibility.Hidden;
            this.lbl_data.Visibility = Visibility.Hidden;
            this.lbl_Message.Visibility = Visibility.Hidden;
            this.lbl_Body_Message.Visibility = Visibility.Hidden;
            this.img_timeout.Visibility = Visibility.Hidden;
            this.img_update.Visibility = Visibility.Hidden;
            this.img_error.Visibility = Visibility.Hidden;
            this.img_success.Visibility = Visibility.Hidden;
            this.img_conf.Visibility = Visibility.Hidden;
            this.img_banned.Visibility = Visibility.Hidden;
            StackPanel_Body_Container.Children.Clear();
            ci_Visible = false;
        }

        private void NoConnection_Timer_Tick(object sender, EventArgs e)
        {
            if (ci_Visible)
            {
                Dispatcher.Invoke(new Action(() => { this.img_timeout.Visibility = Visibility.Hidden; }));
                ci_Visible = false;
            }
            else
            {
                Dispatcher.Invoke(new Action(() => { this.img_timeout.Visibility = Visibility.Visible; }));
                ci_Visible = true;
            }
        }

        //https://github.com/zanders3/json
        //https://wiki.unity3d.com/index.php/SimpleJSON

        //https://stackoverflow.com/questions/2329978/the-calling-thread-must-be-sta-because-many-ui-components-require-this

        protected string DownloadJSONFile(string Link)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Link);
            request.Method = "GET";
            String UpdaterJSON = String.Empty;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                UpdaterJSON = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
            }
            return UpdaterJSON;
        }

        bool json_parse_pause = false;
        string Order_List_String;
        List<string> Order_List = new List<string>();
        //bool Interrogation_mode = true;

        //now its declared from outside of for loop so hopefully we can reset it to 0 for restart
        int formIndex = 0;

        /// <summary>
        /// Obtain Some Needed Dynamic Links Addresses
        /// </summary>
        protected void RequestDownloadOptionList()
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                CleanScreen();
                this.lbl_status_pos2.Visibility = Visibility.Visible;
                this.img_conf.Visibility = Visibility.Visible;
                this.btn_submit.Visibility = Visibility.Visible;
                this.btn_clear.Visibility = Visibility.Hidden;
            });

            string UpdaterJSON = "";

            try
            {
                //Thread.Sleep(100);
                WebClient client = new WebClient();
                UpdaterJSON = client.DownloadString(HeadquartersURL);
                //client.Dispose();

                //UpdaterJSON = DownloadJSONFile(HeadquartersURL);

                //MessageBox.Show(UpdaterJSON);
            }
            catch (Exception ex)
            {
                TimedoutTimes++;
                if (TimedoutTimes <= MaxTimeoutTime)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        this.lbl_status_pos2.Content = "re-trying to obtain json information from server [" + TimedoutTimes + "/" + MaxTimeoutTime + "]  ...";
                    }));
                    RequestDownloadOptionList();
                }
                else
                {
                    Thread FlagScriptError = new Thread(new ThreadStart(FlagAsScriptError));
                    FlagScriptError.Start();
                    Dispatcher.Invoke(new Action(() => { lbl_Body_Message.Content = "Error on obtainning JSON INFORMATION from Updater Headquarters\nConnection Issue Occurred : " + ex.ToString(); }));
                    //Thread FlagOffline = new Thread(new ThreadStart(FlagAsOffline));
                    //FlagOffline.Start();
                }
                return;
            }

            //MessageBox.Show(UpdaterJSON, "2");
            var RESPONSEJSON = JSON.Parse(UpdaterJSON);

            //try
            //{
            //    if (string.IsNullOrEmpty(RESPONSEJSON))
            //    {
            //        Thread FlagScriptError = new Thread(new ThreadStart(FlagAsScriptError));
            //        FlagScriptError.Start();
            //        Dispatcher.Invoke(new Action(() => { lbl_Body_Message.Content = "Error on obtainning JSON INFORMATION from Updater Headquarters\nLikely Connection Issue Occurred."; }));
            //        return;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Thread FlagScriptError = new Thread(new ThreadStart(FlagAsScriptError));
            //    FlagScriptError.Start();
            //    Dispatcher.Invoke(new Action(() => { lbl_Body_Message.Content = "Error Occurred : " + ex.ToString(); }));
            //    return;
            //}

            try
            {
                if (RESPONSEJSON["status"].Value == "generated")
                {
                    ClientDownloadDirectory = RESPONSEJSON["dl_dir"].Value;
                    UpdateHashList = RESPONSEJSON["hash_list"].Value;

                    LogMessage("INFO: HashList successfully downloaded");
                    this.currentData = "HashList downloaded.";

                    //MessageBox.Show(DownloadList, "ULTRAMOD UPDATER", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }
            catch (Exception ex)
            {
                Thread FlagScriptError = new Thread(new ThreadStart(FlagAsScriptError));
                FlagScriptError.Start();
                Dispatcher.Invoke(new Action(() => { lbl_Body_Message.Content = "Error Occurred : " + ex.ToString(); }));
                return;
            }

            try
            {
                if (RESPONSEJSON["status"].Value != "success")
                {
                    Thread FlagScriptError = new Thread(new ThreadStart(FlagAsScriptError));
                    FlagScriptError.Start();
                    Dispatcher.Invoke(new Action(() => { lbl_Body_Message.Content = "JSON INFORMATION Obtained from Updater Headquarters\nis Corrupted"; }));
                    return;
                }

            }
            catch (Exception ex)
            {
                Thread FlagScriptError = new Thread(new ThreadStart(FlagAsScriptError));
                FlagScriptError.Start();
                Dispatcher.Invoke(new Action(() => { lbl_Body_Message.Content = "Error Occurred : " + ex.ToString(); }));
                throw;
            }

            try
            {
                //for (int i = 0; string.IsNullOrEmpty(RESPONSEJSON["options"][i]["name"].Value) == false; i++)
                for (; string.IsNullOrEmpty(RESPONSEJSON["options"][formIndex]["name"].Value) == false; formIndex++)
                {
                    if (!string.IsNullOrEmpty(RESPONSEJSON["options"][formIndex]["requires"].Value) && !Order_List.Contains(RESPONSEJSON["options"][formIndex]["requires"].Value))
                        continue;

                    if (!string.IsNullOrEmpty(RESPONSEJSON["options"][formIndex]["avoid"].Value) && Order_List.Contains(RESPONSEJSON["options"][formIndex]["avoid"].Value))
                        continue;

                    Dispatcher.Invoke(new Action(() => { StackPanel_Body_Container.Children.Clear(); }));

                    //https://stackoverflow.com/questions/350027/setting-wpf-image-source-in-code
                    if (!string.IsNullOrEmpty(RESPONSEJSON["options"][formIndex]["image"].Value) /*&& ResourceExists("Resources/" + RESPONSEJSON["options"][formIndex]["image"].Value)*/)
                    {
                        if (ResourceExists("Resources/" + RESPONSEJSON["options"][formIndex]["image"].Value))
                        {
                            var uriSource = new Uri(@"/ULTRAMOD_Updater;component/Resources/" + RESPONSEJSON["options"][formIndex]["image"].Value, UriKind.Relative);
                            Dispatcher.Invoke(new Action(() => { img_conf.Source = new BitmapImage(uriSource); }));
                        }
                        else LogMessage("ERROR: Could not Load " + RESPONSEJSON["options"][formIndex]["image"].Value + " Resource. make sure to use latest updater to get proper performance.");
                    }
                    else
                    {
                        var uriSource = new Uri(@"/ULTRAMOD_Updater;component/Resources/img_config.png", UriKind.Relative);
                        Dispatcher.Invoke(new Action(() => { img_conf.Source = new BitmapImage(uriSource); }));
                    }

                    if (!string.IsNullOrEmpty(RESPONSEJSON["options"][formIndex]["label"].Value))
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            //https://stackoverflow.com/questions/18659435/programmatically-add-label-to-grid
                            System.Windows.Controls.Label newLbl = new Label();
                            newLbl.Margin = new Thickness(-7, 0, 0, 0);
                            newLbl.FontSize = 16;
                            newLbl.FontWeight = FontWeights.Bold;
                            newLbl.Content = RESPONSEJSON["options"][formIndex]["label"].Value;
                            StackPanel_Body_Container.Children.Add(newLbl);
                        }));
                    }

                    if (RESPONSEJSON["options"][formIndex]["type"] == "chooselist")
                    {
                        //if (!string.IsNullOrEmpty(RESPONSEJSON["options"][formIndex]["requires"].Value) && !Order_List.Contains(RESPONSEJSON["options"][formIndex]["requires"].Value))
                        //    continue;

                        //if (!string.IsNullOrEmpty(RESPONSEJSON["options"][formIndex]["avoid"].Value) && Order_List.Contains(RESPONSEJSON["options"][formIndex]["avoid"].Value))
                        //    continue;

                        System.Windows.Controls.RadioButton rbtn;
                        for (int x = 0; string.IsNullOrEmpty(RESPONSEJSON["options"][formIndex]["list"][x]["name"].Value) == false; x++)
                        {
                            if (!string.IsNullOrEmpty(RESPONSEJSON["options"][formIndex]["list"][x]["requires"].Value) && !Order_List.Contains(RESPONSEJSON["options"][formIndex]["list"][x]["requires"].Value))
                                continue;

                            if (!string.IsNullOrEmpty(RESPONSEJSON["options"][formIndex]["list"][x]["avoid"].Value) && Order_List.Contains(RESPONSEJSON["options"][formIndex]["list"][x]["avoid"].Value))
                                continue;

                            if (RESPONSEJSON["options"][formIndex]["list"][x]["type"].Value == "label")
                            {
                                Dispatcher.Invoke(new Action(() =>
                                {
                                    System.Windows.Controls.Label lbl = new Label();
                                    lbl.Content = RESPONSEJSON["options"][formIndex]["list"][x]["content"].Value;
                                    lbl.Margin = new Thickness(1, 3, 0, 0);

                                    StackPanel_Body_Container.Children.Add(lbl);
                                }));
                            }

                            Dispatcher.Invoke(new Action(() =>
                            {
                                rbtn = new RadioButton();
                                rbtn.Tag = RESPONSEJSON["options"][formIndex]["list"][x]["name"].Value;//x.ToString()
                                rbtn.Content = RESPONSEJSON["options"][formIndex]["list"][x]["content"].Value;
                                rbtn.Margin = new Thickness(1, 3, 0, 0);
                                if (!string.IsNullOrEmpty(RESPONSEJSON["options"][formIndex]["list"][x]["enabled"].Value) && RESPONSEJSON["options"][formIndex]["list"][x]["enabled"].Value == "false")
                                    rbtn.IsEnabled = false;

                                StackPanel_Body_Container.Children.Add(rbtn);
                            }));
                        }
                    }
                    else if (RESPONSEJSON["options"][formIndex]["type"] == "checklist")
                    {
                        //if (!string.IsNullOrEmpty(RESPONSEJSON["options"][formIndex]["requires"].Value) && !Order_List.Contains(RESPONSEJSON["options"][formIndex]["requires"].Value))
                        //    continue;

                        //if (!string.IsNullOrEmpty(RESPONSEJSON["options"][formIndex]["avoid"].Value) && Order_List.Contains(RESPONSEJSON["options"][formIndex]["avoid"].Value))
                        //    continue;

                        System.Windows.Controls.CheckBox checkbox;
                        for (int x = 0; string.IsNullOrEmpty(RESPONSEJSON["options"][formIndex]["list"][x]["name"].Value) == false; x++)
                        {
                            //MessageBox.Show(RESPONSEJSON["options"][i]["list"][x]["name"].Value + " : " + RESPONSEJSON["options"][i]["list"][x]["content"].Value , "[DEBUG]");

                            if (!string.IsNullOrEmpty(RESPONSEJSON["options"][formIndex]["list"][x]["requires"].Value) && !Order_List.Contains(RESPONSEJSON["options"][formIndex]["list"][x]["requires"].Value))
                                continue;

                            if (!string.IsNullOrEmpty(RESPONSEJSON["options"][formIndex]["list"][x]["avoid"].Value) && Order_List.Contains(RESPONSEJSON["options"][formIndex]["list"][x]["avoid"].Value))
                                continue;

                            if (RESPONSEJSON["options"][formIndex]["list"][x]["type"].Value == "label")
                            {
                                Dispatcher.Invoke(new Action(() =>
                                {
                                    System.Windows.Controls.Label lbl = new Label();
                                    lbl.Content = RESPONSEJSON["options"][formIndex]["list"][x]["content"].Value;
                                    lbl.Margin = new Thickness(1, 3, 0, 0);

                                    StackPanel_Body_Container.Children.Add(lbl);
                                }));
                            }

                            Dispatcher.Invoke(new Action(() =>
                            {
                                checkbox = new CheckBox();
                                checkbox.Tag = RESPONSEJSON["options"][formIndex]["list"][x]["name"].Value;//x.ToString()
                                checkbox.Content = RESPONSEJSON["options"][formIndex]["list"][x]["content"].Value;
                                checkbox.Margin = new Thickness(1, 3, 0, 0);
                                if (!string.IsNullOrEmpty(RESPONSEJSON["options"][formIndex]["list"][x]["enabled"].Value) && RESPONSEJSON["options"][formIndex]["list"][x]["enabled"].Value == "false")
                                    checkbox.IsEnabled = false;
                                if (!string.IsNullOrEmpty(RESPONSEJSON["options"][formIndex]["list"][x]["default"].Value) && RESPONSEJSON["options"][formIndex]["list"][x]["default"].Value == "checked")
                                    checkbox.IsChecked = true;

                                StackPanel_Body_Container.Children.Add(checkbox);
                            }));
                        }
                    }
                    else if (RESPONSEJSON["options"][formIndex]["type"] == "labellist")
                    {
                        //if (!string.IsNullOrEmpty(RESPONSEJSON["options"][formIndex]["requires"].Value) && !Order_List.Contains(RESPONSEJSON["options"][formIndex]["requires"].Value))
                        //    continue;

                        //if (!string.IsNullOrEmpty(RESPONSEJSON["options"][formIndex]["avoid"].Value) && Order_List.Contains(RESPONSEJSON["options"][formIndex]["avoid"].Value))
                        //    continue;

                        System.Windows.Controls.Label lbl;
                        for (int x = 0; string.IsNullOrEmpty(RESPONSEJSON["options"][formIndex]["list"][x]["name"].Value) == false; x++)
                        {
                            if (!string.IsNullOrEmpty(RESPONSEJSON["options"][formIndex]["list"][x]["requires"].Value) && !Order_List.Contains(RESPONSEJSON["options"][formIndex]["list"][x]["requires"].Value))
                                continue;

                            if (!string.IsNullOrEmpty(RESPONSEJSON["options"][formIndex]["list"][x]["avoid"].Value) && Order_List.Contains(RESPONSEJSON["options"][formIndex]["list"][x]["avoid"].Value))
                                continue;

                            Dispatcher.Invoke(new Action(() =>
                            {
                                lbl = new Label();
                                //lbl.Tag = RESPONSEJSON["options"][i]["list"][x]["name"].Value;//x.ToString()
                                lbl.Content = RESPONSEJSON["options"][formIndex]["list"][x].Value;
                                lbl.Margin = new Thickness(1, 3, 0, 0);

                                StackPanel_Body_Container.Children.Add(lbl);
                            }));
                        }
                    }

                    json_parse_pause = true;

                    while (json_parse_pause)
                    {
                        //I'd like the wait here
                    }
                }
                Order_List_String = Order_List_String.Remove(Order_List_String.Length - 1);

                Receive_Generated_DownloadList();
            }
            catch (Exception ex)
            {
                Thread FlagScriptError = new Thread(new ThreadStart(FlagAsScriptError));
                FlagScriptError.Start();
                Dispatcher.Invoke(new Action(() => { lbl_Body_Message.Content = "ERROR ON PARSING INFORMATION JSON : " + ex.ToString(); }));
            }
        }

        public static string DeleteLines(string s, int linesToRemove)
        {
            return s.Split(Environment.NewLine.ToCharArray(),
                           linesToRemove + 1
                ).Skip(linesToRemove)
                .FirstOrDefault();
        }

        string GetLine(string text, int lineNo)
        {
            string[] lines = text.Replace("\r", "").Split('\n');
            return lines.Length >= lineNo ? lines[lineNo - 1] : null;
        }

        private void Receive_Generated_DownloadList()
        {
            try
            {
                //WebClient client = new WebClient();
                //string DownloadList = client.DownloadString(HeadquartersURL + "?order=" + Order_List_String);

                //MessageBox.Show(Order_List_String, "[DEBUG][Receive_Generated_DownloadList]", MessageBoxButton.OK, MessageBoxImage.Information);

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(HeadquartersURL + "?orderlist=" + Order_List_String);
                request.Method = "GET";
                String DownloadList = String.Empty;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    DownloadList = reader.ReadToEnd();
                    reader.Close();
                    dataStream.Close();
                }

                ClientDownloadDirectory = GetLine(DownloadList, 1);
                UpdateHashList = DeleteLines(DownloadList, 1);

                LogMessage("INFO: HashList successfully downloaded");
                this.currentData = "HashList downloaded.";

                //MessageBox.Show(DownloadList, "[DEBUG][Receive_Generated_DownloadList]", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                TimedoutTimes++;
                if (TimedoutTimes <= MaxTimeoutTime)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        this.lbl_status_pos2.Content = "re-trying to obtain client hashlist from server [" + TimedoutTimes + "/" + MaxTimeoutTime + "]  ...";
                    }));
                    Receive_Generated_DownloadList();
                }
                else
                {
                    //MessageBox.Show(ex.ToString());
                    Thread FlagOffline = new Thread(new ThreadStart(FlagAsOffline));
                    FlagOffline.Start();
                }
            }
            Dispatcher.Invoke(new Action(() => { StartPreLoader(); }));
        }

        //https://stackoverflow.com/questions/20844178/fetching-selected-radio-button-in-wpf
        private void btn_submit_Click(object sender, RoutedEventArgs e)
        {
            int i = 0;
            foreach (var element in StackPanel_Body_Container.Children.OfType<RadioButton>())
            {
                if (element.IsEnabled) i++;
            }
            var checked_rbtn = StackPanel_Body_Container.Children.OfType<RadioButton>().FirstOrDefault(r => r.IsChecked.HasValue && r.IsChecked.Value);

            if (i > 0 && checked_rbtn == null)
            {
                MessageBox.Show("Please Select at Least one of switchs to continue", "ULTRAMOD UPDATER", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (checked_rbtn != null)
            {
                Order_List_String = Order_List_String + checked_rbtn.Tag.ToString() + "*";
                Order_List.Add(checked_rbtn.Tag.ToString());
            }

            //https://stackoverflow.com/questions/20844178/fetching-selected-radio-button-in-wpf
            //https://stackoverflow.com/questions/31734227/wpf-checkbox-check-ischecked
            foreach (var element in StackPanel_Body_Container.Children.OfType<CheckBox>())
            {
                if (element != null && (bool)element.IsChecked)
                {
                    Order_List_String = Order_List_String + element.Tag + "*";
                    Order_List.Add(element.Tag.ToString());
                }
            }
            //MessageBox.Show(Order_List_String,"[DEBUG]");
            this.btn_clear.Visibility = Visibility.Visible;
            json_parse_pause = false;
        }
        private void btn_clear_Click(object sender, RoutedEventArgs e)
        {
            Order_List_String = "";
            Order_List.Clear();
            this.btn_clear.Visibility = Visibility.Hidden;

            var uriSource = new Uri(@"/ULTRAMOD_Updater;component/Resources/img_config.png", UriKind.Relative);
            img_conf.Source = new BitmapImage(uriSource);

            formIndex = -1;//-1 because once we release loop lock and when it ends it will increase i valuse by end
            json_parse_pause = false;

            //Thread RequestDownloadOptions = new Thread(RequestDownloadOptionList);
            //RequestDownloadOptions.Start();
        }

        /// <summary>
        /// Get File Md5
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        protected string GetMD5HashFromFile(string fileName)
        {
            FileStream file = new FileStream(fileName, FileMode.Open);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(file);
            file.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        /// <summary>
        /// Extract File from Emmbded Resource
        /// </summary>
        /// <param name="nameSpace"></param>
        /// <param name="outDirectory"></param>
        /// <param name="internalFilePath"></param>
        /// <param name="resourceName"></param>
        static void Extract(string nameSpace, string outDirectory, string internalFilePath, string resourceName)
        {
            //nameSpace = the namespace of your project, located right above your class' name;
            //outDirectory = where the file will be extracted to;
            //internalFilePath = the name of the folder inside visual studio which the files are in;
            //resourceName = the name of the file;
            Assembly assembly = Assembly.GetCallingAssembly();

            using (Stream s = assembly.GetManifestResourceStream(nameSpace + "." + (internalFilePath == "" ? "" : internalFilePath + ".") + resourceName))
            using (BinaryReader r = new BinaryReader(s))
            using (FileStream fs = new FileStream(outDirectory + "\\" + resourceName, FileMode.OpenOrCreate))
            using (BinaryWriter w = new BinaryWriter(fs))
            {
                w.Write(r.ReadBytes((int)s.Length));
            }
        }

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            //if (dlError == true || Auth_Result == false || json_parse_pause == true)
            Environment.Exit(1);
        }

        private void btn_continue_update_Click(object sender, RoutedEventArgs e)
        {
            wait_for_me = false;
        }

        private void btn_continue_Click(object sender, RoutedEventArgs e)
        {
            wait_for_me = false;
        }

        private void btn_open_logfile_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Environment.CurrentDirectory + "\\" + logfile);
        }

        //https://stackoverflow.com/questions/2013481/detect-whether-wpf-resource-exists-based-on-uri
        public static bool ResourceExists(string resourcePath)
        {
            var assembly = Assembly.GetExecutingAssembly();

            return ResourceExists(assembly, resourcePath);
        }

        public static bool ResourceExists(Assembly assembly, string resourcePath)
        {
            return GetResourcePaths(assembly)
                .Contains(resourcePath.ToLowerInvariant());
        }

        public static IEnumerable<object> GetResourcePaths(Assembly assembly)
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var resourceName = assembly.GetName().Name + ".g";
            var resourceManager = new System.Resources.ResourceManager(resourceName, assembly);

            try
            {
                var resourceSet = resourceManager.GetResourceSet(culture, true, true);

                foreach (System.Collections.DictionaryEntry resource in resourceSet)
                {
                    yield return resource.Key;
                }
            }
            finally
            {
                resourceManager.ReleaseAllResources();
            }
        }
    }

    //https://stackoverflow.com/questions/5306029/what-is-the-easy-way-to-set-spacing-between-items-in-stackpanel
    public class MarginSetter
    {
        public static Thickness GetMargin(DependencyObject obj)
        {
            return (Thickness)obj.GetValue(MarginProperty);
        }

        public static void SetMargin(DependencyObject obj, Thickness value)
        {
            obj.SetValue(MarginProperty, value);
        }

        // Using a DependencyProperty as the backing store for Margin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarginProperty =
            DependencyProperty.RegisterAttached("Margin", typeof(Thickness), typeof(MarginSetter), new UIPropertyMetadata(new Thickness(), CreateThicknesForChildren));

        public static void CreateThicknesForChildren(object sender, DependencyPropertyChangedEventArgs e)
        {
            var panel = sender as Panel;

            if (panel == null) return;

            foreach (var child in panel.Children)
            {
                var fe = child as FrameworkElement;

                if (fe == null) continue;

                fe.Margin = MarginSetter.GetMargin(panel);
            }
        }
    }
}