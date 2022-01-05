namespace Enums
{
    public enum Directions {
        Left, Up, Right, Down
    }
    
    /* CURRENT DIR - DESIRED DIR
     * 
     * LEFT  (0) - UP    (1) = -1 (TURN RIGHT)
     * LEFT  (0) - RIGHT (2) = -2 (TURN LEFT OR RIGHT)
     * LEFT  (0) - DOWN  (3) = -3 (TURN LEFT)
     * UP    (1) - LEFT  (0) =  1 (TURN LEFT)
     * UP    (1) - RIGHT (2) = -1 (TURN RIGHT)
     * UP    (1) - DOWN  (3) = -2 (TURN RIGHT OR LEFT)
     * RIGHT (2) - LEFT  (0) =  2 (TURN RIGHT OR LEFT)
     * RIGHT (2) - UP    (1) =  1 (TURN LEFT)
     * RIGHT (2) - DOWN  (3) = -1 (TURN RIGHT)
     * DOWN  (3) - LEFT  (0) =  3 (TURN RIGHT)
     * DOWN  (3) - RIGHT (2) =  1 (TURN LEFT)
     * DOWN  (3) - UP    (1) =  2 (TURN LEFT OR RIGHT)
     *
     * -1 = TURN RIGHT
     * -2 = TURN LEFT OR RIGHT
     * -3 = TURN LEFT
     *
     *  1 = TURN LEFT
     *  2 = TURN LEFT OR RIGHT
     *  3 = TURN RIGHT
     * 
     */
}