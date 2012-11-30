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
using guice;
using randori.behaviors;
using randori.dom;

namespace randori.template {
    public class TemplateBuilder {
        DomWalker domWalker;
        JsString templateAsString;
        InjectionClassBuilder classBuilder;

        //We need to both parse it and remove it... if we dont then any behaviors setup on the template
        //will be created on the older nodes by the DomWalker. That is a problem
        public HtmlElement parseAndReplaceTemplate(HtmlElement rootTemplateNode) {
            templateAsString = rootTemplateNode.outerHTML;
            jQuery replacementNode = jQueryContext.J( "<div></div>" );
            jQueryContext.J(rootTemplateNode).replaceWith(replacementNode);
            return replacementNode[ 0 ];
        }

        public HtmlElement replaceTemplate(HtmlElement existingNode, JsObject data, AbstractBehavior parentBehavior) {
            jQuery newNode = renderTemplateClone(data, parentBehavior);
            jQuery jExistingNode = jQueryContext.J(existingNode);

            jExistingNode.replaceWith(newNode);
            return newNode[0];
        }

        private JsString returnFieldName( JsString token ) {
            return token.substr( 1, token.length - 2 );
        }

        public jQuery renderTemplateClone(JsObject data, AbstractBehavior parentBehavior ) {
            JsString token;
            JsString field;
            dynamic dereferencedValue;
            JsRegExp keyRegex = new JsRegExp(@"\{[\w\W]+?\}", "g");
            JsRegExpResult foundKeys = templateAsString.match(keyRegex);
            JsString output = templateAsString;

            if (foundKeys != null) {
                for ( int j = 0; j < foundKeys.length; j++ ) {

                    token = foundKeys[ j ];
                    field = returnFieldName( token );

                    if (field.indexOf(".") != -1) {
                        dereferencedValue = resolveComplexName(data, field);
                    } else if (field != "*") {
                        dereferencedValue = data[field];
                    } else {
                        dereferencedValue = data;
                    }

                    output = output.replace(token, dereferencedValue);
                }
            }

            jQuery fragmentJquery = jQueryContext.J( output );

            Node fragment = fragmentJquery[0];
            domWalker.walkDomFragment(fragment, classBuilder, parentBehavior);

            return fragmentJquery;
        }

        private JsObject resolveComplexName(JsObject root, JsString name) {
            JsObject nextLevel = root;

            JsArray<JsString> path = name.split(".");
            for (int i = 0; i < path.length; i++) {
                nextLevel = nextLevel[path[i]].As<JsObject>();
                if (nextLevel.As<JsString>() == JsContext.undefined) {
                    return null;
                }
            }

            return nextLevel;
        }

        public TemplateBuilder( DomWalker domWalker, InjectionClassBuilder classBuilder ) {
            this.domWalker = domWalker;
            this.classBuilder = classBuilder;
        }
    }
}
