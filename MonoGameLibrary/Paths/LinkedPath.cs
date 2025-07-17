using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Paths
{
    /// <summary>
    /// Represents a set of paths linked together.
    /// This class can be used to create complex paths by linking multiple Path objects.
    /// </summary>
    public class LinkedPath : Path
    {
        private List<Path> _paths;

        /// Represents the starting point of the path.
        /// </summary>
        public override Vector2 StartingPoint
        {
            get
            {
                if (_paths.Count == 0)
                {
                    return Vector2.Zero; // Default starting point if no paths are linked.
                }
                else
                {
                    // The starting point is the starting point of the first path in the linked paths.
                    return _paths[0].StartingPoint;
                }
            }
            set
            {
                if (_paths.Count > 0)
                {
                    // Set the starting point of the first path in the linked paths.
                    _paths[0].StartingPoint = value;
                }
            }
        }

        /// <summary>
        /// The end point of the path is always computed based on 
        /// the starting point and the path's parameters.
        /// </summary>
        public override Vector2 EndingPoint
        {
            get
            {
                if (_paths.Count == 0)
                {
                    return StartingPoint;
                }
                else
                {
                    // The ending point is the ending point of the last path in the linked paths.
                    return _paths[_paths.Count - 1].EndingPoint;
                }
            }
        }

        /// <summary>
        /// A list of relevant control points for the path, if any.
        /// Useful for visualizing the path for debug purposes.
        /// </summary>
        public override List<Vector2> ControlPoints
        {
            get
            {
                List<Vector2> controlPoints = new List<Vector2>();
                foreach (var path in _paths)
                {
                    if (path.StartingPoint != this.StartingPoint)
                    {
                        controlPoints.Add(path.StartingPoint);
                    }
                    controlPoints.AddRange(path.ControlPoints);
                    if (path.EndingPoint != this.EndingPoint)
                    {
                        controlPoints.Add(path.EndingPoint);
                    }
                }
                return controlPoints;
            }
        }

        public override float Length
        {
            get
            {
                float totalLength = 0f;
                foreach (var path in _paths)
                {
                    totalLength += path.Length;
                }
                return totalLength;
            }
        }

        public LinkedPath()
        {
            _paths = new List<Path>();
        }

        public LinkedPath(IEnumerable<Path> paths)
        {
            _paths = new List<Path>(paths);
        }

        public void AddPath(Path path)
        {
            if (path != null)
            {
                _paths.Add(path);
            }
        }

        public void ClearPaths()
        {
            _paths.Clear();
        }

        public override Vector2 ComputePositionFromDistance(float distance)
        {
            if (_paths.Count == 0)
            {
                return StartingPoint; // If no paths are linked, return the starting point.
            }

            float accumulatedDistance = 0f;

            foreach (var path in _paths)
            {
                float pathLength = path.Length;
                if (accumulatedDistance + pathLength >= distance)
                {
                    // The distance falls within this path.
                    return path.ComputePositionFromDistance(distance - accumulatedDistance);
                }
                accumulatedDistance += pathLength;
            }

            // If the distance exceeds the total length, return the ending point of the last path.
            return EndingPoint;
        }

        public static LinkedPath FromFile(ContentManager content, string fileName, Vector2 origin)
        {
            LinkedPath linkedPath = new LinkedPath();

            string filepath = System.IO.Path.Combine(content.RootDirectory, fileName);

            using (Stream stream = TitleContainer.OpenStream(filepath))
            {
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    XDocument doc = XDocument.Load(reader);
                    XElement root = doc.Root;

                    var paths = root.Element("Paths")?.Elements("Path");

                    if (paths != null)
                    {
                        foreach (var _path in paths)
                        {
                            string type = _path.Attribute("type")?.Value;
                            if (type == "LinePath")
                            {
                                linkedPath.AddPath(LinePath.LoadFromXML(_path, origin));
                            }
                            else if (type == "ArcPath")
                            {
                                linkedPath.AddPath(ArcPath.LoadFromXML(_path, origin));
                            }
                            else
                            {
                                throw new InvalidOperationException($"Invalid path type: {type}");
                            }
                        }
                    }
                }
            }

            return linkedPath;
        }

        public static LinkedPath FromFile(ContentManager content, string fileName)
        {
            return FromFile(content, fileName, Vector2.Zero);
        }

        public override void Draw(SpriteBatch spriteBatch, Texture2D pixel)
        {
            foreach (Path _path in _paths) {
                _path.Draw(spriteBatch, pixel);
            }
        }
    }
}   