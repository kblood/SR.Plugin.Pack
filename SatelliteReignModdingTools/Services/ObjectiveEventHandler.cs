using UnityEngine;
using System.Collections;

namespace SyndicateMod.Services
{
    public class ObjectiveEventHandler : MonoBehaviour
    {
        public void StartTestEvent(IEnumerator action)
        {
            StartCoroutine(action);
        }
    }
}
