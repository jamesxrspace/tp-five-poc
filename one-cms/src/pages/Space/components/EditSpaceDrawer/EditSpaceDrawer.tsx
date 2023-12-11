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
import { FiSave } from 'react-icons/fi';
import { useSpaceGroupOptions } from '../../hooks/useSpaceGroupOptions';
import { EditSpaceDrawerProps, EditSpaceFormValues } from './EditSpaceDrawer.type';
import Select from '@/components/Select/Select';
import { Option } from '@/components/Select/Select.type';
import Upload from '@/components/Upload/Upload';
import { THUMBNAIL_FILE_TYPE } from '@/constants/file';
import { UploadEvent, UploadState, uploadMachine } from '@/machines/upload';
import { formatDateTime } from '@/utils/date';

const EditSpaceDrawer = ({ isOpen, onClose, space, onEditSpace }: EditSpaceDrawerProps) => {
  const { t } = useTranslation<Namespace>();
  const drawerSize = useBreakpointValue({ base: 'full', md: 'xl' });
  const { isLoading, spaceGroupOptions, fetchNextPage, hasNextPage } = useSpaceGroupOptions();

  const {
    formState: { isSubmitting, errors },
    register,
    control,
    getValues,
    handleSubmit,
    reset,
  } = useForm<EditSpaceFormValues>();

  const [uploadState, uploadSend, uploadService] = useMachine(uploadMachine, {
    actions: {
      onUploaded: ({ uploadedFileUrls }) =>
        editSpace({ ...getValues(), thumbnail: uploadedFileUrls[0] }),
    },
  });
  const { pendingFiles, uploadedFileUrls } = uploadState.context;

  const editSpace = (data: EditSpaceFormValues) => {
    onEditSpace(data);
    reset();
  };
  const onSubmit: SubmitHandler<EditSpaceFormValues> = (data) => {
    if (pendingFiles.length) {
      uploadSend({ type: UploadEvent.UPLOAD });
    } else {
      editSpace({ ...data, thumbnail: uploadedFileUrls[0] });
    }
  };

  useEffect(() => {
    if (isOpen) {
      reset(space);
      uploadSend({
        type: UploadEvent.RESET_UPLOADED_FILES,
        uploadedFileUrls: space?.thumbnail ? [space.thumbnail] : [],
      });
    }
  }, [isOpen, reset, space, uploadSend]);

  if (!space) {
    return null;
  }

  return (
    <Drawer size={drawerSize} isOpen={isOpen} placement="right" onClose={onClose}>
      <DrawerOverlay />
      <form onSubmit={handleSubmit(onSubmit)} noValidate>
        <DrawerContent>
          <DrawerCloseButton />
          <DrawerHeader>{t('space:spaceList.editSpace.title')}</DrawerHeader>

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
                    defaultValue={space.startAt}
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
                    defaultValue={space.endAt}
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
              {t('space:spaceList.editSpace.cancel')}
            </Button>
            <Button
              variant="primary"
              type="submit"
              leftIcon={<FiSave />}
              alignSelf="flex-end"
              isLoading={uploadState.matches(UploadState.UPLOADING) || isSubmitting}
            >
              {t('space:spaceList.editSpace.save')}
            </Button>
          </DrawerFooter>
        </DrawerContent>
      </form>
    </Drawer>
  );
};
export default EditSpaceDrawer;
