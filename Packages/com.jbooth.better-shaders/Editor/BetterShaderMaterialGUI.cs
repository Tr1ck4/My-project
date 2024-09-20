//////////////////////////////////////////////////////
// Better Shaders
// Copyright (c) Jason Booth
//////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace JBooth.BetterShaders
{

   public class SubShaderMaterialEditor
   {
      public virtual void OnGUI(MaterialEditor materialEditor,
         ShaderGUI shaderGUI,
         MaterialProperty[] props,
         Material mat)
      {

      }

      public int stackIndex = -1;
      protected MaterialProperty FindProp(string name, MaterialProperty[] props)
      {
         if (stackIndex >= 0)
         {
            string stripped = "";
            if (name.EndsWith("_ST"))
            {
               name = name.Substring(0, name.Length - 3);
               stripped = "_ST";
            }
            if (name.EndsWith("_TexelSize"))
            {
               name = name.Substring(0, name.Length - 10);
               stripped = "_TexelSize";
            }
            if (name.EndsWith("_HDR"))
            {
               name = name.Substring(0, name.Length - 4);
               stripped = "_HDR";
            }

            name += "_Ext_" + stackIndex + stripped;
            
         }
         foreach (var p in props)
         {
            if (p.name == name)
               return p;
         }
         return null;
      }


      protected T EnumPopup<T>(GUIContent label, MaterialProperty prop) where T : System.Enum
      {
         T e = default(T);
         int v = (int)prop.floatValue;
         e = (T)(object)v;
         EditorGUI.BeginChangeCheck();
         e = (T)EditorGUILayout.EnumPopup(label, e);
         if (EditorGUI.EndChangeCheck())
         {
            prop.floatValue = System.Convert.ToInt32(e);
         }
         return e;
      }

      protected string GetKeywordName(string keyword)
      {
         if (stackIndex >= 0)
         {
            keyword += "_DEF_" + stackIndex;
         }
         return keyword;
      }


      static Dictionary<string, bool> sRolloutStates = new Dictionary<string, bool>();
      static GUIStyle rolloutHeaderStyle;

      static void InitHeaderStyle()
      {
         if (rolloutHeaderStyle == null)
         {
            rolloutHeaderStyle = new GUIStyle(GUI.skin.box);
            rolloutHeaderStyle.fontStyle = FontStyle.Bold;
            rolloutHeaderStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
         }
      }

      public static bool DrawRollout(string name)
      {
         InitHeaderStyle();
         if (!sRolloutStates.ContainsKey(name))
         {
            sRolloutStates[name] = true;
         }
         if (GUILayout.Button(name, GUILayout.Height(24)))
         {
            sRolloutStates[name] = !sRolloutStates[name];
         }
         return sRolloutStates[name];
      }

      public static void DrawHeader(string name)
      {
         InitHeaderStyle();
         GUILayout.Label(name, rolloutHeaderStyle, GUILayout.Height(24), GUILayout.ExpandWidth(true));
      }

   }

   public partial class BetterShaderMaterialGUI : ShaderGUI
   {

      SubShaderMaterialEditor CreateEditorInstance(System.Type type)
      {
         var instance = System.Activator.CreateInstance(type);
         return instance as SubShaderMaterialEditor; 
      }

      public class Chunk
      {
         public SubShaderMaterialEditor editor;
         public int stackIndex = -1;
         public List<MaterialProperty> properties = new List<MaterialProperty>();
      }

      List<Chunk> entries = new List<Chunk>();

      void InitEditors(MaterialProperty[] props, Material targetMat)
      {
         if (props.Length == 0)
            return;
         entries = new List<Chunk>();
         var path = AssetDatabase.GetAssetPath(targetMat.shader);
         var all = AssetDatabase.LoadAllAssetsAtPath(path);
         SubMaterialList subList = null;
         foreach (var a in all)
         {
            if (a is TextAsset)
            {
               subList = JsonUtility.FromJson<SubMaterialList>(((TextAsset)a).text);
            }
         }

         if (subList == null)
         {
            Debug.Log("Sublist not found in shader");
            return;
         }

         var types = System.AppDomain.CurrentDomain.GetAssemblies()
               .SelectMany(s => s.GetTypes())
               .Where(t => t.IsSubclassOf(typeof(SubShaderMaterialEditor)) && t != typeof(SubShaderMaterialEditor));

         Chunk curChunk = new Chunk();
         curChunk.stackIndex = -1;
         entries.Add(curChunk);
         foreach (var p in props)
         {
            // find matching submaterial
            SubShaderMaterial subMat = null;
            foreach (var sm in subList.materials)
            {
               if (sm.properties.Contains(p.name))
               {
                  subMat = sm;
               }
            }

            if (subMat == null)
            {
               if (curChunk.editor == null)
               {
                  curChunk.properties.Add(p);
               }
               else
               {
                  curChunk = new Chunk();
                  curChunk.stackIndex = -1;
                  curChunk.properties.Add(p);
                  entries.Add(curChunk);
               }
            }
            else // have submat
            {
               if (curChunk.editor == null)
               {
                  if (string.IsNullOrEmpty(subMat.editor))
                  {
                     curChunk.properties.Add(p);
                  }
                  else
                  {
                     bool found = false;
                     foreach (var t in types)
                     {
                        string en = subMat.editor.Trim();
                        if (!found && t.Name == en)
                        {
                           curChunk = new Chunk();
                           entries.Add(curChunk);
                           curChunk.editor = CreateEditorInstance(t);
                           curChunk.stackIndex = subMat.stackIndex;
                           curChunk.properties.Add(p);
                           found = true;
                           break;
                        }
                     }
                     if (!found)
                     {
                        Debug.LogWarning("Could not find sub editor " + subMat.editor);
                        if (curChunk.editor == null)
                        {
                           curChunk.properties.Add(p);
                        }
                        else
                        {
                           curChunk = new Chunk();
                           entries.Add(curChunk);
                           curChunk.stackIndex = subMat.stackIndex;
                           curChunk.properties.Add(p);
                        }
                     }
                  }
               }
               else
               {
                  string sm = subMat.editor.Trim();
                  if (curChunk.editor.GetType().Name == sm && curChunk.stackIndex == subMat.stackIndex)
                  {
                     curChunk.properties.Add(p);
                  }
                  else
                  {
                     curChunk = new Chunk();
                     entries.Add(curChunk);
                     curChunk.properties.Add(p);
                     curChunk.stackIndex = subMat.stackIndex;
                     if (!string.IsNullOrEmpty(sm))
                     {
                        bool found = false;
                        foreach (var t in types)
                        {
                           if (!found && t.Name == sm)
                           {
                              curChunk.editor = CreateEditorInstance(t);
                              found = true;
                              break;
                           }
                        }
                        if (!found)
                        {
                           Debug.LogWarning("Could not find sub editor " + subMat.editor);
                        }
                     }
                  }
               }
            }
         }
         
         
      }

      public override void OnClosed(Material m)
      {
         entries.Clear();
      }

      public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
      {
         var oldLabelWidth = EditorGUIUtility.labelWidth;
        
         var targetMat = materialEditor.target as Material;
         if (targetMat.shader == null)
         {
            base.OnGUI(materialEditor, props);
            return;
         }
         if (entries == null || entries.Count == 0)
         {
            InitEditors(props, targetMat);
         }
         
         if (entries.Count == 0)
         {
            base.OnGUI(materialEditor, props);
            return;
         }
         float oldFieldWith = EditorGUIUtility.fieldWidth;
         foreach (var e in entries)
         {
            if (e.editor == null)
            {
               if (e.properties != null && e.properties.Count > 0)
               {
                  materialEditor.SetDefaultGUIWidths();
                  foreach ( var p in e.properties)
                  {
                     if (!p.flags.HasFlag(MaterialProperty.PropFlags.HideInInspector))
                     {
                        materialEditor.ShaderProperty(p, p.name);
                     }
                  }
                  EditorGUIUtility.labelWidth = oldLabelWidth;
                  EditorGUIUtility.fieldWidth = oldFieldWith;

               }
            }
            else
            {
               EditorGUIUtility.labelWidth = oldLabelWidth;
               e.editor.stackIndex = e.stackIndex;
               e.editor.OnGUI(materialEditor, this, e.properties.ToArray(), targetMat);
            }
         }
         SubShaderMaterialEditor.DrawHeader("Unity");
         if (UnityEngine.Rendering.SupportedRenderingFeatures.active.editableMaterialRenderQueue)
            materialEditor.RenderQueueField();
         materialEditor.EnableInstancingField();
         materialEditor.DoubleSidedGIField();
         
      }


   }
}
