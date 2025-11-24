export const todoItemQueryKeys = {
    all: ["todoItems"] as const,
    lists: () => [...todoItemQueryKeys.all, 'list'] as const,
    details: () => [...todoItemQueryKeys.all, 'detail'] as const,
    detail: (id: string) => [...todoItemQueryKeys.details(), id] as const,
};