
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using GameMachine;

namespace GameMachine.Pathfinding
{
    public class Pathfinder2d
    {
        public struct Point
        {
            public int x, y;
		 
            public Point (int px, int py)
            {
                x = px;
                y = py;
            }
        }

        public Pathfinder2d (int width, int height)
        {
            addPathfinder (1, width, height);
        }

        public Point[] FindPath (Point start, Point end)
        {
            int numPaths = findpath2d (1, start.x, start.y, end.x, end.y);
			
            Point[] paths = new Point[numPaths];

            for (int i = 0; i < numPaths; i++) {
                paths [i] = new Point (getXAt (1, i), getYAt (1, i));
            }
            return paths;
        }

        public void setPassable (int x, int y)
        {
            setPassable (1, y, x);
        }

        public void setBlocked (int x, int y)
        {
            setBlocked (1, y, x);
        }

        [DllImport("detour_path")]
        public static extern void addPathfinder (int pathfinder, int width, int height);

        [DllImport("detour_path")]
        public static extern void setPassable (int pathfinder, int x, int y);

        [DllImport("detour_path")]
        public static extern void setBlocked (int pathfinder, int x, int y);

        [DllImport("detour_path")]
        public static extern int getXAt (int pathfinder, int index);

        [DllImport("detour_path")]
        public static extern int getYAt (int pathfinder, int index);
		

        [DllImport("detour_path")]
        public static extern int findpath2d (
			int pathfinder,
            int startx,
            int starty,
            int endx,
            int endy
        );
    }
}
