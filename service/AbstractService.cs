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

namespace randori.service {
    public abstract class AbstractService {
        readonly protected XMLHttpRequest xmlHttpRequest;

        protected string createUri( string protocol, string host, string port, string path ) {
            var uri = protocol + "://" + host;
            if ( port != null ) {
                uri = uri + ":" + port;
            }
            uri = uri + "/" + path;
            return uri;
        }

        protected AbstractToken sendRequest(string verb, string protocol, string host, string port, string path) {

            var serviceToken = new ServiceToken();

            xmlHttpRequest.open(verb, createUri(protocol, host, port, path), true );
            xmlHttpRequest.onreadystatechange += serviceToken.onReadyStateChange;

            xmlHttpRequest.send("");

            return serviceToken;
        }

        protected AbstractService( XMLHttpRequest xmlHttpRequest) {
            this.xmlHttpRequest = xmlHttpRequest;
        }
    }
}
