﻿using CoAPnet.Protocol;
using CoAPnet.Protocol.Options;
using System;
using System.Collections.Generic;

namespace CoAPnet.Client
{
    public sealed class CoapRequestToMessageConverter
    {
        readonly CoapMessageOptionFactory _optionFactory = new CoapMessageOptionFactory();

        public CoapMessage Convert(CoapRequest request)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));

            var message = new CoapMessage
            {
                Type = CoapMessageType.Confirmable,
                Code = GetMessageCode(request.Method),
                Options = new List<CoapMessageOption>(),
                Payload = request.Payload
            };

            ApplyUriHost(request, message);
            ApplyUriPort(request, message);
            ApplyUriPath(request, message);
            ApplyUriQuery(request, message);

            return message;
        }

        void ApplyUriHost(CoapRequest request, CoapMessage message)
        {
            if (string.IsNullOrEmpty(request.Options.UriHost))
            {
                return;
            }

            message.Options.Add(_optionFactory.CreateUriHost(request.Options.UriHost));
        }

        void ApplyUriPort(CoapRequest request, CoapMessage message)
        {
            if (!request.Options.UriPort.HasValue)
            {
                return;
            }

            message.Options.Add(_optionFactory.CreateUriPort((uint)request.Options.UriPort.Value));
        }

        void ApplyUriPath(CoapRequest request, CoapMessage message)
        {
            if (string.IsNullOrEmpty(request.Options.UriPath))
            {
                return;
            }

            var paths = request.Options.UriPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var path in paths)
            {
                message.Options.Add(_optionFactory.CreateUriPath(path));
            }
        }

        void ApplyUriQuery(CoapRequest request, CoapMessage message)
        {
            if (request.Options.UriQuery == null)
            {
                return;
            }

            foreach (var query in request.Options.UriQuery)
            {
                message.Options.Add(_optionFactory.CreateUriQuery(query));
            }
        }

        static CoapMessageCode GetMessageCode(CoapRequestMethod method)
        {
            switch (method)
            {
                case CoapRequestMethod.Get: return CoapMessageCodes.Get;
                case CoapRequestMethod.Post: return CoapMessageCodes.Post;
                case CoapRequestMethod.Delete: return CoapMessageCodes.Delete;
                case CoapRequestMethod.Put: return CoapMessageCodes.Put;
                default: throw new NotSupportedException();
            }
        }
    }
}
