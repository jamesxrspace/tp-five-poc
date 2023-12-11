import { useMutation } from '@tanstack/react-query';
import { useAuth } from '../useAuth';
import { UploadFilesParams, uploadFiles } from '@/api/file';

export const useS3Upload = () => {
  const [state] = useAuth();
  const accessToken = state.context.accessToken;

  return useMutation({
    mutationFn: async ({ files, ...rest }: UploadFilesParams) =>
      uploadFiles({ files, accessToken, ...rest }),
  });
};
