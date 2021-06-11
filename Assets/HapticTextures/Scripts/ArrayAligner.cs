using Ultraleap.Haptics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

/// <summary>
/// This script will first check to see which array is currently plugged in to the pc. 
/// It will then adjust the positioning of the Lead Controller origin depending on whether 
/// it has found an Ultrahaptics Stratos Inspire or Explore haptics array.
/// </summary>

public class ArrayAligner : MonoBehaviour
{
    private const string USI = "USI";
    private const string USX = "USX";

    [SerializeField]
    private GameObject _leapHandController;

    private string _deviceInfo;
    private Library _library;
    private IDevice _device;

    private void Awake()
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
        }
        catch
        {
            Debug.LogError("Unable to connect to UL Haptic device.");
            return;
        }


        _deviceInfo =_device.Identifier;

        if (_deviceInfo.Contains("USI"))
        {
            _leapHandController.transform.position = new Vector3(0, -0.00006f, -0.089f);
        }

        if (_deviceInfo.Contains("USX"))
        {
            _leapHandController.transform.position = new Vector3(0, 0, 0.121f);
        }
    }
}
