import { Space } from '@/api/openapi';

export interface SpaceCardProps {
  space: Space;
  onEditSpace: (space: Space) => void;
  onDeleteSpace: (space: Space) => void;
}
