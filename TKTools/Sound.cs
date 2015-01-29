using OpenTK.Audio;
using System;
using System.IO;
using OpenAL = OpenTK.Audio.OpenAL;

namespace TKTools
{
	public class Sound : IDisposable
	{
		static AudioContext context;

		int buffer;
		int source;

		public Sound(string filename)
		{
			try
			{
				if (context == null)
					context = new AudioContext();

				buffer = AL.GenBuffer();
				source = AL.GenSource();

				int channels, bitsPerSample, sampleRate;
				var soundData = LoadWave(File.Open(filename, FileMode.Open), out channels, out bitsPerSample, out sampleRate);

				ALFormat format = (ALFormat)0;

				if (channels == 1) format = bitsPerSample == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
				if (channels == 2) format = bitsPerSample == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;

				AL.BufferData(buffer, format, soundData, soundData.Length, sampleRate);

				AL.Source(source, ALSourcei.Buffer, buffer);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}

		public void Play()
		{
			AL.Source(source, ALSourceb.Looping, false);
			AL.SourcePlay(source);
		}

		public void Loop()
		{
			AL.Source(source, ALSourceb.Looping, true);
			AL.SourcePlay(source);
		}

		public void Stop()
		{
			AL.SourceStop(source);
		}

		public void Dispose()
		{
			AL.DeleteBuffer(buffer);
			AL.DeleteSource(source);
		}

		public static byte[] LoadWave(Stream stream, out int channels, out int bits, out int rate)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");

			using (BinaryReader reader = new BinaryReader(stream))
			{
				// RIFF header
				string signature = new string(reader.ReadChars(4));
				if (signature != "RIFF")
					throw new NotSupportedException("Specified stream is not a wave file.");

				int riff_chunck_size = reader.ReadInt32();

				string format = new string(reader.ReadChars(4));
				if (format != "WAVE")
					throw new NotSupportedException("Specified stream is not a wave file.");

				// WAVE header
				string format_signature = new string(reader.ReadChars(4));
				if (format_signature != "fmt ")
					throw new NotSupportedException("Specified wave file is not supported.");

				int format_chunk_size = reader.ReadInt32();
				int audio_format = reader.ReadInt16();
				int num_channels = reader.ReadInt16();
				int sample_rate = reader.ReadInt32();
				int byte_rate = reader.ReadInt32();
				int block_align = reader.ReadInt16();
				int bits_per_sample = reader.ReadInt16();

				string data_signature = new string(reader.ReadChars(4));
				if (data_signature != "data")
					throw new NotSupportedException("Specified wave file is not supported.");

				int data_chunk_size = reader.ReadInt32();

				channels = num_channels;
				bits = bits_per_sample;
				rate = sample_rate;

				return reader.ReadBytes((int)reader.BaseStream.Length);
			}
		}
	}
}