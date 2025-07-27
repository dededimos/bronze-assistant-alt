using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Validators;

public class ValidatorCabinCode : AbstractValidator<string>
{
    // ^ = start , [] = one of the chars inside , {} = matches for this number of charachters , | = or , $ = end
    //Do not Forget the Parenthesis ()      !!!!!!!

    //1st Charachter RegExpression Model Indicator (Used by other Validators)
    public static readonly Regex ModelIndicatorChars = new("^[oOΟο0Wwς689VvωΩMmΜμNnνΝWwςHhΗηEeΕεDdΔδFfΦφQq;]$");
    
    //Model RegExpression (Both Charachters)
    public static readonly Regex ModelCodes = new("^([oOΟο0][Wwς])|(6[Wwς])|(8[Wwς])|(9[SsΣσCcΨψ4FfΦφBbΒβAaΑα])|([VvωΩ][SsΣσFfΦφ4AaΑα])|([MmΜμ][vVΩω])|([NnνΝQq;][VvΩωBbΒβPpΠπ])|([Wwς][sSΣσ])|([HhΗη][BbΒβ])|([DdΔδ][BbΒβ])|([EeΕε]8)|([ΦφFf][LlλΛ])$");
    //public static readonly Regex MirrorCodes = new("^60(([HhΗη][78])|([χΧxX][46])|([0OoΟο][0OoΟο])|([MmμΜ]3)|([ΝνNn][679cCψΨαΑaA])|([IiιΙ][αΑaAmMμΜcCψΨlLλΛ]))$");
    //Length-Height RegExpression
    public static readonly Regex OnlyNumbers = new("^[0-9]{1,2}$");

    public static readonly Regex OnlyNumbers3 = new("^[0-9]{1,3}$");

    //MetalFinish RegExpression
    public static readonly Regex MetalFinishCodes = new("^1|[BbβΒ]|[MmμΜ]|[AaαΑ]|4|[GgΓγ]|2|5|[eEεΕ]$");
    
    //GlassFinish RegExpression
    public static readonly Regex GlassFinishCodes = new("^[oOΟο0]|[AaαΑ]|[FfΦφ]|[MmμΜ]|[sSΣσ]|[EeΕε]$");

    //RegEx Special End Charachters (Dose not check for the whole model
    //, only allows the charachters that could have been typed in our CodeSet
    public static readonly Regex SpecialEndingChar1 = new("^[1268RrρΡaAαΑ]{0,1}$"); // 0-1chars only of 1/2/8/RrΡρ/6
    public static readonly Regex SpecialEndingChars2 = new("^(2[186])|([RrρΡ]1)|(68)|(81)$"); //2chars only of 21/28/26 or R1(For Model W) or 68 or 81
    public static readonly Regex SpecialEndingChars3 = new("^(268)|(281)$"); // for Corner cabins 6-8mm , 8-10mm 


    /// <summary>
    /// A Cabin Code Validator
    /// </summary>
    /// <param name="isValidatingTyping">Weather this is Validating as charachters are typed (True) else a Complete code Sequence (false)</param>
    public ValidatorCabinCode(bool isValidatingTyping = false)
    {
        //Validates a Cabin Code example full type 9A20-10-85268

        if (!isValidatingTyping)
        {
            //Not Active on typing (Code can be Empty)
            RuleFor(code => code.Length)
            .NotEmpty().WithErrorCode("EmptyCabinCode")
            .GreaterThanOrEqualTo(2).WithErrorCode("CodeLessThan2Chars");
        }
        if (isValidatingTyping)
        {
            //1st Charachter Validation (Only runs when Length is 1 , afterwards the ModelCodes Regex Runs)
            RuleFor(code => code.Substring(0, 1))
                .Matches(ModelIndicatorChars)
                .When(code => code.Length == 1, ApplyConditionTo.CurrentValidator)
                .WithErrorCode("FirstCharNotSupported");
        }
        
        //ModelCode 1st-2nd Charachters , 
        RuleFor(code => code.Substring(0, 2))
            .Matches(ModelCodes)
            .When(code=>code.Length>=2,ApplyConditionTo.CurrentValidator)
            .WithErrorCode("WrongModelCode");

        //Length 3rd Charachter
        RuleFor(code => code.Substring(2, 1))
            .Matches(OnlyNumbers)
            .When(code => code.Length == 3, ApplyConditionTo.CurrentValidator)
            .WithErrorCode("WrongLength");

        //Length 3rd and 4th Charachter Together
        RuleFor(code => code.Substring(2, 2))
            .Matches(OnlyNumbers)
            .When(code => code.Length >= 4, ApplyConditionTo.CurrentValidator)
            .WithErrorCode("WrongLength");

        //Dash 5th Charachter
        RuleFor(Code => Code.Substring(4, 1))
            .Equal("-")
            .When(code => code.Length >= 5, ApplyConditionTo.CurrentValidator)
            .WithErrorCode("OnlyDashAllowed5");

        //Metal Finish Code 6th Charachter
        RuleFor(code => code.Substring(5, 1))
            .Matches(MetalFinishCodes)
            .When(code => code.Length >= 6, ApplyConditionTo.CurrentValidator)
            .WithErrorCode("WrongFinishCode");

        //Charachter Glass 7th
        RuleFor(code => code.Substring(6, 1))
            .Matches(GlassFinishCodes)
            .When(code => code.Length >= 7, ApplyConditionTo.CurrentValidator)
            .WithErrorCode("WrongGlassFinishCode");

        //Dash After 8th Charachter
        RuleFor(Code => Code.Substring(7, 1))
            .Equal("-")
            .When(code => code.Length >= 8, ApplyConditionTo.CurrentValidator)
            .WithErrorCode("OnlyDashAllowed8");

        //Height 9th Charachter
        RuleFor(code => code.Substring(8, 1))
            .Matches(OnlyNumbers)
            .When(code => code.Length == 9, ApplyConditionTo.CurrentValidator)
            .WithErrorCode("WrongHeight");

        //Height 9th and 10th Charachter together
        RuleFor(code => code.Substring(8, 2))
            .Matches(OnlyNumbers)
            .When(code => code.Length >= 10, ApplyConditionTo.CurrentValidator)
            .WithErrorCode("WrongHeight");

        //11th Special Charachter (Runs only for the 11th Spot , afterwards the 12spot takes the Validation)
        RuleFor(code => code.Substring(10, 1))
            .Matches(SpecialEndingChar1)
            .When(code => code.Length == 11, ApplyConditionTo.CurrentValidator)
            .WithErrorCode("Wrong11Digit");

        //12th Special Charachter  (Checks two Charachters).
        RuleFor(code => code.Substring(10, 2))
            .Matches(SpecialEndingChars2)
            .When(code => code.Length >= 12, ApplyConditionTo.CurrentValidator)
            .WithErrorCode("Wrong12Digit");

        //13th Special Charachter (Checks three Charachters) and only for 13th Charachters in total
        RuleFor(code => code.Substring(10, 3))
            .Matches(SpecialEndingChars3)
            .When(code => code.Length == 13, ApplyConditionTo.CurrentValidator)
            .WithErrorCode("Wrong13Digit");

        //Code Must be less than 14 charachters <=13
        RuleFor(code => code.Length)
            .LessThanOrEqualTo(13).WithErrorCode("CodeAbove13Chars");
    }
}
