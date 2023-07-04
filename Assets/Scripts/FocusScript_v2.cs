//========= Copyright 2018, HTC Corporation. All rights reserved. ===========
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ViveSR.anipal.Eye
{
    public class FocusScript_v2 : MonoBehaviour
    {
        private FocusInfo FocusInfo;
        public ExperimentValues experimentValues;
        private readonly float MaxDistance = 20;
        private readonly GazeIndex[] GazePriority = new GazeIndex[] { GazeIndex.COMBINE, GazeIndex.LEFT, GazeIndex.RIGHT };
        private static EyeData_v2 eyeData = new EyeData_v2();
        private bool eye_callback_registered = false;
        private bool isLookingAtDartboard = false;
        private float lookTimer = 0f;
        private const float requiredLookTime = 2f;
        private void Start()
        {
            if (!SRanipal_Eye_Framework.Instance.EnableEye)
            {
                enabled = false;
                return;
            }
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

            bool foundDartboard = false;
            bool isLookingAtDartboardPreviousFrame = isLookingAtDartboard; // Store the previous state of looking at the dartboard
            isLookingAtDartboard = false; // Assume not looking at the dartboard until proven otherwise

            foreach (GazeIndex index in GazePriority)
            {
                Ray GazeRay;
                int dart_board_layer_id = LayerMask.NameToLayer("NoReflection");
                bool eye_focus;
                if (eye_callback_registered)
                    eye_focus = SRanipal_Eye_v2.Focus(index, out GazeRay, out FocusInfo, 0, MaxDistance, (1 << dart_board_layer_id), eyeData);
                else
                    eye_focus = SRanipal_Eye_v2.Focus(index, out GazeRay, out FocusInfo, 0, MaxDistance, (1 << dart_board_layer_id));

                if (eye_focus)
                {
                    DartBoard dartBoard = FocusInfo.transform.GetComponent<DartBoard>();
                    if (dartBoard != null)
                    {
                        dartBoard.Focus(FocusInfo.point);

                        isLookingAtDartboard = true; // Set the flag to true when looking at the dartboard

                        if (!isLookingAtDartboardPreviousFrame)
                        {
                            lookTimer = 0f; // Reset the timer when starting to look at the dartboard
                            Debug.Log("Started looking at dartboard");
                        }

                        lookTimer += Time.deltaTime;
                        if (lookTimer >= requiredLookTime)
                        {
                            if (experimentValues.currentSession == 1)
                            {
                                SceneManager.LoadScene("TestPupilScene");
                            }
                            else
                            {

                                SceneManager.LoadScene("Experiment");
                            }
                        }

                        foundDartboard = true;
                        break;
                    }
                }
            }

            if (!foundDartboard && isLookingAtDartboardPreviousFrame)
            {
                // Reset the timer if the user was previously looking at the dartboard but not anymore
                isLookingAtDartboard = false;
                lookTimer = 0f;
                Debug.Log("Stopped looking at dartboard");
            }
        }

        private void Release()
        {
            if (eye_callback_registered == true)
            {
                SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                eye_callback_registered = false;
            }
        }
        private static void EyeCallback(ref EyeData_v2 eye_data)
        {
            eyeData = eye_data;
        }
    }
}