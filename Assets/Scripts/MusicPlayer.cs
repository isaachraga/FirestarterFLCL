using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string Songs = "";
    FMOD.Studio.PARAMETER_ID SongParameterId;
    FMOD.Studio.PARAMETER_ID RepeatParameterId;
    public int SongNum;
    public bool RandomSong;
    int RndSongNum;
    public bool Repeat;
    
    FMOD.Studio.EventInstance MusicState;

    List<int> RndList = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        RandomSong = true;
        Repeat = false;
        StartRandom();

        MusicState = FMODUnity.RuntimeManager.CreateInstance(Songs);

        FMOD.Studio.EventDescription SongEventDescription;
        MusicState.getDescription(out SongEventDescription);
        FMOD.Studio.PARAMETER_DESCRIPTION SongParameterDescription;
        SongEventDescription.getParameterDescriptionByName("Song", out SongParameterDescription);
        SongParameterId = SongParameterDescription.id;

        FMOD.Studio.EventDescription RepeatEventDescription;
        MusicState.getDescription(out RepeatEventDescription);
        FMOD.Studio.PARAMETER_DESCRIPTION RepeatParameterDescription;
        RepeatEventDescription.getParameterDescriptionByName("Repeat", out RepeatParameterDescription);
        RepeatParameterId = RepeatParameterDescription.id;

        MusicState.start();
    }

    // Update is called once per frame
    void Update()
    {
        MusicState.setParameterByID(SongParameterId, (float)SongNum);
        
    }

    void NextSong()
    {
        if (RandomSong)
        {
            if (RndSongNum >= 9)
            {
                RndSongNum = 0;
            }
            else
            {
                RndSongNum++;
            }
            SongNum = RndList.IndexOf(RndSongNum);
            
        }
        else
        {
            if(SongNum >= 9)
            {
                SongNum = 0;
            }
            else
            {
                SongNum++;
            }
        }

    }

    void PreviousSong()
    {
        if (RandomSong)
        {
            if (RndSongNum <= 0)
            {
                RndSongNum = 9;
            }
            else
            {
                RndSongNum++;
            }
            SongNum = RndList.IndexOf(RndSongNum);
        }
        else
        {
            if (SongNum <= 0)
            {
                SongNum = 9;
            }
            else
            {
                SongNum--;
            }
        }

    }

    void PlaySong()
    {
        MusicState.setPaused(false);

    }

    void PauseSong()
    {
        MusicState.setPaused(true);

    }

    void ToggleRepeat()
    {
        if (Repeat)
        {
            Repeat = false;
            MusicState.setParameterByID(RepeatParameterId, 0);
        }
        else
        {
            Repeat = true;
            MusicState.setParameterByID(RepeatParameterId, 1);
        }

    }

    void StartRandom()
    {
        while (RndList.Count < 10)
        {
            int RndNum = (int)Random.Range(0f, 10f);
            if (!RndList.Contains(RndNum))
            {
                RndList.Add(RndNum);
            }
           
        }

        RndSongNum = 0;
    }

    void ToggleRandom()
    {
        if (RandomSong)
        {
            RandomSong = false;
        }
        else
        {
            RandomSong = true;
            RndList.Clear();

            while(RndList.Count < 10)
            {
                int RndNum = (int)Random.Range(0f, 10f);
                if (!RndList.Contains(RndNum))
                {
                    RndList.Add(RndNum);
                }
            }
            RndSongNum = 0;
        }
    
    }

    void StopMusic()
    {
        FMOD.Studio.Bus playerBus = FMODUnity.RuntimeManager.GetBus("bus:/player");
        playerBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
        MusicState.release();
    }
}
