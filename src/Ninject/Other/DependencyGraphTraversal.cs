using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ninject.Other
{
    public class DependencyGraphTraversal
    {
        static public void VisitGraph(IEnumerable<DependencyGraphNode> nodes, IPrintingDependencyGraphVisitor visitor)
        {
            Dictionary<DependencyGraphNode, int> dependencyTreeSize = new Dictionary<DependencyGraphNode, int>();
            Dictionary<DependencyGraphNode, int> countOfUsages = new Dictionary<DependencyGraphNode, int>();

            foreach (var node in nodes)
                countOfUsages[node] = 0;

            foreach (var node in nodes.OrderBy(n => n.Dependencies.Count()))
            {
                int nodeWeight = 0;

                foreach (var boundService in node.GetBoundServices(nodes))
                {
                    if (!dependencyTreeSize.ContainsKey(boundService))
                        nodeWeight += 1;  // depedendency exists, but could not be bound
                    else
                        nodeWeight += dependencyTreeSize[boundService];

                    countOfUsages[boundService] += 1;
                }

                dependencyTreeSize[node] = nodeWeight;
            }

            var nodesToVisit = new Stack<KeyValuePair<DependencyGraphNode, int>>();

            foreach (var node in nodes.Where(n => countOfUsages[n] == 0).OrderByDescending(n => dependencyTreeSize[n]))
            {
                nodesToVisit.Push(new KeyValuePair<DependencyGraphNode, int>(node, 0));
            }

            while (nodesToVisit.Any())
            {
                var next = nodesToVisit.Pop();

                visitor.BeginNodeVisit(next.Key, next.Value);

                foreach (var boundService in next.Key.GetBoundServices(nodes))
                    nodesToVisit.Push(new KeyValuePair<DependencyGraphNode, int>(boundService, next.Value + 1));
            }
        }
    }
}
