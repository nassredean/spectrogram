using UnityEngine;
using System.Collections;

public class AudioSpectrum : MonoBehaviour
{
    // Frequencies for standard 1/3 octave bands
    private static readonly float[] centerFrequencies = { 12.5f, 16, 20, 25, 31.5f, 40, 50, 63, 80, 100, 125, 160, 200, 250, 315, 400, 500, 630, 800, 1000, 1250, 1600, 2000, 2500, 3150, 4000, 5000, 6300, 8000, 10000, 12500, 16000, 20000 };
    private static readonly float[] lowerFrequencies = { 11.2f, 14.1f, 17.8f, 22.4f, 28.2f, 35.5f, 44.7f, 56.2f, 70.8f, 89.1f, 112, 141, 178, 224, 282, 355, 447, 562, 708, 891,  1122, 1413, 1778, 2239, 2818, 3548, 4467, 5623, 7079, 8913,  11220, 14130, 17780 };
    private static readonly float[] upperFrequencies = { 14.1f, 17.8f, 22.4f, 28.2f, 35.5f, 44.7f, 56.2f, 70.8f, 89.1f, 112,   141, 178, 224, 282, 355, 447, 562, 708, 891, 1122, 1413, 1778, 2239, 2818, 3548, 4467, 5623, 7079, 8913, 11220, 14130, 17780, 22390 };
    //  A one-third octave band is defined as a frequency band whose upper band-edge frequency (f2) is the lower band frequency (f1) times the cube root of two which is 2^(1/3) ~= 1.260
    private static readonly float bandwidth = Mathf.Pow(2, 1f / 3f);
    // Number of values (the length of the samples array provided) must be a power of 2.
    // At an output sample rate of 44100 and 4096 samples, our frequencyResolution = 4100/2/4096 = 5.38HZ (each element refers to a frequency 5.38HZ higher than the previous one)
    private static readonly int numberOfSamples = 4096;
    float[] spectrum;
    float[] octaveBands;

    public float[] OctaveBands
    {
        get { return octaveBands; }
    }

    void InitializeArrays()
    {
        if (spectrum == null)
        {
            spectrum = new float[numberOfSamples];
        }
        if (octaveBands == null)
        {
            octaveBands = new float[centerFrequencies.Length];
        }
    }

    int FrequencyToSpectrumIndex(float f)
    {
        /* 
        *  AudioSettings.outputSampleRate is how fast samples are taken.
        *  Picture an analog audio track. A "sample" is a measurement at one specific time in that audio track.
        *  How often that snapshot is taken represents the sample rate or sampling frequency.
        *  It’s measured in "samples per second" and is usually expressed in kiloHertz (kHz), a unit meaning 1,000 times per second.
        *  Audio CDs, for example, have a sample rate of 44.1kHz, which means that the analog signal is sampled 44,100 times per second.
        *  
        *  If we have a sample rate of 44100 and 4096 samples, then we have 44100 / 4096 ~= 10.77 hertz per sample
        *  
        */
        // float hertzPerSample = AudioSettings.outputSampleRate / spectrum.Length;
        // return Mathf.FloorToInt(f / hertzPerSample);
        int i = Mathf.FloorToInt((f / AudioSettings.outputSampleRate) * 2.0f * spectrum.Length);
        return Mathf.Clamp(i, 0, spectrum.Length - 1);
    }

    void Awake()
    {
        InitializeArrays();
    }

    void Update()
    {
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        for (var bandIndex = 0; bandIndex < octaveBands.Length; bandIndex++)
        {
            // Get the minimum and maximum frequencies as indexes into the spectrum array
            int imin = FrequencyToSpectrumIndex(lowerFrequencies[bandIndex]);
            int imax = FrequencyToSpectrumIndex(upperFrequencies[bandIndex]);

            var meanLevels = 0.0f;
            // Iterate over the spectrum indexes that make the octave band, find the mean
            for (int i = imin; i <= imax; i++)
            {
                meanLevels += spectrum[i];
            }
            octaveBands[bandIndex] = meanLevels / (imax - imin + 1);
        }
    }
}

