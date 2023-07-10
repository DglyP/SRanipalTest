using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {
            public class DataGatherer : MonoBehaviour
            {
                public OrganizeData organizeData;
                public OrganizeData_csv organizeData_Csv;
                public ExperimentValues experimentValues;

                EyeData_v2 eye;
                Vector2 LeftPupil;
                Vector2 RightPupil;
                float LeftBlink;
                float RightBlink;

                private void Start()
                {
                    //Debug.Log("Started Script of Data");
                }

                public IEnumerator GatherData()
                {
                    while (true)
                    {
                        SRanipal_Eye_API.GetEyeData_v2(ref eye);
                        experimentValues.UserLeftPupilSize = eye.verbose_data.left.pupil_diameter_mm;
                        experimentValues.UserRightPupilSize = eye.verbose_data.right.pupil_diameter_mm;
                        organizeData_Csv.AppendDataToCsv(false);
                        yield return null;
                    }
                }

                public IEnumerator TestPupils(string state)
                {
                    while (true)
                    {
                        if (SRanipal_Eye_API.GetEyeData_v2(ref eye) == ViveSR.Error.WORK)
                        {
                            float leftPupilSize = eye.verbose_data.left.pupil_diameter_mm;
                            float rightPupilSize = eye.verbose_data.right.pupil_diameter_mm;
                            if (leftPupilSize >= 0 && rightPupilSize >= 0)
                            {
                                if (experimentValues.minPupilSize < 0)
                                    experimentValues.minPupilSize = Mathf.Max(leftPupilSize, rightPupilSize);
                                else
                                    experimentValues.minPupilSize = Mathf.Min(experimentValues.minPupilSize, leftPupilSize, rightPupilSize);

                                experimentValues.maxPupilSize = Mathf.Max(experimentValues.maxPupilSize, leftPupilSize, rightPupilSize);
                            }
                            organizeData_Csv.AppendDataToCsv(false);
                        }
                        yield return null;
                    }
                }
            }
        }
    }
}
