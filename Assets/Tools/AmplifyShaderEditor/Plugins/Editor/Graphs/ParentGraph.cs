// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class ParentGraph : ISerializationCallbackReceiver
	{
		public enum NodeLOD
		{
			LOD0,
			LOD1,
			LOD2,
			LOD3,
			LOD4
		}

		private NodeLOD m_lodLevel = NodeLOD.LOD0;
		private GUIStyle nodeStyleOff;
		private GUIStyle nodeStyleOn;
		private GUIStyle nodeTitle;
		private GUIStyle commentaryBackground;

		public delegate void EmptyGraphDetected( ParentGraph graph );
		public event EmptyGraphDetected OnEmptyGraphDetectedEvt;

		public delegate void NodeEvent( ParentNode node );
		public event NodeEvent OnNodeEvent;

		public event MasterNode.OnMaterialUpdated OnMaterialUpdatedEvent;
		public event MasterNode.OnMaterialUpdated OnShaderUpdatedEvent;

		private bool m_afterDeserializeFlag = true;
		[SerializeField]
		private int m_validNodeId;

		[SerializeField]
		private List<ParentNode> m_nodes;

		// Sampler Nodes registry
		[SerializeField]
		private NodeUsageRegister m_samplerNodes;

		[SerializeField]
		private NodeUsageRegister m_texturePropertyNodes;

		[SerializeField]
		private NodeUsageRegister m_textureArrayNodes;

		// Screen Color Nodes registry
		[SerializeField]
		private NodeUsageRegister m_screenColorNodes;

		[SerializeField]
		private NodeUsageRegister m_localVarNodes;

		[SerializeField]
		private NodeUsageRegister m_propertyNodes;


		[SerializeField]
		private int m_masterNodeId = Constants.INVALID_NODE_ID;

		[SerializeField]
		private bool m_isDirty;

		[SerializeField]
		private bool m_saveIsDirty = false;

		[SerializeField]
		private int m_nodeClicked;

		[SerializeField]
		private int m_loadedShaderVersion;

		[SerializeField]
		private int m_instancePropertyCount = 0;

		[SerializeField]
		private int m_virtualTextureCount = 0;

		[SerializeField]
		private PrecisionType m_currentPrecision = PrecisionType.Float;

		private List<ParentNode> m_visibleNodes = new List<ParentNode>();

		private List<ParentNode> m_nodePreviewList = new List<ParentNode>();

		private Dictionary<int, ParentNode> m_nodesDict;
		private List<ParentNode> m_selectedNodes;
		private List<ParentNode> m_markedForDeletion;
		private List<WireReference> m_highlightedWires;
		private Type m_masterNodeDefaultType;

		private NodeGrid m_nodeGrid;

		private bool m_markedToDeSelect = false;
		private int m_markToSelect = -1;
		private bool m_markToReOrder = false;

		private bool m_hasUnConnectedNodes = false;

		private bool m_checkSelectedWireHighlights = false;

		//private Rect m_auxRect = new Rect();

		// Bezier info
		private List<WireBezierReference> m_bezierReferences;
		private const int MaxBezierReferences = 50;
		private int m_wireBezierCount = 0;

		protected int m_normalDependentCount = 0;

		public ParentGraph()
		{
			m_normalDependentCount = 0;
			m_nodeGrid = new NodeGrid();
			m_nodes = new List<ParentNode>();
			m_samplerNodes = new NodeUsageRegister();
			m_propertyNodes = new NodeUsageRegister();
			m_texturePropertyNodes = new NodeUsageRegister();
			m_textureArrayNodes = new NodeUsageRegister();
			m_screenColorNodes = new NodeUsageRegister();
			m_localVarNodes = new NodeUsageRegister();

			m_selectedNodes = new List<ParentNode>();
			m_markedForDeletion = new List<ParentNode>();
			m_highlightedWires = new List<WireReference>();
			m_nodesDict = new Dictionary<int, ParentNode>();
			m_validNodeId = 0;
			IsDirty = false;
			SaveIsDirty = false;
			m_masterNodeDefaultType = typeof( StandardSurfaceOutputNode );

			m_bezierReferences = new List<WireBezierReference>( MaxBezierReferences );
			for ( int i = 0; i < MaxBezierReferences; i++ )
			{
				m_bezierReferences.Add( new WireBezierReference() );
			}

			nodeStyleOff = UIUtils.GetCustomStyle( CustomStyle.NodeWindowOff );
			nodeStyleOn = UIUtils.GetCustomStyle( CustomStyle.NodeWindowOn );
			nodeTitle = UIUtils.GetCustomStyle( CustomStyle.NodeHeader );
			commentaryBackground = UIUtils.GetCustomStyle( CustomStyle.CommentaryBackground );
		}

		public int GetValidId()
		{
			return m_validNodeId++;
		}

		void UpdateIdFromNode( ParentNode node )
		{
			if ( node.UniqueId >= m_validNodeId )
			{
				m_validNodeId = node.UniqueId + 1;
			}
		}

		public void CleanUnusedNodes()
		{
			List<ParentNode> unusedNodes = new List<ParentNode>();
			for ( int i = 0; i < m_nodes.Count; i++ )
			{
				if ( m_nodes[ i ].ConnStatus == NodeConnectionStatus.Not_Connected )
				{
					unusedNodes.Add( m_nodes[ i ] );
				}
			}

			for ( int i = 0; i < unusedNodes.Count; i++ )
			{
				DestroyNode( unusedNodes[ i ] );
			}
			unusedNodes.Clear();
			unusedNodes = null;

			IsDirty = true;
		}

		// Destroy all nodes excluding Master Node
		public void ClearGraph()
		{
			List<ParentNode> list = new List<ParentNode>();
			int count = m_nodes.Count;
			for ( int i = 0; i < count; i++ )
			{
				if ( m_nodes[ i ].UniqueId != m_masterNodeId )
				{
					list.Add( m_nodes[ i ] );
				}
			}

			while ( list.Count > 0 )
			{
				DestroyNode( list[ 0 ] );
				list.RemoveAt( 0 );
			}
		}

		public void CleanNodes()
		{
			for ( int i = 0; i < m_nodes.Count; i++ )
			{
				m_nodes[ i ].Destroy();
				GameObject.DestroyImmediate( m_nodes[ i ] );
			}

			m_masterNodeId = Constants.INVALID_NODE_ID;
			m_validNodeId = 0;
			m_instancePropertyCount = 0;
			m_virtualTextureCount = 0;

			m_nodes.Clear();
			m_samplerNodes.Clear();
			m_propertyNodes.Clear();
			m_texturePropertyNodes.Clear();
			m_textureArrayNodes.Clear();
			m_screenColorNodes.Clear();
			m_localVarNodes.Clear();
			m_selectedNodes.Clear();
			m_markedForDeletion.Clear();
		}

		public void ResetHighlightedWires()
		{
			for ( int i = 0; i < m_highlightedWires.Count; i++ )
			{
				m_highlightedWires[ i ].WireStatus = WireStatus.Default;
			}
			m_highlightedWires.Clear();
		}

		public void HighlightWiresStartingNode( ParentNode node )
		{
			for ( int outputIdx = 0; outputIdx < node.OutputPorts.Count; outputIdx++ )
			{
				for ( int extIdx = 0; extIdx < node.OutputPorts[ outputIdx ].ExternalReferences.Count; extIdx++ )
				{
					WireReference wireRef = node.OutputPorts[ outputIdx ].ExternalReferences[ extIdx ];
					ParentNode nextNode = GetNode( wireRef.NodeId );
					if ( nextNode && nextNode.ConnStatus == NodeConnectionStatus.Connected )
					{
						InputPort port = nextNode.GetInputPortByUniqueId( wireRef.PortId );
						if ( port.ExternalReferences.Count == 0 || port.ExternalReferences[ 0 ].WireStatus == WireStatus.Highlighted )
						{
							// if even one wire is already highlighted then this tells us that this node was already been analysed
							return;
						}

						port.ExternalReferences[ 0 ].WireStatus = WireStatus.Highlighted;
						m_highlightedWires.Add( port.ExternalReferences[ 0 ] );
						HighlightWiresStartingNode( nextNode );
					}
				}
			}
		}

		void PropagateHighlightDeselection( ParentNode node, int portId = -1 )
		{
			if ( portId > -1 )
			{
				InputPort port = node.GetInputPortByUniqueId( portId );
				port.ExternalReferences[ 0 ].WireStatus = WireStatus.Default;
			}

			if ( node.Selected )
				return;

			for ( int i = 0; i < node.InputPorts.Count; i++ )
			{
				if ( node.InputPorts[ i ].ExternalReferences.Count > 0 && node.InputPorts[ i ].ExternalReferences[ 0 ].WireStatus == WireStatus.Highlighted )
				{
					// even though node is deselected, it receives wire highlight from a previous one 
					return;
				}
			}

			for ( int outputIdx = 0; outputIdx < node.OutputPorts.Count; outputIdx++ )
			{
				for ( int extIdx = 0; extIdx < node.OutputPorts[ outputIdx ].ExternalReferences.Count; extIdx++ )
				{
					WireReference wireRef = node.OutputPorts[ outputIdx ].ExternalReferences[ extIdx ];
					ParentNode nextNode = GetNode( wireRef.NodeId );
					PropagateHighlightDeselection( nextNode, wireRef.PortId );
				}
			}
		}

		public void ResetNodesData()
		{
			int count = m_nodes.Count;
			for ( int i = 0; i < count; i++ )
			{
				m_nodes[ i ].ResetNodeData();
			}
		}

		public void Destroy()
		{
			for ( int i = 0; i < m_nodes.Count; i++ )
			{
				if ( m_nodes[ i ] != null )
				{
					m_nodes[ i ].Destroy();
					GameObject.DestroyImmediate( m_nodes[ i ] );
				}
			}

			m_masterNodeId = Constants.INVALID_NODE_ID;
			m_validNodeId = 0;
			m_instancePropertyCount = 0;

			m_nodeGrid.Destroy();
			m_nodeGrid = null;

			m_nodes.Clear();
			m_nodes = null;

			m_samplerNodes.Destroy();
			m_samplerNodes = null;

			m_propertyNodes.Destroy();
			m_propertyNodes = null;

			m_texturePropertyNodes.Destroy();
			m_texturePropertyNodes = null;

			m_textureArrayNodes.Destroy();
			m_textureArrayNodes = null;

			m_screenColorNodes.Destroy();
			m_screenColorNodes = null;

			m_localVarNodes.Destroy();
			m_localVarNodes = null;

			m_selectedNodes.Clear();
			m_selectedNodes = null;

			m_markedForDeletion.Clear();
			m_markedForDeletion = null;

			m_nodesDict.Clear();
			m_nodesDict = null;

			m_nodePreviewList.Clear();
			m_nodePreviewList = null;

			IsDirty = true;

			OnNodeEvent = null;

			OnMaterialUpdatedEvent = null;
			OnShaderUpdatedEvent = null;
			OnEmptyGraphDetectedEvt = null;

			nodeStyleOff = null;
			nodeStyleOn = null;
			nodeTitle = null;
			commentaryBackground = null;
		}

		void OnNodeChangeSizeEvent( ParentNode node )
		{
			m_nodeGrid.RemoveNodeFromGrid( node, true );
			m_nodeGrid.AddNodeToGrid( node );
		}

		void OnNodeFinishMoving( ParentNode node, bool testOnlySelected, InteractionMode interactionMode )
		{
			if ( OnNodeEvent != null )
				OnNodeEvent( node );

			m_nodeGrid.RemoveNodeFromGrid( node, true );
			m_nodeGrid.AddNodeToGrid( node );

			if ( testOnlySelected )
			{
				for ( int i = m_visibleNodes.Count - 1; i > -1; i-- )
				{
					if ( node.UniqueId != m_visibleNodes[ i ].UniqueId )
					{
						switch ( interactionMode )
						{
							case InteractionMode.Target:
							{
								node.OnNodeInteraction( m_visibleNodes[ i ] );
							}
							break;
							case InteractionMode.Other:
							{
								m_visibleNodes[ i ].OnNodeInteraction( node );
							}
							break;
							case InteractionMode.Both:
							{
								node.OnNodeInteraction( m_visibleNodes[ i ] );
								m_visibleNodes[ i ].OnNodeInteraction( node );
							}
							break;
						}
					}
				}
			}
			else
			{
				for ( int i = m_nodes.Count - 1; i > -1; i-- )
				{
					if ( node.UniqueId != m_nodes[ i ].UniqueId )
					{
						switch ( interactionMode )
						{
							case InteractionMode.Target:
							{
								node.OnNodeInteraction( m_nodes[ i ] );
							}
							break;
							case InteractionMode.Other:
							{
								m_nodes[ i ].OnNodeInteraction( node );
							}
							break;
							case InteractionMode.Both:
							{
								node.OnNodeInteraction( m_nodes[ i ] );
								m_nodes[ i ].OnNodeInteraction( node );
							}
							break;
						}
					}
				}
			}
		}


		public void OnNodeReOrderEvent( ParentNode node, int index )
		{
			if ( node.Depth < index )
			{
				Debug.LogWarning( "Reorder canceled: This is a specific method for when reordering needs to be done and a its original index is higher than the new one" );
			}
			else
			{
				m_nodes.Remove( node );
				m_nodes.Insert( index, node );
				m_markToReOrder = true;
			}
		}

		public void AddNode( ParentNode node, bool updateId = false, bool addLast = true, bool registerUndo = true )
		{
			if ( registerUndo )
				UIUtils.RegisterCreatedObjectUndo( node );

			if ( OnNodeEvent != null )
			{
				OnNodeEvent( node );
			}
			if ( updateId )
			{
				node.UniqueId = GetValidId();
			}
			else
			{
				UpdateIdFromNode( node );
			}

			node.SetMaterialMode( CurrentMaterial );

			if ( addLast )
			{
				m_nodes.Add( node );
				node.Depth = m_nodes.Count;
			}
			else
			{
				m_nodes.Insert( 0, node );
				node.Depth = 0;
			}

			if ( m_nodesDict.ContainsKey( node.UniqueId ) )
				m_nodesDict[ node.UniqueId ] = node;
			else
				m_nodesDict.Add( node.UniqueId, node );

			m_nodeGrid.AddNodeToGrid( node );
			node.OnNodeChangeSizeEvent += OnNodeChangeSizeEvent;
			node.OnNodeReOrderEvent += OnNodeReOrderEvent;
			IsDirty = true;
		}

		public void RemoveNode( ParentNode node )
		{
			m_nodes.Remove( node );
			m_nodesDict.Remove( node.UniqueId );
			if ( node.UniqueId == m_masterNodeId )
			{
				m_masterNodeId = -1;
			}
			node.Destroy();
			IsDirty = true;
			m_markToReOrder = true;
		}

		public ParentNode GetClickedNode()
		{
			if ( m_nodeClicked < 0 )
				return null;
			return GetNode( m_nodeClicked );
		}

		public ParentNode GetNode( int nodeId )
		{
			if ( m_nodesDict.Count != m_nodes.Count )
			{
				m_nodesDict.Clear();
				for ( int i = 0; i < m_nodes.Count; i++ )
				{
					if ( m_nodes[ i ] != null )
						m_nodesDict.Add( m_nodes[ i ].UniqueId, m_nodes[ i ] );
				}
			}

			if ( m_nodesDict.ContainsKey( nodeId ) )
				return m_nodesDict[ nodeId ];

			return null;
		}

		public void ForceReOrder()
		{
			m_nodes.Sort( ( x, y ) => x.Depth.CompareTo( y.Depth ) );
		}

		public bool Draw( DrawInfo drawInfo )
		{

			if ( m_afterDeserializeFlag )
			{
				m_afterDeserializeFlag = false;
				CleanCorruptedNodes();
				if ( m_nodes.Count == 0 )
				{
					UIUtils.CreateNewGraph( "Empty" );
					SaveIsDirty = true;
					if ( OnEmptyGraphDetectedEvt != null )
						OnEmptyGraphDetectedEvt( this );
				}
			}

			if ( m_markedToDeSelect )
				DeSelectAll();

			if ( m_markToSelect > -1 )
			{
				AddToSelectedNodes( GetNode( m_markToSelect ) );
				m_markToSelect = -1;
			}

			if ( m_markToReOrder )
			{
				m_markToReOrder = false;
				for ( int i = 0; i < m_nodes.Count; i++ )
				{
					m_nodes[ i ].Depth = i;
				}
			}

			// Resizing Nods per LOD level
			NodeLOD newLevel = NodeLOD.LOD0;
			float referenceValue;
			if ( drawInfo.InvertedZoom > 0.5f )
			{
				newLevel = NodeLOD.LOD0;
				referenceValue = 4;
			}
			else if ( drawInfo.InvertedZoom > 0.25f )
			{
				newLevel = NodeLOD.LOD1;
				referenceValue = 2;
			}
			else if ( drawInfo.InvertedZoom > 0.15f )
			{
				newLevel = NodeLOD.LOD2;
				referenceValue = 1;
			}
			else if ( drawInfo.InvertedZoom > 0.1f )
			{
				newLevel = NodeLOD.LOD3;
				referenceValue = 0;
			}
			else
			{
				newLevel = NodeLOD.LOD4;
				referenceValue = 0;
			}

			// Just a sanity check
			nodeStyleOff = UIUtils.GetCustomStyle( CustomStyle.NodeWindowOff );
			nodeStyleOn = UIUtils.GetCustomStyle( CustomStyle.NodeWindowOn );
			nodeTitle = UIUtils.GetCustomStyle( CustomStyle.NodeHeader );
			commentaryBackground = UIUtils.GetCustomStyle( CustomStyle.CommentaryBackground );

			// only resize if it changes (check one value to fix any external change, ie: loading)
			if ( newLevel != m_lodLevel || ( UIUtils.MainSkin != null && UIUtils.MainSkin.textField.border.left != referenceValue ) )
			{
				m_lodLevel = newLevel;
				switch ( m_lodLevel )
				{
					default:
					case NodeLOD.LOD0:
					{
						UIUtils.MainSkin.textField.border.left = 4;
						UIUtils.MainSkin.textField.border.right = 4;
						UIUtils.MainSkin.textField.border.top = 4;
						UIUtils.MainSkin.textField.border.bottom = 4;

						nodeStyleOff.border.left = 6;
						nodeStyleOff.border.right = 6;
						nodeStyleOff.border.top = 6;
						nodeStyleOff.border.bottom = 6;

						nodeStyleOn.border.left = 6;
						nodeStyleOn.border.right = 6;
						nodeStyleOn.border.top = 6;
						nodeStyleOn.border.bottom = 6;

						nodeTitle.border.left = 6;
						nodeTitle.border.right = 6;
						nodeTitle.border.top = 6;
						nodeTitle.border.bottom = 4;

						commentaryBackground.border.left = 6;
						commentaryBackground.border.right = 6;
						commentaryBackground.border.top = 6;
						commentaryBackground.border.bottom = 6;
					}
					break;
					case NodeLOD.LOD1:
					{
						UIUtils.MainSkin.textField.border.left = 2;
						UIUtils.MainSkin.textField.border.right = 2;
						UIUtils.MainSkin.textField.border.top = 2;
						UIUtils.MainSkin.textField.border.bottom = 2;

						nodeStyleOff.border.left = 5;
						nodeStyleOff.border.right = 5;
						nodeStyleOff.border.top = 5;
						nodeStyleOff.border.bottom = 5;

						nodeStyleOn.border.left = 5;
						nodeStyleOn.border.right = 5;
						nodeStyleOn.border.top = 5;
						nodeStyleOn.border.bottom = 5;

						nodeTitle.border.left = 5;
						nodeTitle.border.right = 5;
						nodeTitle.border.top = 5;
						nodeTitle.border.bottom = 2;

						commentaryBackground.border.left = 5;
						commentaryBackground.border.right = 5;
						commentaryBackground.border.top = 5;
						commentaryBackground.border.bottom = 5;
					}
					break;
					case NodeLOD.LOD2:
					{
						UIUtils.MainSkin.textField.border.left = 1;
						UIUtils.MainSkin.textField.border.right = 1;
						UIUtils.MainSkin.textField.border.top = 1;
						UIUtils.MainSkin.textField.border.bottom = 1;

						nodeStyleOff.border.left = 2;
						nodeStyleOff.border.right = 2;
						nodeStyleOff.border.top = 2;
						nodeStyleOff.border.bottom = 3;

						nodeStyleOn.border.left = 4;
						nodeStyleOn.border.right = 4;
						nodeStyleOn.border.top = 4;
						nodeStyleOn.border.bottom = 3;

						nodeTitle.border.left = 2;
						nodeTitle.border.right = 2;
						nodeTitle.border.top = 2;
						nodeTitle.border.bottom = 2;

						commentaryBackground.border.left = 2;
						commentaryBackground.border.right = 2;
						commentaryBackground.border.top = 2;
						commentaryBackground.border.bottom = 3;
					}
					break;
					case NodeLOD.LOD3:
					case NodeLOD.LOD4:
					{
						UIUtils.MainSkin.textField.border.left = 0;
						UIUtils.MainSkin.textField.border.right = 0;
						UIUtils.MainSkin.textField.border.top = 0;
						UIUtils.MainSkin.textField.border.bottom = 0;

						nodeStyleOff.border.left = 1;
						nodeStyleOff.border.right = 1;
						nodeStyleOff.border.top = 1;
						nodeStyleOff.border.bottom = 2;

						nodeStyleOn.border.left = 2;
						nodeStyleOn.border.right = 2;
						nodeStyleOn.border.top = 2;
						nodeStyleOn.border.bottom = 2;

						nodeTitle.border.left = 1;
						nodeTitle.border.right = 1;
						nodeTitle.border.top = 1;
						nodeTitle.border.bottom = 1;

						commentaryBackground.border.left = 1;
						commentaryBackground.border.right = 1;
						commentaryBackground.border.top = 1;
						commentaryBackground.border.bottom = 2;
					}
					break;
				}
			}

			m_visibleNodes.Clear();
			int nullCount = 0;
			m_hasUnConnectedNodes = false;
			bool repaint = false;
			MasterNode masterNode = GetNode( m_masterNodeId ) as MasterNode;
			Material currentMaterial = masterNode != null ? masterNode.CurrentMaterial : null;
			EditorGUI.BeginChangeCheck();
			for ( int i = 0; i < m_nodes.Count; i++ )
			{
				if ( m_nodes[ i ] != null )
				{
					//repaint = repaint || _nodes[ i ].SafeDraw( drawInfo );
					if ( !m_nodes[ i ].IsOnGrid )
					{
						m_nodeGrid.AddNodeToGrid( m_nodes[ i ] );
					}

					bool restoreMouse = false;
					if ( Event.current.type == EventType.mouseDown && m_nodes[ i ].UniqueId != m_nodeClicked )
					{
						restoreMouse = true;
						Event.current.type = EventType.ignore;
					}

					m_nodes[ i ].Draw( drawInfo );

					if ( restoreMouse )
					{
						Event.current.type = EventType.mouseDown;
					}

					m_hasUnConnectedNodes = m_hasUnConnectedNodes ||
											( m_nodes[ i ].ConnStatus != NodeConnectionStatus.Connected && m_nodes[ i ].ConnStatus != NodeConnectionStatus.Island );

					if ( m_nodes[ i ].RequireMaterialUpdate && currentMaterial != null )
					{
						m_nodes[ i ].UpdateMaterial( currentMaterial );
						if ( ASEMaterialInspector.Instance != null )
						{
							 ASEMaterialInspector.Instance.Repaint();
						}
						//if ( currentMaterial == Selection.activeObject )
						//{

						//}
					}

					if ( m_nodes[ i ].IsVisible )
						m_visibleNodes.Add( m_nodes[ i ] );

					IsDirty = ( m_isDirty || m_nodes[ i ].IsDirty );
					SaveIsDirty = ( m_saveIsDirty || m_nodes[ i ].SaveIsDirty );
				}
				else
					nullCount += 1;
			}

			// Revert LOD changes to LOD0 (only if it's different)
			if ( UIUtils.MainSkin.textField.border.left != 4 )
			{
				UIUtils.MainSkin.textField.border.left = 4;
				UIUtils.MainSkin.textField.border.right = 4;
				UIUtils.MainSkin.textField.border.top = 4;
				UIUtils.MainSkin.textField.border.bottom = 4;

				nodeStyleOff.border.left = 6;
				nodeStyleOff.border.right = 6;
				nodeStyleOff.border.top = 6;
				nodeStyleOff.border.bottom = 6;

				nodeStyleOn.border.left = 6;
				nodeStyleOn.border.right = 6;
				nodeStyleOn.border.top = 6;
				nodeStyleOn.border.bottom = 6;

				nodeTitle.border.left = 6;
				nodeTitle.border.right = 6;
				nodeTitle.border.top = 6;
				nodeTitle.border.bottom = 4;

				commentaryBackground.border.left = 6;
				commentaryBackground.border.right = 6;
				commentaryBackground.border.top = 6;
				commentaryBackground.border.bottom = 6;
			}

			if ( m_checkSelectedWireHighlights )
			{
				m_checkSelectedWireHighlights = false;
				ResetHighlightedWires();
				for ( int i = 0; i < m_selectedNodes.Count; i++ )
				{
					HighlightWiresStartingNode( m_selectedNodes[ i ] );
				}
			}

			if ( EditorGUI.EndChangeCheck() )
			{
				repaint = true;
				SaveIsDirty = true;
			}


			if ( nullCount == m_nodes.Count )
				m_nodes.Clear();


			return repaint;
		}

		public bool UpdateMarkForDeletion()
		{
			if ( m_markedForDeletion.Count != 0 )
			{
				DeleteMarkedForDeletionNodes();
				return true;
			}
			return false;
		}

		public void DrawWires( Texture2D wireTex, DrawInfo drawInfo, bool contextPaletteActive, Vector3 contextPalettePos )
		{
			Handles.BeginGUI();
			// Draw connected node wires
			m_wireBezierCount = 0;
			for ( int nodeIdx = 0; nodeIdx < m_nodes.Count; nodeIdx++ )
			{
				ParentNode node = m_nodes[ nodeIdx ];
				if ( ( object ) node == null )
					return;

				for ( int inputPortIdx = 0; inputPortIdx < node.InputPorts.Count; inputPortIdx++ )
				{
					InputPort inputPort = node.InputPorts[ inputPortIdx ];
					if ( inputPort.ExternalReferences.Count > 0 )
					{
						bool cleanInvalidConnections = false;
						for ( int wireIdx = 0; wireIdx < inputPort.ExternalReferences.Count; wireIdx++ )
						{
							WireReference reference = inputPort.ExternalReferences[ wireIdx ];
							if ( reference.NodeId != -1 && reference.PortId != -1 )
							{
								ParentNode outputNode = GetNode( reference.NodeId );
								if ( outputNode != null )
								{
									OutputPort outputPort = outputNode.GetOutputPortByUniqueId( reference.PortId );
									Vector3 endPos = new Vector3( inputPort.Position.x, inputPort.Position.y );
									Vector3 startPos = new Vector3( outputPort.Position.x, outputPort.Position.y );
									float x = ( startPos.x < endPos.x ) ? startPos.x : endPos.x;
									float y = ( startPos.y < endPos.y ) ? startPos.y : endPos.y;
									float width = Mathf.Abs( startPos.x - endPos.x ) + outputPort.Position.width;
									float height = Mathf.Abs( startPos.y - endPos.y ) + outputPort.Position.height;
									Rect portsBoundingBox = new Rect( x, y, width, height );

									bool isVisible = node.IsVisible || outputNode.IsVisible;
									if ( !isVisible )
									{
										isVisible = drawInfo.TransformedCameraArea.Overlaps( portsBoundingBox );
									}

									if ( isVisible )
									{

										Rect bezierBB = DrawBezier( drawInfo.InvertedZoom, startPos, endPos, inputPort.DataType, outputPort.DataType, reference.WireStatus, wireTex, node, outputNode );
										bezierBB.x -= Constants.OUTSIDE_WIRE_MARGIN;
										bezierBB.y -= Constants.OUTSIDE_WIRE_MARGIN;

										bezierBB.width += Constants.OUTSIDE_WIRE_MARGIN * 2;
										bezierBB.height += Constants.OUTSIDE_WIRE_MARGIN * 2;

										if ( m_wireBezierCount < m_bezierReferences.Count )
										{
											m_bezierReferences[ m_wireBezierCount ].UpdateInfo( ref bezierBB, inputPort.NodeId, inputPort.PortId, outputPort.NodeId, outputPort.PortId );
										}
										else
										{
											m_bezierReferences.Add( new WireBezierReference( ref bezierBB, inputPort.NodeId, inputPort.PortId, outputPort.NodeId, outputPort.PortId ) );
										}
										m_wireBezierCount++;

									}
								}
								else
								{
									UIUtils.ShowMessage( "Detected Invalid connection from node " + node.UniqueId + " port " + inputPortIdx + " to Node " + reference.NodeId + " port " + reference.PortId, MessageSeverity.Error );
									cleanInvalidConnections = true;
									inputPort.ExternalReferences[ wireIdx ].Invalidate();
								}
							}
						}

						if ( cleanInvalidConnections )
						{
							inputPort.RemoveInvalidConnections();
						}
					}
				}
			}

			//Draw selected wire
			if ( UIUtils.ValidReferences() )
			{
				if ( UIUtils.InputPortReference.IsValid )
				{
					InputPort inputPort = GetNode( UIUtils.InputPortReference.NodeId ).GetInputPortByUniqueId( UIUtils.InputPortReference.PortId );
					Vector3 endPos = Vector3.zero;
					if ( UIUtils.SnapEnabled )
					{
						Vector2 pos = ( UIUtils.SnapPosition + drawInfo.CameraOffset ) * drawInfo.InvertedZoom;
						endPos = new Vector3( pos.x, pos.y ) + UIUtils.ScaledPortsDelta;
					}
					else
					{
						endPos = contextPaletteActive ? contextPalettePos : new Vector3( Event.current.mousePosition.x, Event.current.mousePosition.y );
					}

					Vector3 startPos = new Vector3( inputPort.Position.x, inputPort.Position.y );
					DrawBezier( drawInfo.InvertedZoom, endPos, startPos, inputPort.DataType, inputPort.DataType, WireStatus.Default, wireTex );
				}

				if ( UIUtils.OutputPortReference.IsValid )
				{
					OutputPort outputPort = GetNode( UIUtils.OutputPortReference.NodeId ).GetOutputPortByUniqueId( UIUtils.OutputPortReference.PortId );
					Vector3 endPos = Vector3.zero;
					if ( UIUtils.SnapEnabled )
					{
						Vector2 pos = ( UIUtils.SnapPosition + drawInfo.CameraOffset ) * drawInfo.InvertedZoom;
						endPos = new Vector3( pos.x, pos.y ) + UIUtils.ScaledPortsDelta;
					}
					else
					{
						endPos = contextPaletteActive ? contextPalettePos : new Vector3( Event.current.mousePosition.x, Event.current.mousePosition.y );
					}
					Vector3 startPos = new Vector3( outputPort.Position.x, outputPort.Position.y );
					DrawBezier( drawInfo.InvertedZoom, startPos, endPos, outputPort.DataType, outputPort.DataType, WireStatus.Default, wireTex );
				}
			}
			Handles.EndGUI();
		}

		Rect DrawBezier( float invertedZoom, Vector3 startPos, Vector3 endPos, WirePortDataType inputDataType, WirePortDataType outputDataType, WireStatus wireStatus, Texture2D wireTex, ParentNode inputNode = null, ParentNode outputNode = null )
		{
			startPos += UIUtils.ScaledPortsDelta;
			endPos += UIUtils.ScaledPortsDelta;

			float wiresTickness =/* drawInfo.InvertedZoom * */Constants.WIRE_WIDTH;

			// Calculate the 4 points for bezier taking into account wire nodes and their automatic tangents
			float mag = ( endPos - startPos ).magnitude;
			float resizedMag = Mathf.Min( mag, Constants.HORIZONTAL_TANGENT_SIZE * invertedZoom );

			Vector3 startTangent = new Vector3( startPos.x + resizedMag, startPos.y );
			Vector3 endTangent = new Vector3( endPos.x - resizedMag, endPos.y );

			if ( ( object ) inputNode != null && inputNode.GetType() == typeof( WireNode ) )
				endTangent = endPos + ( ( inputNode as WireNode ).TangentDirection ) * mag * 0.33f;

			if ( ( object ) outputNode != null && outputNode.GetType() == typeof( WireNode ) )
				startTangent = startPos - ( ( outputNode as WireNode ).TangentDirection ) * mag * 0.33f;

			///////////////Draw tangents
			//Rect box1 = new Rect( new Vector2( startTangent.x, startTangent.y ), new Vector2( 10, 10 ) );
			//box1.x -= box1.width * 0.5f;
			//box1.y -= box1.height * 0.5f;
			//GUI.Box( box1, string.Empty, UIUtils.CurrentWindow.CustomStylesInstance.Box );

			//Rect box2 = new Rect( new Vector2( endTangent.x, endTangent.y ), new Vector2( 10, 10 ) );
			//box2.x -= box2.width * 0.5f;
			//box2.y -= box2.height * 0.5f;
			//GUI.Box( box2, string.Empty, UIUtils.CurrentWindow.CustomStylesInstance.Box );

			//m_auxRect.Set( 0, 0, UIUtils.CurrentWindow.position.width, UIUtils.CurrentWindow.position.height );
			//GLDraw.BeginGroup( m_auxRect );
			Rect boundBox = new Rect();
			int segments = Mathf.Clamp( Mathf.FloorToInt( ( endPos - startPos ).magnitude * 0.2f * invertedZoom ), 15, 35 );
			if ( UIUtils.CurrentWindow.Options.ColoredPorts && wireStatus != WireStatus.Highlighted )
				boundBox = GLDraw.DrawBezier( startPos, startTangent, endPos, endTangent, UIUtils.GetColorForDataType( outputDataType, false, false ), UIUtils.GetColorForDataType( inputDataType, false, false ), wiresTickness * 0.7f, segments );
			else
				boundBox = GLDraw.DrawBezier( startPos, startTangent, endPos, endTangent, UIUtils.GetColorFromWireStatus( wireStatus ), wiresTickness * 0.7f, segments );
			//GLDraw.EndGroup();

			//GUI.Box( m_auxRect, string.Empty, UIUtils.CurrentWindow.CustomStylesInstance.Box );
			//GUI.Box( boundBox, string.Empty, UIUtils.CurrentWindow.CustomStylesInstance.Box );
			//if ( UIUtils.CurrentWindow.Options.ColoredPorts && wireStatus != WireStatus.Highlighted )
			//	Handles.DrawBezier( startPos, endPos, startTangent, endTangent, UIUtils.GetColorForDataType( outputDataType, false, false ), wireTex, wiresTickness );
			//else
			//	Handles.DrawBezier( startPos, endPos, startTangent, endTangent, UIUtils.GetColorFromWireStatus( wireStatus ), wireTex, wiresTickness );

			//Handles.DrawLine( startPos, startTangent );
			//Handles.DrawLine( endPos, endTangent );
			return boundBox;
		}


		public void DrawBezierBoundingBox()
		{
			for ( int i = 0; i < m_wireBezierCount; i++ )
			{
				m_bezierReferences[ i ].DebugDraw();
			}
		}


		public WireBezierReference GetWireBezierInPos( Vector2 position )
		{
			for ( int i = 0; i < m_wireBezierCount; i++ )
			{
				if ( m_bezierReferences[ i ].Contains( position ) )
					return m_bezierReferences[ i ];
			}
			return null;
		}


		public List<WireBezierReference> GetWireBezierListInPos( Vector2 position )
		{
			List<WireBezierReference> list = new List<WireBezierReference>();
			for ( int i = 0; i < m_wireBezierCount; i++ )
			{
				if ( m_bezierReferences[ i ].Contains( position ) )
					list.Add( m_bezierReferences[ i ] );
			}

			return list;
		}


		public void MoveSelectedNodes( Vector2 delta, bool snap = false )
		{
			for ( int i = 0; i < m_selectedNodes.Count; i++ )
			{
				if ( !m_selectedNodes[ i ].MovingInFrame )
					m_selectedNodes[ i ].Move( delta, snap );
			}
			IsDirty = true;
		}

		public void SetConnection( int InNodeId, int InPortId, int OutNodeId, int OutPortId )
		{
			ParentNode inNode = GetNode( InNodeId );
			ParentNode outNode = GetNode( OutNodeId );
			InputPort inputPort = null;
			OutputPort outputPort = null;
			if ( inNode != null && outNode != null )
			{
				inputPort = inNode.GetInputPortByUniqueId( InPortId );
				outputPort = outNode.GetOutputPortByUniqueId( OutPortId );
				if ( inputPort != null && outputPort != null )
				{
					if ( inputPort.IsConnectedTo( OutNodeId, OutPortId ) || outputPort.IsConnectedTo( InNodeId, InPortId ) )
					{
						if ( DebugConsoleWindow.DeveloperMode )
							UIUtils.ShowMessage( "Node/Port already connected " + InNodeId, MessageSeverity.Error );
						return;
					}

					if ( !inputPort.CheckValidType( outputPort.DataType ) )
					{
						if ( DebugConsoleWindow.DeveloperMode )
							UIUtils.ShowIncompatiblePortMessage( true, inNode, inputPort, outNode, outputPort );
						return;
					}

					if ( !outputPort.CheckValidType( inputPort.DataType ) )
					{

						if ( DebugConsoleWindow.DeveloperMode )
							UIUtils.ShowIncompatiblePortMessage( false, outNode, outputPort, inNode, inputPort );
						return;
					}
					if ( !inputPort.Available || !outputPort.Available )
					{
						if ( DebugConsoleWindow.DeveloperMode )
							UIUtils.ShowMessage( "Ports not available to connection", MessageSeverity.Warning );

						return;
					}

					if ( inputPort.ConnectTo( OutNodeId, OutPortId, outputPort.DataType, false ) )
					{
						inNode.OnInputPortConnected( InPortId, OutNodeId, OutPortId );
					}


					if ( outputPort.ConnectTo( InNodeId, InPortId, inputPort.DataType, inputPort.TypeLocked ) )
					{
						outNode.OnOutputPortConnected( OutPortId, InNodeId, InPortId );
					}
				}
				else if ( ( object ) inputPort == null )
				{
					if ( DebugConsoleWindow.DeveloperMode )
						UIUtils.ShowMessage( "Input Port " + InPortId + " doesn't exist on node " + InNodeId, MessageSeverity.Error );
				}
				else
				{
					if ( DebugConsoleWindow.DeveloperMode )
						UIUtils.ShowMessage( "Output Port " + OutPortId + " doesn't exist on node " + OutNodeId, MessageSeverity.Error );
				}
			}
			else if ( ( object ) inNode == null )
			{
				if ( DebugConsoleWindow.DeveloperMode )
					UIUtils.ShowMessage( "Input node " + InNodeId + " doesn't exist", MessageSeverity.Error );
			}
			else
			{
				if ( DebugConsoleWindow.DeveloperMode )
					UIUtils.ShowMessage( "Output node " + OutNodeId + " doesn't exist", MessageSeverity.Error );
			}
		}

		public void CreateConnection( int inNodeId, int inPortId, int outNodeId, int outPortId )
		{
			ParentNode outputNode = GetNode( outNodeId );
			if ( outputNode != null )
			{
				OutputPort outputPort = outputNode.OutputPorts[ outPortId ];
				if ( outputPort != null )
				{
					ParentNode inputNode = GetNode( inNodeId );
					InputPort inputPort = inputNode.GetInputPortByUniqueId( inPortId );

					if ( !inputPort.CheckValidType( outputPort.DataType ) )
					{
						UIUtils.ShowIncompatiblePortMessage( true, inputNode, inputPort, outputNode, outputPort );
						return;
					}

					if ( !outputPort.CheckValidType( inputPort.DataType ) )
					{
						UIUtils.ShowIncompatiblePortMessage( false, outputNode, outputPort, inputNode, inputPort );
						return;
					}

					inputPort.DummyAdd( outputPort.NodeId, outputPort.PortId );
					outputPort.DummyAdd( inNodeId, inPortId );

					if ( UIUtils.DetectNodeLoopsFrom( inputNode, new Dictionary<int, int>() ) )
					{
						inputPort.DummyRemove();
						outputPort.DummyRemove();
						UIUtils.InvalidateReferences();
						UIUtils.ShowMessage( "Infinite Loop detected" );
						Event.current.Use();
						return;
					}

					inputPort.DummyRemove();
					outputPort.DummyRemove();

					if ( inputPort.IsConnected )
					{
						DeleteConnection( true, inNodeId, inPortId, true, false );
					}

					//link output to input
					if ( outputPort.ConnectTo( inNodeId, inPortId, inputPort.DataType, inputPort.TypeLocked ) )
						outputNode.OnOutputPortConnected( outputPort.PortId, inNodeId, inPortId );

					//link input to output
					if ( inputPort.ConnectTo( outputPort.NodeId, outputPort.PortId, outputPort.DataType, inputPort.TypeLocked ) )
						inputNode.OnInputPortConnected( inPortId, outputNode.UniqueId, outputPort.PortId );

					MarkWireHighlights();
				}
				UIUtils.CurrentWindow.ShaderIsModified = true;
			}
		}

		public void DeleteInvalidConnections()
		{
			int count = m_nodes.Count;
			for ( int nodeIdx = 0; nodeIdx < count; nodeIdx++ )
			{
				{
					int inputCount = m_nodes[ nodeIdx ].InputPorts.Count;
					for ( int inputIdx = 0; inputIdx < inputCount; inputIdx++ )
					{
						if ( !m_nodes[ nodeIdx ].InputPorts[ inputIdx ].Visible && m_nodes[ nodeIdx ].InputPorts[ inputIdx ].IsConnected )
						{
							DeleteConnection( true, m_nodes[ nodeIdx ].UniqueId, m_nodes[ nodeIdx ].InputPorts[ inputIdx ].PortId, true, true );
						}
					}
				}
				{
					int outputCount = m_nodes[ nodeIdx ].OutputPorts.Count;
					for ( int outputIdx = 0; outputIdx < outputCount; outputIdx++ )
					{
						if ( !m_nodes[ nodeIdx ].OutputPorts[ outputIdx ].Visible && m_nodes[ nodeIdx ].OutputPorts[ outputIdx ].IsConnected )
						{
							DeleteConnection( false, m_nodes[ nodeIdx ].UniqueId, m_nodes[ nodeIdx ].OutputPorts[ outputIdx ].PortId, true, true );
						}
					}
				}
			}
		}

		public void DeleteConnection( bool isInput, int nodeId, int portId, bool registerOnLog, bool propagateCallback )
		{
			ParentNode node = GetNode( nodeId );
			if ( ( object ) node == null )
				return;

			if ( isInput )
			{
				InputPort inputPort = node.GetInputPortByUniqueId( portId );
				if ( inputPort.IsConnected )
				{

					if ( node.ConnStatus == NodeConnectionStatus.Connected )
					{
						inputPort.GetOutputNode().DeactivateNode( portId, false );
						m_checkSelectedWireHighlights = true;
					}

					for ( int i = 0; i < inputPort.ExternalReferences.Count; i++ )
					{
						WireReference inputReference = inputPort.ExternalReferences[ i ];
						ParentNode outputNode = GetNode( inputReference.NodeId );
						outputNode.GetOutputPortByUniqueId( inputReference.PortId ).InvalidateConnection( inputPort.NodeId, inputPort.PortId );
						if ( propagateCallback )
							outputNode.OnOutputPortDisconnected( inputReference.PortId );
					}
					inputPort.InvalidateAllConnections();
					if ( propagateCallback )
						node.OnInputPortDisconnected( portId );
				}
			}
			else
			{
				OutputPort outputPort = node.GetOutputPortByUniqueId( portId );
				if ( outputPort.IsConnected )
				{
					if ( propagateCallback )
						node.OnOutputPortDisconnected( portId );

					for ( int i = 0; i < outputPort.ExternalReferences.Count; i++ )
					{
						WireReference outputReference = outputPort.ExternalReferences[ i ];
						ParentNode inputNode = GetNode( outputReference.NodeId );
						if ( inputNode.ConnStatus == NodeConnectionStatus.Connected )
						{
							node.DeactivateNode( portId, false );
							m_checkSelectedWireHighlights = true;
						}
						inputNode.GetInputPortByUniqueId( outputReference.PortId ).InvalidateConnection( outputPort.NodeId, outputPort.PortId );
						if ( propagateCallback )
							inputNode.OnInputPortDisconnected( outputReference.PortId );
					}
					outputPort.InvalidateAllConnections();
				}
			}
			IsDirty = true;
		}

		public void DeleteSelectedNodes()
		{
			bool invalidateMasterNode = false;
			int count = m_selectedNodes.Count;
			for ( int nodeIdx = 0; nodeIdx < count; nodeIdx++ )
			{
				ParentNode node = m_selectedNodes[ nodeIdx ];
				if ( node.UniqueId == m_masterNodeId )
				{
					invalidateMasterNode = true;
				}
				else
				{
					DestroyNode( node );
				}
			}

			if ( invalidateMasterNode )
			{
				CurrentMasterNode.Selected = false;
			}
			//Clear all references
			m_selectedNodes.Clear();
			IsDirty = true;
		}


		public void MarkWireNodeSequence( WireNode node, bool isInput )
		{
			if ( ( object ) node == null )
			{
				return;
			}

			m_markedForDeletion.Add( node );

			if ( isInput && node.InputPorts[ 0 ].IsConnected )
			{
				MarkWireNodeSequence( GetNode( node.InputPorts[ 0 ].ExternalReferences[ 0 ].NodeId ) as WireNode, isInput );
			}
			else if ( !isInput && node.OutputPorts[ 0 ].IsConnected )
			{
				MarkWireNodeSequence( GetNode( node.OutputPorts[ 0 ].ExternalReferences[ 0 ].NodeId ) as WireNode, isInput );
			}
		}

		public void DeleteMarkedForDeletionNodes()
		{
			bool invalidateMasterNode = false;
			int count = m_markedForDeletion.Count;
			for ( int nodeIdx = 0; nodeIdx < count; nodeIdx++ )
			{
				ParentNode node = m_markedForDeletion[ nodeIdx ];
				if ( node.UniqueId == m_masterNodeId )
				{
					invalidateMasterNode = true;
				}
				else
				{
					if ( node.Selected )
					{
						m_selectedNodes.Remove( node );
						node.Selected = false;
					}
					DestroyNode( node );
				}
			}

			if ( invalidateMasterNode )
			{
				CurrentMasterNode.Selected = false;
			}
			//Clear all references
			m_markedForDeletion.Clear();
			IsDirty = true;
		}

		public void DestroyNode( int nodeId )
		{
			ParentNode node = GetNode( nodeId );
			DestroyNode( node );
		}

		public void DestroyNode( ParentNode node )
		{
			if ( ( object ) node == null )
			{
				UIUtils.ShowMessage( "Attempting to destroying a inexistant node ", MessageSeverity.Warning );
				return;
			}

			if ( node.ConnStatus == NodeConnectionStatus.Connected && !m_checkSelectedWireHighlights )
			{
				ResetHighlightedWires();
				m_checkSelectedWireHighlights = true;
			}

			//TODO: check better placement of this code (reconnects wires from wire nodes)
			if ( node.GetType() == typeof( WireNode ) )
			{
				if ( node.InputPorts[ 0 ].ExternalReferences != null && node.InputPorts[ 0 ].ExternalReferences.Count > 0 )
				{
					WireReference backPort = node.InputPorts[ 0 ].ExternalReferences[ 0 ];
					for ( int i = 0; i < node.OutputPorts[ 0 ].ExternalReferences.Count; i++ )
					{
						UIUtils.CurrentWindow.ConnectInputToOutput( node.OutputPorts[ 0 ].ExternalReferences[ i ].NodeId, node.OutputPorts[ 0 ].ExternalReferences[ i ].PortId, backPort.NodeId, backPort.PortId );
					}
				}
			}

			if ( node.UniqueId != m_masterNodeId )
			{
				m_nodeGrid.RemoveNodeFromGrid( node, false );
				//Send Deactivation signal if active
				if ( node.ConnStatus == NodeConnectionStatus.Connected )
				{
					node.DeactivateNode( -1, true );
				}

				//Invalidate references
				//Invalidate input references
				for ( int inputPortIdx = 0; inputPortIdx < node.InputPorts.Count; inputPortIdx++ )
				{
					InputPort inputPort = node.InputPorts[ inputPortIdx ];
					if ( inputPort.IsConnected )
					{
						for ( int wireIdx = 0; wireIdx < inputPort.ExternalReferences.Count; wireIdx++ )
						{
							WireReference inputReference = inputPort.ExternalReferences[ wireIdx ];
							ParentNode outputNode = GetNode( inputReference.NodeId );
							outputNode.GetOutputPortByUniqueId( inputReference.PortId ).InvalidateConnection( inputPort.NodeId, inputPort.PortId );
							outputNode.OnOutputPortDisconnected( inputReference.PortId );
						}
						inputPort.InvalidateAllConnections();
					}
				}

				//Invalidate output reference
				for ( int outputPortIdx = 0; outputPortIdx < node.OutputPorts.Count; outputPortIdx++ )
				{
					OutputPort outputPort = node.OutputPorts[ outputPortIdx ];
					if ( outputPort.IsConnected )
					{
						for ( int wireIdx = 0; wireIdx < outputPort.ExternalReferences.Count; wireIdx++ )
						{
							WireReference outputReference = outputPort.ExternalReferences[ wireIdx ];
							ParentNode outnode = GetNode( outputReference.NodeId );
							if ( outnode != null )
							{
								outnode.GetInputPortByUniqueId( outputReference.PortId ).InvalidateConnection( outputPort.NodeId, outputPort.PortId );
								outnode.OnInputPortDisconnected( outputReference.PortId );
							}
						}
						outputPort.InvalidateAllConnections();
					}
				}

				//Remove node from main list
				m_nodes.Remove( node );
				m_nodesDict.Remove( node.UniqueId );
				node.Destroy();
				UIUtils.DestroyObjectImmediate( node );
				IsDirty = true;
				m_markToReOrder = true;
			}
			else
			{
				DeselectNode( node );
				UIUtils.ShowMessage( "Attempting to destroy a master node" );
			}
		}

		void AddToSelectedNodes( ParentNode node )
		{
			node.Selected = true;
			m_selectedNodes.Add( node );
			node.OnNodeStoppedMovingEvent += OnNodeFinishMoving;
			if ( node.ConnStatus == NodeConnectionStatus.Connected )
			{
				HighlightWiresStartingNode( node );
			}
		}

		void RemoveFromSelectedNodes( ParentNode node )
		{
			node.Selected = false;
			m_selectedNodes.Remove( node );
			node.OnNodeStoppedMovingEvent -= OnNodeFinishMoving;
		}

		public void SelectNode( ParentNode node, bool append, bool reorder )
		{

			if ( append )
			{
				if ( !m_selectedNodes.Contains( node ) )
				{
					AddToSelectedNodes( node );
				}
			}
			else
			{
				DeSelectAll();
				AddToSelectedNodes( node );
			}
			if ( reorder && !node.ReorderLocked )
			{
				m_nodes.Remove( node );
				m_nodes.Add( node );
				m_markToReOrder = true;
			}
		}

		public void MultipleSelection( Rect selectionArea, bool append, bool reorder )
		{
			if ( !append )
				DeSelectAll();

			for ( int i = 0; i < m_nodes.Count; i++ )
			{
				if ( !m_nodes[ i ].Selected && selectionArea.Overlaps( m_nodes[ i ].Position, true ) )
				{
					m_nodes[ i ].Selected = true;
					AddToSelectedNodes( m_nodes[ i ] );
				}
			}
			if ( reorder )
			{
				for ( int i = 0; i < m_selectedNodes.Count; i++ )
				{
					if ( !m_selectedNodes[ i ].ReorderLocked )
					{
						m_nodes.Remove( m_selectedNodes[ i ] );
						m_nodes.Add( m_selectedNodes[ i ] );
						m_markToReOrder = true;
					}
				}
			}
		}

		public void SelectAll()
		{
			for ( int i = 0; i < m_nodes.Count; i++ )
			{
				if ( !m_nodes[ i ].Selected )
					AddToSelectedNodes( m_nodes[ i ] );
			}
		}

		public void SelectMasterNode()
		{
			if ( m_masterNodeId != Constants.INVALID_NODE_ID )
			{
				SelectNode( CurrentMasterNode, false, false );
			}
		}

		public void DeselectNode( int nodeId )
		{
			ParentNode node = GetNode( nodeId );
			if ( node )
			{
				m_selectedNodes.Remove( node );
				node.Selected = false;
			}
		}

		public void DeselectNode( ParentNode node )
		{
			m_selectedNodes.Remove( node );
			node.Selected = false;
			PropagateHighlightDeselection( node );
		}



		public void DeSelectAll()
		{
			m_markedToDeSelect = false;
			for ( int i = 0; i < m_selectedNodes.Count; i++ )
			{
				m_selectedNodes[ i ].Selected = false;
				m_selectedNodes[ i ].OnNodeStoppedMovingEvent -= OnNodeFinishMoving;
			}
			m_selectedNodes.Clear();
			ResetHighlightedWires();
		}

		public void AssignMasterNode()
		{
			if ( m_selectedNodes.Count == 1 )
			{
				MasterNode newMasterNode = m_selectedNodes[ 0 ] as MasterNode;
				if ( newMasterNode != null )
				{
					if ( m_masterNodeId != Constants.INVALID_NODE_ID && m_masterNodeId != newMasterNode.UniqueId )
					{
						MasterNode oldMasterNode = GetNode( m_masterNodeId ) as MasterNode;
						if ( oldMasterNode != null )
							oldMasterNode.IsMainMasterNode = false;
					}
					m_masterNodeId = newMasterNode.UniqueId;
					newMasterNode.IsMainMasterNode = true;
					newMasterNode.OnMaterialUpdatedEvent += OnMaterialUpdatedEvent;
					newMasterNode.OnShaderUpdatedEvent += OnShaderUpdatedEvent;
				}
			}

			IsDirty = true;
		}

		public void AssignMasterNode( MasterNode node, bool onlyUpdateGraphId )
		{
			AssignMasterNode( node.UniqueId, onlyUpdateGraphId );
			node.OnMaterialUpdatedEvent += OnMaterialUpdatedEvent;
			node.OnShaderUpdatedEvent += OnShaderUpdatedEvent;
		}

		public void AssignMasterNode( int nodeId, bool onlyUpdateGraphId )
		{
			if ( nodeId < 0 || m_masterNodeId == nodeId )
				return;

			if ( m_masterNodeId > Constants.INVALID_NODE_ID )
			{
				MasterNode oldMasterNode = ( GetNode( nodeId ) as MasterNode );
				if ( oldMasterNode != null )
					oldMasterNode.IsMainMasterNode = false;
			}

			if ( onlyUpdateGraphId )
			{
				m_masterNodeId = nodeId;
			}
			else
			{
				MasterNode masterNode = ( GetNode( nodeId ) as MasterNode );
				if ( masterNode != null )
				{
					masterNode.IsMainMasterNode = true;
					m_masterNodeId = nodeId;
				}
			}

			IsDirty = true;
		}

		public void DrawGrid( DrawInfo drawInfo )
		{
			m_nodeGrid.DrawGrid( drawInfo );
		}

		public float MaxNodeDist
		{
			get { return m_nodeGrid.MaxNodeDist; }
		}

		public List<ParentNode> GetNodesInGrid( Vector2 transformedMousePos )
		{
			return m_nodeGrid.GetNodesOn( transformedMousePos );
		}

		public void FireMasterNode( Shader selectedShader )
		{
			( GetNode( m_masterNodeId ) as MasterNode ).Execute( selectedShader );
		}

		public Shader FireMasterNode( string pathname, bool isFullPath )
		{
			return ( GetNode( m_masterNodeId ) as MasterNode ).Execute( pathname, isFullPath );
		}

		public void ForceSignalPropagationOnMasterNode()
		{
			( GetNode( m_masterNodeId ) as MasterNode ).GenerateSignalPropagation();
			List<ParentNode> localVarNodes = m_localVarNodes.NodesList;
			int count = localVarNodes.Count;
			for ( int i = 0; i < count; i++ )
			{
				SignalGeneratorNode node = localVarNodes[ i ] as SignalGeneratorNode;
				if ( node != null )
				{
					node.GenerateSignalPropagation();
				}
			}
		}

		public void UpdateShaderOnMasterNode( Shader newShader )
		{
			( GetNode( m_masterNodeId ) as MasterNode ).UpdateFromShader( newShader );
		}

		public void CopyValuesFromMaterial( Material material )
		{
			Material currMaterial = CurrentMaterial;
			if ( currMaterial == material )
			{
				for ( int i = 0; i < m_nodes.Count; i++ )
				{
					m_nodes[ i ].ForceUpdateFromMaterial( material );
				}
			}
		}

		public void UpdateMaterialOnMasterNode( Material material )
		{
			( GetNode( m_masterNodeId ) as MasterNode ).UpdateMasterNodeMaterial( material );
		}

		public void SetMaterialModeOnGraph( Material mat )
		{
			for ( int i = 0; i < m_nodes.Count; i++ )
			{
				m_nodes[ i ].SetMaterialMode( mat );
			}
		}

		public ParentNode CheckNodeAt( Vector3 pos, bool checkForRMBIgnore = false )
		{
			ParentNode selectedNode = null;

			// this is checked on the inverse order to give priority to nodes that are drawn on top  ( last on the list )
			for ( int i = m_nodes.Count - 1; i > -1; i-- )
			{
				if ( m_nodes[ i ].GlobalPosition.Contains( pos ) )
				{
					if ( checkForRMBIgnore )
					{
						if ( !m_nodes[ i ].RMBIgnore )
						{
							selectedNode = m_nodes[ i ];
							break;
						}
					}
					else
					{
						selectedNode = m_nodes[ i ];
						break;
					}
				}
			}
			return selectedNode;
		}

		public void ResetNodesLocalVariables()
		{
			for ( int i = 0; i < m_nodes.Count; i++ )
			{
				m_nodes[ i ].Reset();
				m_nodes[ i ].ResetOutputLocals();
			}
		}

		public void ResetNodesLocalVariables( ParentNode node )
		{
			node.Reset();
			node.ResetOutputLocals();
			int count = node.InputPorts.Count;
			for ( int i = 0; i < count; i++ )
			{
				if ( node.InputPorts[ i ].IsConnected )
				{
					ResetNodesLocalVariables( m_nodesDict[ node.InputPorts[ i ].GetConnection().NodeId ] );
				}
			}
		}

		public override string ToString()
		{
			string dump = ( "Parent Graph \n" );
			for ( int i = 0; i < m_nodes.Count; i++ )
			{
				dump += ( m_nodes[ i ] + "\n" );
			}
			return dump;
		}

		public void OrderNodesByGraphDepth()
		{
			CurrentMasterNode.SetupNodeCategories();
			int count = m_nodes.Count;
			for ( int i = 0; i < count; i++ )
			{
				if ( m_nodes[ i ].ConnStatus == NodeConnectionStatus.Island )
				{
					m_nodes[ i ].CalculateCustomGraphDepth();
				}
			}

			m_nodes.Sort( ( x, y ) => { return y.GraphDepth.CompareTo( x.GraphDepth ); } );
		}

		public void WriteToString( ref string nodesInfo, ref string connectionsInfo )
		{
			for ( int i = 0; i < m_nodes.Count; i++ )
			{
				m_nodes[ i ].WriteToString( ref nodesInfo, ref connectionsInfo );
				m_nodes[ i ].WriteInputDataToString( ref nodesInfo );
				m_nodes[ i ].WriteOutputDataToString( ref nodesInfo );
				IOUtils.AddLineTerminator( ref nodesInfo );
			}
		}

		public void Reset()
		{
			SaveIsDirty = false;
			IsDirty = false;
		}

		public void OnBeforeSerialize()
		{
			DeSelectAll();
		}

		public void OnAfterDeserialize()
		{
			m_afterDeserializeFlag = true;
		}

		public void CleanCorruptedNodes()
		{
			for ( int i = 0; i < m_nodes.Count; i++ )
			{
				if ( ( object ) m_nodes[ i ] == null )
				{
					m_nodes.RemoveAt( i );
					CleanCorruptedNodes();
				}
			}
		}

		public ParentNode CreateNode( Type type, bool registerUndo, int nodeId = -1, bool addLast = true )
		{
			ParentNode newNode = ScriptableObject.CreateInstance( type ) as ParentNode;
			if ( newNode )
			{
				newNode.UniqueId = nodeId;
				AddNode( newNode, nodeId < 0, addLast, registerUndo );
			}
			return newNode;
		}

		public ParentNode CreateNode( Type type, bool registerUndo, Vector2 pos, int nodeId = -1, bool addLast = true )
		{
			ParentNode newNode = CreateNode( type, registerUndo, nodeId, addLast );
			if ( newNode )
			{
				newNode.Vec2Position = pos;
			}
			return newNode;
		}

		public void CreateNewEmpty( string name )
		{
			CleanNodes();
			MasterNode newMasterNode = CreateNode( m_masterNodeDefaultType, false ) as MasterNode;
			newMasterNode.SetName( name );
			m_masterNodeId = newMasterNode.UniqueId;
			newMasterNode.OnMaterialUpdatedEvent += OnMaterialUpdatedEvent;
			newMasterNode.OnShaderUpdatedEvent += OnShaderUpdatedEvent;
			newMasterNode.IsMainMasterNode = true;
		}

		public Vector2 SelectedNodesCentroid
		{
			get
			{
				if ( m_selectedNodes.Count == 0 )
					return Vector2.zero;
				Vector2 pos = new Vector2( 0, 0 );
				for ( int i = 0; i < m_selectedNodes.Count; i++ )
				{
					pos += m_selectedNodes[ i ].Vec2Position;
				}

				pos /= m_selectedNodes.Count;
				return pos;
			}
		}

		public void AddVirtualTextureCount()
		{
			m_virtualTextureCount += 1;
		}

		public void RemoveVirtualTextureCount()
		{
			m_virtualTextureCount -= 1;
			if ( m_virtualTextureCount < 0 )
			{
				Debug.LogWarning( "Invalid virtual texture count" );
			}
		}

		public bool HasVirtualTexture { get { return m_virtualTextureCount > 0; } }

		public void AddInstancePropertyCount()
		{
			m_instancePropertyCount += 1;
		}

		public void RemoveInstancePropertyCount()
		{
			m_instancePropertyCount -= 1;
			if ( m_instancePropertyCount < 0 )
			{
				Debug.LogWarning( "Invalid property instance count" );
			}
		}

		public bool IsInstancedShader { get { return m_instancePropertyCount > 0; } }

		public void AddNormalDependentCount() { m_normalDependentCount += 1; }

		public void RemoveNormalDependentCount()
		{
			m_normalDependentCount -= 1;
			if ( m_normalDependentCount < 0 )
			{
				Debug.LogWarning( "Invalid normal dependentCount count" );
			}
		}

		public bool IsNormalDependent { get { return m_normalDependentCount > 0; } }

		public void MarkToDeselect() { m_markedToDeSelect = true; }
		public void MarkToSelect( int nodeId ) { m_markToSelect = nodeId; }
		public void MarkWireHighlights() { m_checkSelectedWireHighlights = true; }
		public List<ParentNode> SelectedNodes { get { return m_selectedNodes; } }
		public List<ParentNode> MarkedForDeletionNodes { get { return m_markedForDeletion; } }
		public int CurrentMasterNodeId { get { return m_masterNodeId; } }

		public Shader CurrentShader
		{
			get
			{
				MasterNode masterNode = GetNode( m_masterNodeId ) as MasterNode;
				if ( masterNode != null )
					return masterNode.CurrentShader;
				return null;
			}
		}

		public Material CurrentMaterial
		{
			get
			{
				MasterNode masterNode = GetNode( m_masterNodeId ) as MasterNode;
				if ( masterNode != null )
					return masterNode.CurrentMaterial;
				return null;
			}
		}

		public MasterNode CurrentMasterNode { get { return GetNode( m_masterNodeId ) as MasterNode; } }
		public StandardSurfaceOutputNode CurrentStandardSurface { get { return GetNode( m_masterNodeId ) as StandardSurfaceOutputNode; } }
		public List<ParentNode> AllNodes { get { return m_nodes; } }
		public int NodeCount { get { return m_nodes.Count; } }
		public List<ParentNode> VisibleNodes { get { return m_visibleNodes; } }

		public int NodeClicked
		{
			set { m_nodeClicked = value; }
			get { return m_nodeClicked; }
		}

		public bool IsDirty
		{
			set { m_isDirty = value && UIUtils.DirtyMask; }
			get
			{
				bool value = m_isDirty;
				m_isDirty = false;
				return value;
			}
		}

		public bool SaveIsDirty
		{
			set { m_saveIsDirty = value && UIUtils.DirtyMask; }
			get
			{
				bool value = m_saveIsDirty;
				m_saveIsDirty = false;
				return value;
			}
		}
		public int LoadedShaderVersion
		{
			get { return m_loadedShaderVersion; }
			set { m_loadedShaderVersion = value; }
		}

		public bool HasUnConnectedNodes { get { return m_hasUnConnectedNodes; } }
		public NodeUsageRegister SamplerNodes { get { return m_samplerNodes; } }
		public NodeUsageRegister TexturePropertyNodes { get { return m_texturePropertyNodes; } }
		public NodeUsageRegister TextureArrayNodes { get { return m_textureArrayNodes; } }
		public NodeUsageRegister PropertyNodes { get { return m_propertyNodes; } }
		public NodeUsageRegister ScreenColorNodes { get { return m_screenColorNodes; } }
		public NodeUsageRegister LocalVarNodes { get { return m_localVarNodes; } }
		public PrecisionType CurrentPrecision
		{
			get { return m_currentPrecision; }
			set { m_currentPrecision = value; }
		}

		public NodeLOD LodLevel
		{
			get { return m_lodLevel; }
		}

		public List<ParentNode> NodePreviewList { get { return m_nodePreviewList; } set { m_nodePreviewList = value; } }
	}
}
