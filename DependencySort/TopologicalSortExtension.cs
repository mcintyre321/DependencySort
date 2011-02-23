using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencySort
{
    public delegate IEnumerable<T> GetDependentTypes<T>(T item, IEnumerable<T> potentialDependencies);

    public static class TopologicalSortExtension
    {
        public static IEnumerable<Type> TopologicalSort(this IEnumerable<Type> items)
        {
            return items.TopologicalSort(DependencySort.TopologicalSort.Default);
        }

        public static IEnumerable<T> TopologicalSort<T>(this IEnumerable<T> items)
        {
            return items.TopologicalSort<T>(DependencySort.TopologicalSort.ByType<T>(t => t.GetType()));
        }


        public static IEnumerable<T> TopologicalSort<T>(this IEnumerable<T> items, GetDependentTypes<T> getDependentTypes)
        {
            var list = new List<Node<T>>();
            var nodes = items.Select(i => new Node<T>(i)).ToList();
            nodes.ForEach(n => Visit(n, nodes, list, getDependentTypes));
            return list.Select(n => n.Value);
        }

        public class Node<T>
        {
            public T Value { get; private set; }
            public bool Visited { get; set; }
            public Node(T value)
            {
                this.Value = value;
            }
        }

        private static void Visit<T>(Node<T> node, IEnumerable<Node<T>> nodes, IList<Node<T>> sorted, GetDependentTypes<T> getDependentItems)
        {
            if (node.Visited)
            {
                return;
            }
            node.Visited = true;
            var dependentItems = getDependentItems(node.Value, nodes.Select(l => l.Value));
            var dependentNodes = from i in dependentItems join n in nodes on i equals n.Value select n;

            foreach (Node<T> dependency in dependentNodes)
            {
                Visit(dependency, nodes, sorted, getDependentItems);
            }
            sorted.Insert(0, node);
            return;
        }
    }
}