using System;
using System.IO;
using System.Linq;
using Ninject.Infrastructure.Introspection;

namespace Ninject.Other
{
    public interface IPrintingDependencyGraphVisitor
    {
        void BeginNodeVisit(DependencyGraphNode node, int i);
    }

    public class PrintingDependencyGraphVisitor : IPrintingDependencyGraphVisitor
    {
        private readonly TextWriter _writer;

        public PrintingDependencyGraphVisitor(TextWriter writer)
        {
            _writer = writer;
        }

        public void BeginNodeVisit(DependencyGraphNode node, int i)
        {
            _writer.Write(String.Concat(Enumerable.Range(0, i).Select(_ => "    ").ToArray()));

            if (node.ImplementationName == null
                || node.ImplementationName == node.InterfaceType.Format())
            {
                _writer.WriteLine("{0}", node.InterfaceType.Name);
            } else
            {
                _writer.WriteLine("{0} ({1})", node.ImplementationName, node.InterfaceType.Format());
            }
        }
    }
}