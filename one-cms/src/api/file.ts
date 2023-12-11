import { GAME_SERVER_URL } from './../constants/environment';
import { AssetApi, Configuration, CreateUploadRequest } from './openapi';
import { toast } from '@/constants/toast';
import i18n from '@/i18n';
import { getFileModels } from '@/utils/file';

export interface UploadFilesParams extends Omit<CreateUploadRequest, 'files'> {
  accessToken?: string;
  files: File[];
  onError?: () => void;
}
export const uploadFiles = async ({ accessToken, files, onError, ...rest }: UploadFilesParams) => {
  if (!accessToken || !files.length) {
    return Promise.reject();
  }

  const assetApi = new AssetApi(
    new Configuration({
      basePath: GAME_SERVER_URL,
      accessToken,
    }),
  );

  try {
    const fileModels = await getFileModels(files);
    const createRes = await assetApi.createUploadRequest({
      createUploadRequest: { ...rest, files: fileModels },
    });
    if (!createRes.data?.presignedUrls || !createRes.data?.requestId) {
      throw Error(createRes.message);
    }

    const uploadFileRes = await Promise.all(
      Object.entries(createRes.data.presignedUrls).map(([key, url]) => {
        const file = files.find((file) => file.name === key);
        if (!file) return Promise.reject();

        return fetch(url, {
          method: 'PUT',
          body: file,
        });
      }),
    );

    if (uploadFileRes.some((res) => !res.ok)) {
      throw Error('Upload failed');
    }
    const confirmRes = await assetApi.confirmUploaded({
      requestId: createRes.data.requestId,
    });
    if (!confirmRes.data) {
      throw Error(confirmRes.message);
    }

    return confirmRes;
  } catch {
    onError
      ? onError()
      : toast({
          description: i18n.t('upload.errorMessage'),
          status: 'error',
        });
  }
};
