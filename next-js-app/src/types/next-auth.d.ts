import { LoginResponse } from '@/generated-types/Api';
import type { User, UserObject } from 'next-auth';

export interface LoginResponse {
  token_refresh: string;
  token_access: string;
}

declare module 'next-auth' {
  export interface UserObject {
    id: string;
  }
  export interface DecodedJWT {
    token_type: 'refresh' | 'access';
    exp: number;
    iat: number;
    jti: string;
    user_id: number;
  }
  export interface AuthValidity {
    valid_until: number;
    refresh_until: number;
  }
  export interface User {
    tokens: LoginResponse;
    user: UserObject;
    validity: AuthValidity;
  }
  export interface Session {
    user: UserObject;
    validity: AuthValidity;
    tokens: LoginResponse;
  }
}

declare module 'next-auth/jwt' {
  export interface JWT {
    data: User;
  }
}

declare module 'next/server' {
  interface NextRequest {
    auth?: Session | null;
  }
}
