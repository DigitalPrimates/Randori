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

        public static JsDate RFC3339StringToLocalDate(JsString dateString) {
            JsDate localDate = null;

            if (dateString == null || dateString.length != 10 ) {
                throw new JsError("Invalid Date String");
            }

            var dateMatch = new JsRegExp(@"(\d\d\d\d)-(\d\d)-(\d\d)");
            var match = dateString.match(dateMatch);

            if (match != null) {
                dynamic yearString = match[1];
                dynamic monthString = match[2];
                dynamic dayString = match[3];

                localDate = new JsDate(yearString, monthString-1, dayString);
            }

            return localDate;
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

        public static void shallowMerge(JsObject src, JsObject dst) {
            if (src is JsObject && dst is JsObject) {
                foreach (var key in src) {
                    if (src.hasOwnProperty(key)) {
                        dst[key] = src[key];
                    }
                }
            }
        }

        public static object clone(object obj) {
            if ( ( obj == null) || ( JsContext.@typeof( obj ) != "object" ) ) {
                return obj;
            }

            //Dates
            if ( obj is JsDate ) {
                var copy = new JsDate();
                copy.setTime( obj.As<JsDate>().getTime() );
                return copy;
            }

            //Array
            if (obj is JsArray) {
                var copy = new JsArray();
                var current = obj.As<JsArray>();

                for (var i = 0; i < current.length; i++ ) {
                    copy[ i ] = clone( current[ i ] );
                }

                return copy;
            }

            //Object
            if (obj is JsObject) {
                var copy = new JsObject();
                var current = obj.As<JsObject>();

                foreach (var key in current ) {
                    if ( current.hasOwnProperty( key )) {
                        copy[ key ] = clone( current[ key ] );
                    }
                }

                return copy;
            }

            return null;
        }
    }
}
