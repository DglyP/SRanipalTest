using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {
            public class ExperimentLogic : MonoBehaviour
            {
                EyeData_v2 eye;

                public ExperimentValues experimentValues;
                private int currentStimuli = 0;
                public GameObject realHuman;
                public GameObject avatar;
                public GameObject eyes;
                public GameObject startInstructions;
                public GameObject endInstructions;
                public TextMeshProUGUI textComponent;
                private string currentAvatar;
                private AvatarController avatarController;
                public Material RealMaterial;
                public Material AvatarMaterial;
                public Material EyesMaterial;
                List<string> keyList;
                Dictionary<string, string> processedDictionary;
                Dictionary<string, string> randomizedDictionary;
                private bool isCoroutineRunning = false;
                private bool coroutineDone;
                private bool instructionsGiven;

                public OrganizeData organizeData;
                public OrganizeData_csv organizeData_csv;

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

                Dictionary<string, string> MultiplyDictionary(Dictionary<string, string> dictionary, int multiplier)
                {
                    Dictionary<string, string> multipliedDictionary = new Dictionary<string, string>();
                    int uniqueIdentifier = 0;

                    foreach (KeyValuePair<string, string> pair in dictionary)
                    {
                        for (int i = 0; i < multiplier; i++)
                        {
                            string key = pair.Key + "_" + uniqueIdentifier.ToString();
                            multipliedDictionary.Add(key, pair.Value);
                            uniqueIdentifier++;
                        }
                    }

                    return multipliedDictionary;
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
                    //Debug.Log("Stimuli amount is " + experimentValues.stimuliAmount);
                    experimentValues.stimuliAmount = experimentValues.stimuliAmount == 0 ? 9 : experimentValues.stimuliAmount;
                    experimentValues.stageReady = true;

                    Dictionary<string, string> originalDictionary = new Dictionary<string, string>();

                    originalDictionary.Add("real_0", "0");
                    originalDictionary.Add("real_1", "0.5");
                    originalDictionary.Add("real_2", "1");
                    originalDictionary.Add("eyes_3", "0");
                    originalDictionary.Add("eyes_4", "0.5");
                    originalDictionary.Add("eyes_5", "1");
                    originalDictionary.Add("avatar_6", "0");
                    originalDictionary.Add("avatar_7", "0.5");
                    originalDictionary.Add("avatar_8", "1");

                    int stimuliAmount = (experimentValues.stimuliAmount / 9); // Get the value of stimuliAmount
                    processedDictionary = MultiplyDictionary(originalDictionary, stimuliAmount);

                    if (SRanipal_Eye_API.GetEyeData_v2(ref eye) != ViveSR.Error.WORK)
                    {
                        // Change the content of the text
                        textComponent.text = "Please check the eye-tracker";
                        textComponent.gameObject.SetActive(true);
                    }
                    else
                    {
                        textComponent.gameObject.SetActive(false);
                    }

                    //string[] avatarList = { "real", "avatar", "eyes" };
                    //string[] irisList = { "1", "0", "0.5" };

                    //keyList = CreateListOfAvatars(avatarList, irisList, experimentValues.stimuliAmount);

                    List<string> randomAvatarList = processedDictionary.Keys.ToList();
                    ShuffleList(randomAvatarList); // Shuffle the list of avatars
                    keyList = randomAvatarList;

                    List<string> valueList = new List<string>();
                    foreach (string key in randomAvatarList)
                    {
                        string value = processedDictionary[key];
                        valueList.Add(value);
                    }


                    Debug.Log("List of Keys: " + string.Join("  -  ", keyList));
                    Debug.Log("List of Values: " + string.Join("  -  ", valueList));
                    // Log the list of keys
                    //organizeData.AppendDataToXml("Avatar_List_" + string.Join("-", keyList), "Avatar_Values_" + (string.Join("-", valueList)), false);
                }

                void ChangeAvatar()
                {
                    realHuman.gameObject.SetActive(false);
                    avatar.gameObject.SetActive(false);
                    eyes.gameObject.SetActive(false);

                    switch (currentAvatar)
                    {
                        case var k when k.Contains("real"):
                            //Debug.Log("Making Human");
                            realHuman.gameObject.SetActive(true);
                            //organizeData.AppendDataToXml("Stimuli", "Human", false);
                            experimentValues.currentAvatarShown = "Human";
                            organizeData_csv.AppendDataToCsv(false);
                            PrepareModel(realHuman, processedDictionary[currentAvatar], RealMaterial);
                            break;
                        case var k when k.Contains("avatar"):
                            //Debug.Log("Making Anime");
                            avatar.gameObject.SetActive(true);
                            //organizeData.AppendDataToXml("Stimuli", "Anime", false);
                            experimentValues.currentAvatarShown = "Anime";
                            organizeData_csv.AppendDataToCsv(false);
                            PrepareModel(avatar, processedDictionary[currentAvatar], AvatarMaterial);
                            break;
                        case var k when k.Contains("eyes"):
                            //Debug.Log("Making Eyes");
                            eyes.gameObject.SetActive(true);
                           // organizeData.AppendDataToXml("Stimuli", "Eyes", false);
                            experimentValues.currentAvatarShown = "Eyes";
                            organizeData_csv.AppendDataToCsv(false);
                            PrepareModel(eyes, processedDictionary[currentAvatar], EyesMaterial);
                            break;
                        default:
                            // Handle any other case, if needed
                            break;
                    }
                }


                private void PrepareModel(GameObject model, string irisValue, Material material)
                {
                    Debug.Log("The pupil should be changing towards " + irisValue + " here");
                    Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
                    foreach (Renderer rendererComponent in renderers)
                    {
                        rendererComponent.enabled = false; // Disable rendering
                    }

                    avatarController = model.GetComponent<AvatarController>();

                    float startstimuliAmount = material.GetFloat(Shader.PropertyToID("Vector1_FEA38ABB"));
                    float initialStimuliAmount = float.Parse(irisValue);
                    Debug.Log(startstimuliAmount + " changing to " + initialStimuliAmount);

                    experimentValues.PupilSizeChanging = false;
                    material.SetFloat(Shader.PropertyToID("Vector1_FEA38ABB"), initialStimuliAmount);
                    experimentValues.StimuliPupilSize = initialStimuliAmount;
                    organizeData_csv.AppendDataToCsv(true);


                    //avatarController.StartCoroutine(avatarController.ChangePupilSize(startstimuliAmount, initialStimuliAmount, duration, material));

                    Debug.Log( " Done, the pupil of the stimuli is " + initialStimuliAmount + " see " + experimentValues.StimuliPupilSize + "and" + material.GetFloat(Shader.PropertyToID("Vector1_FEA38ABB")) );

                }

                private IEnumerator GiveInstructions()
                {
                    //Debug.Log("Waiting for 5 seconds...");
                    startInstructions.SetActive(true);
                    coroutineDone = false;
                    yield return new WaitForSeconds(5f);
                    coroutineDone = true;
                    startInstructions.SetActive(false);
                    //Debug.Log("Continuing after 5 seconds.");
                    isCoroutineRunning = false; // Set the flag to indicate that the coroutine has finished
                    instructionsGiven = true;
                }

                private IEnumerator EndSession()
                {
                    //Debug.Log("Waiting for 5 seconds after end...");
                    realHuman.gameObject.SetActive(false);
                    avatar.gameObject.SetActive(false);
                    eyes.gameObject.SetActive(false);
                    endInstructions.SetActive(true);
                    yield return new WaitForSeconds(5f);
                    endInstructions.SetActive(false);
                    //Debug.Log("Continuing after 5 seconds...");
                    if (experimentValues.currentSession == experimentValues.numberOfSessions)
                    {
                        SceneManager.LoadScene("EndScene");
                    }
                    else
                    {
                        SceneManager.LoadScene("StartScene");
                    }
                    isCoroutineRunning = false; // Set the flag to indicate that the coroutine has finished
                }


                void CheckIfDone()
                {
                    if (currentStimuli >= experimentValues.stimuliAmount && experimentValues.stageReady)
                    {
                        if (!isCoroutineRunning) // Check if the coroutine is already running
                        {
                            isCoroutineRunning = true;
                            StartCoroutine(EndSession());
                        }
                    }

                    if (currentStimuli == 0)
                    {
                        if (experimentValues.stageReady)
                        {
                            //Debug.Log("Instructions are being shown");
                            if (!isCoroutineRunning && !instructionsGiven) // Check if the coroutine is already running
                            {
                                isCoroutineRunning = true;
                                StartCoroutine(GiveInstructions());
                            }
                            if (coroutineDone)
                            {
                                //Debug.Log("5 seconds passed");
                                currentAvatar = keyList[currentStimuli];
                                currentStimuli += 1;
                                ChangeAvatar();
                                experimentValues.stageReady = false;
                            }
                        }
                    }
                    if (currentStimuli > 0 && currentStimuli < keyList.Count)
                    {
                        if (experimentValues.stageReady)
                        {
                            currentAvatar = keyList[currentStimuli];
                            currentStimuli += 1;
                            ChangeAvatar();
                            experimentValues.stageReady = false;
                        }
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
