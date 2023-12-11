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
import { useSpaceGroupOptions } from '../../hooks/useSpaceGroupOptions';
import { AddSpaceFormValues, AddSpaceDrawerProps } from './AddSpaceDrawer.type';
import Select from '@/components/Select/Select';
import { Option } from '@/components/Select/Select.type';
import Upload from '@/components/Upload/Upload';
import { THUMBNAIL_FILE_TYPE } from '@/constants/file';
import { UploadEvent, UploadState, uploadMachine } from '@/machines/upload';
import { formatDateTime } from '@/utils/date';

const AddSpaceDrawer = ({ isOpen, onClose, onAddSpace }: AddSpaceDrawerProps) => {
  const { t } = useTranslation<Namespace>();
  const drawerSize = useBreakpointValue({ base: 'full', md: 'xl' });
  const { isLoading, spaceGroupOptions, fetchNextPage, hasNextPage } = useSpaceGroupOptions();
  const {
    register,
    control,
    getValues,
    formState: { errors, isSubmitting },
    handleSubmit,
    reset,
  } = useForm<AddSpaceFormValues>();

  const [uploadState, uploadSend, uploadService] = useMachine(uploadMachine, {
    actions: {
      onUploaded: ({ uploadedFileUrls }) =>
        addSpace({ ...getValues(), thumbnail: uploadedFileUrls[0] }),
    },
  });
  const { pendingFiles, uploadedFileUrls } = uploadState.context;

  const addSpace = (data: AddSpaceFormValues) => {
    onAddSpace(data);
    reset();
  };
  const onSubmit: SubmitHandler<AddSpaceFormValues> = (data) => {
    if (pendingFiles.length) {
      uploadSend({ type: UploadEvent.UPLOAD });
    } else {
      addSpace({ ...data, thumbnail: uploadedFileUrls[0] });
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
          <DrawerHeader>{t('space:spaceList.addSpace.title')}</DrawerHeader>

          <DrawerBody>
            <Stack spacing={4}>
              <FormControl isInvalid={!!errors.name} isRequired>
                <FormLabel>{t('space:spaceList.field.name')}</FormLabel>
                <Input {...register('name', { required: t('common:form.error.required') })} />
                {errors.name && <FormErrorMessage>{errors.name.message}</FormErrorMessage>}
              </FormControl>

              <FormControl isInvalid={!!errors.spaceGroupId}>
                <FormLabel>{t('space:spaceList.field.spaceGroupId')}</FormLabel>
                <Controller
                  control={control}
                  name="spaceGroupId"
                  render={({ field: { value, onChange } }) => (
                    <Select<Option, false>
                      value={value}
                      options={spaceGroupOptions}
                      onChange={(value) => onChange(value)}
                      isLoading={isLoading}
                      onMenuScrollToBottom={() => hasNextPage && fetchNextPage()}
                    />
                  )}
                />
                {errors.spaceGroupId && (
                  <FormErrorMessage>{errors.spaceGroupId.message}</FormErrorMessage>
                )}
              </FormControl>

              <FormControl>
                <FormLabel>{t('space:spaceList.field.thumbnail')}</FormLabel>
                <Upload service={uploadService} maxFiles={1} accept={THUMBNAIL_FILE_TYPE} />
              </FormControl>

              <FormControl isInvalid={!!errors.description}>
                <FormLabel>{t('space:spaceList.field.description')}</FormLabel>
                <Input {...register('description')} />
                {errors.description && (
                  <FormErrorMessage>{errors.description.message}</FormErrorMessage>
                )}
              </FormControl>

              <FormControl isInvalid={!!errors.addressable}>
                <FormLabel>{t('space:spaceList.field.addressable')}</FormLabel>
                <Input {...register('addressable')} />
                {errors.addressable && (
                  <FormErrorMessage>{errors.addressable.message}</FormErrorMessage>
                )}
              </FormControl>

              <Flex gap={4}>
                <FormControl isInvalid={!!errors.startAt}>
                  <FormLabel>{t('space:spaceList.field.startAt')}</FormLabel>
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
                  <FormLabel>{t('space:spaceList.field.endAt')}</FormLabel>
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
              {t('space:spaceList.addSpace.cancel')}
            </Button>
            <Button
              variant="primary"
              type="submit"
              leftIcon={<FiPlus />}
              isLoading={uploadState.matches(UploadState.UPLOADING) || isSubmitting}
            >
              {t('space:spaceList.addSpace.add')}
            </Button>
          </DrawerFooter>
        </DrawerContent>
      </form>
    </Drawer>
  );
};
export default AddSpaceDrawer;
