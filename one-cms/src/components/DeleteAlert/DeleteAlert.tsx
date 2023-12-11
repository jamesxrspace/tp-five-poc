import {
  AlertDialog,
  AlertDialogBody,
  AlertDialogContent,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogOverlay,
  Button,
} from '@chakra-ui/react';
import { useRef } from 'react';
import { useTranslation } from 'react-i18next';
import { DeleteAlertProps } from './DeleteAlert.type';

const DeleteAlert = ({ title, description, isOpen, onClose, onDelete }: DeleteAlertProps) => {
  const { t } = useTranslation();
  const cancelRef = useRef(null);

  return (
    <AlertDialog leastDestructiveRef={cancelRef} isOpen={isOpen} onClose={onClose}>
      <AlertDialogOverlay>
        <AlertDialogContent>
          <AlertDialogHeader fontSize="lg" fontWeight="bold">
            {title || t('alert.delete.title')}
          </AlertDialogHeader>

          <AlertDialogBody>{description || t('alert.delete.description')}</AlertDialogBody>

          <AlertDialogFooter>
            <Button ref={cancelRef} onClick={onClose}>
              {t('alert.delete.cancel')}
            </Button>
            <Button colorScheme="red" onClick={onDelete} ml={3}>
              {t('alert.delete.confirm')}
            </Button>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialogOverlay>
    </AlertDialog>
  );
};
export default DeleteAlert;
