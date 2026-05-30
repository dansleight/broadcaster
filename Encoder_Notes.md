
### Determine Video Device

The application will run this, but when debugging, it's often useful to determine how the device is mounted. The first `video_` device that belongs to `USB2 Video` or `USB3 Video` is the one we will be using.

```bash
v4l2-ctl --list-devices
```

If you want to preview, which can be useful, you can run:

```bash
ffplay -f v4l2 -i /dev/video6
```

Just replace `/dev/video6` with the device identified when listing devices.


### Determine the Audio Device

Similar to above, we want the audio device assocated with `[USB2 Video]`:

```bash
arecord -l
```

### Determine the offset

It's necessary with these inexpensive USB Video devices to determine the offset between the video and the audio. The only way I've determined to do that is to create a short test video, and examine the output, something like:

```bash
ffmpeg -hide_banner \
  -thread_queue_size 4096 \
  -f mjpeg \
  -use_wallclock_as_timestamps 1 \
  -framerate 30 \
  -i <(v4l2-ctl -d /dev/video0 \
        --set-fmt-video=width=1280,height=720,pixelformat=MJPG \
        --set-parm=30 \
        --stream-mmap --stream-count=0 --stream-to=-) \
  -thread_queue_size 4096 \
  -f alsa -i hw:1,0 \
  -vf format=yuv420p \
  -c:v libx264 -preset veryfast -tune zerolatency -profile:v high \
  -b:v 2500k -maxrate 2500k -bufsize 2500k \
  -g 30 -keyint_min 30 \
  -c:a aac -ac 2 -ar 48000 -b:a 128k \
  -t 15 /tmp/test_sync.mp4
```

In the output, you'll see two inputs, something like:
```
Input #0, mjpeg, from '/dev/fd/63':
  Duration: N/A, start: 1776473579.941737, bitrate: N/A
  Stream #0:0: Video: mjpeg (Baseline), yuvj422p(pc, bt470bg/unknown/unknown), 1280x720, 30 fps, 30 tbr, 1200k tbn
[aist#1:0/pcm_s16le @ 0x55dc4e3b10c0] Guessed Channel Layout: stereo
Input #1, alsa, from 'hw:1,0':
  Duration: N/A, start: 1776473580.689415, bitrate: 1536 kb/s
  Stream #1:0: Audio: pcm_s16le, 48000 Hz, stereo, s16, 1536 kb/s
```

The important thing is the start times. Subtract the `mjpeg` start time from the `alsa` start time to get the delta, which is used when starting the stream as the `-itsoffset` argument in `ffmpeg`.
