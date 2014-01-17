using System;
using Microsoft.Xna.Framework;

namespace GlyphEngine.Services
{
    public interface IPartitioningService : IGameComponent
    {
        void Init(params object[] args);
        System.Collections.Generic.List<GlyphEngine.SceneGraph.ISceneNode> GetNearby(GlyphEngine.SceneGraph.ISceneNode node);
        bool RegisterEntity(GlyphEngine.SceneGraph.ISceneNode entity, ref Microsoft.Xna.Framework.Point size);        
        void Update(Microsoft.Xna.Framework.GameTime gameTime);
    }
}
