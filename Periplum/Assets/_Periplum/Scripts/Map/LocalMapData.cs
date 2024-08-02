using Periplum;
using System;

[Serializable]
public class LocalMapData
{
    public TimedLineData timedLineData = null;
    public SerializableVector3 position;
    public SerializableVector3 target;
}