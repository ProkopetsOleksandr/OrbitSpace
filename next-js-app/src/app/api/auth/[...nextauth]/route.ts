import NextAuth from 'next-auth';
import CredentialsProvider from 'next-auth/providers/credentials';

const handler = NextAuth({
  session: { strategy: 'jwt' },
  providers: [
    CredentialsProvider({
      credentials: {
        email: { type: 'text' },
        password: { type: 'password' }
      },
      async authorize(credentials, req) {
        if (!credentials?.email || !credentials?.password) {
          throw new Error('Invalid credentials');
        }

        try {
          const res = await fetch(`${process.env.BACKEND_BASE_URL}/api/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
              email: credentials.email,
              password: credentials.password
            })
          });

          if (!res.ok) {
            throw new Error('Invalid credentials or server error');
          }

          const data = await res.json();

          if (data?.user && data?.accessToken) {
            return {
              id: data.user.id,
              name: data.user.username,
              accessToken: data.accessToken
            };
          }

          throw new Error('Invalid response from server');
        } catch (error) {
          throw new Error('Authentication failed');
        }
      }
    })
  ],
  callbacks: {
    async jwt({ token, user }) {
      token.id = user.id;
      token.name = user.name;
      token.accessToken = user.accessToken;

      return token;
    },
    async session({ session, token }) {
      session.user = {
        id: token.id,
        name: token.name
      };

      return session;
    }
  },
  secret: process.env.NEXTAUTH_SECRET,
  pages: {
    signIn: '/login'
  }
});

export { handler as GET, handler as POST };
