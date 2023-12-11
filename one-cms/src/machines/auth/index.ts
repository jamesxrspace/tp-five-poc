import { interpret } from 'xstate';
import { authMachine } from '@/machines/auth/machine';

export * from '@/machines/auth/machine';
export * from '@/machines/auth/type';

export const authService = interpret(authMachine).start();
