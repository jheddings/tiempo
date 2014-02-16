//=============================================================================
// Copyright © 2009 Heddway, All Rights Reserved
// $Id: SpecCond.cs 46 2011-01-24 03:43:14Z jheddings $
//=============================================================================
using System;

namespace Tiempo.Service.Conditions {
    internal abstract class SpecCond : Condition {

        public int Offset { get; set; }
        public abstract DateTime SpecTime { get; }

        ///////////////////////////////////////////////////////////////////////
        public sealed override DateTime NextTime {
            get {
                DateTime next = this.SpecTime;
                if ((next < DateTime.MaxValue) && (next > DateTime.MinValue)) {
                    next = next.AddMinutes(this.Offset);
                }

                return next;
            }
        }
    }
}