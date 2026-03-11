namespace OrbitSpace.Application.Common;

public static class ErrorCode
{
    public static class Common
    {
        public const string NotFound = "NOT_FOUND";
        public const string ValidationError = "VALIDATION_ERROR";
        public const string Unauthorized = "UNAUTHORIZED";
    }

    public static class Auth
    {
        public const string EmailAlreadyExists = "AUTH_EMAIL_ALREADY_EXISTS";
        public const string InvalidCredentials = "AUTH_INVALID_CREDENTIALS";
        public const string EmailNotVerified = "AUTH_EMAIL_NOT_VERIFIED";
        public const string AccountDisabled = "AUTH_ACCOUNT_DISABLED";
        public const string InvalidRefreshToken = "AUTH_INVALID_REFRESH_TOKEN";
        public const string VerificationLinkExpired = "AUTH_VERIFICATION_LINK_EXPIRED";
        public const string EmailAlreadyVerified = "AUTH_EMAIL_ALREADY_VERIFIED";
    }
}
