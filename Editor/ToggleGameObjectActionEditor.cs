﻿using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEditor;
using UdonSharpEditor;
using System.Linq;

namespace JanSharp
{
    [InitializeOnLoad]
    public static class ToggleGameObjectActionOnBuild
    {
        static ToggleGameObjectActionOnBuild() => OnBuildUtil.RegisterType<ToggleGameObjectAction>(OnBuild, order: 1);

        private static bool OnBuild(ToggleGameObjectAction toggleGameObjectAction)
        {
            ActivatorEditorUtil.AddActivatorToListeners(toggleGameObjectAction.activator, ListenerType.OnStateChanged, toggleGameObjectAction);
            return true;
        }
    }
}
