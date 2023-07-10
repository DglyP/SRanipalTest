//========= Copyright 2018, HTC Corporation. All rights reserved. ===========
using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;
using static System.Net.Mime.MediaTypeNames;

namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {
            public class FullHzGather : MonoBehaviour
            {
                private static EyeData_v2 eyeData = new EyeData_v2();
                private bool eye_callback_registered = false;
                public OrganizeData_csv organizeData_csv;
                public ExperimentValues experimentValues;

                public static int timeStamp;
                public static Vector3 gazeOriginLeft, gazeOriginRight;
                public static Vector3 gazeDirectionRight, gazeDirectionLeft, gazeDirectionCombined;
                public static float pupilDiameterLeft, pupilDiameterRight, pupilDiameterCombined;
                public static float eyeOpenLeft, eyeOpenRight, eyeOpenCombined;
                public static Vector2 pupilPositionLeft, pupilPositionRight, pupilPositionCombined;


                private static long referenceTimestamp = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
                public static int participantID;
                public static int currentSession;
                public static string unityTime;
                public static string currentAvatarShown;
                public static int numberOfSessions;
                public static int stimuliAmount;
                public static bool stageReady;
                public static string logFilePath;
                public static string logFilePathCsv;
                public static float WaitForPupilToAdjust;
                public static float DataGatheringTime;
                public static float WaitForStartPupilChange;

                //Values for printing data

                public static float minPupilSize;
                public static float maxPupilSize;
                public static float StimuliStartSize;
                public static float StimuliEndSize;
                public static float StimuliPupilSize;
                public static bool PupilSizeChanging;
                public static bool PupilDataActive;
                public static float UserLeftPupilSize;
                public static float UserRightPupilSize;
                // Create the data string to be appended to the CSV
                public static string currentTime = System.DateTime.Now.ToString("HH:mm:ss:fff");

                public static int frame = 0;

                private static StreamWriter streamwriter;
                private string filepath;
                private string dataLabels = "Time" + "," + "frame" + "," + "timestamp" + "," +
                                            "gazeOrigin_L.X" + "," + "gazeOrigin_L.Y" + "," + "gazeOrigin_L.Z" + "," +
                                            "gazeOrigin_R.X" + "," + "gazeOrigin_R.Y" + "," + "gazeOrigin_R.Z" + "," +
                                            "gazeDir_L.X" + "," + "gazeDir_L.Y" + "," + "gazeDir_L.Z" + "," +
                                            "gazeDir_R.X" + "," + "gazeDir_R.Y" + "," + "gazeDir_R.Z" + "," +
                                            "pupilDia_L" + "," + "pupilDia_R" + "," +
                                            "eyeOpenness_L" + "," + "eyeOpenness_R" + "," +
                                            "pupilPosL.X" + "," + "pupilPosL.Y" + "," +
                                            "pupilPosR.X" + "," + "pupilPosR.Y" + "," +
                                            "Participant" + "," + "UnityTime" + "," + "CurrentSession" + "," +
                                            "ConstrictedSize" + "," + "DilatedSize" + "," +
                                            "CurrentStimuli" + "," + "StimuliStartSize" + "," +
                                            "StimuliEndSize" + "," + "StimuliPupilSize" + "," +
                                            "PupilSizeChanging" + "," + "PupilDataActive" + "," +
                                            "LeftPupilSize" + "," + "RightPupilSize";

                //Participant Time    CurrentSession ConstrictedSize DilatedSize CurrentStimuli  StimuliStartSize StimuliEndSize  StimuliPupilSize PupilSizeChanging   PupilDataActive LeftPupilSize   RightPupilSize

                private void Start()
                {
                    if (!SRanipal_Eye_Framework.Instance.EnableEye)
                        {
                        enabled = false;
                        return;
                        }
                    filepath = "Full_Participant_" + experimentValues.participantID + "_SaveData.csv";
                    streamwriter = new StreamWriter(filepath, true) { AutoFlush = true };
                    streamwriter.WriteLine(dataLabels);
                }

                private void Update()
                {
                    if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING &&
                        SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT) return;

                    if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eye_callback_registered == false)
                        {
                        SRanipal_Eye_v2.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                        eye_callback_registered = true;
                        }
                    else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eye_callback_registered == true)
                        {
                        SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                        eye_callback_registered = false;
                        }

                    currentTime = System.DateTime.Now.ToString("HH:mm:ss:fff");
                    frame = Time.frameCount;

                    organizeData_csv.AppendDataToCsv(false);
                    // Create the data string to be appended to the CSV
                    participantID = experimentValues.participantID;
                    unityTime = experimentValues.unityTime;
                    currentSession = experimentValues.currentSession;
                    currentAvatarShown = experimentValues.currentAvatarShown;
                    numberOfSessions = experimentValues.numberOfSessions;
                    minPupilSize = experimentValues.minPupilSize;
                    maxPupilSize = experimentValues.maxPupilSize;
                    StimuliStartSize = experimentValues.StimuliStartSize;
                    StimuliEndSize = experimentValues.StimuliEndSize;
                    StimuliPupilSize = experimentValues.StimuliPupilSize;
                    PupilSizeChanging = experimentValues.PupilSizeChanging;
                    PupilDataActive = experimentValues.PupilDataActive;
                    UserLeftPupilSize = experimentValues.UserLeftPupilSize;
                    UserRightPupilSize = experimentValues.UserRightPupilSize;

                }
                private void Release()
                {
                    if (eye_callback_registered == true)
                        {
                        SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                        streamwriter.Close();
                        eye_callback_registered = false;
                        }
                }

                private static void EyeCallback(ref EyeData_v2 eye_data)
                {
                    //Debug.Log("callback hogehoge");
                    eyeData = eye_data;

                    // The time when the frame was capturing. in millisecond.
                    timeStamp = eyeData.timestamp;

                    // The point in the eye from which the gaze ray originates in meter miles.(right-handed coordinate system)
                    gazeOriginLeft = eyeData.verbose_data.left.gaze_origin_mm;
                    gazeOriginRight = eyeData.verbose_data.right.gaze_origin_mm;
                    //Debug.Log("gazeOriginLeft: " + gazeOriginLeft);

                    // The normalized gaze direction of the eye in [0,1].(right-handed coordinate system)
                    gazeDirectionLeft = eyeData.verbose_data.left.gaze_direction_normalized;
                    gazeDirectionRight = eyeData.verbose_data.right.gaze_direction_normalized;
                    gazeDirectionCombined = eyeData.verbose_data.combined.eye_data.gaze_direction_normalized;
                    //Debug.Log("gaze_direction_left: " + gazeDirectionLeft);

                    // The diameter of the pupil in milli meter
                    pupilDiameterLeft = eyeData.verbose_data.left.pupil_diameter_mm;
                    pupilDiameterRight = eyeData.verbose_data.right.pupil_diameter_mm;
                    pupilDiameterCombined = eyeData.verbose_data.combined.eye_data.pupil_diameter_mm;
                    //Debug.Log("pupilDiameterLeft: " + pupilDiameterLeft);

                    // A value representing how open the eye is in [0,1]
                    eyeOpenLeft = eyeData.verbose_data.left.eye_openness;
                    eyeOpenRight = eyeData.verbose_data.right.eye_openness;
                    eyeOpenCombined = eyeData.verbose_data.combined.eye_data.eye_openness;
                    //Debug.Log("eyeOpenLeft: " + eyeOpenLeft);

                    // The normalized position of a pupil in [0,1]
                    pupilPositionLeft = eyeData.verbose_data.left.pupil_position_in_sensor_area;
                    pupilPositionRight = eyeData.verbose_data.right.pupil_position_in_sensor_area;
                    pupilPositionCombined = eyeData.verbose_data.combined.eye_data.pupil_position_in_sensor_area;
                    //Debug.Log("pupilPositionLeft: " + pupilPositionLeft);

                    //Debug.Log("writing to csv");

                    streamwriter.WriteLine(
    currentTime + "," + frame + "," + timeStamp + "," +
    gazeOriginLeft.x + "," + gazeOriginLeft.y + "," + gazeOriginLeft.z + "," +
    gazeOriginRight.x + "," + gazeOriginRight.y + "," + gazeOriginRight.z + "," +
    gazeDirectionLeft.x + "," + gazeDirectionLeft.y + "," + gazeDirectionLeft.z + "," +
    gazeDirectionRight.x + "," + gazeDirectionRight.y + "," + gazeDirectionRight.z + "," +
    pupilDiameterLeft + "," + pupilDiameterRight + "," +
    eyeOpenLeft + "," + eyeOpenRight + "," +
    pupilPositionLeft.x + "," + pupilPositionLeft.y + "," +
    pupilPositionRight.x + "," + pupilPositionRight.y + "," +
    participantID + "," + unityTime + "," + currentSession + "," +
    minPupilSize + "," + maxPupilSize + "," + currentAvatarShown + "," +
    StimuliStartSize + "," + StimuliEndSize + "," + StimuliPupilSize + "," +
    PupilSizeChanging + "," + PupilDataActive + "," + UserLeftPupilSize + "," + UserRightPupilSize);

                    //Participant Time    CurrentSession ConstrictedSize DilatedSize CurrentStimuli  StimuliStartSize StimuliEndSize  StimuliPupilSize PupilSizeChanging   PupilDataActive LeftPupilSize   RightPupilSize
                }
            }
        }
    }
}
