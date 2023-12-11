import {
  Button,
  Drawer,
  DrawerBody,
  DrawerCloseButton,
  DrawerContent,
  DrawerFooter,
  DrawerHeader,
  DrawerOverlay,
  Flex,
  FormControl,
  FormErrorMessage,
  FormLabel,
  Input,
  Stack,
  useBreakpointValue,
} from '@chakra-ui/react';
import { useMachine } from '@xstate/react';
import { Namespace } from 'i18next';
import { useEffect } from 'react';
import { Controller, SubmitHandler, useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { FiPlus } from 'react-icons/fi';
import { useSpaceOptions } from '../../hooks/useSpaceOptions';
import { AddSpaceGroupFormValues, AddSpaceGroupDrawerProps } from './AddSpaceGroupDrawer.type';
import Select from '@/components/Select/Select';
import Upload from '@/components/Upload/Upload';
import { THUMBNAIL_FILE_TYPE } from '@/constants/file';
import { UploadEvent, uploadMachine } from '@/machines/upload';
import { formatDateTime } from '@/utils/date';

const AddSpaceGroupDrawer = ({ isOpen, onClose, onAddSpaceGroup }: AddSpaceGroupDrawerProps) => {
  const { t } = useTranslation<Namespace>();

  const drawerSize = useBreakpointValue({ base: 'full', md: 'xl' });
  const { isLoading, spaceOptions, fetchNextPage, hasNextPage } = useSpaceOptions();

  const {
    reset,
    register,
    control,
    formState: { errors, isSubmitting },
    getValues,
    handleSubmit,
  } = useForm<AddSpaceGroupFormValues>();

  const [uploadState, uploadSend, uploadService] = useMachine(uploadMachine, {
    actions: {
      onUploaded: ({ uploadedFileUrls }) =>
        addSpaceGroup({ ...getValues(), thumbnail: uploadedFileUrls[0] }),
    },
  });
  const { pendingFiles, uploadedFileUrls } = uploadState.context;

  const addSpaceGroup = (data: AddSpaceGroupFormValues) => {
    onAddSpaceGroup(data);
    reset();
  };
  const onSubmit: SubmitHandler<AddSpaceGroupFormValues> = (data) => {
    if (pendingFiles.length) {
      uploadSend({ type: UploadEvent.UPLOAD });
    } else {
      addSpaceGroup({ ...data, thumbnail: uploadedFileUrls[0] });
    }
  };

  useEffect(() => {
    if (isOpen) {
      reset({});
      uploadSend({ type: UploadEvent.RESET_UPLOADED_FILES, uploadedFileUrls: [] });
    }
  }, [isOpen, reset, uploadSend]);

  return (
    <Drawer size={drawerSize} isOpen={isOpen} placement="right" onClose={onClose}>
      <DrawerOverlay />
      <form onSubmit={handleSubmit(onSubmit)} noValidate>
        <DrawerContent>
          <DrawerCloseButton />
          <DrawerHeader>{t('space:spaceGroupList.addGroup.title')}</DrawerHeader>

          <DrawerBody>
            <Stack spacing={4}>
              <FormControl isInvalid={!!errors.name} isRequired>
                <FormLabel>{t('space:spaceGroupList.field.name')}</FormLabel>
                <Input {...register('name', { required: t('common:form.error.required') })} />
                {errors.name && <FormErrorMessage>{errors.name.message}</FormErrorMessage>}
              </FormControl>

              <FormControl isInvalid={!!errors.description}>
                <FormLabel>{t('space:spaceGroupList.field.description')}</FormLabel>
                <Input {...register('description')} />
                {errors.description && (
                  <FormErrorMessage>{errors.description.message}</FormErrorMessage>
                )}
              </FormControl>

              <FormControl>
                <FormLabel>{t('space:spaceGroupList.field.thumbnail')}</FormLabel>
                <Upload service={uploadService} maxFiles={1} accept={THUMBNAIL_FILE_TYPE} />
              </FormControl>

              <FormControl isInvalid={!!errors.spaceIds}>
                <FormLabel>{t('space:spaceGroupList.field.spaceIds')}</FormLabel>
                <Controller
                  control={control}
                  name="spaceIds"
                  render={({ field: { value, onChange } }) => (
                    <Select
                      isMulti
                      value={value}
                      options={spaceOptions}
                      onChange={(value) => onChange(value)}
                      isLoading={isLoading}
                      onMenuScrollToBottom={() => hasNextPage && fetchNextPage()}
                    />
                  )}
                />
                {errors.spaceIds && <FormErrorMessage>{errors.spaceIds.message}</FormErrorMessage>}
              </FormControl>

              <Flex gap={4}>
                <FormControl isInvalid={!!errors.startAt}>
                  <FormLabel>{t('space:spaceGroupList.field.startAt')}</FormLabel>
                  <Controller
                    control={control}
                    name="startAt"
                    render={({ field: { value, onChange } }) => (
                      <Input
                        type="datetime-local"
                        value={formatDateTime(value)}
                        onChange={(e) => onChange(new Date(e.target.value))}
                      />
                    )}
                  />
                  {errors.startAt && <FormErrorMessage>{errors.startAt.message}</FormErrorMessage>}
                </FormControl>
                <FormControl isInvalid={!!errors.endAt}>
                  <FormLabel>{t('space:spaceGroupList.field.endAt')}</FormLabel>
                  <Controller
                    control={control}
                    name="endAt"
                    render={({ field: { value, onChange } }) => (
                      <Input
                        type="datetime-local"
                        value={formatDateTime(value)}
                        onChange={(e) => onChange(new Date(e.target.value))}
                      />
                    )}
                  />
                  {errors.endAt && <FormErrorMessage>{errors.endAt.message}</FormErrorMessage>}
                </FormControl>
              </Flex>
            </Stack>
          </DrawerBody>

          <DrawerFooter>
            <Button variant="outline" mr={3} onClick={onClose}>
              {t('space:spaceGroupList.addGroup.cancel')}
            </Button>
            <Button variant="primary" type="submit" leftIcon={<FiPlus />} isLoading={isSubmitting}>
              {t('space:spaceGroupList.addGroup.add')}
            </Button>
          </DrawerFooter>
        </DrawerContent>
      </form>
    </Drawer>
  );
};
export default AddSpaceGroupDrawer;
