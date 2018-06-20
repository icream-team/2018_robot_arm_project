using UnityEngine;
using System.Collections;

namespace PVR
{
    namespace Unity
    {
        public class PVREyeOffset : MonoBehaviour
        {

            public enum Eyes
            {
                Left = 0,
                Right
            }
            public Eyes Eye;

            // Use this for initialization
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {
                if (PVRSession.instance.ready)
                {
                    transform.localPosition = Math.ConvertPosition(PVRSession.instance.hmdToEyeOffset[Eye == Eyes.Left ? 0 : 1]);
                    transform.localRotation = Math.ConvertOrientation(PVRSession.instance.hmdToEyeRotation[Eye == Eyes.Left ? 0 : 1]);
                }
            }
        }
    }
}