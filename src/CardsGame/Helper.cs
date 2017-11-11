using System;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace CardsGame {
    public static class Helper {
        private const ushort DefaultMessageLength = 4;
        internal const ushort CardAmount = 10;
        internal const string EndGameMessage = "2000";
        
        public static string FileNameByCard(string p) {
            StringBuilder fileName = new StringBuilder(20);
            bool tryParseCardNumber = int.TryParse(p.Substring(0, 2), out int cardNumber);
            if (!tryParseCardNumber)
                throw new Exception($"Card number {p.Substring(0, 2)} is invalid");
            switch (cardNumber) {
                case 1:
                    fileName.Append("ace_of_");
                    break;
                case 11:
                    fileName.Append("jack_of_");
                    break;
                case 12:
                    fileName.Append("queen_of_");
                    break;
                case 13:
                    fileName.Append("king_of_");
                    break;
                default:
                    fileName.AppendFormat("_{0}_of_", cardNumber);
                    break;
            }
            switch (p[2]) {
                case 'C':
                    fileName.Append("clubs");
                    break;
                case 'D':
                    fileName.Append("diamonds");
                    break;
                case 'H':
                    fileName.Append("hearts");
                    break;
                case 'S':
                    fileName.Append("spades");
                    break;
            }
            if (cardNumber > 10) {
                fileName.Append("2");
            }
            if (p == "01S") {
                fileName.Append("2");
            }
            return fileName.ToString();
        }

        public static string RandomCard() {
            char[] types = { 'C', 'D', 'H', 'S' };
            Random rnd = new Random();
            int rndNum = rnd.Next(1, 13);
            char rndChar = types[rnd.Next(0, 3)];
            string card = rndNum.ToString() + rndChar;
            return rndNum < 10 ? "0" + card : card;
        }

        public abstract class ClientForm : Form {
            protected ConnectionState TcpConnectionState = ConnectionState.NotConnected;
            private TcpClient _client;
            private NetworkStream _stream;
            private byte[] _buffer;

            protected abstract void DebugMessage(string message);

            protected abstract void ReceivedDataProcessor(byte[] buffer);
            protected abstract void EndConnectionProcessor();

            protected void Connect(string hostname, int port) {
                _client = new TcpClient();
                TcpConnectionState = ConnectionState.Connecting;
                _client.Connect(hostname, port);
                TcpConnectionState = ConnectionState.Connected;
                DebugMessage($"Connected to: {hostname}:{port}");
                _stream = _client.GetStream();
                _buffer = new byte[DefaultMessageLength];
            }

            protected IAsyncResult BeginRead() {
                return _stream.BeginRead(_buffer, 0, DefaultMessageLength, ReadCallback, _stream);
            }

            protected IAsyncResult BeginSend(string data) {
                DebugMessage($"Sending: {data}");
                return _stream.BeginWrite(
                    new ASCIIEncoding().GetBytes(data), 0, DefaultMessageLength, WriteCallback, _stream);
            }

            private void ReadCallback(IAsyncResult ar) {
                try {
                    int received = _stream.EndRead(ar);
                    if (received == 0) { return; }
                    byte[] receiveBuffer = new byte[received];
                    Buffer.BlockCopy(_buffer, 0, receiveBuffer, 0, received);
                    DebugMessage($"Received: {Encoding.Default.GetString(_buffer)}");
                    ReceivedDataProcessor(receiveBuffer);
                } catch (Exception e) {
                    Console.WriteLine("ReadCallback failed: {0}", e.Message);
                    CloseClient();
                }
            }

            private void WriteCallback(IAsyncResult ar) {
                try {
                    _stream.EndWrite(ar);
                } catch (Exception e) {
                    Console.WriteLine("WriteCallback failed: {0}", e.Message);
                }
            }

            protected void CloseClient() {
                EndConnectionProcessor();
                if (_client.Connected)
                    _client.Close();
                TcpConnectionState = ConnectionState.NotConnected;
            }
        }

        public enum ConnectionState { NotConnected, Connected, Connecting }
    }
}
