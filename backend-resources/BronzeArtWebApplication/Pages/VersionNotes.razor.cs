using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace BronzeArtWebApplication.Pages;

public partial class VersionNotes : ComponentBase
{
    private readonly Dictionary<string, List<string>> patchNotes = [];

    protected override async Task OnInitializedAsync()
    {
        await Task.Delay(50);
        await Task.Run(() =>
        {
            patchNotes.Add(ApplicationVersion, version215);
            patchNotes.Add("Version 2.14 - 10-04-2025", version214);
            patchNotes.Add("Version 2.13 - 07-04-2025", version213);
            patchNotes.Add("Version 2.12 - 31-03-2025", version212);
            patchNotes.Add("Version 2.11b - 26-03-2025", version211);
            patchNotes.Add("Version 2.10 - 22-03-2025", version210);
            patchNotes.Add("Version 2.08c - 20-03-2025", version208);
            patchNotes.Add("Version 2.07 - 14-03-2025", version207);
            patchNotes.Add("Version 2.06 - 01-01-2024", version206);
            patchNotes.Add("Version 2.05 - 10-12-2023", version205);
            patchNotes.Add("Version 2.04 - 21-11-2023", version204);
            patchNotes.Add("Version 2.03 - 19-11-2023", version203);
            patchNotes.Add("Version 2.02 - 16-11-2023", version202);
            patchNotes.Add("Version 2.01 - 16-11-2023", version201);
            patchNotes.Add("Version 2.00 - 14-11-2023", version200);
            patchNotes.Add("Version 1.13 - 15-01-2023", version113);
            patchNotes.Add("Version 1.12 - 24-10-2022", version112);
            patchNotes.Add("Version 1.11 - 17-10-2022", version111);
            patchNotes.Add("Version 1.10 - 10-10-2022", version110);
            patchNotes.Add("Version 1.09 - 22-09-2022", version109);
            patchNotes.Add("Version 1.08 - 18-07-2022", version108);
            patchNotes.Add("Version 1.07c - 10-07-2022", version107c);
            patchNotes.Add("Version 1.07b - 04-07-2022", version107b);
            patchNotes.Add("Version 1.07a - 27-06-2022", version107a);
            patchNotes.Add("Version 1.06 - 19-06-2022", version106);
            patchNotes.Add("Version 1.05b - 13-06-2022", version105b);
            patchNotes.Add("Version 1.05 - 29-05-2022", version105);
            patchNotes.Add("Version 1.04 - 01-05-2022", version104);
            patchNotes.Add("Version 1.03 - 25-04-2022", version103);
            patchNotes.Add("Version 1.02 - 18-04-2022", version102);
            patchNotes.Add("Version 1.01 - 11-04-2022", version101);
            patchNotes.Add("Version 1.00 - 03-04-2022", version100);
            patchNotes.Add("Version 0.40 - 16-03-2022", version040);
            patchNotes.Add("Version 0.39b - 08-03-2022", version039b);
            patchNotes.Add("Version 0.39a - 01-03-2022", version039a);
            patchNotes.Add("Version 0.38 - 12-02-2022", version038);
            patchNotes.Add("Version 0.37 - 04-02-2022", version037);
            patchNotes.Add("Version 0.36 - 27-01-2022", version036);
            patchNotes.Add("Version 0.35 - 24-01-2022", version035);
            patchNotes.Add("Version 0.34c - 13-01-2022", version034c);
            patchNotes.Add("Version 0.34b - 11-01-2022", version034b);
            patchNotes.Add("Version 0.34 - 10-01-2022", version034);
            patchNotes.Add("Version 0.33 - 08-01-2022", version033);
            patchNotes.Add("Version 0.32 - 05-01-2022", version032);
            patchNotes.Add("Version 0.31 - 03-01-2022", version031);
            patchNotes.Add("Version 0.30 - 30-12-2021", version030);
            patchNotes.Add("Version 0.29 - 13-12-2021", version029);
            patchNotes.Add("Version 0.28 - 08-12-2021", version028);
            patchNotes.Add("Version 0.27 - 07-12-2021", version027);
            patchNotes.Add("Version 0.26 - 25-11-2021", version026);
            patchNotes.Add("Version 0.24-25 - 14-11-2021", version025);
            patchNotes.Add("Version 0.23 - 31-10-2021", version023);
            patchNotes.Add("Version 0.22 - 28-10-2021", version022);
            patchNotes.Add("Version 0.21 - 26-10-2021", version021);
            patchNotes.Add("Version 0.20 - 25-10-2021", version020);
            patchNotes.Add("Version 0.19 - 24-10-2021", version019);
            patchNotes.Add("Version 0.18 - 20-10-2021", version018);
            patchNotes.Add("Version 0.17 - 17-10-2021", version017);
            patchNotes.Add("Version 0.16 - 14-10-2021", version016);
            patchNotes.Add("Version 0.15 - 10-10-2021", version015);
            patchNotes.Add("Version 0.14 - 08-10-2021", version014);
            patchNotes.Add("Version 0.13 - 07-10-2021", version013);
            patchNotes.Add("Version 0.12 - 03-10-2021", version012);
        });
    }

    public const string ApplicationVersion = "Version 2.15 - 21-04-2025";


    private readonly List<string> version012 =
    [
        "1. Changed Total Length in Choose Buttons",
        "2. Added Log-In / LogOut / Authentication States",
        "3. Retail Users and B2B Users",
        "4. Log-In LogOut Buttons",
        "5. Language Button Display Change &amp; App MenuVersion Text",
        "6. User Privilages and Discount Claims for B2B Users",
        "7. ShowP on Auth Hide Otherwise",
        "8. Large Screen Made Mirror View Changed",
        "9. Added Blazored Local Storage to Preserve some States of the App",
        "10.Added Local Storage Language Key -- Preserves Selected Language for all Sessions with the Browser",
        "11.Discount of B2B Customers Visible along with Calculated Price",
        "12.Added Printing Button on Table Mirror Options (Still Not Functioning Properly - Page Needs Clearing)",
        "13.Added Font-Weight to Totals and Headers of Mirror Table ",
        "14.Added Fog Switch Option (Still Not Functioning Properly) - User Must Choose an AntiFog if The Switch is Chosen"
    ];
    private readonly List<string> version013 =
    [
        "1.Changed Fog Switch Option to Menu",
        "2.Added Pricing to Circular Mirrors",
        "3.Fixed Language Delay Display Bug to Menu on Changing the Language",
        "4.Deprecated Javascript Pictures PreLoading ,Slow App not Helping in First Render",
        "5.Added Print Div Function (Not Working Properly Yet)"
    ];
    private readonly List<string> version014 =
    [
        "1.Refactored Assemble Mirror Component",
        "2.Added Shape/Lights/Supports/Dimensions Dialog as Seperate Components",
        "3.Added Styles Static Info - Containing Global Styles/Variants/Colors for Components",
        "4.Added Sandblast/Touch/Fog/Magnif/Screen Dialog as Seperate Components"
    ];
    private readonly List<string> version015 =
    [
        "1.Refactored Pricing Table to a Seperate Component",
        "2.Refactored Dimensions Table to Multiple Components",
        "3.DimensionInput Box Component",
        "4.Discount Box Component -- Only for Authorized View",
        "5.Added Account Claim of PowerUser -- Possibility of Manipulating the Discount",
        "6.Added Various String to Globalization Languages",
        "7.Added Dimensions-Discount Above Main Table",
        "8.Reserved Area for the Drawn Mirror below Options Photos",
        "9.Added Error Message on Not Selecting Shape/Light/Dimensions (This has to be Done by The Mirror Validator)"
    ];
    private readonly List<string> version016 =
    [
        "1.Added Mirror Validation",
        "2.Added Error Messages Display in all Localizations when Certain Options not Selected",
        "3.Tied the Validator with MirrorPricing Table Show"
    ];
    private readonly List<string> version017 =
    [
        "1. Fixed Printing Offer Page to a visible Print(May not Work on Mobile Devices)",
        "2. Added Button to Print Pricing Table Leading Directly to Printing Dialog and Back to Page after Printing",
        "3. Added Webkit Printing Colors for Chrome Browsers",
        "4. Fixed Full Table Descriptions for Lightings",
        "5. Fixed Descriptions For Included Options to Mirror Price",
        "6. Position of Supports and Dimensions Single Button Interchanged",
        "7. Upon Choosing NoLighted Mirror -- Sandblast Becomes N9 or H7",
        "8. New Dialog Frame Finishes",
        "9. Added DescKeys for SupportFinishTypes",
        "10.Added Images & Photo Paths for Anodized-Electroplated Finishes",
        "11.Added DescKeys for Enum Finishes Anodized/Electroplated",
        "12.Fixed FrameFinish Dialog Implementation (State/Pop/Disabled/SingleButton)",
        "13.Added Codes to SupportModel",
        "14.Added SubCodes to FrameFinishes",
        "15.Fixed Dimensions 121-185 Pricing Bug",
        "16.Fixed Dimensions Frame Pricing Bug 90+",
        "17.Validation for Dimensions of Frame",
        "18.Validation on Sandblast Selection when We have Frame",
        "19.Fixed Various CSS wierd Overflows/Scrolls"
    ];
    private readonly List<string> version018 =
    [
        "1.BugFix - Rounded Corners No Longer Appear When there is a Visible Frame to a Rectangular Mirror",
        "2.New Button Observe Mirror Goes to Mirror Draw",
        "3.Implemented Draw for Circular and Rectangular Mirror",
        "4.Implemented Draw for General Dimensions of Rectangular and Circular Mirrors",
        "5.Implemented Draw for Simple Magnifyer & Led Magnifyer",
        "6.Implemented Draw for Touch Button",
        "7.BugFix - Sandblasts Now Appear on Selection Table Price is always included on the Mirror",
        "8.Implemented Draws for All Sandblast Variants"
    ];
    private readonly List<string> version019 =
    [
        "1. BugFix - Added Pricing for Circular Mirror Supports Except Perimetrical Frame",
        "2. BugFix - Added Shifting to Mirror Glass Draw when there is a Visible Frame",
        "3. BugFix - Changed Stroke Color of Frame to White (Connections are Visible now)",
        "4. (DEPRECATED)Changed Background Color of Selected Options Photos to white",
        "5. Added Display11 Draw with Anchored Text",
        "6. Touch Button Draw Does not Appear along with Display11",
        "7. Added Header for Front Draw",
        "8. Deprecated Svg text Radio strings",
        "9. Seperate Partial Class for MirrorGlassDraw",
        "10.Fixed Sandblast Dimensions to the Ratio of Real Ones",
        "11.Added Rounded Corners to Mirrors and Sandblasts where Applicable",
        "12.Added Meteo Draws",
        "13.Fixed Placement Bug on Mirror Extras Drawings",
        "14.Added AutoCorrect Actions on Making Invalid Selections",
        "15.Added Inform User SnackBar on AutoCorrection Actions",
        "16.Added Numerous Validation Rules",
        "17.BugFix - FogSwitch is Disabled when Mirror Without Light is Selected",
        "18.BugFix - Now Support of Mirror With Light Does not Change to Perimetrical when Previously VisibleFrame was Chosen ",
        "19.BugFix - Now Touch Switch is Not Selected when Previously Dimmer or Sensor have been Selected",
        "20.BugFix - Now ChooseExtras Button is Disabled on Circular Mirrors Always (Cannot Select Lid or Rounding for Circulars)",
        "21.BugFix - Now Magnifyers With Led are Concealed when Mirror Without Light is Selected",
        "22.BugFix - Mirror Code does not wrap anymore , Neither in Pricing Table nor in Printing",
        "23.Bug Fix - Added Draw Extra Margins to Sandblasts X6-X4-6000 When there is Visible Frame",
        "24.Bug Fix - Fixed Mirror Extras Boxes Margin when Frame and Sandblasts X6/X4/6000 are Selected"
    ];
    private readonly List<string> version020 =
    [
        "1. Changed Components naming for Draws",
        "2. Bug Fix -- Draw of Magnifyer in the Front should now appear correct with the inside Mirror Margin",
        "3. Fixed Rear Draws for Radio/Meteo/Touch/Magnifyer",
        "4. Fixed Rear Draw for Perimetrical Support in Rectangular Mirrors",
        "5. Fixed M3 Design Sandblast Margin from the Side",
        "6. Fixed Rear Draw for Double Supports in Rectangular Mirrors",
        "7. Fixed Rear Draw for Frame in Rectangular Mirrors",
        "8. Fixed Double Supports for Circular Mirrors (Appearing also on Perimetrical Currentlly)",
        "9. User Information Snackbar Messages Now Stay up for 10s (Previously 5s)",
        "10.Added Pricing to Extras (inside Dialog) except TouchSwitchFog",
        " ",
        "Not Addressed -- Anti-Fog Draws not Functioning",
        "Not Addressed -- Draws Collision When Dimensions Are Small",
        "Not Addressed -- Front Supports Draws not Functioning"
    ];
    private readonly List<string> version021 =
    [
        "1.Fixed seperator '.' and ',' parsing when used Languages where non English.Drawn Paths Should behave the same in all Cultures",
    ];
    private readonly List<string> version022 =
    [
        "1.Added PriingTableData class with Strings Format on PricingTableRows Builder",
        "2.Edit on Pricing table is Broken and Not Working",
        "3.Bug-Fix Changed Login Tokens to Session Storage--with SSO Cookie Silent LogIn (Cookie is not Created until User Changes Language ?!)",
        "4.Deprecated String Builder Messaging Choose Mirror/Light e.t.c. -- Fluent Validation has taken its place",
        "5.Refactored Frame Finish Pricing Methods",
        "6.Added Tooltip for Frames/Fog/Magnifyers/Extras/Screens/Switches CheckBox selections",
        "  Non Authenticated Users see only the Description",
        "  Authenticated Users see also the Catalogue Price for the Current Selection",
        "7.Added User Service"
    ];
    private readonly List<string> version023 =
    [
        "1. Added New Dialog -- Catalogue Mirrors",
        "   -User can Now Pick from Fixed Size Mirrors at a lower Price",
        "   -Picking a Size Passes the Data as if the Choices Where made through the Custom Dialog Story",
        "   -User can add More Items to the Fixed Size Mirror Retaining the Lower Price",
        "   -If user Assembles a Fixed Mirror by the DialogStory the Lower Price is retained again",
        "2. Fixed Italian Translation of Daylight",
        "3. Added Navigation to new Dialog",
        "4. Added Main Button Navigate to Catalogue Mirrors",
        "5. Story Dialog Becomes FogDialog When Catalogue Mirror is Selected",
        "6. Added Mirror Property 'Series'",
        "   -Catalogue Mirrors Categorized According to their Series",
        "   -Deprecated Segregation with Sandblasting for Series",
        "7. Added N7 Mirror Photo/Pricing and Ready Dimensions",
        "8. Added N6 Mirror Pricing and Ready Dimensions",
        "9. Bug Fix - Some Circular Mirrors from The Catalogue where not Recongnized",
        "10.Bug Fix - Mirrors from the Catalogue with Extras otherthan Touch , where not recognized",
        "11.Price Bug Fix - When Selecting Catalogue Mirrors with Same Lights but Different Properties",
        "12.Bug Fix - Support Finish will no Longer Apply , When Supports Change from Visible Frame to Another Type"
    ];
    private readonly List<string> version025 =
    [
        "1. Added Mirror Code Input Field",
        "2. User Can Type A Mirror Code to Select certain Combinations of Mirror Properties",
        "3. Added Submit Button Icon to Code Input Field",
        "4. Added Error Message for Invalid SKU Inputs ",
        "5. Code Input Field is Not Shown in Mobile Devices or in Very small Screens",
        "6. Fixed a Bug where Certain Catalogue Mirror Codes Where not Recognized at first Pick",
        "7. Fixed a Bug Where Simple Mirrors had a Lighted Mirror Photo as Display",
        "8. Fixed a Bug where Catalogue Mirrors where not recognized when additional Extras where Added",
        "9. Fixed a Bug where H7 Catalogue Mirrors with Rounding where not getting the Rounding",
        "10.Refactored Mirror Draws",
        "   Draw Page is Now more Elegant",
        "   Added Draws for Fog Extras",
        "   Added Support for Draws Collision",
        "   Draws Still Collide if Mirror Extras are Packed to small Space",
        "   Added Support for More Complex Draws",
        "   Added Support for Circular Mirror Frames (Not Implemented)",
        "   Added Fog Draws and Text",
        "   Added Circular Mirror Frame Draw",
        "   Added Shapes Support for new Shapes",
        "   Added Lines and ArrowHeads Support for Dimensions Presentation",
        "   Added Line Equations for Dimensions with Slope other than Vertical-Horizontal",
        "   Added Components for Live Draw Creation",
        "   Added Collision Red Alerts",
        "   Added Collision Information Text",
        "   Added Colision Validation Info",
        "   Added Support to Move Draws Around (Not Implemented)",
        "11.Fixed a Bug where Robotto Fonts were not Working with Greek Culture",
        "12.Added Secondary and Tertiary Discounts",
        "13.Added Combined Discount",
        "14.Added Decimals to Combined Discount and Step of 1%",
        "15.Fixed a Bug where Languages could not be Changed in Older Browser Versions"
    ];
    private readonly List<string> version026 =
    [
        "1. Fixed a Bug where the User could Select N7 Catalogue Mirrors and Change their Touch Button",
        "2. Fixed a Bug where Front Draws of the Mirror Extras where Incorrect when Visible Frame was Selected",
        "3. Fixed a Bug where Collision Red Boxes where incorrect on the Front Draw , when Visible Frame was Selected",
        "4. Fixed a Bug where Prices for Low Dimension Mirrors 40,50,60cm where different in Height & Length (40x60 diff-60x40)",
        "5. Updated to MudBlazor 6.0",
        "6. Updated SDKs to .Net6",
        "7. Removed Breaking Link to Greek Robotto Fonts",
        "8. Revamped Drawing and Fixed Inconsistencies in Draws and Collisions",
        "   Added Draws Arrangment Support",
        "   Added Auto Collision Corrections Algorithm to Fog with Magnifyer",
        "   Added Auto Collision Corrections Algorithm to Fog with Screens & Switches",
        "   Added Auto Collision Corrections Algorith to Screens/Switches with Magnifyer",
        "   Improved Collision Message to Appear Only when Items do not Fit According to Internal Algorithm",
        "   Added Show Extras Bounding Area",
        "9. Changed N6 Catalogue Mirrors to Triple Lighting and Dimmer (Up From Double Lighting)",
        "10.Changed Several Mirrors From Catalogue's Warm Light to Daylight",
        "11.User Can Now Change Switch to Dimmer or Sensor to a Catalogue Mirror"
    ];
    private readonly List<string> version027 =
    [
        "1. Changed Application Version Notes Component",
        "2. Refactored Assmble Mirror page (Broken into Multiple Components)",
        "3. Moved Claims Retrival From AssemblePage to UserService",
        "4. Fixed a Bug where User and Power User Discounts where reset to Defaults Whenever Page Navigation was Used",
        "5. Added UserDependency to ViewModelService",
        "6. Moved Insert Code Box to ButtonOptions Panel",
        "7. Added Retail and Wholesale Offer Modes by Selection(Selection Menu Top Right)",
        "   Retail Mode : User Can Apply Discount to His Customer as well as have Increased Catalogue Prices",
        "   Wholesale Mode : User Can Check his Buying Prices",
        "   Guest Mode (Non Authenticated) : Guests Can Only Check Descriptions and SKUs Of Mirrors",
        "   Added Retail Only Accounts for Certain Users",
        "   Retail Mode :Net Price Column shows as Final Price -Discount%",
        "   Retail Mode : Table Shows 1 Price Column when there is no Discount Applied",
        "   Retail Mode : Table Shows 2 Price Columns when there Applied Discount (The Catalogue Price Text is Crossed)",
        "8. Changed Icon for Customized Mirror Button",
        "9. Added Options Button and PageOptions Dialog",
        "   Certain Users Can Now Change and Save a few Options Concerning the Mirrors Page",
        "   Added Max Retail Discount Option",
        "11.Created Base for Cabin Models (Cabins User-Interface Coming Soon)",
        "12.Moved Catalogue Button from Button Panel to Top Center Along with Create Button",
        "13.Moved Insert Code Field to the Buttons Panel",
        "14.Draws of the Mirror are now Printed in the Mirror Offer Page",
        "15.Fixed a Bug where App State was lost when user Printed an Offer"
    ];
    private readonly List<string> version028 =
    [
        "1.Theme Support for Certain Users",
        "2.Fixed a Bug where not recognized Language strings From Previous Versions where causing an Application Crush",
        "3.Changed Language First Set to App Level",
        "4.Changed Language Button Selector to ComboBox Selector",
        "5.Replaced Language Globe Icon with Flags",
        "6.Added AssembleCabinViewModel Boilerplate"
    ];
    private readonly List<string> version029 =
    [
        "1.Added Cabins Libraries",
        "2.Added Cabin Images and Descriptions",
        "3.Added Create Shower UI Elements",
        "4.Added Creation By Opening & By Series",
        "5.Added and Categorized Photos & Descriptions of :",
        "  OpeningsGeneral - FoldingOpenings - SlidingOpenings - Standard Door Openings - Door On Panel Openings",
        "  SeriesGeneral - B6000 Models - Inox304 Modes",
        "6.Added Dialog NavigationMethods Openings and Series , Creation Story",
        "7.Added Demo Navigation to Cabin Selection"
    ];
    private readonly List<string> version030 =
    [
        "1.Added Cabins Missing Choices",
        "2.Added Dialog for Dimensions Input",
        "3.Added Dialog for StepDimensions Input",
        "4.Added Draws to help in Dimensioning",
        "5.Added Navigation",
        "6.Refactored Cabin Navigation Choices",
        "  Deprecated Almost all Dialogs for the CabinSeries/Opening Stories",
        "  Added Window Logic Back/Forth/Close/Reset",
        "  Kept Dialogs for Smaller Windows",
        "  Added CabinPanel Final Window",
        "  Improved Photos where Missing",
        "  Removed Top Buttons",
        "  Introduced Starting Carousel and Choices",
        "7.Fixed Dimensions Dialog Action Buttons",
        "8.Fixed Step Dimensions Dialog Action Buttons and Missing Sketches"

    ];
    private readonly List<string> version031 =
    [
        "1.Fixed Supports Descriptions For Mirrors (Double-Perimetrical)",
        "2.Showers Working Selection Menu - Dimensions -Step - Glass - ColorMenu- Extras Selections",
        "3.Lots of Sketches - Photos - SVG Icons",
        "4.SVG Icon Animatable Components",
        "5.Pricing Table Cabins",
        "6.Boilerplate to Implement Browser Navigatable Windows",
        "7.Boilerplate to Implement Customer Personalized States",
        "8.Auto Code Generator - Cabin Sides",
        "9.Boilerplate for Auto Draw Generation Cabin Front - Side - Plan View",
        "Fixes needed :",
        "   a.Prices Implementation",
        "   b.Side Sketches for each Code",
        "   c.Full Descriptions Pricing Table",
        "   d.Cabin Properties Selection Menu and Buttons",
        "   e.Glass Properties Dialog Aesthetics",
        "   f.Cabin Draw Implementation",
        "   g.Printing Offer Page",
        "   h.Logos and Logo Animations",
        "   i.Carousel Bigger and More Photos",
        "   j.Main Index Page to Choose from Both apps",
        "   k.New Features Page",
        "   l.Help/FAQ Page"
    ];
    private readonly List<string> version032 =
    [
        "1.Added Side Sketches",
        "2.Fixed Dimensions Panel front",
        "3.NEW Cabin Demo Anchor on Top of Layout",
        "4.Improved Cabin Panel Function",
        "5.Introduced Selected Cabins Table"
    ];
    private readonly List<string> version033 =
    [
        "1. Added Side Sketches",
        "2. Modified UserService and Theme Selection",
        "3. Retail/Wholesale Function now Preserved for the Whole App Instance not only for mirrors",
        "4. Fixed Bottom Caption Text on Ready Mirrors Dialog",
        "5. Modified Screen BreakPoints for Dimensions Sketch",
        "6. Added Footer to MainLayout",
        "7. Added Domain Support",
        "8. Removed Local Storage Retail Discount Save",
        "9. Added User Options Boilerplate -- More Options to be Added",
        "10.Added Boilerplate Validation for Cabins -- Proper Validation to be Added",
        "11.Added Minimum & Maximum Discount Display in Retail Options",
        "12.Fixed IncreaseFactor Display in Options while in Wholesale Mode"
    ];
    private readonly List<string> version034 =
    [
        "1.Added Backend Testing"
    ];
    private readonly List<string> version034b =
    [
        "1.Added Vat Options",
        "2.Themes Visibility Option"
    ];
    private readonly List<string> version034c =
    [
        "1.Fixed an Issue where Price Totals were not Updating properly ,while changing selections",
    ];
    private readonly List<string> version035 =
    [
        "1. Fixed Language Flag Images for smaller Screens",
        "2. Fixed Cabins Pricing Demo",
        "3. Added CabinPricingRules",
        "4. Fixed CabinPricingTable Footer Totals and Missing Prices",
        "5. Added Demo CabinOffer Printing",
        "6. Added CabinProducts Price Calculatuion Tooltip",
        "7. Added Discount/Pricing Tab",
        "8. Combined Step Selection with Cabin Extras",
        "9. Added Icons for BronzeClean & SafeKids Selections",
        "10.Added Description Tooltips for BronzeClean & SafeKids",
        "11.Added Theme Functionality for Cabins",
        "12.Added Code Support for Cabin Technical Draws implementation",
        "13.Refactored Cabin Models and ExtraOptions Additions",
        "14.Added Delay to Load Message",
        "15.Fixed Descriptions and Codes",
        "16.Added Demo Wording",
        "Known Bugs & Issues:",
        "1.Various Cabin Selections Appear with '0' Price",
        "2.Carousel Photos need Enlarging for Bigger Screens",
        "3.Some Model Photos are not Representing the Exact Cabin Selection Draw",
        "4.Finish Thumbnail Photos are not Correct for All Finishes",
        "5.Prices for SafeKids & Bronze Clean are not Correct",
        "6.Fog Touch Switch appears on Draws when a Screen is Also Selected",
        "7.Bluetooth Mirror Addition does not Have a draw Representation",
        "8.Incomplete Mirrors Catalogue(Not all items are Included)"
    ];
    private readonly List<string> version036 =
    [
        "1. Fixed Not Vallid Pricing and Errors for Certain Cabin Selections",
        "2. Refactored Cabin Validations",
        "3. Validation ErrorCodes and Resource Dictionaries Updated",
        "4. Validation Added with Error Message on Last Panel of Cabins Selection",
        "5. Fixed Semicircular(9C) Broken Pricing Rules",
        "6. Fixed Semicircular(9C) Step Selections",
        "7. Fixed Code of Flipper Panel",
        "8. Fixed Various Dimension Limits for All Models",
        "9. Fixed Chrome & White MetalFinish Images",
        "10.Added Direction for CabinDraws and Sketches",
        "11.Corrected Some Photos to Match Sketch Default Direction",
        "12.Shrinked CabinPanel Icons",
        "13.Changed BClean-SafeKids Coloring and Shadows",
        "14.Changed Dimensions Panel Appearence and Added Draw Title",
        "15.Fixed a Bug where StepCut Was Not Removed if both Length/Height where zero"

    ];
    private readonly List<string> version037 =
    [
        "1. Added Title Description for Selected Model",
        "2. StepCuts will be removed when either Step-Length or Step-Height have a zero or null Value",
        "3. Corrected Title Code Wording for Flipper Panel to FL",
        "4. Added Code Base for Messaging Functioning",
        "5. Removed Special Dimension 25% Rule from All Except B6000 and Inox304",
        "6. Added New Price Rule when SQM Exceed the Catalogues SQM",
        "7. Added Length Constraint and Message for 6W Glasses with Length > 1200mm",
        "8. Fixed Various Height and Length Constraints for All Models",
        "9. Added Extra Hinge Price Rule -- Though Currently Never runs as Constraints do not allow Selections where this rule Runs",
        "10.Changed Cabin Models so that they know from which draw they come from -- Keeping information of their new Tollerances and Default Options",
        "11.Fixed Cabin Direction Issues and Added Direction Description on Tables",
        "12.Changed Cabin Panel Left Photos Layout",
        "13.Added BaseLengths Button near Dimensions",
        "14.Added Menu PopUp for Base Lengths",
        "15.Added Flipping for Sketches in Step Cuts according to User Selection",
        "16.Added List of All Applied Rules as Tooltip for PowerUsers",
        "17.Added Flipping to Sketch on Pricing Table Also"
    ];
    private readonly List<string> version038 =
    [
        "1. Revamped Cabin Quote Printed Page",
        "2. Deprecated Old Draw Enums",
        "3. Added Information Table to Printed Quote and Cabin Panel",
        "4. Fixed Various Icons (Sizes - Images)",
        "5. Added Reversible Property to Cabins",
        "6. Added Small Icons with Tooltips Summing Up Selections",
        "7. Added Print Overlay to HTML Page before Printing - User only sees Overlay",
        "8. Added 9B Print",
        "9. Fixed a Bug where 6-8mm Glasses did not pass correctly thickness to Additional Sides",
        "10.Fixed a Bug where ",
        "11.Added Collision Detection and Position Recalculatiion for the FOG Switch Draw in Mirrors",
        "   Fog Switch Draw will no longer appear while having a Screen or Touch Active",
        "12.Added SafeKids Correct Pricing by Number of Glasses per Cabin Synthesis",
        "13.Fixed Opening Display for All Models Except 9C",
        "14.Fixed Glass Calculations Updates on Changed Selections"
    ];
    private readonly List<string> version039a =
    [
        "1. Small Discount Boxes will be Hidden on very small Screens",
        "2. Migrated Drawing Library",
        "3. Deprecated Test jsPDF",
        "4. Fixed some Dictational errors in Greek Language Pack",
        "5. Shifted All Cabin Draws to a single Component",
        "6. Fixed a bug Where Default Values of Additional Parts of A Draw did not Have the Same Height",
        "7. Fixed a Bug Where Flipper Thickness was Possible to be other than 6mm",
        "8. Added A9 Circular Mirrors (Without Light - With Black Frame)",
        " - Modified Validation Messages for Circular Mirrors With Frame",
        " - Modified Presentation Photo",
        " - Modified Frame Pricing",
        " - Code Input now Accepts A9 Mirrors",
        "9. Added Drawings for Cabin",
        " - User Can Select to View Draw or Photo",
        " - Draw Changes Live upon Changing Dimensions",
        " - Added Draw Not Available Message",
        " - Added Draw for 9S Cabin",
        " - Added Door Open Animation",
        " - Added Draw for 94 Cabin with Door Animation",
        " - Added Draw for 9A Cabin with Door Animation",
        " - Added Draw for 9B Cabin",
        " - Added Draw for 9F Cabin",
        " - Added Draw for DB Cabin",
        " - Added Draws for E Cabin",
        " - Added Draws for W Cabin",
        " - Added Draw for WFlipper",
        " - Added Draw for VS Cabin with Door Animation",
        "10.Added Buttons to Show Cabin Draw Sides",
        "11.Added Draw Not Available wherever there is no Draw",
        "12.Added Glass Building for WFlipper",
        "13.Added Fixed a Bug Where VS Cabin was getting a Magnet while Combined with a Fixed Panel VF",
        "14.Fixed Animation Bug for Cabin9A Right Draw"

    ];
    private readonly List<string> version039b =
    [
        "1. Added Draw for V4 Cabin",
        "2. Added Inox304 Stopper Bumpers",
        "3. Added Coloring to Glass Finishes",
        "4. Added Coloring to Polycarbonics",
        "5. Fixed Copper Finish to a closer tone",
        "6. Shrinked Sketch-Photo Buttons - Moved them on top corner",
        "7. Added Draws for VF and VA",
        "8. Fixed Animations for VA Doors",
        "9. Added HB Draws and Fixed Support Bar Heights in Draws",
        "10.Added Step Draws for B6000 Models and Free Models , VS,V4 Models",
        "11.Added Magnet Profile for DB Draw",
        "12.Added Draws for NB && NP Models",
        "13.Added Draws for WS and Door Animation",
        "14.Fixed Opening for WS Doors"
    ];
    private readonly List<string> version040 =
    [
        "1. Fixed DB Photo Small Inconsistencies",
        "2. Remade Cabin Quotes for All Models",
        "3. Fixed a Bug Where Magnet Closure was not Default for All DB,NP,NB Draws",
        "4. Fixed Preloading photos for Pirnting on CabinPanelWindow",
        "5. Fixed A bug In Focusing while placing dimensions for a Cabin-Model",
        "6. Small Aesthetic Changes on Cabin Panel",
        "7. Cabin Assembly main Page Change",
        "8. Changed Login-Logout Button Aesthetics",
        "9. Changed Language Selector Background",
        "10.Changed Buttons of Cabins",
        "11.Small Changes in Aesthetics Mirrors Page",
        "12.Properly Outlined Cabin Draws"
    ];
    private readonly List<string> version100 =
    [
        "1. Added Main Selection Page",
        "2. Added Quotes Missing Descriptions for English and Italian",
        "3. Added Home Button on Main ToolBar along with Button Tooltips",
        "4. Added Retail-Mode function to Cabins",
        "5. Added Custom Navigation Icons for Cabins-Mirrors",
        "6. Added Photo and Draw Button to Smaller Screens - Title Shifting to bottom",
        "7. Added NEW Capsule Mirror",
        "8. Added Semicircle Shapes",
        "9. Added Lighted Rectangular Mirrors with Frame Without Sandblast",
        "10.Added New Functions Tooltip",
        "11.Removed Perimetrical Support from Simple Circular-Capsule Mirrors",
        "12.Removed SVG dimension Draw Helper on XS Screens",
        "13.Removed About Page",
        "14.Changed Cabin Selection Icons in XS Screens",
        "15.Changed Pricing Calculations for Mirrors , now segregated in divisions",
        "16.Changed Footer Background",
        "17.Changed HomePage Coloring",
        "18.Changed Footer with Logos",
        "19.Changed Glass Icon for GlassProperties",
        "20.Changed App Bar Navigation Icons visibility to xsmall screens",
        "21.Changed Main Cabin Page to different Screen Widths",
        "22.Changed Description Handling and Pricing Rules Library for Cabins",
        "23.Changed Model and Step Dialog to Fit in XS screens",
        "24.Changed Visibility of Step ToolTip in XS Screens",
        "25.Fixed a bug where backgrounds where not consistent in smaller screens",
        "26.Fixed a Bug Where Discount Decimals where truncated during calculations",
        "27.Fixed erroneous H8 Mirror Sandblast Distance from Edge",
        "28.Fixed a Bug where Front Supports where not Created in the Pricing Table",
        "29.Fixed a Bug in Quotes, where a small Part of the Header was getting Printed on the Previous Page",
        "30.Fixed various Descriptions in Greek (dictation)",
        "31.Fixed a Bug where Mirror Code was Not Set Properly when Selecting a Catalogue Mirror",
        "32.Fixed a Bug where Circle inside Circle containment Math was not Correct",
        "33.Fixed Circular and Capsule Support Photos",
        "34.Fixed Mirror Dialogs to fit Properly on Smaller Screens",
        "35.Fixed Mirrors Carousel for Smaller Screens",
        "36.Fixed a Bug where , Reseting a Cabins Story when Semicircular Models where selected , was causing a crash"
    ];
    private readonly List<string> version101 =
    [
        "1. Added Ellipse Shape and Shape Data",
        "2. Added Another Reset Button in Mirrors on top",
        "3. Added Ellipse Photos,Descriptions,Series,Code Generation,Draws,Containment",
        "4. Added Containment Correction Mini-Algorithm right after Draw Initilization in Mirrors",
        "5. Added Link Creation for Mirrors",
        "  -Users Can Now Copy a Link of the Created Mirror and Share it with someone Else Without Printing an Offer",
        "  -Pasting the Link will create the Selected Mirror again",
        "  -Users that are Logged in Can see their Price for the Linked Mirror",
        "6. Added Link Creation for Cabins",
        "  -Users Can Now Copy a Link of the Created Cabin and Share it with someone Else Without Printing an Offer",
        "  -Pasting the Link will create the Selected Structure again",
        "  -Users can Further Change modify what they want and make another link e.t.c.",
        "7. Changed Reset Icon Visual in Mirrors",
        "8. Changed Printing Button and Link Button Visuals (Both Mirrors and Cabins)",
        "9. Changed Mirror Shapes Dialog for Larger Screens",
        "10.Updated MudBlazor Libraries",
        "11.Updated Localization Libraries",
        "12.Fixed a Bug Where V4 Models Step ,Was Correctly placed at the FirstFixedPanel ,but Its Length was erroneously placed to the Second one"
    ];
    private readonly List<string> version102 =
    [
        "1.Fixed a Bug where Mirror Modals were behaving strangely (not Closing) after Printing a Quote",
        "2.Fixed a Bug where the Mirrors Draw was Overlapping the Header in a Printed Quote",
        "3.Fixed a Bug where Whitelabel Logos where not printing on Cabin Quotes",
        "4.Added Out Of Bounds Shapes Validation to the Main Mirror Page",
        "5.Added Height Constraint Tooltip for 9C Structures"
    ];
    private readonly List<string> version103 =
    [
        "1.Moved Step Button to Dimensions Panel",
        "2.Fixed a Bug Where Pasting a Link of a Cabin with Extras was Failing at glass Calculations",
        "3.Changed Glass Treatments Selections",
        "4.Added Backgrounded Ttiles to each user Input Panel",
        "5.Added Step Draw to HB",
        "6.Added a Middle Line to NB and NP Aluminium Draws",
        "7.Added Glass Validator  Error Codes and HB1 Checks",
        "8.Added Cabin Draws to Quotes - Hidden - Still Clunky",
        "9.Added Bathtub Panels to Cabin Selection",
        "-Added Photos",
        "-Added Sketches",
        "-Added StoryWindows",
        "-Fixed DrawNumbers and Mappings",
        "-Fixed Measure Draw",
        "-Added Pricing",
        "-Added Corner Radius to Bathtub Draws",
        "-Added Print Quote Essentials"
    ];
    private readonly List<string> version104 =
    [
        "1.Added Various Application Tests",
        "2.Added Page Titles to main Pages",
        "3.Added Canonical for main Pages",
        "4.Added meta description for main Pages in all Languages",
        "5.Added meta Alternate Language for main Pages",
        "6.Fixed a Bug where NB Aluminiums where not drawn correctly when Flipped"
    ];
    private readonly List<string> version105 =
    [
        "1.Added Accessories Library",
        "2.Added Accessories Selection Page",
        "-Added Head Content and Language Strings for meta tags",
        "-Added Accessories Pull from BronzeDBContainers",
        "3.Fixed a Bug where Some blocking agents where preventing Files flagging them invalidly",
        "Application should now work in some of those Situations without getting flagged",
        "4.Added API Service",
        "5.Added Boilerplate Code for Retrieving Accessories"
    ];
    private readonly List<string> version105b =
    [
        "1. Added Accessories Selection Page",
        "2. Added Series Selection Window",
        "3. Added Accessory Card",
        "4. Added Accessory Details Dialog",
        "5. Added New Pricing to Cabins",
        "6. Updated Bronze Clean Price Rule (Applies to Single Side only)",
        "7. Updated Customized Dimensioning Price Rules as per New Catalogue",
        "8. Added 1900 and 2000 as Base Heights to V , HB , W Models",
        "9. Added Length Constraint for 6mm Fixed Panels (up to 1000mm)",
        "10.Fixed a Bug where Rule Application Descriptions where not displaying correctly when rule applied more than once",
        "11.Fixed a Bug where Customized Dimension Rule was applying the Factor to All Extras and Finishes instead of only to the Base Price",
        "12.Fixed Mirrors Greek Wording on Shape Dialog. All Mirror Shapes now have a similar wording Pattern in Greek",
        "13.Fixed Edition of Secondary Types of Accessories, Added small Dialog",
        "14.Fixed a Bug Where Linking a 9C Cabin was throwing an Exception when SafeKids or BronzeClean was selected"
    ];
    private readonly List<string> version106 =
    [
        "1. Fixed a Bug where Photos Were not accepted for Accessories",
        "2. Fixed a few long descriptions in Italian",
        "3. Fixed Non Saving Accessory Finish Value on Edit",
        "4. Added Property Edit Buttons inside Accessory Edit Dialog",
        "5. Added user scalable zoom , x3",
        "6. Added Cabin Search by Code on Assemble Cabin Main Page",
        "7. Added showers pricing calculations and fixed various Bugs on Calculation Info",
        "8. Added Correct MIME types for Pdfs , pngs , jpgs",
        "9. Shrinked Finish Photos in Accessory Property Edit Dialog",
        "10.Deprecated HeaderContainer , Kept Page Title , Changed to a single Canonical",
        "11.Changed DB searches to Id",
        "12.Various Changes on User Authentication"
    ];
    private readonly List<string> version107a =
    [
        "1.Added Accessories Filters",
        "-Primary Types Filter",
        "-Secondary Types Filter",
        "-Series,Finish,MountingType,Size,Shape Filters",
        "-Live Multi-Selection Filtering",
        "2.Changed Main Menu Drawer Aesthetics and Clipping",
        "3.Added All Accessories Button",
        "4.Added Query Loaders",
    ];
    private readonly List<string> version107b =
    [
        "1.Fixed a Bug where the Menu Drawer-Choice of Accessories was pointing to the Mirrors Page",
        "2.Fixed Accessories Filter Bugs",
        "3.Added Reset Filter Button",
        "4.Added Skeletons While Filter Sorts and Processes Queries",
        "5.Fixed a Bug Where snackbars where Getting Printed in the Offer Windows when Active",
        "6.Added Recents/Saved Structures for Cabins"
    ];
    private readonly List<string> version107c =
    [
        "1.Added Accessories Skeleton Card",
        "2.Added more secure data inputs in Options Managment",
        "3.Updated to MudBlazor 6.0.10 -- TrimmerBug on 6.0.11",
        "4.Added Local DB Backups",
        "5.Added news Dialog , Deprecated Tooltip",
        "6.Fixed AccessoryCard Dialog Presentation"
    ];
    private readonly List<string> version108 =
    [
        "1. Added AutoComplete Search Bar, Manage-Users Can Now Search Accessories by Typing and Autocompleted Values Appear",
        "   -Search By : Code / Finish / Type / Secondary Type / Series",
        "2. Fixed Messaging on Authentication Events",
        "3. Added Greek/Italian Messaging on Application Startup",
        "4. Added Background to HomePage",
        "5. Added Images Download",
        "6. Expanded Filters on startup",
        "7. Added Pagers for Accessories Query Result Window",
        "8. Altered Main Page Animations",
        "9. Added Sorted by Series/Item in Accessories Query",
        "10.Added MenuBar Accessories Navigation Button"
    ];
    private readonly List<string> version109 =
    [
        "1. Deprecated Old Unit Tests",
        "2. Updated Various Libraries to fresh Stable Versions",
        "3. Introduced Various Option Selections to Cabin Structures",
        "4. Refactored all Calculations and Drawings to make Room for the new Models",
        "5. Added Code to Use more than one StepCuts in Structures where this can be applied",
        "6. Added Accurately Depictions of Supports/Strips/Profiles and Parts Generally in Drawn Strctures",
        "7. Added Various Constraints Customizations to all Structures",
        "8. Added Handles Selections",
        "9. Added Wall Fixing Selections",
        "10.Added Closing Selections",
        "11.Added Hinge Type Selections",
        "12.Added All New Selections Draws",
        "13.Added Accurate Calculations of Openings regardless of selected Options",
        "14.Added Dynamic Settings",
        "15.Added Validations for all new Selections",
        "16.Added Pricing for all new Models",
        "17.Added new Pricing for older Models",
        "18.Added Photos and refactored Quote Sheets to reflect those changes",
        "19.Added Support to introduce story mode for the whole Cabin Structure",
        "20.Added Mandatory Options , Structures must Have some options selected to Proceed",
        "21.Added All New Options To Quote Sheets",
        "22.Added New Special Dimension Rules and Calculations",
        "23.Added Special Dimensions Calculations Descriptors",
        "24.Added Live Tollerances According to Selected Parts",
        "25.Added Parts Pricing and Rules",
        "26.Added Special Dimension Wording on Cabins Pricing Table",
        "27.Added Pricing Rule for W All Around Frame",
        "28.Changed CabinPanel Display and Extra Options Display",
        "29.Changed Discount Box to Include Horizontal Box Alignment",
        "30.Changed the way Synthesis Structures are generated from Links to Include all Options of Parts Selected",
        "31.Fixed 9C Draw Element to not show by Default",
        "32.Fixed a Bug where Base Height was not matching to Nearest but only to First Found",
        "33.Fixed a bug where all models when changing handle had Customized Dimensioning Applied when only B6000 Should",
        "34.Fixed all Lengths to be based on Nominal Length rather than MinimumLength. Parts Changes no longer trigger a length change",
        "35.Fixed a Bug where 9A , VA opening of Secondary Piece was erronously that of the Primary Piece",
        "36.Fixed a Bug where Openings were not Calculated Accurately for all Models Resulting in Smaller Openings than the Real ones",
        "37.Fixed a Bug where Handles where placed slightly off Postition",
        "38.Fixed scroller Bug in Step Dialog",
        "39.Fixed a Bug where NP structures where not accepting Handles",
        "40.Fixed wording on 9B QuoteSheet",
    ];
    private readonly List<string> version110 =
    [
        "1. Fixed a Bug where by CodeBox Typing 60IA - 60NA Mirrors , Sandblast was not Recognized ",
        "2. Fixed a Bug where White Finish Cabins Photo Path was Broken",
        "3. Fixed a Bug where Animations where not resolving due to comma as decimal seperator in -Axis xy - values",
        "4. Fixed User Accounts Cabin Structures Discounting",
        "5. Fixed a Bug Where Mirrors with Extras where not having their Link Applied Correctly",
        "6. Fixed a space Bug in Accessories Series Managment",
        "7. Added 46PA-10-40 Handle",
        "8. Added WL funcionality to Cabins",
        "9. Added info wordings for priced structures",
        "10.Increased Parts Dialog Selection Card Size",
        "11.Prevented google Translate from messing up Price updating.Users should not use Auto Translation for an appropriate experience with the App.",
        "12.Added Minimum Allowable Price per Structure",
        "13.Modified Calculations Information Box and Special Dimension Rule Wording",
        "14.Modified Accessories Primary Image",
        "15.Seperated PIFs for Cabins and Mirrors",
    ];
    private readonly List<string> version111 =
    [
        "1. Fixed a Bug where Handles in Double Door Models 94,V4 Where charged only Once",
        "2. Fixed a Bug where Default V4 VS Handles where erronously being charged when structure had a VF",
        "3. Fixed a Bug where VF Prices where lower than the Catalogue",
        "4. Fixed Pricing for 6/8mm , 8/10mm , 10mm Glass Selections",
        "5. Removed Uneccessary Loggings",
        "6. Altered Length and Height Constraints for most Models",
        "7. Altered item list position of GlassThicknesses",
        "8. Added Glass Thickness Constraints on Height/Length surpassed breakpoints",
        "9. Added Validation User Prompt on Surpassing Height/Length Breakpoint",
        "10.Added Pricing rule Premium for Irregular Glass Thickness",
        "11.Added Calculations Descriptors for Irregular Glass Rule",
        "12.Disabled Selectable Glass Thicknesses when Dimensions are not Appropriate",
        "13.Removed Accessories Pricing from WhiteLabeled Deployments",
        "14.Merged ComplexRules Descriptors into a single Popup ToolTip"
    ];
    private readonly List<string> version112 =
    [
        "1.Added Excel Support Lib",
        "2.Added Scroll to Top while selecting different accessories filters ",
        "3.Added Notes field in Showers and Mirrors. Notes are Printed along with the rest information",
        "4.Fixed WS Pricing as per Catalogue (Prices shown where much higher)",
        "5.Fixed a Bug where WS was calculated as a special dimension every time"
    ];
    private readonly List<string> version113 =
    [
        "1. Added Cabin Creation Unit Tests",
        "2. Added service builders for parts and glasses",
        "3. Refactored Calculations and Validations",
        "4. Refactored Draws Handling",
        "5. Refactored Cabin Parts components and Descriptions",
        "6. Refactored Cabins Generation Factory",
        "7. Refactored Cabin Parts Modules",
        "8. Increased the Maximum Length allowed for 6mm Fixed Panels to 1100mm",
        "9. Increased Min Calculated p for 94 Models",
        "10.Increased constraints for Minimum Length of HB Structures",
        "11.Deprecated old CanHaveStepDictionaries added prop to Repository",
        "12.Deprecated Unused NP Constraints",
        "13.Deprecated unused W Constraints , integrated to all models constraints",
        "14.Fixed a Bug where changing the Wall Side Fixing of Inox304 Models was not reflected on the Printed Quote",
        "15.Fixed a Bug where 9C+9F Was throwing an Exception on Reseting Selections",
        "16.Fixed a Bug where ,Removing a Handle in models where its possible, was throwing an Exception",
        "17.Fixed a Bug where White Finish picture link was still broken (uppercase letter in path)",
        "18.Fixed a Bug where Not Set Glass Finish was Throwing on Code Generation",
        "19.Fixed Added Catalogue Pricing , white label",
        "20.Fixed a Bug where 8W Bathtub Models where throwing errors",
        "21.Fixed a Bug where 1900mm was Calculated as a CatalogueDimension in V4 rather than 2000mm",
        "22.Fixed Openings to be more Accurate for all Sliders 94,9A,9S,V4,VS,VA,WS",
        "23.Fixed a Bug where 8-10mm Glass for WS was Recognized as irregular thickness when it should be standard",
        "24.Fixed a Bug where 9C models had Handle Options when they should not",
        "25.Fixed a Bug where VA pricing calculations for special Glass finishes was incorrect",
        "26.Fixed a Bug where 8WFlipper was considered Reversible"
    ];
    private readonly List<string> version200 =
    [
        "1. Refactored Cabin Parts and Part Sets",
        "2. Refactored Accessories Db and Modeling",
        "3. Deprecated Old Acccessories Db",
        "4. Updated Libraries to newer versions",
        "5. Fixed Scroll Bar appearing besides app bar",
        "6. Fixed Scroll Bar appearing on Index Page on certain viewports",
        "7. Fixed Footer Seperation from main Content",
        "8. Added New Accessories Menu",
        "9. Added Series-Type-Finish Navigation",
        "10.Added Item Basic Cards",
        "11.Added Search Box By Indexed Terms",
        "12.Added Navigate to Search Item",
        "13.Added Grid,Compact,Table,Grouped Views in Accessories",
        "14.Added Accessories Pricing",
        "15.Added Finishes per Item , Hover - Select",
        "16.Added Detailed Accessory Card",
        "17.Added Printable Accessories Sheets",
        "18.Added B2B Pricing Foundation",
        "19.Added Accessories Photos Downloads",
        "20.Added Printable Templates",
        "21.Added xls Generation Foundations",
        "22.Added pages animations",
        "23.Added Routing to accessories",
        "24.Added Routing Pages per Trait and Code",
        "25.Added Accessories CMS",
        "26.Changed Theme Color",
        "27.Added Various CSS tones and Colors , using a more centralized approach",
        "28.Refactored cluttered code to components",
        "29.Updated Language Descriptors",
        "30.Extended Authenticated Users Claims",
        "31.Added Accessories Dimensioning",
        "32.Added Dimension Photo per Accessory Color",
        "33.Managed Missing Accessory Color Photos with tagged Finish photo",
        "34.Changed Backend to minimal APIs",
        "35.Changed Database Entities",
        "36.Added Building of Indices and Memory Repositories",
        "37.Added Pricing Rules for Accessories",
        "38.Added Quote Basket for Accessories",
        "39.Added Printable Quotes and Orders",
        "40.Added option of Customized pricing",
        "41.Added Auto Scroll to Search Term",
        "42.Added Quote Basket App Modes",
        "43.Added Quote Basket Retrieval",
        "44.Added Message Service",
        "45.Added Pricing Rules to Quote Basket",
        "46.Added Extensions to Users",
        "47.Added Extensions to Rules",
        "48.Added Side Menu to Quote Basket (Notes , Search By Code e.t.c)",
        "49.Added Toolbar to Quote Basket with Various options per mode",
        "50.Added Print Templates per App Mode",
        "51.Added Option to Include Sheets in Quote",
        "52.Added various Dialogs",
        "53.Added service to integrate render freagments to dialogs",
        "54.Added Cache system base through storage for future improvmenets in retrieval speed",
        "55.Added integrations with other parts of the cms",

    ];
    private readonly List<string> version201 =
    [
        "1.Fixed Pricing missing issue",
        "2.Fixed Some missing translations reverted from older versions",
    ];
    private readonly List<string> version202 =
    [
        "1.Fixed Irrelevant Discount Boxes appearing in Wholesale",
        "2.Fixed Tooltip In Printing",
        "3.Fixed Tooltip in More Options Menu"
    ];
    private readonly List<string> version203 =
    [
        "1. Added CheckMarks and X-Checkmarks on row Edit Close",
        "2. Fixed decimal Seperator constraints in Discount and Price Edit Boxes",
        "3. Scrapped Debounce interval for edit Boxes , updates happen upon losing focus or pressing enter",
        "4. Fixed Add by Code box keeping focus after additions",
        "5. Added Add Accessory by pressing enter on Add by Code box",
        "6. Removed add Button completely from Code Box",
        "7. Added valid searches in Greek or Latin Carachters to Code Box",
        "8. Added Additional Regexm",
        "9. Added Options Descriptors to wholesale tooltips",
        "10.Added CheckMark and X-Checkmark on Row edits finished",
        "11.Fixed Inox304 Data Errors",
        "12.Reduced Size of Accessories Sheets (Reduced Photo Quality in Printing)",
        "13.Improved Detailed Card on Smaller Screens",
        "14.Added Description and Extended Description to Accessories that have one",
        "15.Added Side Menu on Basket Empty Page",
        "16.Added Table Copy with or without Photos in all Functions (Accessories)",
        "17.Added Table Copy with Photos in all Functions (Cabins)",
        "18.Fixed Authentication missing issue when refreshing the page of accessories"
    ];
    private readonly List<string> version204 =
    [
        "1.Constrained Quote Name to Fit in A4 Page",
        "2.Fixed Size issues with the Retrieve basket Table in Small Screens",
    ];
    private readonly List<string> version205 =
    [
        "1. Added Generic Excel Reports",
        "2. Added Table Quote/Request Reports for Guest-Retail-Wholesale",
        "3. Added Reports Styling Options",
        "4. Changed Position of AcceptEdit,Cancel Edit",
        "5. Closing Side QuoteOptions when line under Edit",
        "6. Added Local Busy Indicator",
        "7. Added All Accessories Report",
        "8. Added Formula Option in Report Generation",
        "9. Added All Acc.Report Per B2B User",
        "10.Fixed Various Typos and Translations",
        "11.Added Long running Task Cancellation option",
        "12.Searching for an Accessory now Navigates to the Detailed Card instead of the Series"
    ];
    private readonly List<string> version206 =
    [
        "1. Added QP and QB Selections as Profile Hinge in NB-NP Structures",
        "2. Removed the 'new' Badge from the accessories App bar icon",
        "3. Added Pricing , SKU , Validations for QP - QB Models",
        "4. Removed Polished Gold from Inox304 models",
        "5. Fixed Hinge Profile Selection Dialogs and Cabin Code Changes",
        "6. Corrected Draw Height Inconsistency in W Models when Combined with Q Models",
        "7. Fixed Second Wall Profile Draw in V4 Not appearing upon selecting Profiles instead of Supports",
        "8. Fixed Inconsistent Hinge Profile Header on NB-QB-NP-QP Print Template",
        "9. Added Copper Finish to B6000 Models ",
        "10.Fixed a Bug where Fog-Switch was not Removed upon Removing Anti-Fog Option",
        "11.Fixed a Bug where The Dimensions Controller allowed input of height greater than the valid ones"
    ];
    private readonly List<string> version207 =
    [
        "1.Updated Target Framework to .NET8",
        "2.Updated MudBlazor to 8.03",
        "3.Updated Multiple Libraries to match newer framework",
        "4.Updated Handles Dialog to not contain scrollers (big screens vert/horiz , small screens horiz)",
        "5.Updated Default Border Radius for all elements",
        "6.Updated Reg.Mach Dialog to not include Scrollers",
        "7.Fixed Border - Margins - Padding on news Dialog",
        "8.Fixed a Bug where Photos of Accessories where not appearing on the Sheet printing Page",
        "9.Added new mirrors foundation code (new mirrors are not visible yet)",
    ];
    private readonly List<string> version208 =
    [
        "1. Added New Standard Mirror Sizes and fixed Coding to all previous Mirrors",
        "2. Added new Prices for Framed Black Mirrors",
        "3. Added new Prices for Framed Non-Black Mirrors",
        "4. Added Extras per Mirror in Catalogue Mirrors",
        "5. Added Surcharge to Ral Color Mirrors",
        "6. Added Circular Mirrors with Frame in specific Dimensions",
        "7. Added Specific Images for Mirrors with Frame",
        "8. Added new Validation Messages for Circular Mirrors with Frame",
        "9. Added Possibility of Opening Finish Selection Dialog for Circular Mirrors",
        "10.Added Extra for the Mirrors Channel Placement when there Is Frame without Sandblast on an H7 Mirror",
        "11.Added Three Digit Dimension Codes to Customized Cabins",
        "12.Fixed a Bug that was calculating wrongfully mirrors as Customized",
        "13.Fixed a Bug that Additional Extras Where not Removed from the Previous Mirror upon selecting a New Standard Mirror",
        "14.Fixed a Duplication mirror Bug , where certain mirrors of different dimensions were leading to a single mirror",
        "15.Fixed 99999€ Bug in Circular Mirrors with Frame",
        "16.Fixed Dialogs Navigation for Circular Framed Mirrors",
        "17.Updated Price Calculation for Mirrors Frame Support",
        "18.Updated Tables for all Mirrors",
        "19.Updated Message for Circular Mirrors with Frame where Dimensions are out of bounds",
        "20.Updated Price Calculation for Circular , Capsule and Ellipse Custom Mirrors",
        "21.Changed the Standard Mirrors Dialog to two Columns",
        "22.Improved algorith for calculating Customized Mirrors",
        "23.Deprecated Mirror Electroplated Finishes",
        "24.Improved Catalogue Mirrors Table appearence",
        "25.Improved Mirrors Landing Page to match those of the Cabins",
        "26.Centered index page on application startup",
        "27.Updated to .NET9 with b",
        "28.Downgrade to MudBlazor 8.3 with c"
    ];
    private readonly List<string> version210 = ["1.Critical Error Fix in Browsers returning 2 letter Language instead of Expected 4 ('el' => 'el-GR')"];
    
    private readonly List<string> version211 = 
    [
        "1. Improved Index.html Page UI",
        "2. Removed Annoying extra loading screen on initilization",
        "3. Improved Main Layout Drawer Background and Bottom Logo Placement",
        "4. Improved Fade-In animation to use Y translate instead of width",
        "5. Improved Animation on Index.razor",
        "6. Added Blur effect to background of Index.razor",
        "7. Added Icons animation on Create Cabin or Mirror",
        "8. Added Mirrors Catalog View with Improved Styles",
        "9. Added Filters to Mirrors Catalog",
        "10.Transformed the Catalog into a Page from a Dialog",
        "11.Added Catalog Page Route and functionality",
        "12.Added subtle gradient on Loading Screen",
        "13.Added More Mirror Dimensions to Catalog Mirrors",
        "14.Added Sticky Headers to Mirrors Catalog",
        "15.Added Capsule and Ellipse Catalog Mirrors",
        "16.Fixed Helper Text on Dimensions Dialog ,which was displaying older ranges for all mirrors",
        "17.Fixed Rectangular Frame Constraint to 180cm",
        "18.Fixed Mirror Size Constraint to 220cm",
    ];
    private readonly List<string> version212 = 
    [
        "1. Improved Navigation Service for Mirrors Dialogs",
        "2. Improved Bluetooth Icon Fonts",
        "3. Improved Lights Dialog to be more appealing and Compact (New lights must be expanded)",
        "4. Improved Mirror Catalog Page",
        "5. Added More Photos to Mirrors Carousel of newer Designs",
        "6. Added Mirror Series NS-N1-N2-R9-R7-P8-P9-ND-ES-EL",
        "7. Added Additional Light Selections with Dotless LEDs and 16Watt strips",
        "8. Added Series for all Customizable Mirrors and New Mirrors",
        "9. Added Shapes for all new Mirrors",
        "10.Added Validation for Premium Lines and Selectable Lights",
        "11.Added Mirror Options Items for new Design Mirrors (Photos-Descriptions-Pricing-Functions)",
        "12.Added Dimensions Dialog Title Icon",
        "13.Added Fixed Dimensions Button above Pricing Table for Series that are not customizable",
        "14.Added all new Mirrors to Catalogue Standard Sizes",
        "15.Added Customized Ordering on Mirror Catalog Series",
        "16.Added Foundations for including lamps into Mirrors (not live yet)",
        "17.Added Display FM 11 in black Variance with Dimmer Support",
        "18.Fixed Lights Dialog , Pricing and Descriptions for New Lights",
        "19.Fixed a Bug where Price was not calculated on N7 Mirrors when Finish was altered from black",
        "20.Fixed Pricing of Frame for P8-P9 Mirrors to correctly return 0",
        "21.Fixed H7 Framed mirror Image to not display the radiator reflection",
        "22.Fixed a Bug in Initilization of Mirrors , when navigating through Dialogs reapplying touch Buttons",
        "23.Fixed Margin Inconsistencies between elements on Mirror Assembly Page",
        "24.Fixed a Bug where price Calculation in Mirrors without light was not executed for dimensions above 185cm",
        "25.Fixed 55x85 Mirror Code to Properly Reflect button in the middle of Short Dimension",
        "26.Restricted Shape Generation for new Abstract shapes and Genesis-Isavella-ES-EL lines (Print and App)",
        "27.Restricted Access to Dimensions for Series that are not customizable",
        "28.Changed Main Button for Mirrors in Index Page",
        "29.Certain Mirror Series now have options blocked from Edition (R8,R9,P8,P9,NS,ND,N1,N2,ES,EL)",
        "30.Refactored Dimensions Dialog to include Fixed Dimensions in Mirrors that cannot be customized",
    ];
    private readonly List<string> version213 = 
    [
        "1.Fixed a Bug where Mirror Eco Touch was not up for Selection",
        "2.Fixed a Bug where Touch Switch in Fog was appearing as selected when it was not",
        "3.Fixed a Bug where the accessories Filters Comboboxes where overlaying their label along with the selection",
        "4.Added Foundations for Showing items Availability , admins can now see the availability as beta",
    ];

    private readonly List<string> version214 = 
    [
        "1. Fixed a Bug where The show all Filter in Catalog of Mirrors was keeping the search term unchanged",
        "2. Fixed Codes for 0000-11-FM-BLU and 0000-11-FM-BLA",
        "3. Fixed a Bug where the extra Light Channel was not showing in the Mirrors Pricing Table 0000-AL-KAN",
        "4. Fixed a Typo-Bug where the LightWithRounding Validation message was not translated from its key",
        "5. Fixed a Bug where Catalogue equivalent Mirrors where matching Customized Mirrors in error , when light was set to Without or With",
        "6. Fixed a Bug where several translation keys for validation messages where not displaying correctly",
        "7. Fixed a Bug where double straight doors opening was showing only the opening of the first door",
        "8. Fixed a Bug where N6 Mirrors where priced higher incorrectly",
        "9. Removed 60N6-80-80DM3 Mirror from the catalogue"
    ];

    private readonly List<string> version215 = 
    [
        "1. Updated Libraries V.IdMod",
        "2. Fixed a Bug where propagation events in FogDialog where firing multiple times"
    ];


}
