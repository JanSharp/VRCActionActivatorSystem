﻿using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
#if UNITY_EDITOR && !COMPILER_UDONSHARP
using UnityEditor;
using UdonSharpEditor;
using System.Linq;
#endif

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class MovementAction : ConfigurableActionBase
    {
        [SerializeField] private ObjectPositionSync targetObject;
        [SerializeField] private Vector3 movementOnEvent;

        public void OnEvent()
        {
            targetObject.TargetPosition += movementOnEvent;
        }
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [InitializeOnLoad]
    public static class MovementActionOnBuild
    {
        static MovementActionOnBuild() => OnBuildUtil.RegisterType<MovementAction>(ActivatorEditorUtil.BasicActionOnBuild, order: 1);
    }
    #endif
}
