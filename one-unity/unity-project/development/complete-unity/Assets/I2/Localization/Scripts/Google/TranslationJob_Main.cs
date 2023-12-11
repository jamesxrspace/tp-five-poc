using System.Collections.Generic;

namespace I2.Loc
{
    using TranslationDictionary = Dictionary<string, TranslationQuery>;

    public class TranslationJob_Main : TranslationJob
    {
#if ENABLE_I2_LOCALIZATION_INSECURE_PROTOCOL
        TranslationJob_WEB mWeb; 
#endif
        TranslationJob_POST mPost;
        TranslationJob_GET mGet;

        TranslationDictionary _requests;
        GoogleTranslation.fnOnTranslationReady _OnTranslationReady;
        public string mErrorMessage;

        public TranslationJob_Main(TranslationDictionary requests, GoogleTranslation.fnOnTranslationReady OnTranslationReady)
        {
            _requests = requests;
            _OnTranslationReady = OnTranslationReady;

#if ENABLE_I2_LOCALIZATION_INSECURE_PROTOCOL
            //mWeb = new TranslationJob_WEB(requests, OnTranslationReady);  
#endif
            mPost = new TranslationJob_POST(requests, OnTranslationReady);
        }

        public override eJobState GetState()
        {
#if ENABLE_I2_LOCALIZATION_INSECURE_PROTOCOL
            if (mWeb != null)
            {
                var state = mWeb.GetState();
                switch (state)
                {
                    case eJobState.Running: return eJobState.Running;
                    case eJobState.Succeeded:
                        {
                            mJobState = eJobState.Succeeded;
                            break;
                        }
                    case eJobState.Failed:
                        {
                            mWeb.Dispose();
                            mWeb = null;
                            mPost = new TranslationJob_POST(_requests, _OnTranslationReady);
                            break;
                        }
                }
            } 
#endif
            if (mPost != null)
            {
                var state = mPost.GetState();
                switch (state)
                {
                    case eJobState.Running: return eJobState.Running;
                    case eJobState.Succeeded:
                        {
                            mJobState = eJobState.Succeeded;
                            break;
                        }
                    case eJobState.Failed:
                        {
                            mPost.Dispose();
                            mPost = null;
                            mGet = new TranslationJob_GET(_requests, _OnTranslationReady);
                            break;
                        }
                }
            }
            if (mGet != null)
            {
                var state = mGet.GetState();
                switch (state)
                {
                    case eJobState.Running: return eJobState.Running;
                    case eJobState.Succeeded:
                        {
                            mJobState = eJobState.Succeeded;
                            break;
                        }
                    case eJobState.Failed:
                        {
                            mErrorMessage = mGet.mErrorMessage;
                            if (_OnTranslationReady != null)
                                _OnTranslationReady(_requests, mErrorMessage);
                            mGet.Dispose();
                            mGet = null;
                            break;
                        }
                }
            }

            return mJobState;
        }

        public override void Dispose()
        {
            if (mPost != null) mPost.Dispose();
            if (mGet != null) mGet.Dispose();
            mPost = null;
            mGet = null;
        }
    }
}