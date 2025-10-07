import { SignJWT, jwtVerify } from 'jose';
import { cookies } from 'next/headers';
import 'server-only';

const alg = 'HS512';
const sessionCookieName = 'session';
const secretKey = process.env.JWT_SECRET;
const encodedKey = new TextEncoder().encode(secretKey);

export async function createSession(token: string) {
  const expiresAt = new Date(Date.now() + 7 * 24 * 60 * 60 * 1000);

  (await cookies()).set(sessionCookieName, token, {
    httpOnly: true,
    secure: true,
    expires: expiresAt,
    sameSite: true
  });
}

export async function deleteSession() {
  (await cookies()).delete(sessionCookieName);
}

export async function decryptSession(session: string | undefined = '') {
  try {
    const { payload } = await jwtVerify(session, encodedKey, { algorithms: [alg] });
    return payload;
  } catch (error) {
    console.log('Failed to verify session');
  }
}
