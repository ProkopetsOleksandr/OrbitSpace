import { components, GoalStatus, LifeArea } from '@/shared/types/api-types';

export type Goal = components['schemas']['Goal'];
export type CreateGoalPayload = components['schemas']['CreateGoalPayload'];
export type UpdateGoalPayload = components['schemas']['UpdateGoalPayload'];

export { GoalStatus, LifeArea };
