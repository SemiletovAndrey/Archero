using System;
using UnityEngine;

namespace Template.Scripts.DataHolders
{
    [Serializable]
    public struct SimpleTimer
    {
        [SerializeField] private float _startTime;
        [SerializeField] private float _delay;

        private SimpleTimer(float delay)
        {
            _startTime = Time.time;
            _delay = delay;
            IsRunning = true;
        }
        
        [field: SerializeField] public bool IsRunning { get; private set; }

        public float? RemainingTime()
        {
            if (!IsRunning)
            {
                return null;
            }
            
            float endTime = _startTime + _delay;
            float currentTime = Time.time;
            
            if (currentTime > endTime)
            {
                return null;
            }
            
            return endTime - currentTime;
        }

        public bool Expired()
        {
            return RemainingTime() == null;
        }

        public static SimpleTimer CreateFromSeconds(float delay)
        {
            return new SimpleTimer(delay);
        }
    }
}