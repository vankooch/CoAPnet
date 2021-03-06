﻿using CoAPnet.Transport;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.Security;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoAPnet.Extensions.DTLS
{
    public sealed class DtlsCoapTransportLayer : ICoapTransportLayer
    {
        UdpTransport _udpTransport;
        DtlsTransport _dtlsTransport;

        public IDtlsCredentials Credentials
        {
            get; set;
        }

        public DtlsVersion DtlsVersion
        {
            get; set;
        } = DtlsVersion.V1_2;

        public Task ConnectAsync(CoapTransportLayerConnectOptions connectOptions, CancellationToken cancellationToken)
        {
            _udpTransport = new UdpTransport(connectOptions);

            var clientProtocol = new DtlsClientProtocol(new SecureRandom());
            var client = new DtlsClient(ConvertProtocolVersion(DtlsVersion), (PreSharedKey)Credentials);
            _dtlsTransport = clientProtocol.Connect(client, _udpTransport);

            return Task.FromResult(0);
        }

        public Task<int> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            int received;
            do
            {
                received = _dtlsTransport.Receive(buffer.Array, buffer.Offset, buffer.Count, 100);
            }
            while (received == 0 && !cancellationToken.IsCancellationRequested);

            return Task.FromResult(received);
        }

        public Task SendAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            _dtlsTransport.Send(buffer.Array, buffer.Offset, buffer.Count);
            return Task.FromResult(0);
        }

        public void Dispose()
        {
            _dtlsTransport?.Close();
            _udpTransport?.Dispose();
        }

        ProtocolVersion ConvertProtocolVersion(DtlsVersion dtlsVersion)
        {
            if (dtlsVersion == DtlsVersion.V1_0)
            {
                return ProtocolVersion.DTLSv10;
            }

            if (dtlsVersion == DtlsVersion.V1_2)
            {
                return ProtocolVersion.DTLSv12;
            }

            throw new NotSupportedException();
        }
    }
}
