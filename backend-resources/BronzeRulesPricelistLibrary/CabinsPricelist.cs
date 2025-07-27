using CommonHelpers;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Helpers;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary
{
    public static class CabinsPricelist
    {
        #region 1. B6000 Pricelists - UPDATED 12-06-2022

        #region 1. 9S Pricelist - PRICES UPDATED 12-06-2022

        private static readonly decimal BronzeClean9S = 70;
        private static readonly decimal SafeKids9S = 125;
        private static readonly decimal CustomizeFactor9S = 0.25m;

        private static readonly List<int> BaseHeight9S = new()  { 1850 };
        private static readonly List<int> BaseLengths9S = new() { 1000, 1050, 1100, 1150, 1200, 1250, 1300, 1350, 1400, 1450, 1500, 1550 };
        private static readonly List<CabinThicknessEnum> BaseThicknesses9S = new() { CabinThicknessEnum.Thick6mm, CabinThicknessEnum.Thick8mm };

        /// <summary>
        /// Nominal Length - Price -- 9S Height:185cm, 6mm Transparent , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePrice9S_6_185_Transp_Chrome = new()
        {
            {    0, 370 },          //Min Price
            { 1000, 370 },          //OLD PRICES { 1000, 280 },
            { 1050, 380 },                     //{ 1050, 290 },
            { 1100, 380 },                     //{ 1100, 290 },
            { 1150, 395 },                     //{ 1150, 300 },
            { 1200, 395 },                     //{ 1200, 300 },
            { 1250, 415 },                     //{ 1250, 315 },
            { 1300, 415 },                     //{ 1300, 315 },
            { 1350, 420 },                     //{ 1350, 320 },
            { 1400, 430 },                     //{ 1400, 325 },
            { 1450, 445 },                     //{ 1450, 340 },
            { 1500, 460 },                     //{ 1500, 350 },
            { 1550, 475 }                      //{ 1550, 360 }
        };

        /// <summary>
        /// Nominal Length - Price -- 9S Height:185cm, 8mm Transparent , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePrice9S_8_185_Transp_Chrome = new()
        {
            {    0, 470 },          //Min Price
            { 1000, 470 },       //OLD PRICES { 1000, 395 },
            { 1050, 480 },                  //{ 1050, 405 },
            { 1100, 480 },                  //{ 1100, 405 },
            { 1150, 505 },                  //{ 1150, 425 },
            { 1200, 505 },                  //{ 1200, 425 },
            { 1250, 520 },                  //{ 1250, 440 },
            { 1300, 525 },                  //{ 1300, 445 },
            { 1350, 545 },                  //{ 1350, 460 },
            { 1400, 550 },                  //{ 1400, 465 },
            { 1450, 570 },                  //{ 1450, 480 },
            { 1500, 590 },                  //{ 1500, 500 },
            { 1550, 610 }                   //{ 1550, 515 }
        };

        /// <summary>
        /// Nominal Length - Price -- 9S Height:185cm, 6mm Special , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePrice9S_6_185_Special_Chrome = new()
        {
            {    0, 525 },       //Min Price
            { 1000, 525 },       //OLD PRICES { 1000, 400 },
            { 1050, 555 },                  //{ 1050, 420 },
            { 1100, 555 },                  //{ 1100, 420 },
            { 1150, 580 },                  //{ 1150, 440 },
            { 1200, 580 },                  //{ 1200, 440 },
            { 1250, 605 },                  //{ 1250, 460 },
            { 1300, 605 },                  //{ 1300, 460 },
            { 1350, 620 },                  //{ 1350, 470 },
            { 1400, 630 },                  //{ 1400, 480 },
            { 1450, 660 },                  //{ 1450, 500 },
            { 1500, 680 },                  //{ 1500, 515 },
            { 1550, 695 }                   //{ 1550, 530 }
        };

        /// <summary>
        /// Nominal Length - Price -- 9S Height:185cm, 8mm Special , Chrome
        /// The Same Difference as per the 6mm
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePrice9S_8_185_Special_Chrome = new()
        {
            //8mm Transparent Glass plus the Difference of 6mm Transparent with Special Glass
            {    0, BasePrice9S_8_185_Transp_Chrome[   0] + (BasePrice9S_6_185_Special_Chrome[   0] - BasePrice9S_6_185_Transp_Chrome[   0])},    //Min Price
            { 1000, BasePrice9S_8_185_Transp_Chrome[1000] + (BasePrice9S_6_185_Special_Chrome[1000] - BasePrice9S_6_185_Transp_Chrome[1000])},    //NEW PRICES { 1000, 625 }, DIFF: 155  **     OLD PRICES { 1000, 515 },   DIFF: 120
            { 1050, BasePrice9S_8_185_Transp_Chrome[1050] + (BasePrice9S_6_185_Special_Chrome[1050] - BasePrice9S_6_185_Transp_Chrome[1050]) },   //           { 1050, 655 }, DIFF: 175  **                { 1050, 535 },   DIFF: 130
            { 1100, BasePrice9S_8_185_Transp_Chrome[1100] + (BasePrice9S_6_185_Special_Chrome[1100] - BasePrice9S_6_185_Transp_Chrome[1100]) },   //           { 1100, 655 }, DIFF: 175  **                { 1100, 535 },   DIFF: 130
            { 1150, BasePrice9S_8_185_Transp_Chrome[1150] + (BasePrice9S_6_185_Special_Chrome[1150] - BasePrice9S_6_185_Transp_Chrome[1150]) },   //           { 1150, 690 }, DIFF: 185  **                { 1150, 565 },   DIFF: 140
            { 1200, BasePrice9S_8_185_Transp_Chrome[1200] + (BasePrice9S_6_185_Special_Chrome[1200] - BasePrice9S_6_185_Transp_Chrome[1200]) },   //           { 1200, 690 }, DIFF: 185  **                { 1200, 565 },   DIFF: 140
            { 1250, BasePrice9S_8_185_Transp_Chrome[1250] + (BasePrice9S_6_185_Special_Chrome[1250] - BasePrice9S_6_185_Transp_Chrome[1250]) },   //           { 1250, 710 }, DIFF: 190  **                { 1250, 585 },   DIFF: 145
            { 1300, BasePrice9S_8_185_Transp_Chrome[1300] + (BasePrice9S_6_185_Special_Chrome[1300] - BasePrice9S_6_185_Transp_Chrome[1300]) },   //           { 1300, 715 }, DIFF: 190  **                { 1300, 590 },   DIFF: 145
            { 1350, BasePrice9S_8_185_Transp_Chrome[1350] + (BasePrice9S_6_185_Special_Chrome[1350] - BasePrice9S_6_185_Transp_Chrome[1350]) },   //           { 1350, 745 }, DIFF: 200  **                { 1350, 610 },   DIFF: 150
            { 1400, BasePrice9S_8_185_Transp_Chrome[1400] + (BasePrice9S_6_185_Special_Chrome[1400] - BasePrice9S_6_185_Transp_Chrome[1400]) },   //           { 1400, 750 }, DIFF: 200  **                { 1400, 620 },   DIFF: 155
            { 1450, BasePrice9S_8_185_Transp_Chrome[1450] + (BasePrice9S_6_185_Special_Chrome[1450] - BasePrice9S_6_185_Transp_Chrome[1450]) },   //           { 1450, 785 }, DIFF: 215  **                { 1450, 640 },   DIFF: 160
            { 1500, BasePrice9S_8_185_Transp_Chrome[1500] + (BasePrice9S_6_185_Special_Chrome[1500] - BasePrice9S_6_185_Transp_Chrome[1500]) },   //           { 1500, 810 }, DIFF: 220  **                { 1500, 665 },   DIFF: 165
            { 1550, BasePrice9S_8_185_Transp_Chrome[1550] + (BasePrice9S_6_185_Special_Chrome[1550] - BasePrice9S_6_185_Transp_Chrome[1550]) }    //           { 1550, 830 }  DIFF: 220  **                { 1550, 685 }    DIFF: 170
        };

        /// <summary>
        /// The Dictionary containing the Various Combinations of Base Prices for 9S Model
        /// Must be Declared after the BaseCombinations are declared ,otherwise the nested Dictionaries Throw Null Exceptions (They have not been created yet when called)
        /// </summary>
        private static readonly Dictionary<(CabinThicknessEnum, GlassFinishEnum), Dictionary<int, decimal>> BasePricelists9S = new()
        {
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Transparent),   BasePrice9S_6_185_Transp_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Serigraphy),    BasePrice9S_6_185_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Special),       BasePrice9S_6_185_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Satin),         BasePrice9S_6_185_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Fume),          BasePrice9S_6_185_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Frosted),       BasePrice9S_6_185_Special_Chrome },
            //Added 17-10-2022
            { (CabinThicknessEnum.Thick6mm8mm, GlassFinishEnum.Transparent),   BasePrice9S_6_185_Transp_Chrome },
            { (CabinThicknessEnum.Thick6mm8mm, GlassFinishEnum.Serigraphy),    BasePrice9S_6_185_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm8mm, GlassFinishEnum.Special),       BasePrice9S_6_185_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm8mm, GlassFinishEnum.Satin),         BasePrice9S_6_185_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm8mm, GlassFinishEnum.Fume),          BasePrice9S_6_185_Special_Chrome },
            
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Transparent),   BasePrice9S_8_185_Transp_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Serigraphy),    BasePrice9S_8_185_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Special),       BasePrice9S_8_185_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Satin),         BasePrice9S_8_185_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Fume),          BasePrice9S_8_185_Special_Chrome },
        };

        /// <summary>
        /// The Extra Cost of Finish 9S Model
        /// </summary>
        private static readonly Dictionary<CabinFinishEnum, decimal?> FinishPricing9S = new()
        {
            { CabinFinishEnum.Polished,       0 },
            { CabinFinishEnum.Brushed,      275 },//Anodizing
            { CabinFinishEnum.BlackMat,     170 },//RAL
            { CabinFinishEnum.WhiteMat,     170 },//RAL
            { CabinFinishEnum.Bronze,       515 },//Anodizing
            { CabinFinishEnum.BrushedGold,  515 },//Anodizing
            { CabinFinishEnum.Gold,         null },//Not Available
            { CabinFinishEnum.Copper,       515 },//Not Available
            { CabinFinishEnum.Special,      null },//Not Available
            { CabinFinishEnum.NotSet,       null } //Not Available
        };
        #endregion

        #region 2. 94 Pricelist - PRICES UPDATED 12-06-2022

        private static readonly decimal BronzeClean94 = 70;
        private static readonly decimal SafeKids94 = 210;
        private static readonly decimal CustomizeFactor94 = 0.25m;

        private static readonly List<int> BaseHeight94 = new() { 1850 };
        private static readonly List<int> BaseLengths94 = new() { 1600, 1650, 1700, 1750, 1800 };
        private static readonly List<CabinThicknessEnum> BaseThicknesses94 = new() { CabinThicknessEnum.Thick6mm, CabinThicknessEnum.Thick8mm };

        /// <summary>
        /// Nominal Length - Price -- 94 Height:185cm, 6mm Transparent , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePrice94_6_185_Transp_Chrome = new()
        {
            {    0, 555 },            //MIN Price
            { 1600, 525 },            //OLDPRICES { 1600, 400 },
            { 1650, 555 },            //          { 1650, 420 },
            { 1700, 555 },            //          { 1700, 420 },
            { 1750, 585 },            //          { 1750, 445 },
            { 1800, 585 }             //          { 1800, 445 }
        };

        /// <summary>
        /// Nominal Length - Price -- 94 Height:185cm, 8mm Transparent , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePrice94_8_185_Transp_Chrome = new()
        {
            {    0, 665 },             //MIN Price
            { 1600, 665 },             //OLD PRICES { 1600, 560 },
            { 1650, 700 },             //           { 1650, 590 },
            { 1700, 700 },             //           { 1700, 590 },
            { 1750, 745 },             //           { 1750, 630 },
            { 1800, 745 }              //           { 1800, 630 }
        };

        /// <summary>
        /// Nominal Length - Price -- 94 Height:185cm, 6mm Special , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePrice94_6_185_Special_Chrome = new()
        {
            {    0, 765 },              //MIN Price
            { 1600, 765 },              //OLD PRICES    { 1600, 580 },
            { 1650, 795 },              //              { 1650, 605 },
            { 1700, 805 },              //              { 1700, 610 },
            { 1750, 850 },              //              { 1750, 645 },
            { 1800, 855 }               //              { 1800, 650 }
        };

        /// <summary>
        /// Nominal Length - Price -- 94 Height:185cm, 8mm Special , Chrome
        /// The Same Difference as per the 6mm
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePrice94_8_185_Special_Chrome = new()
        {
            //Difference of Special Glass From Transparent
            {    0, BasePrice94_8_185_Transp_Chrome[   0] + (BasePrice94_6_185_Special_Chrome[   0] - BasePrice94_6_185_Transp_Chrome[   0]) },   //Min Price
            { 1600, BasePrice94_8_185_Transp_Chrome[1600] + (BasePrice94_6_185_Special_Chrome[1600] - BasePrice94_6_185_Transp_Chrome[1600]) },   //{ 1600, 905  },   DIFF:240 **  OLD PRICES{ 1600, 740 },   DIFF:180
            { 1650, BasePrice94_8_185_Transp_Chrome[1650] + (BasePrice94_6_185_Special_Chrome[1650] - BasePrice94_6_185_Transp_Chrome[1650]) },   //{ 1650, 940  },   DIFF:240 **            { 1650, 775 },   DIFF:185
            { 1700, BasePrice94_8_185_Transp_Chrome[1700] + (BasePrice94_6_185_Special_Chrome[1700] - BasePrice94_6_185_Transp_Chrome[1700]) },   //{ 1700, 950  },   DIFF:250 **            { 1700, 780 },   DIFF:190
            { 1750, BasePrice94_8_185_Transp_Chrome[1750] + (BasePrice94_6_185_Special_Chrome[1750] - BasePrice94_6_185_Transp_Chrome[1750]) },   //{ 1750, 1010 },   DIFF:265 **            { 1750, 830 },   DIFF:200
            { 1800, BasePrice94_8_185_Transp_Chrome[1800] + (BasePrice94_6_185_Special_Chrome[1800] - BasePrice94_6_185_Transp_Chrome[1800]) },   //{ 1800, 1015 }    DIFF:270 **            { 1800, 835 }    DIFF:205
        };

        /// <summary>
        /// The Dictionary containing the Various Combinations of Base Prices for 94 Model
        /// Must be Declared after the BaseCombinations are declared ,otherwise the nested Dictionaries Throw Null Exceptions (They have not been created yet when called)
        /// </summary>
        private static readonly Dictionary<(CabinThicknessEnum, GlassFinishEnum), Dictionary<int, decimal>> BasePricelists94 = new()
        {
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Transparent),   BasePrice94_6_185_Transp_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Serigraphy),    BasePrice94_6_185_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Special),       BasePrice94_6_185_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Satin),         BasePrice94_6_185_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Fume),          BasePrice94_6_185_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Frosted),       BasePrice94_6_185_Special_Chrome },

            //Added 17-10-2022
            { (CabinThicknessEnum.Thick6mm8mm, GlassFinishEnum.Transparent),   BasePrice94_6_185_Transp_Chrome  },
            { (CabinThicknessEnum.Thick6mm8mm, GlassFinishEnum.Serigraphy),    BasePrice94_6_185_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm8mm, GlassFinishEnum.Special),       BasePrice94_6_185_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm8mm, GlassFinishEnum.Satin),         BasePrice94_6_185_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm8mm, GlassFinishEnum.Fume),          BasePrice94_6_185_Special_Chrome },

            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Transparent),   BasePrice94_8_185_Transp_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Serigraphy),    BasePrice94_8_185_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Special),       BasePrice94_8_185_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Satin),         BasePrice94_8_185_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Fume),          BasePrice94_8_185_Special_Chrome },
        };

        /// <summary>
        /// The Extra Cost of Finish 94 Model
        /// </summary>
        private static readonly Dictionary<CabinFinishEnum, decimal?> FinishPricing94 = new()
        {
            { CabinFinishEnum.Polished,       0 },
            { CabinFinishEnum.Brushed,      315 },//Anodizing
            { CabinFinishEnum.BlackMat,     225 },//RAL
            { CabinFinishEnum.WhiteMat,     225 },//RAL
            { CabinFinishEnum.Bronze,       630 },//Anodizing
            { CabinFinishEnum.BrushedGold,  630 },//Anodizing
            { CabinFinishEnum.Gold,         null },//Not Available
            { CabinFinishEnum.Copper,       630 },//Not Available
            { CabinFinishEnum.Special,      null },//Not Available
            { CabinFinishEnum.NotSet,       null } //Not Available
        };

        #endregion

        #region 3. 9B Pricelist - PRICES UPDATED 12-06-2022

        private static readonly decimal BronzeClean9B = 60;
        private static readonly decimal SafeKids9B = 85;
        private static readonly decimal CustomizeFactor9B = 0.25m;

        private static readonly List<int> BaseHeight9B = new() { 1850 };
        private static readonly List<int> BaseLengths9B = new() { 700, 800, 900, 1000, 1100, 1200, 1300, 1400 };
        private static readonly List<CabinThicknessEnum> BaseThicknesses9B = new() { CabinThicknessEnum.Thick6mm};

        /// <summary>
        /// Nominal Length - Price -- 9B Height:185cm, 6mm Transparent , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePrice9B_6_185_Transp_Chrome = new()
        {
            {   0,  330 },                              //MIN Price
            { 700,  330 },                              //OLD PRICES    { 700,  250 },
            //{ 750,  370 }, OUT OF NORMAL FLOW         //              { 750,  280 }, OUT PRICE NOT IN NORMAL FLOW
            { 800,  340 },                              //              { 800,  260 },
            //{ 850,  380 }, OUT OF NORMAL FLOW         //              { 850,  290 }, OUR PRICE NOT IN NORMAL FLOW
            { 900,  355 },                              //              { 900,  270 },
            { 1000, 395 },                              //              { 1000, 300 },
            { 1100, 410 },                              //              { 1100, 310 },
            { 1200, 420 },                              //              { 1200, 320 },
            { 1300, 440 },                              //              { 1300, 335 },
            { 1400, 455 }                               //              { 1400, 345 } 
        };

        /// <summary>
        /// Nominal Length - Price -- 9B Height:185cm, 6mm Special , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePrice9B_6_185_Special_Chrome = new()
        {
            {   0,  460 },                              //MIN Price
            { 700,  460 },                      //OLD PRICES { 700,  350 },
            //{ 750,  465 }, OUT OF NORMAL FLOW //           { 750,  355 }, OUT OF NORMAL FLOW
            { 800,  485 },                      //           { 800,  370 },
            //{ 850,  495 }, OUT OF NORMAL FLOW //           { 850,  375 }, OUT OF NORMAL FLOW
            { 900,  500 },                      //           { 900,  380 },
            { 1000, 555 },                      //           { 1000, 420 },
            { 1100, 580 },                      //           { 1100, 440 },
            { 1200, 605 },                      //           { 1200, 460 },
            { 1300, 630 },                      //           { 1300, 480 },
            { 1400, 660 }                       //           { 1400, 500 }
        };

        /// <summary>
        /// The Dictionary containing the Various Combinations of Base Prices for 9B Model
        /// Must be Declared after the BaseCombinations are declared ,otherwise the nested Dictionaries Throw Null Exceptions (They have not been created yet when called)
        /// </summary>
        private static readonly Dictionary<(CabinThicknessEnum, GlassFinishEnum), Dictionary<int, decimal>> BasePricelists9B = new()
        {
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Transparent),   BasePrice9B_6_185_Transp_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Serigraphy),    BasePrice9B_6_185_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Special),       BasePrice9B_6_185_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Satin),         BasePrice9B_6_185_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Fume),          BasePrice9B_6_185_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Frosted),       BasePrice9B_6_185_Special_Chrome },
        };

        /// <summary>
        /// The Extra Cost of Finish 9B Model
        /// </summary>
        private static readonly Dictionary<CabinFinishEnum, decimal?> FinishPricing9B = new()
        {
            { CabinFinishEnum.Polished,       0 },
            { CabinFinishEnum.Brushed,      275 },//Anodizing
            { CabinFinishEnum.BlackMat,     170 },//RAL
            { CabinFinishEnum.WhiteMat,     170 },//RAL
            { CabinFinishEnum.Bronze,       515 },//Anodizing
            { CabinFinishEnum.BrushedGold,  515 },//Anodizing
            { CabinFinishEnum.Gold,         null },//Not Available
            { CabinFinishEnum.Copper,       515 },//Not Available
            { CabinFinishEnum.Special,      null },//Not Available
            { CabinFinishEnum.NotSet,       null } //Not Available
        };
        #endregion

        #region 4. 9F Pricelist - PRICES UPDATED 12-06-2022

        private static readonly decimal BronzeClean9F = 35;
        private static readonly decimal SafeKids9F = 85;
        private static readonly decimal CustomizeFactor9F = 0.25m;

        private static readonly List<int> BaseHeight9F = new() { 1850 };
        private static readonly List<int> BaseLengths9F = new() { 700, 750, 800, 850, 900, 950, 1000, 1050 };
        private static readonly List<CabinThicknessEnum> BaseThicknesses9F = new() { CabinThicknessEnum.Thick6mm, CabinThicknessEnum.Thick8mm };

        /// <summary>
        /// Nominal Length - Price -- 9F Height:185cm, 6mm Transparent , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePrice9F_6_185_Transp_Chrome = new()
        {
            {   0,  260 },                      //Min Price
            { 700,  260 },                      //OLD PRICE { 700,  160 },
            { 750,  265 },                      //          { 750,  200 },
            { 800,  280 },                      //          { 800,  175 },
            { 850,  285 },                      //          { 850,  215 },
            { 900,  285 },                      //          { 900,  180 },
            { 950,  290 },                      //          { 950,  220 },
            { 1000, 310 },                      //          { 1000, 235 },
            { 1050, 320 }                       //          { 1050, 245 }
        };

        /// <summary>
        /// Nominal Length - Price -- 9F Height:185cm, 8mm Transparent , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePrice9F_8_185_Transp_Chrome = new()
        {
            {   0,  270 },                 //Min Price
            { 700,  270 },                 //OLD PRICES { 700,  230 },
            { 750,  285 },                 //           { 750,  240 },
            { 800,  295 },                 //           { 800,  250 },
            { 850,  310 },                 //           { 850,  260 },
            { 900,  315 },                 //           { 900,  265 },
            { 950,  320 },                 //           { 950,  270 },
            { 1000, 340 },                 //           { 1000, 285 },
            { 1050, 355 }                  //           { 1050, 300 }
        };

        /// <summary>
        /// Nominal Length - Price -- 9F Height:185cm, 6mm Special , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePrice9F_6_185_Special_Chrome = new()
        {
            {   0,  360 },              //Min Price
            { 700,  360 },              //OLD PRICES{ 700,  205 },
            { 750,  380 },              //          { 750,  290 }, 
            { 800,  385 },              //          { 800,  225 },
            { 850,  410 },              //          { 850,  310 }, 
            { 900,  430 },              //          { 900,  275 },
            { 950,  430 },              //          { 950,  325 },
            { 1000, 445 },              //          { 1000, 340 },
            { 1050, 475 }               //          { 1050, 360 } 
        };

        /// <summary>
        /// Nominal Length - Price -- 9F Height:185cm, 8mm Special , Chrome
        /// The Same Difference as per the 6mm
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePrice9F_8_185_Special_Chrome = new()
        {
            //Difference of Special Glass From Transparent
            {   0,  BasePrice9F_8_185_Transp_Chrome[  0] +  (BasePrice9F_6_185_Special_Chrome[  0]  - BasePrice9F_6_185_Transp_Chrome[  0] ) }, //Min Price
            { 700,  BasePrice9F_8_185_Transp_Chrome[700] +  (BasePrice9F_6_185_Special_Chrome[700]  - BasePrice9F_6_185_Transp_Chrome[700] ) }, //{ 700,  370 }, DIFF:100  ** OLD PRICES { 700,  275 }, DIFF:45
            { 750,  BasePrice9F_8_185_Transp_Chrome[750] +  (BasePrice9F_6_185_Special_Chrome[750]  - BasePrice9F_6_185_Transp_Chrome[750] ) }, //{ 750,  400 }, DIFF:115  **            { 750,  330 }, DIFF:90
            { 800,  BasePrice9F_8_185_Transp_Chrome[800] +  (BasePrice9F_6_185_Special_Chrome[800]  - BasePrice9F_6_185_Transp_Chrome[800] ) }, //{ 800,  400 }, DIFF:105  **            { 800,  300 }, DIFF:50
            { 850,  BasePrice9F_8_185_Transp_Chrome[850] +  (BasePrice9F_6_185_Special_Chrome[850]  - BasePrice9F_6_185_Transp_Chrome[850] ) }, //{ 850,  435 }, DIFF:125  **            { 850,  355 }, DIFF:95
            { 900,  BasePrice9F_8_185_Transp_Chrome[900] +  (BasePrice9F_6_185_Special_Chrome[900]  - BasePrice9F_6_185_Transp_Chrome[900] ) }, //{ 900,  460 }, DIFF:145  **            { 900,  360 }, DIFF:95
            { 950,  BasePrice9F_8_185_Transp_Chrome[950] +  (BasePrice9F_6_185_Special_Chrome[950]  - BasePrice9F_6_185_Transp_Chrome[950] ) }, //{ 950,  460 }, DIFF:140  **            { 950,  375 }, DIFF:105
            { 1000, BasePrice9F_8_185_Transp_Chrome[1000] + (BasePrice9F_6_185_Special_Chrome[1000] - BasePrice9F_6_185_Transp_Chrome[1000]) }, //{ 1000, 475 }, DIFF:135  **            { 1000, 390 }, DIFF:105
            { 1050, BasePrice9F_8_185_Transp_Chrome[1050] + (BasePrice9F_6_185_Special_Chrome[1050] - BasePrice9F_6_185_Transp_Chrome[1050]) }  //{ 1050, 510 }  DIFF:155  **            { 1050, 415 }  DIFF:115
        };

        /// <summary>
        /// The Dictionary containing the Various Combinations of Base Prices for 9F Model
        /// Must be Declared after the BaseCombinations are declared ,otherwise the nested Dictionaries Throw Null Exceptions (They have not been created yet when called)
        /// </summary>
        private static readonly Dictionary<(CabinThicknessEnum, GlassFinishEnum), Dictionary<int, decimal>> BasePricelists9F = new()
        {
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Transparent),   BasePrice9F_6_185_Transp_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Serigraphy),    BasePrice9F_6_185_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Special),       BasePrice9F_6_185_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Satin),         BasePrice9F_6_185_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Fume),          BasePrice9F_6_185_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Frosted),       BasePrice9F_6_185_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Transparent),   BasePrice9F_8_185_Transp_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Serigraphy),    BasePrice9F_8_185_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Special),       BasePrice9F_8_185_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Satin),         BasePrice9F_8_185_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Fume),          BasePrice9F_8_185_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Frosted),       BasePrice9F_8_185_Special_Chrome },
        };

        /// <summary>
        /// The Extra Cost of Finish 9F Model
        /// </summary>
        private static readonly Dictionary<CabinFinishEnum, decimal?> FinishPricing9F = new()
        {
            { CabinFinishEnum.Polished,       0 },
            { CabinFinishEnum.Brushed,      185 },//Anodizing
            { CabinFinishEnum.BlackMat,     105 },//RAL
            { CabinFinishEnum.WhiteMat,     105 },//RAL
            { CabinFinishEnum.Bronze,       290 },//Anodizing
            { CabinFinishEnum.BrushedGold,  290 },//Anodizing
            { CabinFinishEnum.Gold,         null },//Not Available
            { CabinFinishEnum.Copper,       290 },//Not Available
            { CabinFinishEnum.Special,      null },//Not Available
            { CabinFinishEnum.NotSet,       null } //Not Available
        };

        #endregion

        #region 5. 9A Pricelist - PRICES UPDATED 12-06-2022

        private static readonly decimal BronzeClean9A = 70; //Normally its '35' but 9A is always as a double item 
        private static readonly decimal SafeKids9A = 105;
        private static readonly decimal CustomizeFactor9A = 0.25m;

        private static readonly List<int> BaseHeight9A = new() { 1850 };
        private static readonly List<int> BaseLengths9A = new() { 700, 800, 900, 1000, 1200 };
        private static readonly List<CabinThicknessEnum> BaseThicknesses9A = new() { CabinThicknessEnum.Thick6mm, CabinThicknessEnum.Thick8mm };

        /// <summary>
        /// Nominal Length - Price -- 9A Height:185cm, 6mm Transparent , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePrice9A_6_185_Transp_Chrome = new()
        {
            {   0,  265 },              //Min Price
            { 700,  265 },              //OLD PRICES { 700,  200 },
            { 800,  275 },              //           { 800,  210 },
            { 900,  290 },              //           { 900,  220 },
            { 1000, 310 },              //           { 1000, 235 },
            { 1200, 340 }               //           { 1200, 260 }
        };

        /// <summary>
        /// Nominal Length - Price -- 9A Height:185cm, 8mm Transparent , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePrice9A_8_185_Transp_Chrome = new()
        {
            {   0,  330 },                  //Min Price
            { 700,  330 },                  //OLD PRICES { 700,  280 },
            { 800,  350 },                  //           { 800,  295 },
            { 900,  375 },                  //           { 900,  315 },
            { 1000, 395 },                  //           { 1000, 335 },
            { 1200, 440 }                   //           { 1200, 370 }
        };

        /// <summary>
        /// Nominal Length - Price -- 9A Height:185cm, 6mm Special , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePrice9A_6_185_Special_Chrome = new()
        {
            {   0,  380 },              //Min Price
            { 700,  380 },              //OLD PRICES { 700,  290 },
            { 800,  410 },              //           { 800,  310 },
            { 900,  435 },              //           { 900,  330 },
            { 1000, 460 },              //           { 1000, 350 },
            { 1200, 515 }               //           { 1200, 390 }
        };

        /// <summary>
        /// Nominal Length - Price -- 9A Height:185cm, 8mm Special , Chrome
        /// The Same Difference as per the 6mm
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePrice9A_8_185_Special_Chrome = new()
        {
            //Difference of Special Glass From Transparent
            {   0,  BasePrice9A_8_185_Transp_Chrome[  0] +  (BasePrice9A_6_185_Special_Chrome[  0]  - BasePrice9A_6_185_Transp_Chrome[  0] ) },  //Min Price
            { 700,  BasePrice9A_8_185_Transp_Chrome[700] +  (BasePrice9A_6_185_Special_Chrome[700]  - BasePrice9A_6_185_Transp_Chrome[700] ) },  //{ 700,  445 }, DIIF:115 ** OLD PRICES { 700,  370 },  DIFF:90
            { 800,  BasePrice9A_8_185_Transp_Chrome[800] +  (BasePrice9A_6_185_Special_Chrome[800]  - BasePrice9A_6_185_Transp_Chrome[800] ) },  //{ 800,  485 }, DIIF:135 **            { 800,  395 },  DIFF:100
            { 900,  BasePrice9A_8_185_Transp_Chrome[900] +  (BasePrice9A_6_185_Special_Chrome[900]  - BasePrice9A_6_185_Transp_Chrome[900] ) },  //{ 900,  520 }, DIIF:145 **            { 900,  425 },  DIFF:110
            { 1000, BasePrice9A_8_185_Transp_Chrome[1000] + (BasePrice9A_6_185_Special_Chrome[1000] - BasePrice9A_6_185_Transp_Chrome[1000]) },  //{ 1000, 545 }, DIIF:150 **            { 1000, 450 },  DIFF:115
            { 1200, BasePrice9A_8_185_Transp_Chrome[1200] + (BasePrice9A_6_185_Special_Chrome[1200] - BasePrice9A_6_185_Transp_Chrome[1200]) },  //{ 1200, 615 }  DIIF:175 **            { 1200, 500 }   DIFF:130
        };

        /// <summary>
        /// The Dictionary containing the Various Combinations of Base Prices for 9A Model
        /// Must be Declared after the BaseCombinations are declared ,otherwise the nested Dictionaries Throw Null Exceptions (They have not been created yet when called)
        /// </summary>
        private static readonly Dictionary<(CabinThicknessEnum, GlassFinishEnum), Dictionary<int, decimal>> BasePricelists9A = new()
        {
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Transparent),   BasePrice9A_6_185_Transp_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Serigraphy),    BasePrice9A_6_185_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Special),       BasePrice9A_6_185_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Satin),         BasePrice9A_6_185_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Fume),          BasePrice9A_6_185_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Frosted),       BasePrice9A_6_185_Special_Chrome },

            //Added 17-10-2022
            { (CabinThicknessEnum.Thick6mm8mm, GlassFinishEnum.Transparent),   BasePrice9A_6_185_Transp_Chrome },
            { (CabinThicknessEnum.Thick6mm8mm, GlassFinishEnum.Serigraphy),    BasePrice9A_6_185_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm8mm, GlassFinishEnum.Special),       BasePrice9A_6_185_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm8mm, GlassFinishEnum.Satin),         BasePrice9A_6_185_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm8mm, GlassFinishEnum.Fume),          BasePrice9A_6_185_Special_Chrome },

            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Transparent),   BasePrice9A_8_185_Transp_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Serigraphy),    BasePrice9A_8_185_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Special),       BasePrice9A_8_185_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Satin),         BasePrice9A_8_185_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Fume),          BasePrice9A_8_185_Special_Chrome },
        };

        /// <summary>
        /// The Extra Cost of Finish 9A Model
        /// </summary>
        private static readonly Dictionary<CabinFinishEnum, decimal?> FinishPricing9A = new()
        {
            { CabinFinishEnum.Polished,       0 },
            { CabinFinishEnum.Brushed,      160 },//Anodizing
            { CabinFinishEnum.BlackMat,     105 },//RAL
            { CabinFinishEnum.WhiteMat,     105 },//RAL
            { CabinFinishEnum.Bronze,       315 },//Anodizing
            { CabinFinishEnum.BrushedGold,  315 },//Anodizing
            { CabinFinishEnum.Gold,         null },//Not Available
            { CabinFinishEnum.Copper,       315 },//Not Available
            { CabinFinishEnum.Special,      null },//Not Available
            { CabinFinishEnum.NotSet,       null } //Not Available
        };

        #endregion

        #region 6. 9C Pricelist - PRICES UPDATED 12-06-2022

        private static readonly decimal BronzeClean9C = 70;
        private static readonly decimal SafeKids9C = 250;
        private static readonly decimal CustomizeFactor9C = 0.25m;

        private static readonly List<int> BaseHeight9C = new() { 1850 };
        private static readonly List<int> BaseLengths9C = new() { 800, 900, 1000 };
        private static readonly List<CabinThicknessEnum> BaseThicknesses9C = new() { CabinThicknessEnum.Thick6mm, CabinThicknessEnum.Thick6mm8mm };

        /// <summary>
        /// Nominal Length - Price -- 9C Height:185cm, 6mm Transparent , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePrice9C_6_185_Transp_Chrome = new()
        {
            {    0,  500 },          //MinPrice
            {  800,  500 },          //OLD PRICES { 800,  380 },
            {  900,  515 },          //           { 900,  390 },
            { 1000,  630 }           //           { 1000, 480 }
        };

        /// <summary>
        /// Nominal Length - Price -- 9C Height:185cm, 6/8mm Transparent , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePrice9C_68_185_Transp_Chrome = new()
        {
            {    0,  585 },          //MinPrice
            {  800,  585 },          //OLD PRICES { 800,  495 },
            {  900,  600 },          //           { 900,  505 },
            { 1000,  735 }           //           { 1000, 620 }
        };

        /// <summary>
        /// Nominal Length - Price -- 9C Height:185cm, 6mm Special , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePrice9C_6_185_Special_Chrome = new()
        {
            {   0, 565 },          //MinPrice
            { 800, 565 },           //OLD PRICES { 800, 430 },
            { 900, 580 },           //           { 900, 440 },
        };

        /// <summary>
        /// The Dictionary containing the Various Combinations of Base Prices for 9C Model
        /// Must be Declared after the BaseCombinations are declared ,otherwise the nested Dictionaries Throw Null Exceptions (They have not been created yet when called)
        /// </summary>
        private static readonly Dictionary<(CabinThicknessEnum, GlassFinishEnum), Dictionary<int, decimal>> BasePricelists9C = new()
        {
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Transparent),       BasePrice9C_6_185_Transp_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Serigraphy),        BasePrice9C_6_185_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Special),           BasePrice9C_6_185_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Satin),             BasePrice9C_6_185_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Fume),              BasePrice9C_6_185_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Frosted),           BasePrice9C_6_185_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm8mm, GlassFinishEnum.Transparent),    BasePrice9C_68_185_Transp_Chrome }
        };

        /// <summary>
        /// The Extra Cost of Finish 9C Model
        /// </summary>
        private static readonly Dictionary<CabinFinishEnum, decimal?> FinishPricing9C = new()
        {
            { CabinFinishEnum.Polished,       0 },
            { CabinFinishEnum.Brushed,      315 },//Anodizing
            { CabinFinishEnum.BlackMat,     225 },//RAL
            { CabinFinishEnum.WhiteMat,     225 },//RAL
            { CabinFinishEnum.Bronze,       630 },//Anodizing
            { CabinFinishEnum.BrushedGold,  630 },//Anodizing
            { CabinFinishEnum.Gold,         null },//Not Available
            { CabinFinishEnum.Copper,       630 },//Not Available
            { CabinFinishEnum.Special,      null },//Not Available
            { CabinFinishEnum.NotSet,       null } //Not Available
        };
        #endregion

        #endregion

        #region 2. Free Pricelists - UPDATED PRICES 12-06-2022

        #region 2.1 W Pricelist  - UPDATED PRICES 12-06-2022

        private static readonly decimal BronzeCleanW = 60;
        private static readonly decimal SafeKidsW = 85;
        private static readonly decimal W_10mm_Premium = 100;
        private static readonly decimal CustomizeFactorW = 0.15m;

        private static readonly List<int> BaseHeightW = new() { 1900, 2000 };
        private static readonly List<int> BaseLengthsW = new() { 700, 800, 900, 1000, 1200, 1400 };
        private static readonly List<CabinThicknessEnum> BaseThicknessesW = new() { CabinThicknessEnum.Thick6mm, CabinThicknessEnum.Thick8mm, CabinThicknessEnum.Thick10mm };

        /// <summary>
        /// Nominal Length - Price -- W Height:200cm, 8mm Transparent , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceW_8_200_Transp_Chrome = new()
        {
            {   0,  250 },              //Min Price
            { 700,  320 },              //OLD PRICES { 700,  270 },
            { 800,  325 },              //           { 800,  275 },
            { 900,  330 },              //           { 900,  280 },
            { 1000, 345 },              //           { 1000, 290 },
            { 1200, 360 },              //           { 1200, 305 },
            { 1400, 390 }               //           { 1400, 330 }
        };

        /// <summary>
        /// Nominal Length - Price -- W Height:200cm, 8mm Special , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceW_8_200_Special_Chrome = new()
        {
            {   0,  325 },                  //Min Price
            { 700,  405 },                  //OLD PRICES { 700, 340 },
            { 800,  420 },                  //           { 800, 355 },
            { 900,  430 },                  //           { 900, 365 },
            { 1000, 450 },                  //           { 1000, 380 },
            { 1200, 485 },                  //           { 1200, 410 },
            { 1400, 525 }                   //           { 1400, 445 }
        };

        /// <summary>
        /// Nominal Length - Price -- W Height:200cm, 6mm Transparent , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceW_6_200_Transp_Chrome = new()
        {
            {   0,   240 },             //Min Price
            { 700,   300 },             //OLD PRICES { 700,   240 },
            { 800,   305 },             //           { 800,   245 },
            { 900,   315 },             //           { 900,   250 },
            { 1000,  325 },             //           { 1000,  260 },    
            { 1200,  9999},             //           { 1200,  280 },    NOT AVAILABLE DUE TO LENGTH CONSTRAINT
            { 1400,  9999}              //           { 1400,  300 }     NOT AVAILABLE DUE TO LENGTH CONSTRAINT
        };

        /// <summary>
        /// Nominal Length - Price -- 8W Height:200cm, 6mm Special , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceW_6_200_Special_Chrome = new()
        {
            {   0,  315 },              //Min Price
            { 700,  380 },              //OLD PRICE { 700,  305 },
            { 800,  400 },              //          { 800,  320 },
            { 900,  415 },              //          { 900,  330 },
            { 1000, 430 },              //          { 1000, 345 },  NOT AVAILABLE DUE TO CONSTRAINTS
            { 1200, 9999},              //          { 1200, 380 },  NOT AVAILABLE DUE TO CONSTRAINTS
            { 1400, 9999}               //          { 1400, 415 }   NOT AVAILABLE DUE TO CONSTRAINTS
        };

        //*******************FOR DEPRECATION*******************
        /// <summary>
        /// Nominal Length - Price -- W Height:200cm, 10mm Transparent , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceW_10_200_Transp_Chrome = new()
        {
            //8mm PRICE + PREMIUM
            {   0, BasePriceW_8_200_Transp_Chrome[  0]  * 1.2m }, //Min Price
            { 700, BasePriceW_8_200_Transp_Chrome[700]  + W_10mm_Premium }, //{  700,  420 }, ** OLD PRICE {  700,  350 },
            { 800, BasePriceW_8_200_Transp_Chrome[800]  + W_10mm_Premium }, //{  800,  425 }, **           {  800,  355 },
            { 900, BasePriceW_8_200_Transp_Chrome[900]  + W_10mm_Premium }, //{  900,  430 }, **           {  900,  360 },
            { 1000,BasePriceW_8_200_Transp_Chrome[1000] + W_10mm_Premium }, //{ 1000,  445 }, **           { 1000,  370 },
            { 1200,BasePriceW_8_200_Transp_Chrome[1200] + W_10mm_Premium }, //{ 1200,  460 }, **           { 1200,  385 },
            { 1400,BasePriceW_8_200_Transp_Chrome[1400] + W_10mm_Premium }  //{ 1400,  490 }  **           { 1400,  410 }
        };
        //*******************FOR DEPRECATION*******************
        /// <summary>
        /// Nominal Length - Price -- W Height:200cm, 10mm Special , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceW_10_200_Special_Chrome = new()
        {
            //8mm + PREMIUM
            {    0, BasePriceW_8_200_Special_Chrome[  0]  * 1.2m }, //Min Price
            {  700, BasePriceW_8_200_Special_Chrome[700]  + W_10mm_Premium }, //{ 700,  420 }, ** OLD PRICES  {  700, 420 },
            {  800, BasePriceW_8_200_Special_Chrome[800]  + W_10mm_Premium }, //{ 800,  435 }, **             {  800, 435 },
            {  900, BasePriceW_8_200_Special_Chrome[900]  + W_10mm_Premium }, //{ 900,  445 }, **             {  900, 445 },
            { 1000, BasePriceW_8_200_Special_Chrome[1000] + W_10mm_Premium }, //{ 1000, 460 }, **             { 1000, 460 },
            { 1200, BasePriceW_8_200_Special_Chrome[1200] + W_10mm_Premium }, //{ 1200, 490 }, **             { 1200, 490 },
            { 1400, BasePriceW_8_200_Special_Chrome[1400] + W_10mm_Premium }, //{ 1400, 525 }  **             { 1400, 525 } 
        };


        /// <summary>
        /// Nominal Length - Price -- W Height:200cm, 8mm Transparent , BLACK
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceW_8_200_Transp_Black = new()
        {
            {   0,  330 },//Min Price
            { 700,  390 },
            { 800,  395 },
            { 900,  405 },
            { 1000, 415 },
            { 1200, 430 },
            { 1400, 460 }
        };

        /// <summary>
        /// Nominal Length - Price -- W Height:200cm, 8mm Special , BLACK
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceW_8_200_Special_Black = new()
        {
            {   0,  430 },//Min Price
            { 700,  475 },
            { 800,  490 },
            { 900,  505 },
            { 1000, 520 },
            { 1200, 555 },
            { 1400, 600 } 
        };

        //*******************FOR DEPRECATION*******************
        /// <summary>
        /// Nominal Length - Price -- W Height:200cm, 10mm Transparent , Black
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceW_10_200_Transp_Black = new()
        {
            //8mm PRICE + PREMIUM
            {   0, BasePriceW_8_200_Transp_Black[  0]  * 1.2m }, //Min Price
            { 700, BasePriceW_8_200_Transp_Black[700]  + W_10mm_Premium },
            { 800, BasePriceW_8_200_Transp_Black[800]  + W_10mm_Premium },
            { 900, BasePriceW_8_200_Transp_Black[900]  + W_10mm_Premium },
            { 1000,BasePriceW_8_200_Transp_Black[1000] + W_10mm_Premium },
            { 1200,BasePriceW_8_200_Transp_Black[1200] + W_10mm_Premium },
            { 1400,BasePriceW_8_200_Transp_Black[1400] + W_10mm_Premium } 
        };
        //*******************FOR DEPRECATION*******************
        /// <summary>
        /// Nominal Length - Price -- W Height:200cm, 10mm Special , BLACK
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceW_10_200_Special_Black = new()
        {
            //8mm + PREMIUM
            {    0, BasePriceW_8_200_Special_Black[  0]  * 1.2m }, //Min Price
            {  700, BasePriceW_8_200_Special_Black[700]  + W_10mm_Premium },
            {  800, BasePriceW_8_200_Special_Black[800]  + W_10mm_Premium },
            {  900, BasePriceW_8_200_Special_Black[900]  + W_10mm_Premium },
            { 1000, BasePriceW_8_200_Special_Black[1000] + W_10mm_Premium },
            { 1200, BasePriceW_8_200_Special_Black[1200] + W_10mm_Premium },
            { 1400, BasePriceW_8_200_Special_Black[1400] + W_10mm_Premium },
        };
        
        /// <summary>
        /// The Dictionary containing the Various Combinations of Base Prices for W Model
        /// Must be Declared after the BaseCombinations are declared ,otherwise the nested Dictionaries Throw Null Exceptions (They have not been created yet when called)
        /// </summary>
        private static readonly Dictionary<(CabinThicknessEnum, GlassFinishEnum), Dictionary<int, decimal>> BasePricelistsW = new()
        {
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Transparent),   BasePriceW_6_200_Transp_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Serigraphy),    BasePriceW_6_200_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Special),       BasePriceW_6_200_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Satin),         BasePriceW_6_200_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Fume),          BasePriceW_6_200_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Frosted),       BasePriceW_6_200_Special_Chrome },

            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Transparent),   BasePriceW_8_200_Transp_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Serigraphy),    BasePriceW_8_200_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Special),       BasePriceW_8_200_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Satin),         BasePriceW_8_200_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Fume),          BasePriceW_8_200_Special_Chrome },

            //Changed 17-10-2022 Takes Price from 8mm Catalogue there is No Catalogue 10
            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Transparent),  BasePriceW_8_200_Transp_Chrome },
            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Serigraphy),   BasePriceW_8_200_Special_Chrome },
            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Special),      BasePriceW_8_200_Special_Chrome },
            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Satin),        BasePriceW_8_200_Special_Chrome },
            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Fume),         BasePriceW_8_200_Special_Chrome },
        };

        /// <summary>
        /// The Extra Cost of Finish 8W Model
        /// </summary>
        private static readonly Dictionary<CabinFinishEnum, decimal?> FinishPricingW = new()
        {
            { CabinFinishEnum.Polished,       0 },
            { CabinFinishEnum.Brushed,      160 },//Anodizing
            { CabinFinishEnum.BlackMat,       0 },//RAL Takes Price from the Basic 8mm Black Model 22-09-2022
            { CabinFinishEnum.WhiteMat,     105 },//RAL
            { CabinFinishEnum.Bronze,       235 },//Anodizing
            { CabinFinishEnum.BrushedGold,  235 },//Anodizing
            { CabinFinishEnum.Gold,         645 },//PLATING
            { CabinFinishEnum.Copper,       645 },//PLATING
            { CabinFinishEnum.Special,      null },//Not Available
            { CabinFinishEnum.NotSet,       null } //Not Available
        };

        /// <summary>
        /// All around exra Frame Price for 8W
        /// </summary>
        public static readonly Dictionary<CabinFinishEnum, decimal> W_Frame_Pricelist = new()
        {
            { CabinFinishEnum.Polished,     120 },
            { CabinFinishEnum.Brushed,      120 },//Anodizing
            { CabinFinishEnum.BlackMat,     120 },//RAL Takes Price from the Basic 8mm Black Model 22-09-2022
            { CabinFinishEnum.WhiteMat,     120 },//RAL
            { CabinFinishEnum.Bronze,       355 },//Anodizing
            { CabinFinishEnum.BrushedGold,  355 },//Anodizing
            { CabinFinishEnum.Gold,         685 },//PLATING
            { CabinFinishEnum.Copper,       685 },//PLATING
            { CabinFinishEnum.Special,      9999},//Not Available
            { CabinFinishEnum.NotSet,       9999} //Not Available
        };

        #endregion

        #region 2.2 8E Pricelist - UPDATED PRICES 12-06-2022

        private static readonly decimal BronzeClean8E = 60;
        private static readonly decimal SafeKids8E = 85;
        private static readonly decimal CustomizeFactorE = 0.15m;

        private static readonly List<int> BaseHeightE = new() { 1900, 2000 };
        private static readonly List<int> BaseLengthsE = new() { 700, 800, 900, 1000, 1200, 1400 };
        private static readonly List<CabinThicknessEnum> BaseThicknessesE = new() { CabinThicknessEnum.Thick6mm, CabinThicknessEnum.Thick8mm, CabinThicknessEnum.Thick10mm };

        /// <summary>
        /// Nominal Length - Price -- E Height:200cm, 8mm Transparent , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceE_8_200_Transp_Chrome = new()
        {
            {    0, 250 },                  //Min Price
            {  700, 320 },                  //OLD PRICES {  700, 270 },
            {  800, 325 },                  //           {  800, 275 },
            {  900, 330 },                  //           {  900, 280 },
            { 1000, 345 },                  //           { 1000, 290 },
            { 1200, 360 },                  //           { 1200, 305 },
            { 1400, 390 }                   //           { 1400, 330 }
        };

        /// <summary>
        /// Nominal Length - Price -- E Height:200cm, 8mm Special , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceE_8_200_Special_Chrome = new()
        {
            {    0, 325 },                  //Min Price
            {  700, 405 },                  //OLD PRICES {  700, 340 },
            {  800, 420 },                  //           {  800, 355 },
            {  900, 430 },                  //           {  900, 365 },
            { 1000, 450 },                  //           { 1000, 380 },
            { 1200, 485 },                  //           { 1200, 410 },
            { 1400, 525 }                   //           { 1400, 445 }
        };

        //*********************************************************************************************
        //*******SHOULD BE DEPRECATED AT 6mm**************SHOULD BE DEPRECATED AT 6mm******************
        /// <summary>
        /// Nominal Length - Price -- E Height:200cm, 6mm Transparent , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceE_6_200_Transp_Chrome = new()
        {
            {  700, 9999 },
            {  800, 9999 },
            {  900, 9999 },
            { 1000, 9999 },
            { 1200, 9999 }
        };
        
        /// <summary>
        /// Nominal Length - Price -- E Height:200cm, 6mm Special , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceE_6_200_Special_Chrome = new()
        {
            {  700, 9999 },
            {  800, 9999 },
            {  900, 9999 },
            { 1000, 9999 },
            { 1200, 9999 }
        };
        //*********************************************************************************************
        //*********************************************************************************************

        //***********************FOR DEPRECATION NOT USED ANYMORE***********************
        /// <summary>
        /// Nominal Length - Price -- E Height:200cm, 10mm Transparent , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceE_10_200_Transp_Chrome = new()
        {
            //8mm + W10mm Premium
            {    0, BasePriceE_8_200_Transp_Chrome[  0]  * 1.2m }, //Min Price
            {  700, BasePriceE_8_200_Transp_Chrome[700]  + W_10mm_Premium },              //{  700, 420 }, ** OLD PRICES { 700, 350 },
            {  800, BasePriceE_8_200_Transp_Chrome[800]  + W_10mm_Premium },              //{  800, 425 }, **            { 800, 255 },
            {  900, BasePriceE_8_200_Transp_Chrome[900]  + W_10mm_Premium },              //{  900, 430 }, **            { 900, 360 },
            { 1000, BasePriceE_8_200_Transp_Chrome[1000] + W_10mm_Premium },              //{ 1000, 445 }, **            { 1000, 370 },
            { 1200, BasePriceE_8_200_Transp_Chrome[1200] + W_10mm_Premium },              //{ 1200, 460 }, **            { 1200, 385 },
            { 1400, BasePriceE_8_200_Transp_Chrome[1400] + W_10mm_Premium }               //{ 1400, 490 }  **            { 1400, 410 }
        };
        //***********************FOR DEPRECATION NOT USED ANYMORE***********************
        /// <summary>
        /// Nominal Length - Price -- E Height:200cm, 10mm Special , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceE_10_200_Special_Chrome = new()
        {
            {    0, BasePriceE_8_200_Special_Chrome[  0]  * 1.2m },  //Min Price
            {  700, BasePriceE_8_200_Special_Chrome[700]  + W_10mm_Premium },  //{  700, 420 }, ** OLD PRICES { 700, 420 },
            {  800, BasePriceE_8_200_Special_Chrome[800]  + W_10mm_Premium },  //{  800, 435 }, **            { 800, 435 },
            {  900, BasePriceE_8_200_Special_Chrome[900]  + W_10mm_Premium },  //{  900, 445 }, **            { 900, 445 },
            { 1000, BasePriceE_8_200_Special_Chrome[1000] + W_10mm_Premium },  //{ 1000, 460 }, **            { 1000, 460 },
            { 1200, BasePriceE_8_200_Special_Chrome[1200] + W_10mm_Premium },  //{ 1200, 490 }, **            { 1200, 490 },
            { 1400, BasePriceE_8_200_Special_Chrome[1400] + W_10mm_Premium }   //{ 1400, 525 }  **           { 1400, 525 }
        };

        /// <summary>
        /// The Dictionary containing the Various Combinations of Base Prices for E Model
        /// Must be Declared after the BaseCombinations are declared ,otherwise the nested Dictionaries Throw Null Exceptions (They have not been created yet when called)
        /// </summary>
        private static readonly Dictionary<(CabinThicknessEnum, GlassFinishEnum), Dictionary<int, decimal>> BasePricelistsE = new()
        {
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Transparent),   BasePriceE_6_200_Transp_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Serigraphy),    BasePriceE_6_200_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Special),       BasePriceE_6_200_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Satin),         BasePriceE_6_200_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Fume),          BasePriceE_6_200_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Frosted),       BasePriceE_6_200_Special_Chrome },

            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Transparent),   BasePriceE_8_200_Transp_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Serigraphy),    BasePriceE_8_200_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Special),       BasePriceE_8_200_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Satin),         BasePriceE_8_200_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Fume),          BasePriceE_8_200_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Frosted),       BasePriceE_8_200_Special_Chrome },

            //Changed at 17-10-2022
            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Transparent),  BasePriceE_8_200_Transp_Chrome },
            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Serigraphy),   BasePriceE_8_200_Special_Chrome },
            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Special),      BasePriceE_8_200_Special_Chrome },
            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Satin),        BasePriceE_8_200_Special_Chrome },
            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Fume),         BasePriceE_8_200_Special_Chrome },
        };

        /// <summary>
        /// The Extra Cost of Finish E Model
        /// </summary>
        private static readonly Dictionary<CabinFinishEnum, decimal?> FinishPricingE = new()
        {
            { CabinFinishEnum.Polished,       0 },
            { CabinFinishEnum.Brushed,      145 },//Anodizing
            { CabinFinishEnum.BlackMat,     130 },//RAL
            { CabinFinishEnum.WhiteMat,     130 },//RAL
            { CabinFinishEnum.Bronze,       145 },//Anodizing
            { CabinFinishEnum.BrushedGold,  145 },//Anodizing
            { CabinFinishEnum.Gold,         250 },//PLATING
            { CabinFinishEnum.Copper,       250 },//PLATING
            { CabinFinishEnum.Special,      null },//Not Available
            { CabinFinishEnum.NotSet,       null } //Not Available
        };

        #endregion

        #region 2.3 8WFlipper Pricelist - UPDATED PRICES - 12-06-2022

        private static readonly decimal BronzeClean8WFlipper = 60;
        private static readonly decimal SafeKids8WFlipper = 85;
        private static readonly decimal CustomizeFactorWFlipper = 0m;

        private static readonly List<int> BaseHeightWFlipper = new() { 1900, 2000 };
        private static readonly List<int> BaseLengthsWFlipper = new() { 320 };
        private static readonly List<CabinThicknessEnum> BaseThicknessesWFlipper = new() { CabinThicknessEnum.Thick6mm };

        /// <summary>
        /// Nominal Length - Price -- 8WFlipper Height:200cm, 8mm Transparent , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePrice8WFlip_6_200_Transp_Chrome = new()
        {
            {   0, 250 },//Min Price
            { 320, 250 } // OLD PRICE { 320, 210 }
        };

        /// <summary>
        /// Nominal Length - Price -- 8WFlipper Height:200cm, 6mm Special , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePrice8WFlip_6_200_Special_Chrome = new()
        {
            {   0, 310 },//Min Price
            { 320, 310 } //OLD PRICE { 320, 260 }
        };

        /// <summary>
        /// The Dictionary containing the Various Combinations of Base Prices for 8WFlipper Model
        /// Must be Declared after the BaseCombinations are declared ,otherwise the nested Dictionaries Throw Null Exceptions (They have not been created yet when called)
        /// </summary>
        private static readonly Dictionary<(CabinThicknessEnum, GlassFinishEnum), Dictionary<int, decimal>> BasePricelists8WFlipper = new()
        {
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Transparent),   BasePrice8WFlip_6_200_Transp_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Serigraphy),    BasePrice8WFlip_6_200_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Special),       BasePrice8WFlip_6_200_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Satin),         BasePrice8WFlip_6_200_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Fume),          BasePrice8WFlip_6_200_Special_Chrome }
        };

        //PRICES FOR THE FLIPPER PANEL ARE NOT IN CATALOGUE!!!
        /// <summary>
        /// The Extra Cost of Finish 8WFlipper Model
        /// </summary>
        private static readonly Dictionary<CabinFinishEnum, decimal?> FinishPricing8WFlipper = new()
        {
            { CabinFinishEnum.Polished,      0 },
            { CabinFinishEnum.Brushed,      30 },//Plating
            { CabinFinishEnum.BlackMat,     30 },//RAL
            { CabinFinishEnum.WhiteMat,     30 },//RAL
            { CabinFinishEnum.Bronze,       50 },//ANODIZING?
            { CabinFinishEnum.BrushedGold,  50 },//ANODIZING?
            { CabinFinishEnum.Gold,         50 },//Pating
            { CabinFinishEnum.Copper,       50 },//Pating
            { CabinFinishEnum.Special,      null },//Not Available
            { CabinFinishEnum.NotSet,       null } //Not Available
        };

        #endregion

        #endregion

        #region 3. INOX304 Pricelists - UPDATED PRICES 12-06-2022

        #region 3.1 VS Pricelist -UPDATED PRICES 12-06-2022

        private static readonly decimal BronzeCleanVS = 70;
        private static readonly decimal SafeKidsVS = 125;
        private static readonly decimal CustomizeFactorVS = 0.15m;

        private static readonly List<int> BaseHeightVS = new() { 1900, 2000 };
        private static readonly List<int> BaseLengthsVS = new() { 1000, 1050, 1100, 1150, 1200, 1300, 1400 };
        private static readonly List<CabinThicknessEnum> BaseThicknessesVS = new() { CabinThicknessEnum.Thick8mm, CabinThicknessEnum.Thick10mm };

        /// <summary>
        /// Nominal Length - Price -- VS Height:190cm, 8mm Transparent , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceVS_8_190_Transp_Chrome = new()
        {
            {    0, 680 },              //Min Price
            { 1000, 680 },              //OLD PRICES { 1000, 575 },
            { 1050, 685 },              //           { 1050, 580 },
            { 1100, 695 },              //           { 1100, 585 },
            { 1150, 705 },              //           { 1150, 595 },
            { 1200, 710 },              //           { 1200, 600 },
            { 1300, 780 },              //           { 1300, 660 },
            { 1400, 800 }               //           { 1400, 675 }
        };

        /// <summary>
        /// Nominal Length - Price -- VS Height:190cm, 8mm Special , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceVS_8_190_Special_Chrome = new()
        {
            {    0, 830 },              //Min Price
            { 1000, 830 },              //OLD PRICES { 1000, 700 },
            { 1050, 840 },              //           { 1050, 710 },
            { 1100, 855 },              //           { 1100, 720 },
            { 1150, 870 },              //           { 1150, 735 },
            { 1200, 880 },              //           { 1200, 745 },
            { 1300, 965 },              //           { 1300, 815 },
            { 1400, 995 }               //           { 1400, 840 }
        };

        /// <summary>
        /// The Dictionary containing the Various Combinations of Base Prices for VS Model
        /// Must be Declared after the BaseCombinations are declared ,otherwise the nested Dictionaries Throw Null Exceptions (They have not been created yet when called)
        /// </summary>
        private static readonly Dictionary<(CabinThicknessEnum, GlassFinishEnum), Dictionary<int, decimal>> BasePricelistsVS = new()
        {
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Transparent),   BasePriceVS_8_190_Transp_Chrome  },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Serigraphy),    BasePriceVS_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Special),       BasePriceVS_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Satin),         BasePriceVS_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Fume),          BasePriceVS_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Frosted),       BasePriceVS_8_190_Special_Chrome },

            { (CabinThicknessEnum.Thick8mm10mm, GlassFinishEnum.Transparent),   BasePriceVS_8_190_Transp_Chrome  },
            { (CabinThicknessEnum.Thick8mm10mm, GlassFinishEnum.Serigraphy),    BasePriceVS_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm10mm, GlassFinishEnum.Special),       BasePriceVS_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm10mm, GlassFinishEnum.Satin),         BasePriceVS_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm10mm, GlassFinishEnum.Fume),          BasePriceVS_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm10mm, GlassFinishEnum.Frosted),       BasePriceVS_8_190_Special_Chrome },

            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Transparent),   BasePriceVS_8_190_Transp_Chrome  },
            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Serigraphy),    BasePriceVS_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Special),       BasePriceVS_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Satin),         BasePriceVS_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Fume),          BasePriceVS_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Frosted),       BasePriceVS_8_190_Special_Chrome },
        };

        /// <summary>
        /// The Extra Cost of Finish VS Model
        /// </summary>
        private static readonly Dictionary<CabinFinishEnum, decimal?> FinishPricingVS = new()
        {
            { CabinFinishEnum.Polished,       0 },
            { CabinFinishEnum.Brushed,      170 },//Plating --TOO MUCH
            { CabinFinishEnum.BlackMat,     170 },//RAL
            { CabinFinishEnum.WhiteMat,     170 },//RAL
            { CabinFinishEnum.Bronze,       775 },//Plating
            { CabinFinishEnum.BrushedGold,  775 },//Plating
            { CabinFinishEnum.Gold,         775 },//Plating
            { CabinFinishEnum.Copper,       775 },//Plating
            { CabinFinishEnum.Special,      null },//Not Available
            { CabinFinishEnum.NotSet,       null } //Not Available
        };

        #endregion       

        #region 3.2 V4 Pricelist - UPDATED PRICES 12-06-2022

        private static readonly decimal BronzeCleanV4 = 70;
        private static readonly decimal SafeKidsV4 = 250;
        private static readonly decimal CustomizeFactorV4 = 0.15m;

        private static readonly List<int> BaseHeightV4 = new() { 1900, 2000 };
        private static readonly List<int> BaseLengthsV4 = new() { 1500, 1600, 1700, 1800, 2000 };
        private static readonly List<CabinThicknessEnum> BaseThicknessesV4 = new() { CabinThicknessEnum.Thick8mm, CabinThicknessEnum.Thick10mm };

        /// <summary>
        /// Nominal Length - Price -- V4 Height:190cm, 8mm Transparent , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceV4_8_190_Transp_Chrome = new()
        {
            {    0, 1085 },          //Min Price
            { 1500, 1085 },          //OLD PRICE { 1500, 915 },
            { 1600, 1120 },          //          { 1600, 945 },
            { 1700, 1135 },          //          { 1700, 960 },
            { 1800, 1160 },          //          { 1800, 980 },
            { 2000, 1180 }           //          { 2000, 995 }
        };

        /// <summary>
        /// Nominal Length - Price -- V4 Height:190cm, 8mm Special , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceV4_8_190_Special_Chrome = new()
        {
            {    0, 1290 },                 //Min Price
            { 1500, 1290 },                 //OLD PRICES { 1500, 1090 },
            { 1600, 1340 },                 //           { 1600, 1130 },
            { 1700, 1370 },                 //           { 1700, 1155 },
            { 1800, 1390 },                 //           { 1800, 1175 },
            { 2000, 1420 }                  //           { 2000, 1200 }
        };

        /// <summary>
        /// The Dictionary containing the Various Combinations of Base Prices for V4 Model
        /// Must be Declared after the BaseCombinations are declared ,otherwise the nested Dictionaries Throw Null Exceptions (They have not been created yet when called)
        /// </summary>
        private static readonly Dictionary<(CabinThicknessEnum, GlassFinishEnum), Dictionary<int, decimal>> BasePricelistsV4 = new()
        {
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Transparent),   BasePriceV4_8_190_Transp_Chrome  },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Serigraphy),    BasePriceV4_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Special),       BasePriceV4_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Satin),         BasePriceV4_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Fume),          BasePriceV4_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Frosted),       BasePriceV4_8_190_Special_Chrome },

            { (CabinThicknessEnum.Thick8mm10mm, GlassFinishEnum.Transparent),   BasePriceV4_8_190_Transp_Chrome  },
            { (CabinThicknessEnum.Thick8mm10mm, GlassFinishEnum.Serigraphy),    BasePriceV4_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm10mm, GlassFinishEnum.Special),       BasePriceV4_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm10mm, GlassFinishEnum.Satin),         BasePriceV4_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm10mm, GlassFinishEnum.Fume),          BasePriceV4_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm10mm, GlassFinishEnum.Frosted),       BasePriceV4_8_190_Special_Chrome },

            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Transparent),   BasePriceV4_8_190_Transp_Chrome  },
            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Serigraphy),    BasePriceV4_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Special),       BasePriceV4_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Satin),         BasePriceV4_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Fume),          BasePriceV4_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Frosted),       BasePriceV4_8_190_Special_Chrome },
        };

        /// <summary>
        /// The Extra Cost of Finish V4 Model
        /// </summary>
        private static readonly Dictionary<CabinFinishEnum, decimal?> FinishPricingV4 = new()
        {
            { CabinFinishEnum.Polished,       0 },
            { CabinFinishEnum.Brushed,      250 },//Plating --TOO MUCH
            { CabinFinishEnum.BlackMat,     250 },//RAL
            { CabinFinishEnum.WhiteMat,     250 },//RAL
            { CabinFinishEnum.Bronze,       985 },//Plating
            { CabinFinishEnum.BrushedGold,  985 },//Plating
            { CabinFinishEnum.Gold,         985 },//Plating
            { CabinFinishEnum.Copper,       985 },//Plating
            { CabinFinishEnum.Special,      null },//Not Available
            { CabinFinishEnum.NotSet,       null } //Not Available
        };

        #endregion 

        #region 3.3 VA Pricelist - UPDATED PRICES  12-06-2022

        private static readonly decimal BronzeCleanVA = 70;
        private static readonly decimal SafeKidsVA = 105;
        private static readonly decimal CustomizeFactorVA = 0.15m;

        private static readonly List<int> BaseHeightVA = new() { 1900, 2000 };
        private static readonly List<int> BaseLengthsVA = new() { 700, 750, 800, 850, 900 };
        private static readonly List<CabinThicknessEnum> BaseThicknessesVA = new() { CabinThicknessEnum.Thick8mm };

        /// <summary>
        /// Nominal Length - Price -- VA Height:190cm, 8mm Transparent , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceVA_8_190_Transp_Chrome = new()
        {
            {   0, 560 },
            { 700, 560 },
            { 750, 570 },
            { 800, 580 },
            { 850, 590 },
            { 900, 595 },
        };

        /// <summary>
        /// Nominal Length - Price -- VA Height:190cm, 8mm Special , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceVA_8_190_Special_Chrome = new()
        {
            {   0, 655 },
            { 700, 655 },
            { 750, 670 },
            { 800, 680 },
            { 850, 700 },
            { 900, 705 },
        };

        /// <summary>
        /// The Dictionary containing the Various Combinations of Base Prices for VA Model
        /// Must be Declared after the BaseCombinations are declared ,otherwise the nested Dictionaries Throw Null Exceptions (They have not been created yet when called)
        /// </summary>
        private static readonly Dictionary<(CabinThicknessEnum, GlassFinishEnum), Dictionary<int, decimal>> BasePricelistsVA = new()
        {
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Transparent),   BasePriceVA_8_190_Transp_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Serigraphy),    BasePriceVA_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Special),       BasePriceVA_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Satin),         BasePriceVA_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Fume),          BasePriceVA_8_190_Special_Chrome },

            { (CabinThicknessEnum.Thick8mm10mm, GlassFinishEnum.Transparent),   BasePriceVA_8_190_Transp_Chrome },
            { (CabinThicknessEnum.Thick8mm10mm, GlassFinishEnum.Serigraphy),    BasePriceVA_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm10mm, GlassFinishEnum.Special),       BasePriceVA_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm10mm, GlassFinishEnum.Satin),         BasePriceVA_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm10mm, GlassFinishEnum.Fume),          BasePriceVA_8_190_Special_Chrome },
        };

        /// <summary>
        /// The Extra Cost of Finish VA Model
        /// </summary>
        private static readonly Dictionary<CabinFinishEnum, decimal?> FinishPricingVA = new()
        {
            { CabinFinishEnum.Polished,       0 },
            { CabinFinishEnum.Brushed,      125 },//Plating --TOO MUCH
            { CabinFinishEnum.BlackMat,     125 },//RAL
            { CabinFinishEnum.WhiteMat,     125 },//RAL
            { CabinFinishEnum.Bronze,       495 },//Plating
            { CabinFinishEnum.BrushedGold,  495 },//Plating
            { CabinFinishEnum.Gold,         495 },//Plating
            { CabinFinishEnum.Copper,       495 },//Plating
            { CabinFinishEnum.Special,      null },//Not Available
            { CabinFinishEnum.NotSet,       null } //Not Available
        };

        #endregion 

        #region 3.4 VF Pricelist - UPDATED PRICES 12-06-2022

        private static readonly decimal BronzeCleanVF = 60;
        private static readonly decimal SafeKidsVF = 85;
        private static readonly decimal CustomizeFactorVF = 0.15m;

        private static readonly List<int> BaseHeightVF = new() { 1900, 2000 };
        private static readonly List<int> BaseLengthsVF = new() { 700, 800, 900, 1000, 1100 };
        private static readonly List<CabinThicknessEnum> BaseThicknessesVF = new() { CabinThicknessEnum.Thick8mm, CabinThicknessEnum.Thick10mm };

        /// <summary>
        /// Nominal Length - Price -- VF Height:190cm, 8mm Transparent , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceVF_8_190_Transp_Chrome = new()
        {
            {   0,  250 }, //Min Price
            {  700, 280 },              //OLD PRICES { 700, 155 },
            {  800, 295 },              //           { 800, 180 },
            {  900, 310 },              //           { 900, 200 },
            { 1000, 325 },              //           { 1000, 225 },
            { 1100, 340 }               //           { 1100, 250 }
        };

        /// <summary>
        /// Nominal Length - Price -- VF Height:190cm, 8mm Special , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceVF_8_190_Special_Chrome = new()
        {
            {   0,  385 }, //Min Price
            {  700, 385 },              //OLD PRICES {  700, 210 },
            {  800, 395 },              //           {  800, 240 },
            {  900, 405 },              //           {  900, 280 },
            { 1000, 420 },              //           { 1000, 310 },
            { 1100, 435 }               //           { 1100, 340 }
        };

        /// <summary>
        /// The Dictionary containing the Various Combinations of Base Prices for VF Model
        /// Must be Declared after the BaseCombinations are declared ,otherwise the nested Dictionaries Throw Null Exceptions (They have not been created yet when called)
        /// </summary>
        private static readonly Dictionary<(CabinThicknessEnum, GlassFinishEnum), Dictionary<int, decimal>> BasePricelistsVF = new()
        {
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Transparent),   BasePriceVF_8_190_Transp_Chrome  },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Serigraphy),    BasePriceVF_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Special),       BasePriceVF_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Satin),         BasePriceVF_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Fume),          BasePriceVF_8_190_Special_Chrome },

            { (CabinThicknessEnum.Thick8mm10mm, GlassFinishEnum.Transparent),   BasePriceVF_8_190_Transp_Chrome  },
            { (CabinThicknessEnum.Thick8mm10mm, GlassFinishEnum.Serigraphy),    BasePriceVF_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm10mm, GlassFinishEnum.Special),       BasePriceVF_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm10mm, GlassFinishEnum.Satin),         BasePriceVF_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm10mm, GlassFinishEnum.Fume),          BasePriceVF_8_190_Special_Chrome },

            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Transparent),   BasePriceVF_8_190_Transp_Chrome  },
            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Serigraphy),    BasePriceVF_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Special),       BasePriceVF_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Satin),         BasePriceVF_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Fume),          BasePriceVF_8_190_Special_Chrome },
        };

        /// <summary>
        /// The Extra Cost of Finish VF Model
        /// </summary>
        private static readonly Dictionary<CabinFinishEnum, decimal?> FinishPricingVF = new()
        {
            { CabinFinishEnum.Polished,       0 },
            { CabinFinishEnum.Brushed,      125 },//Plating --TOO MUCH TOO TOO MUCH
            { CabinFinishEnum.BlackMat,     125 },//RAL --TOO MUCH
            { CabinFinishEnum.WhiteMat,     125 },//RAL --TOO MUCH
            { CabinFinishEnum.Bronze,       250 },//Plating --TOO MUCH TOO TOO MUCH
            { CabinFinishEnum.BrushedGold,  250 },//Plating --TOO MUCH TOO TOO MUCH
            { CabinFinishEnum.Gold,         250 },//Plating --TOO MUCH TOO TOO MUCH
            { CabinFinishEnum.Copper,       250 },//Plating --TOO MUCH TOO TOO MUCH
            { CabinFinishEnum.Special,      null },//Not Available
            { CabinFinishEnum.NotSet,       null } //Not Available
        };

        #endregion

        #endregion

        #region 4.NB-QB Pricelist - UPDATED PRICES 16-12-2022

        private static readonly decimal BronzeCleanNB = 60;
        private static readonly decimal SafeKidsNB = 85;
        private static readonly decimal CustomizeFactorNB = 0.15m;
        private static readonly decimal BronzeCleanQB = 60;
        private static readonly decimal SafeKidsQB = 85;
        private static readonly decimal CustomizeFactorQB = 0.15m;

        private static readonly List<int> BaseHeightNB = new() { 1900 };
        private static readonly List<int> BaseLengthsNB = new() { 350, 700, 750, 800, 850, 900 };
        private static readonly List<int> BaseHeightQB = new() { 1900 };
        private static readonly List<int> BaseLengthsQB = new() { 350, 700, 750, 800, 850, 900 };
        private static readonly List<CabinThicknessEnum> BaseThicknessesNB = new() { CabinThicknessEnum.Thick6mm };
        private static readonly List<CabinThicknessEnum> BaseThicknessesQB = new() { CabinThicknessEnum.Thick6mm };

        /// <summary>
        /// Nominal Length - Price -- NB Height:190cm, 6mm Transparent , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceNB_6_190_Transp_Chrome = new()
        {
            {   0, 285 }, //Min Price
            { 350, 285 },               //OLD PRICES { 350, 240 },
            { 700, 375 },               //           { 700, 315 },
            { 750, 385 },               //           { 750, 325 },
            { 800, 405 },               //           { 800, 340 },
            { 850, 410 },               //           { 850, 345 },
            { 900, 420 }                //           { 900, 355 }
        };
        /// <summary>
        /// Nominal Length - Price -- QB Height:190cm, 6mm Transparent , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceQB_6_190_Transp_Chrome = new()
        {
            {   0, 345 }, //Min Price
            { 350, 345 },            
            { 700, 430 },            
            { 750, 445 },            
            { 800, 460 },            
            { 850, 470 },            
            { 900, 480 }             
        };

        /// <summary>
        /// Nominal Length - Price -- NB Height:190cm, 6mm Special , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceNB_6_190_Special_Chrome = new()
        {
            {   0, 390 }, //Min Price
            { 350, 390 },                   //OLD PRICES { 350, 295 },
            { 700, 515 },                   //           { 700, 390 },
            { 750, 540 },                   //           { 750, 410 },
            { 800, 560 },                   //           { 800, 425 },
            { 850, 580 },                   //           { 850, 440 },
            { 900, 590 }                    //           { 900, 450 }
        };
        /// <summary>
        /// Nominal Length - Price -- QB Height:190cm, 6mm Special , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceQB_6_190_Special_Chrome = new()
        {
            {   0, 505 }, //Min Price
            { 350, 505 },            
            { 700, 630 },            
            { 750, 660 },            
            { 800, 680 },            
            { 850, 695 },            
            { 900, 710 }             
        };

        /// <summary>
        /// The Dictionary containing the Various Combinations of Base Prices for NB Model
        /// Must be Declared after the BaseCombinations are declared ,otherwise the nested Dictionaries Throw Null Exceptions (They have not been created yet when called)
        /// </summary>
        private static readonly Dictionary<(CabinThicknessEnum, GlassFinishEnum), Dictionary<int, decimal>> BasePricelistsNB = new()
        {
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Transparent),   BasePriceNB_6_190_Transp_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Serigraphy),    BasePriceNB_6_190_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Special),       BasePriceNB_6_190_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Satin),         BasePriceNB_6_190_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Fume),          BasePriceNB_6_190_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Frosted),       BasePriceNB_6_190_Special_Chrome },
        };
        /// <summary>
        /// The Dictionary containing the Various Combinations of Base Prices for QB Model
        /// Must be Declared after the BaseCombinations are declared ,otherwise the nested Dictionaries Throw Null Exceptions (They have not been created yet when called)
        /// </summary>
        private static readonly Dictionary<(CabinThicknessEnum, GlassFinishEnum), Dictionary<int, decimal>> BasePricelistsQB = new()
        {
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Transparent),   BasePriceQB_6_190_Transp_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Serigraphy),    BasePriceQB_6_190_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Special),       BasePriceQB_6_190_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Satin),         BasePriceQB_6_190_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Fume),          BasePriceQB_6_190_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Frosted),       BasePriceQB_6_190_Special_Chrome },
        };

        /// <summary>
        /// The Extra Cost of Finish NB Model
        /// </summary>
        private static readonly Dictionary<CabinFinishEnum, decimal?> FinishPricingNB = new()
        {
            { CabinFinishEnum.Polished,       0 },
            { CabinFinishEnum.Brushed,      185 },//Anodizing MISTAKE PRICE
            { CabinFinishEnum.BlackMat,     120 },//RAL
            { CabinFinishEnum.WhiteMat,     120 },//RAL
            { CabinFinishEnum.Bronze,       210 },//Anodizing MISTAKE PRICE
            { CabinFinishEnum.BrushedGold,  210 },//Anodizing MISTAKE PRICE
            { CabinFinishEnum.Gold,         null },//Not Available
            { CabinFinishEnum.Copper,       null },//Not Available
            { CabinFinishEnum.Special,      null },//Not Available
            { CabinFinishEnum.NotSet,       null } //Not Available
        };
        /// <summary>
        /// The Extra Cost of Finish QB Model
        /// </summary>
        private static readonly Dictionary<CabinFinishEnum, decimal?> FinishPricingQB = new()
        {
            { CabinFinishEnum.Polished,       0 },
            { CabinFinishEnum.Brushed,      185 },//Anodizing MISTAKE PRICE
            { CabinFinishEnum.BlackMat,     120 },//RAL
            { CabinFinishEnum.WhiteMat,     120 },//RAL
            { CabinFinishEnum.Bronze,       210 },//Anodizing MISTAKE PRICE
            { CabinFinishEnum.BrushedGold,  210 },//Anodizing MISTAKE PRICE
            { CabinFinishEnum.Gold,         null },//Not Available
            { CabinFinishEnum.Copper,       null },//Not Available
            { CabinFinishEnum.Special,      null },//Not Available
            { CabinFinishEnum.NotSet,       null } //Not Available
        };

        #endregion

        #region 5.NP-QP Pricelist - UPDATED PRICES 12-06-2022

        private static readonly decimal BronzeCleanNP = 70;
        private static readonly decimal SafeKidsNP = 125;
        private static readonly decimal CustomizeFactorNP = 0.15m;

        private static readonly List<int> BaseHeightNP = new() { 1900 };
        private static readonly List<int> BaseLengthsNP = new() { 700, 750, 800, 850, 900 };
        private static readonly List<CabinThicknessEnum> BaseThicknessesNP = new() { CabinThicknessEnum.Thick6mm };

        private static readonly decimal BronzeCleanQP = 70;
        private static readonly decimal SafeKidsQP = 125;
        private static readonly decimal CustomizeFactorQP = 0.15m;

        private static readonly List<int> BaseHeightQP = new() { 1900 };
        private static readonly List<int> BaseLengthsQP = new() { 700, 750, 800, 850, 900 };
        private static readonly List<CabinThicknessEnum> BaseThicknessesQP = new() { CabinThicknessEnum.Thick6mm };

        /// <summary>
        /// Nominal Length - Price -- NP Height:190cm, 6mm Transparent , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceNP_6_190_Transp_Chrome = new()
        {
            {   0, 550 }, //Min Price
            { 700, 550 },               //OLD PRICES { 700, 465 },
            { 750, 570 },               //           { 750, 480 },
            { 800, 570 },               //           { 800, 480 },
            { 850, 590 },               //           { 850, 500 },
            { 900, 590 }                //           { 900, 500 }
        };
        /// <summary>
        /// Nominal Length - Price -- QP Height:190cm, 6mm Transparent , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceQP_6_190_Transp_Chrome = new()
        {
            {   0, 645 }, //Min Price
            { 700, 645 },            
            { 750, 665 },            
            { 800, 665 },            
            { 850, 685 },            
            { 900, 685 },
        };

        /// <summary>
        /// Nominal Length - Price -- NP Height:190cm, 6mm Special , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceNP_6_190_Special_Chrome = new()
        {
            {   0, 745 }, //Min Price
            { 700, 745 },               //OLD PRICES { 700, 565 },
            { 750, 770 },               //           { 750, 585 },
            { 800, 775 },               //           { 800, 590 },
            { 850, 805 },               //           { 850, 610 },
            { 900, 810 }                //           { 900, 615 }
        };
        /// <summary>
        /// Nominal Length - Price -- QP Height:190cm, 6mm Special , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceQP_6_190_Special_Chrome = new()
        {
            {   0, 860 }, //Min Price
            { 700, 860 },
            { 750, 890 },
            { 800, 895 },
            { 850, 920 },
            { 900, 930 },
        };

        /// <summary>
        /// The Dictionary containing the Various Combinations of Base Prices for NP Model
        /// Must be Declared after the BaseCombinations are declared ,otherwise the nested Dictionaries Throw Null Exceptions (They have not been created yet when called)
        /// </summary>
        private static readonly Dictionary<(CabinThicknessEnum, GlassFinishEnum), Dictionary<int, decimal>> BasePricelistsNP = new()
        {
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Transparent),   BasePriceNP_6_190_Transp_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Serigraphy),    BasePriceNP_6_190_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Special),       BasePriceNP_6_190_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Satin),         BasePriceNP_6_190_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Fume),          BasePriceNP_6_190_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Frosted),       BasePriceNP_6_190_Special_Chrome },
        };
        /// <summary>
        /// The Dictionary containing the Various Combinations of Base Prices for QP Model
        /// Must be Declared after the BaseCombinations are declared ,otherwise the nested Dictionaries Throw Null Exceptions (They have not been created yet when called)
        /// </summary>
        private static readonly Dictionary<(CabinThicknessEnum, GlassFinishEnum), Dictionary<int, decimal>> BasePricelistsQP = new()
        {
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Transparent),   BasePriceQP_6_190_Transp_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Serigraphy),    BasePriceQP_6_190_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Special),       BasePriceQP_6_190_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Satin),         BasePriceQP_6_190_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Fume),          BasePriceQP_6_190_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Frosted),       BasePriceQP_6_190_Special_Chrome },
        };

        /// <summary>
        /// The Extra Cost of Finish NP Model
        /// </summary>
        private static readonly Dictionary<CabinFinishEnum, decimal?> FinishPricingNP = new()
        {
            { CabinFinishEnum.Polished,       0 },
            { CabinFinishEnum.Brushed,      275 },//Anodizing
            { CabinFinishEnum.BlackMat,     205 },//RAL
            { CabinFinishEnum.WhiteMat,     205 },//RAL
            { CabinFinishEnum.Bronze,       330 },//Anodizing
            { CabinFinishEnum.BrushedGold,  330 },//Anodizing
            { CabinFinishEnum.Gold,         null },//Not Available
            { CabinFinishEnum.Copper,       null },//Not Available
            { CabinFinishEnum.Special,      null },//Not Available
            { CabinFinishEnum.NotSet,       null } //Not Available
        };
        /// <summary>
        /// The Extra Cost of Finish QP Model
        /// </summary>
        private static readonly Dictionary<CabinFinishEnum, decimal?> FinishPricingQP = new()
        {
            { CabinFinishEnum.Polished,       0 },
            { CabinFinishEnum.Brushed,      275 },//Anodizing
            { CabinFinishEnum.BlackMat,     205 },//RAL
            { CabinFinishEnum.WhiteMat,     205 },//RAL
            { CabinFinishEnum.Bronze,       330 },//Anodizing
            { CabinFinishEnum.BrushedGold,  330 },//Anodizing
            { CabinFinishEnum.Gold,         null },//Not Available
            { CabinFinishEnum.Copper,       null },//Not Available
            { CabinFinishEnum.Special,      null },//Not Available
            { CabinFinishEnum.NotSet,       null } //Not Available
        };

        #endregion

        #region 6.WS Pricelist - UPDATED PRICES 12-06-2022

        private static readonly decimal BronzeCleanWS = 70;
        private static readonly decimal SafeKidsWS = 125;
        private static readonly decimal CustomizeFactorWS = 0.15m;

        private static readonly List<int> BaseHeightWS = new() { 1900, 2000 };
        private static readonly List<int> BaseLengthsWS = new() { 700, 800, 900, 1000, 1200 };
        private static readonly List<CabinThicknessEnum> BaseThicknessesWS = new() { CabinThicknessEnum.Thick8mm10mm };

        /// <summary>
        /// Nominal Length - Price -- WS Height:200cm, 8/10mm Transparent , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceWS_810_200_Transp_Chrome = new()
        {
            {   0,  550 }, //Min Price
            {  700, 550 },                  //OLD PRICES {  700, 440 },
            {  800, 570 },                  //           {  800, 455 },
            {  900, 575 },                  //           {  900, 460 },
            { 1000, 585 },                  //           { 1000, 470 },
            { 1200, 605 },                  //           { 1200, 485 },
                                            //           { 1400, 505 } DEPRECATED SIZE
        };

        //*********PRICES HERE ARE NOT FROM CATALOGUE***********
        /// <summary>
        /// Nominal Length - Price -- WS Height:200cm, 8/10mm Special , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceWS_810_200_Special_Chrome = new()
        {
            {   0,  635 }, //Min Price
            {  700, 635 },                  //OLD PRICES {  700, 535 },
            {  800, 665 },                  //           {  800, 550 },
            {  900, 675 },                  //           {  900, 555 },
            { 1000, 690 },                  //           { 1000, 565 },
            { 1200, 730 },                  //           { 1200, 580 },
                                            //           { 1400, 600 } DEPRECATED SIZE
        };
        //******************************************************

        /// <summary>
        /// The Dictionary containing the Various Combinations of Base Prices for WS Model
        /// Must be Declared after the BaseCombinations are declared ,otherwise the nested Dictionaries Throw Null Exceptions (They have not been created yet when called)
        /// </summary>
        private static readonly Dictionary<(CabinThicknessEnum, GlassFinishEnum), Dictionary<int, decimal>> BasePricelistsWS = new()
        {
            { (CabinThicknessEnum.Thick8mm10mm, GlassFinishEnum.Transparent),   BasePriceWS_810_200_Transp_Chrome },
            { (CabinThicknessEnum.Thick8mm10mm, GlassFinishEnum.Satin),         BasePriceWS_810_200_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm10mm, GlassFinishEnum.Fume),          BasePriceWS_810_200_Special_Chrome },
        };

        /// <summary>
        /// The Extra Cost of Finish WS Model
        /// </summary>
        private static readonly Dictionary<CabinFinishEnum, decimal?> FinishPricingWS = new()
        {
            { CabinFinishEnum.Polished,       0 },
            { CabinFinishEnum.Brushed,      225 },//Plating 
            { CabinFinishEnum.BlackMat,     170 },//RAL-- MISTAKE?!?!?!?!
            { CabinFinishEnum.WhiteMat,     170 },//RAL-- MISTAKE?!?!?!?!
            { CabinFinishEnum.Bronze,       765 },//Plating
            { CabinFinishEnum.BrushedGold,  765 },//Plating
            { CabinFinishEnum.Gold,         765 },//Plating
            { CabinFinishEnum.Copper,       765 },//Plating
            { CabinFinishEnum.Special,      null },//Not Available
            { CabinFinishEnum.NotSet,       null } //Not Available
        };

        #endregion

        #region 7.HB Pricelist - UPDATED PRICES 12-06-2022

        private static readonly decimal BronzeCleanHB = 70;
        private static readonly decimal SafeKidsHB = 125;
        private static readonly decimal CustomizeFactorHB = 0.15m;

        private static readonly List<int> BaseHeightHB = new() { 1900, 2000 };
        private static readonly List<int> BaseLengthsHB = new() { 700, 800, 900, 1000, 1100, 1200, 1300 };
        private static readonly List<CabinThicknessEnum> BaseThicknessesHB = new() { CabinThicknessEnum.Thick8mm, CabinThicknessEnum.Thick10mm };

        /// <summary>
        /// Nominal Length - Price -- HB Height:190cm, 8mm Transparent , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceHB_8_190_Transp_Chrome = new()
        {
            {   0,  600 }, //Min Price
            {  700, 655 },              //OLD PRICES {  700, 555 },
            {  800, 685 },              //           {  750, 580 }, DEPRECATED SIZE
            {  900, 705 },              //           {  800, 580 },
            { 1000, 735 },              //           {  850, 595 }, DEPRECATED SIZE
            { 1100, 765 },              //           {  900, 595 },
            { 1200, 795 },              //           {  950, 620 }, DEPRECATED SIZE
            { 1300, 825 }               //           { 1000, 620 }
        };

        //************************FOR DEPRECATION NOT USED ANYMORE************************
        /// <summary>
        /// Nominal Length - Price -- HB Height:190cm, 10mm Transparent , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceHB_10_190_Transp_Chrome = new()
        {
            //Add 10mm Premium
            {    0, BasePriceHB_8_190_Transp_Chrome[  0]  * 1.2m }, //Min Price
            {  700, BasePriceHB_8_190_Transp_Chrome[700]  + W_10mm_Premium },
            {  800, BasePriceHB_8_190_Transp_Chrome[800]  + W_10mm_Premium },
            {  900, BasePriceHB_8_190_Transp_Chrome[900]  + W_10mm_Premium },
            { 1000, BasePriceHB_8_190_Transp_Chrome[1000] + W_10mm_Premium },
            { 1100, BasePriceHB_8_190_Transp_Chrome[1100] + W_10mm_Premium },
            { 1200, BasePriceHB_8_190_Transp_Chrome[1200] + W_10mm_Premium },
            { 1300, BasePriceHB_8_190_Transp_Chrome[1300] + W_10mm_Premium },
        };

        /// <summary>
        /// Nominal Length - Price -- HB Height:190cm, 8mm Special , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceHB_8_190_Special_Chrome = new()
        {
            {    0,  710 }, //Min Price
            {  700,  790 },                  //OLD PRICES {  700, 665 },
            {  800,  830 },                  //           {  750, 700 }, DEPRECATED SIZE
            {  900,  855 },                  //           {  800, 700 },
            { 1000,  900 },                  //           {  850, 720 }, DEPRECATED SIZE
            { 1100,  940 },                  //           {  900, 720 },
            { 1200,  995 },                  //           {  950, 760 }, DEPRECATED SIZE
            { 1300, 1030 }                   //           { 1000, 760 }
        };

        //************************FOR DEPRECATION NOT USED ANYMORE************************
        /// <summary>
        /// Nominal Length - Price -- HB Height:190cm, 10mm Special , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceHB_10_190_Special_Chrome = new()
        {
            //Add 10mm Premium
            {    0, BasePriceHB_8_190_Special_Chrome[  0]  * 1.2m }, // Min Price
            {  700, BasePriceHB_8_190_Special_Chrome[700]  + W_10mm_Premium },
            {  800, BasePriceHB_8_190_Special_Chrome[800]  + W_10mm_Premium },
            {  900, BasePriceHB_8_190_Special_Chrome[900]  + W_10mm_Premium },
            { 1000, BasePriceHB_8_190_Special_Chrome[1000] + W_10mm_Premium },
            { 1100, BasePriceHB_8_190_Special_Chrome[1100] + W_10mm_Premium },
            { 1200, BasePriceHB_8_190_Special_Chrome[1200] + W_10mm_Premium },
            { 1300, BasePriceHB_8_190_Special_Chrome[1300] + W_10mm_Premium },
        };

        /// <summary>
        /// The Dictionary containing the Various Combinations of Base Prices for HB Model
        /// Must be Declared after the BaseCombinations are declared ,otherwise the nested Dictionaries Throw Null Exceptions (They have not been created yet when called)
        /// </summary>
        private static readonly Dictionary<(CabinThicknessEnum, GlassFinishEnum), Dictionary<int, decimal>> BasePricelistsHB = new()
        {
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Transparent),   BasePriceHB_8_190_Transp_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Satin),         BasePriceHB_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Fume),          BasePriceHB_8_190_Special_Chrome },

            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Transparent),  BasePriceHB_8_190_Transp_Chrome },
            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Satin),        BasePriceHB_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Fume),         BasePriceHB_8_190_Special_Chrome },
        };

        /// <summary>
        /// The Extra Cost of Finish HB Model
        /// </summary>
        private static readonly Dictionary<CabinFinishEnum, decimal?> FinishPricingHB = new()
        {
            { CabinFinishEnum.Polished,       0 },
            { CabinFinishEnum.Brushed,      330 },//PLATING -- MISTAKE
            { CabinFinishEnum.BlackMat,     275 },//RAL
            { CabinFinishEnum.WhiteMat,     275 },//RAL
            { CabinFinishEnum.Bronze,       920 },//PLATING -- MISTAKE
            { CabinFinishEnum.BrushedGold,  920 },//PLATING -- MISTAKE
            { CabinFinishEnum.Gold,         920 },//Not Available
            { CabinFinishEnum.Copper,       920 },//Not Available
            { CabinFinishEnum.Special,      null },//Not Available
            { CabinFinishEnum.NotSet,       null } //Not Available
        };

        #endregion

        #region 8.DB Pricelist - UPDATED PRICES 12-06-2022

        private static readonly decimal BronzeCleanDB = 60;
        private static readonly decimal SafeKidsDB = 85;
        private static readonly decimal CustomizeFactorDB = 0.15m;

        private static readonly List<int> BaseHeightDB = new() { 1900, 2000 };
        private static readonly List<int> BaseLengthsDB = new() { 350, 550, 600, 650, 700, 750};
        private static readonly List<CabinThicknessEnum> BaseThicknessesDB = new() { CabinThicknessEnum.Thick8mm, CabinThicknessEnum.Thick10mm };

        /// <summary>
        /// Nominal Length - Price -- DB Height:190cm, 8mm Transparent , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceDB_8_190_Transp_Chrome = new()
        {
            {   0, 365 }, //Min Price
            { 350, 395 },              //OLD PRICES { 350, 335 },
            { 550, 425 },              //           { 700, 395 },
            { 600, 440 },              //           { 750, 490 },
            { 650, 455 },              //           { 800, 500 }, DEPRECATED SIZE
            { 700, 470 },              //           { 900, 520 }, DEPRECATED SIZE
            { 750, 580 },
        };

        /// <summary>
        /// Nominal Length - Price -- DB Height:190cm, 8mm Special , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceDB_8_190_Special_Chrome = new()
        {
            {   0, 430 }, //Min Price
            { 350, 475 },               //OLD PRICES { 350, 400 },
            { 550, 535 },               //           { 700, 485 },
            { 600, 545 },               //           { 750, 600 },
            { 650, 565 },               //           { 800, 605 }, DEPRECATED SIZE
            { 700, 575 },               //           { 900, 635 }, DEPRECATED SIZE
            { 750, 710 },
        };

        /// <summary>
        /// The Dictionary containing the Various Combinations of Base Prices for DB Model
        /// Must be Declared after the BaseCombinations are declared ,otherwise the nested Dictionaries Throw Null Exceptions (They have not been created yet when called)
        /// </summary>
        private static readonly Dictionary<(CabinThicknessEnum, GlassFinishEnum), Dictionary<int, decimal>> BasePricelistsDB = new()
        {
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Transparent),   BasePriceDB_8_190_Transp_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Satin),         BasePriceDB_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Fume),          BasePriceDB_8_190_Special_Chrome },

            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Transparent),   BasePriceDB_8_190_Transp_Chrome },
            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Satin),         BasePriceDB_8_190_Special_Chrome },
            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Fume),          BasePriceDB_8_190_Special_Chrome },
        };

        /// <summary>
        /// The Extra Cost of Finish DB Model
        /// </summary>
        private static readonly Dictionary<CabinFinishEnum, decimal?> FinishPricingDB = new()
        {
            { CabinFinishEnum.Polished,       0 },
            { CabinFinishEnum.Brushed,      160 },//ANODIZE?
            { CabinFinishEnum.BlackMat,     145 },//RAL
            { CabinFinishEnum.WhiteMat,     145 },//RAL
            { CabinFinishEnum.Bronze,       225 },//PLATING
            { CabinFinishEnum.BrushedGold,  225 },//PLATING
            { CabinFinishEnum.Gold,         225 },//Not Available
            { CabinFinishEnum.Copper,       225 },//Not Available
            { CabinFinishEnum.Special,      null },//Not Available
            { CabinFinishEnum.NotSet,       null } //Not Available
        };

        #endregion

        #region 9.BathtubGlasses Pricelist - UPDATED PRICES 13-06-2022

        #region 9.1 8W40 Pricelists - UPDATED PRICES - 13-06-2022
        private static readonly decimal BronzeClean8W40 = 60;
        private static readonly decimal SafeKids8W40 = 85;
        private static readonly decimal CustomizeFactor8W40 = 0.15m;

        private static readonly List<int> BaseHeight8W40 = new() { 1400 };
        private static readonly List<int> BaseLengths8W40 = new() { 700, 800, 900 };
        private static readonly List<CabinThicknessEnum> BaseThicknesses8W40 = new() { CabinThicknessEnum.Thick8mm };

        /// <summary>
        /// Nominal Length - Price -- 8W40 Height:140cm, 8mm Transparent , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePrice8W40_8_140_Transp_Chrome = new()
        {
            {   0, 250 },//Min Price
            { 700, 265 },           //OLD PRICES { 700, 225 },
            { 800, 270 },           //           { 800, 230 },
            { 900, 280 }            //           { 900, 235 }
        };

        /// <summary>
        /// Nominal Length - Price -- 8W40 Height:140cm, 8mm Special , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePrice8W40_8_140_Special_Chrome = new()
        {
            {   0, 360 },//Min Price
            { 700, 390 },               //OLD PRICES { 700, 295 },
            { 800, 395 },               //           { 800, 300 },
            { 900, 415 }                //           { 900, 315 }
        };

        /// <summary>
        /// The Dictionary containing the Various Combinations of Base Prices for 8W40 Model
        /// Must be Declared after the BaseCombinations are declared ,otherwise the nested Dictionaries Throw Null Exceptions (They have not been created yet when called)
        /// </summary>
        private static readonly Dictionary<(CabinThicknessEnum, GlassFinishEnum), Dictionary<int, decimal>> BasePricelists8W40 = new()
        {
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Transparent)    , BasePrice8W40_8_140_Transp_Chrome  },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Serigraphy)     , BasePrice8W40_8_140_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Special)        , BasePrice8W40_8_140_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Satin)          , BasePrice8W40_8_140_Special_Chrome },
            { (CabinThicknessEnum.Thick8mm, GlassFinishEnum.Fume)           , BasePrice8W40_8_140_Special_Chrome },

            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Transparent)    , BasePrice8W40_8_140_Transp_Chrome  },
            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Serigraphy)     , BasePrice8W40_8_140_Special_Chrome },
            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Special)        , BasePrice8W40_8_140_Special_Chrome },
            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Satin)          , BasePrice8W40_8_140_Special_Chrome },
            { (CabinThicknessEnum.Thick10mm, GlassFinishEnum.Fume)           , BasePrice8W40_8_140_Special_Chrome },
        };

        /// <summary>
        /// The Extra Cost of Finish 8W40 Model
        /// </summary>
        private static readonly Dictionary<CabinFinishEnum, decimal?> FinishPricing8W40 = new()
        {
            //SAME PRICING WITH 8W
            { CabinFinishEnum.Polished,       0 },
            { CabinFinishEnum.Brushed,      130 },//Anodizing
            { CabinFinishEnum.BlackMat,     105 },//RAL
            { CabinFinishEnum.WhiteMat,     105 },//RAL
            { CabinFinishEnum.Bronze,       185 },//Anodizing
            { CabinFinishEnum.BrushedGold,  185 },//Anodizing
            { CabinFinishEnum.Gold,         null },//Not Available
            { CabinFinishEnum.Copper,       null },//Not Available
            { CabinFinishEnum.Special,      null },//Not Available
            { CabinFinishEnum.NotSet,       null } //Not Available
        };
        #endregion

        #region 9.2 NV Pricelists - UPDATED PRICES 13-06-2022
        private static readonly decimal BronzeCleanNV = 60;
        private static readonly decimal SafeKidsNV = 85;
        private static readonly decimal CustomizeFactorNV = 0.25m;

        private static readonly List<int> BaseHeightNV = new() { 1400 };
        private static readonly List<int> BaseLengthsNV = new() { 800 };
        private static readonly List<CabinThicknessEnum> BaseThicknessesNV = new() { CabinThicknessEnum.Thick6mm };

        /// <summary>
        /// Nominal Length - Price -- NV Height:140cm, 6mm Transparent , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceNV_6_140_Transp_Chrome = new()
        {
            {   0, 265 },//Min Price
            { 800, 265 } //OLD PRICE { 800, 200 }
        };

        /// <summary>
        /// Nominal Length - Price -- NV Height:140cm, 6mm Special , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceNV_6_140_Special_Chrome = new()
        {
            {   0, 340 },//Min Price
            { 800, 340 }, //OLD PRICE { 800, 265 },
        };

        /// <summary>
        /// The Dictionary containing the Various Combinations of Base Prices for NV Model
        /// Must be Declared after the BaseCombinations are declared ,otherwise the nested Dictionaries Throw Null Exceptions (They have not been created yet when called)
        /// </summary>
        private static readonly Dictionary<(CabinThicknessEnum, GlassFinishEnum), Dictionary<int, decimal>> BasePricelistsNV = new()
        {
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Transparent),   BasePriceNV_6_140_Transp_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Serigraphy),    BasePriceNV_6_140_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Special),       BasePriceNV_6_140_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Satin),         BasePriceNV_6_140_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Fume),          BasePriceNV_6_140_Special_Chrome },
        };

        /// <summary>
        /// The Extra Cost of Finish NV Model
        /// </summary>
        private static readonly Dictionary<CabinFinishEnum, decimal?> FinishPricingNV = new()
        {
            //SAME PRICING WITH 8W
            { CabinFinishEnum.Polished,       0 },
            { CabinFinishEnum.Brushed,      210 },//Anodizing
            { CabinFinishEnum.BlackMat,     160 },//RAL
            { CabinFinishEnum.WhiteMat,     160 },//RAL
            { CabinFinishEnum.Bronze,       235 },//Anodizing
            { CabinFinishEnum.BrushedGold,  235 },//Anodizing
            { CabinFinishEnum.Gold,         null },//Not Available
            { CabinFinishEnum.Copper,       null },//Not Available
            { CabinFinishEnum.Special,      null },//Not Available
            { CabinFinishEnum.NotSet,       null } //Not Available
        };
        #endregion

        #region 9.3 NV2 Pricelists - UPDATED PRICES 13-06-2022
        private static readonly decimal BronzeCleanNV2 = 70;
        private static readonly decimal SafeKidsNV2 = 125;
        private static readonly decimal CustomizeFactorNV2 = 0.25m;

        private static readonly List<int> BaseHeightNV2 = new() { 1400 };
        private static readonly List<int> BaseLengthsNV2 = new() { 1200 };
        private static readonly List<CabinThicknessEnum> BaseThicknessesNV2 = new() { CabinThicknessEnum.Thick6mm };

        /// <summary>
        /// Nominal Length - Price -- NV2 Height:140cm, 6mm Transparent , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceNV2_6_140_Transp_Chrome = new()
        {
            {    0, 335 },//Min Price
            { 1200, 335 } , //OLD PRICE { 1200, 255 }
        };

        /// <summary>
        /// Nominal Length - Price -- NV2 Height:140cm, 6mm Special , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceNV2_6_140_Special_Chrome = new()
        {
            {    0, 460 },//Min Price
            { 1200, 460 }, //OLD PRICE { 1200, 350 },
        };

        /// <summary>
        /// The Dictionary containing the Various Combinations of Base Prices for NV2 Model
        /// Must be Declared after the BaseCombinations are declared ,otherwise the nested Dictionaries Throw Null Exceptions (They have not been created yet when called)
        /// </summary>
        private static readonly Dictionary<(CabinThicknessEnum, GlassFinishEnum), Dictionary<int, decimal>> BasePricelistsNV2 = new()
        {
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Transparent),   BasePriceNV2_6_140_Transp_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Serigraphy),    BasePriceNV2_6_140_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Special),       BasePriceNV2_6_140_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Satin),         BasePriceNV2_6_140_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Fume),          BasePriceNV2_6_140_Special_Chrome },
        };

        /// <summary>
        /// The Extra Cost of Finish NV2 Model
        /// </summary>
        private static readonly Dictionary<CabinFinishEnum, decimal?> FinishPricingNV2 = new()
        {
            //SAME PRICING WITH 8W
            { CabinFinishEnum.Polished,       0 },
            { CabinFinishEnum.Brushed,      250 },//Anodizing
            { CabinFinishEnum.BlackMat,     185 },//RAL
            { CabinFinishEnum.WhiteMat,     185 },//RAL
            { CabinFinishEnum.Bronze,       315 },//Anodizing
            { CabinFinishEnum.BrushedGold,  315 },//Anodizing
            { CabinFinishEnum.Gold,         null },//Not Available
            { CabinFinishEnum.Copper,       null },//Not Available
            { CabinFinishEnum.Special,      null },//Not Available
            { CabinFinishEnum.NotSet,       null } //Not Available
        };
        #endregion

        #region 9.4 MV2 Pricelists - UPDATED PRICES 13-06-2022

        private static readonly decimal BronzeCleanMV2 = 70;
        private static readonly decimal SafeKidsMV2 = 125;
        private static readonly decimal CustomizeFactorMV2 = 0.25m;

        private static readonly List<int> BaseHeightMV2 = new() { 1400 };
        private static readonly List<int> BaseLengthsMV2 = new() { 1200 };
        private static readonly List<CabinThicknessEnum> BaseThicknessesMV2 = new() { CabinThicknessEnum.Thick6mm };

        /// <summary>
        /// Nominal Length - Price -- NV2 Height:140cm, 6mm Transparent , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceMV2_6_140_Transp_Chrome = new()
        {
            {    0, 375 },//Min Price
            { 1200, 375 } , //OLD PRICE { 1200, 285 }
        };

        /// <summary>
        /// Nominal Length - Price -- NV2 Height:140cm, 6mm Special , Chrome
        /// </summary>
        private static readonly Dictionary<int, decimal> BasePriceMV2_6_140_Special_Chrome = new()
        {
            {    0, 505 },//Min Price
            { 1200, 505 }, // OLD PRICE { 1200, 385 },
        };

        /// <summary>
        /// The Dictionary containing the Various Combinations of Base Prices for NV2 Model
        /// Must be Declared after the BaseCombinations are declared ,otherwise the nested Dictionaries Throw Null Exceptions (They have not been created yet when called)
        /// </summary>
        private static readonly Dictionary<(CabinThicknessEnum, GlassFinishEnum), Dictionary<int, decimal>> BasePricelistsMV2 = new()
        {
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Transparent),   BasePriceMV2_6_140_Transp_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Serigraphy),    BasePriceMV2_6_140_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Special),       BasePriceMV2_6_140_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Satin),         BasePriceMV2_6_140_Special_Chrome },
            { (CabinThicknessEnum.Thick6mm, GlassFinishEnum.Fume),          BasePriceMV2_6_140_Special_Chrome },
        };

        /// <summary>
        /// The Extra Cost of Finish NV2 Model
        /// </summary>
        private static readonly Dictionary<CabinFinishEnum, decimal?> FinishPricingMV2 = new()
        {
            //SAME PRICING WITH 8W
            { CabinFinishEnum.Polished,       0 },
            { CabinFinishEnum.Brushed,      290 },//Anodizing
            { CabinFinishEnum.BlackMat,     235 },//RAL
            { CabinFinishEnum.WhiteMat,     235 },//RAL
            { CabinFinishEnum.Bronze,       340 },//Anodizing
            { CabinFinishEnum.BrushedGold,  340 },//Anodizing
            { CabinFinishEnum.Gold,         null },//Not Available
            { CabinFinishEnum.Copper,       null },//Not Available
            { CabinFinishEnum.Special,      null },//Not Available
            { CabinFinishEnum.NotSet,       null } //Not Available
        };
        #endregion

        #endregion

        /// <summary>
        /// Dictionary Pointing to all Base Cabin PriceLists
        /// </summary>
        public static readonly Dictionary<CabinModelEnum,
                               Dictionary<(CabinThicknessEnum, GlassFinishEnum),
                               Dictionary<int, decimal>>>
                               Pricelist = new()
                               {
                                   { CabinModelEnum.Model9S,        BasePricelists9S },
                                   { CabinModelEnum.Model94,        BasePricelists94 },
                                   { CabinModelEnum.Model9B,        BasePricelists9B },
                                   { CabinModelEnum.Model9F,        BasePricelists9F },
                                   { CabinModelEnum.Model9A,        BasePricelists9A },
                                   { CabinModelEnum.Model9C,        BasePricelists9C },
                                   { CabinModelEnum.ModelW,         BasePricelistsW },
                                   { CabinModelEnum.ModelE,         BasePricelistsE },
                                   { CabinModelEnum.ModelWFlipper,  BasePricelists8WFlipper },
                                   { CabinModelEnum.ModelVS,        BasePricelistsVS },
                                   { CabinModelEnum.ModelV4,        BasePricelistsV4 },
                                   { CabinModelEnum.ModelVA,        BasePricelistsVA },
                                   { CabinModelEnum.ModelVF,        BasePricelistsVF },
                                   { CabinModelEnum.ModelNB,        BasePricelistsNB },
                                   { CabinModelEnum.ModelNP,        BasePricelistsNP },
                                   { CabinModelEnum.ModelWS,        BasePricelistsWS },
                                   { CabinModelEnum.ModelHB,        BasePricelistsHB },
                                   { CabinModelEnum.ModelDB,        BasePricelistsDB },
                                   { CabinModelEnum.Model8W40 ,     BasePricelists8W40 },
                                   { CabinModelEnum.ModelNV,        BasePricelistsNV },
                                   { CabinModelEnum.ModelNV2,       BasePricelistsNV2 },
                                   { CabinModelEnum.ModelMV2,       BasePricelistsMV2 },
                                   { CabinModelEnum.ModelGlassContainer , new() },
                                   { CabinModelEnum.ModelQB,        BasePricelistsQB},
                                   { CabinModelEnum.ModelQP,        BasePricelistsQP},
                               };

        /// <summary>
        /// The Base Lengths Of the Catalogue
        /// </summary>
        private static readonly Dictionary<CabinModelEnum, List<int>> BaseLengths = new()
        {
            { CabinModelEnum.Model9S,       BaseLengths9S },
            { CabinModelEnum.Model94,       BaseLengths94 },
            { CabinModelEnum.Model9B,       BaseLengths9B },
            { CabinModelEnum.Model9F,       BaseLengths9F },
            { CabinModelEnum.Model9A,       BaseLengths9A },
            { CabinModelEnum.Model9C,       BaseLengths9C },
            { CabinModelEnum.ModelW,        BaseLengthsW },
            { CabinModelEnum.ModelE,        BaseLengthsE },
            { CabinModelEnum.ModelWFlipper, BaseLengthsWFlipper },
            { CabinModelEnum.ModelVS,       BaseLengthsVS },
            { CabinModelEnum.ModelV4,       BaseLengthsV4 },
            { CabinModelEnum.ModelVA,       BaseLengthsVA },
            { CabinModelEnum.ModelVF,       BaseLengthsVF },
            { CabinModelEnum.ModelNB,       BaseLengthsNB },
            { CabinModelEnum.ModelNP,       BaseLengthsNP },
            { CabinModelEnum.ModelHB,       BaseLengthsHB },
            { CabinModelEnum.ModelWS,       BaseLengthsWS },
            { CabinModelEnum.ModelDB,       BaseLengthsDB },
            { CabinModelEnum.Model8W40,     BaseLengths8W40 },
            { CabinModelEnum.ModelNV,       BaseLengthsNV },
            { CabinModelEnum.ModelNV2,      BaseLengthsNV2 },
            { CabinModelEnum.ModelMV2,      BaseLengthsMV2 },
            { CabinModelEnum.ModelGlassContainer ,  new() },
            { CabinModelEnum.ModelQB,       BaseLengthsQB },
            { CabinModelEnum.ModelQP,       BaseLengthsQP },
        };
        private static readonly Dictionary<CabinModelEnum, List<CabinThicknessEnum>> BaseThicknesses = new()
        {
            { CabinModelEnum.Model9S,       BaseThicknesses9S },
            { CabinModelEnum.Model94,       BaseThicknesses94 },
            { CabinModelEnum.Model9B,       BaseThicknesses9B },
            { CabinModelEnum.Model9F,       BaseThicknesses9F },
            { CabinModelEnum.Model9A,       BaseThicknesses9A },
            { CabinModelEnum.Model9C,       BaseThicknesses9C },
            { CabinModelEnum.ModelW,        BaseThicknessesW },
            { CabinModelEnum.ModelE,        BaseThicknessesE },
            { CabinModelEnum.ModelWFlipper, BaseThicknessesWFlipper },
            { CabinModelEnum.ModelVS,       BaseThicknessesVS },
            { CabinModelEnum.ModelV4,       BaseThicknessesV4 },
            { CabinModelEnum.ModelVA,       BaseThicknessesVA },
            { CabinModelEnum.ModelVF,       BaseThicknessesVF },
            { CabinModelEnum.ModelNB,       BaseThicknessesNB },
            { CabinModelEnum.ModelNP,       BaseThicknessesNP },
            { CabinModelEnum.ModelHB,       BaseThicknessesHB },
            { CabinModelEnum.ModelWS,       BaseThicknessesWS },
            { CabinModelEnum.ModelDB,       BaseThicknessesDB },
            { CabinModelEnum.Model8W40,     BaseThicknesses8W40 },
            { CabinModelEnum.ModelNV,       BaseThicknessesNV },
            { CabinModelEnum.ModelNV2,      BaseThicknessesNV2 },
            { CabinModelEnum.ModelMV2,      BaseThicknessesMV2 },
            { CabinModelEnum.ModelGlassContainer , new() },
            { CabinModelEnum.ModelQB,       BaseThicknessesQB },
            { CabinModelEnum.ModelQP,       BaseThicknessesQP },
        };

        /// <summary>
        /// The Base Height of the Catalogue
        /// </summary>
        private static readonly Dictionary<CabinModelEnum, List<int>> BaseHeights = new()
        {
            { CabinModelEnum.Model9S,       BaseHeight9S },
            { CabinModelEnum.Model94,       BaseHeight94 },
            { CabinModelEnum.Model9B,       BaseHeight9B },
            { CabinModelEnum.Model9F,       BaseHeight9F },
            { CabinModelEnum.Model9A,       BaseHeight9A },
            { CabinModelEnum.Model9C,       BaseHeight9C },
            { CabinModelEnum.ModelW,        BaseHeightW  },
            { CabinModelEnum.ModelE,        BaseHeightE  },
            { CabinModelEnum.ModelWFlipper, BaseHeightWFlipper },
            { CabinModelEnum.ModelVS,       BaseHeightVS },
            { CabinModelEnum.ModelV4,       BaseHeightV4 },
            { CabinModelEnum.ModelVA,       BaseHeightVA },
            { CabinModelEnum.ModelVF,       BaseHeightVF },
            { CabinModelEnum.ModelNB,       BaseHeightNB },
            { CabinModelEnum.ModelNP,       BaseHeightNP },
            { CabinModelEnum.ModelWS,       BaseHeightWS },
            { CabinModelEnum.ModelHB,       BaseHeightHB },
            { CabinModelEnum.ModelDB,       BaseHeightDB },
            { CabinModelEnum.Model8W40,     BaseHeight8W40 },
            { CabinModelEnum.ModelNV,       BaseHeightNV },
            { CabinModelEnum.ModelNV2,      BaseHeightNV2 },
            { CabinModelEnum.ModelMV2,      BaseHeightMV2 },
            { CabinModelEnum.ModelGlassContainer , new() },
            { CabinModelEnum.ModelQB,       BaseHeightQB },
            { CabinModelEnum.ModelQP,       BaseHeightQP },
        };

        /// <summary>
        /// Dictionary pointing to all The Finishes of each model
        /// </summary>
        public static readonly Dictionary<CabinModelEnum, Dictionary<CabinFinishEnum, decimal?>> FinishesPricelist = new()
        {
            { CabinModelEnum.Model9S,       FinishPricing9S },
            { CabinModelEnum.Model94,       FinishPricing94 },
            { CabinModelEnum.Model9B,       FinishPricing9B },
            { CabinModelEnum.Model9F,       FinishPricing9F },
            { CabinModelEnum.Model9A,       FinishPricing9A },
            { CabinModelEnum.Model9C,       FinishPricing9C },
            { CabinModelEnum.ModelW,        FinishPricingW  },
            { CabinModelEnum.ModelE,        FinishPricingE  },
            { CabinModelEnum.ModelWFlipper, FinishPricing8WFlipper },
            { CabinModelEnum.ModelVS,       FinishPricingVS },
            { CabinModelEnum.ModelV4,       FinishPricingV4 },
            { CabinModelEnum.ModelVA,       FinishPricingVA },
            { CabinModelEnum.ModelVF,       FinishPricingVF },
            { CabinModelEnum.ModelNB,       FinishPricingNB },
            { CabinModelEnum.ModelNP,       FinishPricingNP },
            { CabinModelEnum.ModelWS,       FinishPricingWS },
            { CabinModelEnum.ModelHB,       FinishPricingHB },
            { CabinModelEnum.ModelDB,       FinishPricingDB },
            { CabinModelEnum.Model8W40,     FinishPricing8W40 },
            { CabinModelEnum.ModelNV,       FinishPricingNV },
            { CabinModelEnum.ModelNV2,      FinishPricingNV2 },
            { CabinModelEnum.ModelMV2,      FinishPricingMV2 },
            { CabinModelEnum.ModelGlassContainer , new() },
            { CabinModelEnum.ModelQB,       FinishPricingQB },
            { CabinModelEnum.ModelQP,       FinishPricingQP },
        };

        /// <summary>
        /// Dictionary Pointing to Bronze Clean Price per Model
        /// </summary>
        public static readonly Dictionary<CabinModelEnum, decimal> BronzeCleanPricelist = new()
        {
            { CabinModelEnum.Model9S,       BronzeClean9S },
            { CabinModelEnum.Model94,       BronzeClean94 },
            { CabinModelEnum.Model9B,       BronzeClean9B },
            { CabinModelEnum.Model9F,       BronzeClean9F },
            { CabinModelEnum.Model9A,       BronzeClean9A },
            { CabinModelEnum.Model9C,       BronzeClean9C },
            { CabinModelEnum.ModelW,        BronzeCleanW },
            { CabinModelEnum.ModelE,        BronzeClean8E },
            { CabinModelEnum.ModelWFlipper, BronzeClean8WFlipper },
            { CabinModelEnum.ModelVS,       BronzeCleanVS },
            { CabinModelEnum.ModelV4,       BronzeCleanV4 },
            { CabinModelEnum.ModelVA,       BronzeCleanVA },
            { CabinModelEnum.ModelVF,       BronzeCleanVF },
            { CabinModelEnum.ModelNB,       BronzeCleanNB },
            { CabinModelEnum.ModelNP,       BronzeCleanNP },
            { CabinModelEnum.ModelWS,       BronzeCleanWS },
            { CabinModelEnum.ModelHB,       BronzeCleanHB },
            { CabinModelEnum.ModelDB,       BronzeCleanDB },
            { CabinModelEnum.Model8W40,     BronzeClean8W40 },
            { CabinModelEnum.ModelNV,       BronzeCleanNV },
            { CabinModelEnum.ModelNV2,      BronzeCleanNV2 },
            { CabinModelEnum.ModelMV2,      BronzeCleanMV2 },
            { CabinModelEnum.ModelGlassContainer , 0 },
            { CabinModelEnum.ModelQB,       BronzeCleanQB },
            { CabinModelEnum.ModelQP,       BronzeCleanQP },
        };

        /// <summary>
        /// Dictionary Pointing to SafeKids Price per Model
        /// </summary>
        public static readonly Dictionary<CabinModelEnum, decimal> SafeKidsPricelist = new()
        {
            { CabinModelEnum.Model9S,       SafeKids9S },
            { CabinModelEnum.Model94,       SafeKids94 },
            { CabinModelEnum.Model9B,       SafeKids9B },
            { CabinModelEnum.Model9F,       SafeKids9F },
            { CabinModelEnum.Model9A,       SafeKids9A },
            { CabinModelEnum.Model9C,       SafeKids9C },
            { CabinModelEnum.ModelW,        SafeKidsW },
            { CabinModelEnum.ModelE,        SafeKids8E },
            { CabinModelEnum.ModelWFlipper, SafeKids8WFlipper },
            { CabinModelEnum.ModelVS,       SafeKidsVS },
            { CabinModelEnum.ModelV4,       SafeKidsV4 },
            { CabinModelEnum.ModelVA,       SafeKidsVA },
            { CabinModelEnum.ModelVF,       SafeKidsVF },
            { CabinModelEnum.ModelNB,       SafeKidsNB },
            { CabinModelEnum.ModelNP,       SafeKidsNP },
            { CabinModelEnum.ModelWS,       SafeKidsWS },
            { CabinModelEnum.ModelHB,       SafeKidsHB },
            { CabinModelEnum.ModelDB,       SafeKidsDB },
            { CabinModelEnum.Model8W40,     SafeKids8W40 },
            { CabinModelEnum.ModelNV,       SafeKidsNV },
            { CabinModelEnum.ModelNV2,      SafeKidsNV2 },
            { CabinModelEnum.ModelMV2,      SafeKidsMV2 },
            { CabinModelEnum.ModelGlassContainer , 0 },
            { CabinModelEnum.ModelQB,       SafeKidsQB },
            { CabinModelEnum.ModelQP,       SafeKidsQP },
        };

        /// <summary>
        /// Dictionary Pointing to the % Increase Factor of a Customize Dimension According to the Chosen Model
        /// </summary>
        public static readonly Dictionary<CabinModelEnum, decimal> CustomizeFactors = new()
        {
            { CabinModelEnum.Model9S,       CustomizeFactor9S },
            { CabinModelEnum.Model94,       CustomizeFactor94 },
            { CabinModelEnum.Model9B,       CustomizeFactor9B },
            { CabinModelEnum.Model9F,       CustomizeFactor9F },
            { CabinModelEnum.Model9A,       CustomizeFactor9A },
            { CabinModelEnum.Model9C,       CustomizeFactor9C },
            { CabinModelEnum.ModelW,        CustomizeFactorW },
            { CabinModelEnum.ModelE,        CustomizeFactorE },
            { CabinModelEnum.ModelWFlipper, CustomizeFactorWFlipper },
            { CabinModelEnum.ModelVS,       CustomizeFactorVS },
            { CabinModelEnum.ModelV4,       CustomizeFactorV4 },
            { CabinModelEnum.ModelVA,       CustomizeFactorVA },
            { CabinModelEnum.ModelVF,       CustomizeFactorVF },
            { CabinModelEnum.ModelNB,       CustomizeFactorNB },
            { CabinModelEnum.ModelNP,       CustomizeFactorNP },
            { CabinModelEnum.ModelWS,       CustomizeFactorWS },
            { CabinModelEnum.ModelHB,       CustomizeFactorHB },
            { CabinModelEnum.ModelDB,       CustomizeFactorDB },
            { CabinModelEnum.Model8W40,     CustomizeFactor8W40 },
            { CabinModelEnum.ModelNV,       CustomizeFactorNV },
            { CabinModelEnum.ModelNV2,      CustomizeFactorNV2 },
            { CabinModelEnum.ModelMV2,      CustomizeFactorMV2 },
            { CabinModelEnum.ModelGlassContainer , 0 },
            { CabinModelEnum.ModelQB,       CustomizeFactorQB },
            { CabinModelEnum.ModelQP,       CustomizeFactorQP },
        };

        /// <summary>
        /// A Dictionary of Code - Catalogue Price of all the Cabin Parts
        /// </summary>
        public static readonly Dictionary<string, decimal> PartsPrices = new()
        {
            //Handles
            {"NA00-10-P",       10m },
            {"VA00-10-P",       45m },
            {"9A00-10-P145",    55m },
            {"00NA-10-PM",      65m },
            {"VS00-10-P551",   250m },
            {"46PA-10-40",      85m },

        };

        public static readonly Dictionary<CabinThicknessEnum, decimal> ThicknessPremiumPerSQM = new()
        {
            { CabinThicknessEnum.NotSet       ,  0 },
            { CabinThicknessEnum.Thick5mm     ,  0 },
            { CabinThicknessEnum.Thick6mm     ,  0 },
            { CabinThicknessEnum.Thick6mm8mm  , 40 }, //From 6mm to 6-8 (40Euro/Sqm of new Glass)
            { CabinThicknessEnum.Thick8mm     ,  0 }, //From 6mm to 8mm (40Euro/Sqm of new Glass)
            { CabinThicknessEnum.Thick8mm10mm , 40 }, //From 6mm to 8mm (40Euro/Sqm of new Glass)
            { CabinThicknessEnum.Thick10mm    , 40 }, //From 6mm to 8mm (40Euro/Sqm of new Glass)
        };

        /// <summary>
        /// Get The Base Price for the Requested Cabin ,
        /// Thicknesses not available in the Catalogue Get Base Price as their immediate lowest available thickness in the Catalogue
        /// Any premiums due to Thickness Change are added afterwards in the Special Dimension Rule
        /// </summary>
        /// <param name="cabin">The Cabin for which to get the Base Price</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">On Non Found Pricelist for the given Cabin </exception>
        /// <exception cref="NullReferenceException">On Null Reference of Cabin-Model-Thickness-GlassFinish</exception>
        public static decimal GetBasePrice(Cabin cabin)
        {
            decimal result;
            //We can Get a result only when those are not null
            if (cabin != null && cabin.Model != null && cabin.Thicknesses != null && cabin.GlassFinish != null)
            {
                CabinModelEnum model = (CabinModelEnum)cabin.Model;
                CabinThicknessEnum thickness = (CabinThicknessEnum)cabin.Thicknesses;
                GlassFinishEnum glassFinish = (GlassFinishEnum)cabin.GlassFinish;

                //THIS IS NOT CORRECT MUST BE REFACTORED! SPECIAL CASE BLACK 8W
                //Check and Return Black 8W Catalogue Price (Seperate from Finishes for Black as of 22-09-2022)
                if(model is CabinModelEnum.ModelW && cabin.MetalFinish is CabinFinishEnum.BlackMat)
                {
                    return GetModelWBlackPrice(cabin as CabinW);
                }

                //Try to Get Dictionary for the Requested Model
                //Try to Get Dictionary for the Requested Thickness/GlassFinish
                //Get the Closest Dimension to the Catalogue
                //Try to Get Dictionary for the Requested Dimension

                if (Pricelist.ContainsKey(model))
                {
                    if (Pricelist[model].ContainsKey((thickness, glassFinish)))
                    {
                        //Get Closest Number for this Pricelist
                        int nearestPricelistLength = CalculationsHelpers.MatchToNearest(cabin.NominalLength, BaseLengths[model]);
                        //Find the Price!
                        result = Pricelist[model][(thickness, glassFinish)][nearestPricelistLength];
                    }
                    else
                    {
                        //throw new ArgumentException($"Pricelist Combination of ({thickness}{glassFinish}) not Found for {model}");
                        return 0;
                    }
                }
                else
                {
                    return 0;
                    //throw new ArgumentException($"{nameof(Pricelist)} was not found for requested:{model}");
                }
            }
            else
            {
                throw new NullReferenceException($"Null Reference in, {nameof(Cabin)} Object Properties ,while Calling :{nameof(GetBasePrice)}");
            }
            return result;
        }

        /// <summary>
        /// Gets the Minimum Allowed Price for any Cabin
        /// </summary>
        /// <param name="cabin"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public static decimal GetMinPrice(Cabin cabin)
        {
            decimal result = 0;

            //We can Get a result only when those are not null
            if (cabin != null && cabin.Model != null && cabin.Thicknesses != null && cabin.GlassFinish != null)
            {
                CabinModelEnum model = (CabinModelEnum)cabin.Model;
                CabinThicknessEnum thickness = (CabinThicknessEnum)cabin.Thicknesses;
                GlassFinishEnum glassFinish = (GlassFinishEnum)cabin.GlassFinish;

                //THIS IS NOT CORRECT MUST BE REFACTORED! SPECIAL CASE BLACK 8W
                //Check and Return Black 8W Catalogue Price (Seperate from Finishes for Black as of 22-09-2022)
                if (model is CabinModelEnum.ModelW && cabin.MetalFinish is CabinFinishEnum.BlackMat)
                {
                    return GetModelWBlackMinimumPrice(cabin as CabinW);
                }

                //Try to Get Dictionary for the Requested Model
                //Try to Get Dictionary for the Requested Thickness/GlassFinish
                //Get the Closest Dimension to the Catalogue
                //Try to Get Dictionary for the Requested Dimension

                if (Pricelist.ContainsKey(model))
                {
                    if (Pricelist[model].ContainsKey((thickness, glassFinish)))
                    {
                        //Find the Min Price!
                        Pricelist[model][(thickness, glassFinish)].TryGetValue(0,out decimal minPrice);
                        result = minPrice; //returns zero if min not found
                    }
                    else
                    {
                        result = 99999;
                        //throw new ArgumentException($"Pricelist Combination of ({thickness}{glassFinish}) not Found for {model}");
                    }
                }
                else
                {
                    throw new ArgumentException($"{nameof(Pricelist)} was not found for requested:{model}");
                }
            }
            else
            {
                throw new NullReferenceException($"Null Reference in, {nameof(Cabin)} Object Properties ,while Calling :{nameof(GetBasePrice)}");
            }

            return result;
        }

        /// <summary>
        /// Gets the Catalogue Price for a W model in Black Finish
        /// </summary>
        /// <param name="cabin">The CabinW in Black Finish ONLY</param>
        /// <returns>The Catalogue Price</returns>
        /// <exception cref="ArgumentException">Null Cabin</exception>
        /// <exception cref="Exception">Null Model</exception>
        /// <exception cref="NotImplementedException">Not implemented Combination of Glass Thickness-GlassFinish</exception>
        private static decimal GetModelWBlackPrice(CabinW? cabin)
        {
            if (cabin is null || cabin.Model is null)
            {
                throw new ArgumentException("Cabin is Not W model could not retrieve Price");
            }
            if (cabin.MetalFinish is not CabinFinishEnum.BlackMat)
            {
                throw new ArgumentException("Cabin W is not Black , could not retrieve Price");
            }
            CabinModelEnum model = cabin.Model ?? throw new Exception("Model is Null could not retrieve W price for Black Finish");
            int nearestPricelistLength = CalculationsHelpers.MatchToNearest(cabin.NominalLength, BaseLengths[model]);

            if (cabin.Thicknesses is CabinThicknessEnum.Thick8mm or CabinThicknessEnum.Thick6mm or CabinThicknessEnum.Thick10mm)
            {
                return cabin.GlassFinish is GlassFinishEnum.Transparent
                    ? BasePriceW_8_200_Transp_Black[nearestPricelistLength]
                    : BasePriceW_8_200_Special_Black[nearestPricelistLength];
            }
            //else if(cabin.Thicknesses is CabinThicknessEnum.Thick10mm) DEPRECATED 17-10-2022 -- CHANGED WITH RULE AS PREMIUM FOR DIFFERENT GLASS THICKNESS
            //{
            //    return cabin.GlassFinish is GlassFinishEnum.Transparent
            //        ? BasePriceW_10_200_Transp_Black[nearestPricelistLength]
            //        : BasePriceW_10_200_Special_Black[nearestPricelistLength];
            //}
            else
            {
                throw new NotImplementedException("Prices for W Black model are not implemented for this Combination");
            }
        }
        /// <summary>
        /// Gets the Minimum Allowed Price for a W model in Black Finish
        /// </summary>
        /// <param name="cabin"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        /// <exception cref="NotImplementedException"></exception>
        private static decimal GetModelWBlackMinimumPrice(CabinW? cabin)
        {
            if (cabin is null || cabin.Model is null)
            {
                throw new ArgumentException("Cabin is Not W model could not retrieve Price");
            }
            if (cabin.MetalFinish is not CabinFinishEnum.BlackMat)
            {
                throw new ArgumentException("Cabin W is not Black , could not retrieve Price");
            }
            CabinModelEnum model = cabin.Model ?? throw new Exception("Model is Null could not retrieve W price for Black Finish");

            if (cabin.Thicknesses is CabinThicknessEnum.Thick8mm or CabinThicknessEnum.Thick6mm)
            {
                return cabin.GlassFinish is GlassFinishEnum.Transparent
                    ? (BasePriceW_8_200_Transp_Black.TryGetValue(0,out decimal transpPrice) ? transpPrice : 0)
                    : (BasePriceW_8_200_Special_Black.TryGetValue(0,out decimal specPrice) ? specPrice : 0) ;
            }
            else if (cabin.Thicknesses is CabinThicknessEnum.Thick10mm)
            {
                return cabin.GlassFinish is GlassFinishEnum.Transparent
                    ? (BasePriceW_10_200_Transp_Black.TryGetValue(0,out decimal transpPrice) ? transpPrice : 0)
                    : (BasePriceW_10_200_Special_Black.TryGetValue(0, out decimal specPrice) ? specPrice : 0);
            }
            else
            {
                throw new NotImplementedException("Prices for W Black model are not implemented for this Combination");
            }
        }

        /// <summary>
        /// Gets the Price for the Provided Code -or- Throws an Exception if the Code is not Found in the Pricelist
        /// </summary>
        /// <param name="partCode">The Code of the Part</param>
        /// <returns>The Price or Throws InvalidOperationException</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static decimal GetBasePartPrice(string partCode)
        {
            if (PartsPrices.TryGetValue(partCode,out decimal price))
            {
                return price;
            }
            else
            {
                throw new InvalidOperationException("The Provided Code does not Have a Price");
            }
        }

        /// <summary>
        /// Gets the Frame Price for the Provided Finish -or- Throws an Exception if the Finish is not Found in the Pricelist
        /// </summary>
        /// <param name="frameFinish">The Finish of the Frame</param>
        /// <returns>The Price or Throws InvalidOperationException</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static decimal GetBaseWFramePrice(CabinFinishEnum frameFinish)
        {
            if (W_Frame_Pricelist.TryGetValue(frameFinish,out decimal price))
            {
                return price;
            }
            else
            {
                throw new InvalidOperationException("The Provided FrameFinish does not Have a Price");
            }
        }

        /// <summary>
        /// Retrieves the Base Lengths Catalogue List for the Selected Model
        /// </summary>
        /// <param name="model">The Model</param>
        /// <returns>A List of Base Lengths</returns>
        /// <exception cref="NotImplementedException">When model does not Have a BaseLenghts List Implemented</exception>
        public static List<int> GetBaseLengths(CabinModelEnum model)
        {
            BaseLengths.TryGetValue(model, out List<int>? baseLengths);
            if (baseLengths != null)
            {
                return baseLengths;
            }
            else
            {
                throw new NotImplementedException("Base Lengths Are not Implemented for Selected Model");
            }
        }

        /// <summary>
        /// Retrieves the Base Catalogue Height for the Selected Model
        /// </summary>
        /// <param name="model">The Model</param>
        /// <returns>The Base Catalogue Height</returns>
        /// <exception cref="NotImplementedException">When model does not Have a BaseHeight Implemented</exception>
        public static List<int> GetBaseHeights(CabinModelEnum model)
        {
            BaseHeights.TryGetValue(model, out List<int>? baseHeights);
            if (baseHeights != null)
            {
                return baseHeights;
            }
            else
            {
                throw new NotImplementedException("Base Heights Are not Implemented for Selected Model");
            }
        }
        /// <summary>
        /// Retrieves the Base Catalogue Thicknesses for the Selected Model
        /// </summary>
        /// <param name="model">The Model</param>
        /// <returns>The Base Catalogue Thicknesses</returns>
        /// <exception cref="NotImplementedException">When model does not Have a BaseThickness Implemented</exception>
        public static List<CabinThicknessEnum> GetBaseThicknesses(CabinModelEnum model)

        {
            BaseThicknesses.TryGetValue(model, out List<CabinThicknessEnum>? baseThicknesses);
            if (baseThicknesses != null)
            {
                return baseThicknesses;
            }
            else
            {
                throw new NotImplementedException("Base Thickness is not Implemented for Selected Model");
            }
        }

        public static Dictionary<CabinModelEnum,List<int>> GetAllBaseLengths()
        {
            return new(BaseLengths);
        }
        public static Dictionary<CabinModelEnum, List<int>> GetAllBaseHeights()
        {
            return new(BaseHeights);
        }
        public static Dictionary<CabinModelEnum, List<CabinThicknessEnum>> GetAllBaseThicknesses()
        {
            return new(BaseThicknesses);
        }
    }
}
