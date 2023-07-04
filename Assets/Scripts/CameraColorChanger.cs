using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ViveSR.anipal.Eye;

public class CameraColorChanger : MonoBehaviour
{
    public GameObject Square;
    public GameObject pupilDataGatherer;
    private bool isTestingPupils;

    private void Awake()
    {
    }

    private void Start()
    {
        DataGatherer dataGatherer = pupilDataGatherer.GetComponent<DataGatherer>();

        if (dataGatherer != null)
        {
            StartCoroutine(GatherDataCoroutine(dataGatherer));
        }
        else
        {
            Debug.LogError("DataGatherer component not found on the pupilDataGatherer GameObject!");
        }
    }

    private IEnumerator GatherDataCoroutine(DataGatherer dataGatherer)
    {
        isTestingPupils = true;

        // Start testing pupils with "White" color
        StartCoroutine(dataGatherer.TestPupils("White"));

        yield return new WaitForSeconds(5f);

        // Stop testing pupils with "White" color
        StopTestingPupils(dataGatherer);

        // Start testing pupils with "Black" color
        StartCoroutine(dataGatherer.TestPupils("Black"));
        Square.SetActive(true);

        yield return new WaitForSeconds(5f);

        // Stop testing pupils with "Black" color
        StopTestingPupils(dataGatherer);

        // Load the "Experiment" scene
        SceneManager.LoadScene("Experiment");
    }

    private void StopTestingPupils(DataGatherer dataGatherer)
    {
        // Stop the coroutine by setting the boolean variable to false
        isTestingPupils = false;

        // Wait for the current frame to complete to ensure the coroutine is stopped
        StartCoroutine(WaitForEndOfFrame(() =>
        {
            // Stop the actual coroutine by calling StopCoroutine with the coroutine method name
            StopCoroutine(dataGatherer.TestPupils("White"));
        }));
    }

    private IEnumerator WaitForEndOfFrame(Action callback)
    {
        yield return new WaitForEndOfFrame();
        callback?.Invoke();
    }
}
