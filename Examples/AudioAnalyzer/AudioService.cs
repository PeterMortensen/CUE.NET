using System;
using System.ComponentModel;
using System.Threading;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace AudioAnalyzer
{
    public class AudioService
    {
        #region Properties & Fields

        LomontFFT _fftTransoformer = new LomontFFT();
        private AudioCapture _audioCapture;
        private BackgroundWorker _worker;
        private AutoResetEvent _workerCancelEvent = new AutoResetEvent(false);

        private Action<byte[]> _fftDataCallback;

        #endregion

        #region Constructors

        public AudioService(Action<byte[]> fftDataCallback)
        {
            this._fftDataCallback = fftDataCallback;
        }

        #endregion

        #region Methods

        public void Start(string audioDevice)
        {
            if (_worker != null) return;

            _audioCapture = new AudioCapture(audioDevice, 8000, ALFormat.Mono8, 256);
            _audioCapture.CheckErrors();

            _worker = new BackgroundWorker { WorkerSupportsCancellation = true };
            _worker.DoWork += ProcessData;
            _worker.RunWorkerAsync();
        }

        public void Stop()
        {
            if (_worker == null) return;

            if (_worker.IsBusy)
            {
                _worker.CancelAsync();
                _workerCancelEvent.WaitOne();
            }

            _worker.Dispose();
            _worker = null;
            _audioCapture.Stop();
        }

        // Calculation is taken from https://github.com/billism1/KeyboardAudio/blob/master/KeyboardAudio/Program.cs as of 29.09.2015
        private void ProcessData(object sender, DoWorkEventArgs args)
        {
            try
            {
                const float AMPLITUDE = 10.0f;

                byte[] audioBuffer = new byte[256];
                byte[] fftData = new byte[256];
                double[] fft = new double[256];
                //double fftavg = 0;

                _audioCapture.Start();
                while (!_worker.CancellationPending)
                {
                    // reset mem
                    for (int i = 0; i < 256; i++)
                    {
                        audioBuffer[i] = 0;
                        fftData[i] = 0;
                        fft[i] = 0;
                    }

                    _audioCapture.ReadSamples(audioBuffer, 256);
                    for (int i = 0; i < 256; i++)
                        fft[i] = (audioBuffer[i] - 128)*AMPLITUDE;

                    _fftTransoformer.TableFFT(fft, true);
                    for (int i = 0; i < 256; i += 2)
                    {
                        double fftmag = Math.Sqrt((fft[i]*fft[i]) + (fft[i + 1]*fft[i + 1]));
                        //fftavg += fftmag;
                        fftData[i] = (byte) fftmag;
                        fftData[i + 1] = fftData[i];
                    }
                    //fftavg /= 10;

                    _fftDataCallback?.Invoke(fftData);

                    Thread.Sleep(20);
                }
                _workerCancelEvent.Set();
            }
            catch (Exception ex)
            {
            }
        }

        #endregion
    }
}
