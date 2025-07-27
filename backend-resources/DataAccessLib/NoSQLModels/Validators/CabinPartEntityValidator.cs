using FluentValidation;
using ShowerEnclosuresModelsLibrary.Validators.zPartsValidators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLib.NoSQLModels.Validators
{
    public class CabinPartEntityValidator : AbstractValidator<CabinPartEntity>
    {
        public static class ErrorCodes
        {
            public const string EmptyObjectIdOnEntity = "EmptyObjectIdOnEntity";
        }

        public CabinPartEntityValidator(bool isNew)
        {
            RuleFor(e => e.Part).SetValidator(new CabinPartBaseValidator<CabinPart>());

            When(e=> !isNew, () =>
            {
                RuleFor(e => e.Id).NotEmpty().WithErrorCode("EmptyObjectIdOnEntity");
            });
        }
    }
}
