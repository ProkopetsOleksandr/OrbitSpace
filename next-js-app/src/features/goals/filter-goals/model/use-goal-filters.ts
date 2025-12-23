'use client';

import { usePathname, useRouter, useSearchParams } from 'next/navigation';

export interface GoalFilters {
  search: string | null;
}

export const useGoalFilters = () => {
  const router = useRouter();
  const pathname = usePathname();
  const searchParams = useSearchParams();

  const filters: GoalFilters = {
    search: searchParams?.get('search') ?? null
  };

  const setFilters = (newFilters: Partial<GoalFilters>) => {
    if (!pathname) {
      return;
    }

    const params = new URLSearchParams(searchParams?.toString());

    Object.entries(newFilters).forEach(([key, value]) => {
      if (!value) {
        params.delete(key);
      } else {
        params.set(key, value);
      }
    });

    router.push(`${pathname}?${params.toString()}`, { scroll: false });
  };

  const clearFilters = () => {
    if (pathname) {
      router.push(pathname, { scroll: false });
    }
  };

  return { filters, setFilters, clearFilters };
};
