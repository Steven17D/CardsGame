using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using CardsGame.Properties;
using static CardsGame.Helper;

namespace CardsGame {
    public partial class GameClientForm : ClientForm {
        private PictureBox _opponentPic;
        private string _opponentCard, _drawnCard;
        private ushort _opponentScoreValue, _yourScoreValue;
        private ushort _amountOfOpenedCards;

        public GameClientForm() {
            InitializeComponent();
        }

        private void Desk_Load(object sender, EventArgs e) {
            YourScore.Hide();
            OpponentsScore.Hide();
            MainButton.Width = 140;
            MainButton.Text = "Connect to server";
        }

        private void MainButton_Click(object sender, EventArgs e) {
            switch (TcpConnectionState) {
                case ConnectionState.NotConnected:
                    MainButton.Text = "Connecting to server...";
                    MainButton.Enabled = false;
                    Connect("127.0.0.1", 8820);
                    BeginRead();
                    break;
                case ConnectionState.Connecting:
                case ConnectionState.Connected:
                    CloseClient();
                    Environment.Exit(0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void GenerateCards() {
            YourScore.Show();
            YourScore.Text = "Your Score: 0";
            OpponentsScore.Show();
            OpponentsScore.Text = "Opponent's Score: 0";
            _opponentPic = new PictureBox
            {
                Name = "Opponent's Card",
                Image = Resources.card_back_blue,
                Location = new Point(Width - 125, 10),
                Size = new Size(100, 114),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            Controls.Add(_opponentPic);
            Point nextLocation = new Point(10, 134);
            for (int i = 0; i < 10; i++) {
                PictureBox currentPic = new PictureBox
                {
                    Name = "picDynamic" + i,
                    Image = Resources.card_back_red,
                    Location = nextLocation,
                    Size = new Size(100, 114),
                    SizeMode = PictureBoxSizeMode.StretchImage
                };
                currentPic.Click += (sender, e) => {
                    if (!string.IsNullOrEmpty(_drawnCard)) return;
                    // Generate a random card
                    _drawnCard = RandomCard();
                    currentPic.Name = _drawnCard;
                    currentPic.Image = (Image)Resources.ResourceManager.GetObject(FileNameByCard(currentPic.Name));
                    _amountOfOpenedCards++;
                    // Send the Generated card
                    BeginSend("1" + currentPic.Name);
                    // Receive an answer
                    BeginRead();
                    // Remove click event handler
                    FieldInfo currentPicFieldInfo = typeof(Control).GetField("EventClick", 
                        BindingFlags.Static | BindingFlags.NonPublic);
                    object obj = currentPicFieldInfo?.GetValue(currentPic);
                    PropertyInfo currentPicPropertyInfo = currentPic.GetType().GetProperty("Events",  
                        BindingFlags.NonPublic | BindingFlags.Instance);
                    EventHandlerList list = (EventHandlerList)currentPicPropertyInfo?.GetValue(currentPic, null);
                    list?.RemoveHandler(obj, list[obj]);
                };
                Controls.Add(currentPic);
                nextLocation.X += currentPic.Size.Width + 10;
                MainButton.Text = "Forfiet";
            }
        }

        private void UpdateScore(string you, string opp) {
            if (you == opp) {
                //score doesnt change
            } else if (int.Parse(you) == 1 || int.Parse(opp) == 1) {
                if (int.Parse(you) == 1) {
                    _yourScoreValue++;
                } else if (int.Parse(opp) == 1) {
                    _opponentScoreValue++;
                }
            } else {
                if (int.Parse(you) > int.Parse(opp)) {
                    _yourScoreValue++;
                } else {
                    _opponentScoreValue++;
                }
            }
            YourScore.Text = "Your score: " + _yourScoreValue;
            OpponentsScore.Text = "Opponents score: " + _opponentScoreValue;
        }

        protected override void ReceivedDataProcessor(byte[] buffer) {
            try {
                switch ((char)buffer[0]) {
                    case '0':
                        Invoke((MethodInvoker)delegate {
                            MainButton.Text = "Forfiet";
                            MainButton.Enabled = true;
                            GenerateCards();
                        });
                        break;
                    case '1':
                        _opponentCard = Encoding.Default.GetString(buffer).Substring(startIndex:1);
                        _opponentPic.Image = (Image)Resources.ResourceManager.GetObject(FileNameByCard(_opponentCard));
                        UpdateScore(_drawnCard.Substring(0, 2), _opponentCard.Substring(0, 2));
                        _drawnCard = string.Empty;
                        DebugMessage($"Received opponent card: {_opponentCard}");
                        if (_amountOfOpenedCards == CardAmount) {
                            MessageBox.Show(
                                $"Congradulation!\nYou {(_opponentScoreValue > _yourScoreValue ? "LOST" : "WON")}!", 
                                "Game Ended");
                        }
                        break;
                    case '2':
                        EndConnectionProcessor();
                        break;
                    default:
                        DebugMessage($"Got unknown code: {Encoding.Default.GetString(buffer)}");
                        break;
                }
            } catch (Exception e) {
                DebugMessage("ERROR: " + e.Message);
            }
        }

        protected override void EndConnectionProcessor() {
            BeginSend(EndGameMessage);
        }

        protected override void DebugMessage(string message) {
            Console.WriteLine(message);
        }
    }
}
