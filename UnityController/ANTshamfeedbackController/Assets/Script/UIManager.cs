using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class UIManager : MonoBehaviour
{
    //Participant ID
    public TextMeshProUGUI participantID;
    public GameObject InputFieldForParticipantID;

    //Feedback Type
    public ToggleGroup FeedbackToogleGroup;

    //Session Type
    public ToggleGroup SessionTypeToogleGroup;

    //Input lenght Session
    public TextMeshProUGUI inputlengthSession;

    //Summary Text
    public TextMeshProUGUI SummaryText;
    public TextMeshProUGUI EllapsedTimeInExperienceText;
    public TextMeshProUGUI NbOfClickMandatory;
    public TextMeshProUGUI NbOfClickFree;

    //Start stop Button
    public Button StartStopButton;

    //Logic
    public List<Toggle> Toggles = new List<Toggle>();
    public bool experimentIsRunning = false;
    private float timeWhenButtonToStartWasPressed = 0;
    private float lenghtOfFixedSessionsInSeconds = 5;

    //Alert Menu
    public GameObject alertMenu;
    public Button closeAlertMenuButton;
    public TextMeshProUGUI alertMessage;
    public Image bgAlertMenu;

    #region INIT
    public void Start()
    {
        CleanAll();

        Toggles = GameObject.FindObjectsOfType<Toggle>().ToList();

        StartStopButton.onClick.AddListener(OnStartStopExperimentButtonPressed);
        closeAlertMenuButton.onClick.AddListener(OnCloseAlertMenuButtonPressed);

        alertMenu.SetActive(false);

    }

    public void CleanAll()
    {
        participantID.text = string.Empty;
        EllapsedTimeInExperienceText.text = string.Empty;
    }
    #endregion

    #region UPDATE
    public void Update()
    {
        UpdateExperimentString();
        UpdateEllapsedTime();
    }

    private void UpdateExperimentString()
    {
        string v_summaryExperimentText = string.Empty;
        v_summaryExperimentText += participantID.text;
        v_summaryExperimentText += "\n\n";

        List<string> listToogleOn = new List<string>();
        foreach (Toggle toggle in Toggles)
        {
            if (toggle.isOn)
            {
                listToogleOn.Add(toggle.name);
            }
        }
        listToogleOn.Reverse();
        v_summaryExperimentText += string.Join("\n\n", listToogleOn);

        v_summaryExperimentText += "\n\n Lenght Planned: " + inputlengthSession.text;
        SummaryText.text = v_summaryExperimentText;
    }

    private void UpdateEllapsedTime()
    {
        if (experimentIsRunning)
        {
            float currentTime = Time.realtimeSinceStartup;
            float ellaspedTime = currentTime - timeWhenButtonToStartWasPressed;

            int minutes = Mathf.FloorToInt(ellaspedTime / 60F);
            int seconds = Mathf.FloorToInt(ellaspedTime - minutes * 60);

            string niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);

            EllapsedTimeInExperienceText.text = niceTime;
        }
        else
        {
            EllapsedTimeInExperienceText.text = string.Empty;
        }
    }
    #endregion

    #region STUDENT ACTION
    public void OnStartStopExperimentButtonPressed()
    {
        //check if there is an ID
        if (string.IsNullOrWhiteSpace(participantID.text))
        {
            alertMenu.SetActive(true);
            return;
        }

        if (experimentIsRunning == true)
        {
            experimentIsRunning = false;
            StartStopButton.gameObject.GetComponent<Image>().color = Color.green;
            StartStopButton.transform.Find("text").GetComponent<TextMeshProUGUI>().text = "START";
            InputFieldForParticipantID.SetActive(true);

            InformGameManagerExperimentEnded();
        }
        else
        {
            timeWhenButtonToStartWasPressed = Time.realtimeSinceStartup;
            experimentIsRunning = true;
            StartStopButton.gameObject.GetComponent<Image>().color = Color.red;
            StartStopButton.transform.Find("text").GetComponent<TextMeshProUGUI>().text = "STOP";
            InputFieldForParticipantID.SetActive(false);

            NbOfClickFree.text = "0";
            NbOfClickMandatory.text = "0";

            /*string v_stringValue = inputlengthSession.text.ToString();
            int.TryParse(v_stringValue, out int lenghtOfFixedSessionsInSeconds);

            bool success = int.TryParse(v_stringValue, out lenghtOfFixedSessionsInSeconds);

            if (success)
            {
                Debug.Log("Converted number: " + lenghtOfFixedSessionsInSeconds);
                // Perform your logic with the converted number
            }
            else
            {
                Debug.Log("Invalid input. Cannot convert to number.");
            }*/

            InformGameManagerExperimentStarted();
        }
    }

    private void OnCloseAlertMenuButtonPressed()
    {
        alertMenu.SetActive(false);
    }
    #endregion

    #region LOGIC
    private void InformGameManagerExperimentStarted()
    {
        GameManager.Instance.OnStudentWishToStartExperiment(participantID.text,
            FeedbackToogleGroup.ActiveToggles().FirstOrDefault().name,
            lenghtOfFixedSessionsInSeconds,
            SessionTypeToogleGroup.ActiveToggles().FirstOrDefault().name);
    }

    private void InformGameManagerExperimentEnded()
    {
        GameManager.Instance.OnStudentWishToStopExperiment(participantID.text);
    }

    public void ShowAlertMenuWithMessage(string a_alertMessage, Color a_BGcolor)
    {
        alertMenu.SetActive(true);
        alertMessage.text = a_alertMessage;

        bgAlertMenu.color = a_BGcolor;
    }
    #endregion

    #region PARTICIPANT ACTION
    public void OnMandatoryExperimentOver()
    {
        ShowAlertMenuWithMessage("Mandatory Over, wait for user", Color.yellow);
    }

    public void OnLaunchFreeTimeSession()
    {
        OnCloseAlertMenuButtonPressed();
        timeWhenButtonToStartWasPressed = Time.realtimeSinceStartup;
    }

    public void OnUserClickedBoringButtonInMandatoryMode(string a_totalNbOfClick)
    {
        NbOfClickMandatory.text = "Total Clicked Mandatory: " + a_totalNbOfClick;
    }

    public void OnUserClickedBoringButtonInFreeMode(string a_totalNbOfClick)
    {
        NbOfClickFree.text = "Total Clicked Free: " + a_totalNbOfClick;
    }
    #endregion
}
