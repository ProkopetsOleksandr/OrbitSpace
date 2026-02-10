export const activityQueryKeys = {
  all: ['activities'] as const,

  lists: () => [...activityQueryKeys.all, 'list'] as const,
  list: (filters: string) => [...activityQueryKeys.lists(), filters] as const,

  details: () => [...activityQueryKeys.all, 'detail'] as const,
  detail: (id: string) => [...activityQueryKeys.details(), id] as const
};
