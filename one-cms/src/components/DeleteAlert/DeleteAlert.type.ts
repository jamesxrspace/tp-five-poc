export interface DeleteAlertProps {
  title?: string;
  description?: string;
  isOpen: boolean;
  onClose: () => void;
  onDelete: () => void;
}
