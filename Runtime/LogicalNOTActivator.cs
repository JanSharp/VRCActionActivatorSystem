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
    public class LogicalNOTActivator : ActivatorBase
    {
        public ActivatorBase inputActivator;

        private void Start()
        {
            OnEvent();
        }

        public void OnEvent()
        {
            State = !(bool)inputActivator.GetProgramVariable("state");
        }
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [InitializeOnLoad]
    public static class LogicalNOTActivatorOnBuild
    {
        static LogicalNOTActivatorOnBuild()
        {
            OnBuildUtil.RegisterType<LogicalNOTActivator>(ActivatorEditorUtil.ActivatorOnBuildBase, order: 0);
            OnBuildUtil.RegisterType<LogicalNOTActivator>(SecondOnBuild, order: 1);
        }

        private static bool SecondOnBuild(LogicalNOTActivator logicalNOTActivator)
        {
            ActivatorEditorUtil.AddActivatorToListeners(logicalNOTActivator.inputActivator, ListenerType.OnStateChanged, logicalNOTActivator);
            return true;
        }
    }
    #endif
}