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

namespace randori.async {

    [JsType(JsMode.Prototype, Export = false)]
    public delegate object OnFullfilledDelegate<T>(T result);

    [JsType(JsMode.Prototype, Export = false)]
    public delegate object OnRejectedDelegate(object reason);

    [JsType(JsMode.Prototype, Export = false)]
    public delegate void OnFullfilledNoReturnDelegate<T>(T result);

    [JsType(JsMode.Prototype, Export = false)]
    public delegate void OnRejectedNoReturnDelegate(object reason);
    enum PromiseState {
        Pending, Rejected, Fullfilled
    }

    [JsType(JsMode.Json, Export = false, Name = "Object")]
    class ThenContract<T,T1> {
        public OnFullfilledDelegate<T> fullfilledHandler;
        public OnRejectedDelegate rejectedHandler;
        public Promise<T1> promise;
    }

    public class Promise<T> {
        readonly JsArray<dynamic> thenContracts;
        PromiseState state = PromiseState.Pending;

        public T value;
        public object reason;

        private bool isFunction( dynamic obj ) {
            return !!(obj && obj.constructor && obj.call && obj.apply);
        }

        public Promise<object> then(OnFullfilledDelegate<T> onFulfilled = null, OnRejectedDelegate onRejected = null) {
            return then<object>( onFulfilled, onRejected );
        }

        //3.2.1 Both onFulfilled and onRejected are optional arguments
        public Promise<T1> then<T1>(OnFullfilledDelegate<T> onFulfilled = null, OnRejectedDelegate onRejected = null) {
            var promise = new Promise<T1>();

            //3.2.1.1
            if (!isFunction(onFulfilled)) {
                onFulfilled = null;
            }

            //3.2.1.2
            if (!isFunction(onRejected)) {
                onRejected = null;
            }

            //3.2.5
            var thenContract = new ThenContract<T,T1>{fullfilledHandler = onFulfilled, rejectedHandler = onRejected, promise = promise};
            thenContracts.push( thenContract );

            if (state == PromiseState.Fullfilled) {
                //3.2.4
                HtmlContext.setTimeout(delegate {
                    fullfill(value);
                }, 1);
            } else if (state == PromiseState.Rejected) {
                //3.2.4
                HtmlContext.setTimeout(delegate {
                    internalReject(reason);
                }, 1);
            }

            //3.2.6
            return promise;
        }

        public void resolve(T response) {
            //3.2.2 & 3.2.2.3
            if (state == PromiseState.Pending) {

                //3.1.2.2
                this.value = response;

                fullfill(response);                    
            }
        }

        private void fullfill(T response) {

            //3.1.1.1
            state = PromiseState.Fullfilled;

            //3.2.2.2
            while (thenContracts.length > 0) {
                var thenContract = thenContracts.shift();

                if (thenContract.fullfilledHandler != null) {

                    try {
                        //3.2.2.1 & 3.2.5.1
                        dynamic callBackResult = thenContract.fullfilledHandler( response );

                        if ( callBackResult && callBackResult.then != null) {
                            //3.2.6.3
                            Promise<T> returnedPromise = callBackResult;
                            returnedPromise.then(
                                delegate(T innerResponse) {
                                    //3.2.6.3.2
                                    thenContract.promise.resolve(innerResponse);
                                    return null;
                                },
                                delegate(object innerReason) {
                                    //3.2.6.3.3
                                    thenContract.promise.reject(innerReason);
                                    return null;
                                }
                             );
                        } else {
                            //3.2.6.1
                            thenContract.promise.resolve( callBackResult );
                        } 
                    } catch (JsError error ) {
                        //3.2.6.2
                        thenContract.promise.reject( error );
                    }
                } else {
                    //3.2.6.4
                    thenContract.promise.resolve(response);
                }
            }
        }

        public void reject(object reason) {

            //3.2.3.3
            if (state == PromiseState.Pending) {

                //3.1.3.2
                this.reason = reason;

                internalReject(reason);
            }
        }

        private void internalReject( object reason ) {
            //3.1.1.1
            state = PromiseState.Rejected;

            //3.2.3.2
            while (thenContracts.length > 0) {
                var thenContract = thenContracts.shift();

                if (thenContract.rejectedHandler != null) {
                    try {

                        //3.2.3.1 & 3.2.5.2
                        dynamic callBackResult = thenContract.rejectedHandler(reason);

                        if (callBackResult && callBackResult.then != null) {
                            //3.2.6.3
                            Promise<T> returnedPromise = callBackResult.As<T>();
                            returnedPromise.then(
                                delegate(T innerResponse) {
                                    //3.2.6.3.2
                                    thenContract.promise.resolve(innerResponse);
                                    return null;
                                },
                                delegate(object innerReason) {
                                    //3.2.6.3.3
                                    thenContract.promise.reject(innerReason);
                                    return null;
                                }
                              );
                        } else {
                            //3.2.6.1
                            thenContract.promise.resolve(callBackResult);
                        } 
                    } catch (JsError error) {
                        //3.2.6.2
                        thenContract.promise.reject(error);
                    }
                } else {
                    //3.2.6.5
                    thenContract.promise.reject(reason);
                }
            }
        }

        public Promise() {
            this.thenContracts = new JsArray<dynamic>();
        }
    }
}