using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class minimax : MonoBehaviour
{
    /*
    This class, along with helper functions from ReadBoard, use the minimax algorithm to solve a game of
    naughts and crosses.
    */
    List<int[]> boardState;
    public GameObject gellBlobRef;
    public GameObject finish;
    private bool boardEndState = false;
    
    private void applyMinimax() 
    {
        /*
        This function starts the minimax algorithm for the AI to choose the best move.
        Firstly, it uses helper functions from ReadBoard to generate a list of all possible
        moves that the AI can take.
        Secondly, it loops through all these possible moves (board states) and applys the minimax
        algorithm (depth of 3) to look ahead. This generates the most optimal move (assuming also
        that the player is playing optimally).
        Finally, it places its move on the grid by changing the corrispoding moves cell (GellableCube GameObject)
        */

        // Evaluate the current board for a winner and generate a list of potential moves for the AI (player -1, blue)
        ReadBoard boardManager = GetComponent<ReadBoard>();
        boardState = boardManager.getBoardState();
        int currentEvaluation = boardManager.boardEvaluation(boardState);
        List<List<int[]>> children = boardManager.getPossibleMoves(boardState, -1);

        // set the 'BestScore' to some value greater than 1, as the maximum score that can be returned is 1, and the AI
        // is the minimiser in this case, so will want to reduce the score of the board state as much as possible.
        float bestChildScore = 10;
        List<int[]> bestMove = children[0]; // also stores the move that corrisponds with the best score
        foreach (List<int[]> child in children) 
        {
            // for each possible move (state), run the algorithm and save the 'best move' (lowest scoring state).
            float score = minimaxAlgorithm(child, 3, true);
            if ( score < bestChildScore )
            {
                bestChildScore = score;
                bestMove = new List<int[]>(child);
            }
            // print("child: " + boardManager.printBoard(child) + ", has score:" + score + ", best (lowest) score: " + bestChildScore);
        }
        // place the move onto the board
        placeBlue(stateToCellName(bestMove));
    }

    private float minimaxAlgorithm(List<int[]> position, int depth, bool maximisingPlayer) 
    {
        /*
        The core algorithm for evaluating board states. It scores a given position (state) based on minimsing
        the possible loss for the worst case senario. It does this by 'looking ahead' at possible moves (maximising)
        that the player could take, and then the following moves it could take (minimising), and repeates this process
        to a given depth. The scores for each state are then returned and therefore propogated up towards the root, 
        minimising and maximising all the possible child states acordingly, untill the most optimal move is chosen.  
        */
        ReadBoard boardManager = GetComponent<ReadBoard>();
        float Eval;
        int currentPositionEvaluation = GetComponent<ReadBoard>().boardEvaluation(position);
        //print("minimax(" + boardManager.printBoard(position) + ", " + depth + ", " + maximisingPlayer + ")");

        // recursion terminator, propogate score (current board evaluation, if the current state is a winning state
        // for 1 or -1 (player 'orange' or AI 'blue', respectivly), or 0 if its a draw/incomplete state)
        if (depth == 0 || isBoardEndState(position)) 
        {
            //print("(" + depth + "==0 || " + currentPositionEvaluation + "!=0) Returning: " + currentPositionEvaluation);
            return currentPositionEvaluation;
        }

        // if evaluating best possible player moves (maximising possible states)
        if (maximisingPlayer)   
        {
            //print("Maximise");
            float maxEval = -10;
            List<List<int[]>> children = boardManager.getPossibleMoves(position, 1);
            foreach (List<int[]> child in children) 
            {
                // for all possible moves, choose move with the highest score
                Eval = minimaxAlgorithm(child, depth-1, false);
                //print("Max(" + maxEval + ", " + Eval + ")");
                maxEval = Mathf.Max(maxEval, Eval);
            }
            //print("MAX: child: " + boardManager.printBoard(position) + ", score: " + maxEval);
            return maxEval;
        } else {
            // if evaluating best possible AI moves (minimising possible states)
            //print("Minimise");
            float minEval = 10;
            List<List<int[]>> children = boardManager.getPossibleMoves(position, -1);
            foreach (List<int[]> child in children) 
            {
                 // for all possible moves, choose move with the lowest score
                Eval = minimaxAlgorithm(child, depth-1, true);
                //print("Min(" + minEval + ", " + Eval + ")");
                minEval = Mathf.Min(minEval, Eval);
            }
            //print("MIN: child: " + boardManager.printBoard(position) + ", score: " + minEval);
            return minEval;
        }
    }

    private bool isBoardEndState(List<int[]> b) 
    {
        /*
        Parameters: List<int[]> b: the state of a board to be evaluated, where each int[] denotes
            the rows top, mid, and bot respectivly.
        Returns: bool: true if the board has iether been won by a player, or if it is a draw. False otherwise
        */
        ReadBoard boardManager = GetComponent<ReadBoard>();
        boardState = boardManager.getBoardState();
        int currentEvaluation = boardManager.boardEvaluation(b);

        // if there are no more moves to make OR board is a draw
        if (currentEvaluation != 0 || boardManager.isBoardDraw(b)) 
        {
            return true;
        }
        return false;
    }

    private string stateToCellName(List<int[]> newState) 
    {
        /*
        Parameters: List<int[]> newState: the state of a board to be evaluated, where each int[] denotes
            the rows top, mid, and bot respectivly. This must have at most one difference to the current board state.
        Returns: String: The name of the gameObject that corrisponds to a given position on the board.

        This is a helper function used to convert a specific state to the name of a game object. It compares
        the input state, newState, with the current state of the board. It then finds which specific move has been made, 
        which will be the only difference between the two states (current board state and newState). It then converts this
        move into the corrisponding name of the gameObject its associated with.
        */
        ReadBoard boardManager = GetComponent<ReadBoard>();

        // separate out the newState into a sequence of rows
        int[] newMove_bTop = newState[0];
        int[] newMove_bMid = newState[1];
        int[] newMove_bBot = newState[2];
        // separate out the current board state into a sequence of rows
        int[] state_bTop = boardState[0];
        int[] state_bMid = boardState[1];
        int[] state_bBot = boardState[2];

        int rowEval = -1;

        // check if the top rows are equal
        rowEval = rowsEqualEval(state_bTop, newMove_bTop);
        if (rowEval != -1) 
        {
            // if not, get the position that is different in the newState, and return the corrisponding
            // name of the gameObject 
            switch (rowEval)
            {
                case 0:
                    return "TopLeft";
                case 1:
                    return "TopMid";
                case 2:
                    return "TopRight";
            }
        }

        // check if the mid rows are equal
        rowEval = rowsEqualEval(state_bMid, newMove_bMid);
        if (rowEval != -1) 
        {
            // if not, get the position that is different in the newState, and return the corrisponding
            // name of the gameObject 
            switch (rowEval)
            {
                case 0:
                    return "MidLeft";
                case 1:
                    return "MidMid";
                case 2:
                    return "MidRight";
            }
        }

        // check if the not rows are equal
        rowEval = rowsEqualEval(state_bBot, newMove_bBot);
        if (rowEval != -1) 
        {
            // if not, get the position that is different in the newState, and return the corrisponding
            // name of the gameObject 
            switch (rowEval)
            {
                case 0:
                    return "BotLeft";
                case 1:
                    return "BotMid";
                case 2:
                    return "BotRight";
            }
        }
        // if the newState has no difference, return error.
        return "minimax.stateToCellName() Error";
    }

    private int rowsEqualEval(int[] r1, int[] r2) 
    {
        /*
        Parameters: int[] r1, int[] r2: the two rows on the board to be evaluated.
        Returns: int: the index of the difference in both rows, -1 if they are equal.
        */
        for (int i = 0; i < 3; i++) 
        { 
            if (r1[i] != r2[i]) 
            {
                return i;
            }
        }
        return -1;
    }
    private void placeBlue(string cellName) 
    {
        /*
        Parameters: string cellName: the name of the GameObject to change.

        Changes the material of the cell (gameobject) with the name cellName to blue. This denotes an AI move.
        */
        GameObject[] cells =  GetComponent<ReadBoard>().cells;
        foreach(GameObject cell in cells) 
        {
            if (cell.name == cellName) 
            {
                // Allow for object to be effected by gravity, change its material to blue, set its color and isColored values
                cell.GetComponent<Rigidbody>().isKinematic = false;
                cell.GetComponent<MeshRenderer>().material = cell.GetComponent<GellableCube>().BlueGellMat;
                cell.GetComponent<GellableCube>().currentColor = "blue";
                cell.GetComponent<GellableCube>().isColored = true;
            }
        }
    }

    void OnGUI()
    {
        bool paused = FindObjectOfType<LevelInfoText>().infoIsActive;
        // if the board is in an 'end' state (player or AI has won, or its a draw), display text
        if (boardEndState) 
        {
            GUI.Box(new Rect(0, Screen.height - (Screen.height/8), Screen.width, Screen.height), "You have lost or drawn the game against the AI. Press escape to restart.");
        } else if (!paused) {
            // display information about how the AI can move. Using this method allows for the player to 'cheat' the AI, 
            // (by not allowing them to move) so they can win and therefore progress.
            GUI.Box(new Rect(0, Screen.height - (Screen.height/10), Screen.width, Screen.height), "Press 'f' make the AI play.");
        }
    }

    void Start() 
    {
        // remove the finish (so the player has to beat the AI) and allow the AI to play the first move
        finish.SetActive(false);
        applyMinimax();
    }

    void Update() 
    {
        // costantly update the board state to get the most recent values.
        boardState = GetComponent<ReadBoard>().getBoardState();
        // print("------------------------Board state is: " + GetComponent<ReadBoard>().printBoard(boardState));
        if ( isBoardEndState(boardState) ) 
        {
            // if the board is in an 'end' state (player or AI has won, or its a draw)
            int currentEvaluation = GetComponent<ReadBoard>().boardEvaluation(boardState);
            if (currentEvaluation == -1) 
            {
                // AI has won, display text
                boardEndState = true;
            } else if (currentEvaluation == 1) {
                // player has won, activate the finish
                finish.SetActive(true);
            } else {
                // its a draw, display text
                boardEndState = true;
            }
        } else if (Input.GetKeyDown("f"))
        {
            // allow AI to make a move
            applyMinimax();
        }
    }
}
