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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WPFVideoPlayer.Components
{
    /// <summary>
    /// Interaction logic for FullVideo.xaml
    /// </summary>
    public partial class FullVideo : Window
    {

        DispatcherTimer timer;
        DispatcherTimer panelTimer;
        public bool isPlaying = false;

        public bool isFullScreen = false;

        bool isDragging = false;

        public double lastVolume = 0;
        bool isMute = false;

        int panelTime = 0;

        private Uri source;

        public FullVideo(Uri source)
        {
            this.source = source;
            InitializeComponent();

            myVideoPlayer.Source = this.source;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += Timer_Tick;

            panelTimer = new DispatcherTimer();
            panelTimer.Interval = TimeSpan.FromMilliseconds(800);
            panelTimer.Tick += PanelTimer_Tick;
        }

        private void PanelTimer_Tick(object? sender, EventArgs e)
        {
            panelTime++;
            if (panelTime >= 5)
            {
                HideControlPanel();
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (!isDragging)
            {
                videoTimeSlider.Value = myVideoPlayer.Position.TotalSeconds;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            isPlaying = true;

            PlayMedia();
            panelTimer.Start();
            InitializePropertyValues();
        }

        void InitializePropertyValues()
        {
            //videoVolumeSlider.Value = 0.5;
            //myVideoPlayer.Volume = (double)videoVolumeSlider.Value;
            myVideoPlayer.Volume = 1;
        }
        private void VideoTimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //int SliderValue = (int)videoTimeSlider.Value;

            // Overloaded constructor takes the arguments days, hours, minutes, seconds, milliseconds.
            // Create a TimeSpan with miliseconds equal to the slider value.
            //TimeSpan ts = new TimeSpan(0, 0, 0, 0, SliderValue);
            //myVideoPlayer.Position = ts;

            lblCurrentPosition.Text = myVideoPlayer.Position.ToString(@"mm\:ss");
        }

        private void videoTimeSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            isDragging = true;
        }

        private void videoTimeSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            isDragging = false;
            myVideoPlayer.Position = TimeSpan.FromSeconds(videoTimeSlider.Value);
        }

        private void MyVideoPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            // videoTimeSlider.Maximum = myVideoPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;

            if (myVideoPlayer.NaturalDuration.HasTimeSpan)
            {
                TimeSpan ts = myVideoPlayer.NaturalDuration.TimeSpan;
                videoTimeSlider.Maximum = ts.TotalSeconds;
                videoTimeSlider.SmallChange = 1;
                videoTimeSlider.LargeChange = Math.Min(10, ts.Seconds / 10);
                lblMaximumPosition.Text = myVideoPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
            }
            timer.Start();

        }

        private void MyVideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            isPlaying = false;
            CloseVideo();
        }

        private void myVideoPlayer_MouseEnter(object sender, MouseEventArgs e)
        {
           // VideoControlPanel.Visibility = Visibility.Visible;
        }

        private void myVideoPlayer_MouseLeave(object sender, MouseEventArgs e)
        {
            //if (isFullScreen == false)
            //    VideoControlPanel.Visibility = Visibility.Hidden;
        }

        private void myVideoPlayer_TouchDown(object sender, TouchEventArgs e)
        {
            if (isPlaying)
            {
                //PauseMedia();
                //isPlaying = false;
            }
            else
            {
                PlayMedia();
                isPlaying = true;
            }

            ShowControlPanel();
        }

        private void myVideoPlayer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isPlaying)
            {
                //PauseMedia();
                //isPlaying = false;
            }
            else
            {
                PlayMedia();
                isPlaying = true;
            }

            ShowControlPanel();
        }

        private void PlayMedia()
        {
            btnPlay.IsChecked = true;
            myVideoPlayer.Play();
        }

        private void StopMedia()
        {
            btnPlay.IsChecked = false;
            myVideoPlayer.Stop();
        }

        private void PauseMedia()
        {
            myVideoPlayer.Pause();
        }

        private void ShowControlPanel()
        {
            controlRow.Height = new GridLength(75,GridUnitType.Pixel);
            VideoControlPanel.Visibility = Visibility.Visible;
            panelTime = 0;
            panelTimer.Start();
        }

        private void HideControlPanel()
        {
            controlRow.Height = new GridLength(0);
            VideoControlPanel.Visibility = Visibility.Collapsed;
            panelTime = 0;
            panelTimer.Stop();
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (isPlaying)
            {
                PauseMedia();
                isPlaying = false;
            }
            else
            {
                PlayMedia();
                isPlaying = true;
            }
        }

        private void CloseVideo()
        {
            StopMedia();
            this.Close();
        }
        private void btnFullScreen_Click(object sender, RoutedEventArgs e)
        {
            CloseVideo();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
