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

using System.Collections.Generic;
using SharpKit.Html;
using SharpKit.JavaScript;
using randori.data;

namespace randori.styles {

    /* Not able to make this work as I want just yet. Feature request filed
    [JsType(JsMode.Json)]
    public class StyleExtensionType {
        public static readonly string Module = "module";
        public static readonly string Mediator = "mediator";
        public static readonly string Behavior = "behavior";
        public static readonly string Content = "content";
        public static readonly string Formatter = "formatter";
        public static readonly string Validator = "validator";
    } */

    [JsType(Export = false)]
    public delegate void DataRetrievedDelegate( object o );

    public class StyleExtensionManager {

        readonly StyleExtensionMap map;

        private JsArray<HtmlElement> findChildNodesForSelector(JsArray<HtmlElement> elements, JsArray<JsString> selectorArray) {
            var selector = selectorArray.shift();

            //We need to actually abstract this so we can deal with IE and Opera returning a collection instead of a NodeList

            var newElements = new JsArray<HtmlElement>();

            if ( selector.substr( 0, 1 ) == "." ) {
                var className = selector.substring( 1 );
                //Lets assume this is a class selector
                while ( elements.length > 0 ) {
                    var element = elements.pop();
                    //Check this element first
                    if ( element.classList.contains( className ) ) {
                        newElements.push( element );
                    }

                    //now its descendants
                    var nodes = element.getElementsByClassName( className );
                    for ( var j=0; j<nodes.length; j++) {
                        newElements.push( nodes[ j ].As<HtmlElement>() );
                    }
                }

            } else {
                //invalid but going to assume type for now
                while (elements.length > 0) {
                    var element = elements.pop();
                    var nodes = element.getElementsByTagName(selector);
                    
                    for (var j = 0; j < nodes.length; j++) {
                        newElements.push( nodes[j].As<HtmlElement>() );
                    }
                }
            }

            //Only recurse if there is another selector
            if (selectorArray.length > 0) {
                newElements = findChildNodesForSelector(newElements, selectorArray);
            }

            return newElements;
        }

        private JsArray<HtmlElement> findChildNodesForCompoundSelector(HtmlElement element, JsString selector) {
            //lets start with simple ones
            var selectors = selector.split( " " );

            var ar = new JsArray<HtmlElement>();
            ar.push(element);
            var elements = findChildNodesForSelector( ar, selectors );

            return elements;
        }

        public HashMap<StyleExtensionMapEntry> getExtensionsForFragment(HtmlElement element) {

            var hashmap = new HashMap<StyleExtensionMapEntry>();
            //We need to loop over all of the relevant entries in the map that define some behavior
            var allEntries = map.getAllRandoriSelectorEntries();

            for ( var i=0; i<allEntries.length; i++) {
                JsArray<HtmlElement> implementingNodes = findChildNodesForCompoundSelector(element, allEntries[i]);

                //For each of those entries, we need to see if we have any elements in this DOM fragment that implement any of those classes
                for ( var j=0; j<implementingNodes.length; j++) {

                    var implementingElement = implementingNodes[ j ];
                    var value = hashmap.get( implementingElement );

                    if ( value == null ) {
                        //Get the needed entry
                        var extensionEntry = map.getExtensionEntry(allEntries[i]);

                        //give us a copy so we can screw with it at will
                        hashmap.put(implementingElement, extensionEntry.clone());
                    } else {
                        //We already have data for this node, so we need to merge the new data into the existing one
                        var extensionEntry = map.getExtensionEntry(allEntries[i]);

                        extensionEntry.mergeTo( (StyleExtensionMapEntry)value );
                    }
                }
            }

            //return the hashmap which can be queried and applied to the Dom
            return hashmap;
        }


        //TODO consider moving this somewhere more appropriate
        /*
        public StyleExtensionMapEntry getMergedEntryForElement(HtmlElement element) {
            JsString cssClassList = element.getAttribute("class");
            StyleExtensionMapEntry mergedEntry = null;

            if (cssClassList != null) {
                var cssClassArray = cssClassList.split(" ");
                for (int i = 0; i < cssClassArray.length; i++) {
                    var cssClass = cssClassArray[i].As<JsString>();
                    var extensionMapEntry = map.getExtensionEntry(cssClass);

                    if (extensionMapEntry != null) {
                        if (mergedEntry == null) {
                            //Waiting as long as we can to instantiate
                            mergedEntry = new StyleExtensionMapEntry();
                        }

                        extensionMapEntry.mergeTo(mergedEntry);
                    }
                }
            }

            return mergedEntry;
        }*/

        public bool parsingNeeded(HtmlLinkElement link) {
            return ( link.rel == "stylesheet/randori" );
        }

        JsString resetLinkAndReturnURL(HtmlLinkElement link) {

            //reset it and let the browser handle it now, we have the url we need
            //we will grab it next. So long as we are caching files, it will be retrieved synchronoulsy from the cache
            link.rel = "stylesheet";

            return link.href;
        }

        /*
         * 
         * We dont need this right now but keeping the code cause we might need it again someday soon
        JsString correctPaths(JsString sheet, JsString urlPrefix) {
            //First, fix the path of any objects in the system. This fines url references and preprends paths
            //Right now this just works for relative urls, I am going to need to fix it for absolute
            //Now using this one instead for optional quotes
            //(url\s?\(\s?['"]?)([\w\W]*?)(['"]?\s?\))

            var pathReplace = new JsRegExp("(url\\s?\\(\\s?[\'\"]?)([\\w\\W]*?)([\'\"]?\\s?\\))", "g");
            JsString replacementString = "$1" + urlPrefix + "/$2$3";
            sheet = sheet.replace(pathReplace, replacementString);
            return sheet;
        }*/

        void resolveSheet(JsString url) {
            var sheetRequest = new XMLHttpRequest();
            JsString behaviorSheet = "";
            JsString prefix;

            sheetRequest.open("GET", url, false);
            sheetRequest.send("");

            if (sheetRequest.status == 404) {
                throw new JsError("Cannot Find StyleSheet " + url);
            }

            int lastSlash = url.lastIndexOf("/");
            prefix = url.substring(0, lastSlash);

            parseAndPersistBehaviors(sheetRequest.responseText);
        }

        void parseAndPersistBehaviors(JsString sheet) {

            JsString classSelector;
            JsRegExpResult randoriVendorItemsResult;
            JsRegExpResult randoriVendorItemInfoResult;
            JsRegExpResult CSSClassSelectorNameResult;
            /*
             * This regular expression then grabs all of the class selectors
             * \.[\w\W]*?\}
             * 
             * This expression finds an -randori vendor prefix styles in the current cssSelector and returns 2 groups, the first
             * is the type, the second is the value
             * 
             * \s?-randori-([\w\W]+?)\s?:\s?["']?([\w\W]+?)["']?;
             * 
             */

            var allClassSelectors = new JsRegExp(@"^[\w\W]*?\}", "gm");

            const string RANDORI_VENDOR_ITEM_EXPRESSION = @"\s?-randori-([\w\W]+?)\s?:\s?[""']?([\w\W]+?)[""']?;";
            //These two are the same save for the global flag. The global flag seems to disable all capturing groups immediately
            var anyVendorItems = new JsRegExp(RANDORI_VENDOR_ITEM_EXPRESSION, "g");

            //This is the same as the one in findRelevant classes save for the the global flag... which is really important
            //The global flag seems to disable all capturing groups immediately
            var eachVendorItem = new JsRegExp(RANDORI_VENDOR_ITEM_EXPRESSION);

            var classSelectorName = new JsRegExp(@"^(.+?)\s+?{","m");
            JsString CSSClassSelectorName;
            JsString randoriVendorItemStr;

            var selectors = sheet.match(allClassSelectors);

            if (selectors != null ) {
                for (int i = 0; i < selectors.length; i++) {
                    classSelector = selectors[i];

                    randoriVendorItemsResult = classSelector.match(anyVendorItems);
                    if (randoriVendorItemsResult != null) {

                        CSSClassSelectorNameResult = classSelector.match(classSelectorName);
                        CSSClassSelectorName = CSSClassSelectorNameResult[1];

                        for (int j = 0; j < randoriVendorItemsResult.length; j++) {
                            randoriVendorItemStr = randoriVendorItemsResult[j];
                            randoriVendorItemInfoResult = randoriVendorItemStr.match(eachVendorItem);
                            map.addCSSEntry(CSSClassSelectorName, randoriVendorItemInfoResult[1], randoriVendorItemInfoResult[2]);
                            if (HtmlContext.window.console != null) {
                                HtmlContext.console.log(CSSClassSelectorName + " specifies a " + randoriVendorItemInfoResult[1] + " implemented by class " + randoriVendorItemInfoResult[2]);
                            }
                        }
                    }
                }
            }
        }

        public void parseAndReleaseLinkElement(HtmlLinkElement element) {
            resolveSheet(resetLinkAndReturnURL(element));
        }

        StyleExtensionManager(StyleExtensionMap map) {
            this.map = map;
        }
    }
}