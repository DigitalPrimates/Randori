/***
 * Copyright 2012 LTN Consulting, Inc. /dba Digital Primates®
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * 
 * @author Michael Labriola <labriola@digitalprimates.net>
 */
using SharpKit.JavaScript;

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
