/************************************************************************************
____________________________ULTRAMOD UPDATER Project__________________________******
***********************************************************************************
**
** - Name        : win_Adv.xaml Code
** - Description : Advertisment form
** - Author      : Hosseinpourziyaie
** - Note        : -----------------
** - Started on  : N/A        | Ended on : N/A
**
** [Copyright © ULTRAMOD/Hosseinpourziyaie 2019] <hosseinpourziyaie@gmail.com>
**
************************************************************************************/

using System;
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
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using System.Threading;

namespace ULTRAMOD_Updater
{
    /// <summary>
    /// Interaction logic for win_Adv.xaml
    /// </summary>
    public partial class win_Adv : MetroWindow
    {
        string ImageLink;
        /*int*/double Duration;
        win_Main Mainwindow;
        System.Windows.Threading.DispatcherTimer Advertisment_Timer = new System.Windows.Threading.DispatcherTimer();


        public win_Adv(win_Main main, string PicLink, /*int*/double Time)
        {
            InitializeComponent();
            ImageLink = PicLink;
            Duration = Time * 100;
            progressbar_timer.Maximum = Duration;
            Mainwindow = main;
        }
        private void win_Adv_Load(object sender, RoutedEventArgs e)
        {
            Advertisment_Timer.Tick += Advertisment_Timer_Tick;
            Advertisment_Timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            Advertisment_Timer.Stop();
            //Advertisment_Timer.IsEnabled = false;

            //pic_adv.Stretch = Stretch.UniformToFill;

            Thread firsThread = new Thread(new ThreadStart(LoadImage));
            firsThread.Start();
        }

        private void image_DecodeFailed(object sender, ExceptionEventArgs e)
        {
            Duration = 100;
            progressbar_timer.Maximum = 100;//Duration = 100 means 1 sec just if we couldnt load image
            this.lbl_loading.Visibility = Visibility.Hidden;
            Advertisment_Timer.Start();
        }

        private void image_DownloadFailed(object sender, ExceptionEventArgs e)
        {
            Duration = 100;
            progressbar_timer.Maximum = 100;//Duration = 100 means 1 sec just if we couldnt load image
            this.lbl_loading.Visibility = Visibility.Hidden;
            Advertisment_Timer.Start();
        }

        private void image_DownloadCompleted(object sender, EventArgs e)
        {
            this.pic_loading.Visibility = Visibility.Hidden;
            this.lbl_loading.Visibility = Visibility.Hidden;
            Advertisment_Timer.Start();
        }

        private void LoadImage()
        {
            try
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    this.lbl_loading.Visibility = Visibility.Visible;
                    var uri = new Uri(ImageLink);
                    var bitmap = new BitmapImage(uri);
                    bitmap.DownloadCompleted += image_DownloadCompleted;
                    bitmap.DecodeFailed += image_DecodeFailed;
                    bitmap.DownloadFailed += image_DownloadFailed;

                    pic_adv.Source = bitmap;

                    //pic_loading.Visible = Visibility.Hidden;
                }));

                //Advertisment_Timer.IsEnabled = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                //this.Close();
                //throw;
            }
        }

        private void Advertisment_Timer_Tick(object sender, EventArgs e)
        {
            // code goes here
            if (progressbar_timer.Value != Duration)
            {
                progressbar_timer.Value++;
            }
            else
            {
                Advertisment_Timer.Stop();
                this.Close();
            }
        }

        private void win_Adv_Closed(object sender, EventArgs e)
        {
            //https://stackoverflow.com/questions/257587/bring-a-window-to-the-front-in-wpf
            Mainwindow.Show();
            Mainwindow.Topmost = true;
            Mainwindow.Topmost = false;//re-bind for messagebox,etc
            //Mainwindow.WindowState = WindowState.Normal;
        }

        //another good example : https://stackoverflow.com/questions/5439042/loading-an-image-in-a-background-thread-in-wpf
        /*var worker = new BackgroundWorker() { WorkerReportsProgress = true };

        // DoWork runs on a brackground thread...no thouchy uiy.
     worker.DoWork += (sender, args) =>
     {
        var uri = args.Argument as Uri;
        var image = new BitmapImage();

        image.BeginInit();
        image.DownloadProgress += (s, e) => worker.ReportProgress(e.Progress);
        image.DownloadFailed += (s, e) => Dispatcher.CurrentDispatcher.InvokeShutdown();
        image.DecodeFailed += (s, e) => Dispatcher.CurrentDispatcher.InvokeShutdown();
        image.DownloadCompleted += (s, e) =>
        {
           image.Freeze();
           args.Result = image;
           Dispatcher.CurrentDispatcher.InvokeShutdown();
        };
        image.UriSource = uri;
        image.EndInit();

        // !!! if IsDownloading == false the image is cached and NO events will fire !!!

        if (image.IsDownloading == false)
        {
           image.Freeze();
           args.Result = image;
        }
        else
        {
           // block until InvokeShutdown() is called. 
           Dispatcher.Run();
        }
     };

     // ProgressChanged runs on the UI thread
     worker.ProgressChanged += (s, args) => progressBar.Value = args.ProgressPercentage;

     // RunWorkerCompleted runs on the UI thread
     worker.RunWorkerCompleted += (s, args) =>
     {
        if (args.Error == null)
        {
           uiImage.Source = args.Result as BitmapImage;
        }
     };

     var imageUri = new Uri(@"http://farm6.static.flickr.com/5204/5275574073_1c5b004117_b.jpg");

     worker.RunWorkerAsync(imageUri);*/


    }
}
