import { components } from './v1';

// Enums
export { GoalStatus, LifeArea, TodoItemStatus } from './v1';

// Goal related types
export type Goal = components['schemas']['Goal'];
export type CreateGoalPayload = components['schemas']['CreateGoalPayload'];
export type UpdateGoalPayload = components['schemas']['UpdateGoalPayload'];

// TodoItem related types
export type TodoItem = components['schemas']['TodoItem'];
export type CreateTodoItemPayload = components['schemas']['CreateTodoItemPayload'];
export type UpdateTodoItemPayload = components['schemas']['UpdateTodoItemPayload'];
