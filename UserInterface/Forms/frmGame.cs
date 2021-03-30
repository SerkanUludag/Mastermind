using Mastermind;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UserInterface.Forms
{
    public partial class frmGame : Form
    {
        private bool isPlayerTurn;
        private Game game;
        private List<string> validInputs;

        public frmGame()
        {
            InitializeComponent();
            game = new Game();
            isPlayerTurn = true;
            validInputs = game.GetAllPossibleAnswers();
            richTextBox1.ReadOnly = true;
            button2.Visible = false;
            displayPlayerGuess();
        }

        // check if input is valid and determine if should start the round
        private void button1_Click(object sender, EventArgs e)
        {

            if (game.getGameStatus() == Game.eGameConditions.playerWon || game.getGameStatus() == Game.eGameConditions.wrongHintEntered)
            {
                Application.Exit();
            }
            else
            {
                if (validInputs.Contains(textBox1.Text) || isPlayerTurn == false)
                {
                    startTurn();

                }
                else
                {
                    displayInvalidNumber();
                }
            }

            textBox1.Text = "";
        }

        // check whose turn and start the round
        private void startTurn()
        {
            if(game.getGameStatus() == Game.eGameConditions.ongoing)
            {
                if (isPlayerTurn)
                {
                    game.setCurrentPlayerGuess(textBox1.Text);
                    button1.Text = "Enter hint";
                    richTextBox1.AppendText(textBox1.Text);

                    if (!game.PlayerTurn().success)
                    {
                        displayComputerHint();
                        isPlayerTurn = false;
                        displayComputerGuess();
                        button2.Visible = true;
                    }
                    else
                    {
                        game.setGameStatus(Game.eGameConditions.playerWon);
                        richTextBox1.AppendText("\n\n***PLAYER WON***");
                        button1.Text = "Exit";

                    }                    
                }
                else
                {
                    
                    game.setCurrentPlayerHint(textBox1.Text.Replace(" ", string.Empty));
                    if (game.IsHintValid(textBox1.Text.Replace(" ", string.Empty)))
                    {
                        button1.Text = "Make a guess";
                        game.ComputerTurn();

                        if(game.getGameStatus() == Game.eGameConditions.wrongHintEntered)
                        {
                            button1.Text = "Exit";
                            richTextBox1.AppendText("\nWrong hint entered. Start a new game.");
                            
                        }
                        else
                        {
                            displayPlayerGuess();

                            isPlayerTurn = true;
                        }
                        
                    }
                    else
                    {
                        displayInvalidHint();
                    }

                    
                    button2.Visible = false;
                }
                
            }
            else if (game.getGameStatus() == Game.eGameConditions.playerWon)
            {
                richTextBox1.AppendText("\n\n***PLAYER WON***");
            }
            else if(game.getGameStatus() == Game.eGameConditions.computerWon)
            {
                richTextBox1.AppendText("\n\n***COMPUTER WON***");
            }
            else if(game.getGameStatus() == Game.eGameConditions.wrongHintEntered)
            {
                
            }           
        }

        // check if computer guess is true
        private void button2_Click(object sender, EventArgs e)
        {

            richTextBox1.AppendText("\n\n***COMPUTER WON***");
            button2.Text = "Exit";

            if (game.getGameStatus() == Game.eGameConditions.computerWon)
            {
                Application.Exit();
            }

            game.setGameStatus(Game.eGameConditions.computerWon);

        }

        private void displayInvalidHint()
        {
            richTextBox1.AppendText(textBox1.Text);
            richTextBox1.AppendText("\nHint is not valid. Try again");
            richTextBox1.AppendText($"\nYour hint #{game.getRound()}: ");

        }

        private void displayComputerHint()
        {
            richTextBox1.AppendText($"\nComputer hint #{game.getRound()}: ");
            richTextBox1.AppendText(game.PlayerTurn().hint == "" ? "No hint" : game.PlayerTurn().hint);
            richTextBox1.AppendText("\n");
        }

        private void displayComputerGuess()
        {
            richTextBox1.AppendText($"\nComputer guess #{game.getRound()}: ");
            richTextBox1.AppendText($"{game.getCurrentComputerGuess()}");
            richTextBox1.AppendText("\nIf computer guess is true, press 'True' button");
            richTextBox1.AppendText("\nType hint (e.g. +1-2) or leave blank if there is not");
            richTextBox1.AppendText($"\nYour hint #{game.getRound()}: ");
            
        }

        private void displayPlayerGuess()
        {
            if(game.getRound() != 1)
            {
                richTextBox1.AppendText($"{game.HintFormat(textBox1.Text.Replace(" ", string.Empty))}");
            }
            
            richTextBox1.AppendText(game.getRound() == 1 ? "" : "\n");
            richTextBox1.AppendText("/////////////////////////////////////////////////////////");
            richTextBox1.AppendText($"\nRound: {game.getRound()}");
            richTextBox1.AppendText("\nMake a guess - Enter 4 digit number with unique digits");
            richTextBox1.AppendText($"\nYour guess #{game.getRound()}: ");
        }
        private void displayInvalidNumber()
        {
            richTextBox1.AppendText(textBox1.Text);
            richTextBox1.AppendText("\nEnter valid number");
            richTextBox1.AppendText($"\nYour guess #{game.getRound()}: ");
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }
    }
}