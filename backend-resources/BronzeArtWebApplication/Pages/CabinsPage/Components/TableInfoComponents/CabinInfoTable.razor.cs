using BronzeRulesPricelistLibrary.Models;
using Microsoft.AspNetCore.Components;
using ShowerEnclosuresModelsLibrary.Models;
using System.Collections.Generic;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using System;

namespace BronzeArtWebApplication.Pages.CabinsPage.Components.TableInfoComponents
{
    public partial class CabinInfoTable : ComponentBase
    {
        [Parameter] public CabinSynthesis Synthesis { get; set; }

        protected override void OnParametersSet()
        {

            base.OnParametersSet();
        }
    }
}
