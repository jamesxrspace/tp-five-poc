﻿
// <auto-generated />

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TPFive.Cross
{
    /// <summary>
    /// This is from TPFive.Game.Logging Service.
    /// </summary>
    public sealed partial class Bridge
    {

        public delegate void LoggingDelegate(
            System.Type t,

            int level,

            object message);

        /// <summary>
        /// Define handler for Logging.
        /// </summary>
        public static LoggingDelegate Logging;

    }
}