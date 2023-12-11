import { Stack } from '@chakra-ui/react';
import { useActor } from '@xstate/react';
import { FileCard } from '../FileCard/FileCard';
import { FileDropzone } from '../FileDropzone/FileDropzone';
import { UploadProps } from './Upload.type';
import { UploadEvent, UploadState } from '@/machines/upload';

const Upload = ({ service, maxFiles, ...rest }: UploadProps) => {
  const [state, send] = useActor(service);
  const { uploadedFileUrls, pendingFiles } = state.context;

  const isUploading = state.matches(UploadState.UPLOADING);
  const canDrop = !maxFiles || uploadedFileUrls.length + pendingFiles.length < maxFiles;

  const onDrop = (files: File[]) => send({ type: UploadEvent.ADD_FILES, files });

  return (
    <Stack>
      {canDrop && (
        <FileDropzone
          isDisabled={isUploading}
          isUploading={isUploading}
          dropzoneOptions={{ maxFiles, ...rest, onDrop }}
        />
      )}
      {uploadedFileUrls.map((url, index) => (
        <FileCard
          key={`${url}${index}`}
          file={url}
          onRemove={() => send({ type: UploadEvent.REMOVE_FILE, file: url })}
        />
      ))}
      {pendingFiles.map((file, index) => (
        <FileCard
          key={`${file.name}${index}`}
          file={file}
          onRemove={() => send({ type: UploadEvent.REMOVE_FILE, file })}
        />
      ))}
    </Stack>
  );
};
export default Upload;
