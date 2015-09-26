using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using AudioAnalyzer.Annotations;
using AudioAnalyzer.RndStuff;
using CUE.NET;
using CUE.NET.Devices.Keyboard;
using CUE.NET.Devices.Keyboard.Brushes;
using CUE.NET.Devices.Keyboard.Keys;
using CUE.NET.Exceptions;
using OpenTK.Audio;

namespace AudioAnalyzer
{
    //TODO DarthAffe 26.09.2015: Refactor everything ...
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Properties & Fields

        private readonly CorsairKeyboard _keyboard;
        private readonly AudioService _audioService;
        private readonly FFTSpectrumBrush _spectrumBrush = new FFTSpectrumBrush();
        private readonly RainbowBrush _rainbowBrush = new RainbowBrush { Alpha = 64 };

        private IEnumerable<string> _availableAudioDevices;
        public IEnumerable<string> AvailableAudioDevices => _availableAudioDevices ?? (_availableAudioDevices = AudioCapture.AvailableDevices);

        private string _selectedAudioDevice = AudioCapture.DefaultDevice;
        public string SelectedAudioDevice
        {
            get { return _selectedAudioDevice; }
            set
            {
                _selectedAudioDevice = value;
                OnPropertyChanged();
            }
        }

        private bool _showRainbow = true;
        public bool ShowRainbow
        {
            get { return _showRainbow; }
            set
            {
                if (_showRainbow != value)
                    _keyboard.Brush = value ? (IBrush)_rainbowBrush : new SolidColorBrush(Color.FromArgb(64, 0, 0, 0));

                _showRainbow = value;
                OnPropertyChanged();
            }
        }
        private int _spectrumAVal = 255;
        private string _spectrumA = "255";
        public string SpectrumA
        {
            get { return _spectrumA; }
            set
            {
                _spectrumA = value;
                int val;
                if (string.IsNullOrEmpty(value))
                    val = 0;
                else if (!int.TryParse(value, out val))
                    return;

                if (val < 0) val = 0;
                else if (val > 255) val = 255;

                _spectrumAVal = val;
                OnPropertyChanged();

                _spectrumBrush.Color = Color.FromArgb(_spectrumAVal, _spectrumRVal, _spectrumGVal, _spectrumBVal);
            }
        }

        private int _spectrumRVal = 255;
        private string _spectrumR = "255";
        public string SpectrumR
        {
            get { return _spectrumR; }
            set
            {
                _spectrumR = value;
                int val;
                if (string.IsNullOrEmpty(value))
                    val = 0;
                else if (!int.TryParse(value, out val))
                    return;

                if (val < 0) val = 0;
                else if (val > 255) val = 255;

                _spectrumRVal = val;
                OnPropertyChanged();

                _spectrumBrush.Color = Color.FromArgb(_spectrumAVal, _spectrumRVal, _spectrumGVal, _spectrumBVal);
            }
        }

        private int _spectrumGVal = 255;
        private string _spectrumG = "255";
        public string SpectrumG
        {
            get { return _spectrumG; }
            set
            {
                _spectrumG = value;
                int val;
                if (string.IsNullOrEmpty(value))
                    val = 0;
                else if (!int.TryParse(value, out val))
                    return;

                if (val < 0) val = 0;
                else if (val > 255) val = 255;

                _spectrumGVal = val;
                OnPropertyChanged();

                _spectrumBrush.Color = Color.FromArgb(_spectrumAVal, _spectrumRVal, _spectrumGVal, _spectrumBVal);
            }
        }

        private int _spectrumBVal = 255;
        private string _spectrumB = "255";
        public string SpectrumB
        {
            get { return _spectrumB; }
            set
            {
                _spectrumB = value;
                int val;
                if (string.IsNullOrEmpty(value))
                    val = 0;
                else if (!int.TryParse(value, out val))
                    return;

                if (val < 0) val = 0;
                else if (val > 255) val = 255;

                _spectrumBVal = val;
                OnPropertyChanged();

                _spectrumBrush.Color = Color.FromArgb(_spectrumAVal, _spectrumRVal, _spectrumGVal, _spectrumBVal);
            }
        }

        private bool _isRunning;
        public bool IsRunning
        {
            get { return _isRunning; }
            set
            {
                _isRunning = value;
                OnPropertyChanged();
                StartCommand.RaiseCanExecuteChanged();
                StopCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region Commands

        private ActionCommand _startCommand;
        public ActionCommand StartCommand => _startCommand ?? (_startCommand = new ActionCommand(Start, () => !IsRunning));

        private ActionCommand _stopCommand;
        public ActionCommand StopCommand => _stopCommand ?? (_stopCommand = new ActionCommand(Stop, () => IsRunning));

        #endregion

        #region Constructors

        public MainWindowViewModel()
        {
            CueSDK.Initialize();
            _keyboard = CueSDK.KeyboardSDK;
            if (_keyboard == null)
                throw new WrapperException("No keyboard found");

            _keyboard.Brush = ShowRainbow ? (IBrush)_rainbowBrush : new SolidColorBrush(Color.FromArgb(64, 0, 0, 0));
            new ListKeyGroup(_keyboard, _keyboard.Keys.ToArray()) { Brush = ShowRainbow ? (IBrush)_spectrumBrush : new SolidColorBrush(Color.FromArgb(64, 0, 0, 0)) };
            _audioService = new AudioService(Update);
        }

        #endregion

        #region Methods

        private void Update(byte[] fftData)
        {
            _spectrumBrush.Update(fftData);
            _keyboard.UpdateLeds();

            _rainbowBrush.StartHue += 4;
            _rainbowBrush.EndHue += 4;
        }

        private void Start()
        {
            IsRunning = true;
            _audioService.Start(SelectedAudioDevice);
        }

        private void Stop()
        {
            //TODO DarthAffe 26.09.2015: Fix crash at restart
            //IsRunning = false;
            _audioService.Stop();
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
