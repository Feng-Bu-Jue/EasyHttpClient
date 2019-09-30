﻿using EasyHttpClient.ActionFilters;
using EasyHttpClient.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient
{

    public class ParameterDescription
    {
        internal ParameterDescription()
        {
        }

        public int Order { get; set; }
        public ParameterInfo ParameterInfo { get; internal set; }
        public IParameterScopeAttribute[] ScopeAttributes { get; internal set; }
    }

}
