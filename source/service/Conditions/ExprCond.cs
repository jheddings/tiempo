//=============================================================================
// Copyright © 2006 Heddway, All Rights Reserved
// $Id: ExprCond.cs 40 2010-11-26 22:08:16Z jheddings $
//=============================================================================
using System;

namespace Tiempo.Service.Conditions {
    internal class ExprCond : Condition {

        public override DateTime NextTime {
            get { return DateTime.MaxValue; }
        }
    }
}