using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ExperimentValues")]
public class ExperimentValues : ScriptableObject
{
    public int participantID;
    public int currentSession;
    public string currentAvatarShown;
    public int numberOfSessions;
    public int stimuliAmount;
    public bool stageReady;
    public string logFilePath;
    public string logFilePathCsv;
    public float WaitForPupilToAdjust;
    public float DataGatheringTime;
    public float WaitForStartPupilChange;

    //Values for printing data

    public float minPupilSize;
    public float maxPupilSize;
    public float StimuliStartSize;
    public float StimuliEndSize;
    public float StimuliPupilSize;
    public bool PupilSizeChanging;
    public bool PupilDataActive;
    public float UserLeftPupilSize;
    public float UserRightPupilSize;

    //Participant	Time	CurrentSession	ConstrictedSize	DilatedSize	CurrentStimuli	StimuliStartSize	StimuliEndSize	StimuliPupilSize	PupilSizeChanging	PupilDataActive	LeftPupilSize	RightPupilSize																				
}