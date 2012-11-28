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

namespace randori.i18n {

    [JsType(Export = false)]
    public delegate void TranslationResult(JsString domain, JsArray<Translation> translations);

    [JsType(JsMode.Json)]
    public enum Domains { Labels, Messages, Reference };

    [JsType(Export=false,Name = "Object")]
    public class Translation {
        public JsString key;
        public dynamic value;
    }

    public abstract class AbstractTranslator {

        public TranslationResult translationResult;

        public abstract JsArray<Translation> synchronousTranslate(JsString domain, JsArray<JsString> keys);

        public abstract void translate( JsString domain, JsArray<JsString> keys );

        public AbstractTranslator() {
        }
    }
}