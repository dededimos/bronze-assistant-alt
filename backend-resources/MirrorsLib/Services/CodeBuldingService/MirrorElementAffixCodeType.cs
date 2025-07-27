namespace MirrorsLib.Services.CodeBuldingService
{
    public enum MirrorElementAffixCodeType
    {
        /// <summary>
        /// Do not use the Code of the Element
        /// </summary>
        NoneCode = 0,
        /// <summary>
        /// Use the Full Code of the Element
        /// </summary>
        FullElementCode = 1,
        /// <summary>
        /// Use the Short Code of the Element
        /// </summary>
        ShortElementCode = 2,
        /// <summary>
        /// Use the Minimal Code of the Element
        /// </summary>
        MinimalElementCode = 3,
    }
}
