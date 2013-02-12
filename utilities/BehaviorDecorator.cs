/***
 * Copyright 2013 LTN Consulting, Inc. /dba Digital Primates®
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

using System;
using SharpKit.Html;
using SharpKit.JavaScript;

namespace randori.utilities {
    [JsType(JsMode.Prototype, Export = false)]
    public class FutureBehavior
    {
        public Action verifyAndRegister;
        public Action removeAndCleanup;
        public Action<HtmlElement> provideDecoratedElement;
        public Action<string, object> injectPotentialNode;
    }

    public class BehaviorDecorator {

        /* Decorates an arbitrary object to create a behavior */
        public void decorateObject(dynamic behavior) {
            FutureBehavior futureBehavior = behavior;

            futureBehavior.verifyAndRegister = verifyAndRegister;
            futureBehavior.provideDecoratedElement = provideDecoratedElement;
            futureBehavior.injectPotentialNode = injectPotentialNode;
            futureBehavior.removeAndCleanup = removeAndCleanup;
        }

        private static void verifyAndRegister() {
        }

        private static void removeAndCleanup() {
        }

        private static void provideDecoratedElement(HtmlElement element) {

        }

        private static void injectPotentialNode(string id, object node) {
        }

        public BehaviorDecorator() {
        }
    }
}

