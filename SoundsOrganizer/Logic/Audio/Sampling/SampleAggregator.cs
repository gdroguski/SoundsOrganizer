﻿using System;
using NAudio.Dsp;
using NAudio.Wave;

namespace SoundsOrganizer.Logic.Audio.Sampling
{
    public class SampleAggregator : ISampleProvider
    {
        // Volume
        public event EventHandler<MaxSampleEventArgs> MaximumCalculated;
        public int NotificationCount { get; set; }
        private int _count;
        private float _maxValue;
        private float _minValue;

        // FFT
        public event EventHandler<FftEventArgs> FftCalculated;
        public bool PerformFFT { get; set; }
        private readonly Complex[] _fftBuffer;
        private readonly FftEventArgs _fftArgs;
        private int _fftPos;
        private readonly int _fftLength;
        private readonly int _dim;
        private readonly ISampleProvider _source;

        private readonly int _channels;

        public WaveFormat WaveFormat => _source.WaveFormat;

        public SampleAggregator(ISampleProvider source, int fftLength = 1024)
        {
            _channels = source.WaveFormat.Channels;
            if (!IsPowerOfTwo(fftLength))
            {
                throw new ArgumentException("FFT Length must be a power of two");
            }
            _dim = (int)Math.Log(fftLength, 2.0);
            _fftLength = fftLength;
            _fftBuffer = new Complex[fftLength];
            _fftArgs = new FftEventArgs(_fftBuffer);
            _source = source;
        }

        private bool IsPowerOfTwo(int x)
        {
            return (x & (x - 1)) == 0;
        }

        public void Reset()
        {
            _count = 0;
            _maxValue = _minValue = 0;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            var samplesRead = _source.Read(buffer, offset, count);

            for (var n = 0; n < samplesRead; n += _channels)
            {
                Add(buffer[n + offset]);
            }
            return samplesRead;
        }

        private void Add(float value)
        {
            if (PerformFFT && FftCalculated != null)
            {
                _fftBuffer[_fftPos].X = (float)(value * FastFourierTransform.HammingWindow(_fftPos, _fftLength));
                _fftBuffer[_fftPos].Y = 0;
                _fftPos++;
                if (_fftPos >= _fftBuffer.Length)
                {
                    _fftPos = 0;

                    FastFourierTransform.FFT(true, _dim, _fftBuffer);
                    FftCalculated(this, _fftArgs);
                }
            }

            _maxValue = Math.Max(_maxValue, value);
            _minValue = Math.Min(_minValue, value);
            _count++;
            if (_count >= NotificationCount && NotificationCount > 0)
            {
                MaximumCalculated?.Invoke(this, new MaxSampleEventArgs(_minValue, _maxValue));
                Reset();
            }
        }
    }
}
