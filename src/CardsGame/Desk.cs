using System;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using CardsGame.Properties;

namespace CardsGame {
    public partial class Desk : Helper.ClientForm {
        private PictureBox opponentPic, lastOpenedCard;
        private bool cardOpened;
        private string opponentCard = "No card";
        private int opponentScoreValue, yourScoreValue;
        static AutoResetEvent autoEvent = new AutoResetEvent(false);

        public Desk() {
            InitializeComponent();
        }

        private void Desk_Load(object sender, EventArgs e) {
            YourScore.Hide();
            OpponentsScore.Hide();
            MainButton.Width = 140;
            MainButton.Text = "Connect to server";
        }

        private void MainButton_Click(object sender, EventArgs e) {
            // TODO: Add enum with state of the desk states
            switch (MainButton.Text) {
                case "Connect to server":
                    MainButton.Text = "Connecting to server...";
                    MainButton.Enabled = false;
                    Connect("127.0.0.1", 8820);
                    Stream.BeginRead(Buffer, 0, 4, ReadCallback, Stream);
                    break;
                case "Forfiet":
                    CloseClient();
                    break;
            }
        }

        private void GenerateCards() {
            YourScore.Show();
            YourScore.Text = "Your Score: 0";
            OpponentsScore.Show();
            OpponentsScore.Text = "Opponent's Score: 0";
            opponentPic = new PictureBox();
            opponentPic.Name = "Opponent's Card";
            opponentPic.Image = Resources.card_back_blue;
            opponentPic.Location = new Point(Width - 125, 10);
            opponentPic.Size = new Size(100, 114);
            opponentPic.SizeMode = PictureBoxSizeMode.StretchImage;
            Controls.Add(opponentPic);
            Point nextLocation = new Point(10, 134);
            for (int i = 0; i < 10; i++) {
                PictureBox currentPic = new PictureBox();
                currentPic.Name = "picDynamic" + i;
                currentPic.Image = Resources.card_back_red;
                currentPic.Location = nextLocation;
                currentPic.Size = new Size(100, 114);
                currentPic.SizeMode = PictureBoxSizeMode.StretchImage;
                currentPic.Click += delegate {
                    lastOpenedCard = currentPic;
                    currentPic.Name = RandomCard();
                    string file = fileNameByCard(currentPic.Name);
                    currentPic.Image = (Image)Resources.ResourceManager.GetObject(file);
                    cardOpened = true;

                    byte[] buffer = new ASCIIEncoding().GetBytes("1" + currentPic.Name);
                    Stream.Write(buffer, 0, 4);
                    Stream.Flush();
                    DebugMessage("waiting for card");
                    opponentPic.Image = (Image)Resources.ResourceManager.GetObject(fileNameByCard(opponentCard));
                    updateScore(currentPic.Name.Substring(0, 2), opponentCard.Substring(0, 2));
                    opponentCard = "No card";
                };
                Controls.Add(currentPic);
                nextLocation.X += currentPic.Size.Width + 10;
                MainButton.Text = "Forfiet";
            }
        }

        private void updateScore(string you, string opp) {
            if (you == opp) {
                //score doesnt change
            } else if (int.Parse(you) == 1 || int.Parse(opp) == 1) {
                if (int.Parse(you) == 1) {
                    yourScoreValue++;
                } else if (int.Parse(opp) == 1) {
                    opponentScoreValue++;
                }
            } else {
                if (int.Parse(you) > int.Parse(opp)) {
                    yourScoreValue++;
                } else {
                    opponentScoreValue++;
                }
            }
            YourScore.Text = "Your score: " + yourScoreValue;
            OpponentsScore.Text = "Opponents score: " + opponentScoreValue;
        }

        private string fileNameByCard(string p) {
            string file;
            switch (int.Parse(p.Substring(0, 2))) {
                case 1:
                    file = "ace_of_";
                    break;
                case 11:
                    file = "jack_of_";
                    break;
                case 12:
                    file = "queen_of_";
                    break;
                case 13:
                    file = "king_of_";
                    break;
                default:
                    file = "_" + int.Parse(p.Substring(0, 2)) + "_of_";
                    break;
            }
            switch (p[2]) {
                case 'C':
                    file += "clubs";
                    break;
                case 'D':
                    file += "diamonds";
                    break;
                case 'H':
                    file += "hearts";
                    break;
                case 'S':
                    file += "spades";
                    break;
            }
            if (int.Parse(p.Substring(0, 2)) > 10) {
                file += "2";
            }
            if (p == "01S") {
                file += "2";
            }
            return file;
        }

        private string RandomCard() {
            char[] types = { 'C', 'D', 'H', 'S' };
            Random rnd = new Random();
            int rndNum = rnd.Next(1, 13);
            char rndChar = types[rnd.Next(0, 3)];
            string card = rndNum.ToString() + rndChar;
            return (rndNum < 10) ? "0" + card : card;
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
                        Invoke((MethodInvoker)delegate {
                            Thread.BeginCriticalRegion();
                            opponentCard = ((char)buffer[1]) + ((char)buffer[2]).ToString() + ((char)buffer[3]);
                            opponentPic.Image = (Image)Resources.ResourceManager.GetObject(fileNameByCard((opponentCard)));
                            Thread.EndCriticalRegion();
                        });
                        DebugMessage("updated to" + ((char)buffer[1]) + ((char)buffer[2]) + ((char)buffer[3])
                            + "\nREAL: " + opponentCard);
                        break;
                    case '2':
                        EndConnectionProcessor();
                        break;
                    default:
                        DebugMessage("Got unknown code" + buffer);
                        break;
                }
            } catch (Exception e) {
                DebugMessage("ERROR: " + e.Message);
            }
        }

        protected override void EndConnectionProcessor() {
            byte[] buffer = new ASCIIEncoding().GetBytes("2000");
            Stream.Write(buffer, 0, 4);
            Stream.Flush();
        }

        private void DebugMessage(string message) {
            Debug.Text = message;
        }
    }
}
