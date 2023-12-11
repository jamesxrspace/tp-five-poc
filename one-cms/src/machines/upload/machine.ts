import * as R from 'ramda';
import { createMachine, assign } from 'xstate';
import { authService } from '../auth';
import {
  AddPendingFilesEvent,
  RemoveFileEvent,
  DoneUploadFilesEvent,
  UploadEventType,
  ResetUploadedFilesEvent,
} from './type';
import { uploadFiles } from '@/api/file';
import { S3Object } from '@/api/openapi';

enum S {
  IDLE = 'IDLE',
  UPLOADING = 'UPLOADING',
}

enum E {
  ADD_FILES = 'ADD_FILES',
  UPLOAD = 'UPLOAD',
  REMOVE_FILE = 'REMOVE_FILE',
  RESET_UPLOADED_FILES = 'RESET_UPLOADED_FILES',
}

type UploadContext = {
  uploadedFileUrls: string[];
  pendingFiles: File[];
};

const uploadMachine = createMachine<UploadContext, UploadEventType>(
  {
    predictableActionArguments: true,
    initial: S.IDLE,
    context: {
      uploadedFileUrls: [],
      pendingFiles: [],
    },
    states: {
      [S.IDLE]: {
        on: {
          [E.ADD_FILES]: {
            actions: ['assignPendingFiles'],
          },
          [E.UPLOAD]: {
            target: S.UPLOADING,
          },
          [E.REMOVE_FILE]: {
            actions: ['removeFile'],
          },
          [E.RESET_UPLOADED_FILES]: {
            actions: ['resetUploadedFileUrls'],
          },
        },
      },
      [S.UPLOADING]: {
        invoke: {
          src: 'uploadPendingFiles',
          onDone: {
            target: S.IDLE,
            actions: ['updateUploadedFiles', 'resetPendingFiles', 'onUploaded'],
          },
        },
      },
    },
  },
  {
    actions: {
      assignPendingFiles: assign({
        pendingFiles: (_, event: AddPendingFilesEvent) => event.files,
      }),
      removeFile: assign(({ pendingFiles, uploadedFileUrls }, { file }: RemoveFileEvent) => {
        if (typeof file === 'string') {
          return {
            uploadedFileUrls: uploadedFileUrls.filter((file) => file !== file),
          };
        }
        return {
          pendingFiles: pendingFiles.filter((file) => file.name !== file.name),
        };
      }),
      updateUploadedFiles: assign({
        uploadedFileUrls: ({ uploadedFileUrls }, { data }: DoneUploadFilesEvent) => {
          const urls: string[] = R.pipe(
            R.propOr({}, 'data'),
            R.values,
            R.map<S3Object, 'url'>(R.propOr('', 'url')),
          )(data);
          return uploadedFileUrls.concat(urls);
        },
      }),
      resetPendingFiles: assign({
        pendingFiles: [],
      }),
      resetUploadedFileUrls: assign({
        uploadedFileUrls: (_, event: ResetUploadedFilesEvent) => event.uploadedFileUrls,
      }),
    },
    services: {
      uploadPendingFiles: ({ pendingFiles }) => {
        const accessToken = authService.getSnapshot().context.accessToken;
        return uploadFiles({
          files: pendingFiles,
          accessToken: accessToken,
        });
      },
    },
  },
);
export { uploadMachine, S as UploadState, E as UploadEvent };
