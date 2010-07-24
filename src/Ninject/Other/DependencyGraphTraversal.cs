using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ninject.Other
{
    /// <summary>
    /// 
    /// </summary>
    public class DependencyGraphTraversal
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="visitor"></param>
        static public void VisitGraph(IEnumerable<DependencyGraphNode> nodes, IPrintingDependencyGraphVisitor visitor)
        {
            Dictionary<DependencyGraphNode, int> dependencyTreeSize = new Dictionary<DependencyGraphNode, int>();
            Dictionary<DependencyGraphNode, int> countOfUsages = new Dictionary<DependencyGraphNode, int>();

            foreach (var node in nodes)
                countOfUsages[node] = 0;

            foreach (var node in nodes.OrderBy(n => n.GetBoundServices(nodes).Count()))
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

            foreach (var node in nodes.Where(n => countOfUsages[n] == 0).OrderBy(n => dependencyTreeSize[n]))
            {
                nodesToVisit.Push(new KeyValuePair<DependencyGraphNode, int>(node, 0));
            }

            while (nodesToVisit.Any())
            {
                var next = nodesToVisit.Pop();

                DependencyGraphNode node = next.Key;
                int depth = next.Value;

                var missingDependencies = node.GetMissingDependencies(nodes);

                visitor.BeginNodeVisit(node, depth, missingDependencies);

                foreach (var boundService in node.GetBoundServices(nodes))
                    nodesToVisit.Push(new KeyValuePair<DependencyGraphNode, int>(boundService, depth + 1));
            }
        }
    }
}
