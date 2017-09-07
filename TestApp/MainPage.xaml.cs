using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using MipWindowLib.MipRobot;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Media.Playback;
using Windows.Media.SpeechSynthesis;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TestApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        CanvasBitmap joystickBitmap;

        Size joystickCanvasSize;
        Rect joystickDrawRect;
        Vector2 joystickDirection;

        Timer joystickTimer;

        const int JOYSTICK_HALF_SIZE = 50;
        const float MOVE_RATIO_BY_JOYSTICK = 0.01f;
        const int MOVE_TICK_INTERVAL_IN_MS = 66;

        public class Sound
        {
            public MipRobotConstants.SOUND_FILE Value { get; set; }
            public string DisplayValue { get; set; }
        }

        public ObservableCollection<Sound> SoundCollection
        {
            get
            {
                return new ObservableCollection<Sound>
                {
                    new Sound() { DisplayValue ="ONEKHZ_500MS_8K16BIT", Value = MipRobotConstants.SOUND_FILE.ONEKHZ_500MS_8K16BIT },
                    new Sound() { DisplayValue ="ACTION_BURPING", Value = MipRobotConstants.SOUND_FILE.ACTION_BURPING },
                    new Sound() { DisplayValue = "ACTION_DRINKING", Value = MipRobotConstants.SOUND_FILE.ACTION_DRINKING },
                    new Sound() { DisplayValue = "ACTION_EATING", Value = MipRobotConstants.SOUND_FILE.ACTION_EATING },
                    new Sound() { DisplayValue = "ACTION_FARTING_SHORT", Value = MipRobotConstants.SOUND_FILE.ACTION_FARTING_SHORT },
                    new Sound() { DisplayValue = "ACTION_OUT_OF_BREATH", Value = MipRobotConstants.SOUND_FILE.ACTION_OUT_OF_BREATH },
                    new Sound() { DisplayValue = "BOXING_PUNCHCONNECT_1", Value = MipRobotConstants.SOUND_FILE.BOXING_PUNCHCONNECT_1 },
                    new Sound() { DisplayValue = "BOXING_PUNCHCONNECT_2", Value = MipRobotConstants.SOUND_FILE.BOXING_PUNCHCONNECT_2 },
                    new Sound() { DisplayValue = "BOXING_PUNCHCONNECT_3", Value = MipRobotConstants.SOUND_FILE.BOXING_PUNCHCONNECT_3 },
                    new Sound() { DisplayValue = "FREESTYLE_TRACKING_1", Value = MipRobotConstants.SOUND_FILE.FREESTYLE_TRACKING_1 },
                    new Sound() { DisplayValue = "MIP_1", Value = MipRobotConstants.SOUND_FILE.MIP_1 },
                    new Sound() { DisplayValue = "MIP_2", Value = MipRobotConstants.SOUND_FILE.MIP_2 },
                    new Sound() { DisplayValue = "MIP_3", Value =  MipRobotConstants.SOUND_FILE.MIP_3 },
                    new Sound() { DisplayValue = "MIP_APP", Value = MipRobotConstants.SOUND_FILE.MIP_APP },
                    new Sound() { DisplayValue = "MIP_AWWW", Value = MipRobotConstants.SOUND_FILE.MIP_AWWW },
                    new Sound() { DisplayValue = "MIP_BIG_SHOT", Value = MipRobotConstants.SOUND_FILE.MIP_BIG_SHOT },
                    new Sound() { DisplayValue = "MIP_BLEH", Value = MipRobotConstants.SOUND_FILE.MIP_BLEH },
                    new Sound() { DisplayValue = "MIP_BOOM", Value = MipRobotConstants.SOUND_FILE.MIP_BOOM },
                    new Sound() { DisplayValue = "MIP_BYE", Value = MipRobotConstants.SOUND_FILE.MIP_BYE },
                    new Sound() { DisplayValue = "MIP_CONVERSE_1", Value = MipRobotConstants.SOUND_FILE.MIP_CONVERSE_1 },
                    new Sound() { DisplayValue = "MIP_CONVERSE_2", Value = MipRobotConstants.SOUND_FILE.MIP_CONVERSE_2 },
                    new Sound() { DisplayValue = "MIP_DROP", Value = MipRobotConstants.SOUND_FILE.MIP_DROP },
                    new Sound() { DisplayValue = "MIP_DUNNO", Value = MipRobotConstants.SOUND_FILE.MIP_DUNNO },
                    new Sound() { DisplayValue = "MIP_FALL_OVER_1", Value = MipRobotConstants.SOUND_FILE.MIP_FALL_OVER_1 },
                    new Sound() { DisplayValue = "MIP_FALL_OVER_2", Value = MipRobotConstants.SOUND_FILE.MIP_FALL_OVER_2 },
                    new Sound() { DisplayValue = "MIP_FIGHT", Value = MipRobotConstants.SOUND_FILE.MIP_FIGHT },
                    new Sound() { DisplayValue = "MIP_GAME", Value = MipRobotConstants.SOUND_FILE.MIP_GAME },
                    new Sound() { DisplayValue = "MIP_GLOAT", Value = MipRobotConstants.SOUND_FILE.MIP_GLOAT },
                    new Sound() { DisplayValue = "MIP_GO", Value = MipRobotConstants.SOUND_FILE.MIP_GO },
                    new Sound() { DisplayValue = "MIP_GOGOGO", Value = MipRobotConstants.SOUND_FILE.MIP_GOGOGO },
                    new Sound() { DisplayValue = "MIP_GRUNT_1", Value = MipRobotConstants.SOUND_FILE.MIP_GRUNT_1 },
                    new Sound() { DisplayValue = "MIP_GRUNT_2", Value = MipRobotConstants.SOUND_FILE.MIP_GRUNT_2 },
                    new Sound() { DisplayValue = "MIP_GRUNT_3", Value = MipRobotConstants.SOUND_FILE.MIP_GRUNT_3 },
                    new Sound() { DisplayValue = "MIP_HAHA_GOT_IT", Value = MipRobotConstants.SOUND_FILE.MIP_HAHA_GOT_IT },
                    new Sound() { DisplayValue = "MIP_HI_CONFIDENT", Value = MipRobotConstants.SOUND_FILE.MIP_HI_CONFIDENT },
                    new Sound() { DisplayValue = "MIP_HI_NOT_SURE", Value = MipRobotConstants.SOUND_FILE.MIP_HI_NOT_SURE },
                    new Sound() { DisplayValue = "MIP_HI_SCARED", Value = MipRobotConstants.SOUND_FILE.MIP_HI_SCARED },
                    new Sound() { DisplayValue = "MIP_HUH", Value = MipRobotConstants.SOUND_FILE.MIP_HUH },
                    new Sound() { DisplayValue = "MIP_HUMMING_1", Value = MipRobotConstants.SOUND_FILE.MIP_HUMMING_1 },
                    new Sound() { DisplayValue = "MIP_HUMMING_2", Value = MipRobotConstants.SOUND_FILE.MIP_HUMMING_2 },
                    new Sound() { DisplayValue = "MIP_HURT", Value = MipRobotConstants.SOUND_FILE.MIP_HURT },
                    new Sound() { DisplayValue = "MIP_HUUURGH", Value = MipRobotConstants.SOUND_FILE.MIP_HUUURGH },
                    new Sound() { DisplayValue = "MIP_IN_LOVE", Value = MipRobotConstants.SOUND_FILE.MIP_IN_LOVE },
                    new Sound() { DisplayValue = "MIP_IT", Value = MipRobotConstants.SOUND_FILE.MIP_IT },
                    new Sound() { DisplayValue = "MIP_JOKE", Value = MipRobotConstants.SOUND_FILE.MIP_JOKE },
                    new Sound() { DisplayValue = "MIP_K", Value = MipRobotConstants.SOUND_FILE.MIP_K },
                    new Sound() { DisplayValue = "MIP_LOOP_1", Value = MipRobotConstants.SOUND_FILE.MIP_LOOP_1 },
                    new Sound() { DisplayValue = "MIP_LOOP_2", Value = MipRobotConstants.SOUND_FILE.MIP_LOOP_2 },
                    new Sound() { DisplayValue = "MIP_LOW_BATTERY", Value = MipRobotConstants.SOUND_FILE.MIP_LOW_BATTERY },
                    new Sound() { DisplayValue = "MIP_MIPPEE", Value = MipRobotConstants.SOUND_FILE.MIP_MIPPEE },
                    new Sound() { DisplayValue = "MIP_MORE", Value = MipRobotConstants.SOUND_FILE.MIP_MORE },
                    new Sound() { DisplayValue = "MIP_MUAH_HA", Value = MipRobotConstants.SOUND_FILE.MIP_MUAH_HA },
                    new Sound() { DisplayValue = "MIP_MUSIC", Value = MipRobotConstants.SOUND_FILE.MIP_MUSIC },
                    new Sound() { DisplayValue = "MIP_OBSTACLE", Value = MipRobotConstants.SOUND_FILE.MIP_OBSTACLE },
                    new Sound() { DisplayValue = "MIP_OHOH", Value = MipRobotConstants.SOUND_FILE.MIP_OHOH },
                    new Sound() { DisplayValue = "MIP_OH_YEAH = 56", Value = MipRobotConstants.SOUND_FILE.MIP_OH_YEAH },
                    new Sound() { DisplayValue = "MIP_OOPSIE = 57", Value = MipRobotConstants.SOUND_FILE.MIP_OOPSIE },
                    new Sound() { DisplayValue = "MIP_OUCH_1 = 58", Value = MipRobotConstants.SOUND_FILE.MIP_OUCH_1 },
                    new Sound() { DisplayValue = "MIP_OUCH_2 = 59", Value = MipRobotConstants.SOUND_FILE.MIP_OUCH_2 },
                    new Sound() { DisplayValue = "MIP_PLAY = 60", Value = MipRobotConstants.SOUND_FILE.MIP_PLAY },
                    new Sound() { DisplayValue = "MIP_PUSH = 61", Value = MipRobotConstants.SOUND_FILE.MIP_PUSH },
                    new Sound() { DisplayValue = "MIP_RUN = 62", Value = MipRobotConstants.SOUND_FILE.MIP_RUN },
                    new Sound() { DisplayValue = "MIP_SHAKE = 63", Value = MipRobotConstants.SOUND_FILE.MIP_SHAKE },
                    new Sound() { DisplayValue = "MIP_SIGH = 64", Value = MipRobotConstants.SOUND_FILE.MIP_SIGH },
                    new Sound() { DisplayValue = "MIP_SINGING = 65", Value = MipRobotConstants.SOUND_FILE.MIP_SINGING },
                    new Sound() { DisplayValue = "MIP_SNEEZE = 66", Value = MipRobotConstants.SOUND_FILE.MIP_SNEEZE },
                    new Sound() { DisplayValue = "MIP_SNORE = 67", Value = MipRobotConstants.SOUND_FILE.MIP_SNORE },
                    new Sound() { DisplayValue = "MIP_STACK = 68", Value = MipRobotConstants.SOUND_FILE.MIP_STACK },
                    new Sound() { DisplayValue = "MIP_SWIPE_1 = 69", Value = MipRobotConstants.SOUND_FILE.MIP_SWIPE_1 },
                    new Sound() { DisplayValue = "MIP_SWIPE_2 = 70", Value = MipRobotConstants.SOUND_FILE.MIP_SWIPE_2 },
                    new Sound() { DisplayValue = "MIP_TRICKS = 71", Value = MipRobotConstants.SOUND_FILE.MIP_TRICKS },
                    new Sound() { DisplayValue = "MIP_TRIIICK = 72", Value = MipRobotConstants.SOUND_FILE.MIP_TRIIICK },
                    new Sound() { DisplayValue = "MIP_TRUMPET = 73", Value = MipRobotConstants.SOUND_FILE.MIP_TRUMPET },
                    new Sound() { DisplayValue = "MIP_WAAAAA = 74", Value = MipRobotConstants.SOUND_FILE.MIP_WAAAAA },
                    new Sound() { DisplayValue = "MIP_WAKEY = 75", Value = MipRobotConstants.SOUND_FILE.MIP_WAKEY },
                    new Sound() { DisplayValue = "MIP_WHEEE = 76", Value = MipRobotConstants.SOUND_FILE.MIP_WHEEE },
                    new Sound() { DisplayValue = "MIP_WHISTLING = 77", Value = MipRobotConstants.SOUND_FILE.MIP_WHISTLING },
                    new Sound() { DisplayValue = "MIP_WHOAH = 78", Value = MipRobotConstants.SOUND_FILE.MIP_WHOAH },
                    new Sound() { DisplayValue = "MIP_WOO = 79", Value = MipRobotConstants.SOUND_FILE.MIP_WOO },
                    new Sound() { DisplayValue = "MIP_YEAH = 80", Value = MipRobotConstants.SOUND_FILE.MIP_YEAH },
                    new Sound() { DisplayValue = "MIP_YEEESSS = 81", Value = MipRobotConstants.SOUND_FILE.MIP_YEEESSS },
                    new Sound() { DisplayValue = "MIP_YO = 82", Value = MipRobotConstants.SOUND_FILE.MIP_YO },
                    new Sound() { DisplayValue = "MIP_YUMMY = 83", Value = MipRobotConstants.SOUND_FILE.MIP_YUMMY },
                    new Sound() { DisplayValue = "MOOD_ACTIVATED = 84", Value = MipRobotConstants.SOUND_FILE.MOOD_ACTIVATED },
                    new Sound() { DisplayValue = "MOOD_ANGRY = 85", Value = MipRobotConstants.SOUND_FILE.MOOD_ANGRY },
                    new Sound() { DisplayValue = "MOOD_ANXIOUS = 86", Value = MipRobotConstants.SOUND_FILE.MOOD_ANXIOUS },
                    new Sound() { DisplayValue = "MOOD_BORING = 87", Value = MipRobotConstants.SOUND_FILE.MOOD_BORING },
                    new Sound() { DisplayValue = "MOOD_CRANKY = 88", Value = MipRobotConstants.SOUND_FILE.MOOD_CRANKY },
                    new Sound() { DisplayValue = "MOOD_ENERGETIC = 89", Value = MipRobotConstants.SOUND_FILE.MOOD_ENERGETIC },
                    new Sound() { DisplayValue = "MOOD_EXCITED = 90", Value = MipRobotConstants.SOUND_FILE.MOOD_EXCITED },
                    new Sound() { DisplayValue = "MOOD_GIDDY = 91", Value = MipRobotConstants.SOUND_FILE.MOOD_GIDDY },
                    new Sound() { DisplayValue = "MOOD_GRUMPY = 92", Value = MipRobotConstants.SOUND_FILE.MOOD_GRUMPY },
                    new Sound() { DisplayValue = "MOOD_HAPPY = 93", Value = MipRobotConstants.SOUND_FILE.MOOD_HAPPY },
                    new Sound() { DisplayValue = "MOOD_IDEA = 94", Value = MipRobotConstants.SOUND_FILE.MOOD_IDEA },
                    new Sound() { DisplayValue = "MOOD_IMPATIENT = 95", Value = MipRobotConstants.SOUND_FILE.MOOD_IMPATIENT },
                    new Sound() { DisplayValue = "MOOD_NICE = 96", Value = MipRobotConstants.SOUND_FILE.MOOD_NICE },
                    new Sound() { DisplayValue = "MOOD_SAD = 97", Value = MipRobotConstants.SOUND_FILE.MOOD_SAD },
                    new Sound() { DisplayValue = "MOOD_SHORT = 98", Value = MipRobotConstants.SOUND_FILE.MOOD_SHORT },
                    new Sound() { DisplayValue = "MOOD_SLEEPY = 99", Value = MipRobotConstants.SOUND_FILE.MOOD_SLEEPY },
                    new Sound() { DisplayValue = "MOOD_TIRED = 100", Value = MipRobotConstants.SOUND_FILE.MOOD_TIRED },
                    new Sound() { DisplayValue = "SOUND_BOOST = 101", Value = MipRobotConstants.SOUND_FILE.SOUND_BOOST },
                    new Sound() { DisplayValue = "SOUND_CAGE = 102", Value = MipRobotConstants.SOUND_FILE.SOUND_CAGE },
                    new Sound() { DisplayValue = "SOUND_GUNS = 103", Value = MipRobotConstants.SOUND_FILE.SOUND_GUNS },
                    new Sound() { DisplayValue = "SOUND_ZINGS = 104", Value = MipRobotConstants.SOUND_FILE.SOUND_ZINGS },
                    new Sound() { DisplayValue = "SHORT_MUTE_FOR_STOP = 105", Value = MipRobotConstants.SOUND_FILE.SHORT_MUTE_FOR_STOP },
                    new Sound() { DisplayValue = "FREESTYLE_TRACKING_2 = 106", Value = MipRobotConstants.SOUND_FILE.FREESTYLE_TRACKING_2 }
                };
            }
        }

        public class HeadLight
        {
            public MipRobotConstants.HEAD_LED Value { get; set; }
            public string DisplayValue { get; set; }
        }

        public ObservableCollection<HeadLight> HeadLightCollection
        {
            get
            {
                return new ObservableCollection<HeadLight>
                {
                    new HeadLight() { DisplayValue ="OFF", Value = MipRobotConstants.HEAD_LED.OFF },
                    new HeadLight() { DisplayValue ="ON", Value = MipRobotConstants.HEAD_LED.ON },
                    new HeadLight() { DisplayValue = "BLINK_SLOW", Value = MipRobotConstants.HEAD_LED.BLINK_SLOW },
                    new HeadLight() { DisplayValue = "BLINK_FAST", Value = MipRobotConstants.HEAD_LED.BLINK_FAST },
                };
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
            DataContext = this;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            LogWindow.Text += "Connecting\n";
            if (sender == this.ConnectButton)
            {
                await MipRobotFinder.Instance.ScanForRobots();

                MipRobot mip = MipRobotFinder.Instance.FoundRobotList.FirstOrDefault();
                if (mip != null)
                {
                    if (await mip.Connect())
                    {
                        this.ConnectButton.Content = "Connected: " + mip.DeviceName;

                        this.ConnectButton.IsEnabled = false;
                        this.DriveButton.IsEnabled = true;
                        this.DriveCanvas.IsEnabled = true;
                        this.PlaySoundButton.IsEnabled = true;
                        this.ChangeChestButton.IsEnabled = true;
                        this.FalloverButton.IsEnabled = true;

                        mip.MipPositionHandler += Mip_MipPositionHandler;
                        mip.DidConnectedEvent += Mip_DidConnectedEvent;
                        mip.DidDisconnectedEvent += Mip_DidDisconnectedEvent;
                        mip.MipToyActivationStatusHandler += Mip_MipToyActivationStatusHandler;
                        mip.MipGameModeHandler += Mip_MipGameModeHandler;
                        mip.MipVolumeLevelHandler += Mip_MipVolumeLevelHandler;
                        mip.MipWeightLevelHandler += Mip_MipWeightLevelHandler;
                        mip.MipLeaningForwardHandler += Mip_MipLeaningForwardHandler;
                    }
                }
                else
                {
                    await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings-bluetooth:"));
                }
            }
            else if (sender == DriveButton)
            {

            }
            else if (sender == this.FalloverButton)
            {
                await MipRobotFinder.Instance.FirstConnectedRobot().MipFalloverWithSytle(MipRobotConstants.POSITION_VALUE.ON_BACK);
            }
        }

        private void Mip_MipVolumeLevelHandler(object sender, int e)
        {
            
        }

        private void Mip_MipGameModeHandler(object sender, byte e)
        {

        }

        private void Mip_MipWeightLevelHandler(object sender, byte e)
        {

        }

        private void Mip_MipLeaningForwardHandler(object sender, bool leaning)
        {

        }


        private void Mip_MipToyActivationStatusHandler(object sender, BluetoothRobotControlLib.Common.BluetoothRobotConstants.ACTIVATION_STATUS e)
        {
            LogWindow.Text += string.Format("ACTIVATION {0}\n", e);
        }

        private void Mip_DidDisconnectedEvent(object sender, BluetoothRobotControlLib.Common.BluetoothRobot e)
        {
            LogWindow.Text += string.Format("Disconnected\n");
        }

        private void Mip_DidConnectedEvent(object sender, BluetoothRobotControlLib.Common.BluetoothRobot e)
        {
            //LogWindow.Text += string.Format("Connected\n");
        }

        private void Mip_MipPositionHandler(object sender, MipRobotConstants.POSITION_VALUE e)
        {
            // Different thread
            //LogWindow.Text += string.Format("POSITION {0}\n", e);
        }

        void DriveCanvas_CreateResourcesEvent(CanvasControl control, CanvasCreateResourcesEventArgs args)
        {
            args.TrackAsyncAction(DriveCanvas_CreateResourcesAsync(control).AsAsyncAction());
        }

        async Task DriveCanvas_CreateResourcesAsync(CanvasControl sender)
        {
            joystickBitmap = await CanvasBitmap.LoadAsync(sender, "Assets/Joystick.png");

            joystickCanvasSize = new Size(sender.ActualWidth, sender.ActualHeight);

            joystickDrawRect = new Rect(0, 0, JOYSTICK_HALF_SIZE*2, JOYSTICK_HALF_SIZE*2);
            joystickDirection = new Vector2();

            UpdateJoystick(sender, joystickCanvasSize.Width / 2, joystickCanvasSize.Height / 2, true);

            joystickTimer = new Timer(MipMove, this, MOVE_TICK_INTERVAL_IN_MS, Timeout.Infinite);
        }

        void DriveCanvas_DrawEvent(object sender, CanvasDrawEventArgs args)
        {
            args.DrawingSession.DrawImage(joystickBitmap, joystickDrawRect);
        }

        private void DriveCanvas_PointerPressedEvent(object sender, PointerRoutedEventArgs e)
        {
            CanvasControl control = (CanvasControl)sender;
            if (e.Pointer.IsInContact)
            {
                Point pos = e.GetCurrentPoint(control).Position;
                UpdateJoystick(control, pos.X, pos.Y);
            }
        }

        private void DriveCanvas_PointerReleasedEvent(object sender, PointerRoutedEventArgs e)
        {
            CanvasControl control = (CanvasControl)sender;

                Point pos = e.GetCurrentPoint(control).Position;
                UpdateJoystick(control, joystickCanvasSize.Width / 2, joystickCanvasSize.Height / 2);
        }

        private void DriveCanvas_PointerMovedEvent(object sender, PointerRoutedEventArgs e)
        {
            CanvasControl control = (CanvasControl)sender;
            if (e.Pointer.IsInContact)
            {
                Point pos = e.GetCurrentPoint(control).Position;
                UpdateJoystick(control, pos.X, pos.Y);
            }
        }

        private void UpdateJoystick(CanvasControl control, double posX, double posY, bool isFirstTime=false)
        {
            joystickDirection.X = (float)(posX - joystickCanvasSize.Width/2);
            joystickDirection.Y = (float)(posY - joystickCanvasSize.Height/2);

            float overRatio = joystickDirection.Length() / (float)JOYSTICK_HALF_SIZE;
            if (overRatio > 1.0f)
            {
                joystickDirection.X /= overRatio;
                joystickDirection.Y /= overRatio;
            }

            joystickDrawRect.X = joystickCanvasSize.Width / 2 + joystickDirection.X - JOYSTICK_HALF_SIZE;
            joystickDrawRect.Y = joystickCanvasSize.Height / 2 + joystickDirection.Y - JOYSTICK_HALF_SIZE;

            if (!isFirstTime)
            {
                control.Invalidate();
            }
        }

        bool stopped = false;
        private void MipMove(object state)
        {
            if (joystickDirection.Length() > 0.0001f)
            {
                Vector2 mov = new Vector2(joystickDirection.X, -joystickDirection.Y);
                mov *= MOVE_RATIO_BY_JOYSTICK;

                MipRobotFinder.Instance.FoundRobotList.FirstOrDefault()?.MipDrive(mov);
                stopped = false;
            }
            else
            {
                if (!stopped)
                {
                    MipRobotFinder.Instance.FoundRobotList.FirstOrDefault()?.MipStop();
                    stopped = true;
                }
            }

            joystickTimer.Change(MOVE_TICK_INTERVAL_IN_MS, Timeout.Infinite);
        }

        private void Log(string message)
        {
            LogWindow.Text += message;
            LogWindow.Text += "\n";
        }

        private void DriveCanvas_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private async void PlaySoundButton_Click(object sender, RoutedEventArgs e)
        {
            var robot = MipRobotFinder.Instance.FirstConnectedRobot();
            if (robot != null)
            {
                //await robot.SetMipVolumeLevel(1);
                await robot.PlayMipSound(new MipRobotSound((MipRobotConstants.SOUND_FILE)Sounds.SelectedValue));
            }
        }

        private async void ChangeChestButton_Click(object sender, RoutedEventArgs e)
        {
            var robot = MipRobotFinder.Instance.FirstConnectedRobot();
            if (robot != null)
            {
                // rgb values
                await robot.SetMipChestLedWithColor(0xff, 0xff, 0xff, 1);
            }
        }

        private async void HeadLed1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var robot = MipRobotFinder.Instance.FirstConnectedRobot();
            if (robot != null)
            {
                var led = (MipRobotConstants.HEAD_LED) HeadLed1.SelectedValue;
                await robot.SetMipHeadLeds(led, led, led, led);
            }
        }

        private async void VolumeSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            var robot = MipRobotFinder.Instance.FirstConnectedRobot();
            if (robot != null)
            {
                await robot.SetMipVolumeLevel((byte)e.NewValue);
            }
        }

        private async void Disabled_Click(object sender, RoutedEventArgs e)
        {
            var robot = MipRobotFinder.Instance.FirstConnectedRobot();
            if (robot != null)
            {
                await robot.SetGestureRadarMode(MipRobotConstants.GESTURE_OR_RADAR_MODE.DISABLED);
            }
        }

        private async void GestureButton_Click(object sender, RoutedEventArgs e)
        {
            var robot = MipRobotFinder.Instance.FirstConnectedRobot();
            if (robot != null)
            {
                await robot.SetGestureRadarMode(MipRobotConstants.GESTURE_OR_RADAR_MODE.GESTURE);
            }
        }

        private async void RadarButton_Click(object sender, RoutedEventArgs e)
        {
            var robot = MipRobotFinder.Instance.FirstConnectedRobot();
            if (robot != null)
            {
                await robot.SetGestureRadarMode(MipRobotConstants.GESTURE_OR_RADAR_MODE.RADAR);
            }
        }

        private async void ChestSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            var robot = MipRobotFinder.Instance.FirstConnectedRobot();
            if (robot != null)
            {
                await robot.SetMipChestLedWithColor((byte)ChestRSlider.Value, (byte)ChestGSlider.Value, (byte)ChestBSlider.Value, (byte)ChestFadeSlider.Value);
            }
        }

        private void media_MediaEnded(object sender, RoutedEventArgs e)
        {

        }

        private void Left90Button_Click(object sender, RoutedEventArgs e)
        {
            MipRobotFinder.Instance.FoundRobotList.FirstOrDefault()?.MipPunchLeftWithSpeed(10);
        }

        private void Left10Button_Click(object sender, RoutedEventArgs e)
        {
            MipRobotFinder.Instance.FoundRobotList.FirstOrDefault()?.MipTurnLeftByDegrees(10, 10);
        }

        private void Right10Button_Click(object sender, RoutedEventArgs e)
        {
            MipRobotFinder.Instance.FoundRobotList.FirstOrDefault()?.MipTurnRightByDegrees(10, 10);
        }

        private void Right90Button_Click(object sender, RoutedEventArgs e)
        {
            MipRobotFinder.Instance.FoundRobotList.FirstOrDefault()?.MipPunchRightWithSpeed(10);
        }
    }
}
