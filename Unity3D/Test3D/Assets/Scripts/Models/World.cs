using System.Collections.Generic;

public class World
{
    public static World Instance;

    public static World Current
    {
        get
        {
            if (Instance == null)
            {
                Instance=new World();       
            }

            return Instance;
        }
    }

    public int currentLevelIndex { get; set; } 

    public List<Floor> Levels { get; set; }

    public Floor CurrentFloor
    {
        get
        {
            if (this.currentLevelIndex >= 0 && this.currentLevelIndex <= this.Levels.Count)
            {
                return this.Levels[this.currentLevelIndex];
            }

            return null;
        }
    }

    private World()
    {
        this.Levels=new List<Floor>();
        this.currentLevelIndex = -1;
        World.Instance = this;

        this.Load();
        if (this.Levels.Count == 0)
        {
            Floor l = new Floor(this);
            l.Create(100, 100);

            this.Levels.Add(l);
            this.currentLevelIndex = this.Levels.Count- 1;
        }
    }

    public void GotoNextLevel()
    {
        int ix = this.currentLevelIndex + 1;
        if (ix >= this.Levels.Count)
        {
            Floor l = new Floor(this);
            l.Create(100, 100);

            this.Levels.Add(l);
            this.currentLevelIndex = this.Levels.Count- 1;

        }
        else
        {
            this.currentLevelIndex = ix;
        }
    }

    public void GotoPreviousLevel()
    {
        int ix = this.currentLevelIndex - 1;
        if (ix >= 0)
        {
            this.currentLevelIndex = ix;
        }
    }

    public void Save()
    {
    }

    public void Load()
    {
    }
}
