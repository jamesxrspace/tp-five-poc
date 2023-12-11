import { CreateSpaceBody } from '@/api/openapi';

export interface AddSpaceDrawerProps {
  isOpen: boolean;
  onClose: () => void;
  onAddSpace: (space: AddSpaceFormValues) => void;
}

export type AddSpaceFormValues = CreateSpaceBody;
