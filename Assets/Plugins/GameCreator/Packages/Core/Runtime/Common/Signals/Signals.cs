using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [AddComponentMenu("")]
    public class Signals : Singleton<Signals>
    {
        [NonSerialized]
        private Dictionary<PropertyName, List<ISignalReceiver>> m_Signals;
        
        // INITIALIZERS: --------------------------------------------------------------------------

        protected override void OnCreate()
        {
            base.OnCreate();
            this.m_Signals = new Dictionary<PropertyName, List<ISignalReceiver>>();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        /// <summary>
        /// Raises an event with the specified <paramref name="signal"/>. If any receivers are
        /// listening, these will be invoked in order. 
        /// </summary>
        /// 
        public void Raise(SignalArgs args)
        {
            if (ApplicationManager.IsExiting) return;
            if (!this.m_Signals.TryGetValue(args.signal, out List<ISignalReceiver> receivers))
            {
                return;
            }
            
            foreach (ISignalReceiver receiver in receivers)
            {
                receiver.OnReceiveSignal(args);
            }
        }

        // SUBSCRIPTION METHODS: ------------------------------------------------------------------

        /// <summary>
        /// Starts listening for the specific <paramref name="signal"/>. When that signal is raised
        /// it invokes the <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The implementing object that listens to the signal</param>
        /// <param name="signal">The signal to listen</param>
        public void Subscribe(ISignalReceiver source, PropertyName signal)
        {
            if (!this.m_Signals.TryGetValue(signal, out List<ISignalReceiver> receivers))
            {
                receivers = new List<ISignalReceiver>();
                this.m_Signals.Add(signal, receivers);
            }

            foreach (ISignalReceiver receiver in receivers)
            {
                if (receiver == source) return;
            }
            
            receivers.Add(source);
        }

        /// <summary>
        /// Stops the object <paramref name="source"/> implementing the interface from listening
        /// the <paramref name="signal"/> 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="signal"></param>
        public void Unsubscribe(ISignalReceiver source, PropertyName signal)
        {
            if (!this.m_Signals.TryGetValue(signal, out List<ISignalReceiver> receivers)) return;
            receivers.Remove(source);
                
            if (receivers.Count > 0) return;
            this.m_Signals.Remove(signal);
        }
    }
}