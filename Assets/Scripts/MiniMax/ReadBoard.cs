using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadBoard : MonoBehaviour
{
    /*
    This class contains various helper functions for the minimax algorithm to interface with the 
    3D noughts and crosses grid. Most notably it provides ways of reading and evaluating board states.

    Note that the GameObjects (gellableCubes) that denote the cells in the grid are converted through 
    various functions listed here into integer arrays. The Key for these arrays are as follows:
        1: 'Orange' cell, aka the players move
        -1: 'Blue' cell, aka the AI's move
        0: Empty cell, aka a cell that is avalable for the AI and player  
    */

    public GameObject[] cells;

    /* 
    These int arrays denote the current board values.
    From the perspective of the spawn platform:
        The first int array is the 'top' row (furthest away from spawn platform)
        The second is the 'middle' row 
        The third is the 'bottom' row (closest to the spawn platform) 
    */
    private int[] board_top = new int[3];
    private int[] board_mid = new int[3];
    private int[] board_bot = new int[3];


    public List<int[]> getBoardState() 
    {
        /*
        Returns: The state of the board, as a list of integer arrays that denote the rows top, mid and bot, respectivly

        This is the main function for reading the current state of the board. It generates a new List
        of integer arrays, where each integer array denotes the values in a row.
        */
        List<int[]> returnList = new List<int[]>();
        // this functions writes the in-game values of the gellableCube gameobjects to the global int arrays.
        readBoard();
        // add these updated int arrays to a list and return it as the new current state of the board.
        returnList.Add(board_top);
        returnList.Add(board_mid);
        returnList.Add(board_bot);
        return returnList;
    }
    public int boardEvaluation(List<int[]> boardState) 
    {
        /*
        Parameters: List<int[]> boardState: the state of a board to be evaluated, where each int[] denotes
            the rows top, mid, and bot respectivly.
        Returns: an integer denoting the overall 'state' of the board, as follows:
            1: 'Orange' has won the game, ie. there are 3 orange squares that are in a row (including diagonals)
            -1: 'Blue' has won the game, ie. there are 3 blue squares that are in a row (including diagonals)
            0: A draw or incomplete game.

        This function evaluates a given board to see if a player has won.
        */
        // Extract rows of the board state
        int[] bTop = boardState[0];
        int[] bMid = boardState[1];
        int[] bBot = boardState[2];

        // Sum up all values in all rows
        foreach(int[] bRow in boardState) 
        {
            if (boardSum(bRow) == 3) 
            {
                // if they sum to 3, there are 3 'Orange' squares in a row, return 1
                return 1;
            } else if (boardSum(bRow) == -3) 
            {
                // if they sum to -3, there are 3 'Blue' squares in a row, return -1
                return -1;
            }
        }

        // Sum up all values in all collumns
        for (int i = 0; i < 3; i++) 
        {
            // for each column (index of rows), sum values together
            int sum = bTop[i] + bMid[i] + bBot[i];
            if ( sum == 3 ) 
            {
                // if they sum to 3, there are 3 'Orange' squares in a row, return 1
                return 1;
            } else if ( sum == -3 ) 
            {
                // if they sum to -3, there are 3 'Blue' squares in a row, return -1
                return -1;
            }
        }

        // Sum up all values in all diagonals. note: in a 3x3 grid such as this there are only 2 diagonals,
        //      first: a diagonal starting from the top left corner going downwards, and
        //      second: a diagonal starting from the bottom left corner going upwards
        int sumTopLeft = bTop[0] + bMid[1] + bBot[2];
        int sumBotLeft = bBot[0] + bMid[1] + bTop[2];
        if (sumTopLeft == 3 || sumBotLeft == 3) 
        {
            // if they sum to 3, there are 3 'Orange' squares in a row, return 1
            return 1;
        } else if (sumTopLeft == -3 || sumBotLeft == -3) 
        {
            // if they sum to -3, there are 3 'Blue' squares in a row, return -1
            return -1;
        }

        // no winning positions were found, iether the game was a draw, or it is an incomplete board, return 0
        return 0;
    }

    public List<List<int[]>> getPossibleMoves(List<int[]> state, int player) 
    {
        /*
        Parameters: 
            List<int[]> state: the state of a board to be evaluated, where each int[] denotes
                the rows top, mid, and bot respectivly.
            int player: The player who's theoretical turn it is, iether 1 or -1.
        Returns: List<List<int[]>>: A list of all possible board states that the player could move
            during one turn.

        This function returns a list containing all possible moves that a player could make during their
        turn.
        */
        List<List<int[]>> returnList = new List<List<int[]>>();
        List<int[]> stateStore = new List<int[]>();
        int[] rowStore = new int[3];

        int[] bTop = state[0];
        int[] bMid = state[1];
        int[] bBot = state[2];

        for (int i = 0; i < 3; i++)  
        {   // for each columun in the board

            // if there is an avalable move in the top row, clone top row (int[] bTop), substitute
            // space with a player move, and add the new state to the return list.
            if (bTop[i] == 0) 
            {
                stateStore = new List<int[]>();
                rowStore = (int[]) bTop.Clone();
                rowStore[i] = player;
                stateStore.Add(rowStore);
                stateStore.Add(bMid);
                stateStore.Add(bBot);
                returnList.Add(stateStore);
            }

            // if there is an avalable move in the mid row, clone mid row (int[] bMid), substitute
            // space with a player move, and add the new state to the return list.
            if (bMid[i] == 0) 
            {
                stateStore = new List<int[]>();
                rowStore = (int[]) bMid.Clone();
                rowStore[i] = player;
                stateStore.Add(bTop);
                stateStore.Add(rowStore);
                stateStore.Add(bBot);
                returnList.Add(stateStore);
            }
        
            // if there is an avalable move in the bot row, clone bot row (int[] bBot), substitute
            // space with a player move, and add the new state to the return list.
            if (bBot[i] == 0) 
            {
                stateStore = new List<int[]>();
                rowStore = (int[]) bBot.Clone();
                rowStore[i] = player;
                stateStore.Add(bTop);
                stateStore.Add(bMid);
                stateStore.Add(rowStore);
                returnList.Add(stateStore);
            }
        }
        return returnList;
    } 

    public bool isBoardDraw(List<int[]> b) 
    {
        /*
        Parameters: List<int[]> b: the state of a board to be evaluated, where each int[] denotes
                the rows top, mid, and bot respectivly.
        Returns: True if the given board is a draw, false otherwise
        */

        // for all positions if there exists a 0 return false
        for (int i = 0; i < 3; i++) 
        {
            for (int j = 0; j < 3; j++) 
            {
                if (b[i][j] == 0) 
                {
                    return false;
                }
            }
        }
        return true;
    } 

    private void readBoard() 
    {   
        /*
        Translates the colors of each gellableCube GameObject (denoting a grid cell) into an integer in the correct
        position in a board state, based on the gameObjects color (handeled by ColorToInt()) 
        */
        foreach(GameObject cell in cells) 
        {
            switch (cell.name) 
            {
                // for each gellableCube gameobject (denotes a cell), switch using its name, extract its value and place
                // into its respective position in the board state (rows)
                case "TopLeft":
                    board_top[0] = ColorToInt(cell);
                    break;
                case "TopMid":
                    board_top[1] = ColorToInt(cell);
                    break;
                case "TopRight":
                    board_top[2] = ColorToInt(cell);
                    break;
                case "MidLeft":
                    board_mid[0] = ColorToInt(cell);
                    break;
                case "MidMid":
                    board_mid[1] = ColorToInt(cell);
                    break;
                case "MidRight":
                    board_mid[2] = ColorToInt(cell);
                    break;
                case "BotLeft":
                    board_bot[0] = ColorToInt(cell);
                    break;
                case "BotMid":
                    board_bot[1] = ColorToInt(cell);
                    break;
                case "BotRight":
                    board_bot[2] = ColorToInt(cell);
                    break;
                default:
                    break;
            }
        }
    }
    private int boardSum(int[] bVal) 
    {
        /*
        Sum all values in parameter int[] bVal and return
        */
        int sum = 0;
        foreach(int v in bVal) 
        {
            sum += v;
        }
        return sum;
    }

    private int ColorToInt(GameObject cell) 
    {
        /*
        Parameter: GameObject cell: The game obeject to read
        Returns: the number corrisponding to the parameters color as follows:
            'Orange': 1
            'Blue': -1
        */
        string color = cell.GetComponent<GellableCube>().currentColor;
        if (color == "orange") 
        {
            return 1;
        } else if (color == "blue") 
        {
            return -1;
        } else {
            return 0;
        }
    }

    private string printRow(int[] row) 
    {
        /*
        Helper function for printing a row in a board state.
        */
        return "[" + row[0] + ", " + row[1] + ", " + row[2] + "]";
    }

    public string printBoard(List<int[]> b) 
    {
        /*
        Helper function for printing a board state.
        */
        if (b != null) 
        {
            string strT = b[0][0].ToString() + ", " + b[0][1].ToString() + ", " + b[0][2].ToString();
            string strM = b[1][0].ToString() + ", " + b[1][1].ToString() + ", " + b[1][2].ToString();
            string strB = b[2][0].ToString() + ", " + b[2][1].ToString() + ", " + b[2][2].ToString();
            return "[[" + strT + "] [" + strM + "] [" + strB + "]]";
        } else {
            return "INPUT ERROR";
        }

    }

}
