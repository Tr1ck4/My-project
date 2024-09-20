using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace JBooth.BetterShaders
{
#if __BETTERSHADERS__
   public interface IPipelineAdapter
   {
      // return the template
      StringBuilder GetTemplate(Options options, ShaderBuilder.RenderPipeline renderPipeline, BetterShaderUtility util, ref StringBuilder defines);
   }
#endif
}