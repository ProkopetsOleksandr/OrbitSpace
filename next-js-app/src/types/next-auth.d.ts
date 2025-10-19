import { DefaultJWT, DefaultSession, DefaultUser } from 'next-auth'; // Імпортуй дефолти з next-auth (вони pull'ють з @auth/core)
import { JWT } from 'next-auth/jwt'; // Для аугментації JWT

declare module 'next-auth' {
  interface User extends DefaultUser {
    accessToken: string;
  }

  export interface Session extends DefaultSession {}
}

declare module 'next-auth/jwt' {
  export interface JWT extends DefaultJWT {
    accessToken: string;
    accessTokenExpiresInMs?: number;
  }
}
