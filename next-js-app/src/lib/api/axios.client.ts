'use client';

import axios from 'axios';

export const axiosClient = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL,
  headers: { 'Content-Type': 'application/json' }
});

// Интерцептор для добавления JWT токена Clerk
axiosClient.interceptors.request.use(async config => {
  // Clerk доступен в window на клиенте
  const clerk = (window as any).Clerk;
  const token = await clerk?.session?.getToken();
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});
