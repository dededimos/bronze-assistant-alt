using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsModelsLibrary.Enums
{
    /// <summary>
    /// Each Code Represents a Series H7 = Hotel , H8 = Eco e.t.c.
    /// </summary>
    public enum MirrorSeries
    {
        /// <summary>
        /// General Custom Mirror - Anything not Categorized elsewhere
        /// </summary>
        Custom = 0,
        /// <summary>
        /// Rectangle with Light
        /// </summary>
        H7 = 1,
        /// <summary>
        /// Rectangle with Light and Perimetrical Sandblast
        /// </summary>
        H8 = 2,
        /// <summary>
        /// Rectangle with Light and Sandblast Top and Bottom
        /// </summary>
        X6 = 3,
        /// <summary>
        /// Rectangle with Light and Sandblast Left and Right
        /// </summary>
        X4 = 4,
        /// <summary>
        /// Rectangle with Light and Sandblast Left and Right Shifted inside
        /// </summary>
        _6000 = 5,
        /// <summary>
        /// Rectangle with Light and Sandblast Single Horizontal Line on Top
        /// </summary>
        M3 = 6,
        /// <summary>
        /// Circular With Light
        /// </summary>
        N9 = 7,
        /// <summary>
        /// Circular With Light and Perimetrical Sandblast
        /// </summary>
        N6 = 8,
        /// <summary>
        /// Circular With Light and Black Frame (Metal) with Perimetrical Sandblast
        /// </summary>
        N7 = 9,
        /// <summary>
        /// Circular With Light and Black Frame (Aluminium) with Perimetrical Sandblast
        /// </summary>
        A7 = 10,
        /// <summary>
        /// Circular Without Light but with Black Frame
        /// </summary>
        A9 = 11,
        /// <summary>
        /// Unused Series
        /// </summary>
        M8 = 12,
        /// <summary>
        /// Pebble Design with Light
        /// </summary>
        ND = 13,
        /// <summary>
        /// Capsule with Light
        /// </summary>
        NC = 14,
        /// <summary>
        /// Ellipse with Light
        /// </summary>
        NL = 15,
        /// <summary>
        /// Genesis , Rectangle with Light inside Strip and Black Frame with Corner Radius
        /// </summary>
        P8 = 16,
        /// <summary>
        /// Isavella , Circular with Light inside Strip and Black Frame
        /// </summary>
        P9 = 17,
        /// <summary>
        /// Premium Hotel Rectangular with Light
        /// </summary>
        R7 = 18,
        /// <summary>
        /// Premium Diagonios with Light
        /// </summary>
        R9 = 19,
        /// <summary>
        /// Stone Series , (Stone Shaped Mirror)
        /// </summary>
        NS = 20,
        /// <summary>
        /// Round 1000 Series , Circle Segment Cut Horizontally on Bottom With Light
        /// </summary>
        N1 = 21,
        /// <summary>
        /// Round 2000 Series , Circle Segment Cut both Horizontally and Vertically with Light
        /// </summary>
        N2 = 22,
        /// <summary>
        /// Riviera Single Plexi Series , Rectangular Mirror with Light in a Single Plexi
        /// </summary>
        EL = 23,
        /// <summary>
        /// Riviera Double Plexi Series , Rectangular Mirror with Light in Double Plexi
        /// </summary>
        ES = 24,

        /// <summary>
        /// Rectangular Customized Mirror with Light
        /// </summary>
        IM = 25,
        /// <summary>
        /// Rectangular Customized Mirror without Light
        /// </summary>
        IA = 26,
        /// <summary>
        /// Circular Customized Mirror with Light
        /// </summary>
        N9Custom = 27,
        /// <summary>
        /// Circular Customized Mirror without Light
        /// </summary>
        NA = 28,
        /// <summary>
        /// Capsule Customized Mirror with Light
        /// </summary>
        NCCustom = 29,
        /// <summary>
        /// Capsule Customized Mirror without Light
        /// </summary>
        IC = 30,
        /// <summary>
        /// Ellipse Customized Mirror with Light
        /// </summary>
        NLCustom = 31,
        /// <summary>
        /// Ellipse Customized Mirror without Light
        /// </summary>
        IL = 32,
    }
}
