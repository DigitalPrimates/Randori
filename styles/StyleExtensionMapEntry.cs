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

    public class StyleExtensionMapEntry  {

        public JsObject<JsString> hashMap;

        public void addExtensionType(JsString extensionType, JsString extensionValue ) {
            hashMap[extensionType] = extensionValue;
        }

        public bool hasExtensionType(JsString extensionType) {
            return (hashMap[extensionType] != null);
        }

        public JsString getExtensionValue(JsString extensionType) {
            return hashMap[extensionType];
        }

        public StyleExtensionMapEntry clone() {
            var newEntry = new StyleExtensionMapEntry();

            mergeTo( newEntry );

            return newEntry;
        }

        public void mergeTo( StyleExtensionMapEntry entry ) {
            foreach (var extensionType in hashMap) {
                entry.addExtensionType(extensionType, hashMap[extensionType]);
            }
        }

        public StyleExtensionMapEntry() {
            this.hashMap = new JsObject<JsString>();
        }
    }
}