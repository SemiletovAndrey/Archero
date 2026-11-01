using System;

namespace Template.Features.NewInputFeature
{
    public interface IInputBus
    {
        public void Publish<TSignal>(TSignal signal) where TSignal : struct, IInputSignal;
        public void Subscribe<TSignal>(Action<TSignal> callback) where TSignal : struct, IInputSignal;
        public void Unsubscribe<TSignal>(Action<TSignal> callback) where TSignal : struct, IInputSignal;
    }
}