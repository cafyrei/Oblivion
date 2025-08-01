using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;


namespace Oblivion
{

    [Serializable]
    public class PlayerData
    {
        public float Health;
        public int CurrentStage;
        public SerializableVector2 SpawnPosition;

    }


    [Serializable]
    public class SerializableVector2
    {
        public float X;
        public float Y;

        public SerializableVector2() { }

        public SerializableVector2(Vector2 vector)
        {
            X = vector.X;
            Y = vector.Y;
        }

        public Vector2 ToVector2() => new Vector2(X, Y);
    }


}

