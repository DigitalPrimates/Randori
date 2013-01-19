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

using System;
using SharpKit.Html;
using SharpKit.JavaScript;
using randori.behaviors;

namespace randori.js {

    [JsType(JsMode.Prototype,Export = false)]
    public class SpecialType {
        public string className;
        public Action<dynamic> injectionPoints;
        public Func<JsArray> getClassDependencies;
    }

    [JsType(JsMode.Prototype, Export = false)]
    public class FutureBehavior {
        public Action verifyAndRegister;
        public Action<HtmlElement> provideDecoratedElement;
        public Action<string,object> injectPotentialNode;
    }

    [JsType(JsMode.Global)]
    public class GlobalUtilities {
        public static object Typeof(object val) {
            return val;
        }

        public static string dateToRFC3339String(JsDate date) {
            if (date != null ) {
                var jsonDate = date.toJSON();
                return jsonDate.substring(0, 10);
            }

            return null;
        }

        public static void decorateClassForInjection(SpecialType type, string className) {
            type.injectionPoints = defaultInjectionPoints;
            type.getClassDependencies = getClassDependencies;
            type.className = className;            
        }

        private static void defaultInjectionPoints(dynamic t) {
        }

        private static JsArray getClassDependencies() {
            return new JsArray();
        }

        public static void decorateClassAsBehavior(dynamic behavior) {
            FutureBehavior futureBehavior = behavior;

            futureBehavior.verifyAndRegister = verifyAndRegister;
            futureBehavior.provideDecoratedElement = provideDecoratedElement;
            futureBehavior.injectPotentialNode = injectPotentialNode;
        }

        private static void verifyAndRegister() {
            
        }

        private static void provideDecoratedElement( HtmlElement element ) {
            
        }

        private static void injectPotentialNode(string id, object node) {
        }
    }
}
