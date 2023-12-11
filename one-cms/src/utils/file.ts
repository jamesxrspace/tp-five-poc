import { UploadFile } from '@/api/openapi';

export const formatBytes = (bytes: number, decimals = 2) => {
  if (!+bytes) return '0 Bytes';

  const k = 1024;
  const dm = decimals < 0 ? 0 : decimals;
  const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'];

  const i = Math.floor(Math.log(bytes) / Math.log(k));

  return `${parseFloat((bytes / Math.pow(k, i)).toFixed(dm))} ${sizes[i]}`;
};

export const isImageFile = (file: File) => {
  return file.type.startsWith('image/');
};

export const isImageUrl = (url: string): Promise<boolean> =>
  new Promise((resolve) => {
    const img = new Image();

    img.src = url;
    img.onload = () => resolve(true);
    img.onerror = () => resolve(false);
  });

export const getFileModels = async (files: File[]) => {
  const models = await Promise.all(
    files.map(
      (file) =>
        new Promise<UploadFile>((resolve, reject) => {
          const reader = new FileReader();
          reader.onloadend = async () => {
            const arrayBuffer = reader.result as ArrayBuffer;
            if (arrayBuffer === null) {
              reject();
            }
            const buffer = await crypto.subtle.digest('SHA-256', arrayBuffer);
            const checksum = btoa(String.fromCharCode(...new Uint8Array(buffer)));

            resolve({
              fileId: file.name,
              contentType: file.type,
              contentLength: file.size,
              checksum,
            });
          };

          reader.onerror = reject;
          reader.readAsArrayBuffer(file);
        }),
    ),
  );
  return models;
};
