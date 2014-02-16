//=============================================================================
// Copyright © 2009 Heddway, All Rights Reserved
// $Id: Condition.cs 46 2011-01-24 03:43:14Z jheddings $
//=============================================================================
using System;
using System.Xml;
using Tiempo.Service.Conditions;

namespace Tiempo.Service {
    internal abstract class Condition {

        ///////////////////////////////////////////////////////////////////////
        public abstract DateTime NextTime { get; }

        ///////////////////////////////////////////////////////////////////////
        public static Condition Load(XmlNode node) {
            XmlNode child = node.FirstChild;

            switch (child.Name) {
                case "sunset":
                case "sunrise":
                    return SunSpec.Load(child);

                case "time":
                    return TimeCond.Load(child);
            }

            return null;
        }
    }
}