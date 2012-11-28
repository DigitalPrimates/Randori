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
using randori.attributes;

namespace randori.styles {
    [JsType(Export = false)]
    public delegate void DataRetrievedDelegate( object o );

    public class StyleBehaviorManager  {

        private StyleBehaviorMap map;

        JsArray<JsString> findURLs( jQuery links ) {
            HtmlLinkElement link;
            var urls = new JsArray<JsString>(); ;

            if (links != null) {
                for (var i = 0; i < links.length; i++) {
                    link = links[i].As<HtmlLinkElement>();
                    if (link.rel == "stylesheet/randori") {
                        //reset it and let the browser handle it, we have the url we need
                        //we will grab it next
                        link.rel = "stylesheet";
                        urls.push(link.href);
                    }
                }
            }

            return urls;
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

        void loadSheets(JsArray<JsString> urls) {
            var sheetRequest = new XMLHttpRequest();
            JsString behaviorSheet = "";
            JsString url;
            JsString prefix;

            for (int i = 0; i < urls.length; i++) {
                url = urls[i];
                sheetRequest.open("GET", url, false);
                sheetRequest.send("");

                if (sheetRequest.status == 404) {
                    throw new JsError("Cannot Find StyleSheet " + url);
                }

                int lastSlash = url.lastIndexOf("/");
                prefix = url.substring(0, lastSlash);

                parseAndPersistBehaviors(sheetRequest.responseText);
            }
        }

        void parseAndPersistBehaviors(JsString sheet) {

            JsString classSelector;
            JsRegExpResult dpVendorItemsResult;
            JsRegExpResult dpVendorItemInfoResult;
            JsRegExpResult CSSClassSelectorNameResult;
            /*
             * This regular expression then grabs all of the class selectors
             * \.[\w\W]*?\}
             * 
             * This expression finds an -randori vendor prefix styles in the current selector and returns 2 groups, the first
             * is the type, the second is the value
             * 
             * \s?-randori-([\w\W]+?)\s?:\s?["']?([\w\W]+?)["']?;
             * 
             */

            var allClassSelectors = new JsRegExp("\\.[\\w\\W]*?\\}", "g");

            //These two are the same save for the global flag. The global flag seems to disable all capturing groups immediately
            var dpVendorItems = new JsRegExp(@"\s?-randori-([\w\W]+?)\s?:\s?[""']?([\w\W]+?)[""']?;", "g");

            //This is the same as the one in findRelevant classes save for the the global flag... which is really important
            //The global flag seems to disable all capturing groups immediately
            var dpVendorItemsDetail = new JsRegExp(@"\s?-randori-([\w\W]+?)\s?:\s?[""']?([\w\W]+?)[""']?;");

            var classSelectorName = new JsRegExp(@"\.([\W\w]+?)\s+?{");
            JsString CSSClassSelectorName;
            JsString dpVendorItemStr;

            var selectors = sheet.match(allClassSelectors);

            if (selectors != null ) {
                for (int i = 0; i < selectors.length; i++) {
                    classSelector = selectors[i];

                    dpVendorItemsResult = classSelector.match(dpVendorItems);
                    if (dpVendorItemsResult != null) {

                        CSSClassSelectorNameResult = classSelector.match(classSelectorName);
                        CSSClassSelectorName = CSSClassSelectorNameResult[1];

                        for (int j = 0; j < dpVendorItemsResult.length; j++) {
                            dpVendorItemStr = dpVendorItemsResult[j];
                            dpVendorItemInfoResult = dpVendorItemStr.match(dpVendorItemsDetail);
                            map.addBehaviorEntry(CSSClassSelectorName, dpVendorItemInfoResult[1], dpVendorItemInfoResult[2]);
                            HtmlContext.console.log(CSSClassSelectorName + " specifies a " + dpVendorItemInfoResult[1] + " implemented by class " + dpVendorItemInfoResult[2]);
                        }
                    }
                }
            }
        }

        public void parseAndReleaseNodes(jQuery links) {
            var urls = findURLs(links);
            loadSheets(urls);
        }

        StyleBehaviorManager(StyleBehaviorMap map) {
            this.map = map;
        }
    }
}