using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class OrganizeData_csv : MonoBehaviour
{
    public ExperimentValues experimentValues;
    public StartNewSession_csv startNewSession;
    private string logFilePathCsv;

    private void Start()
    {
    }

    public void CreateCsvFile()
    {
        experimentValues.currentSession = 1;
        logFilePathCsv = experimentValues.logFilePathCsv; // Add .csv extension to the file path

        // Create the CSV file and write the header
        using (StreamWriter fileWriter = new StreamWriter(logFilePathCsv))
        {
            fileWriter.WriteLine("Participant,Time,CurrentSession,ConstrictedSize,DilatedSize,CurrentStimuli,StimuliStartSize,StimuliEndSize,StimuliPupilSize,PupilSizeChanging, PupilDataActive, LeftPupilSize,RightPupilSize"); // Modify the header as needed

        }
        startNewSession.AppendSession(logFilePathCsv);
    }

    public void AppendDataToCsv(bool ignore)
    {
        logFilePathCsv = experimentValues.logFilePathCsv; // Add .csv extension to the file path

        // Append the variable data based on isPupilData
        if (ignore)
        {
            //experimentValues.PupilDataActive = true;
        }
        else
        {
            // Create the data string to be appended to the CSV
            string data = $"{experimentValues.participantID},{System.DateTime.Now},{experimentValues.currentSession},{experimentValues.minPupilSize},{experimentValues.maxPupilSize},{experimentValues.currentAvatarShown},{experimentValues.StimuliStartSize},{experimentValues.StimuliEndSize},{experimentValues.StimuliPupilSize},{experimentValues.PupilSizeChanging},{experimentValues.PupilDataActive},{experimentValues.UserLeftPupilSize},{experimentValues.UserRightPupilSize}";

            // Append the data to the CSV file
            using (StreamWriter fileWriter = File.AppendText(logFilePathCsv))
            {
                fileWriter.WriteLine(data);
            }
        }

    }

    public void TestPupilAppendCsv(bool isPupilData)
    {
        logFilePathCsv = experimentValues.logFilePathCsv; // Add .csv extension to the file path

        // Create the data string to be appended to the CSV
        string data = $"{experimentValues.participantID},{experimentValues.currentSession},{System.DateTime.Now}";

        // Append the data to the CSV file
        using (StreamWriter fileWriter = File.AppendText(logFilePathCsv))
        {
            fileWriter.WriteLine(data);
        }
    }
}