using System;
using System.Collections;
using UnityEngine;

namespace Utilities.CoroutineRunner
{
    public class CoroutinePerformer : MonoBehaviour, ICoroutinePerformer
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public Coroutine StartCoroutine(IEnumerator coroutineFunction)
        {
            return StartCoroutine(coroutineFunction);
        }
        
        public Coroutine StopCoroutine(IEnumerator coroutineFunction)
        {
            return StopCoroutine(coroutineFunction);
        }
    }
}