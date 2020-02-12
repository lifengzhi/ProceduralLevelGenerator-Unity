﻿using System.Collections.Generic;
using System.Linq;
using Assets.ProceduralLevelGenerator.Scripts.Generators.Common.RoomTemplates;
using Assets.ProceduralLevelGenerator.Scripts.Generators.Common.Utils;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.ProceduralLevelGenerator.Scripts.Utils
{
    public class RoomTemplateConverter : MonoBehaviour
    {
        public void Convert()
        {
            var grid = gameObject.GetComponent<Grid>();
            if (grid != null)
            {
                DestroyImmediate(grid, true);
            }

            if (gameObject.GetComponent<RoomTemplate>() == null)
            {
                gameObject.AddComponent<RoomTemplate>();
            }

            if (transform.Find(GeneratorConstants.TilemapsRootName) != null)
            {
                var oldRoot = transform.Find(GeneratorConstants.TilemapsRootName).gameObject;

                foreach (var childTransform in oldRoot.transform.Cast<Transform>().ToList())
                {
                    var tilemap = childTransform.GetComponent<Tilemap>();

                    if (tilemap != null)
                    {
                        childTransform.parent = transform;
                    }
                }

                DestroyImmediate(oldRoot);
            }

            // Create tilemaps root
            var tilemapsRoot = new GameObject(GeneratorConstants.TilemapsRootName);
            tilemapsRoot.AddComponent<Grid>();
            tilemapsRoot.transform.parent = gameObject.transform;
            var tilemaps = new List<Tilemap>();

            foreach (var childTransform in transform.Cast<Transform>().ToList())
            {
                var tilemap = childTransform.GetComponent<Tilemap>();

                if (tilemap != null)
                {
                    tilemaps.Add(tilemap);
                    var tilemapRenderer = childTransform.GetComponent<TilemapRenderer>();

                    if (tilemap.name == "Floor")
                    {
                        tilemapRenderer.sortingOrder = 0;
                    }
                    if (tilemap.name == "Walls")
                    {
                        tilemapRenderer.sortingOrder = 1;
                    }
                }
            }

            foreach (var tilemap in tilemaps.OrderBy(x => x.GetComponent<TilemapRenderer>().sortingOrder))
            {
                tilemap.transform.parent = tilemapsRoot.transform;
            }

            // Fix positions
            tilemapsRoot.transform.localPosition = Vector3.zero;
            transform.localPosition = Vector3.zero;
        }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(RoomTemplateConverter))]
    public class RoomTemplateConverterInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            var converter = (RoomTemplateConverter) target;

            DrawDefaultInspector();

            if (GUILayout.Button("Convert"))
            {
                converter.Convert();
                EditorUtility.SetDirty(converter.gameObject);
                DestroyImmediate(converter);
            }
        }
    }
    #endif
}