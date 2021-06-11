using Leap;
using Leap.Unity;
using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// This script will capture Leap Motion Controller information related to hand locations per frame for both left and right hands.
/// </summary>

public class HandData : MonoBehaviour
{
    private List<Hand> _hands = new List<Hand>();
    public Hand currentHand {
    get{
        if(_hands.Count > 0)
        {
            return _hands[0];
        }
        return null;
        }
    }

    [SerializeField]
    private LeapProvider _leapProvider;

    private void OnEnable()
    {
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
        if (_leapProvider != null)
        {
            _leapProvider.OnUpdateFrame -= LeapProviderOnUpdateFrame;
        }
    }
    
    private void LeapProviderOnUpdateFrame(Frame frame)
    {
        if (frame.Hands.Count <= 0 && _hands.Count != 0) 
        {
            _hands.Clear();
            return;
        }

        if (frame.Hands.Count <= 0){
            return;
        }   

        for (int i = 0; i < frame.Hands.Count; i++)
        {
            if (!_hands.Contains(frame.Hands[i]))
            {
                _hands.Add(frame.Hands[i]);
            }
        }
    }

    public Hand GetHand(Chirality hand)
    {
        for (int i = 0; i < _hands.Count; i++)
        {
            if(Convert.ToInt32(_hands[i].IsRight) == (int)hand)
                return _hands[i];
        }
        return null;
    }
}
