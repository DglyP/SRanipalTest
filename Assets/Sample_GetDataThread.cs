using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.IO;
using ViveSR.anipal.Eye;

namespace Test120FPS
{
    public class Sample_GetDataThread : MonoBehaviour
    {
        public EyeData data = new EyeData();
        private Thread thread;
        private const int FrequencyControl = 1;
        private const int MaxFrameCount = 3600;

        void Start()
        {
            thread = new Thread(QueryEyeData);
            thread.Start();
        }

        private void OnApplicationQuit()
        {
            thread.Abort();
        }

        private void OnDisable()
        {
            thread.Abort();
        }

        // You can only use C# native function in Unity's thread.
        // Use EyeData's frame_sequence to calculate frame numbers and record data in a file.
        void QueryEyeData()
        {
            int FrameCount = 0;
            int PrevFrameSequence = 0, CurrFrameSequence = 0;
            bool StartRecord = false;

            while (FrameCount < MaxFrameCount)
            {
                ViveSR.Error error = SRanipal_Eye_API.GetEyeData(ref data);

                if (error == ViveSR.Error.WORK)
                {
                    CurrFrameSequence = data.frame_sequence;

                    if (CurrFrameSequence != PrevFrameSequence)
                    {
                        FrameCount++;
                        PrevFrameSequence = CurrFrameSequence;
                        StartRecord = true;
                    }
                }

                // Record timestamp every 120 frames.
                if (FrameCount % 120 == 0 && StartRecord)
                {
                    long ms = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    string text = "CurrentFrameSequence: " + CurrFrameSequence +
                        " CurrentSystemTime(ms): " + ms.ToString() + Environment.NewLine;
                    File.AppendAllText("DataRecord.txt", text);
                    FrameCount = 0;
                }

                Thread.Sleep(FrequencyControl);
            }
        }
    }
}
