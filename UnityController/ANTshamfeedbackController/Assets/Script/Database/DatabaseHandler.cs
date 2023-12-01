using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using System.Linq;
using System;
using Unity.Mathematics;
using Unity.VisualScripting;

public class DatabaseHandler : Singleton<DatabaseHandler>
{
    MySqlLite m_ANTparticipantDatabase;
    string m_DBfileName = "database.sqlite";

    public List<Participant> participantsList = new List<Participant>();

    public void Init()
    {
        LoadDatabase();
    }


    private void LoadDatabase()
    {
        m_ANTparticipantDatabase = new MySqlLite(m_DBfileName);
    }

    public void RecordData(string a_participantID, string a_feedbackType, string a_lenghtType, string a_sessionType,
        string a_timestampWhenStarted, string a_timestampWhenStopped, string a_timeAtStart, string a_nbOfClicked, string a_lenghtRecorded)
    {
        string v_query =
            @"INSERT INTO ParticipantTrainingSession('ID','FeedbackType','LenghtType','SessionType','TimeStampOnStart','TimeStampOnStop',
            'NbOfClick', 'LenghtRecorded') VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
        v_query = String.Format(v_query, a_participantID, a_feedbackType, a_lenghtType, a_sessionType, a_timestampWhenStarted, a_timestampWhenStopped,
            a_nbOfClicked, a_lenghtRecorded);

        m_ANTparticipantDatabase.insertQuery(v_query);
    }
}
