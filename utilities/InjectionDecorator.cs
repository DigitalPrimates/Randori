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
using SharpKit.JavaScript;

namespace randori.utilities
{

    [JsType(JsMode.Prototype, Export = false)]
    public class InjectableType
    {
        public string className;
        public Action<dynamic> injectionPoints;
        public Func<JsArray> getClassDependencies;
    }

    public class InjectionDecorator
    {

        /* Decorates an arbitrary object to allow it to be injected by guice */
        public void decorateObject(dynamic dependency, string className)
        {
            InjectableType injectableType = dependency;

            injectableType.injectionPoints = defaultInjectionPoints;
            injectableType.getClassDependencies = getClassDependencies;
            injectableType.className = className;
        }

        static void defaultInjectionPoints(dynamic t)
        {
        }

        static JsArray getClassDependencies()
        {
            return new JsArray();
        }

        public InjectionDecorator()
        {
        }
    }
}
