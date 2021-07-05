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
    private Library _library;
    private IDevice _device;
    private SensationEmitter _sensationEmitter;
    private SensationPackage _sensationPackage;
    private Sensation _sensation;
    private Sensation.Instance _sensationInstance;
    public Sensation.Instance sensation { get{ return _sensationInstance;} }
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

        _sensationEmitter = new SensationEmitter(_library);

        _sensationEmitter.Devices.Add(_device, new Ultraleap.Haptics.Transform());

        try
        {
            _sensationPackage = SensationPackage.LoadFromFile(_library, Application.streamingAssetsPath + "/StandardSensations.ssp");
        }
        catch
        {
            Debug.LogError("No Sensation Package Loaded.");
            return;
        }
        
        try
        {
            _sensation = _sensationPackage.GetSensation("CircleWithFixedFrequency");
            _sensationInstance = _sensation.MakeInstance();

            // Initialise sensation with base params.
            _sensationEmitter.SetSensation(_sensationInstance);
            _sensationInstance.Set("position",new[] {0f,0f,0f});
            _sensationInstance.Set("radius",0f);
            _sensationInstance.Set("frequency",0f);
            _sensationInstance.Set("intensity",0);
            _sensationEmitter.UpdateSensationArguments(_sensationInstance);
        }
        catch(Exception ex)
        {
            Debug.LogError("ERROR! " + ex.ToString());
            _sensationEmitter = null;
        }
        _sensationEmitter?.Start();
        _startTime = DateTime.Now;
    }

    public void SetArgument(string argument, UnityEngine.Vector3 value)
    {
        if(_sensationInstance == null)
            return;
        _sensationInstance.Set(argument,new[]{value.x,value.z,value.y});
        _sensationEmitter.UpdateSensationArguments(_sensationInstance);
    }

    public void SetArgument(string argument, float value)
    {
        if(_sensationInstance == null)
            return;
        _sensationInstance.Set(argument,value);
        _sensationEmitter.UpdateSensationArguments(_sensationInstance);
    }

    public void SetArgument(string argument, UnityEngine.Matrix4x4 value)
    {
        if(_sensationInstance == null)
            return;

        _sensationInstance.Set(argument, new[]{
            value[0,0],value[0,1],value[0,2],
            value[1,0],value[1,1],value[1,2],
            value[2,0],value[2,1],value[2,2]});
        _sensationEmitter.UpdateSensationArguments(_sensationInstance);
    }

    private void OnDisable()
    {
          // Dispose/destroy the emitter
        _sensationEmitter?.Stop();
        _library?.Disconnect();
        _library?.Dispose();
    }
}