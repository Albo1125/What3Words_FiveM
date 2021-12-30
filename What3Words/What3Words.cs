using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace What3Words
{
    public class What3Words : BaseScript
    {
        // Bottom left corner of the map (this shall be treated as the zero).
        public const int XOffset = -4135;
        public const int YOffset = -5155;

        // Upper right corner of the map
        public const int MaxX = 4825;
        public const int MaxY = 8400;

        public const int SquareLength = 5;

        //4,858,112 squares.
        // 170 nPr 3 = 4826640
        // 171 nPr 3 = 4912830
        public const int NumWords = 171;

        public static string[] Words = new string[NumWords];

        //Square numbering: 1, 2, ..., x from left to right. When right boundary reached, go up one and start at the left again.
        public const int NumXAxisSquares = (MaxX - XOffset) / SquareLength; //x in the above explanation (number of squares from left to right on one row).

        public What3Words()
        {
            EventHandlers["What3Words:CoordsFromWord"] += new Action<string>((string fullWord) =>
            {
                if (Game.Player != null && Game.Player.Character != null && Game.Player.Character.Exists())
                {
                    if (string.IsNullOrWhiteSpace(fullWord))
                    {
                        TriggerEvent("chatMessage", "W3W", new int[] { 255, 0, 0 }, "No word specified!");
                        return;
                    }

                    string[] splitWords = fullWord.Split('.');
                    if (splitWords.Length != 3)
                    {
                        TriggerEvent("chatMessage", "W3W", new int[] { 255, 0, 0 }, "Invalid word specified (not exactly three words)!");
                        return;
                    }

                    try
                    {
                        Vector2 coords = GetCoordsFromWord(new Word(splitWords[0].ToLower(), splitWords[1].ToLower(), splitWords[2].ToLower()));
                        World.WaypointPosition = new Vector3(coords.X, coords.Y, 0);
                        TriggerEvent("chatMessage", "W3W", new int[] { 255, 0, 0 }, "Destination set to " + fullWord + ".");
                    }
                    catch (ArgumentException)
                    {
                        TriggerEvent("chatMessage", "W3W", new int[] { 255, 0, 0 }, "Invalid word specified (unknown word).");
                        World.RemoveWaypoint();
                    }
                }
            });

            EventHandlers["What3Words:ShowCurrentWord"] += new Action<dynamic>((dynamic d) =>
            {
                if (Game.Player != null && Game.Player.Character != null && Game.Player.Character.Exists())
                {
                    Vector3 pos = Game.PlayerPed.Position;
                    Word w = GetWordFromCoords(pos.X, pos.Y);


                    if (w != null)
                    {
                        TriggerEvent("chatMessage", "W3W", new int[] { 255, 0, 0 }, "Your current location: " + w.FullWord);
                    }
                    else
                    {
                        TriggerEvent("chatMessage", "W3W", new int[] { 255, 0, 0 }, "You are outside the What3Words coordinate boundaries.");
                    }


                    Blip gpsblip = World.GetWaypointBlip();
                    if (gpsblip != null && gpsblip.Exists())
                    {
                        Word gpsWord = GetWordFromCoords(gpsblip.Position.X, gpsblip.Position.Y);
                        if (gpsWord != null)
                        {
                            TriggerEvent("chatMessage", "W3W", new int[] { 255, 0, 0 }, "Your marked GPS location: " + gpsWord.FullWord);
                        }
                        else
                        {
                            TriggerEvent("chatMessage", "W3W", new int[] { 255, 0, 0 }, "Your marked GPS location is outside the What3Words coordinate boundaries.");
                        }
                    }

                }
            });

            string resourceName = API.GetCurrentResourceName();
            string wordstextfile = API.LoadResourceFile(resourceName, "words.txt");
            Words = wordstextfile.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToLower()).ToArray();

            if (Words.Length != NumWords)
            {
                Debug.WriteLine("Error: Not enough words specified in words.txt. Minimum of " + NumWords + " required. Exiting.");
                throw new Exception("Error: Not enough words specified in words.txt. Minimum of " + NumWords + " required. Exiting.");
            }

            if (Words.Distinct().ToArray().Length != Words.Length)
            {
                Debug.WriteLine("Error: Duplicate words specified in words.txt. Exiting.");
                throw new Exception("Error: Duplicate words specified in words.txt. Exiting.");
            }
        }

        public static Word GetWordFromCoords(float x, float y)
        {
            int xRound = (int)x;
            int yRound = (int)y;

            // Assertions to avoid out of range issues.
            if (xRound < XOffset || yRound < YOffset || xRound > MaxX || yRound > MaxY)
            {
                Debug.WriteLine("Coordinates out of range.");
                return null;
            }

            //Calculate coordinate offset.
            int xDiff = xRound - XOffset;
            int yDiff = yRound - YOffset;

            //Calculate x and y square numbers.
            int xSquare = xDiff / SquareLength;
            int ySquare = yDiff / SquareLength;

            //Calculate the absolute square number
            int squareNumber = ySquare * NumXAxisSquares + xSquare;

            //Debug.WriteLine("Square number: " + squareNumber);

            // See how many times 171 ^ 2 fits. That is the first word index. Subtract the difference.
            int firstWordIndex = squareNumber / (int)Math.Pow(NumWords, 2);
            squareNumber -= firstWordIndex * (int)Math.Pow(NumWords, 2);

            //Debug.WriteLine("First word index: " + firstWordIndex);

            // See how many times 171 ^ 1 fits. That is the second word index. Subtract the difference.
            int secondWordIndex = squareNumber / NumWords;
            squareNumber -= secondWordIndex * NumWords;

            //Debug.WriteLine("Second word index: " + secondWordIndex);

            // See how many times 171 ^ 0 = 1 fits. That is the third word index. Subtract the difference (squarenumber should become zero).
            int thirdWordIndex = squareNumber;
            squareNumber -= thirdWordIndex;

            //Debug.WriteLine("Third word index: " + thirdWordIndex);

            //Debug.WriteLine("Square number (Should be zero): " + squareNumber);

            //Return the final word.
            return new Word(Words[firstWordIndex], Words[secondWordIndex], Words[thirdWordIndex]);

        }

        public static Vector2 GetCoordsFromWord(Word word)
        {
            int WordOneIndex = Array.IndexOf(Words, word.WordOne);
            int WordTwoIndex = Array.IndexOf(Words, word.WordTwo);
            int WordThreeIndex = Array.IndexOf(Words, word.WordThree);

            if (WordOneIndex == -1 || WordTwoIndex == -1 || WordThreeIndex == -1)
            {
                throw new ArgumentException("Invalid word specified - not in array!");
            }

            // Calculate the square number by essentially converting the indices to decimal.
            int squareNumber = WordOneIndex * (int)Math.Pow(NumWords, 2) + WordTwoIndex * NumWords + WordThreeIndex;

            // y square number obtained by finding how many times numxXaxissquares fits in squareNumber.
            int ySquare = squareNumber / NumXAxisSquares;

            // x square number obtained by finding the modulo ie remainder.
            int xSquare = squareNumber % NumXAxisSquares;

            // Get absolute coordinates by multiplying x/y square numbers with the square length. Then add the original X/Y offsets again to get absolute world/game coords.
            return new Vector2(xSquare * SquareLength + XOffset, ySquare * SquareLength + YOffset);
        }
    }
}
