///////////////////////////////////////////
///
/// Better Shaders
/// ©2021 Jason Booth
/// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_2020_2_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif
using System.IO;
using JBooth.BetterShaders;

namespace JBooth.BetterShaders
{
#if __BETTERSHADERS__
   [CustomEditor(typeof(StackedShaderImporter))]
   public class StackedShaderImporterEditor : ScriptedImporterEditor
   {
      SerializedProperty propStack;
      SerializedProperty propOptionOverrides;

      // override extraDataType to return the type that will be used in the Editor.
      protected override System.Type extraDataType => typeof(StackedShader);

      // override InitializeExtraDataInstance to set up the data.
      protected override void InitializeExtraDataInstance(Object extraTarget, int targetIndex)
      {
         var stack = (StackedShader)extraTarget;

         string fileContent = File.ReadAllText(((AssetImporter)targets[targetIndex]).assetPath);
         EditorJsonUtility.FromJsonOverwrite(fileContent, stack);
      }

      protected override void Apply()
      {
         base.Apply();
         // After the Importer is applied, rewrite the file with the custom value.
         for (int i = 0; i < targets.Length; i++)
         {
            string path = ((AssetImporter)targets[i]).assetPath;
            File.WriteAllText(path, EditorJsonUtility.ToJson((StackedShader)extraDataTargets[i]));
         }
      }

      UnityEditorInternal.ReorderableList shaderList = null;

      public override void OnEnable()
      {
         base.OnEnable();
         // In OnEnable, retrieve the importerUserSerializedObject property and store it.
         propStack = extraDataSerializedObject.FindProperty("shaders");
         propOptionOverrides = extraDataSerializedObject.FindProperty("optionOverrides");
         
         // Initialises the ReorderableList. We are creating a Reorderable List from the "wave" property. 
         // In this, we want a ReorderableList that is draggable, with a display header, with add and remove buttons        
         shaderList = new UnityEditorInternal.ReorderableList(serializedObject, propStack, true, true, true, true);

         shaderList.drawElementCallback = DrawListItems; // Delegate to draw the elements on the list
         shaderList.drawHeaderCallback = DrawHeader; // Skip this line if you set displayHeader to 'false' in your ReorderableList constructor.

      }

      
      // Draws the elements on the list
      void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
      {
         SerializedProperty element = shaderList.serializedProperty.GetArrayElementAtIndex(index); // The element in the list
         EditorGUI.PropertyField(rect, element);
      }

      //Draws the header
      void DrawHeader(Rect rect)
      {
         string name = "Shader List";
         EditorGUI.LabelField(rect, name);
      }

      public static string BuildExportShader(ShaderBuilder.RenderPipeline renderPipeline, OptionOverrides overrides, string assetPath)
      {
         var stack = AssetDatabase.LoadAssetAtPath<StackedShader>(assetPath);
         if (stack == null)
         {
            stack = StackedShader.CreateInstance<StackedShader>();
            EditorJsonUtility.FromJsonOverwrite(File.ReadAllText(assetPath), stack);
         }
         // Read all blocks
         ShaderBlockReader blocks;
         var text = StackedShaderImporter.BuildShaderString(stack, assetPath, out blocks, renderPipeline, overrides != null ? overrides : stack.optionOverrides);
         return text;
      }

      public override void OnInspectorGUI()
      {
         extraDataSerializedObject.Update();

         shaderList.DoLayoutList();
         EditorGUILayout.PropertyField(propOptionOverrides);
         
         extraDataSerializedObject.ApplyModifiedProperties();

         ApplyRevertGUI();


         BetterShaderImporterEditor.renderPipeline = (ShaderBuilder.RenderPipeline)EditorGUILayout.EnumPopup("Export Pipeline", BetterShaderImporterEditor.renderPipeline);
         if (GUILayout.Button("Export Shader"))
         {
            try
            {
               var assetPath = AssetDatabase.GetAssetPath(target);
               var stack = (StackedShader)extraDataSerializedObject.targetObject;
               // Read all blocks
               ShaderBlockReader blocks;
               var text = StackedShaderImporter.BuildShaderString(stack, AssetDatabase.GetAssetPath(stack), out blocks, BetterShaderImporterEditor.renderPipeline, stack.optionOverrides);

               assetPath = assetPath.Replace(StackedShaderImporter.k_FileExtension, "_" + BetterShaderImporterEditor.renderPipeline.ToString() + ".shader");
               File.WriteAllText(assetPath, text);
               AssetDatabase.Refresh();

            }
            catch (System.Exception e)
            {
               Debug.LogException(e);
            }
         }
      }
   }
#endif

}