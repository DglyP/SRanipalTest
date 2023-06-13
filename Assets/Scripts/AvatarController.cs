using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ViveSR.anipal.Eye;

public class AvatarController : MonoBehaviour
{
    public GameObject EyesWithPupil;
    public Material material;
    private Renderer rendererComponent;
    public ExperimentValues numberOfStimuli;
    public ExperimentValues readyToChange;
    public GameObject pupilDataGatherer;
    private int pupilApertureID;
    public float speed = 4f;

    private Coroutine gatherDataCoroutine;

    private void OnEnable()
    {
        StartCoroutine(PupilManager());
    }

    private void OnDisable()
    {
        StopGatherDataCoroutine();
    }

    private void StopGatherDataCoroutine()
    {
        if (gatherDataCoroutine != null)
        {
            Debug.Log("Stopped gathering data");
            StopCoroutine(gatherDataCoroutine);
            gatherDataCoroutine = null;
        }
    }

    private IEnumerator WaitToGatherData()
    {
        // Call the GatherData coroutine repeatedly during the 15 seconds
        // Start the GatherData coroutine from the DataGatherer script in PupilDataGatherer
        Debug.Log("Waiting for 15 seconds to gather data");
        yield return new WaitForSeconds(15f);
        Debug.Log("Stopping Gathering Data");
        StopCoroutine(gatherDataCoroutine );
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

        Debug.Log("Waiting for 2 seconds before gathering data");
        yield return new WaitForSeconds(2f);
        Debug.Log("2 Seconds passed, now gathering data");

        DataGatherer dataGatherer = pupilDataGatherer.GetComponent<DataGatherer>();
        gatherDataCoroutine = StartCoroutine(dataGatherer.GatherData());

        rendererComponent = EyesWithPupil.GetComponent<Renderer>();

        pupilApertureID = Shader.PropertyToID("Vector1_FEA38ABB");

        float startValue = material.GetFloat(pupilApertureID);

        float targetValue = GetNewPupilSize(startValue);
        float transitionDuration = speed;

        Debug.Log("Performing transition of pupils");
        yield return StartCoroutine(ChangePupilSize(startValue, targetValue, transitionDuration, material));
        Debug.Log("Transition completed!");

        // Wait for 15 seconds before calling GatherData
        yield return StartCoroutine(WaitToGatherData());

        // Stop the GatherData coroutine
        StopGatherDataCoroutine();
    }

    private float GetNewPupilSize(float currentValue)
    {
        if (currentValue < 0.5f)
        {
            return 1f;
        }
        else
        {
            return 0f;
        }
    }

    public IEnumerator ChangePupilSize(float startValue, float targetValue, float duration, Material material)
    {
        float timeElapsed = 0f;
        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / duration);
            float value = Mathf.Lerp(startValue, targetValue, t);
            material.SetFloat(pupilApertureID, value);
            yield return null;
        }
    }
}
