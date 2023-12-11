import { Authing } from '@authing/web';
import { AuthingSPAInitOptions } from '@authing/web/dist/typings/src/global';
import { IS_LOCAL_ENV } from './environment';

const authingOptions: AuthingSPAInitOptions = {
  domain: process.env.REACT_APP_AUTHING_DOMAIN || '',
  userPoolId: process.env.REACT_APP_AUTHING_USER_POOL_ID || '',
  appId: process.env.REACT_APP_AUTHING_APP_ID || '',
  redirectUri: process.env.REACT_APP_AUTHING_REDIRECT_URI || '',
  redirectResponseMode: 'query',
  logoutRedirectUri: process.env.REACT_APP_AUTHING_LOGOUT_REDIRECT_URI || '',
  scope: 'openid offline_access username profile email',
};

const initAuthing = () => {
  if (IS_LOCAL_ENV) {
    return null;
  }

  try {
    const authing = new Authing(authingOptions);
    return authing;
  } catch (error) {
    return null;
  }
};
export const authing = initAuthing();
