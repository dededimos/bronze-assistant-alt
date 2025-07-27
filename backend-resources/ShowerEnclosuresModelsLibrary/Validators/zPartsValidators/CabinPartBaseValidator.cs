using FluentValidation;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Validators.zPartsValidators
{
    public class CabinPartBaseValidator<T> : AbstractValidator<T> where T : CabinPart
    {
        public CabinPartBaseValidator()
        {
            RuleFor(c => c.Code).NotEmpty().WithErrorCode("EmptyPartCode");
            RuleFor(c => c.Description).NotEmpty().WithErrorCode("EmptyPartDescription");
            RuleFor(c => c.Part).NotEmpty().WithErrorCode("EmptyPartType");
            RuleFor(c => c.Material).NotEmpty().WithErrorCode("EmptyPartMaterial");
        }
    }
}
