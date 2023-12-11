import { Space, UpdateSpaceBody } from '@/api/openapi';

export interface EditSpaceDrawerProps {
  isOpen: boolean;
  onClose: () => void;
  space?: Space;
  onEditSpace: (space: EditSpaceFormValues) => void;
}

export type EditSpaceFormValues = UpdateSpaceBody;
