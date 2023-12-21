using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snake
{
    internal class GameState
    {
        public int Rows { get; }
        public int Cols { get; }
        public GridValue[,] Grid { get; }
        public Direction Dir { get; private set; }
        public Direction Dir2 { get; private set; }
        public int Score { get; private set; }
        public int HighScore {  get; private set; }
        public bool GameOver { get; private set; }

        private readonly LinkedList<Direction> dirChanges = new();
        private readonly LinkedList<Direction> dirChanges2 = new();
        
        private readonly LinkedList<Position> snakePositions = new LinkedList<Position>();
        private readonly LinkedList<Position> snake2Positions = new LinkedList<Position>();
        private readonly Random random = new Random();

        public GameState(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            Grid = new GridValue[rows, cols];
            Dir = Direction.Up;
            Dir2 = Direction.Down;
            AddSnake();
            AddFood();
        }


        private void AddSnake()
        {
            int r = Rows / 2;
            AddSnake2();                    
            for (int c = 1; c <= 3; c++)
            {
                Grid[r, c] = GridValue.Snake;
                snakePositions.AddFirst(new Position(r, c));
            }
        }
        private void AddSnake2()
        {
            int r = Rows / 2;

            for (int c = 1; c <= 3; c++)
            {
                Grid[r, c] = GridValue.Snake2;
                snake2Positions.AddFirst(new Position(r, c));
            }
        }


        private IEnumerable<Position> EmptyPositions()
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (Grid[r, c] == GridValue.Empty)
                    {
                        yield return new Position(r, c);
                    }

                }

            }
        }

        private void AddFood()
        {
            List<Position> empty = new List<Position>(EmptyPositions());

            if (empty.Count == 0) { return; }

            Position pos = empty[random.Next(empty.Count)];
            Grid[pos.Row, pos.Col] = GridValue.Food;
        }

        public Position HeadPosition()
        {
            return snakePositions.First.Value;
        }

        public Position TailPosition()
        {
            return snakePositions.Last.Value;
        }

        public IEnumerable<Position> SnakePositions()
        {
            return snakePositions;
        }

        private void AddHead(Position pos)
        {
            snakePositions.AddFirst(pos);
            Grid[pos.Row, pos.Col] = GridValue.Snake;
        }

        private void RemoveTail()
        {
            Position tail = snakePositions.Last.Value;
            Grid[tail.Row, tail.Col] = GridValue.Empty;
            snakePositions.RemoveLast();
        }

        // Add a new method to get the head position of the second snake
        public Position HeadPosition2()
        {
            return snake2Positions.First.Value;
        }

        // Add a new method to get the tail position of the second snake
        public Position TailPosition2()
        {
            return snake2Positions.Last.Value;
        }

        // Add a new method to get the positions of the second snake
        public IEnumerable<Position> Snake2Positions()
        {
            return snake2Positions;
        }

        // Add a new method to add a head to the second snake
        private void AddHead2(Position pos)
        {
            snake2Positions.AddFirst(pos);
            Grid[pos.Row, pos.Col] = GridValue.Snake2;
        }

        // Add a new method to remove the tail of the second snake
        private void RemoveTail2()
        {
            Position tail = snake2Positions.Last.Value;
            Grid[tail.Row, tail.Col] = GridValue.Empty;
            snake2Positions.RemoveLast();
        }

        private Direction GetLastDirection()
        {
            if (dirChanges.Count == 0)
            {
                return Dir;
            }

            return dirChanges.Last.Value;
        }

        private bool CanChangeDirection(Direction newDir)
        {
            if (dirChanges.Count == 2)
            {
                return false;
            }

            Direction lastDir = GetLastDirection();
            return newDir != lastDir && newDir != lastDir.Opposite();
        }
        public void ChangeDirection(Direction dir)// i think this is what i have to work with next this is a reminder to do this tomarrow 
        {
            if (CanChangeDirection(dir))
            {
                dirChanges.AddLast(dir);
            }
        }
        private bool CanChangeDirection2(Direction newDir2)
        {
            if (dirChanges2.Count == 2)
            {
                return false;
            }

            Direction lastDir2 = GetLastDirection2();
            return newDir2 != lastDir2 && newDir2 != lastDir2.Opposite();
        }
        public void ChangeDirection2(Direction dir2)// i think this is what i have to work with next this is a reminder to do this tomarrow 
        {
            if (CanChangeDirection2(dir2))
            {
                dirChanges2.AddLast(dir2);
            }
        }
        private Direction GetLastDirection2()
        {
            if (dirChanges2.Count == 0)
            {
                return Dir2;
            }

            return dirChanges2.Last.Value;
        }

        private bool OutsideGrid(Position pos)
        {
            return pos.Row < 0 || pos.Row >= Rows || pos.Col < 0 || pos.Col >= Cols;
        }

        private GridValue WillHit(Position newHeadPos)
        {
            if (OutsideGrid(newHeadPos))
            {
                return GridValue.Outside;
            }
            if (newHeadPos == TailPosition())
            {
                return GridValue.Empty;
            }
            return Grid[newHeadPos.Row, newHeadPos.Col];
        }

        public void Move()
        {
            if (dirChanges.Count > 0)
            {
                Dir = dirChanges.First.Value;
                dirChanges.RemoveFirst();
            }


            Position newHeadPos = HeadPosition().Translate(Dir);
            GridValue hit = WillHit(newHeadPos);

            if (hit == GridValue.Outside || hit == GridValue.Snake || hit == GridValue.Snake2)
            {
                GameOver = true;
            }
            else if (hit == GridValue.Empty)
            {
                RemoveTail();
                AddHead(newHeadPos);
            }
            else if (hit == GridValue.Food)
            {
                AddHead(newHeadPos);
                Score++;
                AddFood();
            }
            if (dirChanges2.Count > 0)
            {
                Dir2 = dirChanges2.First.Value;
                dirChanges2.RemoveFirst();
            }
            Position newHeadPos2 = HeadPosition2().Translate(Dir2);
            GridValue hit2 = WillHit(newHeadPos2);

            if (hit2 == GridValue.Outside || hit2 == GridValue.Snake2 || hit2 == GridValue.Snake)
            {
                GameOver = true;
            }
            else if (hit2 == GridValue.Empty)
            {
                RemoveTail2();
                AddHead2(newHeadPos2);
            }
            else if (hit2 == GridValue.Food)
            {
                AddHead2(newHeadPos2);
                Score++;
                AddFood();
            }
        }
    }
}

        
    
    

