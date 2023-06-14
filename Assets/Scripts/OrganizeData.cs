using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class OrganizeData : MonoBehaviour
{
    public ExperimentValues experimentValues;

    private string logFilePath; // Path to the XML log file

    private void Start()
    {
        Debug.Log("Started Scene");

        logFilePath = "Participant_" + experimentValues.participantID + "_debug_log.xml"; // Initialize logFilePath here

        bool appendData = File.Exists(logFilePath);

        if (appendData)
        {
            StartNewSession();
        }
        else
        {
            experimentValues.currentSession = 1;
            CreateXmlFile();
            Debug.Log("Debug log written to XML file: " + logFilePath);
        }
    }

    private void CreateXmlFile()
    {
        XmlWriterSettings writerSettings = new XmlWriterSettings();
        writerSettings.Indent = true;

        using (XmlWriter xmlWriter = XmlWriter.Create(logFilePath, writerSettings))
        {
            xmlWriter.WriteStartElement("ParticipantNumber_" + experimentValues.participantID);

            xmlWriter.WriteStartElement("Session_" + experimentValues.currentSession);
            xmlWriter.WriteElementString("Time", System.DateTime.Now.ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();

            xmlWriter.Flush();
        }
    }

    public void StartNewSession()
    {
        // Load the existing XML file
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(logFilePath);

        // Find the <ParticipantNumber> element
        XmlElement participantElement = xmlDoc.SelectSingleNode("//ParticipantNumber_" + experimentValues.participantID) as XmlElement;

        if (participantElement != null)
        {
            // Create the new session element
            XmlElement sessionElement = xmlDoc.CreateElement("Session_" + (experimentValues.currentSession + 1));

            // Create the <Time> element and set its value
            XmlElement timeElement = xmlDoc.CreateElement("Time");
            timeElement.InnerText = System.DateTime.Now.ToString();

            // Append the <Time> element to the <Session> element
            sessionElement.AppendChild(timeElement);

            // Append the new session element to the <ParticipantNumber> element
            participantElement.AppendChild(sessionElement);

            // Save the modified XML file
            xmlDoc.Save(logFilePath);

            Debug.Log("New session added to XML file: " + logFilePath);
        }
        else
        {
            Debug.LogError("No ParticipantNumber tag found in the XML file.");
        }
    }

    public void AppendDataToXml(string variableName, string variableValue)
    {
        // Load the existing XML file
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(logFilePath);

        // Find the last <LogEntry> element
        XmlElement logEntryElement = xmlDoc.SelectSingleNode("//ParticipantNumber_" + experimentValues.participantID + "[last()]") as XmlElement;
        if (logEntryElement != null)
        {
            // Create the <PupilData> element
            XmlElement pupilDataElement = xmlDoc.CreateElement("PupilData");
            pupilDataElement.SetAttribute("VariableName", variableName);
            pupilDataElement.SetAttribute("VariableValue", variableValue);

            // Append the <PupilData> element to the <LogEntry> element
            logEntryElement.AppendChild(pupilDataElement);

            // Save the modified XML file
            xmlDoc.Save(logFilePath);

            Debug.Log("Data appended to XML file: " + logFilePath);
        }
        else
        {
            Debug.LogError("No existing tag found in the XML file.");
        }
    }
}
