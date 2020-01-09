﻿using System.Collections.Generic;
using LambdaCalculusParser.Visitors;

namespace LambdaCalculusParser.Test
{
    class ReductionEventCollector
    {
        public List<string> Events { get; } = new List<string>();

        public void OnAlphaReductionEvent(object sender, AlphaReductionEventArgs e)
        {
            Events.Add("α");
        }

        public void OnBetaReductionEvent(object sender, BetaReductionEventArgs e)
        {
            Events.Add("β");
        }

        public void OnEtaReductionEvent(object sender, EtaReductionEventArgs e)
        {
            Events.Add("η");
        }
    }
}