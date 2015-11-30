using System;
using System.Collections.Generic;
using ITO_Graph;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ITO_Test
{
    [TestClass]
    public class StripActionTest
    {
        [TestMethod]
        public void GenerateEffectsAfter_WhenNoPreAndPostConditions_RetunsSameEffects()
        {
            var effects = new List<Effect>();
            effects.Add(new Effect(1, new EffectParameter(1,1), new EffectParameter(1, 2), new EffectParameter(0, 0)));
            effects.Add(new Effect(1, new EffectParameter(1,1), new EffectParameter(1, 2), new EffectParameter(0, 0)));

            var action = new StripAction(1, new List<Effect>(), new List<Effect>());

            var result = action.GenerateEffectsAfter(effects);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result[0].EffectType);
            Assert.AreEqual(1, result[1].EffectType);
        }

        [TestMethod]
        public void GenerateEffectsAfter_WhenOnePreAndPostCondition_RetunsEffectsWithPostConditions()
        {
            var effects = new List<Effect>();
            effects.Add(new Effect(1, new EffectParameter(1, 1), new EffectParameter(2, 2), new EffectParameter(3, 1)));
            effects.Add(new Effect(2, new EffectParameter(4, 1), new EffectParameter(5, 2), new EffectParameter(6, 0)));

            var preconditions = new List<Effect>();
            preconditions.Add(new Effect(1, new EffectParameter(1,2), new EffectParameter(2, 1), new EffectParameter(3, 2)));

            var postConditions = new List<Effect>();
            postConditions.Add(new Effect(2, new EffectParameter(4, 1), new EffectParameter(5, 1), new EffectParameter(6, 2)));

            var action = new StripAction(1, new List<Effect>(), new List<Effect>());

            var result = action.GenerateEffectsAfter(effects);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result[0].EffectType);
            Assert.AreEqual(2, result[1].EffectType);
            Assert.AreEqual(1, result[1].Parameter1.Value);
            Assert.AreEqual(2, result[1].Parameter2.Value);
            Assert.AreEqual(0, result[1].Parameter3.Value);
        }
    }
}
