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
using randori.timer;

namespace randori.i18n {
    [JsType(JsMode.Prototype)]
    public class LocalizationProvider {

        AbstractTranslator translator;
        JsObject<JsArray<Node>> pendingTranslations;
        Timer timer;

        public JsArray<Translation> translateKeysSynchronously(JsString domain, JsArray<JsString> keys) {
            return translator.synchronousTranslate( domain, keys );
        }

        public void translateNode(Node textNode, JsRegExpResult result) {
            //We have a list of matches within the text node which tell us what we need to get
            //further we have the original node, which we need to preserve as there may be only parts getting translated
            //even though this is bad form <div>[labels.monkey] is a type of [labels.animal]</div> we still need to support it
            for ( int i=0; i<result.length;i++) {
                requestTranslation( result[ i ], textNode );
            }

            scheduleTranslation();
        }

        private void requestTranslation(JsString expression, Node textNode) {
            JsArray<Node> pendingTranslation = pendingTranslations[expression];

            if (pendingTranslation == null) {
                pendingTranslation = new JsArray<Node>();
                pendingTranslations[expression] = pendingTranslation;
            }

            pendingTranslation.push(textNode);
        }
        
        private void scheduleTranslation() {
            //We want to batch a page or so at a time, we instead of sending requests 
            //immediately we defer until 10ms pass where we don't have a request
            timer.reset();
            timer.start();
        }

        private void sendTranslationRequest(Timer timer) {
            JsObject<JsArray<JsString>> domainLabels = new JsObject<JsArray<JsString>>();

            JsRegExp keyValuePair = new JsRegExp(@"\[(labels|messages|reference)\.(\w+)\]");
            JsRegExpResult result;

            JsString domain;
            JsString key;

            //The translation translator works on domains, so we need to break up the available items we have and make appropriate requests to the translator
            foreach (JsString expression in pendingTranslations) {
                result = expression.match(keyValuePair);
                domain = result[1];
                key = result[ 2 ];

                if (domainLabels[domain] == null ) {
                    domainLabels[domain] = new JsArray<JsString>();
                }

                domainLabels[domain].push( key );
            }

            foreach ( JsString domainEntry in domainLabels ) {
                translator.translate(domainEntry, domainLabels[domainEntry]);
            }
        }

        private void provideTranslation( JsString domain, JsArray<Translation> translations) {
            JsString expression;
            JsArray<Node> nodes;

            for ( int i=translations.length-1; i>=0;i--) {
                expression = "[" + domain + "." + translations[ i ].key + "]";

                nodes = pendingTranslations[expression];

                if ( nodes != null ) {
                    for (int j = 0; j < nodes.length; j++) {
                        applyTranslation(nodes[j], expression, translations[i].value);
                    }
                }

                JsContext.delete( pendingTranslations[ expression ] );
            }
        }

        private void applyTranslation(Node node, JsString expression, JsString translation) {
            JsString currentValue = node.nodeValue;
            JsString newValue = currentValue.replace(expression, translation);
            node.nodeValue = newValue;
        }

        public LocalizationProvider(AbstractTranslator translator ) {
            this.translator = translator;
            translator.translationResult += provideTranslation;

            timer = new Timer( 10, 1 );
            timer.timerComplete += sendTranslationRequest;

            pendingTranslations = new JsObject<JsArray<Node>>();
        }
    }
}