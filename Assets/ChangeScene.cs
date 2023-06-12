using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void ChangeToExperimentScene()
    {
        SceneManager.LoadScene("Experiment");
    }
}