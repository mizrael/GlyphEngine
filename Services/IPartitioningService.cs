using System;
using Microsoft.Xna.Framework;

namespace GlyphEngine.Services
{
    public interface IPartitioningService : IGameComponent
    {
        void Init(params object[] args);
        System.Collections.Generic.List<GlyphEngine.SceneGraph.SceneNode> GetNearby(GlyphEngine.SceneGraph.SceneNode node);
        bool RegisterEntity(GlyphEngine.SceneGraph.SceneNode entity, ref Microsoft.Xna.Framework.Point size);        
        void Update(Microsoft.Xna.Framework.GameTime gameTime);
    }
}
