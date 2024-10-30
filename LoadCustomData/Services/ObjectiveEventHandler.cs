using UnityEngine;
using System.Collections;

namespace SRMod.Services
{
    public class ObjectiveEventHandler : MonoBehaviour
    {
        public void StartTestEvent(IEnumerator action)
        {
            StartCoroutine(action);
        }
    }
}
