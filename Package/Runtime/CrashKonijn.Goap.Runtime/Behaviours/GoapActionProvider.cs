﻿using System;
using CrashKonijn.Agent;
using CrashKonijn.Agent.Core;
using CrashKonijn.Agent.Runtime;
using CrashKonijn.Goap.Core;
using UnityEngine;

namespace CrashKonijn.Goap.Runtime
{
    public class GoapActionProvider : ActionProviderBase, IMonoGoapActionProvider
    {
        [field: SerializeField]
        public LoggerConfig LoggerConfig { get; set; } = new LoggerConfig();
        
        [field: SerializeField]
        public AgentTypeBehaviour AgentTypeBehaviour { get; set; }
        
        [field: SerializeField]
        public float DistanceMultiplier { get; set; } = 1f;
        
        private IAgentType agentType;
        public IAgentType AgentType
        {
            get => this.agentType;
            set
            {
                this.agentType = value;
                this.WorldData.SetParent(value.WorldData);
                value.Register(this);
                
                this.Events.Bind(this, value.Events);
            }
        }
        public IGoal CurrentGoal { get; private set; }
        public ILocalWorldData WorldData { get; } = new LocalWorldData();
        public IConnectable[] CurrentPlan { get; private set; } = Array.Empty<IConnectable>();
        public IGoapAgentEvents Events { get; } = new GoapAgentEvents();
        public ILogger<IMonoGoapActionProvider> Logger { get; } = new GoapAgentLogger();

        private IActionReceiver agent;
        public IActionReceiver Agent
        {
            get
            {
                if (this.agent == null)
                    this.agent = this.GetComponent<AgentBehaviour>();
        
                return this.agent;
            }
        }
        
        public Vector3 Position => this.transform.position;

        private void Awake()
        {
            if (this.AgentTypeBehaviour != null)
                this.AgentType = this.AgentTypeBehaviour.AgentType;
            
            this.Logger.Initialize(this.LoggerConfig, this);
        }
        
        private void Start()
        {
            if (this.AgentType == null)
                throw new GoapException($"There is no AgentType assigned to the agent '{this.name}'! Please assign one in the inspector or through code in the Awake method.");
        }
        
        private void OnEnable()
        {
            if (this.AgentType == null)
                return;
            
            this.AgentType.Register(this);
        }

        private void OnDisable()
        {
            this.StopAction(false);

            if (this.AgentType == null)
                return;
            
            this.AgentType.Unregister(this);
            this.Events.Unbind();
        }

        public void SetGoal<TGoal>(bool endAction)
            where TGoal : IGoal
        {
            this.SetGoal(this.AgentType.ResolveGoal<TGoal>(), endAction);
        }

        public void SetGoal(IGoal goal, bool endAction)
        {
            if (goal == this.CurrentGoal)
                return;
            
            this.CurrentGoal = goal;
            this.Agent.Timers.Goal.Touch();
            
            if (this.Agent.ActionState.Action == null)
                this.ResolveAction();
            
            this.Events.GoalStart(goal);
            
            if (endAction)
                this.StopAction();
        }

        public void SetAction(IGoapAction action, IConnectable[] path)
        {
            this.CurrentPlan = path;
            this.Agent.SetAction(this, action, this.WorldData.GetTarget(action));
        }

        public void StopAction(bool resolveAction = true)
        {
            this.Agent.StopAction(resolveAction);
        }
        
        public override void ResolveAction()
        {
            this.Events.Resolve();
            this.Agent.Timers.Resolve.Touch();
        }

        public void ClearGoal()
        {
            this.CurrentGoal = null;
        }
        
        public void SetDistanceMultiplierSpeed(float speed)
        {
            this.DistanceMultiplier = 1f / speed;
        }
    }
}