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
            fileWriter.WriteLine("Participant,UnityTime,CurrentSession,ConstrictedSize,DilatedSize,CurrentStimuli,StimuliStartSize,StimuliEndSize,StimuliPupilSize,PupilSizeChanging,PupilDataActive,LeftPupilSize,RightPupilSize,Frame"); // Modify the header as needed
        }
        startNewSession.AppendSession(logFilePathCsv);
    }

    public void AppendDataToCsv(bool print)
    {
        logFilePathCsv = experimentValues.logFilePathCsv; // Add .csv extension to the file path

        // Append the variable data based on isPupilData
        if (!print)
        {
            //experimentValues.PupilDataActive = true;
            string unityTime = System.DateTime.Now.ToString("HH:mm:ss:fff");
            experimentValues.unityTime = unityTime;
            int frame = Time.frameCount;
            }
        else
        {
            // Create the data string to be appended to the CSV
            string unityTime = System.DateTime.Now.ToString("HH:mm:ss:fff");
            experimentValues.unityTime = unityTime;
            int frame = Time.frameCount;
            string data = $"{experimentValues.participantID},{unityTime},{experimentValues.currentSession},{experimentValues.minPupilSize},{experimentValues.maxPupilSize},{experimentValues.currentAvatarShown},{experimentValues.StimuliStartSize},{experimentValues.StimuliEndSize},{experimentValues.StimuliPupilSize},{experimentValues.PupilSizeChanging},{experimentValues.PupilDataActive},{experimentValues.UserLeftPupilSize},{experimentValues.UserRightPupilSize},{frame.ToString()}";

            // Append the data to the CSV file
            using (StreamWriter fileWriter = File.AppendText(logFilePathCsv))
            {
                fileWriter.WriteLine(data);
            }
        }

    }

    public void TestPupilAppendCsv(bool isPupilData)
    {
        if (isPupilData)
            {
            logFilePathCsv = experimentValues.logFilePathCsv; // Add .csv extension to the file path

            // Create the data string to be appended to the CSV
            string currentTime = System.DateTime.Now.ToString("HH:mm:ss:fff");
            string data = $"{experimentValues.participantID},{currentTime},{experimentValues.currentSession}";

            // Append the data to the CSV file
            using (StreamWriter fileWriter = File.AppendText(logFilePathCsv))
                {
                fileWriter.WriteLine(data);
                }
            }
    }
}
