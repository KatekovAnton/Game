using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace PhysX_test2.UserInterface
{
   public static class Controls
   {
       public abstract class Control
       {
           public Color Color;
           public string Text { set; get; }
           public Vector2 Position;
           public Vector2 Size;
           public Control Parent = null;
           public bool visible = true;

           public virtual void Draw() { }
           public virtual void Update() { }
           public virtual void Dispose() { }
           public virtual void UnLoad() { }
       }

       public interface ISomeInterface 
       {
           void Init();
           void Draw();
           void Update();
           void Dispose();
           void UnLoad();
       }

       public class UserControl : Control
       {
           public List<Control> ChildControls = new List<Control>();

           public override void Draw()
           {
               foreach (Control Control in ChildControls)
                   Control.Draw();
           }

           public void Add(Control item)
           {
               ChildControls.Add(item); 
               item.Parent = this;
           }

           public override void Update()
           {
               foreach (Control Control in ChildControls)
                   Control.Update();
           }

           public override void Dispose() 
           {
               // add disposing code here 
           }
           public override void UnLoad() 
           {
               // add unloading code here
           }
       }
       
       class TextBoxCursor
       {
           private TextBox tb;
           public int cp;
           public int pos
           {
               set
               {
                   if (value >= 0 && value <= tb.Text.Length)
                       cp = value;
                   else
                   {
                       if (value > tb.Text.Length) 
                           cp = tb.Text.Length; 
                       else 
                           cp = 0;
                   }
                   cur_draw_pos = tb.font.MeasureString(tb.Text.Substring(0, cp)).X + 2;
               }
               get
               {
                   if (cp > tb.Text.Length) 
                       cp = tb.Text.Length;
                   else 
                       if (cp<0) 
                           cp = 0;
                   return cp;
               }
           }
           public float cur_draw_pos;
           public TextBoxCursor(TextBox tb)
           { 
               this.tb = tb;
               pos = tb.Text.Length; 
           }
           


       }

       class TextBoxSelection
       {
           public TextBoxCursor Start, End;
           TextBox _tb;
           public TextBoxSelection(TextBox tb)
           {
               this._tb = tb;
               Start = new TextBoxCursor(tb);
               End = new TextBoxCursor(tb);
           }

           public int Length
           {
               get
               {
                   return Math.Abs(End.pos - Start.pos);
               }
           }
       }


       public class TextBox : Control, IKeyboardUser, IAllKeys
       {
           public string SelectedText
           {
               set
               {
                   bool tmp = _insertMode;
                   _insertMode = false;
                   Text = Text.Remove(_sel.Start.pos, _sel.Length);
                   _sel_vis = value.Length > 0;
                   Text = Text.Insert(_cur.pos, value);
                   _cur.pos += value.Length;
                   _insertMode = tmp;
               }
               get
               {
                   int length = _sel.Length;
                   _sel_vis = length != 0;
                   if (length == 0)
                       return "";
                   return Text.Substring(_sel.Start.pos, _sel.Length);
               }
           }

           public Color TextColor;
           public bool Capture = false;
           public bool GlobalUser { set { } get { return false; } }
           public bool IsKeyboardCaptured { set { Capture = value; } get { return Capture; } }
           private List<HotKey> _hotkeys = new List<HotKey>();
           public List<HotKey> hotkeys { get { return _hotkeys; } }
           private Image background_image;
           CashedTexture2D _texture;

           public SpriteFont font;

           private bool _sel_vis = false;
           private bool _cur_vis = false;
           private bool _insertMode = false;
           private byte _i = 0;
           private TextBoxSelection _sel;
           private TextBoxCursor _cur;

           public TextBox(string init_text, Vector2 position, Vector2 size, List<HotKey> _hotkeys, SpriteFont Font, Image background)
           {
               background_image = background;
               font = Font;
               Color = GColors.CTextBack;
               TextColor = GColors.CText;
               Size = size;
               Text = init_text;
               Position = position;
               _hotkeys.Add(new HotKey(new Keys[] { Keys.Left }, Left));
               _hotkeys.Add(new HotKey(new Keys[] { Keys.Right }, Right));
               _hotkeys.Add(new HotKey(new Keys[] { Keys.Back }, BackSpace));
               _hotkeys.Add(new HotKey(new Keys[] { Keys.Delete }, Delete));
               _hotkeys.Add(new HotKey(new Keys[] { Keys.Home }, Home));
               _hotkeys.Add(new HotKey(new Keys[] { Keys.End }, End));
               _hotkeys.Add(new HotKey(new Keys[] { Keys.Insert }, Insert));
               _hotkeys.Add(new HotKey(new Keys[] { Keys.LeftControl, Keys.C }, Copy));
               _hotkeys.Add(new HotKey(new Keys[] { Keys.LeftControl, Keys.V }, Paste));
               _hotkeys.Add(new HotKey(new Keys[] { Keys.LeftControl, Keys.X }, Cut));

               this._hotkeys = _hotkeys;

               _cur = new TextBoxCursor(this);
               _sel = new TextBoxSelection(this);

               _texture = new PackTexture("tex_w2x8\0", true);
           }

           public void KeyPress()
           {
               if (KeyboardManager.Ctrl)
               { 
                   KeyboardManager.key_buffer = ""; 
               }
               else
                   if (KeyboardManager.key_buffer.Length > 0)
                   {
                       if (!_insertMode || _cur.pos == Text.Length)
                           Text = Text.Insert(_cur.pos, KeyboardManager.key_buffer);
                       else
                       {
                           if (KeyboardManager.key_buffer.Length + _cur.pos < Text.Length)
                               Text = Text.Remove(_cur.pos, KeyboardManager.key_buffer.Length).Insert(_cur.pos, KeyboardManager.key_buffer);
                           else
                               Text = Text.Remove(_cur.pos).Insert(_cur.pos, KeyboardManager.key_buffer);
                       }
                       _cur.pos += KeyboardManager.key_buffer.Length;
                       KeyboardManager.key_buffer = "";
                   }
           }

           void Left()
           {
               if (KeyboardManager.Shift)
               {
                   bool f2 = _sel.Start.pos == _cur.pos;
                   bool f3 = _sel.End.pos == _cur.pos;
                   bool f1 = (_sel.End.pos == _sel.Start.pos) || (!f3 && !f2);

                   if (f1)
                   {
                       _sel.End.pos = _cur.pos--;
                       _sel.Start.pos = _cur.pos;
                   }
                   else
                   {
                       if (f2) 
                           _sel.Start.pos = --_cur.pos;
                       if (f3) 
                           _sel.End.pos = --_cur.pos;
                   }
               }
               else
                   _cur.pos--;
           }

           void Right()
           {
               bool f2 = _sel.Start.pos == _cur.pos;
               bool f3 = _sel.End.pos == _cur.pos;
               bool f1 = (_sel.End.pos == _sel.Start.pos) || (!f3 && !f2);

               if (KeyboardManager.Shift)
               {
                   if (f1)
                   {
                       _sel.Start.pos = _cur.pos++;
                       _sel.End.pos = _cur.pos;
                   }
                   else
                   {
                       if (f2)
                           _sel.Start.pos = ++_cur.pos;
                       if (f3) 
                           _sel.End.pos = ++_cur.pos;
                   }
               }
               else
                   _cur.pos++;
           }

           void BackSpace()
           {
               if (_cur.pos > 0)
                   Text = Text.Remove(--_cur.pos, 1);
           }

           void Home()
           {
               _cur.pos = 0;
               if (KeyboardManager.Shift)
                   _sel.End.pos = _cur.pos;
           }

           void End()
           {
               _cur.pos = Text.Length;
               if (KeyboardManager.Shift)
                   _sel.Start.pos = _cur.pos;
           }

           void Insert()
           {
               if (KeyboardManager.Ctrl)
                   Copy();
               else
                   if (KeyboardManager.Shift)
                       Paste();
                   else
                       _insertMode = !_insertMode;
           }

           void Copy()
           {
               KeyboardManager.Clipboard = SelectedText;
           }

           void Cut()
           {
               Copy();
               SelectedText = "";
               _sel.End.pos = _sel.Start.pos;
           }

           void Paste()
           {
               SelectedText = KeyboardManager.Clipboard;
               _sel.Start.cp = _cur.cp;
               _sel.End.cp = _cur.cp;
           }

           void Delete()
           {
               if (_sel_vis)
               {
                   SelectedText = "";
                   _sel.End.pos = _sel.Start.pos;
               }
               else
                   if (_cur.pos < Text.Length)
                   {
                       Text = Text.Remove(_cur.pos, 1);
                       if (_insertMode)
                           Text = Text.Insert(_cur.pos, " ");
                   }

           }

           public override void Draw()
           {
               if (background_image != null) background_image.Draw();
               //Program.game._spriteBatch.Draw(_texture._texture, Position, null, Color, 0, Vector2.Zero, Size, SpriteEffects.None, 0);

               Program.game._spriteBatch.DrawString(font, Text, Position + new Vector2(2, 2), TextColor);
               if (Capture)
               {
                   if (_cur_vis) 
                       Program.game._spriteBatch.Draw(_texture._texture, Position + new Vector2(_cur.cur_draw_pos, 0), null, TextColor, 0, Vector2.Zero, new Vector2(_insertMode ? 3 : 0.5f, 2), SpriteEffects.None, 0);
                   if (_sel_vis) 
                       Program.game._spriteBatch.Draw(_texture._texture, Position + new Vector2(_sel.Start.cur_draw_pos, 1), null, GColors.CHighLightText, 0, Vector2.Zero, new Vector2((_sel.End.cur_draw_pos - _sel.Start.cur_draw_pos) / 2, 1.8f), SpriteEffects.None, 0);
               }
               // base.Draw();
           }

           public override void Update()
           {
               _sel_vis = _sel.Start.pos != _sel.End.pos;

               _i++; 
               if (_i >= Program.game._engine.FPSCounter.FramesPerSecond / 10 + 1)
               { 
                   _cur_vis = !_cur_vis; 
                   _i = 0; 
               }
           }

           ~TextBox()
           {
               _texture.Release();
           }
       }

       public static TextBox CreateTextBox(string init_text, Vector2 position, Vector2 size, List<HotKey> _hotkeys, SpriteFont Font, Image background = null)
       {
           TextBox tb = new TextBox(init_text, position, size, _hotkeys, Font, background);
           return tb;
       }

       public static Image CreateImage(Vector2 position, CashedTexture2D _texture, Color color, Vector2 size)
       {
           Controls.Image img2 = new Controls.Image(position, color, _texture);
           img2.Size = size;
           return img2;
       }

       public static Image CreateImage(Vector2 position, CashedTexture2D _texture)
       {
          return new Controls.Image(position, Color.White, _texture);
       }


       public static TextBox CreateTextBox(Vector2 position, Vector2 size, List<HotKey> _hotkeys)
       {
           Image img = CreateImage(position, new PackTexture("tex_w2x2", true), GColors.CTextBack, size);
           return new TextBox("", position, size, _hotkeys, Content.Fonts._font1 , img); ;
       }

       public static Label CreateLabel(Color color, Vector2 position, SpriteFont Font, string init_text = "")
       {
           Label label = new Label(color, init_text, position, Font);
           return label;
       }

       public class Image : Control
       {
           public CashedTexture2D _texture;
           public Vector2 Size;
           public Image(Vector2 position, Color color, CashedTexture2D tr)
           {
               tr.Retain();
               this.Position = position; 
               this.Color = color; 
               _texture = tr;
               this.Size = new Vector2(tr._texture.Width, tr._texture.Height);
           }

           public override void Draw()
           {
               Program.game._spriteBatch.Draw(_texture._texture,
                   new Rectangle((int)base.Position.X, (int)base.Position.Y, (int)(base.Position.X + Size.X), (int)(base.Position.Y + Size.Y)),
                   null,
                   Color,
                   0,
                   Vector2.Zero,
                   SpriteEffects.None,
                   0);
           }

           ~Image()
           {
               _texture.Release();
           }
       }

       public class Label : Control
       {
           public SpriteFont Font;
           public Label(Color color, string init_text, Vector2 position, SpriteFont font)
           {
               Color = color; Text = init_text; Position = position; Font = font;
           }

           public override void Draw()
           {
               if(visible)
                    Program.game._spriteBatch.DrawString(Font, Text, Position, Color);
           }
       }
   }
}
