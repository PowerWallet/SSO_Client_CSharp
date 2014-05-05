namespace FinApps.SSO.RestClient
{
    public enum ResultCodeTypes
    {
        // ReSharper disable InconsistentNaming
        SUCCESSFUL = 0,

        // Account
        ACCOUNT_ResetUserPassword = 100101,
        ACCOUNT_InvalidUsernamePassword = 100102,
        ACCOUNT_UserNotAllowedToLogin = 100103,
        ACCOUNT_UserAccountLockedOut = 100104,
        ACCOUNT_UserDoesNotExist = 100105,
        ACCOUNT_LoginRequired = 100106,
        ACCOUNT_PasswordReset = 100107,
        ACCOUNT_InvalidPasswordLength = 100110,
        ACCOUNT_PasswordSaveSuccess = 100111,
        ACCOUNT_PasswordsDontMatch = 100112,
        ACCOUNT_InvalidUserToken = 100113,
        ACCOUNT_SessionExpired = 100114,
        ACCOUNT_NewUser = 100116,
        ACCOUNT_AccountSaveSuccessEmailFailed = 100117,
        ACCOUNT_AccountSaveSuccess = 100118,
        ACCOUNT_UserNameAlreadyExists = 100120,
        ACCOUNT_UserDeletedSuccess = 100121,
        ACCOUNT_NewCustomerUser = 100123,
        ACCOUNT_PasswordSaveFailure = 100125,
        ACCOUNT_PasswordResetSuccessCustomer = 100126,
        ACCOUNT_InvalidOriginalPassword = 100128,
        ACCOUNT_NewCustomerUserSavedSuccess = 100129,
        ACCOUNT_UserEmailExistsAlready = 100130,
        ACCOUNT_NewCustomerUserSaveFailure = 100131,
        ACCOUNT_UserDeleteSuccessPFM = 100134,
        ACCOUNT_InvalidPasswordCharacters = 100135,
        ACCOUNT_PasswordRequirementMessage = 100137,


        // Validation
        VALIDATION_IsRequired = 800100,
        VALIDATION_InvalidEmailFormat = 800101,
        VALIDATION_InvalidPhoneNumberFormat = 800102,
        VALIDATION_IsInvalid = 800103,
        VALIDATION_IsInvalidFormat = 800104,
        VALIDATION_InvalidForm = 800105,
        VALIDATION_ModelErrors = 800106,
        VALIDATION_MissingRequiredFields = 800107,
        VALIDATION_IsNullRequired = 800108,
        VALIDATION_InvalidDateFormat = 800109,
        VALIDATION_InvalidValue = 800110,
        VALIDATION_IsRequiredAndNumeric = 800111,

        // Exception
        EXCEPTION_GeneralException = 900101,
        EXCEPTION_DatabaseException = 900102,
        EXCEPTION_AppException = 900103,
        EXCEPTION_ConfigurationException = 900104,
        EXCEPTION_CountryNotAvailable = 900105,
        EXCEPTION_WebServiceException = 900106,
        EXCEPTION_MissingRequiredFields = 900107,
        EXCEPTION_UnableToConnect = 900108,
        EXCEPTION_TimedOutWaitingForResponse = 900109,
        EXCEPTION_UnableToDeSerializeObject = 900110,
        EXCEPTION_UnableToReadWebRequest = 900111,
        EXCEPTION_HttpAntiForgeryException = 900112,
        EXCEPTION_TaskServerSubmissionException = 900113,
        EXCEPTION_TaskServerHealthIssues = 900114,
        EXCEPTION_MissingUploadContent = 900115,
        EXCEPTION_ContentNotInCache = 900116,
        EXCEPTION_InvalidMethodCalled = 900117,
        EXCEPTION_HttpRequestValidation = 900118

        // ReSharper restore InconsistentNaming
    }
}