import { DoneInvokeEvent } from 'xstate';
import { ConfirmUploadedResponse } from '@/api/openapi';
import { UploadEvent } from '@/machines/upload';

export type AddPendingFilesEvent = { type: UploadEvent.ADD_FILES; files: File[] };
export type RemoveFileEvent = { type: UploadEvent.REMOVE_FILE; file: string | File };
export type UploadFilesEvent = { type: UploadEvent.UPLOAD };
export type DoneUploadFilesEvent = DoneInvokeEvent<ConfirmUploadedResponse>;
export type ResetUploadedFilesEvent = {
  type: UploadEvent.RESET_UPLOADED_FILES;
  uploadedFileUrls: string[];
};
export type UploadEventType =
  | AddPendingFilesEvent
  | RemoveFileEvent
  | UploadFilesEvent
  | DoneUploadFilesEvent
  | ResetUploadedFilesEvent;
