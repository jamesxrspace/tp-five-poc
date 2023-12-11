import { CreateSpaceGroupRequest } from '@/api/openapi';

export interface AddSpaceGroupDrawerProps {
  isOpen: boolean;
  onClose: () => void;
  onAddSpaceGroup: (values: AddSpaceGroupFormValues) => void;
}

export type AddSpaceGroupFormValues = CreateSpaceGroupRequest;
