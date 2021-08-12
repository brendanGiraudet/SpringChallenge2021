using System;
using System.Collections.Generic;
using System.Linq;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Player
{
    static void Main(string[] args)
    {
        string[] inputs;

        var cells = GetCells();

        cells.ForEach(Console.Error.WriteLine);

        // game loop
        while (true)
        {
            int day = int.Parse(Console.ReadLine()); // the game lasts 24 days: 0-23
            int nutrients = int.Parse(Console.ReadLine()); // the base score you gain from the next COMPLETE action
            inputs = Console.ReadLine().Split(' ');
            int sun = int.Parse(inputs[0]); // your sun points
            int score = int.Parse(inputs[1]); // your current score
            inputs = Console.ReadLine().Split(' ');
            int oppSun = int.Parse(inputs[0]); // opponent's sun points
            int oppScore = int.Parse(inputs[1]); // opponent's score
            bool oppIsWaiting = inputs[2] != "0"; // whether your opponent is asleep until the next day

            var trees = GetTrees(cells);

            trees.ForEach(Console.Error.WriteLine);

            int numberOfPossibleActions = int.Parse(Console.ReadLine()); // all legal actions
            for (int i = 0; i < numberOfPossibleActions; i++)
            {
                string possibleAction = Console.ReadLine(); // try printing something from here to start with
            }

            // GROW cellIdx | SEED sourceIdx targetIdx | COMPLETE cellIdx | WAIT <message>
            var mineTrees = trees.Where(t => t.IsMine && !t.IsDormant);

            var seed = mineTrees.FirstOrDefault(t => t.Size == 0 && sun >= 1 + trees.Count(t => t.Size == 1 && t.IsMine));
            if (seed != null && NeedTree(1, trees.Where(t=>t.IsMine)))
            {
                Console.WriteLine($"GROW {seed.Cell.Index}");
                continue;
            }

            var smallerTree = mineTrees.FirstOrDefault(t => t.Size == 1 && sun >= 3 + trees.Count(t => t.Size == 2 && t.IsMine));
            if (smallerTree != null && NeedTree(2, trees.Where(t => t.IsMine)))
            {
                Console.WriteLine($"GROW {smallerTree.Cell.Index}");
                continue;
            }

            var middleSizeTree = mineTrees.FirstOrDefault(t => t.Size == 2 && sun >= 7 + trees.Count(t => t.Size == 3 && t.IsMine));
            if (middleSizeTree != null && NeedTree(3, trees.Where(t => t.IsMine)))
            {
                Console.WriteLine($"GROW {middleSizeTree.Cell.Index}");
                continue;
            }

            var tallerTree = mineTrees.FirstOrDefault(t => t.Size == 3 && sun > 3);
            if (tallerTree != null)
            {
                Console.WriteLine($"COMPLETE {tallerTree.Cell.Index}");
                continue;
            }

            var (seededCell, treeSeederCell) = GetCellsToSeed(cells, mineTrees.Where(t => t.Size > 0), trees);
            if (seededCell != null && treeSeederCell != null && sun > mineTrees.Count(t => t.Size == 0) && NeedTree(1, trees.Where(t => t.IsMine)))
            {
                Console.WriteLine($"SEED {treeSeederCell.Index} {seededCell.Index}");
                continue;
            }

            Console.WriteLine("WAIT");
        }
        static bool NeedTree(int size, IEnumerable<Tree> trees)
        {
            return trees.Count(t => t.Size == size) < 3;
        }

        static List<Cell> GetCells()
        {
            var cells = new List<Cell>();
            string[] inputs;
            int numberOfCells = int.Parse(Console.ReadLine()); // 37
            for (int i = 0; i < numberOfCells; i++)
            {
                inputs = Console.ReadLine().Split(' ');

                cells.Add(new Cell
                {
                    Index = int.Parse(inputs[0]), // 0 is the center cell, the next cells spiral outwards
                    Richness = int.Parse(inputs[1]), // 0 if the cell is unusable, 1-3 for usable cells
                    NeighbouringCell0 = int.Parse(inputs[2]), // the index of the neighbouring cell for each direction
                    NeighbouringCell1 = int.Parse(inputs[3]),
                    NeighbouringCell2 = int.Parse(inputs[4]),
                    NeighbouringCell3 = int.Parse(inputs[5]),
                    NeighbouringCell4 = int.Parse(inputs[6]),
                    NeighbouringCell5 = int.Parse(inputs[7])
                });
            }
            return cells;
        }
    }

    private static (Cell seededCell, Cell SeederTreeCell) GetCellsToSeed(List<Cell> cells, IEnumerable<Tree> mineTrees, IEnumerable<Tree> trees)
    {
        foreach (var tree in mineTrees)
        {
            var neighbouringCell = GetNeighbourinCellToSeed(cells, tree.Cell, trees);
            if (neighbouringCell != null) return (neighbouringCell, tree.Cell);
        }
        return (null, null);
    }

    private static Cell GetNeighbourinCellToSeed(List<Cell> cells, Cell treeCell, IEnumerable<Tree> trees)
    {
        if (treeCell == null) return null;

        return cells.Find(c =>
        (
            c.Index.Equals(treeCell.NeighbouringCell0)
            || c.Index.Equals(treeCell.NeighbouringCell1)
            || c.Index.Equals(treeCell.NeighbouringCell2)
            || c.Index.Equals(treeCell.NeighbouringCell3)
            || c.Index.Equals(treeCell.NeighbouringCell4)
            || c.Index.Equals(treeCell.NeighbouringCell5)
        )
        && c.Richness != 0
        && !trees.Any(t => t.Cell.Index.Equals(c.Index)));
    }

    private static List<Cell> GetEmptyCells(List<Cell> cells, List<Tree> trees)
    {
        var treeCells = trees.Select(t => t.Cell);
        return cells.Except(treeCells).ToList();
    }

    private static List<Tree> GetTrees(List<Cell> cells)
    {
        var trees = new List<Tree>();
        int numberOfTrees = int.Parse(Console.ReadLine()); // the current amount of trees
        for (int i = 0; i < numberOfTrees; i++)
        {
            var inputs = Console.ReadLine().Split(' ');
            int cellIndex = int.Parse(inputs[0]); // location of this tree
            trees.Add(new Tree
            {
                Cell = cells.Find(c => c.Index == cellIndex),
                Size = int.Parse(inputs[1]), // size of this tree: 0-3
                IsMine = inputs[2] != "0", // 1 if this is your tree
                IsDormant = inputs[3] != "0" // 1 if this tree is dormant
            });
        }

        return trees;
    }

    class Tree
    {
        public Cell Cell { get; set; }
        public int Size { get; set; }
        public bool IsMine { get; set; }
        public bool IsDormant { get; set; }

        public override string ToString()
        {
            return $"Cell {Cell} \r" +
                $"Size {Size} " +
                $"IsMine {IsMine} " +
                $"IsDormant {IsDormant}";
        }
    }

    class Cell
    {
        public int Index { get; set; }
        public int Richness { get; set; }
        public int NeighbouringCell0 { get; set; }
        public int NeighbouringCell1 { get; set; }
        public int NeighbouringCell2 { get; set; }
        public int NeighbouringCell3 { get; set; }
        public int NeighbouringCell4 { get; set; }
        public int NeighbouringCell5 { get; set; }

        public override string ToString()
        {
            return $"Index {Index} " +
                $"Richness {Richness} " +
                $"NC0 {NeighbouringCell0} " +
                $"NC1 {NeighbouringCell1} " +
                $"NC2 {NeighbouringCell2} " +
                $"NC3 {NeighbouringCell3} " +
                $"NC4 {NeighbouringCell4} " +
                $"NC5 {NeighbouringCell5} ";

        }
    }
}