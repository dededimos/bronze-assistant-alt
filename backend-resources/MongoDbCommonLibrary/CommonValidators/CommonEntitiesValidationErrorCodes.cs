namespace MongoDbCommonLibrary.CommonValidators
{
    public static class CommonEntitiesValidationErrorCodes
    {
        public const string InvalidObjectId = "InvalidObjectId";
        public const string UndefinedOptionsType = "UndefinedOptionsType";
        public const string NameDefaultValueCannotBeEmpty = "NameDefaultValueCannotBeEmpty";
        public const string LocalizedDefaultValueCannotBeEmpty = "LocalizedDefaultValueCannotBeEmpty";
        public const string LocalizedStringAllLocales = "LocalizedStringShouldHaveAllLocales";
        public const string NameAllLocales = "NameShouldHaveAllLocales";
        public const string DescriptionAllOrNoneLocales = "DescriptionAllOrNoneLocales";
        public const string DescriptionDefaultUndefined = "DescriptionDefaultCannotBeEmptyWhenLocalesPresent";
        public const string ExtendedDescriptionAllOrNoneLocales = "ExtendedDescriptionAllOrNoneLocales";
        public const string ExtendedDescriptionDefaultUndefined = "ExtendedDescriptionDefaultCannotBeEmptyWhenLocalesPresent";
    }
}
