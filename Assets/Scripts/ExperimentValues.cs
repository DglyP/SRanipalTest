using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ExperimentValues")]
public class ExperimentValues : ScriptableObject
{
    public int participantID;
    public int currentSession;
    public int numberOfSessions;
    public int stimuliAmount;
    public bool stageReady;
}
