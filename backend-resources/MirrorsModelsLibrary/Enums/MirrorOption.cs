using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsModelsLibrary.Enums
{
    public enum MirrorOption
    {
        [Description("Διακόπτης Αφής")]
        TouchSwitch = 0,
        [Description("Διακ.Αφής Αντιθ/κου")]
        TouchSwitchFog = 1,
        [Description("Dimmer")]
        DimmerSwitch = 2,
        [Description("Φωτοκύτταρο")]
        SensorSwitch = 3,
        [Description("Αντιθ/κο 16W")]
        Fog16W = 4,
        [Description("Αντιθ/κο 24W")]
        Fog24W = 5,
        [Description("Αντιθ/κο 55W")]
        Fog55W = 6,
        [Description("Μεγενθυντκός")]
        Zoom = 7,
        [Description("Μεγενθυντκός με Φως")]
        ZoomLed = 8,
        [Description("Μεγενθυντκός με Φως και Διακόπτη")]
        ZoomLedTouch = 9,
        [Description("Ρολόι")]
        Clock = 10,
        [Description("BlueTooth")]
        BlueTooth = 11,
        [Description("Οθόνη-Ραδιόφωνο")]
        DisplayRadio = 12,
        [Description("Οθόνη Μετεο 20")]
        Display20 = 13,
        [Description("Οθόνη Μετεο 19")]
        Display19 = 14,
        [Description("Πίσω Καπάκι")]
        IPLid = 15,
        [Description("Απαλές Γωνίες")]
        RoundedCorners = 16,

        [Description("Κανάλι LED backlight (Premium Mirrors)")]
        BackLightSealedChannel = 17,
        [Description("PlexiGlass Φωτιζώμενο Πάνω")]
        SingleTopLightedPlexiglass = 18,
        [Description("PlexiGlass Φωτιζώμενο Πάνω και Κάτω")]
        DoubleTopBottomLightedPlexiglass = 19,
        [Description("Κανάλι LED Μπροστά (Genesis και Isavella)")]
        FrontLightSealedChannel = 20,
        [Description("Αντιθαμβωτικό με Χρονοδιακόπτη και Διακόπτη Αφής")]
        SmartAntiFog = 21,
        [Description("Διακόπτη Αφής EcoTouch 60min για Αντιθαμβωτικό")]
        EcoTouch = 22,
        [Description("Οθόνη Νο11 Μαύρη με Dimmer")]
        DisplayRadioBlack = 23,
        [Description("Φωτιστικό Καθρέφτη")]
        Lamp = 24,
        [Description("Κανάλι Αλουμινίου , Φωτός για καθρέφτη με πλαίσιο χωρίς αμμοβολή")]
        LightAluminumChannel = 25,
    }
}
