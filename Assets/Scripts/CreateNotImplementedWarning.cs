using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace malvra
{
    public class CreateNotImplementedWarning : MonoBehaviour
    {
        public GameObject notImplementedWarningPanelPrefab;

        public void CreateWarning()
        {
            GameObject notImplementedWarningPanel = Instantiate<GameObject>(notImplementedWarningPanelPrefab, Vector3.forward, Quaternion.identity);
            GameObject.Destroy(notImplementedWarningPanel, 2.0f);
        }
    }
}
