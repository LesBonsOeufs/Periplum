using System;

[Serializable]
public class LocalMapData
{
    public TimedLine timedLine = null;
    public SerializableVector3 position;
    public SerializableVector3 target;

    [Serializable]
    public class TimedLine
    {
        public DateTime timeLimit;
        public SerializableVector3 startPosition;

        public TimedLine(DateTime timeLimit, SerializableVector3 startPosition)
        {
            this.timeLimit = timeLimit;
            this.startPosition = startPosition;
        }
    }
}