using FluentValidation;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using ShowerEnclosuresModelsLibrary.Validators.B6000Validators;
using ShowerEnclosuresModelsLibrary.Validators.DBValidators;
using ShowerEnclosuresModelsLibrary.Validators.FreeValidators;
using ShowerEnclosuresModelsLibrary.Validators.HBValidators;
using ShowerEnclosuresModelsLibrary.Validators.Inox304Validators;
using ShowerEnclosuresModelsLibrary.Validators.NBValidators;
using ShowerEnclosuresModelsLibrary.Validators.NPValidators;
using ShowerEnclosuresModelsLibrary.Validators.WSValidator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Validators
{
    /// <summary>
    /// The Master Validator for all Cabin Models
    /// </summary>
    public class CabinValidator : AbstractValidator<Cabin>
    {
        /// <summary>
        /// Creates a Master CabinValidator
        /// </summary>
        /// <param name="validationType">The type of Validation</param>
        public CabinValidator(ValidationTypeEnum validationType = ValidationTypeEnum.ValidateAll)
        {
            RuleFor(x => x).SetInheritanceValidator(v =>
            {
                v.Add(new ValidatorCabin94(validationType));
                v.Add(new ValidatorCabin9A(validationType));
                v.Add(new ValidatorCabin9B(validationType));
                v.Add(new ValidatorCabin9C(validationType));
                v.Add(new ValidatorCabin9F(validationType));
                v.Add(new ValidatorCabin9S(validationType));

                v.Add(new ValidatorCabinDB(validationType));

                v.Add(new ValidatorCabinE(validationType));
                v.Add(new ValidatorCabinW(validationType));
                v.Add(new ValidatorCabinWFlipper(validationType));

                v.Add(new ValidatorCabinHB(validationType));

                v.Add(new ValidatorCabinV4(validationType));
                v.Add(new ValidatorCabinVA(validationType));
                v.Add(new ValidatorCabinVF(validationType));
                v.Add(new ValidatorCabinVS(validationType));

                v.Add(new ValidatorCabinNB(validationType));

                v.Add(new ValidatorCabinNP(validationType));

                v.Add(new ValidatorCabinWS(validationType));
            });
        }
    }
}
