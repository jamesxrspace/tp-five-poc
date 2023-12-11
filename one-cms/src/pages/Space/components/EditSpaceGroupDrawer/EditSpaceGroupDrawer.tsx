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
  Switch,
  useBreakpointValue,
} from '@chakra-ui/react';
import { useMachine } from '@xstate/react';
import { Namespace } from 'i18next';
import { isNil } from 'lodash-es';
import { useEffect } from 'react';
import { Controller, SubmitHandler, useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { FiSave } from 'react-icons/fi';
import { useSpaceOptions } from '../../hooks/useSpaceOptions';
import { EditSpaceGroupDrawerProps, EditSpaceGroupFormValues } from './EditSpaceGroupDrawer.type';
import { SpaceGroupStatusEnum } from '@/api/openapi';
import Select from '@/components/Select/Select';
import Upload from '@/components/Upload/Upload';
import { THUMBNAIL_FILE_TYPE } from '@/constants/file';
import { UploadEvent, uploadMachine } from '@/machines/upload';
import { formatDateTime } from '@/utils/date';

const EditSpaceGroupDrawer = ({
  spaceGroup,
  isOpen,
  onClose,
  onEditSpaceGroup,
}: EditSpaceGroupDrawerProps) => {
  const { t } = useTranslation<Namespace>();
  const drawerSize = useBreakpointValue({ base: 'full', md: 'xl' });
  const drawerPlacement = useBreakpointValue<'right' | 'bottom'>({ base: 'bottom', md: 'right' });
  const { isLoading, spaceOptions, fetchNextPage, hasNextPage } = useSpaceOptions();

  const {
    formState: { isSubmitting, errors },
    register,
    control,
    getValues,
    handleSubmit,
    reset,
  } = useForm<EditSpaceGroupFormValues>();

  const [uploadState, uploadSend, uploadService] = useMachine(uploadMachine, {
    actions: {
      onUploaded: ({ uploadedFileUrls }) =>
        editSpaceGroup({ ...getValues(), thumbnail: uploadedFileUrls[0] }),
    },
  });
  const { pendingFiles, uploadedFileUrls } = uploadState.context;

  const editSpaceGroup = (data: EditSpaceGroupFormValues) => {
    onEditSpaceGroup(data);
    reset();
  };
  const onSubmit: SubmitHandler<EditSpaceGroupFormValues> = (data) => {
    if (pendingFiles.length) {
      uploadSend({ type: UploadEvent.UPLOAD });
    } else {
      editSpaceGroup({ ...data, thumbnail: uploadedFileUrls[0] });
    }
  };

  useEffect(() => {
    if (isOpen) {
      reset(spaceGroup);
      uploadSend({
        type: UploadEvent.RESET_UPLOADED_FILES,
        uploadedFileUrls: spaceGroup?.thumbnail ? [spaceGroup.thumbnail] : [],
      });
    }
  }, [spaceGroup, reset, isOpen, uploadSend]);

  if (isNil(spaceGroup)) {
    return null;
  }

  return (
    <Drawer size={drawerSize} isOpen={isOpen} placement={drawerPlacement} onClose={onClose}>
      <DrawerOverlay />
      <form onSubmit={handleSubmit(onSubmit)} noValidate>
        <DrawerContent>
          <DrawerCloseButton />
          <DrawerHeader>{t('space:spaceGroupList.editGroup.title')}</DrawerHeader>

          <DrawerBody>
            <Stack spacing={4}>
              <FormControl isInvalid={!!errors.name} isRequired>
                <FormLabel>{t('space:spaceGroupList.field.name')}</FormLabel>
                <Input {...register('name', { required: t('common:form.error.required') })} />
                {errors.name && <FormErrorMessage>{errors.name.message}</FormErrorMessage>}
              </FormControl>

              <FormControl>
                <FormLabel>{t('space:spaceGroupList.field.thumbnail')}</FormLabel>
                <Upload service={uploadService} maxFiles={1} accept={THUMBNAIL_FILE_TYPE} />
              </FormControl>

              <FormControl isInvalid={!!errors.description}>
                <FormLabel>{t('space:spaceGroupList.field.description')}</FormLabel>
                <Input {...register('description')} />
                {errors.description && (
                  <FormErrorMessage>{errors.description.message}</FormErrorMessage>
                )}
              </FormControl>
              <FormControl isInvalid={!!errors.spaceIds}>
                <FormLabel>{t('space:spaceGroupList.field.spaceIds')}</FormLabel>
                <Controller
                  control={control}
                  name="spaceIds"
                  defaultValue={(spaceGroup?.spaces || []).map((space) => space.spaceId || '')}
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
              <FormControl isInvalid={!!errors.thumbnail}>
                <FormLabel>{t('space:spaceGroupList.field.thumbnail')}</FormLabel>
                <Input {...register('thumbnail')} />
                {errors.thumbnail && (
                  <FormErrorMessage>{errors.thumbnail.message}</FormErrorMessage>
                )}
              </FormControl>
              <FormControl>
                <FormLabel>{t('space:spaceGroupList.field.status')}</FormLabel>
                <Controller
                  control={control}
                  name="status"
                  defaultValue={spaceGroup?.status}
                  render={({ field: { value, onChange } }) => (
                    <Switch
                      isChecked={value === SpaceGroupStatusEnum.Enabled}
                      onChange={() => {
                        onChange(
                          value === SpaceGroupStatusEnum.Enabled
                            ? SpaceGroupStatusEnum.Disabled
                            : SpaceGroupStatusEnum.Enabled,
                        );
                      }}
                    >
                      {value === SpaceGroupStatusEnum.Enabled
                        ? t('space:spaceGroupList.field.statusEnabled')
                        : t('space:spaceGroupList.field.statusDisabled')}
                    </Switch>
                  )}
                />
              </FormControl>
              <Flex gap={4}>
                <FormControl isInvalid={!!errors.startAt}>
                  <FormLabel>{t('space:spaceGroupList.field.startAt')}</FormLabel>
                  <Controller
                    control={control}
                    name="startAt"
                    defaultValue={spaceGroup.startAt}
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
                    defaultValue={spaceGroup.endAt}
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
              {t('space:spaceGroupList.editGroup.cancel')}
            </Button>
            <Button variant="primary" type="submit" leftIcon={<FiSave />} isLoading={isSubmitting}>
              {t('space:spaceGroupList.editGroup.update')}
            </Button>
          </DrawerFooter>
        </DrawerContent>
      </form>
    </Drawer>
  );
};
export default EditSpaceGroupDrawer;
