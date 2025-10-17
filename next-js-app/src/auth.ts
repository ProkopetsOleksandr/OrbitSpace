import { jwtDecode } from 'jwt-decode';
import NextAuth, { Account, AuthValidity, DecodedJWT, Session, User, UserObject } from 'next-auth';
import type { JWT } from 'next-auth/jwt';
import CredentialsProvider from 'next-auth/providers/credentials';
import { LoginResponse } from './types/next-auth';

export const { handlers, signIn, signOut, auth } = NextAuth({
  secret: process.env.NEXTAUTH_SECRET,
  session: { strategy: 'jwt' },
  providers: [
    CredentialsProvider({
      id: 'LoginEmailPassword',
      name: 'LoginEmailPassword',
      credentials: {
        email: { label: 'Email', type: 'text' },
        password: { label: 'Password', type: 'password' }
      },
      authorize: async credentials => authorizeCredentialsProvider(credentials, '/api/auth/login')
    })
  ],
  callbacks: {
    authorized: async ({ auth }) => !!auth,
    jwt: async ({ token, user, account }) => handleJWT(token, user, account),
    session: async ({ session, token, user }) => handleSession(session, token, user)
  },
  pages: {
    signIn: '/login'
  }
});

async function post(body: object, slug: string): Promise<Response> {
  return await fetch(`${process.env.BACKEND_BASE_URL}/${slug}`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body)
  });
}

async function refreshAccessToken(nextAuthJWTCookie: JWT): Promise<JWT> {
  try {
    const body = { token_refresh: nextAuthJWTCookie.data.tokens.token_refresh };
    const res = await post(body, '/api/token_refresh');
    const tokens: LoginResponse = await res.json();
    if (!res.ok) throw tokens;
    const { exp }: DecodedJWT = jwtDecode(tokens.token_access);
    nextAuthJWTCookie.data.validity.valid_until = exp;
    nextAuthJWTCookie.data.tokens.token_access = tokens.token_access;
    return { ...nextAuthJWTCookie };
  } catch (error) {
    return { ...nextAuthJWTCookie, error: 'RefreshAccessTokenError' };
  }
}

function parseResponse(res: LoginResponse): User {
  const access: DecodedJWT = jwtDecode(res.token_access);
  const refresh: DecodedJWT = jwtDecode(res.token_refresh);
  const user: UserObject = { id: access.user_id.toString() };
  const validity: AuthValidity = { valid_until: access.exp, refresh_until: refresh.exp };

  return {
    id: refresh.jti, // User object needs to have a string id so use refresh token id
    tokens: res,
    user: user,
    validity: validity
  };
}

async function authorizeCredentialsProvider(credentials: Partial<Record<'email' | 'password', unknown>>, backendUrl: string) {
  const body = {
    email: credentials.email?.toString() || '',
    password: credentials.password?.toString() || ''
  };
  const res = await post(body, backendUrl);
  const tokens: LoginResponse = await res.json();

  if (!res.ok) {
    throw tokens;
  }

  return parseResponse(tokens);
}

export async function authorizeGoogleProvider(email: string) {
  const body = { email };
  const res = await post(body, 'api/google');
  const tokens: LoginResponse = await res.json();

  if (!res.ok) {
    throw tokens;
  }

  return parseResponse(tokens);
}

async function handleJWT(token: JWT, user: User, account: Account | null | undefined): Promise<JWT | null> {
  if (user && account) {
    return { ...token, data: user };
  }
  if (Date.now() < token.data.validity.valid_until * 1000) {
    return token;
  }
  if (Date.now() < token.data.validity.refresh_until * 1000) {
    // get new token from server
    return await refreshAccessToken(token);
  }
  return null;
}

async function handleSession(session: Session, token: JWT, user: User) {
  session.user = { ...user, ...token.data.user };
  session.validity = token.data.validity;

  // session.error = token.error;

  return session;
}
