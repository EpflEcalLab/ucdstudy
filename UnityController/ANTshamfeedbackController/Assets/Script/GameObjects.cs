using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameObjects : MonoBehaviour
{
    //Participant ID
    public TextMeshProUGUI participantID;
    public GameObject InputFieldForParticipantID;

    //Feedback Type
    public ToggleGroup FeedbackToogleGroup;

    //Session Type
    public ToggleGroup SessionTypeToogleGroup;

    //lenght
    public ToggleGroup LenghtToogleGroup;

    public TextMeshProUGUI SummaryText;
    public TextMeshProUGUI EllapsedTimeInExperienceText;

    public Button StartStopButton;

    public List<Toggle> Toggles = new List<Toggle>();

    public bool experimentIsRunning = false;

    private float timeWhenButtonToStartWasPressed = 0;

    public GameObject alertMenu;
    public Button closeAlertMenuButton;
    public TextMeshProUGUI alertMessage;
    public Image bgAlertMenu;

    public void Start()
    {
        CleanAll();

        Toggles = GameObject.FindObjectsOfType<Toggle>().ToList();

        StartStopButton.onClick.AddListener(OnStartExperimentButtonPressed);
        closeAlertMenuButton.onClick.AddListener(OnCloseAlertMenuButtonPressed);

        alertMenu.SetActive(false);

    }


    public void CleanAll()
    {
        participantID.text = string.Empty;
        EllapsedTimeInExperienceText.text = string.Empty;
    }

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

    private void OnStartExperimentButtonPressed()
    {
        //check if there is an ID
        if(string.IsNullOrWhiteSpace(participantID.text))
        {
            alertMenu.SetActive(true);
            return;
        }

        if(experimentIsRunning == true)
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

            InformGameManagerExperimentStarted();
        }
    }

    private void InformGameManagerExperimentStarted()
    {
        GameManager.Instance.OnExperimentStarted(participantID.text, 
            FeedbackToogleGroup.ActiveToggles().FirstOrDefault().name, 
            LenghtToogleGroup.ActiveToggles().FirstOrDefault().name, 
            SessionTypeToogleGroup.ActiveToggles().FirstOrDefault().name);
    }

    private void InformGameManagerExperimentEnded()
    {
        GameManager.Instance.OnExperimentStopped(participantID.text);
    }

    private void OnCloseAlertMenuButtonPressed()
    {
        alertMenu.SetActive(false);
    }


    public void ShowAlertMenuWithMessage(string a_alertMessage, Color a_BGcolor)
    {
        alertMenu.SetActive(true);
        alertMessage.text = a_alertMessage;

        bgAlertMenu.color = a_BGcolor;
    }
}
