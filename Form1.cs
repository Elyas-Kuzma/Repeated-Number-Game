using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Repeated_Number
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        enum enRoundCase : byte
        {
            eUserWin,
            eUserLoss,
            eNoCaseYet
        }

        struct strGameInfo
        {
            public sbyte WinnerTime;
            public sbyte LossTime;
        }

        strGameInfo GameInfo;
        sbyte[,] arrNumbers = new sbyte[5,5];
        sbyte NumberOfRounds;
        Random randomNumber = new Random();
        sbyte Secound = 10;
        sbyte SearchNumber;
        bool IsCheckedButtonClicked = false;

        //Function

        private void SetNotifyIcon(string Title, string Text)
        {

            notifyIcon1.Icon = SystemIcons.Application;
            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon1.BalloonTipTitle = Title;
            notifyIcon1.BalloonTipText = Text;
            notifyIcon1.ShowBalloonTip(2000);

        }

        private void ShowUserWinnerScreen()
        {
            MessageBox.Show($"You Win in this Round\nYou Take {10-Secound} Secounds","Win");
        }

        private void ShowUserLossScreen()
        {
            MessageBox.Show($"You Lose in This Round", "Loss");
        }

        private sbyte NumberOfRepeated()
        {
            sbyte HowManyRepeated = 0;
            
            foreach(Control ctr in pnlButtonNumbers.Controls)
            {
                if (ctr.Text == SearchNumber.ToString())
                    HowManyRepeated++;
            }

            return HowManyRepeated;

        }

        private enRoundCase IsUserWin()
        {
            mtxtRepeatedNumber.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;

            if (string.IsNullOrEmpty(mtxtRepeatedNumber.Text))
                return enRoundCase.eNoCaseYet;


            sbyte userInput = Convert.ToSByte(mtxtRepeatedNumber.Text);
            if (userInput == NumberOfRepeated())
                return enRoundCase.eUserWin;
            else
               return enRoundCase.eUserLoss;

        }

        private void ShowEndRoundScreen()
        {
            switch(IsUserWin())
            {
                case enRoundCase.eUserWin:
                    ShowUserWinnerScreen();
                    timer1.Stop();
                    GameInfo.WinnerTime++;
                    break;

                case enRoundCase.eUserLoss:
                    ShowUserLossScreen();
                    timer1.Stop();
                    GameInfo.LossTime++;
                    break;

                case enRoundCase.eNoCaseYet:
                    if (Secound <= 0) 
                    {
                        ShowUserLossScreen();
                        timer1.Stop();
                        GameInfo.LossTime++;

                    }
                    break;

            }
        }

        private void FillArreyWithNumber()
        {
            for (sbyte i = 0; i < 5; i++)
                for (sbyte j = 0; j < 5; j++)
                    arrNumbers[i, j] = (sbyte)randomNumber.Next(10);

        }

        private void FillButtonWithRandomNumber()
        {
            int i = 0, j = 0;
            FillArreyWithNumber();

            foreach(Control ctr in pnlButtonNumbers.Controls)
            {
                ctr.Text = arrNumbers[i, j].ToString();
                j++;

                if (j == 5) 
                {
                    j = 0;
                    i++;
                }
            }
        }

        private sbyte GetRoundNumber()
        {
            return (sbyte)nudGameRound.Value;
        }

        private sbyte GetSearchNumber()
        {
            return (sbyte)randomNumber.Next(10);
        }

        private void Round()
        {

            Secound = 10;
            SearchNumber = GetSearchNumber();
            lblSearchNumber.Text = SearchNumber.ToString();

            FillButtonWithRandomNumber();
            lblRound.Text = NumberOfRounds.ToString();

            nudGameRound.Enabled = false;
            panel2.Visible = true;

            timer1.Enabled = true;


        }

        private void PlayGame()
        {
            Round();
        }

        private void NextRound()
        {
            if(NumberOfRounds>1)
            {
                NumberOfRounds--;
                Round();
            }
            else
            {
                SetNotifyIcon("Repeated Number","Game Over");
                ShowEndGameScreen();      
                nudGameRound.Enabled = true;
                
            }

        }

        private void CheckWin()
        {
            if(string.IsNullOrEmpty(mtxtRepeatedNumber.Text) && Secound !=0)
            {
                MessageBox.Show("Enter a number");
                return;
            }


            if(IsUserWin() != enRoundCase.eNoCaseYet && Secound>0)
            {
                timer1.Stop();
                ShowEndRoundScreen();
                NextRound();
                return;

            }
            if(Secound<=0)
            {
                timer1.Stop();
                ShowEndRoundScreen();
                NextRound();
            }
        }

        private void ResetAllButtons()
        {
            int i = 0, j = 0;
           

            foreach (Control ctr in pnlButtonNumbers.Controls)
            {
                ctr.Text = "";
                j++;

                if (j == 5)
                {
                    j = 0;
                    i++;
                }
            }
        }

        private void ResetLabels()
        {
            lblRound.Text = "";
            lblSearchNumber.Text = "";
            lblTime.Text = "";
        }

        private void ShowEndGameScreen()
        {
            if (GameInfo.WinnerTime == GameInfo.LossTime)
                MessageBox.Show($"Win Time {GameInfo.WinnerTime}\nLoss Time {GameInfo.LossTime}", "No Winner");

            if (GameInfo.WinnerTime > GameInfo.LossTime)
                MessageBox.Show($"Win Time {GameInfo.WinnerTime}\nLoss Time {GameInfo.LossTime}","Winner");
            else
                MessageBox.Show($"Win Time {GameInfo.WinnerTime}\nLoss Time {GameInfo.LossTime}", "Loss");


        }

        //Design Function

        private void btnStart_Click(object sender, EventArgs e)
        {
            NumberOfRounds = GetRoundNumber();
            PlayGame();
           
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            Secound--;
            lblTime.Text = Secound.ToString();

            if (Secound <= 0)
            {
                Secound = 0;
                timer1.Stop();
                CheckWin();
            }

        }

        private void btnCheck_Click(object sender, EventArgs e)
        {

            CheckWin();
      
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetAllButtons();
            ResetLabels();

            NumberOfRounds = 0;
            GameInfo.WinnerTime = 0;
            GameInfo.LossTime = 0;
            nudGameRound.Value = 0;
            mtxtRepeatedNumber.Clear();
            panel2.Visible = false;
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mtxtRepeatedNumber.Clear(); 
        }

  
    }
}


