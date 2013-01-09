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
using randori.async;

namespace randori.service {
    [JsType(JsMode.Prototype, OmitCasts = true, NativeOverloads = false)]
    public abstract class AbstractService {
        readonly protected XMLHttpRequest xmlHttpRequest;

        protected string createUri( string protocol, string host, string port, string path ) {
            var uri = "";
            
            if ( ( protocol != null ) && ( host != null ) ) {
                uri += ( protocol + "://" + host );    
            }
            
            if ( port != null ) {
                uri = uri + ":" + port;
            }

            uri = uri + "/" + path;
            return uri;
        }

        protected virtual void modifyHeaders( XMLHttpRequest request ) {
            
        }

        protected virtual Promise<object> sendRequest( string verb, string uri ) {
            var promise = new Promise<object>();

            xmlHttpRequest.open(verb, uri, true);
            xmlHttpRequest.onreadystatechange += delegate(DOMEvent evt) {
                var request = evt.target.As<XMLHttpRequest>();

                if (request.readyState == XMLHttpRequest.DONE) {
                    if (request.status == 200) {
                        promise.resolve(request.responseText);
                    } else {
                        promise.reject(request.statusText);
                    }
                }
            };

            modifyHeaders(xmlHttpRequest);

            xmlHttpRequest.send("");

            return promise;
        }

        protected virtual Promise<object> sendRequest(string verb, string protocol, string host, string port, string path) {
            return sendRequest( verb, createUri( protocol, host, port, path ) );
        }

        protected AbstractService( XMLHttpRequest xmlHttpRequest) {
            this.xmlHttpRequest = xmlHttpRequest;
        }
    }
}
