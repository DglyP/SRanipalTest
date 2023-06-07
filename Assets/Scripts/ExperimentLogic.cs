using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {
            public class ExperimentLogic : MonoBehaviour
            {
                EyeData eye;

                public int NumberOfStimuli;
                private int currentStimuli = 0;
                public GameObject realHuman;
                public GameObject avatar;
                public GameObject eyes;
                public TextMeshProUGUI textComponent;
                private string currentAvatar;
                public bool readyToChange;
                List<string> randomList;

                //Fisher-Yates randomization to create the list of stimuli
                public List<string> GenerateRandomList(string[] avatarList, int amountOfTests)
                {
                    List<string> randomList = new List<string>();

                    int eventCount = avatarList.Length;
                    int occurrences = NumberOfStimuli / eventCount;

                    // Repeat each event an equal number of times
                    foreach (string ev in avatarList)
                    {
                        for (int i = 0; i < occurrences; i++)
                        {
                            randomList.Add(ev);
                        }
                    }

                    // Perform Fisher-Yates shuffle to randomize the list
                    System.Random random = new System.Random();
                    int n = randomList.Count;
                    while (n > 1)
                    {
                        n--;
                        int k = random.Next(n + 1);
                        string temp = randomList[k];
                        randomList[k] = randomList[n];
                        randomList[n] = temp;
                    }

                    return randomList;
                }

                // Start is called before the first frame update
                void Start()
                {
                    NumberOfStimuli = NumberOfStimuli == 0 ? 9 : NumberOfStimuli;

                    if (SRanipal_Eye_API.GetEyeData(ref eye) == ViveSR.Error.WORK)
                    {
                        // Change the content of the text
                        textComponent.text = "Please check the eye-tracker";
                        textComponent.gameObject.SetActive(true);
                    }
                    else
                    {
                        textComponent.gameObject.SetActive(true);
                    }


                    string[] avatarList = { "real", "avatar", "eyes" };
                    randomList = GenerateRandomList(avatarList, NumberOfStimuli);
                    //randomList.Add("Done");

                    string listString = string.Join(", ", randomList.ToArray());
                    Debug.Log(listString);
                }

                void ChangeAvatar()
                {
                    realHuman.gameObject.SetActive(false);
                    avatar.gameObject.SetActive(false);
                    eyes.gameObject.SetActive(false);

                    switch (currentAvatar)
                    {
                        case "real":
                            realHuman.gameObject.SetActive(true);
                            break;
                        case "avatar":
                            avatar.gameObject.SetActive(true);
                            break;
                        case "eyes":
                            eyes.gameObject.SetActive(true);
                            break;
                        default:
                            // Handle any other case, if needed
                            break;
                    }
                }

                void CheckIfDone()
                {

                    if (currentStimuli >= NumberOfStimuli)
                    {
                    }

                    if (readyToChange == true)
                    {
                        readyToChange = false;
                        currentAvatar = randomList[currentStimuli];
                        ChangeAvatar();
                        currentStimuli += 1;
                    }
                }

                // Update is called once per frame
                void Update()
                {
                    CheckIfDone();
                }
            }
        }
    }
}
