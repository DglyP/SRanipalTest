using System;
using System.Collections.Generic;
using System.Linq;
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

                public ExperimentValues numberOfStimuli;
                public ExperimentValues readyToChange;
                private int currentStimuli = 0;
                public GameObject realHuman;
                public GameObject avatar;
                public GameObject eyes;
                public TextMeshProUGUI textComponent;
                private string currentAvatar;
                private AvatarController avatarController;
                public Material RealMaterial;
                public Material AvatarMaterial;
                public Material EyesMaterial;
                List<string> keyList;
                Dictionary<string, string> randomizedDictionary;

                // Fisher-Yates randomization to shuffle a list
                private void ShuffleList<T>(List<T> list)
                {
                    System.Random random = new System.Random();
                    int n = list.Count;
                    while (n > 1)
                    {
                        n--;
                        int k = random.Next(n + 1);
                        T value = list[k];
                        list[k] = list[n];
                        list[n] = value;
                    }
                }

                public Dictionary<string, string> GenerateRandomizedDictionary(string[] items, string[] states, int numberOfStimuli)
                {
                    // Calculate the number of occurrences for each combination
                    int combinations = items.Length * states.Length;
                    int occurrencesPerCombination = numberOfStimuli / combinations;
                    int remainingStimuli = numberOfStimuli % combinations; // Calculate the remaining stimuli

                    // Create the list of all possible combinations
                    List<KeyValuePair<string, string>> combinationsList = new List<KeyValuePair<string, string>>();
                    foreach (string item in items)
                    {
                        foreach (string state in states)
                        {
                            combinationsList.Add(new KeyValuePair<string, string>(item, state));
                        }
                    }

                    // Shuffle the list of combinations
                    ShuffleList(combinationsList);

                    // Create the dictionary with the specified number of occurrences for each combination
                    Dictionary<string, string> randomizedDictionary = new Dictionary<string, string>();
                    int uniqueIdentifier = 0; // Unique identifier for each pair
                    foreach (KeyValuePair<string, string> combination in combinationsList)
                    {
                        int occurrences = occurrencesPerCombination;
                        if (remainingStimuli > 0)
                        {
                            occurrences++; // Add an additional occurrence for the remaining stimuli
                            remainingStimuli--;
                        }

                        for (int i = 0; i < occurrences; i++)
                        {
                            string key = combination.Key + "_" + uniqueIdentifier.ToString(); // Append the unique identifier to the item key
                            string value = combination.Value;
                            randomizedDictionary.Add(key, value);
                            uniqueIdentifier++;
                        }
                    }

                    return randomizedDictionary;
                }

                List<string> CreateListOfAvatars(string[] avatarList, string[] irisList, int numberOfStimuli)
                {
                    // Print the randomized dictionary
                    randomizedDictionary = GenerateRandomizedDictionary(avatarList, irisList, numberOfStimuli);

                    //If you want to see the pairs one by one UnComment this

                    //foreach (KeyValuePair<string, string> pair in randomizedDictionary)
                    //{
                    //    Debug.Log(pair.Key + " and " + pair.Value);
                    //}

                    // Extract the keys from the dictionary as the randomized list
                    List<string> randomAvatarList = randomizedDictionary.Keys.ToList();
                    return randomAvatarList;
                }

                // Start is called before the first frame update
                void Start()
                {
                    Debug.Log("stimuli amount is " + numberOfStimuli.stimuliAmount);
                    numberOfStimuli.stimuliAmount = numberOfStimuli.stimuliAmount == 0 ? 9 : numberOfStimuli.stimuliAmount;

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
                    string[] irisList = { "1", "0", "0.5" };

                    keyList = CreateListOfAvatars(avatarList, irisList, numberOfStimuli.stimuliAmount);

                    List<string> valueList = randomizedDictionary.Values.ToList();

                    // Log the list of keys
                    Debug.Log("List of Keys: " + string.Join("  -  ", keyList));
                    Debug.Log("List of Values: " + string.Join("  -  ", valueList));

                }

                void ChangeAvatar()
                {
                    realHuman.gameObject.SetActive(false);
                    avatar.gameObject.SetActive(false);
                    eyes.gameObject.SetActive(false);

                    switch (currentAvatar)
                    {
                        case var k when k.Contains("real"):
                            realHuman.gameObject.SetActive(true);
                            PrepareModel(realHuman, randomizedDictionary[currentAvatar], RealMaterial);
                            break;
                        case var k when k.Contains("avatar"):
                            avatar.gameObject.SetActive(true);
                            PrepareModel(avatar, randomizedDictionary[currentAvatar], AvatarMaterial);
                            break;
                        case var k when k.Contains("eyes"):
                            eyes.gameObject.SetActive(true);
                            PrepareModel(eyes, randomizedDictionary[currentAvatar], EyesMaterial);
                            break;
                        default:
                            // Handle any other case, if needed
                            break;
                    }
                }

                private void PrepareModel(GameObject model, string irisValue, Material material)
                {
                    Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
                    foreach (Renderer rendererComponent in renderers)
                    {
                        rendererComponent.enabled = false; // Disable rendering
                    }

                    avatarController = model.GetComponent<AvatarController>();

                    // Call the ChangePupilSize method with the desired parameters
                    float startstimuliAmount = 0;
                    float targetstimuliAmount = float.Parse(irisValue);
                    float duration = 0f;
                    avatarController.StartCoroutine(avatarController.ChangePupilSize(startstimuliAmount, targetstimuliAmount, duration, material));
                }

                void CheckIfDone()
                {
                    if (currentStimuli >= numberOfStimuli.stimuliAmount)
                    {
                        // Handle the case when all stimuli have been presented
                    }

                    if (readyToChange.stageReady == true)
                    {
                        readyToChange.stageReady = false;
                        currentAvatar = keyList[currentStimuli];
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
