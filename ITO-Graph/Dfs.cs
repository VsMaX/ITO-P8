using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace ITO_Graph
{
    public class Dfs
    {
        public Stack<Node> TraverseNodes { get; set; }

        public void Traverse()
        {
            while (TraverseNodes.Any())
            {
                Node node = TraverseNodes.Pop();
                foreach (var action in AvailableActions)
                {
                    if (action.FulFillsPreconditions(node.EffectsBefore))
                    {
                        var childNode = new Node();
                        childNode.Action = action;
                        childNode.EffectsAfter = action.GenerateEffectsAfter(node.EffectsBefore);
                        childNode.Parent = node;
                        node.Child = childNode;
                        TraverseNodes.Push(childNode);
                    }
                }
            }
        }

        public IEnumerable<StripAction> AvailableActions { get; set; }
    }

    public class StripAction
    {
        public int Id { get; }
        public List<Effect> Preconditions { get; set; }
        public List<Effect> Postconditions { get; set; }

        public StripAction(int id, List<Effect> preconditions, List<Effect> postconditions)
        {
            Id = id;
            Preconditions = preconditions;
            Postconditions = postconditions;
        }

        public bool FulFillsPreconditions(List<Effect> effects)
        {
            var valueMapping = new Dictionary<int, int>();
            var preconditionsStack = new Stack<Effect>();
            preconditionsStack.Push(null);
            Preconditions.ForEach(x => preconditionsStack.Push(x));

            return FulFillsPreconditionsCore(new Dictionary<int, int>(), effects, preconditionsStack);

            foreach (var preCondition in Preconditions)
            {
                var matchingEffects = effects.Where(
                    x =>
                        x.Parameter1.Type == preCondition.Parameter1.Type &&
                        x.Parameter2.Type == preCondition.Parameter2.Type &&
                        x.Parameter3.Type == preCondition.Parameter3.Type).ToList();

                for (int i = 0; i < matchingEffects.Count(); i++)
                {
                    var matchingEffect = matchingEffects[i];

                    if (!valueMapping.ContainsKey(preCondition.Parameter1.Value))
                        valueMapping[preCondition.Parameter1.Value] = matchingEffect.Parameter1.Value;
                    if (!valueMapping.ContainsKey(preCondition.Parameter2.Value))
                        valueMapping[preCondition.Parameter2.Value] = matchingEffect.Parameter2.Value;
                    if (!valueMapping.ContainsKey(preCondition.Parameter3.Value))
                        valueMapping[preCondition.Parameter3.Value] = matchingEffect.Parameter2.Value;

                    if (valueMapping[preCondition.Parameter1.Value] != matchingEffect.Parameter1.Value &&
                        valueMapping[preCondition.Parameter2.Value] != matchingEffect.Parameter2.Value &&
                        valueMapping[preCondition.Parameter3.Value] != matchingEffect.Parameter3.Value)
                    {

                    }
                }
            }
            throw new NotImplementedException();
        }

        private bool FulFillsPreconditionsCore(Dictionary<int, int> valueMapping, List<Effect> effectList, Stack<Effect> preconditionsStack)
        {
            var preconditionToFulFill = preconditionsStack.Pop();
            if (preconditionToFulFill == null)
                return true;

            var matchingEffects =
                effectList.Where(x => x.EffectType == preconditionToFulFill.EffectType).ToList();

            foreach (var matchingEffect in matchingEffects)
            {
                var valMapping = valueMapping.ToDictionary(x => x.Key, x => x.Value);
                if (!valMapping.ContainsKey(preconditionToFulFill.Parameter1.Value))
                {
                    valMapping.Add(preconditionToFulFill.Parameter1.Value, matchingEffect.Parameter1.Value);
                }
                else if (valMapping[preconditionToFulFill.Parameter1.Value] != matchingEffect.Parameter1.Value)
                {
                    continue;
                }
                if (!valMapping.ContainsKey(preconditionToFulFill.Parameter2.Value))
                {
                    valMapping.Add(preconditionToFulFill.Parameter2.Value, matchingEffect.Parameter2.Value);
                }
                else if (valMapping[preconditionToFulFill.Parameter2.Value] != matchingEffect.Parameter2.Value)
                {
                    continue;
                }
                if (!valueMapping.ContainsKey(preconditionToFulFill.Parameter3.Value))
                {
                    valueMapping.Add(preconditionToFulFill.Parameter3.Value, matchingEffect.Parameter2.Value);
                }
                else if (valMapping[preconditionToFulFill.Parameter3.Value] != matchingEffect.Parameter3.Value)
                {
                    continue;
                }

                if (valueMapping[preconditionToFulFill.Parameter1.Value] == matchingEffect.Parameter1.Value &&
                valueMapping[preconditionToFulFill.Parameter2.Value] == matchingEffect.Parameter2.Value &&
                valueMapping[preconditionToFulFill.Parameter3.Value] == matchingEffect.Parameter3.Value)
                {
                    FulFillsPreconditionsCore(valueMapping, effectList, preconditionsStack);
                }
            }

            return false;
        }

        public class ValueMapping
        {
            public Dictionary<int, int> Param1Mapping = new Dictionary<int, int>();
            public Dictionary<int, int> Param2Mapping = new Dictionary<int, int>();
            public Dictionary<int, int> Param3Mapping = new Dictionary<int, int>();
        }

        public List<Effect> GenerateEffectsAfter(List<Effect> currentEffects)
        {
            var resultEffects = new List<Effect>();
            var dict = new Dictionary<int, int>();

            foreach (var preCondition in Preconditions)
            {
                //na razie zakladamy ze tylko jeden efekt bedzie pasujacy
                var matchingEffects = currentEffects.Where(
                    x =>
                        x.Parameter1.Type == preCondition.Parameter1.Type &&
                        x.Parameter2.Type == preCondition.Parameter2.Type &&
                        x.Parameter3.Type == preCondition.Parameter3.Type).FirstOrDefault();

                if (!dict.ContainsKey(preCondition.Parameter1.Value))
                    dict[preCondition.Parameter1.Value] = matchingEffects.Parameter1.Value;
                if (!dict.ContainsKey(preCondition.Parameter2.Value))
                    dict[preCondition.Parameter2.Value] = matchingEffects.Parameter2.Value;
                if (!dict.ContainsKey(preCondition.Parameter3.Value))
                    dict[preCondition.Parameter3.Value] = matchingEffects.Parameter2.Value;
            }

            foreach (var postCondition in Postconditions)
            {
                resultEffects.Add(new Effect(postCondition.EffectType,
                    new EffectParameter(postCondition.Parameter1.Type, dict[postCondition.Parameter1.Value]),
                    new EffectParameter(postCondition.Parameter2.Type, dict[postCondition.Parameter2.Value]),
                    new EffectParameter(postCondition.Parameter3.Type, dict[postCondition.Parameter3.Value]))
                );
            }
            resultEffects.AddRange(currentEffects);

            return resultEffects;
        }
    }

    public class Effect
    {
        public int EffectType { get; }
        public EffectParameter Parameter1 { get; }
        public EffectParameter Parameter2 { get; }
        public EffectParameter Parameter3 { get; }

        public Effect(int effectType, EffectParameter param1, EffectParameter param2, EffectParameter param3)
        {
            EffectType = effectType;
            Parameter1 = param1;
            Parameter2 = param2;
            Parameter3 = param3;
        }
    }

    public class EffectParameter
    {
        public EffectParameter(int type, int paramValue)
        {
            Type = type;
            Value = paramValue;
        }

        public int Type { get; }
        public int Value { get; }
    }

    public class Node
    {
        public StripAction Action { get; set; }
        public Node Parent { get; set; }
        public Node Child { get; set; }
        public List<Effect> EffectsAfter { get; set; }
        public List<Effect> EffectsBefore { get; set; }
    }

    public class ParameterGroup
    {
        public int Type { get; }

        public List<Effect> Effects { get; set; }

        public ParameterGroup(int type)
        {
            Effects = new List<Effect>();
            this.Type = type;
        }

        public void AddEffect(Effect effect)
        {
            Effects.Add(effect);
        }
    }
}
