using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class OrganizeData : MonoBehaviour
{
    public ExperimentValues experimentValues;
    public StartNewSession startNewSession;
    private string logFilePath;

    private void Start()
    {
    }

    public void CreateXmlFile()
    {
        experimentValues.currentSession = 1;
        logFilePath = experimentValues.logFilePath;
        XmlWriterSettings writerSettings = new XmlWriterSettings();
        writerSettings.Indent = true;

        using (XmlWriter xmlWriter = XmlWriter.Create(logFilePath, writerSettings))
        {
            xmlWriter.WriteStartElement("ParticipantNumber_" + experimentValues.participantID);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
        }

        startNewSession.AppendSession(logFilePath);
    }

    public void AppendDataToXml(string variableName, string variableValue, bool isPupilData)
    {
        logFilePath = experimentValues.logFilePath;
        // Load the existing XML file
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(logFilePath);

        // Find the session element
        XmlElement sessionElement = xmlDoc.SelectSingleNode("ParticipantNumber_" + experimentValues.participantID + "/Session_" + (experimentValues.currentSession)) as XmlElement;

        if (sessionElement != null)
        {
            // Create the appropriate element based on isPupilData
            XmlElement dataElement;
            if (isPupilData)
            {
                XmlNodeList avatarStimuliNodes = sessionElement.SelectNodes("//" + experimentValues.currentAvatarShown + "_Stimuli");
                XmlNode lastAvatarStimuliNode = avatarStimuliNodes[avatarStimuliNodes.Count - 1];

                dataElement = xmlDoc.CreateElement("PupilData");
                dataElement.SetAttribute(variableName, variableValue);
                if (lastAvatarStimuliNode == null)
                {
                    Debug.Log("That tag doesn't exist");
                    sessionElement.AppendChild(dataElement);
                }
                else
                {
                    lastAvatarStimuliNode.AppendChild(dataElement);
                }

            }
            else
            {
                dataElement = xmlDoc.CreateElement(variableValue + "_" + variableName);
                // Append the data element to the session element
                sessionElement.AppendChild(dataElement);
            }

            // Add timestamp attribute
            XmlAttribute timestampAttribute = xmlDoc.CreateAttribute("Timestamp");
            timestampAttribute.Value = DateTime.Now.ToString();
            dataElement.Attributes.Append(timestampAttribute);

            // Save the modified XML file
            xmlDoc.Save(logFilePath);
        }
        else
        {
            Debug.LogError("No session element found in the XML file.");
        }
    }


    public void TestPupilAppend(string variableName, string variableValue, bool isPupilData)
    {
        logFilePath = experimentValues.logFilePath;
        // Load the existing XML file
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(logFilePath);

        // Find the session element
        XmlElement sessionElement = xmlDoc.SelectSingleNode("ParticipantNumber_" + experimentValues.participantID + "/Session_" + (experimentValues.currentSession)) as XmlElement;

        if (sessionElement != null)
        {
            // Create the appropriate element based on isPupilData
            XmlElement dataElement;
            if (isPupilData)
            {
                XmlNodeList avatarStimuliNodes = sessionElement.SelectNodes("/Session_" + (experimentValues.currentSession));
                XmlNode lastAvatarStimuliNode = avatarStimuliNodes[avatarStimuliNodes.Count - 1];

                dataElement = xmlDoc.CreateElement("TestPupilData");
                dataElement.SetAttribute(variableName, variableValue);
                if (lastAvatarStimuliNode == null)
                {
                    Debug.Log("That tag doesn't exist");
                    sessionElement.AppendChild(dataElement);
                }
                else
                {
                    lastAvatarStimuliNode.AppendChild(dataElement);
                }

            }
            else
            {
                dataElement = xmlDoc.CreateElement(variableValue + "_" + variableName);
                // Append the data element to the session element
                sessionElement.AppendChild(dataElement);
            }


            // Save the modified XML file
            xmlDoc.Save(logFilePath);
        }
        else
        {
            Debug.LogError("No session element found in the XML file.");
        }
    }
}

