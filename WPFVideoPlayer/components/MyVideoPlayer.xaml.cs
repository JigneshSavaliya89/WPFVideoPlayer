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
using System.Windows.Threading;

namespace WPFVideoPlayer.Components
{
    /// <summary>
    /// Interaction logic for MyVideoPlayer.xaml
    /// </summary>
    public partial class MyVideoPlayer : UserControl
    {



        public Uri Source
        {
            get { return (Uri)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(Uri), typeof(MyVideoPlayer), new PropertyMetadata(default(Uri)));



        public double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Height.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(double), typeof(MyVideoPlayer), new PropertyMetadata(default(double)));



        public double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Width.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(MyVideoPlayer), new PropertyMetadata(default(double)));




        public Uri ThumbnailSource
        {
            get { return (Uri)GetValue(ThumbnailSourceProperty); }
            set { SetValue(ThumbnailSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ThumbnailSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThumbnailSourceProperty =
            DependencyProperty.Register("ThumbnailSource", typeof(Uri), typeof(MyVideoPlayer), new PropertyMetadata(default(Uri)));



        FullScreen fullScreen;
        DispatcherTimer timer;
        DispatcherTimer panelTimer;
        public bool isPlaying = false;

        private DependencyObject _originalParent = null;
        public bool isFullScreen = false;

        bool isDragging = false;

        public double lastVolume = 0;
        bool isMute = false;

        int panelTime = 0;

        public MyVideoPlayer()
        {
            InitializeComponent();
            fullScreen = new FullScreen();
            isPlaying = false;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += Timer_Tick;

            panelTimer = new DispatcherTimer();
            panelTimer.Interval = TimeSpan.FromMilliseconds(5000);
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

        private void VideoTimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //int SliderValue = (int)videoTimeSlider.Value;

            // Overloaded constructor takes the arguments days, hours, minutes, seconds, milliseconds.
            // Create a TimeSpan with miliseconds equal to the slider value.
            //TimeSpan ts = new TimeSpan(0, 0, 0, 0, SliderValue);
            //myVideoPlayer.Position = ts;

            lblCurrentPosition.Text = myVideoPlayer.Position.ToString(@"mm\:ss");
        }

        private void VideoVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            myVideoPlayer.Volume = (double)videoVolumeSlider.Value;
            if (videoVolumeSlider.Value <= 0)
            {
                btnVolume.IsChecked = true;
                isMute = true;
            }
            else
            {
                btnVolume.IsChecked = false;
                isMute = false;
            }
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
            ToggleFullScreen();
            isPlaying = false;
        }

        void InitializePropertyValues()
        {
            videoVolumeSlider.Value = 0.5;
            myVideoPlayer.Volume = (double)videoVolumeSlider.Value;
            //myVideoPlayer.Volume = 1;
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
            InitializePropertyValues();
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

        private void VideoPlayerRoot_Loaded(object sender, RoutedEventArgs e)
        {
            if (isFullScreen == true)
            {
                videoThumbnail.Visibility = Visibility.Collapsed;
                VideoControlPanel.Visibility = Visibility.Visible;
            }
            else
            {
                if (!isPlaying && videoTimeSlider.Value == 0)
                {
                    videoThumbnail.Visibility = Visibility.Visible;
                }
                VideoControlPanel.Visibility = Visibility.Hidden;
            }
        }

        private void btnBigPlay_Click(object sender, RoutedEventArgs e)
        {
            HideThumbnail();

            PlayMedia();
            InitializePropertyValues();
            isPlaying = true;
            btnPlay.IsChecked = true;

            ShowControlPanel();
        }

        private void myVideoPlayer_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowControlPanel();
        }

        private void myVideoPlayer_MouseLeave(object sender, MouseEventArgs e)
        {
            if (isFullScreen == false)
                HideControlPanel();
        }

        private void VideoControlPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowControlPanel();
        }

        private void VideoControlPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            HideControlPanel();
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

        private void btnFullScreen_Click(object sender, RoutedEventArgs e)
        {
            ToggleFullScreen();
        }

        private void btnVolume_Click(object sender, RoutedEventArgs e)
        {
            if (isMute)
            {
                videoVolumeSlider.Value = lastVolume;
                isMute = false;
            }
            else
            {
                lastVolume = myVideoPlayer.Volume;
                videoVolumeSlider.Value = 0;
                isMute = true;
            }
        }

        private void ToggleFullScreen()
        {
            if (isFullScreen == false)
            {
                _originalParent = this.Parent;
                var panel = _originalParent as Panel;

                if (panel != null)
                {
                    panel.Children.Remove(this);

                    btnFullScreen.IsChecked = true;
                    VideoControlPanel.Visibility = Visibility.Visible;

                    fullScreen.parentPanel.Children.Add(this);
                    VideoPlayerRoot.Height = fullScreen.parentPanel.Height;
                    VideoPlayerRoot.Width = fullScreen.parentPanel.Width;

                    isFullScreen = true;

                    ShowControlPanel();
                    fullScreen.ShowDialog();
                }
            }
            else
            {
                isFullScreen = false;
                var parent = VisualTreeHelper.GetParent(this) as Panel;
                parent.Children.Remove(this);

                fullScreen.Hide();

                var originalParent = _originalParent as Panel;
                originalParent.Children.Add(this);

                VideoPlayerRoot.Height = originalParent.Height;
                VideoPlayerRoot.Width = originalParent.Width;

                HideControlPanel();
            }
        }

        private void PlayMedia()
        {
            myVideoPlayer.Play();
        }

        private void StopMedia()
        {
            myVideoPlayer.Stop();
        }

        private void PauseMedia()
        {
            myVideoPlayer.Pause();
        }

        private void videoThumbnail_TouchDown(object sender, TouchEventArgs e)
        {
            PlayMedia();
            InitializePropertyValues();
            isPlaying = true;
            btnPlay.IsChecked = true;
            videoThumbnail.Visibility = Visibility.Hidden;
            ShowControlPanel();
            ToggleFullScreen();
        }

        private void ShowControlPanel()
        {
            controlRow.Height = new GridLength(75, GridUnitType.Pixel);
            VideoControlPanel.Visibility = Visibility.Visible;

            panelTime = 0;
            panelTimer.Start();
        }

        private void HideControlPanel()
        {
            controlRow.Height = new GridLength(0, GridUnitType.Pixel);
           
            panelTime = 0;
            panelTimer.Stop();
        }

        private void HideThumbnail()
        {
            videoThumbnail.Visibility = Visibility.Collapsed;
            myVideoPlayer.Visibility = Visibility.Visible;
            controlRow.Height = new GridLength(75, GridUnitType.Pixel);
        }

        private void ShowThumbnail()
        {
            videoThumbnail.Visibility = Visibility.Visible;
            myVideoPlayer.Visibility = Visibility.Collapsed;
        }

        private void myVideoPlayer_MouseDown(object sender, MouseButtonEventArgs e)
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

            ShowControlPanel();
        }

        private void VideoPlayerRoot_Unloaded(object sender, RoutedEventArgs e)
        {
            myVideoPlayer.Stop();
        }
    }
}

