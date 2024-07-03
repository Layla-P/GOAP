﻿using CrashKonijn.Goap.Core;
using UnityEngine;

namespace CrashKonijn.Goap.Runtime
{
    [DefaultExecutionOrder(-99)]
    public class AgentTypeBehaviour : MonoBehaviour
    {
        [SerializeField]
        private AgentTypeScriptable config;

        [SerializeField]
        private GoapBehaviour runner;

        public IAgentType AgentType { get; private set; }
        public AgentTypeScriptable Config => this.config;

        private void Awake()
        {
            var agentType = new AgentTypeFactory(this.runner.Config).Create(this.config.Create());

            this.runner.Register(agentType);
            
            this.AgentType = agentType;
        }
    }
}