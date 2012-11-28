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
using SharpKit.jQuery;

namespace randori.styles {

    public class StyleBehaviorMap  {

        public JsObject<StyleBehaviorMapEntry> hashMap;

        public void addBehaviorEntry(JsString cssClassName, JsString behaviorType, JsString className ) {
            StyleBehaviorMapEntry attributes = hashMap[cssClassName];

            if (attributes == null) {
                attributes = new StyleBehaviorMapEntry();
                hashMap[cssClassName] = attributes;
            }

            attributes.addBehaviorType(behaviorType, className);
        }

        public bool hasBehaviorEntry(JsString className) {
            return (hashMap[className] != null);
        }

        public StyleBehaviorMapEntry getBehaviorEntry(JsString cssClassName) {
            return hashMap[cssClassName];
        }

        public StyleBehaviorMap() {
            hashMap = new JsObject<StyleBehaviorMapEntry>();
        }
    }
}