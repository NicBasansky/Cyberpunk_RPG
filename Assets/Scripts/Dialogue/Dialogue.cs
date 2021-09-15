using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "Dialogue", menuName = "New Dialogue", order = 0)]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField]
        List<DialogueNode> nodes = new List<DialogueNode>();
        [SerializeField]
        Vector2 newNodeOffset = new Vector2(220f, 0);

        Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();

#if UNITY_EDITOR
     
        public void CreateNewNode(DialogueNode parentNode)
        {
            DialogueNode newNode = MakeNode(parentNode);
            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");
            Undo.RecordObject(this, "Created Dialogue Node");
            AddNode(newNode);
        }

        private void AddNode(DialogueNode newNode)
        {
            nodes.Add(newNode);
            OnValidate();
        }

        private DialogueNode MakeNode(DialogueNode parentNode)
        {
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.name = System.Guid.NewGuid().ToString();
            if (parentNode != null)
            {
                //newNode.SetIsPlayerSpeaking(!parentNode.GetIsPlayerSpeaking());
                newNode.SetIsPlayerSpeaking(false);
                parentNode.AddToChildren(newNode.name);
                newNode.SetPosition(parentNode.GetNodeRect().position + newNodeOffset);
                newNode.SetParentNode(parentNode);
            }
            return newNode;
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this, "Deleted Dialougue Node");
            nodes.Remove(nodeToDelete);
            CleanDanglingChildren(nodeToDelete);
            OnValidate();
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        private void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                node.RemoveFromChildren(nodeToDelete.name);
            }
        }

#endif

        private void OnValidate() 
        {
            nodeLookup.Clear();

            if (GetAllNodes() == null) return; 

            foreach(DialogueNode node in GetAllNodes())
            {
                if (node == null) return;
                
                nodeLookup[node.name] = node;
            }
        }

        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return nodes;
        }

        public DialogueNode GetRootNode()
        { 
            return nodes[0];
        }

        public IEnumerable<DialogueNode> GetPlayerChildren(DialogueNode parentNode)
        {
            foreach (DialogueNode node in GetAllChildren(parentNode))
            {
                if (node.GetIsPlayerSpeaking())
                {
                    yield return node;
                }
            }
        }

        public IEnumerable<DialogueNode> GetAIChildren(DialogueNode parentNode)
        {
            foreach (DialogueNode child in GetAllChildren(parentNode))
            {
                if (!child.GetIsPlayerSpeaking())
                {
                    yield return child;
                }
            }
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            foreach (string childID in parentNode.GetChildren())
            {
                if (nodeLookup.ContainsKey(childID))
                {
                    yield return nodeLookup[childID];
                }
            }
        }

        // TODO might not need at all
        public IEnumerable<DialogueNode> GetImmediatePlayerSiblingNodes(DialogueNode parentNode)
        {
            foreach (string childID in parentNode.GetChildren())
            {
                if (nodeLookup.ContainsKey(childID))
                {
                    if (nodeLookup[childID].GetParentNode() == parentNode && nodeLookup[childID].GetIsPlayerSpeaking())
                    {
                        yield return nodeLookup[childID];
                    }
                }
            }
            foreach (DialogueNode child in GetPlayerChildren(parentNode))
            {
                if (child.GetParentNode() == parentNode)
                {
                    yield return child;
                }
            }
        }

        public void OnBeforeSerialize()
       {
#if UNITY_EDITOR
            if (nodes.Count == 0)
            {
                DialogueNode newNode = MakeNode(null);
                AddNode(newNode);
            }
            if (AssetDatabase.GetAssetPath(this) != "")
            {
                foreach (DialogueNode node in GetAllNodes())
                {
                    if (AssetDatabase.GetAssetPath(node) == "")
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize()
        {
            // leave empty
        }
    }
}
