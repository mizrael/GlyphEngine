using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using GlyphEngine.SceneGraph;
using GlyphEngine.Components;
using GlyphEngine.Utils;


namespace GlyphEngine.Services
{
    public class SpatialHashService : GameComponent, IPartitioningService
    {
        #region Members

        private int _cols = 0;
        private int _rows = 0;
        
        private int _sceneWidth = 0;
        private int _sceneHeight = 0;
        private int _cellSize = 0;

        private Queue<SpaceHashObject> _removableObjects = null;
        private List<SpaceHashObject> _objsList = null;
        private Dictionary<SceneNode, SpaceHashObject> _objectsDict = null;

        private Dictionary<int, HashSet<SpaceHashObject>> _buckets = null;

        #endregion Members

        public SpatialHashService(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IPartitioningService), this);
        }

        #region Public Methods      
  
        public void Init(params object[] args)            
        {
            if (null == args || 3 != args.Length)
                throw new ArgumentNullException("Input args must be scenewidth, sceneheight and cellsize");

            int scenewidth = (int)args[0];
            int sceneheight = (int)args[1];
            int cellsize = (int)args[2];

            _cols = scenewidth / cellsize;
            _rows = sceneheight / cellsize;

            _sceneWidth = scenewidth;
            _sceneHeight = sceneheight;
            _cellSize = cellsize;

            _removableObjects = new Queue<SpaceHashObject>();
            _objsList = new List<SpaceHashObject>();
            _objectsDict = new Dictionary<SceneNode, SpaceHashObject>();

            _buckets = new Dictionary<int, HashSet<SpaceHashObject>>(_cols * _rows);
            for (int i = 0; i != _cols * _rows; ++i)
            {
                _buckets.Add(i, new HashSet<SpaceHashObject>());
            }
        }

        public bool RegisterEntity(SceneNode entity, ref Point size)
        {
            var transf = entity.Components.Get<TransformComponent>();
            if (null != transf)
            {
                SpaceHashObject obj = null;
                if (!_objectsDict.TryGetValue(entity, out obj))
                {
                    obj = SpaceHashObject.Get();
                    obj.Owner = entity;
                    obj.Transform = transf;

                    _objsList.Add(obj);
                    _objectsDict.Add(entity, obj);
                }

                obj.Size = size;                
                obj.OldPosition = obj.Transform.World.Position;

                UpdateIdForObj(obj, ref obj.Transform.World.Position);
                if (0 != obj.BucketsObjIsIn.Count)
                {
                    int buckCount = obj.BucketsObjIsIn.Count;
                    for (int j = 0; j != buckCount; ++j)
                    {
                        var bucketID = obj.BucketsObjIsIn.ElementAt(j);
                        if (!_buckets[bucketID].Contains(obj))
                            _buckets[bucketID].Add(obj);
                    }
                }

                return true;
            }

            return false;
        }

        public override void Update(GameTime gameTime)
        {
            int count = _objsList.Count;
            int buckCount = 0;
            for (int i = 0; i != count; ++i)
            {
                var currObj = _objsList[i];
                if (!currObj.Owner.Active && !_removableObjects.Contains(currObj))
                {
                    _removableObjects.Enqueue(currObj);
                    continue;
                }

                float distSq = 0f;
                Vector2.DistanceSquared(ref currObj.OldPosition, ref currObj.Transform.World.Position, out distSq);
                if (distSq > _cellSize)
                {
                    UpdateIdForObj(currObj, ref currObj.OldPosition);
                    if (0 != currObj.BucketsObjIsIn.Count)
                    {
                        buckCount = currObj.BucketsObjIsIn.Count;
                        for (int j = 0; j != buckCount; ++j)
                        {
                            var bucketID = currObj.BucketsObjIsIn.ElementAt(j);
                            _buckets[bucketID].Remove(currObj);
                        }
                    }

                    UpdateIdForObj(currObj, ref currObj.Transform.World.Position);
                    if (0 != currObj.BucketsObjIsIn.Count)
                    {
                        buckCount = currObj.BucketsObjIsIn.Count;
                        for (int j = 0; j != buckCount; ++j)
                        {
                            var bucketID = currObj.BucketsObjIsIn.ElementAt(j);
                            if (!_buckets[bucketID].Contains(currObj))
                                _buckets[bucketID].Add(currObj);
                        }
                    }

                    currObj.OldPosition = currObj.Transform.World.Position;
                }
            }

            RemovalStep();
         //   Clear();
        }

        public List<SceneNode> GetNearby(SceneNode node)
        {
            SpaceHashObject obj = null;
            if (!_objectsDict.TryGetValue(node, out obj))
                return null;

           // UpdateIdForObj(obj, ref obj.Transform.World.Position);

            if (0 != obj.BucketsObjIsIn.Count)
            {
                obj.NearbyObjects.Clear();

                int count = obj.BucketsObjIsIn.Count;
                for (int i = 0; i != count; ++i)
                {
                    var bucketID = obj.BucketsObjIsIn.ElementAt(i);
                    var objsInBuckets = _buckets[bucketID];
                    foreach (var neighs in objsInBuckets)
                    {
                        if (neighs.Owner != node && neighs.Owner.Active)
                            obj.NearbyObjects.Add(neighs.Owner);
                    }

                }
                return obj.NearbyObjects;
            }
            return null;
        }
        
        #endregion Methods

        #region Private Methods

        private void Clear()
        {
            int count = _cols * _rows;
            for (int i = 0; i != count; ++i)
            {
                _buckets[i].Clear();
            }
        }

        private void UpdateIdForObj(SpaceHashObject obj, ref Vector2 pos)
        {
            float width = _sceneWidth / _cellSize;

            obj.BucketsObjIsIn.Clear();

            //TopLeft
            obj.CheckPoints[0].X = Math.Max(0f, pos.X - obj.Size.X);
            obj.CheckPoints[0].Y = Math.Max(0f, pos.Y - obj.Size.Y);
            AddBucket(ref obj.CheckPoints[0], ref width, ref obj.BucketsObjIsIn);

            //BottomRight
            obj.CheckPoints[1].X = Math.Min(_sceneWidth - _cellSize, pos.X + obj.Size.X);
            obj.CheckPoints[1].Y = Math.Min(_sceneHeight - _cellSize, pos.Y + obj.Size.Y);
            AddBucket(ref obj.CheckPoints[1], ref width, ref obj.BucketsObjIsIn);

            //TopRight
            obj.CheckPoints[2].X = obj.CheckPoints[1].X;
            obj.CheckPoints[2].Y = obj.CheckPoints[0].Y;
            AddBucket(ref obj.CheckPoints[2], ref width, ref obj.BucketsObjIsIn);

            //BottomLeft
            obj.CheckPoints[3].X = obj.CheckPoints[0].X;
            obj.CheckPoints[3].Y = obj.CheckPoints[1].Y;
            AddBucket(ref obj.CheckPoints[3], ref width, ref obj.BucketsObjIsIn);            
        }

        private void AddBucket(ref Vector2 vector, ref float width, ref HashSet<int> buckettoaddto)
        {
            int cellPosition = (int)((Math.Floor(vector.X / _cellSize)) +
                                      (Math.Floor(vector.Y / _cellSize)) * width);
            if (cellPosition > -1 && cellPosition < _buckets.Count && !buckettoaddto.Contains(cellPosition))
                buckettoaddto.Add(cellPosition);
        }

        private void RemovalStep()
        {
            while (0 != _removableObjects.Count)
            {
                var obj = _removableObjects.Dequeue();
                _objsList.Remove(obj);
                _objectsDict.Remove(obj.Owner);
            }
        }

        #endregion Private Methods
    }

    internal class SpaceHashObject
    {
        private SpaceHashObject() { }

        #region Properties

        public SceneNode Owner = null;
        public TransformComponent Transform = null;
        public Point Size = Point.Zero;        
        public Vector2 OldPosition = Vector2.Zero;

        public List<SceneNode> NearbyObjects = new List<SceneNode>();
        public Vector2[] CheckPoints = new Vector2[4];
        public HashSet<int> BucketsObjIsIn = new HashSet<int>();

        #endregion Properties

        public override int GetHashCode()
        {
            return null != this.Owner ? this.Owner.ID : 0;
        }

        public override bool Equals(object obj)
        {            
            var sho = obj as SpaceHashObject;
            if (null == sho && null == this.Owner) return true;
            if (null != this.Owner) return this.Owner.Equals(sho.Owner);
            return false;
        }

        /*****************************/

        private static Pool<SpaceHashObject> _pool = new Pool<SpaceHashObject>(20, (o => o.Owner.Active) );
        public static SpaceHashObject Get()
        {
            return _pool.New();
        }
    }
}
