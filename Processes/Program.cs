using Processes;

static async void main()
{
    IProcessExtension processExtension = new ProcessExtension();

    var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "a.mp4");

    //var path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\","a.mp4"));

    var a = await processExtension
             .Wrap("packager")
             .WithArgument(
                @$"in={path},stream=audio,output=a_audio.mp4,drm_label=AUDIO",
                @$"in={path},stream=video,output=a_hd.mp4,drm_label=HD",
                $"--enable_raw_key_encryption",
                $"--keys label=AUDIO:key_id=f3c5e0361e6654b28f8049c778b23946:key=a4631a153a443df9eed0593043db7519,label=HD:key_id=6d76f25cb17f5e16b8eaef6bbf582d8e:key=cb541084c99731aef4fff74500c12ead",
                $"--mpd_output {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),"a.mpd")}")
             .ExcuteAsync();

    Console.WriteLine(a.Name);
}

main();