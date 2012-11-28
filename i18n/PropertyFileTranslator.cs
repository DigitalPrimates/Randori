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

namespace randori.i18n {

    [JsType(Export = false)]
    public delegate void TranslationsFileLoaded();

    public class PropertyFileTranslator : AbstractTranslator {
        readonly JsString url;

        readonly JsObject keyValuePairs;
        readonly bool forceReload;
        bool fileLoaded = false;

        override public JsArray<Translation> synchronousTranslate(JsString domain, JsArray<JsString> keys) {
            if (!fileLoaded) {
                makeSynchronousRequest( url );
            }

            return provideTranslations(domain, keys);
        }

        override public void translate(JsString domain, JsArray<JsString> keys) {
            if (!fileLoaded) {
                makeAsynchronousRequest(url, delegate() {
                    //The data is back, translate
                    JsArray<Translation> translations = provideTranslations(domain, keys);
                    if (translationResult != null ) {
                        translationResult(domain, translations);
                    }
                } );
            } else {
                //We already have the file, so just translate
                JsArray<Translation> translations = provideTranslations(domain, keys);
                if (translationResult != null) {
                    translationResult(domain, translations);
                } 
            }
        }

        private JsArray<Translation> provideTranslations(JsString domain, JsArray<JsString> keys) {
            JsArray<Translation> translations = new JsArray<Translation>();
            Translation translation;

            for (int i = 0; i < keys.length; i++) {
                translation = new Translation();
                translation.key = keys[i];
                translation.value = keyValuePairs[keys[i]];
                translations.push(translation);
            }

            return translations;
        }

        private void makeSynchronousRequest(JsString url) {
            XMLHttpRequest request = new XMLHttpRequest();

            if (forceReload) {
                //just bust the cache for now
                url = url + "?rnd=" + JsMath.random();
            }

            request.open("GET", url, false);
            request.send("");

            if (request.status == 404) {
                HtmlContext.alert("Required Content " + url + " cannot be loaded.");
                throw new JsError("Cannot continue, missing required property file " + url);
            }

            parseResult(request.responseText);
        }

        private void makeAsynchronousRequest(JsString url, TranslationsFileLoaded fileLoaded ) {
            XMLHttpRequest request = new XMLHttpRequest();

            if (forceReload) {
                //just bust the cache for now
                url = url + "?rnd=" + JsMath.random();
            }

            request.open("GET", url, true);
            request.onreadystatechange = delegate(DOMEvent evt) {
                if ( request.readyState == 4 && request.status == 200 ) {
                    parseResult( request.responseText );
                    fileLoaded();
                } else if ( request.readyState >= 3 && request.status == 404 ) {
                    HtmlContext.alert( "Required Content " + url + " cannot be loaded." );
                    throw new JsError( "Cannot continue, missing required property file " + url );
                }
            };

            request.send("");
        }

        void parseResult( JsString responseText ) {
            //get each line
            JsRegExp eachLine = new JsRegExp(@"[\w\W]+?[\n\r]+", "g");
            JsRegExpResult eachLineResult = responseText.match( eachLine );

            this.fileLoaded = true;

            if (eachLineResult != null) {
                for ( int i=0;i<eachLineResult.length;i++) {
                    parseLine( eachLineResult[ i ] );
                }
            }
        }

        void parseLine(JsString line) {

            if ( line.length == 0 ) {
                //empty line, bail
                return;
            }

            JsRegExp isComment = new JsRegExp(@"^[#!]");
            JsRegExpResult isCommentResult = line.match(isComment);

            if ( isCommentResult != null ) {
                //its a comment, bail
                return;
            }

            JsRegExp tokenize = new JsRegExp(@"^(\w+)\s?=\s?([\w\W]+?)[\n\r]+");
            JsRegExpResult tokenizeResult = line.match(tokenize);
            JsString key;
            JsString strValue;
            dynamic value;

            if ( tokenizeResult != null && tokenizeResult.length == 3) {
                key = tokenizeResult[ 1 ];
                value = tokenizeResult[ 2 ];

                strValue = value;
                if (strValue.indexOf( "," ) != -1 ) {
                    //this is an array, tokenize it
                    value = strValue.split( ',' );
                }

                keyValuePairs[ key ] = value;
            }
        }

        public PropertyFileTranslator(JsString url, bool forceReload) {
            this.url = url;
            this.forceReload = forceReload;
            keyValuePairs = new JsObject();
        }
    }
}