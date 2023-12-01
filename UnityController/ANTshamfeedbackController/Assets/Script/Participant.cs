using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Participant
{
    public string ID;
    public List<Training> trainingList = new List<Training>();

    public void OnExperimentStarted(string a_feedbackType, string a_lenghtType,
        string a_sessionType)
    {
        Training v_training = new Training();
        v_training.feedbackType = a_feedbackType;
        v_training.lenghtType = a_lenghtType;
        v_training.sessionType = a_sessionType;

        v_training.timeAtStart = Time.time;
        v_training.timestampWhenStarted = System.DateTime.Now;

        trainingList.Add(v_training);
    }

    public void OnExperimentStopped()
    {
        Training v_currentTraining = trainingList.Last();
        if (v_currentTraining != null)
        {
            v_currentTraining.timestampWhenStopped = System.DateTime.Now;
            v_currentTraining.lenghtRecorded = Time.time - v_currentTraining.timeAtStart;

            //Save into database
            DatabaseHandler.Instance.RecordData(ID, v_currentTraining.feedbackType, v_currentTraining.lenghtType,
                v_currentTraining.sessionType, v_currentTraining.timestampWhenStarted.ToString(), 
                v_currentTraining.timestampWhenStopped.ToString(), v_currentTraining.timeAtStart.ToString(),
                v_currentTraining.nbClickedAtTheEnd.ToString(), v_currentTraining.lenghtRecorded.ToString());
        }


    }

    public void OnUserClicked()
    {
        Training v_currentTraining = trainingList.Last();
        v_currentTraining.nbClickedAtTheEnd++;
    }
}

public class Training
{
    public string feedbackType;
    public string lenghtType;
    public string sessionType;

    public System.DateTime timestampWhenStarted;
    public System.DateTime timestampWhenStopped;
    public float timeAtStart;

    public int nbClickedAtTheEnd = 0;
    public float lenghtRecorded = 0;
}