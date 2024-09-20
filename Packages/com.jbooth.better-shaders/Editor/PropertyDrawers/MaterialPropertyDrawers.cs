using UnityEngine;
using UnityEditor;
using System;
using System.Globalization;
using System.Collections.Generic;

namespace JBooth.BetterShaders
{
   internal class GroupRolloutDecorator : MaterialPropertyDrawer
   {
      internal static Dictionary<string, bool> sStates = new Dictionary<string, bool>();

      static GUIStyle headerStyle;
      private readonly string header;

      public GroupRolloutDecorator(string header)
      {
         this.header = header;
      }

      public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
      {
         return 24.0f;
      }

      public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
      {
         if (headerStyle == null)
         {
            headerStyle = new GUIStyle(GUI.skin.box);
            headerStyle.fontStyle = FontStyle.Bold;
            headerStyle.alignment = TextAnchor.MiddleLeft;
            headerStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
         }
         position.height -= 4;
         position.position += new Vector2(0, 2);
         position.width = 100;
         var oldColor = GUI.contentColor;
         GUI.contentColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
         if (GUI.Button(position, header, headerStyle))
         {
            if (sStates.ContainsKey(header))
            {
               sStates[header] = !sStates[header];
            }
            else
            {
               sStates[header] = false;
            }
         }
         
         GUI.contentColor = oldColor;
      }
   }

   internal class GroupFoldoutDecorator : MaterialPropertyDrawer
   {
      private readonly string header;

      public GroupFoldoutDecorator(string header)
      {
         this.header = header;
      }

      public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
      {
         return 24.0f;
      }

      public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
      {
         position.height -= 4;
         position.position += new Vector2(0, 2);
         position.width = 100;
         bool state;
         if (!GroupRolloutDecorator.sStates.TryGetValue(header, out state))
         {
            state = true;
         }
         bool nstate = EditorGUI.Foldout(position, state, header);
         if (nstate != state)
         {
            GroupRolloutDecorator.sStates[header] = nstate;
         }
      }
   }


   internal class GroupDrawer : MaterialPropertyDrawer
   {
      private readonly string header;

      public GroupDrawer(string header)
      {
         this.header = header;
      }

      public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
      {
         bool state;
         if (GroupRolloutDecorator.sStates.TryGetValue(header, out state))
         {
            if (state == false)
            {
               return 0;
            }
         }
         if (prop.type == MaterialProperty.PropType.Texture)
         {
            return 4 * base.GetPropertyHeight(prop, label, editor);
         }
         return base.GetPropertyHeight(prop, label, editor);
      }

      public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
      {
         position.width -= 18;
         position.position += new Vector2(18, 0);
         bool state;
         if (GroupRolloutDecorator.sStates.TryGetValue(header, out state))
         {
            if (state)
            { 
               editor.DefaultShaderProperty(position, prop, label);
            }
         }
         else
         {
            editor.DefaultShaderProperty(position, prop, label);
         }
      }
   }


   internal class MessageDecorator : MaterialPropertyDrawer
   {
      private readonly string header;

      public MessageDecorator(string header)
      {
         this.header = header;
      }

      public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
      {
         return 48.0f;
      }

      public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
      {
         EditorGUI.HelpBox(position, header, MessageType.Info);
      }
   }

   internal class ShowIfDrawer : MaterialPropertyDrawer
   {
      string propertyName;
      float exactValue = -1;
      public ShowIfDrawer(string p)
      {
         propertyName = p;
      }

      public ShowIfDrawer(string p, float value)
      {
         propertyName = p;
         exactValue = value;
      }

      internal static bool ShouldShow(MaterialProperty prop, string name, float range = -1)
      {
         bool show = true;
         foreach (var t in prop.targets)
         {
            var mat = t as Material;
            if (mat.HasProperty(name))
            {
               float v = mat.GetFloat(name);
               if (range < 0)
               {
                  if (v < 0.5f)
                  {
                     show = false;
                  }
               }
               else
               {
                  if (!(v > range - 0.1 && v < range + 0.1))
                  {
                     show = false;
                  }
               }
            }
         }
         return show;
      }

      public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
      {
         if (!ShouldShow(prop, propertyName, exactValue))
         {
            return 0;
         }
         if (prop.type == MaterialProperty.PropType.Texture)
         {
            return 4 * base.GetPropertyHeight(prop, label, editor);
         }
         return base.GetPropertyHeight(prop, label, editor);
      }

      public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
      {
         if (ShouldShow(prop, propertyName, exactValue))
         {
            if (prop.type == MaterialProperty.PropType.Texture)
            {
               editor.TextureProperty(position, prop, label);
            }
            else
            {
               editor.DefaultShaderProperty(position, prop, label);
            }
         }
      }
   }

   internal class Range01Drawer : MaterialPropertyDrawer
   {
      private GUIContent guiContent;
      private const string tooltip = "x:Lower, y:Upper";
      private const float labelWidth = 100f;

      public Range01Drawer()
      {
         guiContent = new GUIContent(string.Empty, tooltip);
      }

      public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor editor)
      {
         var previousLabelWidth = EditorGUIUtility.labelWidth;
         EditorGUIUtility.labelWidth = labelWidth;

         guiContent.text = label;
         var value = prop.vectorValue;
         var minValue = value.x;
         var maxValue = value.y;

         EditorGUI.BeginChangeCheck();
         EditorGUI.showMixedValue = prop.hasMixedValue;

         EditorGUI.MinMaxSlider(position, guiContent, ref minValue, ref maxValue, 0f, 1f);

         EditorGUI.showMixedValue = false;

         if (EditorGUI.EndChangeCheck())
         {
            value.x = minValue;
            value.y = maxValue;
            prop.vectorValue = value;
         }

         EditorGUIUtility.labelWidth = previousLabelWidth;
      }
   }


   internal class BetterHeaderDecorator : MaterialPropertyDrawer
   {
      static GUIStyle headerStyle;
      private readonly string header;

      public BetterHeaderDecorator(string header)
      {
         this.header = header;
      }

      // so that we can accept Header(1) and display that as text
      public BetterHeaderDecorator(float headerAsNumber)
      {
         this.header = headerAsNumber.ToString(CultureInfo.InvariantCulture);
      }

      public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
      {
         return 24.0f;
      }

      public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
      {
         if (headerStyle == null)
         {
            headerStyle = new GUIStyle(GUI.skin.box);
            headerStyle.fontStyle = FontStyle.Bold;
            headerStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
         }
         var oldColor = GUI.contentColor;
         GUI.contentColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
         //position = EditorGUI.IndentedRect(position);
         GUI.Label(position, header, headerStyle);
         GUI.contentColor = oldColor;
      }
   }



   internal class BetterHeaderToggleDrawer : MaterialPropertyDrawer
   {
      public BetterHeaderToggleDrawer()
      {
      }

      public BetterHeaderToggleDrawer(string keyword)
      {
      }

      protected virtual void SetKeyword(MaterialProperty prop, bool on)
      {
      }

      static bool IsPropertyTypeSuitable(MaterialProperty prop)
      {
         return prop.type == MaterialProperty.PropType.Float || prop.type == MaterialProperty.PropType.Range;
      }

      public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
      {
         return 24;
      }

      static GUIStyle headerStyle;

      public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
      {
         if (!IsPropertyTypeSuitable(prop))
         {
            Debug.LogError("[BetterToggleHeader] is not on a float property");
            return;
         }

         if (headerStyle == null)
         {
            headerStyle = new GUIStyle(GUI.skin.box);
            headerStyle.fontStyle = FontStyle.Bold;
            headerStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
         }

         EditorGUI.BeginChangeCheck();

         bool value = (Math.Abs(prop.floatValue) > 0.001f);
         EditorGUI.showMixedValue = prop.hasMixedValue;

         var oldColor = GUI.contentColor;
         GUI.contentColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
         GUI.Label(position, label, headerStyle);
         position.position = new Vector2(position.position.x+5, position.position.y);
         value = EditorGUI.Toggle(position, value);

         GUI.contentColor = oldColor;

         EditorGUI.showMixedValue = false;
         if (EditorGUI.EndChangeCheck())
         {
            prop.floatValue = value ? 1.0f : 0.0f;
            SetKeyword(prop, value);
         }
      }

      public override void Apply(MaterialProperty prop)
      {
         base.Apply(prop);
         if (!IsPropertyTypeSuitable(prop))
            return;

         if (prop.hasMixedValue)
            return;

         SetKeyword(prop, (Math.Abs(prop.floatValue) > 0.001f));
      }
   }


   internal class BetterHeaderToggleKeywordDrawer : BetterHeaderToggleDrawer
   {
      static GUIStyle headerStyle;
      protected readonly string keyword;
      public BetterHeaderToggleKeywordDrawer()
      {
      }

      public BetterHeaderToggleKeywordDrawer(string keyword)
      {
         this.keyword = keyword;
      }

      protected override void SetKeyword(MaterialProperty prop, bool on)
      {
         SetKeywordInternal(prop, on);
      }

      protected void SetKeywordInternal(MaterialProperty prop, bool on)
      {
         foreach (Material material in prop.targets)
         {
            if (on)
               material.EnableKeyword(keyword);
            else
               material.DisableKeyword(keyword);
         }
      }
   }


   internal class Vec2Drawer : MaterialPropertyDrawer
   {
      internal static Vector2 DrawVector2(Rect rect, string label, Vector2 val)
      {
         Rect r1 = new Rect(rect);
         r1.width = r1.width / 4;
         EditorGUI.PrefixLabel(r1, new GUIContent(label));
         r1.x += r1.width;
         r1.width *= 3;
         return EditorGUI.Vector2Field(r1, "", val);
      }

      // Draw the property inside the given rect
      public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor editor)
      {
         Vector4 value = prop.vectorValue;

         EditorGUI.BeginChangeCheck();
         EditorGUI.showMixedValue = prop.hasMixedValue;

         value = DrawVector2(position, label, value);

         EditorGUI.showMixedValue = false;
         if (EditorGUI.EndChangeCheck())
         {
            // Set the new value if it has changed
            prop.vectorValue = value;
         }
      }
   }


   internal class Vec2SplitDrawer : MaterialPropertyDrawer
   {
      string label1;
      string label2;
      public Vec2SplitDrawer(string l1, string l2)
      {
         label1 = l1;
         label2 = l2;
      }
      public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
      {
         return 2 * base.GetPropertyHeight(prop, label, editor);
      }


      // Draw the property inside the given rect
      public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor editor)
      {
         Rect r1 = position;
         Rect r2 = position;
         r2.height *= 0.5f;
         r2.y += r2.height;

         Vector4 value = prop.vectorValue;
         Vector2 v1 = new Vector2(value.x, value.y);
         Vector2 v2 = new Vector2(value.z, value.w);

         EditorGUI.BeginChangeCheck();
         EditorGUI.showMixedValue = prop.hasMixedValue;

         v1 = Vec2Drawer.DrawVector2(r1, label1, v1);
         v2 = Vec2Drawer.DrawVector2(r2, label2, v2);

         EditorGUI.showMixedValue = false;
         if (EditorGUI.EndChangeCheck())
         {
            prop.vectorValue = new Vector4(v1.x, v1.y, v2.x, v2.y);
         }
      }

      internal class MiniTextureDrawer : MaterialPropertyDrawer
      {
         string label1;
         string label2;

         // Draw the property inside the given rect
         public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor editor)
         {

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;

            Texture tex = editor.TexturePropertyMiniThumbnail(position, prop, label, "");

            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
               prop.textureValue = tex;
            }
         }
      }
   }
}

