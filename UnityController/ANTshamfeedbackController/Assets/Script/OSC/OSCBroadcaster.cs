using OSCsharp.Data;
using System.Collections;
using System.Collections.Generic;
using UniOSC;
using UnityEngine;

public class OSCBroadcaster : UniOSCEventDispatcher
{
    #region INIT
    public override void Awake()
    {
        base.Awake();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        ClearData();
    }
    public override void OnDisable()
    {
        base.OnDisable();
    }
    #endregion

    #region LOGIC
    public void StartExperimentWithConfig(string a_feedbackType, string a_sessionType)
    {
        ClearData();

        AppendData(OSCLookUpTable.GetIntValueAccordingToInputStringFeedbackType(a_feedbackType));
        AppendData(OSCLookUpTable.GetIntValueAccordingToInputStringSessionType(a_sessionType));

        OscMessage msg = ((OscMessage)_OSCeArg.Packet);
        msg.Address = OSCAddress.StartExperiment;
        _SendOSCMessage(_OSCeArg);
    }

    public void StopExperiment()
    {
        ClearData();

        AppendData(4);

        OscMessage msg = ((OscMessage)_OSCeArg.Packet);
        msg.Address = OSCAddress.StopExperiment;
        _SendOSCMessage(_OSCeArg);
    }

    public void SendButtonClicked()
    {
        ClearData();

        AppendData(4);

        OscMessage msg = ((OscMessage)_OSCeArg.Packet);
        msg.Address = OSCAddress.ButtonClicked;
        _SendOSCMessage(_OSCeArg);
    }
    #endregion


}
