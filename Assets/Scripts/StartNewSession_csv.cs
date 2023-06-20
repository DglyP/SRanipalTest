using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class StartNewSession_csv : MonoBehaviour
{
    public OrganizeData_csv organizeDataCsv;
    public ExperimentValues experimentValues;

    private string logFilePathCsv; // Path to the XML log file

    // Start is called before the first frame update
    void Start()
    {
        experimentValues.logFilePathCsv = "Participant_" + experimentValues.participantID + "_debug_log.csv"; // Initialize logFilePathCsv here
        logFilePathCsv = experimentValues.logFilePathCsv;
        //Debug.Log("Started Scene");

        bool appendData = File.Exists(logFilePathCsv); // Check if the CSV file exists

        if (appendData)
        {
            AppendSession(logFilePathCsv);
        }
        else
        {
            organizeDataCsv.CreateCsvFile();
            // Debug.Log("Debug log written to CSV file: " + logFilePathCsv);
        }
    }

    public void AppendSession(string logFilePathCsv)
    {
        // Load the existing CSV file
        /*
        using (StreamWriter fileWriter = File.AppendText(logFilePathCsv))
        {
            // Write the session information
            fileWriter.WriteLine($"ParticipantNumber,{experimentValues.participantID}");
            fileWriter.WriteLine($"Session,{experimentValues.currentSession}");
            fileWriter.WriteLine($"Time,{System.DateTime.Now}");
        }
        */
        organizeDataCsv.AppendDataToCsv(false);
    }
}