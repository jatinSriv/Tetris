
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace Tetris
{
    /// <summary>
    /// Logic for Tetris
    /// </summary>
    
    public partial class MainWindow : Window
    {
        struct blocks
        {
            public int cells;
            public int steps;
            public Tuple<int, int> center;
            public Tuple<int, int>[] a;
        }

        double SIN(int x)
        {
            return (Math.Sin(x * 3.141592653589 / 180));
        }

        double COS(int x)
        {
            return (Math.Cos(x * 3.141592653589 / 180));
        }

        int val1, val2,nblocks,dir,milliseconds,playing,done,destructing,score,currotate,nowrotate;
        int[,] myGrid;
        Dictionary<int, Rectangle> _rectDict = new Dictionary<int, Rectangle>();
        Rectangle rect;
        blocks[] tBlocks;
        public MainWindow()
        {
            InitializeComponent();
            InitializeGrid();
            InitializeBlocks();
        }

        private void InitializeBlocks()
        {
            nblocks = 7;
            tBlocks = new blocks[nblocks];

            //for I
            tBlocks[0].cells = 4;
            tBlocks[0].steps = 1;
            tBlocks[0].a = new Tuple<int, int>[4];
            tBlocks[0].a[0] = new Tuple<int, int>(0, 0);
            tBlocks[0].a[1] = new Tuple<int, int>(0, 1);
            tBlocks[0].a[2] = new Tuple<int, int>(0, 2);
            tBlocks[0].a[3] = new Tuple<int, int>(0, 3);
            tBlocks[0].center = new Tuple<int, int>(0, 3);

            //for O

            tBlocks[1].cells = 4;
            tBlocks[1].steps = 2;
            tBlocks[1].a = new Tuple<int, int>[4];
            tBlocks[1].a[0] = new Tuple<int, int>(0, 0);
            tBlocks[1].a[1] = new Tuple<int, int>(0, 1);
            tBlocks[1].a[2] = new Tuple<int, int>(1, 0);
            tBlocks[1].a[3] = new Tuple<int, int>(1, 1);
            tBlocks[1].center = new Tuple<int, int>(0, 1);

            //for T

            tBlocks[2].cells = 4;
            tBlocks[2].steps = 2;
            tBlocks[2].a = new Tuple<int, int>[4];
            tBlocks[2].a[0] = new Tuple<int, int>(0, 1);
            tBlocks[2].a[1] = new Tuple<int, int>(1, 0);
            tBlocks[2].a[2] = new Tuple<int, int>(1, 1);
            tBlocks[2].a[3] = new Tuple<int, int>(1, 2);
            tBlocks[2].center = new Tuple<int, int>(0,1);

            //for S

            tBlocks[3].cells = 4;
            tBlocks[3].steps = 2;
            tBlocks[3].a = new Tuple<int, int>[4];
            tBlocks[3].a[0] = new Tuple<int, int>(0, 1);
            tBlocks[3].a[1] = new Tuple<int, int>(0, 2);
            tBlocks[3].a[2] = new Tuple<int, int>(1, 1);
            tBlocks[3].a[3] = new Tuple<int, int>(1, 0);
            tBlocks[3].center = new Tuple<int, int>(0,1);

            //for Z

            tBlocks[4].cells = 4;
            tBlocks[4].steps = 2;
            tBlocks[4].a = new Tuple<int, int>[4];
            tBlocks[4].a[0] = new Tuple<int, int>(0, 0);
            tBlocks[4].a[1] = new Tuple<int, int>(0, 1);
            tBlocks[4].a[2] = new Tuple<int, int>(1, 1);
            tBlocks[4].a[3] = new Tuple<int, int>(1, 2);
            tBlocks[4].center = new Tuple<int, int>(0,1);

            //for J

            tBlocks[5].cells = 4;
            tBlocks[5].steps = 2;
            tBlocks[5].a = new Tuple<int, int>[4];
            tBlocks[5].a[0] = new Tuple<int, int>(0, 0);
            tBlocks[5].a[1] = new Tuple<int, int>(1, 0);
            tBlocks[5].a[2] = new Tuple<int, int>(1, 1);
            tBlocks[5].a[3] = new Tuple<int, int>(1, 2);
            tBlocks[5].center = new Tuple<int, int>(1, 0);

            //for L

            tBlocks[6].cells = 4;
            tBlocks[6].steps = 2;
            tBlocks[6].a = new Tuple<int, int>[4];
            tBlocks[6].a[0] = new Tuple<int, int>(0, 2);
            tBlocks[6].a[1] = new Tuple<int, int>(1, 0);
            tBlocks[6].a[2] = new Tuple<int, int>(1, 1);
            tBlocks[6].a[3] = new Tuple<int, int>(1, 2);
            tBlocks[6].center = new Tuple<int, int>(1,2);

        }

      

        private async void playGameUtil(int v1, int v2,int blockChoice)
        {
            dir = 0;
            populateGrid(v1, v2, blockChoice);
            await Task.Delay(milliseconds);
            dispopulateGrid(v1, v2, blockChoice);
            if (isValidPos(v1 + 1, v2 + dir, blockChoice))
            {
                nowrotate = currotate;
                playGameUtil(v1 + 1, v2 + dir, blockChoice);
            }
            else
            {
                if (isValidPos(v1 + 1, v2, blockChoice))
                {
                    nowrotate = currotate;
                    playGameUtil(v1 + 1, v2, blockChoice);
                }
                else { 
                populateGrid(v1, v2, blockChoice);
                    destructing = 0;
                    checkForDestruction();
                    while (destructing == 0)
                    {
                        await Task.Delay(400);
                    }
                    done = 1;
                    score += 10;
                    Text2.Text = " "+ Convert.ToString(score);
                }
            }
            
            
        }

        private async void checkForDestruction()
        {
            int f;
            for(int i = val1 - 1; i >= 0; i--)
            {
                f = 0;
                for(int j = 0; j < val2; j++)
                {
                    if (myGrid[i,j] != 1)
                        f = 1;
                }
                if (f == 0)
                {
                    score += 50;
                    Text2.Text = " "+Convert.ToString(score);
                    for (int j = 0; j < val2; j++)
                    {
                        myGrid[i, j] = 0;
                        _rectDict[(i * val2) + j].Stroke = new SolidColorBrush(System.Windows.Media.Colors.ForestGreen);
                        _rectDict[(i * val2) + j].Fill = new SolidColorBrush(System.Windows.Media.Colors.Black);
                    }
                    await Task.Delay(400);
                    for(int k = i - 1; k >= 0; k--)
                    {
                        for(int j = 0; j < val2; j++)
                        {
                            myGrid[k + 1, j] = myGrid[k, j];
                        }
                        for(int j = 0; j < val2; j++)
                        {
                            if(myGrid[k+1,j]==1)
                            _rectDict[(k+1) * val2 + j].Fill = new SolidColorBrush(System.Windows.Media.Colors.DarkViolet);
                            else if (myGrid[k + 1, j] == 0)
                            {
                                _rectDict[((k+1) * val2) + j].Stroke = new SolidColorBrush(System.Windows.Media.Colors.ForestGreen);
                                _rectDict[((k+1) * val2) + j].Fill = new SolidColorBrush(System.Windows.Media.Colors.Black);
                            }
                        }
                    }
                    for(int j = 0; j < val2; j++)
                    {
                        myGrid[0, j] = 0;
                        _rectDict[((0) * val2) + j].Stroke = new SolidColorBrush(System.Windows.Media.Colors.ForestGreen);
                        _rectDict[((0) * val2) + j].Fill = new SolidColorBrush(System.Windows.Media.Colors.Black);
                    }
                    i++;
                }
            }
            destructing = 1;
        }

        private void startGame(object sender, RoutedEventArgs e)
        {
            playing = 1;
            milliseconds = 1000;
            done = 0;
            score = 0;
            currotate = 0;
            nowrotate = 0;
            startGameUtil();
            
        }

        private void ResetGameNow(object sender, RoutedEventArgs e)
        {
            score = 0;
            currotate = 0;
            nowrotate = 0;
            Text2.Text = " " + Convert.ToString(score);
            for (int i = 0; i < val1; i++)
                for (int j = 0; j < val2; j++)
                    myGrid[i, j] = 0;


            for (int i = 0; i < val1; i++)
            {
                for (int j = 0; j < val2; j++)
                {
                    _rectDict[i*val2+j].Stroke= new SolidColorBrush(System.Windows.Media.Colors.ForestGreen);
                    _rectDict[i * val2 + j].Fill = new SolidColorBrush(System.Windows.Media.Colors.Black);
                }
            }


        }

        private async void startGameUtil()
        {
            bool ans = true;
            int count = 0;
            dir = 0;
            int blockChoice;
            int timedelay;
            Random rand = new Random();
            while (ans)
            {
                done = 0;
                currotate = 0;
                nowrotate = 0;
                blockChoice = rand.Next(0, 6);
                timedelay = ((20-count)) * milliseconds;
                timedelay -= 1000;
                count += tBlocks[blockChoice].steps;
                if (isValidPos(0, 3, blockChoice))
                {
                    playGameUtil(0, 3, blockChoice);
                   
                }
                else
                {
                    ans = false;
                    done = 1;
                }
                while (done == 0)
                {
                    await Task.Delay(milliseconds);
                }
                milliseconds = 1000;
            }
            playing = 0;
            for (int i = 0; i < val1; i++)
            {
                for (int j = 0; j < val2; j++)
                {
                    _rectDict[i * val2 + j].Stroke = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    _rectDict[i * val2 + j].Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                }
            }
        }

        private void KeyDownHelper(object sender, KeyEventArgs e)
        {
           if(e.Key==Key.Left && playing == 1)
            {
                dir = -1;
            }
           if(e.Key==Key.Right && playing == 1)
            {
                dir = 1;
            }
           if(e.Key==Key.Down && playing == 1)
            { 
                if(milliseconds>=250)
                milliseconds -= 200;
            }
           if(e.Key==Key.Up && playing == 1)
            {
                currotate = (currotate + 1) % 4;
            }
        }

        private void dispopulateGrid(int v1, int v2, int blockChoice)
        {

            int[] pos = new int[2];

            findNewPos(v1 + tBlocks[blockChoice].a[0].Item1, v2 + tBlocks[blockChoice].a[0].Item2, pos, v1 + tBlocks[blockChoice].center.Item1, v2 + tBlocks[blockChoice].center.Item2);
            myGrid[pos[0], pos[1]] = 0;
            _rectDict[(pos[0]) * 10 + (pos[1])].Fill = new SolidColorBrush(System.Windows.Media.Colors.Black);

            findNewPos(v1 + tBlocks[blockChoice].a[1].Item1, v2 + tBlocks[blockChoice].a[1].Item2, pos, v1 + tBlocks[blockChoice].center.Item1, v2 + tBlocks[blockChoice].center.Item2);
            myGrid[pos[0], pos[1]] = 0;
            _rectDict[(pos[0]) * 10 + (pos[1])].Fill = new SolidColorBrush(System.Windows.Media.Colors.Black);

            findNewPos(v1 + tBlocks[blockChoice].a[2].Item1, v2 + tBlocks[blockChoice].a[2].Item2, pos, v1 + tBlocks[blockChoice].center.Item1, v2 + tBlocks[blockChoice].center.Item2);
            myGrid[pos[0], pos[1]] = 0;
            _rectDict[(pos[0]) * 10 + (pos[1])].Fill = new SolidColorBrush(System.Windows.Media.Colors.Black);

            findNewPos(v1 + tBlocks[blockChoice].a[3].Item1, v2 + tBlocks[blockChoice].a[3].Item2, pos, v1 + tBlocks[blockChoice].center.Item1, v2 + tBlocks[blockChoice].center.Item2);
            myGrid[pos[0], pos[1]] = 0;
            _rectDict[(pos[0]) * 10 + (pos[1])].Fill = new SolidColorBrush(System.Windows.Media.Colors.Black);

        }

        private void populateGrid(int v1, int v2, int blockChoice)
        {
            int[] pos = new int[2];

            findNewPos(v1 + tBlocks[blockChoice].a[0].Item1, v2 + tBlocks[blockChoice].a[0].Item2, pos , v1 + tBlocks[blockChoice].center.Item1, v2 + tBlocks[blockChoice].center.Item2);
            myGrid[pos[0],pos[1]] = 1;
            _rectDict[(pos[0]) * 10 + (pos[1])].Fill = new SolidColorBrush(System.Windows.Media.Colors.DarkViolet);

            findNewPos(v1 + tBlocks[blockChoice].a[1].Item1, v2 + tBlocks[blockChoice].a[1].Item2, pos, v1 + tBlocks[blockChoice].center.Item1, v2 + tBlocks[blockChoice].center.Item2);
            myGrid[pos[0], pos[1]] = 1;
            _rectDict[(pos[0]) * 10 + (pos[1])].Fill = new SolidColorBrush(System.Windows.Media.Colors.DarkViolet);

            findNewPos(v1 + tBlocks[blockChoice].a[2].Item1, v2 + tBlocks[blockChoice].a[2].Item2, pos, v1 + tBlocks[blockChoice].center.Item1, v2 + tBlocks[blockChoice].center.Item2);
            myGrid[pos[0], pos[1]] = 1;
            _rectDict[(pos[0]) * 10 + (pos[1])].Fill = new SolidColorBrush(System.Windows.Media.Colors.DarkViolet);

            findNewPos(v1 + tBlocks[blockChoice].a[3].Item1, v2 + tBlocks[blockChoice].a[3].Item2, pos, v1 + tBlocks[blockChoice].center.Item1, v2 + tBlocks[blockChoice].center.Item2);
            myGrid[pos[0], pos[1]] = 1;
            _rectDict[(pos[0]) * 10 + (pos[1])].Fill = new SolidColorBrush(System.Windows.Media.Colors.DarkViolet);

        }

        private void findNewPos(int v1, int v2, int[] pos , int centerx , int centery)
        {
            int x = v1;
            int y = v2;

            int l, m;

            for(int i = 1; i <= nowrotate ; i++)
            {
                x -= centerx;
                y -= centery;

                l = (0 * x) + (-1 * y);
                m = (1 * x) + (0 * y);

                x = l + centerx;
                y = m + centery;

                v1 = x;
                v2 = x;

            }

            pos[0] = x;
            pos[1] = y;
        }

        private void findNewPos2(int v1, int v2, int[] pos, int centerx, int centery)
        {
            int x = v1;
            int y = v2;

            int l, m;

            for (int i = 1; i <= currotate; i++)
            {
                x -= centerx;
                y -= centery;

                l = (0 * x) + (-1 * y);
                m = (1 * x) + (0 * y);

                x = l + centerx;
                y = m + centery;

                v1 = x;
                v2 = x;

            }

            pos[0] = x;
            pos[1] = y;
        }

        private bool isValidPos(int v1, int v2, int blockChoice)
        {

            int[] pos1 = new int[2];
            findNewPos2(v1 + tBlocks[blockChoice].a[0].Item1, v2 + tBlocks[blockChoice].a[0].Item2, pos1, v1 + tBlocks[blockChoice].center.Item1, v2 + tBlocks[blockChoice].center.Item2);

            int[] pos2 = new int[2];
            findNewPos2(v1 + tBlocks[blockChoice].a[1].Item1, v2 + tBlocks[blockChoice].a[1].Item2, pos2, v1 + tBlocks[blockChoice].center.Item1, v2 + tBlocks[blockChoice].center.Item2);

            int[] pos3 = new int[2];
            findNewPos2(v1 + tBlocks[blockChoice].a[2].Item1, v2 + tBlocks[blockChoice].a[2].Item2, pos3, v1 + tBlocks[blockChoice].center.Item1, v2 + tBlocks[blockChoice].center.Item2);

            int[] pos4 = new int[2];
            findNewPos2(v1 + tBlocks[blockChoice].a[3].Item1, v2 + tBlocks[blockChoice].a[3].Item2, pos4, v1 + tBlocks[blockChoice].center.Item1, v2 + tBlocks[blockChoice].center.Item2);

            if ((pos1[0]) >= 0 && (pos1[0]) <= 19 && (pos2[0]) >= 0 && (pos2[0]) <= 19 && (pos3[0]) >= 0 && (pos3[0]) <= 19 && (pos4[0]) >= 0 && (pos4[0]) <= 19 && (pos1[1]) >= 0 && (pos1[1]) <= 9 && (pos2[1]) >= 0 && (pos2[1]) <= 9 && (pos3[1]) >= 0 && (pos3[1]) <= 9 && (pos4[1]) >= 0 && (pos4[1]) <= 9 && myGrid[pos1[0], pos1[1]] == 0 && myGrid[pos2[0], pos2[1]]==0 && myGrid[pos3[0], pos3[1]] == 0 && myGrid[pos4[0], pos4[1]] == 0 )
            {
                return true;
            }
            else
                return false;
        }

        private void InitializeGrid()
        {
            val1 = 20;
            val2 = 10;
            for (int i = 0; i < val1; i++)
            {
                RowDefinition row = new RowDefinition();

                AnswerGrid.RowDefinitions.Add(row);
            }
            for (int i = 0; i < val2; i++)
            {
                AnswerGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            //AnswerGrid.ShowGridLines = true;
            for (int i = 0; i < val1; i++)
            {
                for (int j = 0; j < val2; j++)
                {
                    rect = new Rectangle();
                    rect.Stroke = new SolidColorBrush(System.Windows.Media.Colors.ForestGreen);
                    rect.StrokeThickness = 1;
                    Grid.SetRow(rect, i);
                    Grid.SetColumn(rect, j);
                    rect.Fill = new SolidColorBrush(System.Windows.Media.Colors.Black);
                    AnswerGrid.Children.Add(rect);
                    _rectDict[i * val2 + j] = rect;
                }
            }
            myGrid = new int[val1, val2];
            for (int i = 0; i < val1; i++)
                for (int j = 0; j < val2; j++)
                    myGrid[i, j] = 0;

        }
    }
}
