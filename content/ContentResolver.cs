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
using randori.styles;

namespace randori.content {

    [JsType(JsMode.Prototype)]
    public class ContentResolver {

        private StyleBehaviorMap map;

        public void resolveContent(HtmlElement element) {
            var content = element.getAttribute("data-content");

            //Clean up any of our properties embedded in the DOM
            element.removeAttribute("data-content");

            if (content == null) {
                //content = map.getBehaviorClass("content");
            }

            //load the content
        }

        public ContentResolver(StyleBehaviorMap map) {
            this.map = map;
        }
    }
}
