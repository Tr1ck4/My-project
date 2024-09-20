using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace JBooth.BetterShaders
{
   public class PipelineStandard : IPipelineAdapter
   {
      public StringBuilder GetTemplate(Options options, ShaderBuilder.RenderPipeline renderPipeline, BetterShaderUtility util, ref StringBuilder defines)
      {
         StringBuilder template = new StringBuilder(100000);
         template.Append(util.LoadTemplate("BetterShaders_Template_Standard.txt"));

         var passforward = new StringBuilder(util.LoadTemplate("BetterShaders_Template_Standard_PassForward.txt"));
         var passforwardAdd = new StringBuilder(util.LoadTemplate("BetterShaders_Template_Standard_PassForwardAdd.txt"));
         var passGBuffer = new StringBuilder(util.LoadTemplate("BetterShaders_Template_Standard_PassGBuffer.txt"));
         var passShadow = new StringBuilder(util.LoadTemplate("BetterShaders_Template_Standard_PassShadow.txt"));
         var passMeta = new StringBuilder(util.LoadTemplate("BetterShaders_Template_Standard_PassMeta.txt"));

         if (options.disableShadowCasting == Options.Bool.True)
         {
            passShadow.Clear();
         }
         if (options.disableGBuffer == Options.Bool.True)
         {
            passGBuffer.Clear();
         }
         if (options.workflow == Options.Workflow.Unlit)
         {
            passforwardAdd.Clear();
            passGBuffer.Clear();
            //passforward = new StringBuilder(util.LoadTemplate("BetterShaders_Template_Standard_PassUnlit.txt"));
         }

         // do alpha
         if (options.alpha != Options.AlphaModes.Opaque)
         {
            passGBuffer.Clear();
            passShadow.Clear();
            passforward = passforward.Replace("%FORWARDBASEBLEND%", "Blend SrcAlpha OneMinusSrcAlpha");
            passforwardAdd = passforwardAdd.Replace("%FORWARDADDBLEND%", "Blend SrcAlpha One");
            if (options.alpha == Options.AlphaModes.PreMultiply)
            {
               defines.AppendLine("\n   #define _ALPHAPREMULTIPLY_ON 1");
            }
            else
            {
               defines.AppendLine("\n   #define _ALPHABLEND_ON 1");
            }

            passforward = passforward.Insert(0, "\nZWrite Off ColorMask RGB\n\n");
         }
         else
         {
            passforward = passforward.Replace("%FORWARDBASEBLEND%", "");
            passforwardAdd = passforwardAdd.Replace("%FORWARDADDBLEND%", "");
         }

         template = template.Replace("%PASSGBUFFER%", passGBuffer.ToString());
         template = template.Replace("%PASSMETA%", passMeta.ToString());
         template = template.Replace("%PASSFORWARD%", passforward.ToString());
         template = template.Replace("%PASSFORWARDADD%", passforwardAdd.ToString());
         template = template.Replace("%PASSSHADOW%", passShadow.ToString());
         // giant block of texture sampling stuff, only for standard, as
         // URP/HDRP already have this included.
         defines.AppendLine(util.LoadTemplate("BetterShaders_Template_Standard_CommonHLSL.txt"));

         // HDRP tags are different, blerg..
         string tagString = "";
         if (options.tags != null)
         {
            tagString = options.tags;
         }
         else
         {
            tagString = "\"RenderType\" = \"Opaque\" \"Queue\" = \"Geometry\"";
         }

         if (options.alpha != Options.AlphaModes.Opaque)
         { 
            tagString = tagString.Replace("Geometry", "Transparent");
            tagString = tagString.Replace("Opaque", "Transparent");
            tagString = tagString.Replace("Transparent", "Transparent");
         }

         template = template.Replace("%TAGS%", tagString);

         string subshaderTags = "";

         if (!string.IsNullOrEmpty(options.grabPass))
         {
            subshaderTags += "      GrabPass {" + options.grabPass + "}\n";
         }

         template.Replace("%SUBSHADERTAGS%", subshaderTags);


         return template;
      }
   }
}
