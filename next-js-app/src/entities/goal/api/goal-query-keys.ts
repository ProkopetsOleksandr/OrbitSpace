export const goalQueryKeys = {
  all: ['goals'] as const,

  lists: () => [...goalQueryKeys.all, 'list'] as const,
  list: (filters: string) => [...goalQueryKeys.lists(), filters] as const,

  details: () => [...goalQueryKeys.all, 'detail'] as const,
  detail: (id: string) => [...goalQueryKeys.details(), id] as const
};
