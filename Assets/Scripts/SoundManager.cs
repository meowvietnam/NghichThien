using DG.Tweening;
using LongDH.LongDHModules;
using LongDH.Script.ObjectPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum ETypeSound
{
    Sound,
    Music,
    Haptic
}
public class SoundManager : BaseBehaviour
{
    [SerializeField]
    private GameObject audioPrefab;
    private List<AudioSource> listAudio = new List<AudioSource>();
    private AudioInfo infoCombat => new AudioInfo(sfxCombat[0], true, true);
    private AudioInfo infoHome => new AudioInfo(sfxMap, true, true);
    private AudioInfo infoOpening => new AudioInfo(sfxOpening, true, true);

    public float volumeMusic = 0.5f;
    public float volumeSound = 0.5f;

    public AudioClip sfxOpening;
    public AudioClip sfxMap;
    public AudioClip[] sfxCombat;
    public AudioClip sfxVictory;
    public AudioClip sfxBtnClick;
    public AudioClip sfxLose;

    public static SoundManager Instance;
    public static string KeySaveValueMusic => "KeySaveValueMusic";
    public static string KeySaveValueSound => "KeySaveValueSound";


    private void Awake()
    {
        Instance = this;
        LoadAndApplyAudioSettings();
        PlayOne(infoOpening, false, volumeMusic);
        DontDestroyOnLoad(this.gameObject);
        Observer.AddObserver(EventList.Combat.END_COMBAT, EndCombat);
    }
    private void EndCombat(params object[] data)
    {
        bool isUserWin = (bool)data[2];
        if (isUserWin)
        {
            PlayVictory();

        }
        else
        {
            PlayLose();

        }
    }

    private void LoadAndApplyAudioSettings()
    {
        // Lấy giá trị Music đã lưu, nếu key không tồn tại thì mặc định trả về 1f
        volumeMusic = PlayerPrefs.GetFloat(KeySaveValueMusic, 0.5f);

        // Lấy giá trị Sound đã lưu, nếu key không tồn tại thì mặc định trả về 1f
        volumeSound = PlayerPrefs.GetFloat(KeySaveValueSound, 0.5f);

        // --- GÁN GIÁ TRỊ VÀO HỆ THỐNG CỦA BẠN TẠI ĐÂY ---
        // Ví dụ 1: Nếu bạn dùng AudioMixer (Khuyên dùng vì quản lý được Volume tổng của PlaySoundEffect)
        // SetMixerVolume("MusicParam", MusicVolume);
        // SetMixerVolume("SoundParam", SoundVolume);

        // Ví dụ 2: Nếu bạn gán trực tiếp cho một AudioSource nhạc nền (BGM) đang chạy ở đây:
        // bgmAudioSource.volume = MusicVolume;

        Debug.Log($"[Audio] Đã load cấu hình - Music Volume: {volumeMusic} | Sound Volume: {volumeSound}");
    }
    public void PlaySoundButtonClick()
    {
        PlayOne(new AudioInfo { clip = sfxBtnClick,isLoop = false, isMusic = false},false,volumeSound);
    }
    public void PlayVictory()
    {
        PlayOne(new AudioInfo { clip = sfxVictory, isLoop = false, isMusic = false }, false, volumeSound);
    }
    public void PlayLose()
    {
        PlayOne(new AudioInfo { clip = sfxLose, isLoop = false, isMusic = false }, false, volumeSound);
    }
    public void PlaySound(AudioInfo info, bool isReplay = false)
    {
        PlayOne(info,isReplay,volumeSound);
    }    
    public void StopMusic()
    {
        StopSound(infoCombat);
        StopSound(infoHome);
        StopSound(infoOpening);
    }
    public void PlayMusicHome()
    {
        StopSound(infoCombat); // stop sound bg gamePlay
        StopSound(infoOpening); // stop sound bg opening
        PlayOne(infoHome, true,volumeMusic);
    }
    public void PlayMusicCombat()
    {
        StopSound(infoHome); // stop sound bg gamePlay
        StopSound(infoOpening); // stop sound bg opening
        PlayOne(infoCombat, true, volumeMusic);
    }
    public void StopSound(AudioInfo info)
    {
        listAudio.ForEach(x =>
        {
            if (x.clip == info.clip)
            {
                x.Stop();
            }
        });
    }

    AudioSource CreateAudio()
    {
        var obj = audioPrefab.Spawn(this.transform);
        var audio = obj.GetComponent<AudioSource>();
        listAudio.Add(audio);
        return audio;
    }
    AudioSource GetSource(AudioInfo info, bool isOnlyOne = false)
    {
        var audio = listAudio.Find(x => isOnlyOne ? x.clip == info.clip : !x.isPlaying);
        if (audio == null)
        {
            audio = CreateAudio();
        }
        return audio;
    }
    public void PlayOne(AudioInfo info, bool isReplay = false, float volume = 1f)
    {
       

        var audio = GetSource(info, true);

        audio.clip = info.clip;
        audio.loop = info.isLoop;
        audio.volume = volume;

        //// Check quyền phát
        //if (info.isMusic)
        //{
        //    Debug.LogWarning($"Music OFF — Không phát: {info.clip?.name}");
        //    return;
        //}
        //if (!info.isMusic)
        //{
        //    Debug.LogWarning($"Sound OFF — Không phát: {info.clip?.name}");
        //    return;
        //}
        

        if (audio.isPlaying && !isReplay) return;
        if (isReplay) audio.Stop();

        audio.Play();
    }
    public void SetMusicVolume(float volume)
    {
        this.volumeMusic = volume;
        PlayerPrefs.SetFloat(KeySaveValueMusic, this.volumeMusic);

        foreach (var audio in listAudio)
        {
            
            if (audio.clip == infoCombat.clip || audio.clip == infoHome.clip)
            {
                audio.volume = volume;
            }
        }
    }
    public void SetSoundVolume(float volume)
    {
        this.volumeSound = volume;
        PlayerPrefs.SetFloat(KeySaveValueSound, this.volumeSound);

        foreach (var audio in listAudio)
        {
            if (audio.clip != infoCombat.clip && audio.clip != infoHome.clip)
            {
                audio.volume = volume;
            }
           
        }

       
    }

    public void LocateAwake()
    {
        //DOVirtual.DelayedCall(0.02f, () =>
        //{
        //    _SoundManager.PlayOne(new SoundManager.AudioInfo
        //    {
        //        clip = _GameConfig.soundConfig.sfxBGPlayGame,
        //        isLoop = true,
        //        isMusic = true

        //    }, false, 0.3f);
        //});
    }

    //public void LocateAwake()
    //{
    //    SoundManager.AudioInfo info = new SoundManager.AudioInfo(_GameConfig.soundConfig.sfxBGPlayGame, true, true);
    //    // _SoundManager.StopSound(infoStop); // stop sound bg home
    //    _SoundManager.PlayOne(info,false,0.3f);
    //}

    public struct AudioInfo
    {

        public AudioClip clip;
        public bool isLoop;
        public bool isMusic;


        public AudioInfo(AudioClip clip, bool isLoop, bool isMusic)
        {
            this.clip = clip;
            this.isLoop = isLoop;
            this.isMusic = isMusic;

        }
        public AudioInfo(AudioClip[] clips, bool isLoop, bool isMusic)
        {
            this.clip = clips[Random.Range(0, clips.Length)];
            this.isLoop = isLoop;
            this.isMusic = isMusic;

        }
    }
}
