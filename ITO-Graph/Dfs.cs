using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                        TraverseNodes.Push();
                    }
                }
            }
        }

        public IEnumerable<StripAction> AvailableActions { get; set; }
    }
}
