using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YoutubeExplode;
using YoutubeExplode.Models.MediaStreams;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;

namespace YT2MP3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        DateTime time = DateTime.Now;
        YoutubeClient client = new YoutubeClient();

        String currentUrl;
        String currentId;


        public MainWindow()
        {
            InitializeComponent();
            currentUrl = "https://www.youtube.com/watch?v=sNPnbI1arSE";
        }
        private void DownloadBtnClick(object sender, RoutedEventArgs e)
        {

            String dir = directoryText.Text;
            DownloadVid(currentUrl, dir);
        }

        async void DownloadVid(String url, String dir)


        {
            Action<double> a = new Action<double>(updateProgressBar);
            Progress<double> p = new Progress<double>(a);

            try
            {

                var streamInfoSet = await client.GetVideoMediaStreamInfosAsync(currentId);
                var streamInfo = streamInfoSet.Audio.WithHighestBitrate();

                // Get file extension based on stream's container
                var ext = "wav";


                // Download stream to file
                string[] array = { dir, $"test.{ext}" };
                string fullPath = System.IO.Path.Combine(array);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                await client.DownloadMediaStreamAsync(streamInfo, fullPath, p);

            }
            catch (Exception e)
            {

                if (e is FormatException)
                {
                    messageTxt.Text = "error parsing link";
                }

            }

        }






        private void SelectDir(object sender, RoutedEventArgs e)
        {
            if (CommonOpenFileDialog.IsPlatformSupported)
            {
                CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                dialog.Title = "Choose Directory";
                dialog.IsFolderPicker = true;
                dialog.EnsurePathExists = true;
                dialog.Multiselect = false;

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    string selectedFolder = dialog.FileName;
                    directoryText.Text = selectedFolder;
                }
            }
        }

        private void updateProgressBar(double p)
        {
            dlProgress.Value = p;
            if (p == 1)
            {
                messageTxt.Text = "done";
                dlProgress.Value = 0;
            }

        }

        private async void LoadData()
        {
            try
            {
                currentId = YoutubeClient.ParseVideoId(currentUrl);
                currentUrl = urlTextBox.Text;
                var video = await client.GetVideoAsync(currentId);
                messageTxt.Text = video.Title;
            }
            catch (System.FormatException)
            {
                messageTxt.Text = "Couldn't parse URL";
            }
          
  
        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            currentUrl = ((TextBox)sender).Text;
            LoadData();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                LoadData();
            }
        }



        private void directoryPicker_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Album_Button_Click(object sender, RoutedEventArgs e)
        {
            Window albumWindow = new AlbumWindow(currentUrl);
            albumWindow.Show();
        }
    }
}
