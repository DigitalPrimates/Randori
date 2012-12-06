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

namespace randori.styles {

    public class StyleExtensionMap  {

        public JsObject<StyleExtensionMapEntry> hashMap;

        public void addCSSEntry(JsString cssSelector, JsString extensionType, JsString extensionValue ) {
            StyleExtensionMapEntry attributes = hashMap[cssSelector];

            if (attributes == null) {
                attributes = new StyleExtensionMapEntry();
                hashMap[cssSelector] = attributes;
            }

            attributes.addExtensionType(extensionType, extensionValue);
        }

        public bool hasBehaviorEntry(JsString cssSelector) {
            return (hashMap[cssSelector] != null);
        }

        public StyleExtensionMapEntry getExtensionEntry(JsString cssSelector) {
            return hashMap[cssSelector];
        }

        public JsArray<JsString> getAllRandoriSelectorEntries() {
            var allEntries = new JsArray<JsString>();

            foreach (var cssSelector in hashMap) {
                allEntries.push(cssSelector);
            }

            return allEntries;
        }

        public StyleExtensionMap() {
            hashMap = new JsObject<StyleExtensionMapEntry>();
        }
    }
}