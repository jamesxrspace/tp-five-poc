import { DropzoneOptions } from 'react-dropzone';
import { ActorRefFrom } from 'xstate';
import { uploadMachine } from '@/machines/upload';

export interface UploadProps
  extends Pick<DropzoneOptions, 'accept' | 'minSize' | 'maxSize' | 'maxFiles'> {
  service: ActorRefFrom<typeof uploadMachine>;
}
