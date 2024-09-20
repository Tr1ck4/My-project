///////////////////////////////////////////
///
/// Better Shaders
/// ©2021 Jason Booth
/// 

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;

namespace JBooth.BetterShaders
{
#if __BETTERSHADERS__


   [System.Serializable]
   public class SubShaderMaterial
   {
      public string editor;
      public int stackIndex;
      public List<string> properties;
   }

   [System.Serializable]
   public class SubMaterialList
   {
      public SubShaderMaterial[] materials;
   }


   public class BetterShaderUtility
   {
      string[] paths;

      void Init()
      {
         var filesInAssets = System.IO.Directory.GetFiles("Assets", "BetterShaders_*.txt", System.IO.SearchOption.AllDirectories);
         
         // add templates from packages folder - this covers both having BetterShaders in Assets and in Packages.
         var packageDir = "Packages/com.jbooth.better-shaders";
         if(System.IO.Directory.Exists(packageDir))
         {
            var filesInPackages = System.IO.Directory.GetFiles(packageDir, "BetterShaders_*.txt", System.IO.SearchOption.AllDirectories);
            var filesInAssetsLength = filesInAssets.Length;
            Array.Resize(ref filesInAssets, filesInAssetsLength + filesInPackages.Length);
            Array.Copy(filesInPackages, 0, filesInAssets, filesInAssetsLength, filesInPackages.Length);
         }
         
         paths = filesInAssets;
      }

      /// <summary>
      /// Loads the content of the specified template file.
      /// </summary>
      public string LoadTemplate(string file)
      {
         if (paths == null)
         {
            Init();
         }
         if (paths == null)
         {
            Debug.LogError("No template files found, cannot compile shaders without templates");
            return null;
         }
         foreach (var p in paths)
         {
            if (p.EndsWith(file))
            {
               // must use file here, because assetdatabase might not be built yet.
               return System.IO.File.ReadAllText(p);
            }
         }
         Debug.LogError("Template not found at path : " + file);
         return null;
      }


      // get everything defined in the CBuffer. This makes finding variable names reasonbly easy,
      // since they are all pretty simple declarations. Only the word before the ; matters
      
      static List<string> FindVariablesInCBuffer(string[] lines)
      {
         List<string> names = new List<string>();
         foreach (var l in lines)
         {
            string s = l.Trim();
            if (s.Length > 0)
            {
               var split = s.Split(' ');
               for (int i = split.Length - 1; i >= 0; --i)
               {
                  if (split[i] == ";")
                  {
                     names.Add(split[i - 1]);
                     break;
                  }
                  else if (split[i].EndsWith(";"))
                  {
                     names.Add(split[i].Replace(";", ""));
                     break;
                  }
               }

            }
         }
         return names;
      }


      static List<string> FindVariablesInProperties(string[] lines)
      {
         List<string> names = new List<string>();
         foreach (var l in lines)
         {
            string s = l.StripBetween('[', ']').Trim(); // remove attributes
            if (s.Length > 0)
            {
               var split = s.Split(' ');
               if (split.Length > 0)
               {
                  if (split[0].Contains("("))
                  {
                     string name = split[0].Substring(0, split[0].IndexOf("("));
                     names.Add(name);
                  }
                  else
                  {
                     names.Add(split[0]);
                  }
               }
            }
         }
         return names;
      }

      public static List<string> GetVariableNames(string properties, string cbuffer)
      {
         List<string> all = new List<string>();
         var p = FindVariablesInProperties(properties.ToLines());
         var cb = FindVariablesInCBuffer(cbuffer.ToLines());
         if (p.Count > 0)
         {
            all.AddRange(p);
         }
         if (cb.Count > 0)
         {
            all.AddRange(cb);
         }
         
         // now remove special keywords, like _ST and _TexelSize
         for (int i = 0; i < all.Count; ++i)
         {
            string s = all[i];
            if (s.EndsWith("_ST"))
            {
               all[i] = s.Substring(0, s.Length - 3);
            }
            if (s.EndsWith("_TexelSize"))
            {
               all[i] = s.Substring(0, s.Length - 10);
            }
            if (s.EndsWith("_HDR"))
            {
               all[i] = s.Substring(0, s.Length - 4);
            }
         }
         all = all.Distinct().ToList();

         // we return the list sorted from longest to shortest, so ApplePear gets replaced before Apple or Pear
         var result = new List<string>(all.OrderBy(x => x.Length).Reverse());
         return result;
      }

      public static List<string> FindLocalKeywords(string defines)
      {
         var lines = defines.ToLines();
         List<string> keywords = new List<string>();
         foreach (var l in lines)
         {
            var s = l.StripComments().Trim();
            if (s.Contains("#pragma") && s.Contains("shader_feature_local"))
            {
               var split = s.Split(' ');
               foreach (var sp in split)
               {
                  if (sp != "#pragma" && !sp.Contains("shader_feature_local") && sp != "_")
                  {
                     keywords.Add(sp);
                  }
               }
            }
         }
         keywords = keywords.Distinct().ToList();

         // we return the list sorted from longest to shortest, so ApplePear gets replaced before Apple or Pear
         var result = new List<string>(keywords.OrderBy(x => x.Length).Reverse());
         return keywords;
      }
   }
    



   internal static class StringExtensions
   {
      public static string ExtractBetween(this string s, char begin, char end)
      {
         if (s.Contains(begin) && s.Contains(end))
         {
            int idx = s.IndexOf(begin) + 1;
            return s.Substring(idx, s.IndexOf(end) - idx);
         }
         return "";
      }
      public static string StripBetween(this string s, char begin, char end)
      {
         Regex regex = new Regex(string.Format("\\{0}.*?\\{1}", begin, end));
         return regex.Replace(s, string.Empty);
      }

      public static string StripComments(this string str)
      {
         var blockComments = @"/\*(.*?)\*/";
         var lineComments = @"//(.*?)\r?\n";
         var strings = @"""((\\[^\n]|[^""\n])*)""";
         var verbatimStrings = @"@(""[^""]*"")+";

         string noComments = Regex.Replace(str, blockComments + "|" + lineComments + "|" + strings + "|" + verbatimStrings, me =>
         {
            if (me.Value.StartsWith("/*") || me.Value.StartsWith("//"))
               return me.Value.StartsWith("//") ? System.Environment.NewLine : "";
            return me.Value;
         },
             RegexOptions.Singleline);

         return noComments;
      }

      public static string[] ToLines(this string str)
      {
         return str.Split("\n\r".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
      }

      public static string ReplaceVariable(this string str, string from, string to)
      {
         string regexStr = from + "(?![a-zA-Z0-9])";
         return Regex.Replace(str, regexStr, to);
      }
   }
#endif
}