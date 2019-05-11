using UnityEngine;

//Class to manage the tetris grid
public class Grid : MonoBehaviour
{
    public static int witdh = 10;
    public static int height = 20;
    public static Transform[,] grid = new Transform[witdh, height];

    //Function to round a vector. 
    public static Vector2 roundVec2(Vector2 vector)
    {
        return new Vector2(Mathf.Round(vector.x), Mathf.Round(vector.y));
    }

    //Function to check if one position has a valid position
    public static bool isInsideBorder(Vector2 pos)
    {
        return ((int)pos.x >= 0 && (int)pos.x < witdh && (int)pos.y >= 0);
    }

    //Function to delete all blocks in one row
    private static void deleteOneRow(int row)
    {
        for (int counter = 0; counter < witdh; counter++)
        {
            Destroy(grid[counter, row].gameObject);
            grid[counter, row] = null;
        }
        decreaseAllRowsAbove(row + 1);
    }

    //Function to decrease all bloks in one row
    private static void decreaseAllRow(int row)
    {
        for (int counter = 0; counter < witdh; counter++)
        {
            //If there are a block, change the position (height)
            if (grid[counter, row] != null)
            {
                grid[counter, row - 1] = grid[counter, row];
                grid[counter, row] = null;
                grid[counter, row - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    //Function to decrease the height position for every block in every row above the delete row
    private static void decreaseAllRowsAbove(int row)
    {
        for (int counter = row; counter < height; counter++)
        {
            decreaseAllRow(counter);
        }
    }

    //This function checks if one row is full
    private static bool isOneRowFull(int row)
    {
        for (int counter = 0; counter < witdh; counter++)
        {
            if (grid[counter, row] == null) return false;
        }
        return true;
    }

    //This function will delete all full rows and return afterwards the nummber of deletet rows
    public static int deleteAllFullRows()
    {
        int countRows = 0;
        for (int counter = 0; counter < height; counter++)
        {
            if (isOneRowFull(counter))
            {
                deleteOneRow(counter);
                counter--;
                countRows++;
            }
        }
        return countRows;
    }
}
