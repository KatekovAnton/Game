using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using System.IO;

namespace PhysX_test2.UserInterface
{
    public static class UIContent
    {
        public static Textures Textures;

        public static void Init()
        {
            Textures = new Textures(@"Content\Images\");
            Textures.GetDirectly("`32`32");
            Textures.GetDirectly("`2`2");
            Textures.GetDirectly("`16`16");
            Textures.GetDirectly("`2`64");
            Textures.GetDirectly("`64`2");
            Textures.GetDirectly("`64`32");
            Textures.GetDirectly("`32`64");
          //  uint[] datat = new uint[16*16];
          //  UIContent.Textures["`16`16"].GetData<uint>(datat);
            //insert breakpoint here
        }

    }


    public abstract class AutoLoadingContent : Hashtable
    {
        public dynamic Empty;
        public string path = "";
        public List<string> Names = new List<string>();
        public AutoLoadingContent(string Path)
            : base()
        {
            path = GameConfiguration.AppPath + "\\" + Path;
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }

        public dynamic GetDirectly(string _name) { dynamic f = this[_name]; return this[_name]; }

        public dynamic this[string key]
        {
            get
            {
                if (base[key] == null)
                { Add(key, Load(key)); Names.Add(key); }
                return base[key];
            }
            set
            {
                base[key] = value;
            }
        }

        public void Dispose()
        { foreach (string s in Names) this[s].Dispose(); this.Clear(); Names.Clear(); }

        public abstract dynamic Load(string _name);
    }

    public class Textures : AutoLoadingContent
    {
        public Textures(string path)
            : base(path)
        {
            Empty = new RT(32, 32, "Empty");
            Program.game.GraphicsDevice.SetRenderTarget(Empty);
            Program.game._spriteBatch.Begin();
            Program.game.GraphicsDevice.Clear(Color.Yellow);
            Program.game._spriteBatch.DrawString(Content.Fonts._font1, "!", new Vector2(18, 3), Color.Black);
            Program.game._spriteBatch.End();
            Program.game.GraphicsDevice.SetRenderTarget(null);
            Add("Empty", Empty);
        }

        public void Save(string what)
        {
            this[what].Save(path + what + ".png");
        }

        public override dynamic Load(string _name)
        {
            RT result = Empty;
            if (_name.IndexOf('`') == 0)
            {
                string[] parameters = _name.Substring(1).Split('`');
                result = new RT(Convert.ToInt32(parameters[0]), Convert.ToInt32(parameters[1]), Color.White, _name);
            }
            else
                try
                {
                    System.IO.StreamReader stream = new System.IO.StreamReader(path + _name + ".png");
                    result = new RT(Texture2D.FromStream(Program.game.GraphicsDevice, stream.BaseStream), _name);
                    stream.Close();
                }
                catch (Exception ee)
                {
                     try { result = new RT(Program.game.Content.Load<Texture2D>(_name), _name); }
                     catch 
                     {
                       // insert your pack loading here
                         ExcLog.LogException(ee);
                     }
                }

            return result;
        }

    }


    public class RT : RenderTarget2D
    {
        public Vector2 Origin = Vector2.Zero;
        public int Resource_count = 0;

        public string name;

        public RT(int width, int heigth, string name)
            : base(Program.game.GraphicsDevice, width, heigth, false, SurfaceFormat.Bgr565, DepthFormat.Depth16, 0, RenderTargetUsage.DiscardContents)
        {
            Origin = new Vector2(width / 2, heigth / 2); Resource_count++;
            this.name = name;
        }

        public RT(int width, int heigth, Color color, string name)
            : base(Program.game.GraphicsDevice, width, heigth, false, SurfaceFormat.Bgr565, DepthFormat.Depth16, 0, RenderTargetUsage.DiscardContents)
        {
            Program.game.GraphicsDevice.SetRenderTarget(this);
            Program.game.GraphicsDevice.Clear(color);
            Program.game.GraphicsDevice.SetRenderTarget(null);
            Origin = new Vector2(width / 2, heigth / 2);
            Resource_count++;
            this.name = name;
        }

        public RT(Texture2D texture2d, string name)
            : base(Program.game.GraphicsDevice, texture2d.Width, texture2d.Height, true, SurfaceFormat.Alpha8, DepthFormat.None, 0, RenderTargetUsage.PlatformContents)
        {
            
            Program.game.GraphicsDevice.SetRenderTarget(this);
            Program.game.GraphicsDevice.Clear(Color.FromNonPremultiplied(0, 0, 0, 0));
            Program.game._spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, DepthStencilState.None, RasterizerState.CullNone);
            Program.game._spriteBatch.Draw(texture2d, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            Program.game._spriteBatch.End();
            // Program.game.GraphicsDevice.SetRenderTarget(null);

            //     Draw_in(this, Vector2.Zero, Color.White, 0, new Vector2(1, 1), SpriteEffects.None, BlendState.Opaque);
            Origin = new Vector2(texture2d.Width / 2, texture2d.Height / 2);
            Resource_count++;
            this.name = name;
        }

        public void Save(string filename)
        {
            StreamWriter sw = new StreamWriter(filename);
            this.SaveAsPng(sw.BaseStream, Width, Height);
            sw.Close();
        }

        public new void Dispose()
        {
            if (Resource_count == 0)
                base.Dispose();
            // else Resource_count--;
        }
        public override string ToString()
        {
            return name;
        }

        public void Draw_in(RT Sprite, Vector2 Position, Color Color, float Rotation, Vector2 scale, SpriteEffects SpriteEffects, BlendState BlendState)
        {
            Program.game.GraphicsDevice.SetRenderTarget(this);
            Program.game._spriteBatch.Begin(SpriteSortMode.Immediate, BlendState);
            Program.game._spriteBatch.Draw(Sprite, Position, null, Color, Rotation, Sprite.Origin, scale, SpriteEffects, 0);
            Program.game._spriteBatch.End();
        }

    }

}