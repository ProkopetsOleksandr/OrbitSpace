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

export type AuthError = {
  error: string;
  fieldErrors?: Record<string, string[]>;
};
