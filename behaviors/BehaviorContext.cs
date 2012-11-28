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

namespace randori.behaviors {
    public class BehaviorContext {
        public AbstractBehavior resolvedBehavior;
        readonly private JsObject<object> injectionMap;

        public void addBehavior(JsString id, AbstractBehavior behavior) {
            injectionMap[id] = behavior;
        }

        public void addNode(JsString id, jQuery node) {
            injectionMap[id] = node;
        }

        private JsObject<InjectionPoint> getBehaviorInjectionPoints() {
            dynamic jsInstance = resolvedBehavior;
            dynamic constructor = jsInstance.constructor;

            var map = new JsObject<InjectionPoint>();
            var typeDefinition = new TypeDefinition(constructor);

            JsArray<InjectionPoint> viewPoints = typeDefinition.getViewFields();

            for (int i = 0; i < viewPoints.length; i++) {
                map[viewPoints[i].n] = viewPoints[i];
            }

            return map;
        }

        private void injectViewDependencies() {
            JsObject behaviorHashMap;
            InjectionPoint viewPoint;
            JsObject<InjectionPoint> viewPointsMap;

            behaviorHashMap = resolvedBehavior.As<JsObject>();
            viewPointsMap = getBehaviorInjectionPoints();

            foreach (JsString id in viewPointsMap) {
                viewPoint = viewPointsMap[id];

                //This is a view point they asked to be injected
                if (viewPoint != null) {
                    if (injectionMap[id] != null) {
                        //we have a valid injectable, provide it
                        behaviorHashMap[id] = injectionMap[id];
                    } else if (viewPoint.r) {
                        //we dont have it, but it was required
                        dynamic constructor = behaviorHashMap["constructor"];
                        var typeDefinition = new TypeDefinition(constructor);

                        HtmlContext.alert(typeDefinition.getClassName() + " requires a [View] element with the id of " + id + " but it could not be found");
                        return;
                    }
                }
            }
        }

        public void setupBehavior() {
            injectViewDependencies();
            resolvedBehavior.onRegister();
        }

        public BehaviorContext(AbstractBehavior behavior) {
            this.resolvedBehavior = behavior;
            this.injectionMap = new JsObject<object>();
        }
    }
}
