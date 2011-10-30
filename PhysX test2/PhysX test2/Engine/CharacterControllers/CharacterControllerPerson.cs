using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Microsoft.Xna.Framework.Input;

namespace PhysX_test2.Engine.CharacterControllers
{
    public enum CharacterMoveState
    {
        _new,
        Stay,
        WalkForward,
        WalkBackward,
        WalkLeft,
        WalkRight,
        WalkFL,
        WalkFR,
        WalkBL,
        WalkBR,
        Action
    };

    public enum ActionKeyState
    {
        W,S,A,D,E
    };

    class CharacterParameters
    {
        public float ActionLength = 2.0f;
    }

    public class CharacterControllerPerson:CharacterControllerSuperClass
    {
        
        private Logic.BehaviourModel.ObjectPhysicControllerBehaviourModel _charcterBehaviour;
        private Logic.LevelObject _character;
        public float _oldRotation = -MathHelper.PiOver2;

        public float angle;

        CharacterParameters parameters;

        CharacterMoveState _state;

        public CharacterControllerPerson(Logic.LevelObject charecter)
        {
            _character = charecter;
            _charcterBehaviour = charecter.behaviourmodel as Logic.BehaviourModel.ObjectPhysicControllerBehaviourModel;
            parameters = new CharacterParameters();
            _state = CharacterMoveState._new;
        }

        private float CreateAngleForCharacter(Microsoft.Xna.Framework.Vector3 target)
        {
            double buffer = 0;
            double result = 0;

            float cursorX = target.X;
            float cursorZ = target.Z;

            Vector3 charPos = _charcterBehaviour.CurrentPosition.Translation;
            float charX = charPos.X;
            float charZ = charPos.Z;
            double difX = charX - cursorX;
            double difZ = charZ - cursorZ;
            if(difX>0)
            {
                if (difX == 0)
                    difX = 0.01;
                buffer = Math.Atan(difZ / difX);
            }
            else
            {
                if (difX == 0)
                    difX = 0.01;
                buffer = Math.Atan(difZ / difX)-Math.PI;
            }
            result = _oldRotation - buffer;

            _oldRotation = (float) buffer;
            return (float) result;
        }

        public override void Update(Vector3 _target, float yaang)
        {
            angle = CreateAngleForCharacter(_target);
            _charcterBehaviour.Rotate(angle);
            Vector3 viewVector = _charcterBehaviour.CurrentPosition.Translation - _target;
            Vector3 moveVector = Vector3.Zero;

            //   CharacterMoveState newState = CharacterMoveState.Stay;
            // обработка нажатий клавы
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.W))
            {
                Vector3 move = Extensions.VectorForCharacterMoving(Extensions.Route.Forward, yaang);
                moveVector += move;
                _charcterBehaviour.Move(move);
            }

            if (keyboardState.IsKeyDown(Keys.S))
            {
                Vector3 move = Extensions.VectorForCharacterMoving(Extensions.Route.Back, yaang);
                moveVector += move;
                _charcterBehaviour.Move(move);
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                Vector3 move = Extensions.VectorForCharacterMoving(Extensions.Route.Left, yaang);
                moveVector += move;
                _charcterBehaviour.Move(move);
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                Vector3 move = Extensions.VectorForCharacterMoving(Extensions.Route.Right, yaang);
                moveVector += move;
                _charcterBehaviour.Move(move);
            }
            CharacterMoveState newstate = CharacterMoveState.Stay;
            if (moveVector != Vector3.Zero)
                newstate = calculateCurrentState(moveVector, viewVector);
            if (movestate != newstate)
            {
                Render.AnimRenderObject ro = _character.renderaspect as Render.AnimRenderObject;
                ro.ReceiveEvent(GetEventName(newstate));
                movestate = newstate;
            }
        }

        private string GetEventName(CharacterMoveState newstate)
        {
            switch (newstate)
            {
                case CharacterMoveState.Stay:
                    return "stopMove\0";
                case CharacterMoveState.WalkBackward:
                    return "beginWalkBack\0";
                case CharacterMoveState.WalkForward:
                    return "beginWalk\0";
                default: return "stopMove\0";
            }
        } 
        public CharacterMoveState movestate;

        public float cospi18 = (float)Math.Cos(Math.PI * 1.0 / 8.0);
        //где z -1 там 0 вращения
        public CharacterMoveState calculateCurrentState(Vector3 _move, Vector3 _view)
        {
            Vector2 move = new Vector2(_move.X, _move.Z);
            Vector2 view = new Vector2(_view.X, _view.Z);
            move.Normalize();
            view.Normalize();


            float value = Vector2.Dot(move, view);

            if (1 - value > cospi18)
                return CharacterMoveState.WalkForward;
            if (-1 + value < cospi18)
                return CharacterMoveState.WalkBackward;

            return CharacterMoveState.WalkForward;
        }
    }
}
