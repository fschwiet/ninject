using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ninject.Other
{
    public class DependencyGraphNode
    {
        public Type InterfaceType;
        public string ImplementationName;
        public IEnumerable<Type> Dependencies;

        public bool SatisfiesDependencyOn(Type type)
        {
            return type == InterfaceType
                || IsEnumerableOfInterfaceType(type)
                || IsFactoryOfInterfaceType(type);
        }

        private bool IsEnumerableOfInterfaceType(Type type)
        {
            if (type.IsArray)
            {
                Type service = type.GetElementType();
                return InterfaceType == service;
            }

            if (type.IsGenericType)
            {
                Type gtd = type.GetGenericTypeDefinition();
                Type service = type.GetGenericArguments()[0];

                if (gtd == typeof(List<>) || gtd == typeof(IList<>) || gtd == typeof(ICollection<>) || gtd == typeof(IEnumerable<>))
                {
                    return InterfaceType == service;
                }
            }

            return false;
        }

        private bool IsFactoryOfInterfaceType(Type type)
        {
            if (type.IsGenericType)
            {
                Type gtd = type.GetGenericTypeDefinition();
                Type service = type.GetGenericArguments()[0];

                if (gtd == typeof(Func<>))
                    return InterfaceType == service;
            }

            return false;
        }

        public IEnumerable<DependencyGraphNode> GetBoundServices(IEnumerable<DependencyGraphNode> allNodes)
        {
            foreach (var dependency in Dependencies)
            {
                foreach (var node in allNodes)
                {
                    if (node.SatisfiesDependencyOn(dependency))
                        yield return node;
                }
            }
        }

        public IEnumerable<Type> GetMissingDependencies(IEnumerable<DependencyGraphNode> allNodes)
        {
            foreach (var dependency in Dependencies)
            {
                if (!allNodes.Where(n => n.SatisfiesDependencyOn(dependency)).Any())
                    yield return dependency;
            }
        }
    }
}
