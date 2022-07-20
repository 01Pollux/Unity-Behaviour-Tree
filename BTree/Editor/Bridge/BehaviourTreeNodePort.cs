using System.Collections.Generic;

using UnityEditor.Experimental.GraphView;

using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Plugins.BTree.Editor.Bridge
{
    public class BehaviourTreeNodePort : Port
    {
        // https://github.com/Unity-Technologies/UnityCsReference/blob/master/Modules/GraphViewEditor/Elements/Port.cs
        public BehaviourTreeNodePort(Direction portDirection, Capacity portCapacity) : 
            base(Orientation.Vertical, portDirection, portCapacity, typeof(bool))
        {
            var listener = new DefaultEdgeConnectorListener();
            m_EdgeConnector = new EdgeConnector<Edge>(listener);
            this.AddManipulator(m_EdgeConnector);

            style.width = 100;
            if (direction == Direction.Input)
            {
                style.marginTop = 8f;
                style.marginBottom = -8f;
                style.flexDirection = FlexDirection.Column;
            }
            else
            {
                style.marginTop = -8f;
                style.marginBottom = 8f;
                style.flexDirection = FlexDirection.ColumnReverse;
            }
        }

        private class DefaultEdgeConnectorListener : IEdgeConnectorListener
        {
            private GraphViewChange m_GraphViewChange;
            private List<Edge> m_EdgesToCreate;
            private List<GraphElement> m_EdgesToDelete;

            public DefaultEdgeConnectorListener()
            {
                m_EdgesToCreate = new List<Edge>();
                m_EdgesToDelete = new List<GraphElement>();

                m_GraphViewChange.edgesToCreate = m_EdgesToCreate;
            }

            public void OnDropOutsidePort(Edge edge, Vector2 position) { }
            public void OnDrop(GraphView graphView, Edge edge)
            {
                m_EdgesToCreate.Clear();
                m_EdgesToCreate.Add(edge);

                // We can't just add these edges to delete to the m_GraphViewChange
                // because we want the proper deletion code in GraphView to also
                // be called. Of course, that code (in DeleteElements) also
                // sends a GraphViewChange.
                m_EdgesToDelete.Clear();
                if (edge.input.capacity == Capacity.Single)
                    foreach (Edge edgeToDelete in edge.input.connections)
                        if (edgeToDelete != edge)
                            m_EdgesToDelete.Add(edgeToDelete);
                if (edge.output.capacity == Capacity.Single)
                    foreach (Edge edgeToDelete in edge.output.connections)
                        if (edgeToDelete != edge)
                            m_EdgesToDelete.Add(edgeToDelete);
                if (m_EdgesToDelete.Count > 0)
                    graphView.DeleteElements(m_EdgesToDelete);

                var edgesToCreate = m_EdgesToCreate;
                if (graphView.graphViewChanged != null)
                {
                    edgesToCreate = graphView.graphViewChanged(m_GraphViewChange).edgesToCreate;
                }

                foreach (Edge e in edgesToCreate)
                {
                    graphView.AddElement(e);
                    edge.input.Connect(e);
                    edge.output.Connect(e);
                }
            }
        }

        public override bool ContainsPoint(Vector2 localPoint)
        {
            Rect rect = new(0, 0, layout.width, layout.height);
            return rect.Contains(localPoint);
        }
    }
}
