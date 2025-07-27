using DataAccessLib.NoSQLModels;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.Helpers.Other
{
    public static class VariousMaps
    {
        public static readonly Dictionary<CabinModelEnum, string> CabinModelPhotos = new()
        {
            {CabinModelEnum.Model9A,        "https://storagebronze.blob.core.windows.net/cabins-images/Cabins/9A.png" },
            {CabinModelEnum.Model9S,        "https://storagebronze.blob.core.windows.net/cabins-images/Cabins/9S.png" },
            {CabinModelEnum.Model94,        "https://storagebronze.blob.core.windows.net/cabins-images/Cabins/94.png" },
            {CabinModelEnum.Model9F,        "https://storagebronze.blob.core.windows.net/cabins-images/Cabins/9S9F.png" },
            {CabinModelEnum.Model9B,        "https://storagebronze.blob.core.windows.net/cabins-images/Cabins/9B.png" },
            {CabinModelEnum.ModelW,         "https://storagebronze.blob.core.windows.net/cabins-images/Cabins/W.png" },
            {CabinModelEnum.ModelHB,        "https://storagebronze.blob.core.windows.net/cabins-images/Cabins/HB34.png" },
            {CabinModelEnum.ModelNP,        "https://storagebronze.blob.core.windows.net/cabins-images/Cabins/NP44.png" },
            {CabinModelEnum.ModelVS,        "https://storagebronze.blob.core.windows.net/cabins-images/Cabins/VS.jpg" },
            {CabinModelEnum.ModelVF,        "https://storagebronze.blob.core.windows.net/cabins-images/Cabins/VF.jpg" },
            {CabinModelEnum.ModelV4,        "https://storagebronze.blob.core.windows.net/cabins-images/Cabins/V4.jpg" },
            {CabinModelEnum.ModelVA,        "https://storagebronze.blob.core.windows.net/cabins-images/Cabins/VA.jpg" },
            {CabinModelEnum.ModelWS,        "https://storagebronze.blob.core.windows.net/cabins-images/Cabins/WS.png" },
            {CabinModelEnum.ModelE,         "https://storagebronze.blob.core.windows.net/cabins-images/Cabins/E.png" },
            {CabinModelEnum.ModelWFlipper,  "https://storagebronze.blob.core.windows.net/cabins-images/Cabins/WFlipper.png" },
            {CabinModelEnum.ModelDB,        "https://storagebronze.blob.core.windows.net/cabins-images/Cabins/DB51.png" },
            {CabinModelEnum.ModelNB,        "https://storagebronze.blob.core.windows.net/cabins-images/Cabins/NB31.png" },
            {CabinModelEnum.ModelNV,        "https://storagebronze.blob.core.windows.net/cabins-images/Cabins/NV.png" },
            {CabinModelEnum.ModelMV2,       "https://storagebronze.blob.core.windows.net/cabins-images/Cabins/MV2.png" },
            {CabinModelEnum.ModelNV2,       "https://storagebronze.blob.core.windows.net/cabins-images/Cabins/NV2.png" },
            {CabinModelEnum.Model6WA,       "Not Available" },
            {CabinModelEnum.Model9C,        "https://storagebronze.blob.core.windows.net/cabins-images/Cabins/9C.png" },
            {CabinModelEnum.Model8W40,      "https://storagebronze.blob.core.windows.net/cabins-images/Cabins/8W40.jpg" },
            {CabinModelEnum.ModelGlassContainer, "Not Available" },
            {CabinModelEnum.ModelQB,        "Not Available" },
            {CabinModelEnum.ModelQP,        "Not Available" },
        };

    }
}
