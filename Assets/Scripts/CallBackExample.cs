using UnityEngine;
using ViveSR.anipal.Eye;
using System.Runtime.InteropServices;
using System.IO;

public class CallBackExample : MonoBehaviour
{
    private EyeData_v2 eyeData = new EyeData_v2();
    private bool eye_callback_registered = false;
    private bool isQuitting = false; // Added flag for quitting
    private StreamWriter logWriter; // Added StreamWriter for writing logs to a file

    public OrganizeData_csv organizeData_Csv;
    public ExperimentValues experimentValues;

    private void Start()
    {
        // Open the log file for writing
        string logFilePath = Application.dataPath + "/EyeLogs.csv";
        logWriter = new StreamWriter(logFilePath, false);
        logWriter.WriteLine("Timestamp (ms),Left Pupil Diameter (mm),Right Pupil Diameter (mm)");

    }

    private void OnDisable()
    {
        Release();

        // Close the log file
        if (logWriter != null)
        {
            logWriter.Close();
            logWriter = null;
        }
    }

    private void OnApplicationQuit()
    {
        isQuitting = true; // Set the quitting flag
        Release();

        // Close the log file
        if (logWriter != null)
        {
            logWriter.Close();
            logWriter = null;
        }
    }

    private void Update()
    {
        if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING) return;

        if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback && !eye_callback_registered)
        {
            SRanipal_Eye.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
            eye_callback_registered = true;
        }
        else if (!SRanipal_Eye_Framework.Instance.EnableEyeDataCallback && eye_callback_registered)
        {
            SRanipal_Eye.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
            eye_callback_registered = false;
        }
    }

    private void Release()
    {
        if (eye_callback_registered)
        {
            SRanipal_Eye.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
            eye_callback_registered = false;
        }
    }

    internal class MonoPInvokeCallbackAttribute : System.Attribute
    {
        public MonoPInvokeCallbackAttribute() { }
    }

    [MonoPInvokeCallback]
    private void EyeCallback(ref EyeData_v2 eye_data)
    {
        // Check for the quitting condition
        if (isQuitting)
            return;

        eyeData = eye_data;
        Debug.Log(eyeData.verbose_data.left.pupil_diameter_mm);
        Debug.Log(eyeData.verbose_data.right.pupil_diameter_mm);

        // Get the current timestamp in milliseconds
        long timestamp = System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;

        // Write the data and timestamp to the log file
        if (logWriter != null)
        {
            string logLine = timestamp + "," + eyeData.verbose_data.left.pupil_diameter_mm + "," + eyeData.verbose_data.right.pupil_diameter_mm;
            logWriter.WriteLine(logLine);
            logWriter.Flush();
        }
    }
}
