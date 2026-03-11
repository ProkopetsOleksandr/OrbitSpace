export type AuthUser = {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  dateOfBirth: string;
  emailVerified: boolean;
};

export type AuthResponse = {
  user: AuthUser;
};

// TODO: Why it was created?
export type AuthError = {
  error: string;
  errorCode: string;
  fieldErrors?: Record<string, string[]>;
};

export const AUTH_ERROR_CODE = {
  Auth: {
    EmailAlreadyExists: 'AUTH_EMAIL_ALREADY_EXISTS',
    InvalidCredentials: 'AUTH_INVALID_CREDENTIALS',
    EmailNotVerified: 'AUTH_EMAIL_NOT_VERIFIED',
    AccountDisabled: 'AUTH_ACCOUNT_DISABLED',
    InvalidRefreshToken: 'AUTH_INVALID_REFRESH_TOKEN',
    VerificationLinkExpired: 'AUTH_VERIFICATION_LINK_EXPIRED',
    EmailAlreadyVerified: 'AUTH_EMAIL_ALREADY_VERIFIED'
  },
  Common: {
    NotFound: 'NOT_FOUND',
    ValidationError: 'VALIDATION_ERROR',
    Unauthorized: 'UNAUTHORIZED'
  }
} as const;
