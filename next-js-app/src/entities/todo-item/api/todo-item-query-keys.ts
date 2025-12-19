export const todoItemQueryKeys = {
  all: ['todoItems'] as const,

  lists: () => [...todoItemQueryKeys.all, 'list'] as const,
  list: () => [...todoItemQueryKeys.lists()] as const,

  details: () => [...todoItemQueryKeys.all, 'detail'] as const,
  detail: (id: string) => [...todoItemQueryKeys.details(), id] as const
};
