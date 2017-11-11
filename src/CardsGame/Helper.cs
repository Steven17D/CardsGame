using System;
using System.Net.Sockets;
using System.Windows.Forms;

namespace CardsGame {
    public static class Helper {
        public abstract class ClientForm : Form {
            private TcpClient _client;
            protected NetworkStream Stream;
            protected byte[] Buffer;

            protected abstract void ReceivedDataProcessor(byte[] buffer);
            protected abstract void EndConnectionProcessor();

            protected void Connect(string hostname, int port) {
                _client = new TcpClient();
                _client.Connect(hostname, port);
                Stream = _client.GetStream();
                Buffer = new byte[_client.ReceiveBufferSize];
            }

            protected void ReadCallback(IAsyncResult ar) {
                try {
                    int received = Stream.EndRead(ar);
                    if (received == 0) { return; }
                    Stream.BeginRead(Buffer, 0, 4, ReadCallback, Stream);
                    byte[] receiveBuffer = new byte[received];
                    System.Buffer.BlockCopy(Buffer, 0, receiveBuffer, 0, received);
                    ReceivedDataProcessor(receiveBuffer);
                } catch (Exception e) {
                    Console.WriteLine("ReadCallback failed: {0}", e.Message);
                    CloseClient();
                }
            }

            protected void WriteCallback(IAsyncResult ar) {
                try {
                    Stream.EndWrite(ar);
                } catch (Exception e) {
                    Console.WriteLine("WriteCallback failed: {0}", e.Message);
                }
            }

            protected void CloseClient() {
                EndConnectionProcessor();
                if (_client.Connected)
                    _client.Close();
            }
        }
    }
}
