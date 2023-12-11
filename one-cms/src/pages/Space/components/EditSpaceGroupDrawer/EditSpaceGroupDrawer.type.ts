import { SpaceGroup, UpdateSpaceGroupRequest } from '@/api/openapi';

export interface EditSpaceGroupDrawerProps {
  spaceGroup?: SpaceGroup;
  isOpen: boolean;
  onClose: () => void;
  onEditSpaceGroup: (values: EditSpaceGroupFormValues) => void;
}

export type EditSpaceGroupFormValues = Omit<UpdateSpaceGroupRequest, 'spaceGroupId'>;
