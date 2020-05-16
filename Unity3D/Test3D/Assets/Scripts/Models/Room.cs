using System.Collections.Generic;

public class Room: BaseArea
{
    public Room()
    {
        this.Corridors=new List<Corridor>();    
    }

    public List<Corridor> Corridors { get; internal set; }
}