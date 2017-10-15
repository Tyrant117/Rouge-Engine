
namespace Rougelikeberry.Pathing
{
    /// <summary>
    /// The DirectedEdge class represents a weighted edge in an edge-weighted directed graph. 
    /// </summary>
    public class DirectedEdge
    {
        /// <summary>
        /// Returns the destination vertex of the DirectedEdge
        /// </summary>
        public int From { get; private set; }
        /// <summary>
        /// Returns the start vertex of the DirectedEdge
        /// </summary>
        public int To { get; private set; }
        /// <summary>
        /// Returns the weight of the DirectedEdge
        /// </summary>
        public double Weight { get; private set; }

        /// <summary>
        /// Constructs a directed edge from one specified vertex to another with the given weight
        /// </summary>
        /// <param name="from">The start vertex</param>
        /// <param name="to">The destination vertex</param>
        /// <param name="weight">The weight of the DirectedEdge</param>
        public DirectedEdge(int from, int to, double weight)
        {
            From = from;
            To = to;
            Weight = weight;
        }

        /// <summary>
        /// Returns a string that represents the current DirectedEdge
        /// </summary>
        public override string ToString()
        {
            return string.Format("From: {0}, To: {1}, Weight: {2}", From, To, Weight);
        }
    }
}