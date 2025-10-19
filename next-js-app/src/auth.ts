import NextAuth, { User } from 'next-auth';
import type { JWT } from 'next-auth/jwt';
import CredentialsProvider from 'next-auth/providers/credentials';

interface LoginResponse {
  accessToken: string;
  user: {
    id: string;
    email: string;
    firstName: string;
    lastName: string;
  };
}

export const { handlers, signIn, signOut, auth } = NextAuth({
  debug: true,
  secret: process.env.NEXTAUTH_SECRET,
  session: { strategy: 'jwt' },
  providers: [
    CredentialsProvider({
      credentials: {
        email: { label: 'Email', type: 'text' },
        password: { label: 'Password', type: 'password' }
      },
      async authorize(credentials) {
        const payload = {
          username: credentials.email?.toString() || '',
          password: credentials.password?.toString() || ''
        };

        const res = await fetch(`${process.env.BACKEND_BASE_URL}/api/auth/login`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(payload)
        });

        if (!res.ok) {
          return null;
        }

        try {
          const data: LoginResponse = await res.json();

          return {
            id: data.user.id,
            email: data.user.email,
            name: data.user.firstName + ' ' + data.user.lastName,
            accessToken: data.accessToken
          };
        } catch {
          return null;
        }
      }
    })
  ],
  callbacks: {
    async jwt({ token, user }: { token: JWT; user?: User }) {
      if (user) {
        token.sub = user.id;
        token.email = user.email;
        token.name = user.name;
        token.accessToken = user.accessToken;

        try {
          const payload = JSON.parse(atob(user.accessToken.split('.')[1]));
          token.accessTokenExpiresInMs = payload.exp * 1000;
        } catch (e) {
          return null;
        }

        return token;
      }

      const isTokenValid = token.accessTokenExpiresInMs && Date.now() < token.accessTokenExpiresInMs;
      if (isTokenValid) {
        return token;
      }

      // TODO: refresh token

      return null;
    }
  },
  pages: {
    signIn: '/login'
  }
});

// async function post(body: object, slug: string): Promise<Response> {
//   return await fetch(`${process.env.BACKEND_BASE_URL}/${slug}`, {
//     method: 'POST',
//     headers: { 'Content-Type': 'application/json' },
//     body: JSON.stringify(body)
//   });
// }

// export async function authorizeGoogleProvider(email: string) {
//   const body = { email };
//   const res = await post(body, 'api/google');
//   const tokens: LoginResponse = await res.json();

//   if (!res.ok) {
//     throw tokens;
//   }

//   return parseResponse(tokens);
// }
