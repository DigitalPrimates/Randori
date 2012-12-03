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

using SharpKit.Html;
using SharpKit.JavaScript;

namespace randori.content {

    public class ContentLoader {
        readonly ContentCache contentCache;

        public JsString synchronousLoad(JsString fragmentURL) {
            //We need to check to see if we already have this content. If we do not, then we need to load it now and insert it into the DOM

            var cachedContent = contentCache.getCachedHtmlForUri(fragmentURL);
            if (cachedContent != null) {
                return cachedContent;
            }

            //Else load it now
            var request = new XMLHttpRequest();
            request.open("GET", fragmentURL, false);
            request.send("");

            if (request.status == 404) {
                throw new JsError("Cannot continue, missing required content " + fragmentURL);
            }

            return request.responseText;
        }

        public void asynchronousLoad() {

        }

        public ContentLoader( ContentCache contentCache ) {
            this.contentCache = contentCache;
        }
    }
}
