using UnityEngine;
using System.Collections;

namespace LoadCustomDataMod.Services
{
    public class ObjectiveEventHandler : MonoBehaviour
    {
        public void StartTestEvent(IEnumerator action)
        {
            StartCoroutine(action);
        }
    }
}
