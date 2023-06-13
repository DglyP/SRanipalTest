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

        if (experimentValues.currentSession < experimentValues.numberOfSessions)
        {
            Debug.Log("Increasing currentSession");
            experimentValues.currentSession += 1;
        }
        Debug.Log("Started Scene");
        logFilePath ="Participant_" + experimentValues.participantID + "_" + "debug_log.xml"; // Initialize logFilePath here

        bool appendData = File.Exists(logFilePath);

        if (appendData)
        {
            AppendDataToXml("Time", System.DateTime.Now.ToString());
            AppendDataToXml("Restarted ", experimentValues.participantID.ToString());
            AppendDataToXml("Session_" , experimentValues.currentSession.ToString());
            AppendDataToXml("Time", System.DateTime.Now.ToString());

        }
        else
        {
            //Testing currentSession
            experimentValues.currentSession = 1;
            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.Indent = true;

            using (XmlWriter xmlWriter = XmlWriter.Create(logFilePath, writerSettings))
            {
                xmlWriter.WriteStartElement("ParticipantNumber_" + experimentValues.participantID.ToString());

                xmlWriter.WriteStartElement("Session_" + experimentValues.currentSession.ToString());

                xmlWriter.WriteElementString("Time", System.DateTime.Now.ToString());

                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndElement();

                xmlWriter.Flush();
                xmlWriter.Close();
            }

            Debug.Log("Debug log written to XML file: " + logFilePath);
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
            Debug.LogError("No existing LogEntry found in the XML file.");
        }
    }
}
