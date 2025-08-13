using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Collision;
using MonoGameLibrary.Graphics;

namespace MonoGameLibrary.Paths
{
    public class LinkedPath : Path
    {
        private Vector2 _offset;
        public override Vector2 Offset
        {
            get { return _offset; }
            set
            {
                foreach (var path in Paths)
                {
                    path.Offset = value;
                }
                _offset = value;
            }
        }
        private float _scale = 1.0f;
        public override float Scale
        {
            get { return _scale; }
            set
            {
                foreach (var path in Paths)
                {
                    path.Scale = value;
                }
                _scale = value;
            }
        }
        public List<Path> Paths;
        public override Vector2 StartingPoint
        {
            get
            {
                if (Paths.Count == 0)
                {
                    return Vector2.Zero;
                }
                else
                {
                    return Paths[0].StartingPoint;
                }
            }
        }

        public override Vector2 EndingPoint
        {
            get
            {
                if (Paths.Count == 0)
                {
                    return StartingPoint;
                }
                else
                {
                    return Paths[Paths.Count - 1].EndingPoint;
                }
            }
        }

        public override List<Vector2> ControlPoints
        {
            get
            {
                List<Vector2> controlPoints = new List<Vector2>();
                foreach (var path in Paths)
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
                foreach (var path in Paths)
                {
                    totalLength += path.Length;
                }
                return totalLength;
            }
        }
        public LinkedPath() : this(Vector2.Zero, 1.0f) { }
        public LinkedPath(Vector2 offset, float scale)
        {
            Paths = new List<Path>();
            Offset = offset;
            Scale = scale;
        }

        public LinkedPath(IEnumerable<Path> paths)
        {
            Paths = new List<Path>(paths);
        }

        public void AddPath(Path path)
        {
            if (path != null)
            {
                path.Offset = this.Offset;
                path.Scale = this.Scale;
                Paths.Add(path);
            }
        }

        public void ClearPaths()
        {
            Paths.Clear();
        }

        public override void LoadSprites(TextureAtlas atlas)
        {
            foreach (var path in Paths)
            {
                path.LoadSprites(atlas);
            }
        }

        public override Vector2 ComputePositionFromDistance(float distance)
        {
            if (Paths.Count == 0)
            {
                return StartingPoint; // If no paths are linked, return the starting point.
            }

            float accumulatedDistance = 0f;

            foreach (var path in Paths)
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

        public static LinkedPath FromFile(ContentManager content, string fileName, Vector2 offset, float scale)
        {
            LinkedPath linkedPath = new LinkedPath(offset, scale);

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
                                linkedPath.AddPath(LinePath.LoadFromXML(_path));
                            }
                            else if (type == "ArcPath")
                            {
                                linkedPath.AddPath(ArcPath.LoadFromXML(_path));
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
            return FromFile(content, fileName, Vector2.Zero, 1.0f);
        }

        public override void Draw(SpriteBatch spriteBatch, Texture2D pixel)
        {
            foreach (Path _path in Paths)
            {
                _path.Draw(spriteBatch, pixel);
            }
        }

        public override bool HasCollided(Hitbox hitbox)
        {
            foreach (Path _path in Paths)
            {
                if (_path.HasCollided(hitbox))
                {
                    return true;
                }
            }
            return false;
        }
    }
}   