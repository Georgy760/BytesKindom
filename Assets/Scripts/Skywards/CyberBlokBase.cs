using System.Collections.Generic;
using UnityEngine;

namespace Skywards
{
    
    public delegate void CompleteCallback(GameObject go);
    public class CyberBlokBase : MonoBehaviour
    {
        private Dictionary<string, GameObject> attributesModels;
        private Dictionary<string, string> attributesName;

        private CompleteCallback callback;

        private int stepsNeeded;

        public void Setup(string metadata , int stepsToValidate, CompleteCallback callback)
        {
            stepsNeeded = stepsToValidate;
            attributesName = new Dictionary<string, string>();
            attributesModels = new Dictionary<string, GameObject>();
            this.callback = callback;
        }
    }
}