using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    public List<Participant> participantsList = new List<Participant>();

    public string currentParticipantID = string.Empty;
    public string currentSessionType = string.Empty;
    public string currentFeedbackType = string.Empty;
    public string currentLenghtType = string.Empty;

    public UIManager sceneGameObjects;
    public OSCBroadcaster OSCBroadcaster;

    public enum Status
    {
        OFF,
        MANDATORY,
        WAITFORUSERTOSTARTAGAIN,
        FREE
    };
    public Status status = Status.OFF;

    public float remainingTimeBeforeEndOfManadatory = 0;

    #region INIT
    public void Start()
    {
        sceneGameObjects = GetComponent<UIManager>();

        DatabaseHandler v_databaseHandeler = gameObject.AddComponent<DatabaseHandler>();
        v_databaseHandeler.Init();

        OSCBroadcaster = GameObject.FindAnyObjectByType<OSCBroadcaster>();
        OSCBroadcaster.StopExperiment();
    }
    #endregion

    #region UPDATE
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("boring button pressed");
            OnBoringButtonClicked();
            
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("Left button pressed");
            OnLeftMouseButtonClicked();
        }

        UpdateMandatoryTiming();
    }
    #endregion

    #region Participant Action
    private void OnLeftMouseButtonClicked()
    {
        if(status == Status.WAITFORUSERTOSTARTAGAIN)
        {
            status = Status.FREE;
            currentLenghtType = "Free";
            OnStartExperiment();
            OSCBroadcaster.StartFreeExperiment();
            sceneGameObjects.OnLaunchFreeTimeSession();
        }

        else if(status == Status.FREE)
        {
            status = Status.OFF;
            sceneGameObjects.OnStartStopExperimentButtonPressed();
        }
    }

    

    public void OnBoringButtonClicked()
    {
        if (status == Status.MANDATORY)
        {
            if (participantsList.Any(x => x.ID == currentParticipantID))
            {
                participantsList.Find(x => x.ID == currentParticipantID).OnUserClickedBoringButtonDuringMandatoryMode();
                sceneGameObjects.OnUserClickedBoringButtonInMandatoryMode(
                    participantsList.Find(x => x.ID == currentParticipantID).GetTotalNumberOfClick());
            }
        }
        else if(status == Status.FREE)
        {
            participantsList.Find(x => x.ID == currentParticipantID).OnUserClickedBoringButtonDuringMandatoryMode();
            sceneGameObjects.OnUserClickedBoringButtonInFreeMode(
                participantsList.Find(x => x.ID == currentParticipantID).GetTotalNumberOfClick());
        }

        OSCBroadcaster.SendButtonClicked();
    }
    #endregion

    #region Student Action
    public void OnStudentWishToStartExperiment(string a_participantID, string a_feedbackType, 
        float a_lenghtSession,
        string a_sessionType)
    {
        currentParticipantID = a_participantID;
        currentFeedbackType = a_feedbackType;
        currentLenghtType = "Fixed";
        currentSessionType = a_sessionType;

        OnStartExperiment();

        OSCBroadcaster.StartExperimentWithConfig(a_feedbackType, a_sessionType);

        remainingTimeBeforeEndOfManadatory = a_lenghtSession;
        status = Status.MANDATORY;
    }

    public void OnStudentWishToStopExperiment(string a_participantID)
    {
        OnExperimentStopped(a_participantID);
        OSCBroadcaster.StopExperiment();
    }
    #endregion

    #region LOGIC
    private void OnStartExperiment()
    {
        if (participantsList.Any(x => x.ID == currentParticipantID))
        {
            participantsList.Find(x => x.ID == currentParticipantID).OnExperimentStarted(
                currentFeedbackType, currentLenghtType, currentSessionType);
        }
        else
        {
            Participant v_newParticipant = new Participant();
            v_newParticipant.ID = currentParticipantID;
            v_newParticipant.OnExperimentStarted(currentFeedbackType, 
                currentLenghtType, currentSessionType);
            participantsList.Add(v_newParticipant);
        }
    }

    private void OnMandatoryExperimentStopped()
    {
        OnExperimentStopped(currentParticipantID);
        sceneGameObjects.OnMandatoryExperimentOver();
        status = Status.WAITFORUSERTOSTARTAGAIN;

        OSCBroadcaster.ShowMessageMandatoryIsOverWaitForYou();
    }

    private void OnExperimentStopped(string a_participantID)
    {
        if (participantsList.Any(x => x.ID == a_participantID))
        {
            currentParticipantID = a_participantID;
            participantsList.Find(x => x.ID == a_participantID).OnExperimentStopped();

            sceneGameObjects.ShowAlertMenuWithMessage(
                "participant ID:\n " + a_participantID + "\n\n session recorded",
                Color.white);
        }
        else
        {
            Debug.LogError("Can't find participant with the ID");
            sceneGameObjects.ShowAlertMenuWithMessage(
                "Can't find participant ID:\n " + a_participantID + "\n\n session not recorded",
                Color.red);
        }
    }
    private void UpdateMandatoryTiming()
    {
        if (status == Status.MANDATORY)
        {
            remainingTimeBeforeEndOfManadatory -= Time.deltaTime;
            if (remainingTimeBeforeEndOfManadatory < 0)
            {
                OnMandatoryExperimentStopped();
            }
        }

    }
    #endregion






}
