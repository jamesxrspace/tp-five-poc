import { IconType } from 'react-icons';
import { Navigate, IndexRouteObject, NonIndexRouteObject } from 'react-router-dom';
import { TranslationKey } from '@/i18n/i18n';
import { AuthContext } from '@/machines/auth';

interface BaseCustomRouteObject {
  translationKey?: TranslationKey;
  isHidden?: boolean;
  canEnter?: (authContext: AuthContext) => boolean;
}

export interface CustomIndexRouteObject extends BaseCustomRouteObject, IndexRouteObject {
  name?: never;
  icon?: never;
  redirectTo?: Navigate;
}

export interface CustomNonIndexRouteObject extends BaseCustomRouteObject, NonIndexRouteObject {
  name?: string;
  icon?: IconType;
  children?: (CustomIndexRouteObject | CustomNonIndexRouteObject)[];
}

export type CustomRouteObject = CustomIndexRouteObject | CustomNonIndexRouteObject;
