using System.Linq;
using Microsoft.AspNetCore.Components;

namespace BronzeArtWebApplication.Pages
{
    public partial class Authentication : ComponentBase
    {
        [Parameter] public string Action { get; set; }
    }
}