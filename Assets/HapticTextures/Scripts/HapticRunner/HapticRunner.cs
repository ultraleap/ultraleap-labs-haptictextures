using System;
using Ultraleap.Haptics;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

/// <summary>
/// This script will generate a 'Circle' haptic, whose draw frequency and intensity is modulated based on values
/// calculated in the HapticRenderer script. Separate hand positions can be used for each hand in the scene. These positions
/// can be different to the scanning location set within HapticRenderer. Currently, the only sensation available is 
/// a 'Circle' haptic, however its radius can be altered in the Inspector window.
/// </summary>
public class HapticRunner : MonoBehaviour
{
    public HapticCircle Circle = new HapticCircle();
    private Library _library;
    private IDevice _device;
    private StreamingEmitter _streamingEmitter;
    private DateTime _startTime;

    private void OnEnable()
    {
        _library = new Library();
        try
        {
            _library.Connect();
        }
        catch
        {
            Debug.LogError("Could not connect to UL Haptic Daemon.");
            return;
        }

        try
        {
            _device = _library.FindDevice();
            Debug.Log("Device Connected.");
        }
        catch
        {
            Debug.LogError("Unable to connect to UL Haptic device.");
            return;
        }

        _streamingEmitter = new StreamingEmitter(_library);

        _streamingEmitter.Devices.Add(_device, new Ultraleap.Haptics.Transform());

        _streamingEmitter.SetControlPointCount(1, AdjustRate.AsRequired);
        _streamingEmitter.EmissionCallback = Callback;
        _streamingEmitter.Start();
        _startTime = DateTime.Now;
    }

    private void OnDisable()
    {
          // Dispose/destroy the emitter
        _streamingEmitter.Stop();
        _streamingEmitter.Dispose();
        _streamingEmitter = null;
    }

    // This callback is called every time the device is ready to accept new control point information
    private void Callback(StreamingEmitter emitter, StreamingEmitter.Interval interval, DateTimeOffset submissionDeadline)
    {
        // For each time point in this interval...
        foreach (var sample in interval)
        {
            double seconds = (sample.Time - _startTime).TotalSeconds;

            Vector3 pos = Circle.EvaluateAt(seconds);
            sample.Points[0].Position = pos;
            sample.Points[0].Intensity = Circle.Intensity;
        }
    }
}