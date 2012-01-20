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
            debug_textbox = new Controls.TextBox(init_text, new Vector2(0, GameConfiguration.ScreenResolution.Y - Content.Fonts._font1.MeasureString("A").Y - 5), new Vector2(150, 1), debug_textbox_hotkeys, Content.Fonts._font1);
            Add(debug_textbox);
            command_buffer.Add(init_text);
            KeyboardManager.Manager.AddKeyboardUser(debug_textbox);
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
