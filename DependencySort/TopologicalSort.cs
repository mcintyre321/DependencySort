using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencySort
{
    public class TopologicalSort
    {
        public static IEnumerable<T> PerformTopoSort<T>(IEnumerable<T> items, GetDependentTypes<T> getDependentTypes)
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

        public static GetDependentTypes<Type> Default
        {
            get
            {
                return ByType<Type>(type => type);
            }
        }

        public static GetDependentTypes<T> ByType<T>()
        {
            return ByType<T>(t => t.GetType());
        }

        public static GetDependentTypes<T> ByType<T>(Func<T, Type> getTypeFromT)
        {
            return (t, potentialTypes) =>
                   potentialTypes.Where(
                       pt => typeof(IRunLast).IsAssignableFrom(getTypeFromT(pt)) || typeof(IRunAfter<>).MakeGenericType(getTypeFromT(t)).IsAssignableFrom(getTypeFromT(pt))
                       );
        }

        
    }
}