using NAudio.Wave;
using NAudio.Lame;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using YoutubeExplode;
using YoutubeExplode.Models.MediaStreams;
using Path = System.IO.Path;
using System.Net;
using System.Drawing;

namespace YT2MP3
{
    /// <summary>
    /// Interaction logic for AlbumWindow.xaml
    /// </summary>
    public partial class AlbumWindow : Window
    {
        YoutubeClient client;
        string url;
        string id;
        List<Track> tracks;
        public AlbumWindow(string url)
        {

            InitializeComponent();

            client = new YoutubeClient();
            this.url = url;
            id = YoutubeClient.ParseVideoId(url);

            tracks = new List<Track>();
            MyDataGrid.ItemsSource = tracks;



        }

        private async void Submit_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(tracks.Count);
            if (await DownloadVid(url, dirTxtAlbum.Text, "Album"))
            {
                string albumDir = Path.Combine(dirTxtAlbum.Text, albumNameTxt.Text);
                Directory.CreateDirectory(albumDir);

                splitWavPrep(albumDir, Path.Combine(dirTxtAlbum.Text, "Album.mp3"));
            }

        }
        private async Task<bool> DownloadVid(string url, string dir, string fileName)


        {
            id = YoutubeClient.ParseVideoId(url);
            Action<double> a = new Action<double>(updateProgressBarAlbum);
            Progress<double> p = new Progress<double>(a);

            try
            {

                var streamInfoSet = await client.GetVideoMediaStreamInfosAsync(id);
                var streamInfo = streamInfoSet.Audio.WithHighestBitrate();

               

                // Get file extension based on stream's container
                var ext = "mp3";


                // Download stream to file
                string[] array = { dir, $"{fileName}.{ext}" };
                string fullPath = System.IO.Path.Combine(array);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                await client.DownloadMediaStreamAsync(streamInfo, fullPath, p);
                return true;

            }
            catch (Exception e)
            {

                if (e is FormatException)
                {
                    messageTxtAlbum.Text = "error parsing link";
                }
                return false;

            }

        }


        private byte[] getThumbnail(string id)
        {
            string maxResUrl = "https://img.youtube.com/vi/" + id + "/maxresdefault.jpg";
            string imgUrl = "https://img.youtube.com/vi/" + id + "/hqdefault.jpg";
            try
            {
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(maxResUrl);
                HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                if (myHttpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    imgUrl = maxResUrl;
                }
                myHttpWebResponse.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            

            using (WebClient client = new WebClient())
            {
                using (Stream stream = client.OpenRead(imgUrl))
                {
                    Image img = Image.FromStream(stream);
                    ImageConverter converter = new ImageConverter();
                    return (byte[])converter.ConvertTo(img, typeof(byte[]));
                }
            }
        }

        private void updateProgressBarAlbum(double p)
        {
            dlProgressAlbum.Value = p;
            if (p == 1)
            {
                messageTxtAlbum.Text = "done";
                dlProgressAlbum.Value = 0;
            }

        }

        private void splitWavPrep(string albumPath, string filePath)
        {
            string artist = artistTxt.Text;
            MediaFoundationReader reader = new MediaFoundationReader(filePath);
            int bytesPerMilliSecond = reader.WaveFormat.AverageBytesPerSecond / 1000;
            for (int i = 0; i < tracks.Count; i++)
            {
                int startMilliSeconds = getFullMilliSeconds(tracks[i].StartTime);

                string trackName = tracks[i].Title;

                int duration;
                if (i < tracks.Count - 1)
                {
                    //if there's a next track
                    duration = (int)getFullMilliSeconds(tracks[i + 1].StartTime) - startMilliSeconds;
                }
                else
                {
                    //this is the last track
                    duration = (int)reader.TotalTime.TotalMilliseconds - startMilliSeconds;
                }
                int startPos = startMilliSeconds * bytesPerMilliSecond;
                startPos = startPos - startPos % reader.WaveFormat.BlockAlign;
                int endMilliSeconds = startMilliSeconds + duration;
                int endBytes = (endMilliSeconds - startMilliSeconds) * bytesPerMilliSecond;
                endBytes = endBytes - endBytes % reader.WaveFormat.BlockAlign;

                int endPos = startPos + endBytes;

                string trackPath = Path.Combine(albumPath, trackName + ".mp3");

                ID3TagData tag = new ID3TagData
                {
                    Title = trackName,
                    Artist = artist,
                    Album = albumNameTxt.Text,
                    Comment = "YT2MP3 https://github.com/teknoalex/YT2MP3",
                   AlbumArtist = artist,
                   Track = i.ToString(),
                   AlbumArt = getThumbnail(id)
                };



                splitMp3(trackPath, startPos, endPos, reader,tag);


            }
        }

        private void splitMp3(string trackPath, int startPos, int endPos, MediaFoundationReader reader, ID3TagData tagData)
        {

            int progress = 0;
            using (var writer = new NAudio.Lame.LameMP3FileWriter(trackPath, reader.WaveFormat, NAudio.Lame.LAMEPreset.V3, tagData))
            {
                reader.Position = startPos;
                byte[] buffer = new byte[1024];
                while (reader.Position < endPos)
                {
                    int bytesRequired = (int)(endPos - reader.Position);
                    if (bytesRequired > 0)
                    {
                        int bytesToRead = Math.Min(bytesRequired, buffer.Length);
                        int bytesRead = reader.Read(buffer, 0, bytesToRead);
                        if (bytesRead > 0)
                        {
                            writer.Write(buffer, 0, bytesRead);
                            progress += bytesRead;
                        }
                    }
                }
            }
        }

        


        private static int getFullMilliSeconds(string valueStr)
        {
            //turn 02:22 into 142000 milliseconds
            Regex pattern = new Regex(@"^(?<minute>\d+):(?<second>\d+)");
            string minStr = (pattern.Match(valueStr).Groups["minute"].Value);
            int min = int.Parse(minStr);
            int sec = int.Parse(pattern.Match(valueStr).Groups["second"].Value);
            return ((min * 60) + sec) * 1000;
        }
    }

    class Track
    {
        // public int Count { get; set; }

        public string Title { get; set; }

        public string StartTime { get; set; }
    }
}
