using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mastermind
{
    public class Game
    {
        private string secretNumber;
        private string currentComputerGuess;
        private char[] validDigits;
        private List<string> possibleNumbers;   // a pool that includes all possible numbers that could be the secret number of player
        private int round;

        private string currentPlayerGuess;
        private string currentPlayerHint;

        private eGameConditions gameStatus;
        public enum eGameConditions
        {
            playerWon,
            computerWon,
            ongoing,
            wrongHintEntered
        }

        public Game()
        {
            validDigits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            secretNumber = GenerateNumber().ToString();
            possibleNumbers = GetAllPossibleAnswers();
            currentComputerGuess = possibleNumbers[new Random(DateTime.Now.Millisecond).Next(possibleNumbers.Count)];
            round = 1;
            gameStatus = eGameConditions.ongoing;
        }     

        // Player makes a guess and if false computer turn start if true game is over
        public (bool success, string hint) PlayerTurn()
        {

            bool success = CheckGuess(secretNumber, currentPlayerGuess).success;
            string hint = CheckGuess(secretNumber, currentPlayerGuess).hint;
            
            if (success)
            {
                gameStatus = eGameConditions.playerWon;
            }

            return (success, hint);
        }

        // After computer makes a guess, gets hint from player
        // If there is not hint, computer removes the numbers with any digits that intersects the current guess's digits
        // If there is, apply the current guess to the all possible secret numbers between [1000,9999]
        // Numbers in pool that return the hint which equals to current player's hint, stay in the list. Otherwise, removed from the list
        // Each turn, list is reduced until one number remains in the list and that's the answer
        public void ComputerTurn()
        {

            if (!string.IsNullOrEmpty(currentPlayerHint))
            {
                currentPlayerHint = HintFormat(currentPlayerHint);
            }

            ++round;

            if (currentPlayerHint.Contains("+") || currentPlayerHint.Contains("-"))
            {
                for (int i = 0; i < possibleNumbers.Count; i++)
                {
                    string hintToCompare = CheckGuess(possibleNumbers[i], currentComputerGuess).hint.Replace(" ", string.Empty);

                    if (currentPlayerHint != hintToCompare)
                    {
                        possibleNumbers.RemoveAt(i);
                        i = -1;
                    }
                }

                try
                {
                    currentComputerGuess = possibleNumbers[new Random(DateTime.Now.Millisecond).Next(possibleNumbers.Count)];
                }
                catch (Exception)
                {

                    gameStatus = eGameConditions.wrongHintEntered;
                }
                
            }
            else
            {
                char[] digitsToExclude = NumberUtil.GetDigits(Int32.Parse(currentComputerGuess));
                for (int i = 0; i < possibleNumbers.Count; i++)
                {
                    if (possibleNumbers[i].Contains(digitsToExclude[0]) || possibleNumbers[i].Contains(digitsToExclude[1])
                       || possibleNumbers[i].Contains(digitsToExclude[2]) || possibleNumbers[i].Contains(digitsToExclude[3]))
                    {
                        possibleNumbers.RemoveAt(i);
                        i = -1;
                    }
                }

                try
                {
                    currentComputerGuess = possibleNumbers[new Random(DateTime.Now.Millisecond).Next(possibleNumbers.Count)];
                }
                catch (Exception)
                {

                    gameStatus = eGameConditions.wrongHintEntered;
                }
            }
        }

        // Generates hint according to the number of matching and not matching digits
        private string GenerateHint(int matchingDigitNumber, int notMatchingDigitNumber)
        {
            string hint;

            if (matchingDigitNumber != 0 && notMatchingDigitNumber != 0)
            {
                hint = $"+{matchingDigitNumber} -{notMatchingDigitNumber}";
            }
            else if (matchingDigitNumber == 0 && notMatchingDigitNumber != 0)
            {
                hint = $"-{notMatchingDigitNumber}";
            }
            else if (matchingDigitNumber != 0 && notMatchingDigitNumber == 0)
            {
                hint = $"+{matchingDigitNumber}";
            }
            else
            {
                hint = "";
            }

            return hint;
        }

        // Determines if guess is true or not.
        // If true game ends, otherwise generates hint
        public (bool success, string hint) CheckGuess(string answer, string guess)
        {
            int matching = 0, notMaching = 0;

            char[] digitsOfGuess = NumberUtil.GetDigits(Int32.Parse(guess));
            char[] digitsOfAnswer = NumberUtil.GetDigits(Int32.Parse(answer));

            for (int i=0; i< digitsOfGuess.Length; i++)
            {
                for (int j = 0; j < digitsOfGuess.Length; j++)
                {
                    if (digitsOfGuess[i] == digitsOfAnswer[j])
                    {
                        if(i == j)
                        {
                            matching++;
                            continue;
                        }
                        notMaching++;
                    }

                }               
            }

            if(matching == 4)
            {
                return (true, "NO HINT");
            }
            else
            {
                return (false, GenerateHint(matching, notMaching));
            }

        }

        // All possible numbers with 4 unique digits 
        public List<string> GetAllPossibleAnswers()
        {
            List<string> tokens = new List<string>();
            for (int d1 = 0; d1 < validDigits.Length; d1++)
                for (int d2 = 0; d2 < validDigits.Length; d2++)
                    for (int d3 = 0; d3 < validDigits.Length; d3++)
                        for (int d4 = 0; d4 < validDigits.Length; d4++)
                        {
                            if (d1 != d2 && d1 != d3 && d1 != d4
                                   && d2 != d3 && d2 != d4
                                    && d3 != d4 && d1 != 0)
                            {
                                tokens.Add((validDigits[d1].ToString() + validDigits[d2].ToString() + validDigits[d3].ToString() + validDigits[d4].ToString()));
                            }
                        }
            return tokens;
        }

        // Generates number between 1000, 10000 with unique digits
        private int GenerateNumber()
        {
            Random rand = new Random();
            int number = rand.Next(1000, 10000);
            while (!NumberUtil.IsNumberValid(number))
            {
                number = rand.Next(1000, 10000);
            }
            return number;
        }


        public bool IsHintValid(string hint)
        {
            char[] validChars = new char[] { '0', '1', '2', '3', '4', '+', '-' };
            int[] validHintLengths = new int[] { 0, 2, 4 };

            int numberOfPlus = hint.Contains('+') ? Int32.Parse(hint.Substring((hint.IndexOf("+") + 1), 1)) : 0;
            int numberOfMinus = hint.Contains('-') ? Int32.Parse(hint.Substring((hint.IndexOf("-") + 1), 1)) : 0;

            if (!validHintLengths.Contains(hint.Length))
            {
                return false;
            }
            
            for(int i=0; i<hint.Length; i++)
            {
                if (!validChars.Contains(hint[i])){
                    return false;
                }
            }

            if(numberOfMinus + numberOfPlus > 4)
            {
                return false;
            }

            return true;


        }

        public string HintFormat(string hint)
        {

            int numberOfPlus = hint.Contains('+') ? Int32.Parse(hint.Substring((hint.IndexOf("+") + 1), 1)) : 0;
            int numberOfMinus = hint.Contains('-') ? Int32.Parse(hint.Substring((hint.IndexOf("-") + 1), 1)) : 0;

            if(numberOfMinus + numberOfPlus == 0)
            {
                return "No hint";
            }
            else if(numberOfMinus == 0)
            {
                return $"+{numberOfPlus}";
            }
            else if(numberOfPlus == 0)
            {
                return $"-{numberOfMinus}";
            }
            else
            {
                return $"+{numberOfPlus}-{numberOfMinus}";
            }            
        }

        public void setCurrentPlayerGuess(string guess)
        {
            currentPlayerGuess = guess;
        }

        public void setCurrentPlayerHint(string hint)
        {
            currentPlayerHint = hint;
        }

        public int getRound()
        {
            return round;
        }

        public eGameConditions getGameStatus()
        {
            return gameStatus;
        }
        public void setGameStatus(eGameConditions status)
        {
            gameStatus = status;
        }

        public string getCurrentComputerGuess()
        {
            return currentComputerGuess;
        }
    }
}
