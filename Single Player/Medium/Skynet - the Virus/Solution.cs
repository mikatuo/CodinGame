using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

class Player
{
	#region Private Methods
	/// <summary>
	/// Reads inputs.
	/// </summary>
	/// <param name="nodesCount">Total amount of nodes in the skynet.</param>
	/// <param name="links">Links between nodes.</param>
	/// <param name="exits">Exit gateways.</param>
	static void ReadInput(out int nodesCount, out List<NodeLink> links, out List<int> exits)
	{
		links = new List<NodeLink>();
		exits = new List<int>();
		// read inputs.
		string[] inputs;
		inputs = Console.ReadLine().Split( ' ' );
		nodesCount = int.Parse( inputs[ 0 ] );
		var linksCount = int.Parse( inputs[ 1 ] ); // the number of links
		var exitsCount = int.Parse( inputs[ 2 ] ); // the number of exit gateways
		for ( int i = 0; i < linksCount; i++ ) {
			inputs = Console.ReadLine().Split( ' ' );
			var node1 = int.Parse( inputs[ 0 ] ); // node1 and node2 defines a link between these nodes
			var node2 = int.Parse( inputs[ 1 ] );
			links.Add( new NodeLink( node1, node2 ) );
		}
		for ( int i = 0; i < exitsCount; i++ ) {
			var exit = int.Parse( Console.ReadLine() ); // the index of a gateway node
			exits.Add( exit );
		}
	}
	#endregion

	/// <summary>
	/// Program entry point.
	/// </summary>
	/// <param name="args">Command line arguments.</param>
	static void Main(string[] args)
	{
		// total amount of nodes in the skynet.
		int nodesCount;
		// links between nodes.
		List<NodeLink> links;
		// exit gateways.
		List<int> exits;
		// reads input data.
		ReadInput( out nodesCount, out links, out exits );

		// create the skynet.
		var skynet = new Skynet( nodes: nodesCount, exits: exits );
		foreach ( NodeLink link in links )
			skynet.AddLink( link );
		// infest the skynet with the virus.
		var virus = new Virus( skynet );
		virus.SevereLink += (sender, e) => {
			Console.WriteLine( "{0} {1}", e.Link.NodeA, e.Link.NodeB );
		};

		// game loop.
		while ( true ) {
			var agentPosition = int.Parse( Console.ReadLine() ); // The index of the node on which the Skynet agent is positioned this turn
			// update agent's position. Because the virus has control over the skynet it 
			// will track current position of the agent and respond by severing links to 
			// block agent from reaching an exit gateway.
			skynet.SetAgentPosition( agentPosition );
		}
	}
}

/// <summary>
/// The virus.
/// </summary>
public class Virus
{
	#region Local Variables
	/// <summary>
	/// The skynet that is under control of the virus.
	/// </summary>
	readonly IControlledSkynet _skynet;
	/// <summary>
	/// Collection of links that have been severed.
	/// </summary>
	readonly HashSet<NodeLink> _severedLinks;
	#endregion

	/// <summary>
	/// Indicates that the virus is severing a link.
	/// </summary>
	public event EventHandler<SevereLinkEventArgs> SevereLink;
	
	/// <summary>
	/// Initializes the virus.
	/// </summary>
	/// <param name="skynet">The skynet to take control over.</param>
	public Virus(IControlledSkynet skynet)
	{
		// take control over the skynet.
		_skynet			= skynet;
		_severedLinks	= new HashSet<NodeLink>();
		_skynet.NewAgentPosition += ChooseLinkToSevere;
	}

	/// <summary>
	/// Raises the <see cref="SevereLink"/> event.
	/// </summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void OnSevereLink(SevereLinkEventArgs e)
	{
		_severedLinks.Add( e.Link );
		EventHandler<SevereLinkEventArgs> handler = SevereLink;
		if ( handler != null ) handler( this, e );
	}

	#region Private Event Handlers
	/// <summary>
	/// Chooses a link to severe.
	/// </summary>
	/// <param name="sender">Skynet.</param>
	/// <param name="e">Event arguments.</param>
	void ChooseLinkToSevere(object sender, NewAgentPositionEventArgs e)
	{
		// get all links of exit gateway(s).
		SkynetNode[] exits = _skynet.Exits.ToArray();
		// search for a link with a node where an agent is placed.
		NodeLink? firstPriorityLink		= null;
		NodeLink? secondPriorityLink	= null;
		NodeLink curr;
		for ( int i = 0; i < exits.Length; i++ ) {
			for ( int j = 0; j < exits[ i ].Neighbors.Count; j++ ) {
				// check links that lead to nodes close to the exit.
				curr = new NodeLink(
					nodeA:	exits[ i ].Value.ID,
					nodeB:	exits[ i ].Neighbors[ j ].Value.ID
					);
				// skip already severed links.
				if ( _severedLinks.Contains( curr ) )
					continue;
				// if the agent is on this neighbor node then this  
				// links is the first priority.
				if ( e.Position == exits[ i ].Neighbors[ j ].Value.ID ) {
					firstPriorityLink = curr;
					break;
				}
				// second priority is any link that was not severed yet.
				if ( null == secondPriorityLink )
					secondPriorityLink = curr;
			}
		}
		// severe the link.
		if ( firstPriorityLink.HasValue ) {
			OnSevereLink( new SevereLinkEventArgs( firstPriorityLink.Value ) );
		} else if ( secondPriorityLink.HasValue ) {
			OnSevereLink( new SevereLinkEventArgs( secondPriorityLink.Value ) );
		}
	}
	#endregion
}

/// <summary>
/// A node of the graph that can store some data.
/// </summary>
/// <typeparam name="T">Type of the data the node stores.</typeparam>
public class Node<T>
{
	#region Properties
	/// <summary>
	/// The value stored in the node.
	/// </summary>
	public T Value
	{
		get;
		set;
	}
	/// <summary>
	/// Neighbors of the node.
	/// </summary>
	public NodeList<T> Neighbors
	{
		get;
		set;
	}
	#endregion

	#region Constructors
	/// <summary>
	/// Initializes the node.
	/// </summary>
	/// <param name="value">The value to store in the node.</param>
	public Node(T value)
		: this( value, neighbors: new NodeList<T>() )
	{
	}
	/// <summary>
	/// Initializes the node.
	/// </summary>
	/// <param name="value">The value to store in the node.</param>
	/// <param name="neighbors">Neighbors of the node.</param>
	public Node(T value, NodeList<T> neighbors)
	{
		this.Value		= value;
		this.Neighbors	= neighbors;
	}
	#endregion
}

/// <summary>
/// List of nodes.
/// </summary>
/// <typeparam name="T">Type of the data each node in the collection stores.</typeparam>
public class NodeList<T>
	: Collection<Node<T>>
{
	/// <summary>
	/// Finds a node by the condition.
	/// </summary>
	/// <param name="condition">The condition to check for a value stored inside a node.</param>
	/// <returns>Either node which value matches the condition or <b>NULL</b>.</returns>
	public Node<T> FindByValue(Func<T, bool> condition)
	{
		// search the list for the value.
		foreach ( Node<T> node in base.Items )
			if ( condition( node.Value ) )
				return	node;

		// if we reached here, we didn't find a matching node.
		return	null;
	}
}

/// <summary>
/// A node of a skynet.
/// </summary>
public class SkynetNode
	: Node<SkynetNodeData>
{
	#region Constructors
	/// <summary>
	/// Initializes the node.
	/// </summary>
	/// <param name="value">The value to store in the node.</param>
	public SkynetNode(SkynetNodeData value)
		: base( value )
	{
	}
	/// <summary>
	/// Initializes the node.
	/// </summary>
	/// <param name="id">ID of the node.</param>
	/// <param name="type">Type of the node.</param>
	public SkynetNode(int id, SkynetNodeType type)
		: base( new SkynetNodeData( id, type ) )
	{
	}
	#endregion
}

/// <summary>
/// Data of a skynet node.
/// </summary>
public class SkynetNodeData
{
	/// <summary>
	/// ID of the node.
	/// </summary>
	public int ID
	{
		get;
		private set;
	}
	/// <summary>
	/// Type of the skynet node.
	/// </summary>
	public SkynetNodeType Type
	{
		get;
		private set;
	}

	/// <summary>
	/// Initializes the data.
	/// </summary>
	/// <param name="id">ID of the node.</param>
	/// <param name="type">Type of the node.</param>
	public SkynetNodeData(int id, SkynetNodeType type)
	{
		this.ID		= id;
		this.Type	= type;
	}
}

/// <summary>
/// Type of the skynet node.
/// </summary>
public enum SkynetNodeType
{
	/// <summary>
	/// The node is a general node.
	/// </summary>
	General = 0,
	/// <summary>
	/// The node is an exit node of the skynet.
	/// </summary>
	ExitGateway,
}

/// <summary>
/// A link between two nodes.
/// </summary>
public struct NodeLink
{
	#region Local Variables & Properties
	/// <summary>
	/// A linked node one.
	/// </summary>
	readonly int _nodeA;
	/// <summary>
	/// A linked node two.
	/// </summary>
	readonly int _nodeB;

	/// <summary>
	/// A linked node one.
	/// </summary>
	public int NodeA
	{
		get { return _nodeA; }
	}
	/// <summary>
	/// A linked node two.
	/// </summary>
	public int NodeB
	{
		get { return _nodeB; }
	}
	#endregion

	/// <summary>
	/// Initializes the link between nodes.
	/// </summary>
	/// <param name="nodeA">A linked node one.</param>
	/// <param name="nodeB">A linked node two.</param>
	public NodeLink(int nodeA, int nodeB)
	{
		_nodeA = nodeA;
		_nodeB = nodeB;
	}

	#region Equality
	/// <summary>
	/// Checks if two links are equal.
	/// </summary>
	/// <param name="obj">Object to compare with.</param>
	/// <returns><b>True</b> if they are equal; otherwise, <b>false</b>.</returns>
	public override bool Equals(object obj)
	{
		if ( !(obj is NodeLink) )
			return	false;
		var other = (NodeLink)obj;
		return	this.NodeA == other.NodeA
					&& this.NodeB == other.NodeB;
	}
	/// <summary>
	/// Gets hash code of the link.
	/// </summary>
	/// <returns>Hash.</returns>
	public override int GetHashCode()
	{
		return	this.NodeA.GetHashCode()
					^ this.NodeB.GetHashCode();
	}
	#endregion
}

/// <summary>
/// A skynet.
/// </summary>
public class Skynet
	: IControlledSkynet
{
	#region Local Variables
	/// <summary>
	/// Nodes of the skynet.
	/// </summary>
	readonly Dictionary<int, SkynetNode> _nodes = new Dictionary<int, SkynetNode>();
	/// <summary>
	/// Cache already linked nodes.
	/// </summary>
	readonly HashSet<NodeLink> _links = new HashSet<NodeLink>();
	/// <summary>
	/// ID of the node the agent is positioned at.
	/// </summary>
	int _agentPosition;
	/// <summary>
	/// Exit gateways.
	/// </summary>
	readonly HashSet<SkynetNode> _exits = new HashSet<SkynetNode>();
	#endregion

	/// <summary>
	/// Gets node of the skynet by it's index.
	/// </summary>
	/// <param name="idx">The index of the skynet.</param>
	/// <returns>Node.</returns>
	public SkynetNode this[ int idx ]
	{
		get { return _nodes[ idx ]; }
	}

	/// <summary>
	/// Exit gateways.
	/// </summary>
	public HashSet<SkynetNode> Exits
	{
		get { return _exits; }
	}

	/// <summary>
	/// Indicates that the agent's position has changed.
	/// </summary>
	public event EventHandler<NewAgentPositionEventArgs> NewAgentPosition;

	#region Constructors
	/// <summary>
	/// Initializes the skynet.
	/// </summary>
	/// <param name="nodes">Amount of nodes. Each node will have it's own identifier that starts with 1.</param>
	/// <param name="exits">Identifier of exit gateways available in the skynet.</param>
	public Skynet(int nodes, IList<int> exits)
	{
		var exitIndices = new HashSet<int>( exits );
		// create nodes.
		for ( int nodeId = 0; nodeId < nodes; nodeId++ ) {
			// add node to the net.
			SkynetNodeType type = exitIndices.Contains( nodeId )
									? SkynetNodeType.ExitGateway
									: SkynetNodeType.General;
			_nodes.Add(
				key:	nodeId,
				value:	new SkynetNode(
							id:		nodeId,
							type:	type
						)
				);
			// memo exit gateways.
			if ( type == SkynetNodeType.ExitGateway )
				this.Exits.Add( _nodes[ nodeId ] );
		}
	}
	#endregion

	/// <summary>
	/// Raises the <see cref="NewAgentPosition"/> event.
	/// </summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void OnNewAgentPosition(NewAgentPositionEventArgs e)
	{
		EventHandler<NewAgentPositionEventArgs> handler = NewAgentPosition;
		if ( handler != null ) handler( this, e );
	}

	/// <summary>
	/// Links two nodes with each other.
	/// </summary>
	/// <param name="link"></param>
	public void AddLink(NodeLink link)
	{
		if ( _links.Contains( link ) )
			// don't add duplicates.
			return;
		// link two nodes.
		SkynetNode nodeA = _nodes[ link.NodeA ];
		SkynetNode nodeB = _nodes[ link.NodeB ];
		nodeA.Neighbors.Add( nodeB );
		nodeB.Neighbors.Add( nodeA );
	}
	/// <summary>
	/// Sets agent's current position.
	/// </summary>
	/// <param name="agentPosition">ID of the node the agent is positioned at.</param>
	public void SetAgentPosition(int agentPosition)
	{
		_agentPosition = agentPosition;
		// raise the event that indicates that agent's position has changed.
		OnNewAgentPosition( new NewAgentPositionEventArgs( agentPosition ) );
	}
}

/// <summary>
/// Part of a skynet that is controlled by a virus.
/// </summary>
public interface IControlledSkynet
{
	/// <summary>
	/// Gets node of the skynet by it's index.
	/// </summary>
	/// <param name="idx">The index of the skynet.</param>
	/// <returns>Node.</returns>
	SkynetNode this[ int idx ]
	{
		get;
	}

	/// <summary>
	/// Exit gateways.
	/// </summary>
	HashSet<SkynetNode> Exits
	{
		get;
	}

	/// <summary>
	/// Indicates that the agent's position has changed.
	/// </summary>
	event EventHandler<NewAgentPositionEventArgs> NewAgentPosition;
}

/// <summary>
/// Arguments for an event that indicates that the agent's position has changed.
/// </summary>
public class NewAgentPositionEventArgs
	: EventArgs
{
	/// <summary>
	/// The new position of the agent.
	/// </summary>
	public int Position
	{
		get;
		private set;
	}

	/// <summary>
	/// Initializes the event arguments.
	/// </summary>
	/// <param name="position">The new position of the agent.</param>
	public NewAgentPositionEventArgs(int position)
	{
		this.Position = position;
	}
}

/// <summary>
/// Arguments for an event that indicates that a link is being severed.
/// </summary>
public class SevereLinkEventArgs
	: EventArgs
{
	/// <summary>
	/// A link that is being seveared.
	/// </summary>
	public NodeLink Link
	{
		get;
		private set;
	}

	/// <summary>
	/// Initializes the event arguments.
	/// </summary>
	/// <param name="nodeA">A linked node one.</param>
	/// <param name="nodeB">A linked node two.</param>
	public SevereLinkEventArgs(int nodeA, int nodeB)
		: this( new NodeLink( nodeA, nodeB ) )
	{
	}
	/// <summary>
	/// Initializes the event arguments.
	/// </summary>
	/// <param name="link">A link between two nodes.</param>
	public SevereLinkEventArgs(NodeLink link)
	{
		this.Link = link;
	}
}