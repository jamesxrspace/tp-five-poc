﻿
// <auto-generated />

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TPFive.Cross
{
    /// <summary>
    /// This is from TPFive.Game.SceneFlow Service.
    /// </summary>
    public sealed partial class Bridge
    {

        public delegate IEnumerator LoadSceneDelegate(
            string name,

            LoadSceneMode loadSceneMode,

            int categoryOrder,

            int subOrder,

            UnityEngine.MonoBehaviour lifetimeScope);

        /// <summary>
        /// Define handler for LoadScene.
        /// </summary>
        public static LoadSceneDelegate LoadScene;

        public delegate IEnumerator UnloadSceneDelegate(
            string name,

            int categoryOrder,

            int subOrder);

        /// <summary>
        /// Define handler for UnloadScene.
        /// </summary>
        public static UnloadSceneDelegate UnloadScene;

        public delegate void ChangeSceneDelegate(
            string fromCategory,

            string fromTitle,

            int fromCategoryOrder,

            int fromSuborder,

            string toCategory,

            string toTitle,

            int toCategoryOrder,

            int toSuborder,

            MonoBehaviour lifeTimeScope);

        /// <summary>
        /// Define handler for ChangeScene.
        /// </summary>
        public static ChangeSceneDelegate ChangeScene;

    }
}