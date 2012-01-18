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
           public string Text = "";
           public Vector2 Position;
           public Vector2 Size;
           public Control Parent = null;

           public abstract void Draw();
           public abstract void Update();
           public abstract void Dispose();
           public abstract void UnLoad();
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

           public void Add(Control item) { ChildControls.Add(item); item.Parent = this; }

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

       public class TextBox : UserControl, IKeyboardUser, IAllKeys
       {
           public bool Capture = false;
           public bool GlobalUser { set { } get { return false; } }
           public bool IsKeyboardCaptured { set { Capture = value; } get { return Capture; } }
           List<HotKey> _hotkeys = new List<HotKey>();
           public List<HotKey> hotkeys { get { return _hotkeys; } }

           public SpriteFont font;
           bool cur_vis = false;
           int cp;
           public int cur_pos
           {
               set 
               {
                   if (value >= 0 && value <= Text.Length)
                       cp = value;
                   else
                   { if (value > Text.Length) cp = Text.Length; else cp = 0; }
                   cur_draw_pos = font.MeasureString(Text.Substring(0, cur_pos)).X + 1;
               }
               get 
               {
                  return cp;
               }

           }
           byte i = 0;

           public float cur_draw_pos;

           public TextBox(string init_text, Vector2 position, Vector2 size, List<HotKey> _hotkeys, SpriteFont Font, Color Color)
           {
               font = Font;
               this.Color = Color;
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
               
               this._hotkeys = _hotkeys;
               cur_pos = init_text.Length;
           }
           bool InsertMode = false;
           public void KeyPress()
           {
               if (KeyboardManager.key_buffer.Length > 0)
               {
                   if (!InsertMode || cp == Text.Length)
                       Text = Text.Insert(cp, KeyboardManager.key_buffer);
                   else
                   {
                       Text = Text.Remove(cp, KeyboardManager.key_buffer.Length).Insert(cp, KeyboardManager.key_buffer);
                   }
                   cur_pos += KeyboardManager.key_buffer.Length;
                   KeyboardManager.key_buffer = "";
               }
           }

           void Left()
           {
                cur_pos--;
           }

           void Right()
           {
                cur_pos++;
           }

           void BackSpace()
           { 
               if (cp>0)
               {
                  Text = Text.Remove(cur_pos - 1, 1); cur_pos--;
               }
           }

           void Home()
           {
               cur_pos = 0;
           }

           void End()
           {
               cur_pos = Text.Length;
           }

           void Insert()
           {
               InsertMode = !InsertMode;
           }

           void Delete()
           {
               if (cp < Text.Length)
               {
                   Text = Text.Remove(cur_pos, 1); 
               }
           }

           public override void Draw()
           {
               Program.game._spriteBatch.Draw(UIContent.Textures["`16`16"], Position, null, Color, 0, Vector2.Zero, Size, SpriteEffects.None, 0);
               
               Program.game._spriteBatch.DrawString(font, Text, Position + new Vector2(2,2),Color.White);
               if (Capture)
                   if (cur_vis) Program.game._spriteBatch.Draw(UIContent.Textures["`2`16"], Position + new Vector2(cur_draw_pos, 0), null, Color, 0, Vector2.Zero, new Vector2(InsertMode ? 3:1,1), SpriteEffects.None, 0);

               base.Draw();
           }

           public override void Update()
           {
               i++; if (i == 10)
               {cur_vis = !cur_vis; i = 0;}
               base.Draw();
           }
       }

       public class Label : Control
       {
           public SpriteFont Font;
           public Color Color;
           public Label(Color color)
           { Color = color; }

           public override void Draw()
           {
               Program.game._spriteBatch.DrawString(Font, Text, Position, Color);
           }

           public override void Update()
           {      }

           public override void UnLoad()
           { }

           public override void Dispose()
           { }
           
       }


     /*  public class Panel : Control
       {
           public List<Control> controls = new List<Control>();
           public RadioButton current_radio_button = null;
           public bool can_hiden = true;
           public Panel(string text, Panel parent_)
               : base(text, new Vector2(128, 32), parent_)
           {
               text_size = Fonts.font2.MeasureString(text) - new Vector2(0, 10);
               if (text == "") text_size = Vector2.Zero;
               text_position_add = new Vector2((base.size.X - text_size.X) / 2, base.size.Y - 37);
           }

           public void Add(Control item)
           {
               controls.Add(item);
               item.parent = this;
               hidden = false;
           }

           public bool hidden_;
           public bool hidden
           {
               set
               {
                   hidden_ = value;
                   if (value)
                   { size = new Vector2(128, 18); }
                   else
                   {
                       Vector2 s = new Vector2(128, text_size.Y + 3);
                       foreach (Control c in controls) { s.Y += c.size.Y + 3; }
                       size = s;
                   }

               }
               get { return hidden_; }
           }

           public override void Update(Vector2 pos)
           {
               this.pos = pos;
               if (can_hiden)
                   MouseOver = E.limits(A._g.mouse_pos, pos, new Vector2(size.X, 10));
               else MouseOver = E.limits(A._g.mouse_pos, pos, size * 10);

               if (MouseOver)
                   E.perehod(ref color, Color.FromNonPremultiplied(255, 255, 255, 200), 0.9);
               else E.perehod(ref color, Color.FromNonPremultiplied(200, 200, 200, 200), 0.9);

               if (!hidden)
               {
                   pos += new Vector2(0, text_size.Y);
                   foreach (Control c in controls)
                   {
                       c.Update(pos);
                       pos.Y += c.size.Y + 3;
                   }
               }

           }

           Vector2 pos;

           public override void OnMouseUp()
           {
               if (can_hiden) if (MouseOver)
                       hidden = !hidden;
               if (!hidden)
               {
                   pos += new Vector2(0, text_size.Y);
                   foreach (Control c in controls)
                   {
                       if (typeof(Panel) == c.GetType()) c.OnMouseUp();
                       else
                           if (c.MouseOver) c.OnMouseUp();
                       pos.Y += c.size.Y + 3;
                   }
               }
           }

           public override void Draw(Vector2 pos)
           {
               A._g.sprite_batch.Draw(A._g.Textures.button_texture, pos - new Vector2(5, 5), null, color, 0, Vector2.Zero, size / new Vector2(108, 32), SpriteEffects.None, 0);
               A._g.sprite_batch.DrawString(Fonts.font2, text, pos + text_position_add, Color.LightGray);

               if (!hidden)
               {
                   pos += new Vector2(0, text_size.Y);

                   foreach (Control c in controls)
                   {
                       c.Draw(pos);
                       pos.Y += c.size.Y + 3;
                   }
               }
           }

       }

       public class Texture_Panel : Control
       {
           public List<Control> controls = new List<Control>();
           public new Vector2 size
           {
               get
               {
                   Vector2 s = new Vector2(5, 40);
                   foreach (Control c in controls)
                   { s.X += c.size.X + 3; }
                   return s;
               }
           }

           public Texture_Panel()
               : base("", new Vector2(128, 32), null)
           { }

           public override void Update(Vector2 pos)
           {
               MouseOver = E.limits(A._g.mouse_pos, pos, size);
               foreach (Control c in controls)
               {
                   c.Update(pos);
                   pos.X += c.size.X + 3;
               }
           }

           public void OnMouseUp(Vector2 pos)
           {
               foreach (Control c in controls)
               {
                   if (c.MouseOver) c.OnMouseUp();
                   pos.X += c.size.X + 3;
               }
           }

           public override void OnMouseUp() { }

           public override void Draw(Vector2 pos)
           {
               A._g.sprite_batch.Draw(A._g.Textures.instrument_texture, pos - new Vector2(5, 5), null, Color.White, 0, Vector2.Zero, new Vector2(size.X / 128, 1.2f), SpriteEffects.None, 0);
               A._g.sprite_batch.DrawString(Fonts.font2, text, pos + text_position_add, Color.Black);
               pos += new Vector2(0, text_size.Y);

               foreach (Control c in controls)
               {
                   c.Draw(pos);
                   pos.X += c.size.X + 3;
               }
           }
       }




       abstract public class Control
       {
           public Panel parent = null;
           public string text;
           public Vector2 text_size;
           public bool MouseOver = false;
           public Vector2 size = Vector2.Zero;
           public dynamic Click = null;
           public Vector2 text_position_add = new Vector2(5, 2);
           public Color color = Color.White;
           public Color text_color, text_must_color;

           public Control(string text, Vector2 size, Panel parent)
           {
               this.text = text;
               this.size = size;
               if (parent != null)
               {
                   this.parent = parent;
                   parent.Add(this);
               }
           }

           public Control(string text, Vector2 size)
           {
               this.text = text;
               this.size = size;
           }

           public abstract void OnMouseUp();
           public abstract void Draw(Vector2 position);
           public abstract void Update(Vector2 position);
           public override string ToString()
           { return text; }
       }

       public class Button : Control
       {
           public Button(string text, E._void func)
               : base(text, new Vector2(128, 32))
           {
               Click = func;
               text_color = Color.Black;
               text_must_color = Color.Black;
               text_size = Fonts.font1.MeasureString(text);
               text_position_add = new Vector2((size.X - text_size.X) / 2, size.Y - 30);
           }

           public Button(string text, E._void_obj func)
               : base(text, new Vector2(128, 32))
           {
               Click = func;
               text_color = Color.Black;
               text_must_color = Color.Black;
               text_size = Fonts.font1.MeasureString(text);
               text_position_add = new Vector2((size.X - text_size.X) / 2, size.Y - 30);
           }

           public override void OnMouseUp()
           {
               color = Color.White; if (Click != null)
                   Click();
           }

           public override void Update(Vector2 position)
           {
               MouseOver = E.limits(A._g.mouse_pos, position, size);
               if (MouseOver) E.perehod(ref color, Color.FromNonPremultiplied(255, 255, 255, 200), 0.9); else E.perehod(ref color, Color.FromNonPremultiplied(200, 200, 200, 200), 0.9);

               E.perehod(ref text_color, text_must_color, 0.9f);
           }

           public override void Draw(Vector2 position)
           {
               A._g.sprite_batch.Draw(A._g.Textures.button_texture, position, color);
               A._g.sprite_batch.DrawString(Fonts.font1, text, position + text_position_add, Color.Black);
           }
       }


       public class Ins_Tex : Button
       {
           public Color must_color; public int tex_id = 0;
           public Ins_Tex(int tex_id)
               : base("", Select_Instrument_Texture)
           {
               size = new Vector2(32, 32);
               this.tex_id = tex_id;
               must_color = Color.FromNonPremultiplied(200, 200, 200, 200);
           }

           static public void Select_Instrument_Texture()
           {
               foreach (Ins_Tex ins in A._g.t_panel.controls)
               {
                   ins.must_color = Color.FromNonPremultiplied(200, 200, 200, 200);
                   if (ins.MouseOver) { A._g.current_tex = ins.tex_id; ins.must_color = Color.White; }
               }
           }

           public override void Draw(Vector2 position)
           {
               A._g.sprite_batch.Draw(A._g.Textures.b_tex, position, color);
               A._g.sprite_batch.Draw(A._g.Textures.tex[tex_id], position + new Vector2(2, 2), null, Color.White, 0, Vector2.Zero, 28.0f / (A._g.Textures.tex[tex_id].Width), SpriteEffects.None, 0);
           }
       }



       public class Instrument : RadioButton
       {
           E._void_obj Act3;
           public Instrument(string text, E._void_obj Act3, Panel parent)
               : base(text, parent)
           { this.Act3 = Act3; parent.Add(this); }

           public void Act(Vector3 vect) { if (Act3 != null) Act3(vect); }

           public override void Draw(Vector2 position)
           {
               base.Draw(position);
               A._g.sprite_batch.Draw(A._g.Textures.dot, position + new Vector2(5, 10), text_color);
           }
           public void Select_()
           {
               A._g.current_instrument = (Instrument)A._g.Instruments.current_radio_button;
           }
       }

       public class RadioButton : Button
       {

           public RadioButton(string text, Panel parent)
               : base(text, Select_)
           {
               this.parent = parent;
               text_color = Color.Black;
               text_must_color = Color.Black;
           }

           static public void Select_(dynamic obj)
           {
               foreach (Control ins in obj.parent.controls)
               {
                   if (ins.GetType() == obj.GetType())
                   {
                       ((RadioButton)ins).text_must_color = Color.Black;
                   }
               }
               obj.parent.current_radio_button = obj; obj.text_must_color = Color.White;
               try { obj.Select_(); }
               catch { }
           }

           public override void OnMouseUp()
           { color = Color.White; if (Click != null) Click(this); }

           public override void Draw(Vector2 position)
           {
               A._g.sprite_batch.Draw(A._g.Textures.instrument_texture, position, color);
               A._g.sprite_batch.DrawString(Fonts.font1, text, position + text_position_add, text_color);
           }
       }


       public class CheckBox : Button
       {
           public E._void OnChanged = null;
           public bool Checked_ = false;
           public bool Checked
           {
               set { Checked_ = value; text_must_color = Checked_ ? Color.White : Color.Black; if (OnChanged != null) OnChanged(); }
               get { return Checked_; }
           }

           public CheckBox(string text)
               : base(text, Select_)
           { }

           public CheckBox(string text, E._void OnChanged)
               : base(text, Select_)
           {
               this.OnChanged = OnChanged;
           }

           static public void Select_(dynamic obj)
           { obj.Checked = !obj.Checked; }

           public override void OnMouseUp()
           { color = Color.White; if (Click != null) Click(this); }

           public override void Draw(Vector2 position)
           {
               A._g.sprite_batch.Draw(A._g.Textures.checkbox_texture, position, color);
               A._g.sprite_batch.DrawString(Fonts.font1, text, position + text_position_add, text_color);
           }

       }


       public class Trackbar : Control
       {
           public bool show_value = true;
           public float min = 0, max = 100; public int intvalue;
           public float value_ = 0;
           public float Value { set { if (value < min) value_ = min; else if (value > max) value_ = max; else value_ = value; intvalue = (int)Math.Round(value_); } get { return value_; } }

           public List<string> values;

           public Trackbar(string text, string[] values, int value)
               : base(text, new Vector2(128, 20))
           {
               this.min = 0; this.max = values.Length - 1; this.Value = value; this.values = values.ToList();
               text_position_add = new Vector2(10, 5);
               text_must_color = Color.Black;
           }

           public Trackbar(string[] values, int value, E._void func)
               : base("", new Vector2(128, 20))
           {
               Click = func;
               this.min = 0; this.max = values.Length - 1; this.Value = value; this.values = values.ToList();
               text_position_add = new Vector2(10, 5);
               text_must_color = Color.Black;
           }


           public Trackbar(string text, float min, float max, float value, bool ShowValue)
               : base(text, new Vector2(128, 20))
           {
               this.min = min; this.max = max; this.Value = value; show_value = ShowValue;
               text_size = Fonts.font1.MeasureString(text);
               text_must_color = Color.Black;
               text_position_add = show_value ? new Vector2(10, 5) : new Vector2((size.X - text_size.X) / 2, 5);
           }

           public void Addval(string val)
           { values.Add(val); max = values.Count - 1; }

           public void DeleteVal(string val)
           { values.Remove(val); max = values.Count - 1; }

           public override void OnMouseUp()
           {
               if (Click != null) Click();
           }

           public override void Update(Vector2 position)
           {
               MouseOver = E.limits(A._g.mouse_pos, position - new Vector2(2, 2), size + new Vector2(2, 2));

               if (MouseOver)
               {
                   E.perehod(ref color, Color.FromNonPremultiplied(255, 255, 255, 200), 0.9);
                   if (A._g.mouse.LeftButton == ButtonState.Pressed)
                   {
                       E.perehod(ref text_color, Color.FromNonPremultiplied(255, 255, 255, text_must_color.A), 0.9f);
                       float x = A._g.mouse_pos.X - position.X;
                       if (x < 3) value_ = min;
                       else
                           if (x > size.X - 3) value_ = max;
                           else
                               value_ = min + x * max / size.X;
                   }
                   else
                       E.perehod(ref text_color, text_must_color, 0.9f);
               }
               else
               {
                   E.perehod(ref color, Color.FromNonPremultiplied(200, 200, 200, 200), 0.9);
                   E.perehod(ref text_color, text_must_color, 0.9f);
               }


               int tmp = intvalue;
               intvalue = (int)Math.Round(value_);
               if (tmp != intvalue)
                   if (Click != null) Click();
           }

           public override void Draw(Vector2 position)
           {
               A._g.sprite_batch.Draw(A._g.Textures.b_tex, position + new Vector2(0, 4), null, Color.White, 0, Vector2.Zero, new Vector2(4, 0.1f), SpriteEffects.None, 0);
               A._g.sprite_batch.Draw(A._g.Textures.checkbox_texture, position, null, color, 0, Vector2.Zero, size / new Vector2(128, 32), SpriteEffects.None, 0);
               A._g.sprite_batch.Draw(A._g.Textures.dot, position + new Vector2((value_ - min) / max * size.X / 1.15f + 5, 1), Color.White);

               if (values == null)
                   A._g.sprite_batch.DrawString(Fonts.font2, text + (show_value ? ": " + value_.ToString("#0.000", System.Globalization.NumberFormatInfo.InvariantInfo) : ""), position + text_position_add, text_color);
               else
                   A._g.sprite_batch.DrawString(Fonts.font2, values[intvalue], position + text_position_add, text_color);
           }
       }
       */
   }
}
