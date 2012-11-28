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

    public class StyleBehaviorMapEntry  {

        public JsObject<JsString> hashMap;
        public JsArray<JsString> behaviorTypes;

        public void addBehaviorType(JsString behaviorType, JsString className ) {
            hashMap[behaviorType] = className;
            behaviorTypes.push(behaviorType);
        }

        public bool hasBehaviorType(JsString behaviorType) {
            return (hashMap[behaviorType] != null);
        }

        public JsString getBehaviorClass(JsString behaviorType) {
            return hashMap[behaviorType];
        }

        public StyleBehaviorMapEntry clone() {
            var newEntry = new StyleBehaviorMapEntry();

            for (int i = 0; i < behaviorTypes.length; i++ ) {
                newEntry.addBehaviorType(behaviorTypes[i], hashMap[behaviorTypes[i]]);
            }

            return newEntry;
        }

        public void merge( StyleBehaviorMapEntry entry ) {
            for (int i = 0; i < entry.behaviorTypes.length; i++) {
                this.addBehaviorType(entry.behaviorTypes[i], entry.hashMap[entry.behaviorTypes[i]]);
            }
        }

        public StyleBehaviorMapEntry() {
            this.hashMap = new JsObject<JsString>();
            this.behaviorTypes = new JsArray<JsString>();
        }
    }
}