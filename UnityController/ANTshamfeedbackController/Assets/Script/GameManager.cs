using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    public List<Participant> participantsList = new List<Participant>();
    public string currentParticipantID = string.Empty;

    public GameObjects gameObjects;
    public OSCBroadcaster OSCBroadcaster;

    public void Start()
    {
        gameObjects = GetComponent<GameObjects>();

        DatabaseHandler v_databaseHandeler = gameObject.AddComponent<DatabaseHandler>();
        v_databaseHandeler.Init();

        OSCBroadcaster = GameObject.FindAnyObjectByType<OSCBroadcaster>();
    }

    public void OnExperimentStarted(string a_participantID, string a_feedbackType, string a_lenghtType,
        string a_sessionType)
    {
        if(participantsList.Any(x => x.ID == a_participantID))
        {
            currentParticipantID = a_participantID;
            participantsList.Find(x => x.ID == a_participantID).OnExperimentStarted(a_feedbackType, a_lenghtType, a_sessionType);
        }
        else
        {
            Participant v_newParticipant = new Participant();
            currentParticipantID = a_participantID;
            v_newParticipant.ID = a_participantID;
            v_newParticipant.OnExperimentStarted(a_feedbackType, a_lenghtType, a_sessionType);
            participantsList.Add(v_newParticipant);
        }

        OSCBroadcaster.StartExperimentWithConfig(a_feedbackType, a_sessionType);
    }

    public void OnExperimentStopped(string a_participantID)
    {
        if (participantsList.Any(x => x.ID == a_participantID))
        {
            currentParticipantID = a_participantID;
            participantsList.Find(x => x.ID == a_participantID).OnExperimentStopped();

            gameObjects.ShowAlertMenuWithMessage(
                "participant ID:\n " + a_participantID + "\n\n session recorded",
                Color.white);
        }
        else
        {
            Debug.LogError("Can't find participant with the ID");
            gameObjects.ShowAlertMenuWithMessage(
                "Can't find participant ID:\n " + a_participantID + "\n\n session not recorded",
                Color.red);
        }

        OSCBroadcaster.StopExperiment();
    }

    public void OnUserClicked()
    {
        if (participantsList.Any(x => x.ID == currentParticipantID))
        {
            participantsList.Find(x => x.ID == currentParticipantID).OnUserClicked();
        }

        OSCBroadcaster.SendButtonClicked();
    }
}
