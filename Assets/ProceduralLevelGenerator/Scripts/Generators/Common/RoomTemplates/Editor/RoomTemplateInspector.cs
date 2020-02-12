﻿using System;
using System.Linq;
using Assets.ProceduralLevelGenerator.Scripts.Generators.Common.RoomTemplates.TilemapLayers;
using Assets.ProceduralLevelGenerator.Scripts.Generators.Common.Utils;
using Assets.ProceduralLevelGenerator.Scripts.Utils;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

namespace Assets.ProceduralLevelGenerator.Scripts.Generators.Common.RoomTemplates.Editor
{
    [CustomEditor(typeof(RoomTemplate))]
    public class RoomTemplateInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var roomTemplate = (RoomTemplate) target;

            if (roomTemplate.IsOutlineValid())
            {
                EditorGUILayout.HelpBox("The outline of the room template is valid.", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("The outline of the room template is not valid. Please make sure to follow the rules from the documentation.", MessageType.Error);
            }

            var hasOutlineOverride = roomTemplate.HasOutlineOverride();

            EditorGUILayout.HelpBox($"Using outline override: {hasOutlineOverride}", MessageType.None);

            if (hasOutlineOverride)
            {
                if (GUILayout.Button("Remove outline override", EditorStyles.miniButton))
                {
                    roomTemplate.RemoveOutlineOverride();
                }
            }
            else
            {
                if (GUILayout.Button("Add outline override", EditorStyles.miniButton))
                {
                    roomTemplate.AddOutlineOverride();
                }
            }
        }

        public void OnSceneGUI()
        {
            SceneView.duringSceneGui -= OnSceneGUITest;

            if (PrefabStageUtility.GetCurrentPrefabStage() != null)
            {
                SceneView.duringSceneGui += OnSceneGUITest;
            }
        }

        public void OnSceneGUITest(SceneView sceneView)
        {
            if (target == null)
            {
                SceneView.duringSceneGui -= OnSceneGUITest;
                return;
            }

            if (PrefabStageUtility.GetCurrentPrefabStage() == null)
            {
                SceneView.duringSceneGui -= OnSceneGUITest;
                return;
            }
            
            try
            {
                var roomTemplate = (RoomTemplate) target;
                var tilemaps = RoomTemplateUtils.GetTilemaps(roomTemplate.gameObject);
                var polygon = RoomTemplatesLoader.GetPolygonFromTilemaps(tilemaps);
                var points = polygon.GetPoints();

                Handles.color = Color.yellow;
                for (int i = 0; i < points.Count; i++)
                {
                    var point1 = new Vector3(points[i].X, points[i].Y, -1);
                    var point2 = new Vector3(points[(i + 1) % points.Count].X, points[(i + 1) % points.Count].Y, -1);
                    // Handles.DrawLine(point1 + new Vector3(0.5f, 0.5f), point2 + new Vector3(0.5f, 0.5f));
                    DrawOutline(point1, point2, Color.green);
                }
            }
            catch (ArgumentException)
            {

            }
        }

        private void DrawOutline(Vector3 from, Vector3 to, Color outlineColor, bool drawDiagonal = true)
        {
            var roomTemplate = (RoomTemplate) target;
            from = from + roomTemplate.transform.position;
            to = to + roomTemplate.transform.position;

            if (from.x == to.x || from.y == to.y)
            {
                if (to.x < from.x)
                {
                    from.x += 1;
                }

                if (to.y < from.y)
                {
                    from.y += 1;
                }

                if (to.x >= from.x)
                {
                    to.x += 1;
                }

                if (to.y >= from.y)
                {
                    to.y += 1;
                }
            }
            else
            {
                to = from + new Vector3(1, 1);
            }

            Handles.DrawSolidRectangleWithOutline(new Rect(from, to - from), Color.clear, Color.yellow);
        }
    }
}