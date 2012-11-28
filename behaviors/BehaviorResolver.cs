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
using SharpKit.jQuery;
using guice.reflection;
using randori.styles;
using guice;

namespace randori.behaviors {

    [JsType(JsMode.Prototype)]
    public class BehaviorResolver {

        private StyleBehaviorMap map;
        private InjectionClassBuilder builder;

        private JsString getBehaviorEntryForElement(HtmlElement element) {
            JsString cssClassList = element.getAttribute("class");
            JsString behavior = null; 

            if (cssClassList != null) {
                JsArray cssClassArray = cssClassList.split(" ");
                for (int i = 0; i < cssClassArray.length; i++) {
                    JsString cssClass = cssClassArray[i].As<JsString>();
                    StyleBehaviorMapEntry behaviorMapEntry = map.getBehaviorEntry(cssClass);

                    if ( behaviorMapEntry != null ) {
                        behavior = behaviorMapEntry.getBehaviorClass("behavior");
                        if (behavior != null) {
                            break;
                        }
                    }
                }
            }

            return behavior;
        }

        public BehaviorContext resolveBehavior(HtmlElement element) {
            BehaviorContext context = null;
            var behaviorClass = element.getAttribute("data-behavior");

            if (behaviorClass != null) {
                //Clean up any of our properties embedded in the DOM
                element.removeAttribute("data-behavior");
            } else {
                behaviorClass = getBehaviorEntryForElement(element);
            }

            if ( behaviorClass != null ) {
                var behavior = builder.buildClass(behaviorClass, jQueryContext.J(element)).As<AbstractBehavior>();
                context = new BehaviorContext(behavior);
            }

            return context;
        }

        public BehaviorResolver(InjectionClassBuilder builder, StyleBehaviorMap map) {
            this.map = map;
            this.builder = builder;
        }
    }
}
