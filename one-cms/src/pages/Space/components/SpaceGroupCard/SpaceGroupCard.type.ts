import { SpaceGroup } from '@/api/openapi';

export interface SpaceGroupCardProps {
  spaceGroup: SpaceGroup;
  onEditSpaceGroup: (spaceGroup: SpaceGroup) => void;
  onDeleteSpaceGroup: (spaceGroup: SpaceGroup) => void;
}
