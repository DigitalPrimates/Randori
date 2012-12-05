using SharpKit.JavaScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace randori.content
{
    [JsType(JsMode.Prototype, OmitCasts = true)]
    public class ContentCache
    {

        public static JsObject<JsString, JsString> htmlMergedFiles = new JsObject<JsString, JsString>();

        public JsArray<JsString> getCachedFileList()
        {
            JsArray<JsString> contentList = new JsArray<JsString>();
            foreach (JsString key in ContentCache.htmlMergedFiles)
            {
                contentList.Add(key);
            }

            return contentList;
        }

        public JsString getCachedHtmlForUri(JsString key)
        {
            if (ContentCache.htmlMergedFiles[key] != null)
            {
                return ContentCache.htmlMergedFiles[key];
            }
            return null;
        }
    }
}
