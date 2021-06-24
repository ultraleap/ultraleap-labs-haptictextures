using UnityEngine;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using System;

/// <summary>
/// This script will evaluate a particular texture within a Unity scene provided it has been hit from each 
/// raycast projected from the provided hand scanning position.This process will be done independently for each hand
/// in the scene. For each texture, its height map is evaluated and an intensity value is calculated and sent to the
/// HapticRunner script in order to set the appropriate values for each haptic sensation.Additional modulation can be
/// applied that utilises a user's hand velocity to modulate both haptic sensation draw frequency and intensity.
/// </summary>

public class HapticRenderer : MonoBehaviour
{

    [System.Serializable]
    public class TextureHandProperties
    {
        public Chirality chirality;
        public Transform scanPosition = null;
        public Transform hapticPosition = null;
        [HideInInspector]
        public Matrix4x4 m44 = new Matrix4x4();
        public RaycastHit handRayResult;        
        [ReadOnly] public Vector3 handVelocity = Vector3.zero;
        [ReadOnly] public float handMagnitude = 0f;
    }

    private HandData _handData;

    private HapticRunner _hapticRunner;

    [SerializeField]
    private LeapProvider _leapProvider = null;

    [SerializeField]
    private List<TextureHandProperties> _hands = new List<TextureHandProperties>();

    [SerializeField]
    private float _handVelocityThreshold, _raycastLengthDown;

    [SerializeField]
    private AnimationCurve _frequencyCurve, _intensityCurve;

    [SerializeField,Range(0.001f,0.05f)]
    private float _hapticRadius = 0.02f;

    [SerializeField]
    private bool _modulateIntensityByHandVelocity = false, _modulateFrequencyByHandVelocity = false, _alwaysOn = false;

    private float _currentHandVelocityThreshold;

    private void OnEnable()
    {
        _handData = FindObjectOfType<HandData>();
        _hapticRunner = FindObjectOfType<HapticRunner>();
        if (_leapProvider == null)
        {
            _leapProvider = Hands.Provider;
        }
        if (_leapProvider != null)
        {
            _leapProvider.OnUpdateFrame += LeapProviderOnUpdateFrame;
        }
    }

    private void OnDisable()
    {
        if(_leapProvider != null)
        {
            _leapProvider.OnUpdateFrame -= LeapProviderOnUpdateFrame;
        }
    }

    void LeapProviderOnUpdateFrame(Frame frame)
    {
        if(_handData.currentHand == null)
        {
            _hapticRunner.SetArgument("intensity", 0);
            return;
        }

        for (int i = 0; i < _hands.Count; i++)
        {
            if(Convert.ToInt32(_handData.currentHand.IsRight) == (int)_hands[i].chirality)
            {
                UpdateHand(_hands[i], _handData.currentHand);
                break;
            }
        }
    }

    private void UpdateHand(TextureHandProperties _textureHand, Hand _currentHand)
    {
        _textureHand.handVelocity = _currentHand.PalmVelocity.ToVector3();
        _textureHand.handMagnitude = _textureHand.handVelocity.magnitude;
        _hapticRunner.SetArgument("position",_textureHand.hapticPosition.position);
        _textureHand.m44.SetTRS(Vector3.zero,_textureHand.hapticPosition.rotation,Vector3.one);
        _hapticRunner.SetArgument("radius",_hapticRadius);
        CalcScanFeatures(_textureHand, RaycastToTexture(_textureHand.scanPosition, _raycastLengthDown, out _textureHand.handRayResult));
    }

    private void CalcScanFeatures(TextureHandProperties _hand, TextureAttributes _attribute)
    {
        if (_alwaysOn)
        {
            _currentHandVelocityThreshold = 0;
        }
        else
        {
            _currentHandVelocityThreshold = _handVelocityThreshold;
        }
        if (_attribute != null && _hand.handMagnitude > _currentHandVelocityThreshold)
        {
                if (_modulateIntensityByHandVelocity)
                {
                    _hapticRunner.SetArgument("intensity",HeightValue(
                                _hand.handRayResult.textureCoord,
                                _attribute.heightMap,
                                _attribute.texture.intensityMin,
                                _attribute.texture.intensityMax * GetModulatedIntensityByHandVelocity(_hand.handMagnitude)));
                }
                else
                {
                    _hapticRunner.SetArgument("intensity", HeightValue(
                                _hand.handRayResult.textureCoord,
                                _attribute.heightMap,
                                _attribute.texture.intensityMin,
                                _attribute.texture.intensityMax));
                }
                if (_modulateFrequencyByHandVelocity)
                {
                    _hapticRunner.SetArgument("frequency",_attribute.texture.drawFrequency + ((_attribute.texture.drawFrequency / 2) * GetModulatedFrequencyByHandVelocity(_hand.handMagnitude)));
                }
                else
                {
                    _hapticRunner.SetArgument("frequency", _attribute.texture.drawFrequency);
                }
        }
        else
        {
            _hapticRunner.SetArgument("intensity",0);
        }
    }

    private float GetModulatedFrequencyByHandVelocity(float _velocity)
    {
        if (_velocity > _handVelocityThreshold)
        {
            return _frequencyCurve.Evaluate(_velocity);
        }
        return 0f;
    }    
    
    private float GetModulatedIntensityByHandVelocity(float _velocity)
    {
        if (_velocity > _handVelocityThreshold)
        {
            return _intensityCurve.Evaluate(_velocity);
        }
        return 0f;
    }

    private TextureAttributes RaycastToTexture(Transform _transform, float _rayDistance, out RaycastHit _raycastHit)
    {
        TextureAttributes ta = null;
        if (Physics.Raycast(_transform.position, -_transform.up, out _raycastHit, _rayDistance))
        {
            if (_raycastHit.transform != null)
            {
                ta = _raycastHit.transform.GetComponent<TextureAttributes>();
                if (ta != null && ta.texture != null)
                {
                    return ta;
                }
            }
        }
        return ta;
    }

    private float HeightValue(Vector2 textureLocation, Texture2D heightMap, float minInt, float maxInt)
    {
        return (heightMap.GetPixel(
            (int)(textureLocation.x * heightMap.width),
            (int)(textureLocation.y * heightMap.height)
            ).grayscale * (maxInt - minInt)) + minInt;
    }
}
