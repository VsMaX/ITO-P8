using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITO_Graph
{
    public class Node
    {
        private StripAction Action { get; set; }
        public Node Parent { get; set; }
        public Node Child { get; set; }
        public List<StripEffect> StateAfter { get; set; }

        public bool StateEquals(Node goal)
        {
            bool stateEquals = true;
            foreach (var goalState in goal.StateAfter)
            {
                if (!this.StateAfter.Any(x => x.EffectId == goalState.EffectId && x.EffectValueId == goalState.EffectValueId))
                {
                    stateEquals = false;
                    break;
                }
            }
            return stateEquals;
        }

        public void SetAction(StripAction action)
        {
            this.Action = action;
            this.StateAfter = action.GenerateStateAfter(StateBefore);
        }

        public List<StripEffect> StateBefore { get; set; }
    }
}
