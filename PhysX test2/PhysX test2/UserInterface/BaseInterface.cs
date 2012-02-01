using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PhysX_test2.UserInterface
{
    public class GameInterface : Controls.UserControl, Controls.ISomeInterface
    {
        public Controls.TextBox debug_textbox;
        
        public Controls.Label l_fps;
        public Controls.Label l_Frame_time;
        public Controls.Label l_Visible_objects_count;
        public Controls.Label l_Recalulcalated_objects_count;
        public Controls.Label l_Character_angle;
        public Controls.Label l_Character_name;

        public Controls.UserControl debug_panel;

        public List<string> command_buffer = new List<string>();
        public int current_command = 0;

        // лучше не использовать конструкторы для контролов, а юзать вложенные методы Controls.CreateControl()

        public void Init() 
        {
            
            List<HotKey> debug_textbox_hotkeys = new List<HotKey>();
            debug_textbox_hotkeys.Add(new HotKey(new Keys[] { Keys.Enter },onDebugTextboxEnter));
            debug_textbox_hotkeys.Add(new HotKey(new Keys[] { Keys.Escape }, onDebugTextboxEscape));
            debug_textbox_hotkeys.Add(new HotKey(new Keys[] { Keys.Up }, onDebugTextboxUp));
            debug_textbox_hotkeys.Add(new HotKey(new Keys[] { Keys.Down }, onDebugTextboxDown));
           // string init_text ="debug console textbox";
            debug_textbox = Controls.CreateTextBox(new Vector2(0, GameConfiguration.ScreenResolution.Y - Content.Fonts._font1.MeasureString("A").Y - 5), new Vector2(1000, 1), debug_textbox_hotkeys);
           // command_buffer.Add(init_text);

            KeyboardManager.Manager.AddKeyboardUser(debug_textbox);

            string out_str = "";
            out_str += "                           FPS:\n";
            out_str += "                    Frame time:\n";
            out_str += "         Visible objects count:\n";
            out_str += "  Recalulcalated objects count:\n";
            out_str += "               Character angle:\n\n" ;
            out_str += "       \'Escape\'  - Show MainMenu ( or exit from something else )\n";
            out_str += "            \'O\'  - swich debug render\n";
            out_str += "            \'P\'  - toggle physic model of box\n";
            out_str += "            \'I\'  - force marine to drop gun\n";
            out_str += "            \'~\'  - toggle event console\n";
            out_str += "\'Left Ctrl + ~\'  - console writing\n";
            out_str += "           \'Tab'  - switch camera mode\n";

            Vector2 poss = Content.Fonts._font1.MeasureString("  Recalulcalated objects count:  ");
            poss.Y--;
            Controls.Label l_up_label;
            l_fps =                                 Controls.CreateLabel(GColors.CText,     new Vector2(poss.X, 5),                 Content.Fonts._font1);
            l_Frame_time =                          Controls.CreateLabel(GColors.CText,     new Vector2(poss.X, poss.Y + 5),        Content.Fonts._font1);
            l_Visible_objects_count =               Controls.CreateLabel(GColors.CText,     new Vector2(poss.X, 5 + poss.Y * 2),    Content.Fonts._font1);
            l_Recalulcalated_objects_count =        Controls.CreateLabel(GColors.CText,     new Vector2(poss.X, 5 + poss.Y * 3),    Content.Fonts._font1);
            l_Character_angle =                     Controls.CreateLabel(GColors.CText,     new Vector2(poss.X, 5 + poss.Y * 4),    Content.Fonts._font1);
            l_Character_name =                      Controls.CreateLabel(GColors.CAlarm,    new Vector2(),                          Content.Fonts._font1);
            l_up_label =                            Controls.CreateLabel(GColors.CText,     new Vector2(5, 5),                      Content.Fonts._font1, out_str);

            Controls.Image img2 = Controls.CreateImage(Vector2.Zero, new PackTexture("tex_w2x2\0",true), GColors.CTextBack, Content.Fonts._font1.MeasureString(out_str));
            debug_panel = new Controls.UserControl();
            debug_panel.Position = new Vector2(5, 5);
            
            debug_panel.Add(img2);
            debug_panel.Add(l_fps);
            debug_panel.Add(l_Frame_time);
            debug_panel.Add(l_Visible_objects_count);
            debug_panel.Add(l_Recalulcalated_objects_count);
            debug_panel.Add(l_Character_angle);
            debug_panel.Add(l_Character_name);
            debug_panel.Add(l_up_label);
            debug_panel.Add(debug_textbox);
            Add(debug_panel);

            
        }

        public void onDebugTextboxEnter()
        {
        //    Color color = GColors.CForeColor;

            command_buffer.Add(debug_textbox.Text);
            current_command = command_buffer.Count;
            Scripting.SE.Instance.Execute(debug_textbox.Text);

            if (Scripting.SE.Instance.LastException == null)
                MyGame.ScreenLogMessage(debug_textbox.Text, GColors.CForeColor);

            debug_textbox.Text = "";
        }

        public void onDebugTextboxEscape()
        {
            KeyboardManager.Manager.CaptureRelease();
        }

        public void onDebugTextboxUp()
        {
            if (current_command > 0)
                debug_textbox.Text = command_buffer[--current_command];
        }

        public void onDebugTextboxDown()
        {
            if (current_command < command_buffer.Count - 1)
                debug_textbox.Text = command_buffer[++current_command];
            else
                debug_textbox.Text = "";
        }

       

        public override void Draw()
        {
            //setrendertarget here
            base.Draw();
        }

        public override void Update()
        {
            l_fps.Text = Program.game._engine.FPSCounter.FramesPerSecond.ToString();
            l_Frame_time.Text = Program.game._engine.FPSCounter.FrameTime.ToString();
            l_Visible_objects_count.Text = Program.game._engine.visibleobjectscount.ToString();
            l_Recalulcalated_objects_count.Text = Program.game._engine.gameScene._sceneGraph.recalulcalated().ToString();
            l_Character_angle.Text = Program.game._engine.playerState.ToString();

            base.Update();
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
}
