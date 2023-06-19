﻿using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
#if UNITY_EDITOR && !COMPILER_UDONSHARP
using UnityEditor;
using UdonSharpEditor;
#endif

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LogicalORActivator : ActivatorBase
    {
        public UdonSharpBehaviour[] inputActivators;

        public void OnEvent()
        {
            foreach (var activator in inputActivators)
            {
                if ((bool)activator.GetProgramVariable("state"))
                {
                    State = true;
                    return;
                }
            }
            State = false;
        }
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [InitializeOnLoad]
    public static class LogicalORActivatorOnBuild
    {
        static LogicalORActivatorOnBuild()
        {
            OnBuildUtil.RegisterType<LogicalORActivator>(ActivatorEditorUtil.ActivatorOnBuildBase, order: 0);
            OnBuildUtil.RegisterType<LogicalORActivator>(SecondOnBuild, order: 1);
        }

        private static bool SecondOnBuild(UdonSharpBehaviour behaviour)
        {
            LogicalORActivator logicalORActivator = (LogicalORActivator)behaviour;
            foreach (var activator in logicalORActivator.inputActivators)
                ActivatorEditorUtil.AddActivatorToListeners(activator, ActivatorEditorUtil.ListenerEventType.OnStateChanged, logicalORActivator);
            return true;
        }
    }
    #endif
}
