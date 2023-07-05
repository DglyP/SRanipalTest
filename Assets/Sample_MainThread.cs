using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViveSR.anipal.Eye;

namespace Test120FPS
{
    public class Sample_MainThread : MonoBehaviour
    {
        private Sample_GetDataThread DataThread = null;
        private EyeData_v2 data = new EyeData_v2();
        public OrganizeData_csv organizeData_Csv;
        public ExperimentValues experimentValues;

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
            experimentValues.UserLeftPupilSize = data.verbose_data.left.pupil_diameter_mm;
            experimentValues.UserRightPupilSize = data.verbose_data.right.pupil_diameter_mm;
            organizeData_Csv.AppendDataToCsv(false);
        }
    }
}
