﻿// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Common;
// ReSharper disable PartialTypeWithSinglePart

namespace Doozy.Runtime.Bindy
{
    [Serializable]
    public partial class BindId : CategoryNameId
    {
        public BindId() {}
        public BindId(string category, string name, bool custom = false) : base(category, name, custom) {}
    }
}
