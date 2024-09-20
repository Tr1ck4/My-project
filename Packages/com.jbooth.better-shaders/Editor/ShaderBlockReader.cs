///////////////////////////////////////////
///
/// Better Shaders
/// ©2021 Jason Booth
/// 

using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Linq;

namespace JBooth.BetterShaders
{
#if __BETTERSHADERS__
   internal class ShaderBlockReader
   {
      internal class BlockData
      {
         public BlockData()
         {
            stackIndex = -1;
            subshader = new StringBuilder();
            code = new StringBuilder();
            defines = new StringBuilder();
            options = new StringBuilder();
            blackboard = new StringBuilder();
            properties = new StringBuilder();
            cbuffer = new StringBuilder();
            customPass = new StringBuilder();
            customCBuffers = new Dictionary<string, StringBuilder>();
            instancePropertyBuffer = new Dictionary<string, StringBuilder>();
            customPassBlocks = new Dictionary<string, StringBuilder>();
         }
         public string path;
         public int stackIndex = -1;
         public StringBuilder subshader = null;
         public StringBuilder code = null;
         public StringBuilder defines = null;
         public StringBuilder options = null;
         public StringBuilder properties = null;
         public StringBuilder cbuffer = null;
         public StringBuilder customPass = null;
         public StringBuilder blackboard = null;
         public List<SubShaderMaterial> subMats;
         public Dictionary<string, StringBuilder> customCBuffers;
         public Dictionary<string, StringBuilder> instancePropertyBuffer;
         public Dictionary<string, StringBuilder> customPassBlocks;

         public void ExtractSubMaterial()
         {
            subMats = new List<SubShaderMaterial>();
            if (properties != null && options != null)
            {
               var propNames = BetterShaderUtility.GetVariableNames(properties.ToString(), "");
               string[] optionLines = options.ToString().ToLines();
               foreach (var l in optionLines)
               {
                  string s = l.StripComments().Trim();
                  if (s.StartsWith("SubEditor"))
                  {
                     s = s.Replace("SubEditor", "");
                     s = s.Replace("\"", "");
                     SubShaderMaterial m = new SubShaderMaterial();
                     m.properties = propNames;
                     m.stackIndex = stackIndex;
                     m.editor = s;
                     subMats.Add(m);
                     break;
                  }
               }
            }
            if (subMats.Count == 0)
            {
               SubShaderMaterial m = new SubShaderMaterial();
               m.editor = "";
               m.properties = new List<string>();
               if (properties != null)
               {
                  m.properties = BetterShaderUtility.GetVariableNames(properties.ToString(), "");
               }
               subMats.Add(m);
            }
         }

         void AddOrMerge(ref StringBuilder target, StringBuilder src)
         {
            if (target == null)
            {
               target = src;
            }
            else if (src != null)
            {
               target.AppendLine();
               target.AppendLine(src.ToString());
            }
         }

         void AddOrMerge(ref Dictionary<string, StringBuilder> target, Dictionary<string, StringBuilder> src)
         {
            foreach (var key in src.Keys)
            {
               if (target.ContainsKey(key))
               {
                  target[key].AppendLine(src[key].ToString());
               }
               else
               {
                  target[key] = src[key];
               }
            }
         }

         public void AddOrMerge(BlockData b)
         {
            AddOrMerge(ref options, b.options);
            AddOrMerge(ref properties, b.properties);
            AddOrMerge(ref cbuffer, b.cbuffer);
            AddOrMerge(ref defines, b.defines);
            AddOrMerge(ref subshader, b.subshader);
            AddOrMerge(ref code, b.code);
            AddOrMerge(ref customCBuffers, b.customCBuffers);
            AddOrMerge(ref instancePropertyBuffer, b.instancePropertyBuffer);
            AddOrMerge(ref customPassBlocks, b.customPassBlocks);
            AddOrMerge(ref customPass, b.customPass);

            // Blackboard variables are NOT mutated, so only add them if they
            // have not already been added.
            if (blackboard == null)
            {
               blackboard = b.blackboard;
            }
            else if (b.blackboard != null && !blackboard.ToString().Contains(b.blackboard.ToString()))
            {
               blackboard.AppendLine();
               blackboard.AppendLine(b.blackboard.ToString());
            }

            if (subMats == null)
            {
               subMats = new List<SubShaderMaterial>();
            }
            subMats.AddRange(b.subMats);
           
         }
      }

      public BlockData blocks = null; // list of all blocks found in this and in included files
      public List<string> includes = new List<string>(); // list of included file paths


      public void Read(string assetPath)
      {
         ReadInternal(assetPath, 0);
      }

      
      void ReadIncludes(string assetPath, int recursiveDepth)
      {
         
         var text = File.ReadAllText(assetPath).StripComments();
         int startIndex = 0;
         while (startIndex >= 0)
         {
            startIndex = text.IndexOf("BEGIN_SUBSHADERS", startIndex, System.StringComparison.OrdinalIgnoreCase);
            if (startIndex >= 0)
            {
               int endIndex = text.IndexOf("END_SUBSHADERS", startIndex, System.StringComparison.OrdinalIgnoreCase);
               if (endIndex < 0)
               {
                  Debug.Log("Missing END_SUBSHADERS block");
                  return;
               }
               startIndex += 16;
               var lines = text.Substring(startIndex, endIndex-startIndex).ToLines();
               startIndex = endIndex;
               for (var n = 0; n < lines.Length; ++n)
               {
                  var line = lines[n].Trim();
                  // Check if a file exists at the specified path
                  var includePath = line.Replace("\"", "").Trim(); // Replace quotes
                  if (!string.IsNullOrEmpty(includePath))
                  {
                     var ipath = includePath;
                     if (!File.Exists(ipath))   // check absolute
                     {
                        ipath = Path.GetDirectoryName(assetPath) + "/" + ipath;  // check relative to root
                        if (File.Exists(ipath))
                           includePath = ipath;
                     }

                     if (!File.Exists(includePath))
                     {
                        Debug.LogErrorFormat("SubShader file '{0}' could not be found.", includePath);
                        continue;
                     }

                     // Catch if an include includes itself
                     if (recursiveDepth > 30)
                     {
                        Debug.LogErrorFormat("SubShader file '{0}' skipped due to recursion depth.", includePath);
                        continue;
                     }

                     // Track which files were included
                     ReadIncludes(includePath, recursiveDepth++);
                     includes.Add(includePath);
                  }
               }
            }

         }
      }


      BlockData FileToIncludeData(string file)
      {
         BlockData f = new BlockData();
         f.path = file;
         
         if (file.EndsWith(".cginc"))
         {
            f.defines.Append(File.ReadAllText(file));
            return f;
         }
         var lines = File.ReadAllLines(file);
         for (var n = 0; n < lines.Length; ++n)
         {
            var line = lines[n].Trim();

            if (line.StartsWith("BEGIN_", System.StringComparison.OrdinalIgnoreCase))
            {
               ++n; // skip BEGIN_ line

               string bname = line.Substring("BEGIN_".Length).Trim();
               StringBuilder text = new StringBuilder(2000);
               var endTag = "END_" + bname;
               if (endTag.Contains("("))
                  endTag = endTag.Substring(0, endTag.IndexOf("(")).Trim();
 
               for (; n < lines.Length; ++n)
               {
                  line = lines[n];
                  if (line.StartsWith(endTag, System.StringComparison.OrdinalIgnoreCase))
                     break;

                  text.AppendLine(line);
               }
               if (bname == "CODE")
               {
                  f.code = text;
               }
               else if (bname == "PROPERTIES")
               {
                  f.properties = text;
               }
               else if (bname == "CBUFFER")
               {
                  f.cbuffer = text;
               }
               else if (bname == "BLACKBOARD")
               {
                  f.blackboard = text;
               }
               else if (bname.StartsWith("CBUFFER") && bname.Contains("(") && bname.Contains(")"))
               {
                  string bufferName = bname.ExtractBetween('(', ')');
                  bufferName = bufferName.Replace("\"", "").Trim();
                  f.customCBuffers[bufferName] = text;
               }
               else if (bname.StartsWith("INSTANCING_BUFFER") && bname.Contains("(") && bname.Contains(")"))
               {
                  string bufferName = bname.ExtractBetween('(', ')');
                  bufferName = bufferName.Replace("\"", "").Trim();
                  f.instancePropertyBuffer[bufferName] = text;
               }
               else if (bname.StartsWith("PASS") && bname.Contains("(") && bname.Contains(")"))
               {
                  string bufferName = bname.ExtractBetween('(', ')');
                  bufferName = bufferName.Replace("\"", "").Trim();
                  if (string.IsNullOrEmpty(bufferName))
                  {
                     bufferName = "all";
                  }
                  f.customPassBlocks[bufferName.ToLower()] = text;
               }
               else if (bname.StartsWith("PASS"))
               {
                  f.customPassBlocks["all"] = text;
               }
               else if (bname == "CUSTOM_PASS")
               {
                  f.customPass = text;
               }

               else if (bname == "DEFINES")
               {
                  f.defines = text;
               }
               else if (bname == "SUBSHADER")
               {
                  f.subshader = text;
               }
               else if (bname == "OPTIONS")
               {
                  f.options = text;
               }
               else if (bname == "SUBSHADERS")
               {
                  f.subshader = text;
               }
               else
               {
                  Debug.Log("Unknown block " + bname + " found, discarding");
               }
               continue;
            }
         }

         return f;
      }

      int ChainFunctions(BlockData inc, int chainCount)
      {
         if (inc.code == null)
            return chainCount;
         string asText = inc.code.ToString().StripComments();
         bool incChain = false;
         if (asText.Contains(" SurfaceFunction"))
         {
            inc.code = inc.code.Replace("SurfaceFunction", "Ext_SurfaceFunction" + chainCount + " ");
            incChain = true;
         }
         if (asText.Contains(" ModifyVertex"))
         {
            inc.code = inc.code.Replace("ModifyVertex", "Ext_ModifyVertex" + chainCount + " ");
            incChain = true;
         }
         if (asText.Contains(" ModifyTessellatedVertex"))
         {
            inc.code = inc.code.Replace("ModifyTessellatedVertex", "Ext_ModifyTessellatedVertex" + chainCount + " ");
            incChain = true;
         }
         if (asText.Contains(" FinalColorForward"))
         {
            inc.code = inc.code.Replace("FinalColorForward", "Ext_FinalColorForward" + chainCount + " ");
            incChain = true;
         }
         if (asText.Contains(" FinalGBufferStandard"))
         {
            inc.code = inc.code.Replace("FinalGBufferStandard", "Ext_FinalGBufferStandard" + chainCount + " ");
            incChain = true;
         }

         if (incChain)
            chainCount++;

         return chainCount;
      }

      List<BlockData> RenderIncludes()
      {
         List<BlockData> data = new List<BlockData>();
         for (int i = 0; i < includes.Count; ++i)
         {
            data.Add(FileToIncludeData(includes[i]));
         }
         
         // go through includes, removed duplicates, merge/reindex ones that shouldn't be merged
         List<BlockData> stripped = new List<BlockData>();
         Dictionary<string, int> map = new Dictionary<string, int>();
         int chainCount = 0;
         for (int i = 0; i < data.Count; ++i)
         {
            var inc = data[i];
            if (map.ContainsKey(inc.path))
            {
               int stackIndex = map[inc.path];
               bool needsAdd = false;
               if (inc.cbuffer != null && inc.properties != null && inc.code != null)
               {
                  List<string> curVars = BetterShaderUtility.GetVariableNames(inc.properties.ToString(), inc.cbuffer.ToString());
                  foreach (var key in inc.customCBuffers.Keys)
                  {
                     curVars.AddRange(BetterShaderUtility.GetVariableNames(inc.customCBuffers[key].ToString(), ""));
                  }
                  foreach (var key in inc.instancePropertyBuffer.Keys)
                  {
                     curVars.AddRange(BetterShaderUtility.GetVariableNames(inc.instancePropertyBuffer[key].ToString(), ""));
                  }
                  curVars.OrderByDescending(x => x.Length);

                  inc.stackIndex = stackIndex;
                  string cbuf = inc.cbuffer.ToString();
                  string props = inc.properties.ToString();
                  string code = inc.code.ToString();
                  foreach (var v in curVars)
                  {
                     cbuf = cbuf.ReplaceVariable(v, v + "_Ext_" + stackIndex);
                     props = props.ReplaceVariable(v, v + "_Ext_" + stackIndex);
                     code = code.ReplaceVariable(v, v + "_Ext_" + stackIndex);

                     foreach (var key in inc.customCBuffers.Keys)
                     {
                        var str = inc.customCBuffers[key].ToString();
                        str = str.ReplaceVariable(v, v + "_Ext_" + stackIndex);
                        inc.customCBuffers[key] = new StringBuilder(str);
                     }
                     foreach (var key in inc.instancePropertyBuffer.Keys)
                     {
                        var str = inc.instancePropertyBuffer[key].ToString();
                        str = str.ReplaceVariable(v, v + "_Ext_" + stackIndex);
                        inc.instancePropertyBuffer[key] = new StringBuilder(str);
                     }

                  }
                 
                  if (inc.defines != null)
                  {
                     var keywords = BetterShaderUtility.FindLocalKeywords(inc.defines.ToString());
                     var defs = inc.defines.ToString();
                     foreach (var k in keywords)
                     {
                        cbuf = cbuf.ReplaceVariable(k, k + "_DEF_" + stackIndex);
                        props = props.ReplaceVariable(k, k + "_DEF_" + stackIndex);
                        code = code.ReplaceVariable(k, k + "_DEF_" + stackIndex);
                        defs = defs.ReplaceVariable(k, k + "_DEF_" + stackIndex);
                     }
                     inc.defines = new StringBuilder(defs);
                  }
                  
                  inc.cbuffer = new StringBuilder(cbuf);
                  inc.properties = new StringBuilder(props);
                  inc.code = new StringBuilder(code);

                  inc.cbuffer = inc.cbuffer.Replace("%STACKIDX%", stackIndex.ToString());
                  inc.properties = inc.properties.Replace("%STACKIDX%", stackIndex.ToString());
                  inc.code = inc.code.Replace("%STACKIDX%", stackIndex.ToString());
                  inc.defines = inc.defines.Replace("%STACKIDX%", stackIndex.ToString());

                  needsAdd = true;
               }
               int old = chainCount;
               chainCount = ChainFunctions(inc, chainCount);

               if (chainCount != old || needsAdd)
               {
                  stripped.Add(inc);
                  map[inc.path] = stackIndex + 1;
               }
            }
            else
            {
               chainCount = ChainFunctions(inc, chainCount);
               stripped.Add(inc);
               map.Add(inc.path, 1);
            }
         }

         return stripped;
      }

      public void CombineBlockList(List<BlockData> data)
      {
         List<string> onceBlocks = new List<string>();
         blocks = new BlockData();
         foreach (var b in data)
         {
            // paths can come in relative and funky, like folderA/../FolderB/Base.subshader
            // So we use absolute paths here to do the comparison
            var path = System.IO.Path.GetFullPath(b.path);
            Options o = new Options(b.options); // kinda sucky to parse this multiple times..
            if (o.stackable == Options.Bool.False)
            {
               if (onceBlocks.Contains(path))
               {
                  continue;
               }
               onceBlocks.Add(path);
            }
            b.ExtractSubMaterial();
            blocks.AddOrMerge(b);
         }
      }

      public void ReadStack()
      {
         includes.Reverse();
         List<string> tempIncludes = new List<string>(includes);
         foreach ( var inc in tempIncludes)
         {
            ReadIncludes(inc, 0);
         }

         includes.Reverse();
         List<BlockData> data = RenderIncludes();
         CombineBlockList(data);
      }

      void ReadInternal(string assetPath, int recursiveDepth)
      {
         ReadIncludes(assetPath, 0);
         includes.Add(assetPath);
         List<BlockData> data = RenderIncludes();

         CombineBlockList(data);
      }
   }
#endif
}