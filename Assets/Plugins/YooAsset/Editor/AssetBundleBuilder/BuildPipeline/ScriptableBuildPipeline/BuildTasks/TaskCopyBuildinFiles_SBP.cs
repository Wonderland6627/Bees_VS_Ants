﻿namespace YooAsset.Editor
{
    public class TaskCopyBuildinFiles_SBP : TaskCopyBuildinFiles, IBuildTask
    {
        void IBuildTask.Run(BuildContext context)
        {
            var buildParametersContext = context.GetContextObject<BuildParametersContext>();
            var manifestContext = context.GetContextObject<ManifestContext>();
            var buildMode = buildParametersContext.Parameters.BuildMode;

            if (buildMode == EBuildMode.ForceRebuild || buildMode == EBuildMode.IncrementalBuild)
            {
                if (buildParametersContext.Parameters.BuildinFileCopyOption != EBuildinFileCopyOption.None)
                {
                    CopyBuildinFilesToStreaming(buildParametersContext, manifestContext.Manifest);
                }
            }
        }
    }
}