using Microsoft.AspNetCore.Components;
using MirrorsModelsLibrary.Enums;
using System.Collections.Generic;

namespace BronzeArtWebApplication.Components.MirrorCreationComponents.AssembleMirrorComponents
{
    public partial class MirrorOptionsPhotosDisplay : ComponentBase
    {
        //Because it Runs every time a Method to Determine which are the Options Photo Paths it also Runs the rest of the Rendering Code
        //So even if we change a Light or a Support which are not included in the optionsPhoto Paths 
        //The Component updates their Photos

        [Parameter] public List<string> OptionsPhotoPaths { get; set; }
    }
}
