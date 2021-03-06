﻿namespace CoAPnet.Client
{
    public class CoapObserveOptions
    {
        public CoapObserveRequest Request { get; set; }

        public ICoapResponseHandler ResponseHandler { get; set; }
    }

    public class CoapObserveRequest
    {
        public CoapRequestOptions Options { get; set; } = new CoapRequestOptions();
    }
}
