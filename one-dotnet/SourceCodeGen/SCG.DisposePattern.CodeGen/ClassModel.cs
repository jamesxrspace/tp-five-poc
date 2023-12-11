﻿using System.Reflection;

namespace TPFive.SCG.DisposePattern.CodeGen
{
    internal record ClassModel(
        string Namespace,
        string Name,
        string Modifier,
        PropertyData[] Properties)
    {
        public string Version => Assembly
            .GetExecutingAssembly().GetName().Version.ToString(3);
    }
}
