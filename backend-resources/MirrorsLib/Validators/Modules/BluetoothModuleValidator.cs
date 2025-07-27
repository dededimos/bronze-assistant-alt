using FluentValidation;
using MirrorsLib.MirrorElements.MirrorModules;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsLib.Validators.Modules
{
    public class BluetoothModuleValidator : AbstractValidator<BluetoothModuleInfo>
    {
        public BluetoothModuleValidator()
        {
            RuleFor(b => b.Watt).NotEmpty().WithErrorCode($"WattZero");
            RuleFor(b => b.Speaker1Dimensions.Height).NotEmpty().WithErrorCode($"Speaker1HeightZero");
            RuleFor(b => b.Speaker1Dimensions.Length).NotEmpty().WithErrorCode($"Speaker1LengthZero");
            RuleFor(b => b.Speaker2Dimensions.Height).NotEmpty().WithErrorCode($"Speaker2HeightZero");
            RuleFor(b => b.Speaker2Dimensions.Length).NotEmpty().WithErrorCode($"Speaker2LengthZero");
        }
    }
}
