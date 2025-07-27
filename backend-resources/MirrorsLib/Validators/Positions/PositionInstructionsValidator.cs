using FluentValidation;
using MirrorsLib.Services.PositionService;
using MirrorsLib.Services.PositionService.PositionInstructionsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsLib.Validators.Positions
{
    public class PositionInstructionsValidator : AbstractValidator<PositionInstructionsBase>
    {
        public PositionInstructionsValidator()
        {
            RuleFor(i => i.InstructionsType).Must(i => i != Enums.PositionInstructionsType.UndefinedInstructions).WithErrorCode($"{nameof(PositionInstructionsBase.InstructionsType)} UNDEFINED");
        }
    }

}
