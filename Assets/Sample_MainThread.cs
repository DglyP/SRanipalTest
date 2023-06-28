using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViveSR.anipal.Eye;

namespace Test120FPS
{
    public class Sample_MainThread : MonoBehaviour
    {
        private Sample_GetDataThread DataThread = null;
        private EyeData data = new EyeData();

        // Use this for initialization
        void Start()
        {
            DataThread = FindObjectOfType<Sample_GetDataThread>();
            if (DataThread == null) return;
        }

        // You can get data from another thread and use MonoBehaviour's method here.
        // But in Unity's Update function, you can only have 90 FPS.
        void Update()
        {
            data = DataThread.data;
            Debug.Log("Left eye openness: " + data.verbose_data.left.eye_openness);
            Debug.Log("Right eye openness: " + data.verbose_data.right.eye_openness);
        }
    }
}
