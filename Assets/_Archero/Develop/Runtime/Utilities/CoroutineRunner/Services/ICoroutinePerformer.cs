using System.Collections;
using UnityEngine;

namespace Utilities.CoroutineRunner
{
    public interface ICoroutinePerformer
    {
        public Coroutine StartCoroutine(IEnumerator coroutineFunction);
        public Coroutine StopCoroutine(IEnumerator coroutineFunction);
    }
}