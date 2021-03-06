﻿using CODE_GameLib;
using CODE_GameLib.Doors;
using CODE_GameLib.Enums;
using CODE_GameLib.Interfaces;
using CODE_GameLib.Items;
using System;

namespace CODE_Frontend
{
    public class GameView
    {
        private CharWithColor[,] _board;

        public GameView()
        {
            Console.WriteLine("Please hit any keys or hit escape to exit...");
        }

        public void Draw(Game game)
        {
            Console.Clear();

            if (game.DidPlayerWin())
            {
                DrawWin();
            }
            else if (game.IsPlayerDead())
            {
                DrawLose();
            }
            else
            {
                _board = new CharWithColor[game.Player.CurrentRoom.Width, game.Player.CurrentRoom.Height];
                CalcBoard(game.Player);
                DrawBoard();
            }
        }

        public void DrawEnd()
        {
            Console.WriteLine("Quitting game, goodbye!");
        }

        public void DrawWin()
        {
            Console.WriteLine("You did it. You crazy son of a bitch, you did it! You picked up all the stones!");
            Console.WriteLine("Press esc to quit.");
        }

        public void DrawLose()
        {
            Console.WriteLine("You absolute fucktard you died!");
            Console.WriteLine("Press esc to quit.");
        }

        private void CalcBoard(Player player)
        {
            //put all item position in array
            CalcItem(player);

            //put all walls position in array
            CalcWalls(player);

            //put all door position in array
            CalcDoors(player);

            //put player position in array
            CalcPlayer(player);
        }

        private void CalcPlayer(Player player)
        {
            _board[player.Spot.X, player.Spot.Y] = new CharWithColor('X', ConsoleColor.Yellow);
        }

        private void CalcItem(Player player)
        {
            foreach (var item in player.CurrentRoom.Items)
            {
                if (item.GetType() == typeof(Key))
                {
                    var temp = (Key)item;
                    if (!temp.IsPickedUp)
                    {
                        _board[item.Coordinate.X, item.Coordinate.Y] = new CharWithColor('K', (ConsoleColor)Enum.Parse(typeof(ConsoleColor), temp.ColorCode.Name));
                    }
                }
                else if (item.GetType() == typeof(PressurePlate))
                {
                    _board[item.Coordinate.X, item.Coordinate.Y] = new CharWithColor('T', ConsoleColor.Yellow);
                }
                else if (item.GetType() == typeof(SankaraStone))
                {
                    var temp = (SankaraStone)item;
                    if (!temp.IsPickedUp)
                    {
                        _board[item.Coordinate.X, item.Coordinate.Y] = new CharWithColor('S', ConsoleColor.Yellow);
                    }
                }
                else if (item.GetType() == typeof(SingleUseTrap))
                {
                    var temp = (SingleUseTrap)item;
                    if (!temp.IsUsed)
                    {
                        _board[item.Coordinate.X, item.Coordinate.Y] = new CharWithColor('@', ConsoleColor.Yellow);
                    }
                }
                else if (item.GetType() == typeof(Trap))
                {
                    _board[item.Coordinate.X, item.Coordinate.Y] = new CharWithColor('O', ConsoleColor.Yellow);
                }
            }
        }

        private void CalcDoors(Player player)
        {
            foreach (var kvp in player.CurrentRoom.Connections)
            {
                var coordinate = new Coordinate(0, 0);

                //calculate location door
                switch (kvp.Key)
                {
                    case Direction.North:
                        coordinate.X = (player.CurrentRoom.Width - 1) / 2;
                        coordinate.Y = 0;

                        AddDoor(coordinate, kvp.Value);

                        break;
                    case Direction.East:
                        coordinate.X = player.CurrentRoom.Width - 1;
                        coordinate.Y = (player.CurrentRoom.Height - 1) / 2;

                        AddDoor(coordinate, kvp.Value);

                        break;
                    case Direction.West:
                        coordinate.X = 0;
                        coordinate.Y = (player.CurrentRoom.Height - 1) / 2;

                        AddDoor(coordinate, kvp.Value);

                        break;
                    case Direction.South:
                        coordinate.X = (player.CurrentRoom.Width - 1) / 2;
                        coordinate.Y = player.CurrentRoom.Height - 1;

                        AddDoor(coordinate, kvp.Value);

                        break;
                }

                //check type door
                if (kvp.Value.GetType() == typeof(ToggleDoor))
                {
                    _board[coordinate.X, coordinate.Y] = new CharWithColor('┴', ConsoleColor.Yellow);
                }
                else if (kvp.Value.GetType() == typeof(SingleUseDoor))
                {
                    _board[coordinate.X, coordinate.Y] = new CharWithColor('∩', ConsoleColor.Yellow);
                }
                else if (kvp.Value.GetType() == typeof(ColorCodedDoor))
                {
                    var temp = (ColorCodedDoor)kvp.Value;
                    if (temp.Location == Direction.East || temp.Location == Direction.West)
                    {
                        _board[coordinate.X, coordinate.Y] = new CharWithColor('|',
                            (ConsoleColor)Enum.Parse(typeof(ConsoleColor), temp.ColorCode.Name));
                    }
                    else
                    {
                        _board[coordinate.X, coordinate.Y] = new CharWithColor('=',
                        (ConsoleColor)Enum.Parse(typeof(ConsoleColor), temp.ColorCode.Name));
                    }
                }
                else
                {
                    _board[coordinate.X, coordinate.Y] = new CharWithColor(' ', ConsoleColor.Black);
                }
            }
        }

        private void CalcWalls(Player player)
        {
            for (var i = 0; i < player.CurrentRoom.Width; i++)
            {
                _board[i, 0] = new CharWithColor('#', ConsoleColor.Yellow);
                _board[i, player.CurrentRoom.Height - 1] = new CharWithColor('#', ConsoleColor.Yellow);
            }

            for (var i = 0; i < player.CurrentRoom.Height; i++)
            {
                _board[0, i] = new CharWithColor('#', ConsoleColor.Yellow);
                _board[player.CurrentRoom.Width - 1, i] = new CharWithColor('#', ConsoleColor.Yellow);
            }
        }

        private void AddDoor(Coordinate coordinate, IDoor door)
        {
            if (door.GetType() == typeof(ColorCodedDoor))
            {
                var temp = (ColorCodedDoor)door;
                _board[coordinate.X, coordinate.Y] = new CharWithColor('=',
                    (ConsoleColor)Enum.Parse(typeof(ConsoleColor), temp.ColorCode.Name));
                _board[coordinate.X, coordinate.Y] = new CharWithColor('=', ConsoleColor.Yellow);
            }
            else
            {
                _board[coordinate.X, coordinate.Y] = new CharWithColor(' ', ConsoleColor.Black);
            }
        }

        private void DrawBoard()
        {
            //bool color = false;
            //Console.BackgroundColor = ConsoleColor.DarkGray;

            for (var i = 0; i < _board.GetLength(1); i++)
            {
                for (var j = 0; j < _board.GetLength(0); j++)
                {
                    //if (color)
                    //{
                    //    Console.BackgroundColor = ConsoleColor.DarkGray;
                    //    color = false;
                    //}
                    //else
                    //{
                    //    Console.BackgroundColor = ConsoleColor.White;
                    //    color = true;
                    //}

                    if (_board[j, i] != null)
                    {
                        Console.ForegroundColor = _board[j, i].Color;
                        Console.Write(_board[j, i].C);
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
                Console.WriteLine();
            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
