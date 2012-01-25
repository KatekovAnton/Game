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

        public Controls.UserControl l_panel;


        public List<string> command_buffer = new List<string>();
        public int current_command = 0;
        public void Init() 
        {
            UIContent.Init();
            List<HotKey> debug_textbox_hotkeys = new List<HotKey>();
            debug_textbox_hotkeys.Add(new HotKey(new Keys[] { Keys.Enter },onDebugTextboxEnter));
            debug_textbox_hotkeys.Add(new HotKey(new Keys[] { Keys.Escape }, onDebugTextboxEscape));
            debug_textbox_hotkeys.Add(new HotKey(new Keys[] { Keys.Up }, onDebugTextboxUp));
            debug_textbox_hotkeys.Add(new HotKey(new Keys[] { Keys.Down }, onDebugTextboxDown));
            string init_text ="debug console textbox";
            debug_textbox = Controls.CreateTextBox(init_text, new Vector2(0, GameConfiguration.ScreenResolution.Y - Content.Fonts._font1.MeasureString("A").Y - 5), new Vector2(150, 1), debug_textbox_hotkeys, Content.Fonts._font1);
            Add(debug_textbox);
            command_buffer.Add(init_text);
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
            l_panel = new Controls.UserControl();
            l_panel.Position = new Vector2(5, 5);
            poss.Y--;
            l_fps = (Controls.Label)Controls.CreateLabel(GColors.CText, new Vector2(poss.X, 5), Content.Fonts._font1);
            l_Frame_time = (Controls.Label)Controls.CreateLabel(GColors.CText, new Vector2(poss.X, poss.Y+5), Content.Fonts._font1);
            l_Visible_objects_count = (Controls.Label)Controls.CreateLabel(GColors.CText, new Vector2(poss.X,5+ poss.Y*2), Content.Fonts._font1);
            l_Recalulcalated_objects_count = (Controls.Label)Controls.CreateLabel(GColors.CText, new Vector2(poss.X,5+ poss.Y * 3), Content.Fonts._font1);
            l_Character_angle = (Controls.Label)Controls.CreateLabel(GColors.CText, new Vector2(poss.X,5+ poss.Y * 4), Content.Fonts._font1);

            l_panel.Add(l_fps);
            l_panel.Add(l_Frame_time);
            l_panel.Add(l_Visible_objects_count);
            l_panel.Add(l_Recalulcalated_objects_count);
            l_panel.Add(l_Character_angle);

            l_Character_name = (Controls.Label)Controls.CreateLabel(Microsoft.Xna.Framework.Color.Black, new Vector2(), Content.Fonts._font1);
            l_panel.Add(l_Character_name);

            Vector2 size = Content.Fonts._font1.MeasureString(out_str);

            if (Config.Instance["_ultraLowRender"]) size = new Vector2(502, 246); 
            
                Controls.Image img = new Controls.Image(Vector2.Zero, GColors.CTextBack, new RT((int)size.X + 10, (int)size.Y + 10, Color.White, "sss"));
                Controls.UserControl uc = new Controls.UserControl();
                uc.Add(img);
                uc.Add(Controls.CreateLabel(GColors.CText, new Vector2(5, 5), Content.Fonts._font1, Config.Instance["_use_static_labels"], out_str));
                Controls.Image img2 = new Controls.Image(Vector2.Zero, uc, new RT((int)size.X + 10, (int)size.Y + 10, GColors.CZero, "sss"));

                Add(img2);
            
            Add(l_panel);

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
