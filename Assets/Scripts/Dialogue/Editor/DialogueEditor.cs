using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        Dialogue selectedDialogue = null;
        [NonSerialized]
        GUIStyle nodeStyle;
        [NonSerialized]
        GUIStyle playerNodeStyle;
        [NonSerialized]
        GUIStyle labelStyle;
        [NonSerialized]
        GUIStyle textAreaStyle;
        [NonSerialized]
        GUILayoutOption[] textAreaOptions;
        [NonSerialized]
        DialogueNode draggingNode;
        [NonSerialized]
        Vector2 draggingOffset;
        [NonSerialized]
        DialogueNode creatingNode = null;
        [NonSerialized]
        DialogueNode deletingNode = null;
        [NonSerialized]
        DialogueNode parentLinkingNode = null;
        [NonSerialized]
        DialogueNode childLinkingNode = null;
        Vector2 scrollPosition;
        [NonSerialized]
        bool draggingCanvas = false;
        [NonSerialized]
        Vector2 draggingCanvasOffset;
        [NonSerialized]
        int textAreaHeight = 50;


        const float backgroundSize = 50;
        const int canvasSize = 4000;

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAssetAttribute(1)]
        public static bool OpenAsset(int instanceID, int line)
        {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            if (dialogue != null)
            {
                ShowEditorWindow();
                return false; // false because we will handle the window opening
            }
            return true; // we will not handle the window opening
        }

        private void OnEnable() 
        {
            Selection.selectionChanged += OnSelectionChanged; // every time something is selected in the editor

            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            nodeStyle.border = new RectOffset(12, 12, 12, 12);
            //nodeStyle.wordWrap = true;

            playerNodeStyle = new GUIStyle();
            playerNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            playerNodeStyle.padding = new RectOffset(20, 20, 20, 20);
            playerNodeStyle.border = new RectOffset(12, 12, 12, 12);
            //nodeStyle.wordWrap = true;

            SetUpTextAreaOptions();

            OnSelectionChanged();
        }

        private void SetUpTextAreaOptions()
        {
            

            textAreaOptions = new GUILayoutOption[2];
            textAreaOptions[0] = GUILayout.MaxWidth(160);
            textAreaOptions[1] = GUILayout.ExpandHeight(true);
            
        }

        private void OnSelectionChanged()
        {
            Dialogue dialogue = Selection.activeObject as Dialogue;
            if (dialogue != null)
            {
                selectedDialogue = dialogue;
                Repaint();
            }
        }

        private void OnGUI() 
        {
            if (selectedDialogue == null)
            {
                EditorGUILayout.LabelField("No dialogue selected.");
            }
            else
            {
                ProcessEvents();

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                Rect canvasRect = GUILayoutUtility.GetRect(canvasSize, canvasSize);

                Texture2D background = Resources.Load("backgroundGrid") as Texture2D;
                Rect texCoords = new Rect(0, 0, canvasSize / backgroundSize, canvasSize / backgroundSize);
                GUI.DrawTextureWithTexCoords(canvasRect, background, texCoords);

                foreach (DialogueNode node in selectedDialogue.GetAllNodes()) // Doing separately prevents connections overlapping with nodes
                {
                    DrawConnections(node);                   
                }
                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                { 
                    DrawNode(node);
                }

                EditorGUILayout.EndScrollView();

                if (creatingNode != null)
                {
                    selectedDialogue.CreateNewNode(creatingNode);
                    creatingNode = null;
                }
                if (deletingNode != null)
                {
                    selectedDialogue.DeleteNode(deletingNode);
                    deletingNode = null;
                }
            }
        }

        private void DrawConnections(DialogueNode node)
        {
            Vector3 startPos = new Vector2(node.GetNodeRect().xMax, node.GetNodeRect().center.y);
            foreach(DialogueNode childNode in selectedDialogue.GetAllChildren(node))
            {
                Vector3 endPos = new Vector2(childNode.GetNodeRect().xMin, childNode.GetNodeRect().center.y);
                Vector3 nodeDistance = endPos - startPos;
                Vector3 controlPointOffset = new Vector2(nodeDistance.x * .60f, 0); // 20% of the node distance
                Handles.DrawBezier(startPos, endPos, 
                    startPos + controlPointOffset, 
                    endPos - controlPointOffset, 
                    Color.white, null, 4f);
            }
        }

        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && draggingNode == null)
            {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);
                if (draggingNode != null)
                {
                    draggingOffset = draggingNode.GetNodeRect().position - Event.current.mousePosition;
                    Selection.activeObject = draggingNode;
                }
                else
                { 
                    draggingCanvasOffset = Event.current.mousePosition + scrollPosition;
                    draggingCanvas = true;
                    Selection.activeObject = selectedDialogue;
                }  
            }
            else if (Event.current.type == EventType.MouseDrag && draggingNode != null)
            {         
                draggingNode.SetPosition(Event.current.mousePosition + draggingOffset);
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseDrag && draggingCanvas)
            {
                ScrollByMousePosition();
            }
            else if (Event.current.type == EventType.MouseUp && draggingNode != null)
            {
                draggingNode = null;
            }
            else if (Event.current.type == EventType.MouseUp && draggingCanvas)
            {
                draggingCanvas = false;
            }
        }

        private void ScrollByMousePosition()
        {               
            scrollPosition = draggingCanvasOffset - Event.current.mousePosition;
            GUI.changed = true;
        }

        private DialogueNode GetNodeAtPoint(Vector2 point)
        {
            DialogueNode foundNode = null;
            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                if (node.GetNodeRect().Contains(point))
                {
                    foundNode = node;
                }
            }
            return foundNode;
        }

        private void DrawNode(DialogueNode node)
        {
            GUIStyle style = nodeStyle;
            if (node.GetIsPlayerSpeaking())
            {
                style = playerNodeStyle;
            }
            
            GUILayout.BeginArea(node.GetNodeRect(), style);

            if (node.GetIsPlayerSpeaking())
            {
                // simple question over choices
                GUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Q:", GUILayout.MaxWidth(20f));
                string simpleQuestion = EditorGUILayout.TextField(node.GetSimpleQuestion());
                node.SetSimpleQuestion(simpleQuestion);

                if (node.GetParentNode() != null)
                {
                    foreach (DialogueNode sibling in selectedDialogue.GetPlayerChildren(node.GetParentNode()))//GetImmediatePlayerSiblingNodes(node.GetParentNode()))
                    {
                        {
                            sibling.SetSimpleQuestion(simpleQuestion);
                        }
                    }
                }
                //GUILayout.ExpandHeight(true);
                GUILayout.EndHorizontal();
            }

            //GUILayout.BeginHorizontal();
            //GUILayout.BeginArea(new Rect(0, 0, 160, 130));
            textAreaStyle = new GUIStyle(EditorStyles.textArea);
            textAreaStyle.wordWrap = true;

            node.SetText(EditorGUILayout.TextArea(
                    node.GetText(), textAreaStyle, textAreaOptions));
 
            //GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                creatingNode = node;
            }

            DrawLinkButtons(node);

            if (GUILayout.Button("-"))
            {
                deletingNode = node;
            }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void DrawLinkButtons(DialogueNode node)
        {
            if (parentLinkingNode == null)
            {
                if (GUILayout.Button("Link"))
                {
                    parentLinkingNode = node;
                }
            }
            else if (parentLinkingNode == node) // if we are trying to child to itself
            {
                if (GUILayout.Button("Cancel"))
                {
                    parentLinkingNode = null;
                }
            }
            else if (parentLinkingNode.GetChildren().Contains(node.name))
            {
                if (GUILayout.Button("Unlink"))
                {
                    parentLinkingNode.RemoveFromChildren(node.name);
                    node.SetParentNode(null); // TODO remove?
                    parentLinkingNode = null;
                }
            }
            else 
            {
                if (GUILayout.Button("child"))
                {
                    parentLinkingNode.AddToChildren(node.name);
                    node.SetParentNode(parentLinkingNode);
                    parentLinkingNode = null;
                }
            }          
        }
    }
}

