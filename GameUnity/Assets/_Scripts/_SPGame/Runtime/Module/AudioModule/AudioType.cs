namespace SPGame
{
    /// <summary>
    /// 音频分类
    /// </summary>
    /// <remarks>和 AudioMixer 中分类保持一致</remarks>
    public enum AudioType : byte
    {
        /// <summary>背景音乐</summary>
        Music = 0,

        /// <summary>声音音效</summary>
        Sound = 1,

        /// <summary>UI音效</summary>
        UISound = 2,

        /// <summary>人声音效</summary>
        Voice = 3,
    }
}