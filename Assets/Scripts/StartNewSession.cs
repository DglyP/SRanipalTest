using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class StartNewSession : MonoBehaviour
{
    public OrganizeData organizeData;
    public ExperimentValues experimentValues;

    private string logFilePath; // Path to the XML log file

    // Start is called before the first frame update
    void Start()
    {
        experimentValues.logFilePath = "Participant_" + experimentValues.participantID + "_debug_log.xml"; // Initialize logFilePath here
        logFilePath = experimentValues.logFilePath;
        //Debug.Log("Started Scene");


        bool appendData = File.Exists(logFilePath);


        if (appendData)
        {
            experimentValues.currentSession += 1;
            AppendSession(logFilePath);
        }
        else
        {
            organizeData.CreateXmlFile();
           // Debug.Log("Debug log written to XML file: " + logFilePath);
        }
    }

    public void AppendSession(string logFilePath)
    {
        // Load the existing XML file
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(logFilePath);

        // Find the <ParticipantNumber> element
        XmlElement participantElement = xmlDoc.SelectSingleNode("//ParticipantNumber_" + experimentValues.participantID) as XmlElement;

        if (participantElement != null)
        {
            // Create the new session element
            XmlElement sessionElement = xmlDoc.CreateElement("Session_" + (experimentValues.currentSession));

            // Create the <Time> element and set its value
            XmlElement timeElement = xmlDoc.CreateElement("Time");
            timeElement.InnerText = System.DateTime.Now.ToString();

            // Append the <Time> element to the <Session> element
            sessionElement.AppendChild(timeElement);

            // Append the new session element to the <ParticipantNumber> element
            participantElement.AppendChild(sessionElement);

            // Save the modified XML file
            xmlDoc.Save(logFilePath);

           // Debug.Log("New session added to XML file: " + logFilePath);
        }
        else
        {
            Debug.LogError("No ParticipantNumber tag found in the XML file.");
        }
    }
}
