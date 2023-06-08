using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarController : MonoBehaviour
{
    public GameObject EyesWithPupil;
    public Material material;
    private Renderer rendererComponent;
    public ExperimentValues numberOfStimuli;
    public ExperimentValues readyToChange;
    private int pupilApertureID;
    public float speed = 4f;

    // Define a delegate type for the callback
    public delegate void PupilSizeChangedCallback();

    // Event that will be triggered when the pupil size is changed
    public event PupilSizeChangedCallback OnPupilSizeChanged;

    private void OnEnable()
    {
        StartCoroutine(PupilManager());
        OnPupilSizeChanged += HandlePupilSizeChangedCoroutine;
    }

    private void HandlePupilSizeChangedCoroutine()
    {
        // Implement your logic here
        StartCoroutine(WaitToGatherData());
        Debug.Log("Pupil size done!");
    }

    private IEnumerator WaitToGatherData()
    {
        // Implement your coroutine logic here
        Debug.Log("Waiting to get eye tracking data");
        yield return new WaitForSeconds(15f);
        Debug.Log("Coroutine completed.");
        readyToChange.stageReady = true;
    }

    private IEnumerator PupilManager()
    {
        yield return new WaitForSeconds(1f);

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer rendererComponent in renderers)
        {
            rendererComponent.enabled = true; // Enable rendering
        }

        yield return new WaitForSeconds(5f);
        Debug.Log("FiveSecondsPassed");
        rendererComponent = EyesWithPupil.GetComponent<Renderer>();

        // Assuming the material you want to access is at index 0
        //Material material = rendererComponent.materials[0];

        // Get the ID for the 'PupilAperture' property using the reference in the shader graph
        pupilApertureID = Shader.PropertyToID("Vector1_FEA38ABB");
        Debug.Log("Pupil Aperture ID: " + pupilApertureID);

        // Get the current value of the 'PupilAperture' property
        float startValue = material.GetFloat(pupilApertureID);
        Debug.Log("Pupil Aperture value: " + startValue);

        // Run the method and trigger the event before starting the coroutine
        MethodToRunBeforePupilSizeChanged();
        OnPupilSizeChanged?.Invoke();

        // Specify the target value and duration for the smooth transition
        float targetValue = GetNewPupilSize(startValue);
        float transitionDuration = speed;

        // Perform the smooth transition
        yield return StartCoroutine(ChangePupilSize(startValue, targetValue, transitionDuration, material));

        // After the transition, you can perform any additional actions here
        // Trigger the event after the pupil size has changed
        OnPupilSizeChanged?.Invoke();
    }

    private void MethodToRunBeforePupilSizeChanged()
    {
        // Run your additional method here
    }

    private float GetNewPupilSize(float startValue)
    {
        float targetValue = (startValue < 0.5f) ? 0.8f : (startValue > 0.5f) ? 0f : 0.5f;
        return targetValue;
    }

    public IEnumerator ChangePupilSize(float startValue, float targetValue, float duration, Material material)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float currentValue = Mathf.Lerp(startValue, targetValue, t);
            material.SetFloat(pupilApertureID, currentValue);
            Debug.Log("Changing, currently at " + currentValue);
            yield return null;
            elapsedTime += Time.deltaTime;
        }

        // Ensure the final value is set accurately
        material.SetFloat(pupilApertureID, targetValue);

    }
}
