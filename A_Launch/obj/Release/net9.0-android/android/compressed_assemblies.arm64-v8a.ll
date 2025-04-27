; ModuleID = 'compressed_assemblies.arm64-v8a.ll'
source_filename = "compressed_assemblies.arm64-v8a.ll"
target datalayout = "e-m:e-i8:8:32-i16:16:32-i64:64-i128:128-n32:64-S128"
target triple = "aarch64-unknown-linux-android21"

%struct.CompressedAssemblies = type {
	i32, ; uint32_t count
	ptr ; CompressedAssemblyDescriptor descriptors
}

%struct.CompressedAssemblyDescriptor = type {
	i32, ; uint32_t uncompressed_file_size
	i1, ; bool loaded
	ptr ; uint8_t data
}

@compressed_assemblies = dso_local local_unnamed_addr global %struct.CompressedAssemblies {
	i32 195, ; uint32_t count
	ptr @compressed_assembly_descriptors; CompressedAssemblyDescriptor* descriptors
}, align 8

@compressed_assembly_descriptors = internal dso_local global [195 x %struct.CompressedAssemblyDescriptor] [
	%struct.CompressedAssemblyDescriptor {
		i32 9216, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_0; uint8_t* data
	}, ; 0: Launch_android
	%struct.CompressedAssemblyDescriptor {
		i32 345088, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_1; uint8_t* data
	}, ; 1: Acornima
	%struct.CompressedAssemblyDescriptor {
		i32 876544, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_2; uint8_t* data
	}, ; 2: Jint
	%struct.CompressedAssemblyDescriptor {
		i32 23424, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_3; uint8_t* data
	}, ; 3: Microsoft.DotNet.PlatformAbstractions
	%struct.CompressedAssemblyDescriptor {
		i32 79624, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_4; uint8_t* data
	}, ; 4: Microsoft.Extensions.DependencyModel
	%struct.CompressedAssemblyDescriptor {
		i32 187904, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_5; uint8_t* data
	}, ; 5: NAudio.Core
	%struct.CompressedAssemblyDescriptor {
		i32 87040, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_6; uint8_t* data
	}, ; 6: NAudio.Flac
	%struct.CompressedAssemblyDescriptor {
		i32 72704, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_7; uint8_t* data
	}, ; 7: NLayer
	%struct.CompressedAssemblyDescriptor {
		i32 8192, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_8; uint8_t* data
	}, ; 8: NLayer.NAudioSupport
	%struct.CompressedAssemblyDescriptor {
		i32 81408, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_9; uint8_t* data
	}, ; 9: NVorbis
	%struct.CompressedAssemblyDescriptor {
		i32 161488, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_10; uint8_t* data
	}, ; 10: Silk.NET.Core
	%struct.CompressedAssemblyDescriptor {
		i32 342016, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_11; uint8_t* data
	}, ; 11: Silk.NET.Maths
	%struct.CompressedAssemblyDescriptor {
		i32 68304, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_12; uint8_t* data
	}, ; 12: Silk.NET.OpenAL
	%struct.CompressedAssemblyDescriptor {
		i32 1960656, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_13; uint8_t* data
	}, ; 13: Silk.NET.OpenGLES
	%struct.CompressedAssemblyDescriptor {
		i32 1601744, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_14; uint8_t* data
	}, ; 14: Silk.NET.SDL
	%struct.CompressedAssemblyDescriptor {
		i32 53968, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_15; uint8_t* data
	}, ; 15: Silk.NET.Windowing.Common
	%struct.CompressedAssemblyDescriptor {
		i32 101888, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_16; uint8_t* data
	}, ; 16: Silk.NET.Windowing.Sdl
	%struct.CompressedAssemblyDescriptor {
		i32 2092032, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_17; uint8_t* data
	}, ; 17: SixLabors.ImageSharp
	%struct.CompressedAssemblyDescriptor {
		i32 138240, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_18; uint8_t* data
	}, ; 18: Tomlyn
	%struct.CompressedAssemblyDescriptor {
		i32 370688, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_19; uint8_t* data
	}, ; 19: Engine
	%struct.CompressedAssemblyDescriptor {
		i32 2622464, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_20; uint8_t* data
	}, ; 20: Survivalcraft
	%struct.CompressedAssemblyDescriptor {
		i32 4096, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_21; uint8_t* data
	}, ; 21: _Microsoft.Android.Resource.Designer
	%struct.CompressedAssemblyDescriptor {
		i32 308008, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_22; uint8_t* data
	}, ; 22: Microsoft.CSharp
	%struct.CompressedAssemblyDescriptor {
		i32 430376, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_23; uint8_t* data
	}, ; 23: Microsoft.VisualBasic.Core
	%struct.CompressedAssemblyDescriptor {
		i32 17704, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_24; uint8_t* data
	}, ; 24: Microsoft.VisualBasic
	%struct.CompressedAssemblyDescriptor {
		i32 15656, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_25; uint8_t* data
	}, ; 25: Microsoft.Win32.Primitives
	%struct.CompressedAssemblyDescriptor {
		i32 33576, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_26; uint8_t* data
	}, ; 26: Microsoft.Win32.Registry
	%struct.CompressedAssemblyDescriptor {
		i32 15672, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_27; uint8_t* data
	}, ; 27: System.AppContext
	%struct.CompressedAssemblyDescriptor {
		i32 15672, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_28; uint8_t* data
	}, ; 28: System.Buffers
	%struct.CompressedAssemblyDescriptor {
		i32 89896, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_29; uint8_t* data
	}, ; 29: System.Collections.Concurrent
	%struct.CompressedAssemblyDescriptor {
		i32 255800, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_30; uint8_t* data
	}, ; 30: System.Collections.Immutable
	%struct.CompressedAssemblyDescriptor {
		i32 48424, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_31; uint8_t* data
	}, ; 31: System.Collections.NonGeneric
	%struct.CompressedAssemblyDescriptor {
		i32 48440, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_32; uint8_t* data
	}, ; 32: System.Collections.Specialized
	%struct.CompressedAssemblyDescriptor {
		i32 126760, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_33; uint8_t* data
	}, ; 33: System.Collections
	%struct.CompressedAssemblyDescriptor {
		i32 102712, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_34; uint8_t* data
	}, ; 34: System.ComponentModel.Annotations
	%struct.CompressedAssemblyDescriptor {
		i32 17192, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_35; uint8_t* data
	}, ; 35: System.ComponentModel.DataAnnotations
	%struct.CompressedAssemblyDescriptor {
		i32 26936, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_36; uint8_t* data
	}, ; 36: System.ComponentModel.EventBasedAsync
	%struct.CompressedAssemblyDescriptor {
		i32 42280, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_37; uint8_t* data
	}, ; 37: System.ComponentModel.Primitives
	%struct.CompressedAssemblyDescriptor {
		i32 315688, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_38; uint8_t* data
	}, ; 38: System.ComponentModel.TypeConverter
	%struct.CompressedAssemblyDescriptor {
		i32 16680, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_39; uint8_t* data
	}, ; 39: System.ComponentModel
	%struct.CompressedAssemblyDescriptor {
		i32 19752, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_40; uint8_t* data
	}, ; 40: System.Configuration
	%struct.CompressedAssemblyDescriptor {
		i32 50984, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_41; uint8_t* data
	}, ; 41: System.Console
	%struct.CompressedAssemblyDescriptor {
		i32 23864, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_42; uint8_t* data
	}, ; 42: System.Core
	%struct.CompressedAssemblyDescriptor {
		i32 1016632, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_43; uint8_t* data
	}, ; 43: System.Data.Common
	%struct.CompressedAssemblyDescriptor {
		i32 16168, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_44; uint8_t* data
	}, ; 44: System.Data.DataSetExtensions
	%struct.CompressedAssemblyDescriptor {
		i32 25384, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_45; uint8_t* data
	}, ; 45: System.Data
	%struct.CompressedAssemblyDescriptor {
		i32 16680, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_46; uint8_t* data
	}, ; 46: System.Diagnostics.Contracts
	%struct.CompressedAssemblyDescriptor {
		i32 16168, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_47; uint8_t* data
	}, ; 47: System.Diagnostics.Debug
	%struct.CompressedAssemblyDescriptor {
		i32 184632, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_48; uint8_t* data
	}, ; 48: System.Diagnostics.DiagnosticSource
	%struct.CompressedAssemblyDescriptor {
		i32 29480, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_49; uint8_t* data
	}, ; 49: System.Diagnostics.FileVersionInfo
	%struct.CompressedAssemblyDescriptor {
		i32 127272, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_50; uint8_t* data
	}, ; 50: System.Diagnostics.Process
	%struct.CompressedAssemblyDescriptor {
		i32 26408, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_51; uint8_t* data
	}, ; 51: System.Diagnostics.StackTrace
	%struct.CompressedAssemblyDescriptor {
		i32 32056, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_52; uint8_t* data
	}, ; 52: System.Diagnostics.TextWriterTraceListener
	%struct.CompressedAssemblyDescriptor {
		i32 15656, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_53; uint8_t* data
	}, ; 53: System.Diagnostics.Tools
	%struct.CompressedAssemblyDescriptor {
		i32 59192, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_54; uint8_t* data
	}, ; 54: System.Diagnostics.TraceSource
	%struct.CompressedAssemblyDescriptor {
		i32 16680, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_55; uint8_t* data
	}, ; 55: System.Diagnostics.Tracing
	%struct.CompressedAssemblyDescriptor {
		i32 64824, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_56; uint8_t* data
	}, ; 56: System.Drawing.Primitives
	%struct.CompressedAssemblyDescriptor {
		i32 20776, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_57; uint8_t* data
	}, ; 57: System.Drawing
	%struct.CompressedAssemblyDescriptor {
		i32 16696, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_58; uint8_t* data
	}, ; 58: System.Dynamic.Runtime
	%struct.CompressedAssemblyDescriptor {
		i32 96552, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_59; uint8_t* data
	}, ; 59: System.Formats.Asn1
	%struct.CompressedAssemblyDescriptor {
		i32 121640, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_60; uint8_t* data
	}, ; 60: System.Formats.Tar
	%struct.CompressedAssemblyDescriptor {
		i32 16168, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_61; uint8_t* data
	}, ; 61: System.Globalization.Calendars
	%struct.CompressedAssemblyDescriptor {
		i32 15656, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_62; uint8_t* data
	}, ; 62: System.Globalization.Extensions
	%struct.CompressedAssemblyDescriptor {
		i32 16168, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_63; uint8_t* data
	}, ; 63: System.Globalization
	%struct.CompressedAssemblyDescriptor {
		i32 41256, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_64; uint8_t* data
	}, ; 64: System.IO.Compression.Brotli
	%struct.CompressedAssemblyDescriptor {
		i32 15656, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_65; uint8_t* data
	}, ; 65: System.IO.Compression.FileSystem
	%struct.CompressedAssemblyDescriptor {
		i32 38192, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_66; uint8_t* data
	}, ; 66: System.IO.Compression.ZipFile
	%struct.CompressedAssemblyDescriptor {
		i32 110384, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_67; uint8_t* data
	}, ; 67: System.IO.Compression
	%struct.CompressedAssemblyDescriptor {
		i32 32552, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_68; uint8_t* data
	}, ; 68: System.IO.FileSystem.AccessControl
	%struct.CompressedAssemblyDescriptor {
		i32 48424, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_69; uint8_t* data
	}, ; 69: System.IO.FileSystem.DriveInfo
	%struct.CompressedAssemblyDescriptor {
		i32 15664, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_70; uint8_t* data
	}, ; 70: System.IO.FileSystem.Primitives
	%struct.CompressedAssemblyDescriptor {
		i32 55096, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_71; uint8_t* data
	}, ; 71: System.IO.FileSystem.Watcher
	%struct.CompressedAssemblyDescriptor {
		i32 16168, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_72; uint8_t* data
	}, ; 72: System.IO.FileSystem
	%struct.CompressedAssemblyDescriptor {
		i32 43816, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_73; uint8_t* data
	}, ; 73: System.IO.IsolatedStorage
	%struct.CompressedAssemblyDescriptor {
		i32 48944, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_74; uint8_t* data
	}, ; 74: System.IO.MemoryMappedFiles
	%struct.CompressedAssemblyDescriptor {
		i32 78640, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_75; uint8_t* data
	}, ; 75: System.IO.Pipelines
	%struct.CompressedAssemblyDescriptor {
		i32 23864, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_76; uint8_t* data
	}, ; 76: System.IO.Pipes.AccessControl
	%struct.CompressedAssemblyDescriptor {
		i32 67880, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_77; uint8_t* data
	}, ; 77: System.IO.Pipes
	%struct.CompressedAssemblyDescriptor {
		i32 15656, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_78; uint8_t* data
	}, ; 78: System.IO.UnmanagedMemoryStream
	%struct.CompressedAssemblyDescriptor {
		i32 16168, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_79; uint8_t* data
	}, ; 79: System.IO
	%struct.CompressedAssemblyDescriptor {
		i32 575800, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_80; uint8_t* data
	}, ; 80: System.Linq.Expressions
	%struct.CompressedAssemblyDescriptor {
		i32 223528, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_81; uint8_t* data
	}, ; 81: System.Linq.Parallel
	%struct.CompressedAssemblyDescriptor {
		i32 76584, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_82; uint8_t* data
	}, ; 82: System.Linq.Queryable
	%struct.CompressedAssemblyDescriptor {
		i32 149288, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_83; uint8_t* data
	}, ; 83: System.Linq
	%struct.CompressedAssemblyDescriptor {
		i32 56104, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_84; uint8_t* data
	}, ; 84: System.Memory
	%struct.CompressedAssemblyDescriptor {
		i32 56624, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_85; uint8_t* data
	}, ; 85: System.Net.Http.Json
	%struct.CompressedAssemblyDescriptor {
		i32 676656, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_86; uint8_t* data
	}, ; 86: System.Net.Http
	%struct.CompressedAssemblyDescriptor {
		i32 131880, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_87; uint8_t* data
	}, ; 87: System.Net.HttpListener
	%struct.CompressedAssemblyDescriptor {
		i32 174888, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_88; uint8_t* data
	}, ; 88: System.Net.Mail
	%struct.CompressedAssemblyDescriptor {
		i32 51984, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_89; uint8_t* data
	}, ; 89: System.Net.NameResolution
	%struct.CompressedAssemblyDescriptor {
		i32 66360, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_90; uint8_t* data
	}, ; 90: System.Net.NetworkInformation
	%struct.CompressedAssemblyDescriptor {
		i32 56104, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_91; uint8_t* data
	}, ; 91: System.Net.Ping
	%struct.CompressedAssemblyDescriptor {
		i32 107320, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_92; uint8_t* data
	}, ; 92: System.Net.Primitives
	%struct.CompressedAssemblyDescriptor {
		i32 173368, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_93; uint8_t* data
	}, ; 93: System.Net.Quic
	%struct.CompressedAssemblyDescriptor {
		i32 162088, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_94; uint8_t* data
	}, ; 94: System.Net.Requests
	%struct.CompressedAssemblyDescriptor {
		i32 253744, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_95; uint8_t* data
	}, ; 95: System.Net.Security
	%struct.CompressedAssemblyDescriptor {
		i32 15656, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_96; uint8_t* data
	}, ; 96: System.Net.ServicePoint
	%struct.CompressedAssemblyDescriptor {
		i32 235304, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_97; uint8_t* data
	}, ; 97: System.Net.Sockets
	%struct.CompressedAssemblyDescriptor {
		i32 70952, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_98; uint8_t* data
	}, ; 98: System.Net.WebClient
	%struct.CompressedAssemblyDescriptor {
		i32 33592, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_99; uint8_t* data
	}, ; 99: System.Net.WebHeaderCollection
	%struct.CompressedAssemblyDescriptor {
		i32 23848, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_100; uint8_t* data
	}, ; 100: System.Net.WebProxy
	%struct.CompressedAssemblyDescriptor {
		i32 52008, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_101; uint8_t* data
	}, ; 101: System.Net.WebSockets.Client
	%struct.CompressedAssemblyDescriptor {
		i32 103224, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_102; uint8_t* data
	}, ; 102: System.Net.WebSockets
	%struct.CompressedAssemblyDescriptor {
		i32 17704, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_103; uint8_t* data
	}, ; 103: System.Net
	%struct.CompressedAssemblyDescriptor {
		i32 16184, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_104; uint8_t* data
	}, ; 104: System.Numerics.Vectors
	%struct.CompressedAssemblyDescriptor {
		i32 15664, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_105; uint8_t* data
	}, ; 105: System.Numerics
	%struct.CompressedAssemblyDescriptor {
		i32 41768, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_106; uint8_t* data
	}, ; 106: System.ObjectModel
	%struct.CompressedAssemblyDescriptor {
		i32 852264, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_107; uint8_t* data
	}, ; 107: System.Private.DataContractSerialization
	%struct.CompressedAssemblyDescriptor {
		i32 103208, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_108; uint8_t* data
	}, ; 108: System.Private.Uri
	%struct.CompressedAssemblyDescriptor {
		i32 153912, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_109; uint8_t* data
	}, ; 109: System.Private.Xml.Linq
	%struct.CompressedAssemblyDescriptor {
		i32 3098408, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_110; uint8_t* data
	}, ; 110: System.Private.Xml
	%struct.CompressedAssemblyDescriptor {
		i32 38696, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_111; uint8_t* data
	}, ; 111: System.Reflection.DispatchProxy
	%struct.CompressedAssemblyDescriptor {
		i32 16168, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_112; uint8_t* data
	}, ; 112: System.Reflection.Emit.ILGeneration
	%struct.CompressedAssemblyDescriptor {
		i32 16168, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_113; uint8_t* data
	}, ; 113: System.Reflection.Emit.Lightweight
	%struct.CompressedAssemblyDescriptor {
		i32 130360, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_114; uint8_t* data
	}, ; 114: System.Reflection.Emit
	%struct.CompressedAssemblyDescriptor {
		i32 15656, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_115; uint8_t* data
	}, ; 115: System.Reflection.Extensions
	%struct.CompressedAssemblyDescriptor {
		i32 501560, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_116; uint8_t* data
	}, ; 116: System.Reflection.Metadata
	%struct.CompressedAssemblyDescriptor {
		i32 16168, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_117; uint8_t* data
	}, ; 117: System.Reflection.Primitives
	%struct.CompressedAssemblyDescriptor {
		i32 24360, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_118; uint8_t* data
	}, ; 118: System.Reflection.TypeExtensions
	%struct.CompressedAssemblyDescriptor {
		i32 16688, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_119; uint8_t* data
	}, ; 119: System.Reflection
	%struct.CompressedAssemblyDescriptor {
		i32 15648, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_120; uint8_t* data
	}, ; 120: System.Resources.Reader
	%struct.CompressedAssemblyDescriptor {
		i32 16184, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_121; uint8_t* data
	}, ; 121: System.Resources.ResourceManager
	%struct.CompressedAssemblyDescriptor {
		i32 26920, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_122; uint8_t* data
	}, ; 122: System.Resources.Writer
	%struct.CompressedAssemblyDescriptor {
		i32 15656, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_123; uint8_t* data
	}, ; 123: System.Runtime.CompilerServices.Unsafe
	%struct.CompressedAssemblyDescriptor {
		i32 17720, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_124; uint8_t* data
	}, ; 124: System.Runtime.CompilerServices.VisualC
	%struct.CompressedAssemblyDescriptor {
		i32 18208, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_125; uint8_t* data
	}, ; 125: System.Runtime.Extensions
	%struct.CompressedAssemblyDescriptor {
		i32 15656, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_126; uint8_t* data
	}, ; 126: System.Runtime.Handles
	%struct.CompressedAssemblyDescriptor {
		i32 38696, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_127; uint8_t* data
	}, ; 127: System.Runtime.InteropServices.JavaScript
	%struct.CompressedAssemblyDescriptor {
		i32 15656, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_128; uint8_t* data
	}, ; 128: System.Runtime.InteropServices.RuntimeInformation
	%struct.CompressedAssemblyDescriptor {
		i32 64808, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_129; uint8_t* data
	}, ; 129: System.Runtime.InteropServices
	%struct.CompressedAssemblyDescriptor {
		i32 17704, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_130; uint8_t* data
	}, ; 130: System.Runtime.Intrinsics
	%struct.CompressedAssemblyDescriptor {
		i32 16168, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_131; uint8_t* data
	}, ; 131: System.Runtime.Loader
	%struct.CompressedAssemblyDescriptor {
		i32 143144, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_132; uint8_t* data
	}, ; 132: System.Runtime.Numerics
	%struct.CompressedAssemblyDescriptor {
		i32 66360, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_133; uint8_t* data
	}, ; 133: System.Runtime.Serialization.Formatters
	%struct.CompressedAssemblyDescriptor {
		i32 16184, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_134; uint8_t* data
	}, ; 134: System.Runtime.Serialization.Json
	%struct.CompressedAssemblyDescriptor {
		i32 23856, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_135; uint8_t* data
	}, ; 135: System.Runtime.Serialization.Primitives
	%struct.CompressedAssemblyDescriptor {
		i32 17192, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_136; uint8_t* data
	}, ; 136: System.Runtime.Serialization.Xml
	%struct.CompressedAssemblyDescriptor {
		i32 17192, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_137; uint8_t* data
	}, ; 137: System.Runtime.Serialization
	%struct.CompressedAssemblyDescriptor {
		i32 44840, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_138; uint8_t* data
	}, ; 138: System.Runtime
	%struct.CompressedAssemblyDescriptor {
		i32 58664, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_139; uint8_t* data
	}, ; 139: System.Security.AccessControl
	%struct.CompressedAssemblyDescriptor {
		i32 54056, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_140; uint8_t* data
	}, ; 140: System.Security.Claims
	%struct.CompressedAssemblyDescriptor {
		i32 17704, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_141; uint8_t* data
	}, ; 141: System.Security.Cryptography.Algorithms
	%struct.CompressedAssemblyDescriptor {
		i32 16680, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_142; uint8_t* data
	}, ; 142: System.Security.Cryptography.Cng
	%struct.CompressedAssemblyDescriptor {
		i32 16168, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_143; uint8_t* data
	}, ; 143: System.Security.Cryptography.Csp
	%struct.CompressedAssemblyDescriptor {
		i32 16168, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_144; uint8_t* data
	}, ; 144: System.Security.Cryptography.Encoding
	%struct.CompressedAssemblyDescriptor {
		i32 15656, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_145; uint8_t* data
	}, ; 145: System.Security.Cryptography.OpenSsl
	%struct.CompressedAssemblyDescriptor {
		i32 16168, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_146; uint8_t* data
	}, ; 146: System.Security.Cryptography.Primitives
	%struct.CompressedAssemblyDescriptor {
		i32 17208, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_147; uint8_t* data
	}, ; 147: System.Security.Cryptography.X509Certificates
	%struct.CompressedAssemblyDescriptor {
		i32 705288, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_148; uint8_t* data
	}, ; 148: System.Security.Cryptography
	%struct.CompressedAssemblyDescriptor {
		i32 38184, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_149; uint8_t* data
	}, ; 149: System.Security.Principal.Windows
	%struct.CompressedAssemblyDescriptor {
		i32 15656, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_150; uint8_t* data
	}, ; 150: System.Security.Principal
	%struct.CompressedAssemblyDescriptor {
		i32 15656, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_151; uint8_t* data
	}, ; 151: System.Security.SecureString
	%struct.CompressedAssemblyDescriptor {
		i32 18728, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_152; uint8_t* data
	}, ; 152: System.Security
	%struct.CompressedAssemblyDescriptor {
		i32 17192, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_153; uint8_t* data
	}, ; 153: System.ServiceModel.Web
	%struct.CompressedAssemblyDescriptor {
		i32 16168, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_154; uint8_t* data
	}, ; 154: System.ServiceProcess
	%struct.CompressedAssemblyDescriptor {
		i32 741160, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_155; uint8_t* data
	}, ; 155: System.Text.Encoding.CodePages
	%struct.CompressedAssemblyDescriptor {
		i32 16168, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_156; uint8_t* data
	}, ; 156: System.Text.Encoding.Extensions
	%struct.CompressedAssemblyDescriptor {
		i32 16176, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_157; uint8_t* data
	}, ; 157: System.Text.Encoding
	%struct.CompressedAssemblyDescriptor {
		i32 70440, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_158; uint8_t* data
	}, ; 158: System.Text.Encodings.Web
	%struct.CompressedAssemblyDescriptor {
		i32 617776, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_159; uint8_t* data
	}, ; 159: System.Text.Json
	%struct.CompressedAssemblyDescriptor {
		i32 369456, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_160; uint8_t* data
	}, ; 160: System.Text.RegularExpressions
	%struct.CompressedAssemblyDescriptor {
		i32 57144, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_161; uint8_t* data
	}, ; 161: System.Threading.Channels
	%struct.CompressedAssemblyDescriptor {
		i32 16168, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_162; uint8_t* data
	}, ; 162: System.Threading.Overlapped
	%struct.CompressedAssemblyDescriptor {
		i32 186152, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_163; uint8_t* data
	}, ; 163: System.Threading.Tasks.Dataflow
	%struct.CompressedAssemblyDescriptor {
		i32 16176, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_164; uint8_t* data
	}, ; 164: System.Threading.Tasks.Extensions
	%struct.CompressedAssemblyDescriptor {
		i32 61752, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_165; uint8_t* data
	}, ; 165: System.Threading.Tasks.Parallel
	%struct.CompressedAssemblyDescriptor {
		i32 17208, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_166; uint8_t* data
	}, ; 166: System.Threading.Tasks
	%struct.CompressedAssemblyDescriptor {
		i32 16184, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_167; uint8_t* data
	}, ; 167: System.Threading.Thread
	%struct.CompressedAssemblyDescriptor {
		i32 16168, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_168; uint8_t* data
	}, ; 168: System.Threading.ThreadPool
	%struct.CompressedAssemblyDescriptor {
		i32 15672, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_169; uint8_t* data
	}, ; 169: System.Threading.Timer
	%struct.CompressedAssemblyDescriptor {
		i32 45352, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_170; uint8_t* data
	}, ; 170: System.Threading
	%struct.CompressedAssemblyDescriptor {
		i32 175912, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_171; uint8_t* data
	}, ; 171: System.Transactions.Local
	%struct.CompressedAssemblyDescriptor {
		i32 16672, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_172; uint8_t* data
	}, ; 172: System.Transactions
	%struct.CompressedAssemblyDescriptor {
		i32 15656, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_173; uint8_t* data
	}, ; 173: System.ValueTuple
	%struct.CompressedAssemblyDescriptor {
		i32 30520, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_174; uint8_t* data
	}, ; 174: System.Web.HttpUtility
	%struct.CompressedAssemblyDescriptor {
		i32 15656, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_175; uint8_t* data
	}, ; 175: System.Web
	%struct.CompressedAssemblyDescriptor {
		i32 16184, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_176; uint8_t* data
	}, ; 176: System.Windows
	%struct.CompressedAssemblyDescriptor {
		i32 16168, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_177; uint8_t* data
	}, ; 177: System.Xml.Linq
	%struct.CompressedAssemblyDescriptor {
		i32 22320, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_178; uint8_t* data
	}, ; 178: System.Xml.ReaderWriter
	%struct.CompressedAssemblyDescriptor {
		i32 16680, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_179; uint8_t* data
	}, ; 179: System.Xml.Serialization
	%struct.CompressedAssemblyDescriptor {
		i32 16168, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_180; uint8_t* data
	}, ; 180: System.Xml.XDocument
	%struct.CompressedAssemblyDescriptor {
		i32 16168, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_181; uint8_t* data
	}, ; 181: System.Xml.XPath.XDocument
	%struct.CompressedAssemblyDescriptor {
		i32 16168, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_182; uint8_t* data
	}, ; 182: System.Xml.XPath
	%struct.CompressedAssemblyDescriptor {
		i32 16168, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_183; uint8_t* data
	}, ; 183: System.Xml.XmlDocument
	%struct.CompressedAssemblyDescriptor {
		i32 18224, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_184; uint8_t* data
	}, ; 184: System.Xml.XmlSerializer
	%struct.CompressedAssemblyDescriptor {
		i32 23848, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_185; uint8_t* data
	}, ; 185: System.Xml
	%struct.CompressedAssemblyDescriptor {
		i32 50984, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_186; uint8_t* data
	}, ; 186: System
	%struct.CompressedAssemblyDescriptor {
		i32 16680, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_187; uint8_t* data
	}, ; 187: WindowsBase
	%struct.CompressedAssemblyDescriptor {
		i32 60200, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_188; uint8_t* data
	}, ; 188: mscorlib
	%struct.CompressedAssemblyDescriptor {
		i32 101160, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_189; uint8_t* data
	}, ; 189: netstandard
	%struct.CompressedAssemblyDescriptor {
		i32 240168, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_190; uint8_t* data
	}, ; 190: Java.Interop
	%struct.CompressedAssemblyDescriptor {
		i32 83000, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_191; uint8_t* data
	}, ; 191: Mono.Android.Export
	%struct.CompressedAssemblyDescriptor {
		i32 19000, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_192; uint8_t* data
	}, ; 192: Mono.Android.Runtime
	%struct.CompressedAssemblyDescriptor {
		i32 37449248, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_193; uint8_t* data
	}, ; 193: Mono.Android
	%struct.CompressedAssemblyDescriptor {
		i32 4804392, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_194; uint8_t* data
	} ; 194: System.Private.CoreLib
], align 8

@__compressedAssemblyData_0 = internal dso_local global [9216 x i8] zeroinitializer, align 1
@__compressedAssemblyData_1 = internal dso_local global [345088 x i8] zeroinitializer, align 1
@__compressedAssemblyData_2 = internal dso_local global [876544 x i8] zeroinitializer, align 1
@__compressedAssemblyData_3 = internal dso_local global [23424 x i8] zeroinitializer, align 1
@__compressedAssemblyData_4 = internal dso_local global [79624 x i8] zeroinitializer, align 1
@__compressedAssemblyData_5 = internal dso_local global [187904 x i8] zeroinitializer, align 1
@__compressedAssemblyData_6 = internal dso_local global [87040 x i8] zeroinitializer, align 1
@__compressedAssemblyData_7 = internal dso_local global [72704 x i8] zeroinitializer, align 1
@__compressedAssemblyData_8 = internal dso_local global [8192 x i8] zeroinitializer, align 1
@__compressedAssemblyData_9 = internal dso_local global [81408 x i8] zeroinitializer, align 1
@__compressedAssemblyData_10 = internal dso_local global [161488 x i8] zeroinitializer, align 1
@__compressedAssemblyData_11 = internal dso_local global [342016 x i8] zeroinitializer, align 1
@__compressedAssemblyData_12 = internal dso_local global [68304 x i8] zeroinitializer, align 1
@__compressedAssemblyData_13 = internal dso_local global [1960656 x i8] zeroinitializer, align 1
@__compressedAssemblyData_14 = internal dso_local global [1601744 x i8] zeroinitializer, align 1
@__compressedAssemblyData_15 = internal dso_local global [53968 x i8] zeroinitializer, align 1
@__compressedAssemblyData_16 = internal dso_local global [101888 x i8] zeroinitializer, align 1
@__compressedAssemblyData_17 = internal dso_local global [2092032 x i8] zeroinitializer, align 1
@__compressedAssemblyData_18 = internal dso_local global [138240 x i8] zeroinitializer, align 1
@__compressedAssemblyData_19 = internal dso_local global [370688 x i8] zeroinitializer, align 1
@__compressedAssemblyData_20 = internal dso_local global [2622464 x i8] zeroinitializer, align 1
@__compressedAssemblyData_21 = internal dso_local global [4096 x i8] zeroinitializer, align 1
@__compressedAssemblyData_22 = internal dso_local global [308008 x i8] zeroinitializer, align 1
@__compressedAssemblyData_23 = internal dso_local global [430376 x i8] zeroinitializer, align 1
@__compressedAssemblyData_24 = internal dso_local global [17704 x i8] zeroinitializer, align 1
@__compressedAssemblyData_25 = internal dso_local global [15656 x i8] zeroinitializer, align 1
@__compressedAssemblyData_26 = internal dso_local global [33576 x i8] zeroinitializer, align 1
@__compressedAssemblyData_27 = internal dso_local global [15672 x i8] zeroinitializer, align 1
@__compressedAssemblyData_28 = internal dso_local global [15672 x i8] zeroinitializer, align 1
@__compressedAssemblyData_29 = internal dso_local global [89896 x i8] zeroinitializer, align 1
@__compressedAssemblyData_30 = internal dso_local global [255800 x i8] zeroinitializer, align 1
@__compressedAssemblyData_31 = internal dso_local global [48424 x i8] zeroinitializer, align 1
@__compressedAssemblyData_32 = internal dso_local global [48440 x i8] zeroinitializer, align 1
@__compressedAssemblyData_33 = internal dso_local global [126760 x i8] zeroinitializer, align 1
@__compressedAssemblyData_34 = internal dso_local global [102712 x i8] zeroinitializer, align 1
@__compressedAssemblyData_35 = internal dso_local global [17192 x i8] zeroinitializer, align 1
@__compressedAssemblyData_36 = internal dso_local global [26936 x i8] zeroinitializer, align 1
@__compressedAssemblyData_37 = internal dso_local global [42280 x i8] zeroinitializer, align 1
@__compressedAssemblyData_38 = internal dso_local global [315688 x i8] zeroinitializer, align 1
@__compressedAssemblyData_39 = internal dso_local global [16680 x i8] zeroinitializer, align 1
@__compressedAssemblyData_40 = internal dso_local global [19752 x i8] zeroinitializer, align 1
@__compressedAssemblyData_41 = internal dso_local global [50984 x i8] zeroinitializer, align 1
@__compressedAssemblyData_42 = internal dso_local global [23864 x i8] zeroinitializer, align 1
@__compressedAssemblyData_43 = internal dso_local global [1016632 x i8] zeroinitializer, align 1
@__compressedAssemblyData_44 = internal dso_local global [16168 x i8] zeroinitializer, align 1
@__compressedAssemblyData_45 = internal dso_local global [25384 x i8] zeroinitializer, align 1
@__compressedAssemblyData_46 = internal dso_local global [16680 x i8] zeroinitializer, align 1
@__compressedAssemblyData_47 = internal dso_local global [16168 x i8] zeroinitializer, align 1
@__compressedAssemblyData_48 = internal dso_local global [184632 x i8] zeroinitializer, align 1
@__compressedAssemblyData_49 = internal dso_local global [29480 x i8] zeroinitializer, align 1
@__compressedAssemblyData_50 = internal dso_local global [127272 x i8] zeroinitializer, align 1
@__compressedAssemblyData_51 = internal dso_local global [26408 x i8] zeroinitializer, align 1
@__compressedAssemblyData_52 = internal dso_local global [32056 x i8] zeroinitializer, align 1
@__compressedAssemblyData_53 = internal dso_local global [15656 x i8] zeroinitializer, align 1
@__compressedAssemblyData_54 = internal dso_local global [59192 x i8] zeroinitializer, align 1
@__compressedAssemblyData_55 = internal dso_local global [16680 x i8] zeroinitializer, align 1
@__compressedAssemblyData_56 = internal dso_local global [64824 x i8] zeroinitializer, align 1
@__compressedAssemblyData_57 = internal dso_local global [20776 x i8] zeroinitializer, align 1
@__compressedAssemblyData_58 = internal dso_local global [16696 x i8] zeroinitializer, align 1
@__compressedAssemblyData_59 = internal dso_local global [96552 x i8] zeroinitializer, align 1
@__compressedAssemblyData_60 = internal dso_local global [121640 x i8] zeroinitializer, align 1
@__compressedAssemblyData_61 = internal dso_local global [16168 x i8] zeroinitializer, align 1
@__compressedAssemblyData_62 = internal dso_local global [15656 x i8] zeroinitializer, align 1
@__compressedAssemblyData_63 = internal dso_local global [16168 x i8] zeroinitializer, align 1
@__compressedAssemblyData_64 = internal dso_local global [41256 x i8] zeroinitializer, align 1
@__compressedAssemblyData_65 = internal dso_local global [15656 x i8] zeroinitializer, align 1
@__compressedAssemblyData_66 = internal dso_local global [38192 x i8] zeroinitializer, align 1
@__compressedAssemblyData_67 = internal dso_local global [110384 x i8] zeroinitializer, align 1
@__compressedAssemblyData_68 = internal dso_local global [32552 x i8] zeroinitializer, align 1
@__compressedAssemblyData_69 = internal dso_local global [48424 x i8] zeroinitializer, align 1
@__compressedAssemblyData_70 = internal dso_local global [15664 x i8] zeroinitializer, align 1
@__compressedAssemblyData_71 = internal dso_local global [55096 x i8] zeroinitializer, align 1
@__compressedAssemblyData_72 = internal dso_local global [16168 x i8] zeroinitializer, align 1
@__compressedAssemblyData_73 = internal dso_local global [43816 x i8] zeroinitializer, align 1
@__compressedAssemblyData_74 = internal dso_local global [48944 x i8] zeroinitializer, align 1
@__compressedAssemblyData_75 = internal dso_local global [78640 x i8] zeroinitializer, align 1
@__compressedAssemblyData_76 = internal dso_local global [23864 x i8] zeroinitializer, align 1
@__compressedAssemblyData_77 = internal dso_local global [67880 x i8] zeroinitializer, align 1
@__compressedAssemblyData_78 = internal dso_local global [15656 x i8] zeroinitializer, align 1
@__compressedAssemblyData_79 = internal dso_local global [16168 x i8] zeroinitializer, align 1
@__compressedAssemblyData_80 = internal dso_local global [575800 x i8] zeroinitializer, align 1
@__compressedAssemblyData_81 = internal dso_local global [223528 x i8] zeroinitializer, align 1
@__compressedAssemblyData_82 = internal dso_local global [76584 x i8] zeroinitializer, align 1
@__compressedAssemblyData_83 = internal dso_local global [149288 x i8] zeroinitializer, align 1
@__compressedAssemblyData_84 = internal dso_local global [56104 x i8] zeroinitializer, align 1
@__compressedAssemblyData_85 = internal dso_local global [56624 x i8] zeroinitializer, align 1
@__compressedAssemblyData_86 = internal dso_local global [676656 x i8] zeroinitializer, align 1
@__compressedAssemblyData_87 = internal dso_local global [131880 x i8] zeroinitializer, align 1
@__compressedAssemblyData_88 = internal dso_local global [174888 x i8] zeroinitializer, align 1
@__compressedAssemblyData_89 = internal dso_local global [51984 x i8] zeroinitializer, align 1
@__compressedAssemblyData_90 = internal dso_local global [66360 x i8] zeroinitializer, align 1
@__compressedAssemblyData_91 = internal dso_local global [56104 x i8] zeroinitializer, align 1
@__compressedAssemblyData_92 = internal dso_local global [107320 x i8] zeroinitializer, align 1
@__compressedAssemblyData_93 = internal dso_local global [173368 x i8] zeroinitializer, align 1
@__compressedAssemblyData_94 = internal dso_local global [162088 x i8] zeroinitializer, align 1
@__compressedAssemblyData_95 = internal dso_local global [253744 x i8] zeroinitializer, align 1
@__compressedAssemblyData_96 = internal dso_local global [15656 x i8] zeroinitializer, align 1
@__compressedAssemblyData_97 = internal dso_local global [235304 x i8] zeroinitializer, align 1
@__compressedAssemblyData_98 = internal dso_local global [70952 x i8] zeroinitializer, align 1
@__compressedAssemblyData_99 = internal dso_local global [33592 x i8] zeroinitializer, align 1
@__compressedAssemblyData_100 = internal dso_local global [23848 x i8] zeroinitializer, align 1
@__compressedAssemblyData_101 = internal dso_local global [52008 x i8] zeroinitializer, align 1
@__compressedAssemblyData_102 = internal dso_local global [103224 x i8] zeroinitializer, align 1
@__compressedAssemblyData_103 = internal dso_local global [17704 x i8] zeroinitializer, align 1
@__compressedAssemblyData_104 = internal dso_local global [16184 x i8] zeroinitializer, align 1
@__compressedAssemblyData_105 = internal dso_local global [15664 x i8] zeroinitializer, align 1
@__compressedAssemblyData_106 = internal dso_local global [41768 x i8] zeroinitializer, align 1
@__compressedAssemblyData_107 = internal dso_local global [852264 x i8] zeroinitializer, align 1
@__compressedAssemblyData_108 = internal dso_local global [103208 x i8] zeroinitializer, align 1
@__compressedAssemblyData_109 = internal dso_local global [153912 x i8] zeroinitializer, align 1
@__compressedAssemblyData_110 = internal dso_local global [3098408 x i8] zeroinitializer, align 1
@__compressedAssemblyData_111 = internal dso_local global [38696 x i8] zeroinitializer, align 1
@__compressedAssemblyData_112 = internal dso_local global [16168 x i8] zeroinitializer, align 1
@__compressedAssemblyData_113 = internal dso_local global [16168 x i8] zeroinitializer, align 1
@__compressedAssemblyData_114 = internal dso_local global [130360 x i8] zeroinitializer, align 1
@__compressedAssemblyData_115 = internal dso_local global [15656 x i8] zeroinitializer, align 1
@__compressedAssemblyData_116 = internal dso_local global [501560 x i8] zeroinitializer, align 1
@__compressedAssemblyData_117 = internal dso_local global [16168 x i8] zeroinitializer, align 1
@__compressedAssemblyData_118 = internal dso_local global [24360 x i8] zeroinitializer, align 1
@__compressedAssemblyData_119 = internal dso_local global [16688 x i8] zeroinitializer, align 1
@__compressedAssemblyData_120 = internal dso_local global [15648 x i8] zeroinitializer, align 1
@__compressedAssemblyData_121 = internal dso_local global [16184 x i8] zeroinitializer, align 1
@__compressedAssemblyData_122 = internal dso_local global [26920 x i8] zeroinitializer, align 1
@__compressedAssemblyData_123 = internal dso_local global [15656 x i8] zeroinitializer, align 1
@__compressedAssemblyData_124 = internal dso_local global [17720 x i8] zeroinitializer, align 1
@__compressedAssemblyData_125 = internal dso_local global [18208 x i8] zeroinitializer, align 1
@__compressedAssemblyData_126 = internal dso_local global [15656 x i8] zeroinitializer, align 1
@__compressedAssemblyData_127 = internal dso_local global [38696 x i8] zeroinitializer, align 1
@__compressedAssemblyData_128 = internal dso_local global [15656 x i8] zeroinitializer, align 1
@__compressedAssemblyData_129 = internal dso_local global [64808 x i8] zeroinitializer, align 1
@__compressedAssemblyData_130 = internal dso_local global [17704 x i8] zeroinitializer, align 1
@__compressedAssemblyData_131 = internal dso_local global [16168 x i8] zeroinitializer, align 1
@__compressedAssemblyData_132 = internal dso_local global [143144 x i8] zeroinitializer, align 1
@__compressedAssemblyData_133 = internal dso_local global [66360 x i8] zeroinitializer, align 1
@__compressedAssemblyData_134 = internal dso_local global [16184 x i8] zeroinitializer, align 1
@__compressedAssemblyData_135 = internal dso_local global [23856 x i8] zeroinitializer, align 1
@__compressedAssemblyData_136 = internal dso_local global [17192 x i8] zeroinitializer, align 1
@__compressedAssemblyData_137 = internal dso_local global [17192 x i8] zeroinitializer, align 1
@__compressedAssemblyData_138 = internal dso_local global [44840 x i8] zeroinitializer, align 1
@__compressedAssemblyData_139 = internal dso_local global [58664 x i8] zeroinitializer, align 1
@__compressedAssemblyData_140 = internal dso_local global [54056 x i8] zeroinitializer, align 1
@__compressedAssemblyData_141 = internal dso_local global [17704 x i8] zeroinitializer, align 1
@__compressedAssemblyData_142 = internal dso_local global [16680 x i8] zeroinitializer, align 1
@__compressedAssemblyData_143 = internal dso_local global [16168 x i8] zeroinitializer, align 1
@__compressedAssemblyData_144 = internal dso_local global [16168 x i8] zeroinitializer, align 1
@__compressedAssemblyData_145 = internal dso_local global [15656 x i8] zeroinitializer, align 1
@__compressedAssemblyData_146 = internal dso_local global [16168 x i8] zeroinitializer, align 1
@__compressedAssemblyData_147 = internal dso_local global [17208 x i8] zeroinitializer, align 1
@__compressedAssemblyData_148 = internal dso_local global [705288 x i8] zeroinitializer, align 1
@__compressedAssemblyData_149 = internal dso_local global [38184 x i8] zeroinitializer, align 1
@__compressedAssemblyData_150 = internal dso_local global [15656 x i8] zeroinitializer, align 1
@__compressedAssemblyData_151 = internal dso_local global [15656 x i8] zeroinitializer, align 1
@__compressedAssemblyData_152 = internal dso_local global [18728 x i8] zeroinitializer, align 1
@__compressedAssemblyData_153 = internal dso_local global [17192 x i8] zeroinitializer, align 1
@__compressedAssemblyData_154 = internal dso_local global [16168 x i8] zeroinitializer, align 1
@__compressedAssemblyData_155 = internal dso_local global [741160 x i8] zeroinitializer, align 1
@__compressedAssemblyData_156 = internal dso_local global [16168 x i8] zeroinitializer, align 1
@__compressedAssemblyData_157 = internal dso_local global [16176 x i8] zeroinitializer, align 1
@__compressedAssemblyData_158 = internal dso_local global [70440 x i8] zeroinitializer, align 1
@__compressedAssemblyData_159 = internal dso_local global [617776 x i8] zeroinitializer, align 1
@__compressedAssemblyData_160 = internal dso_local global [369456 x i8] zeroinitializer, align 1
@__compressedAssemblyData_161 = internal dso_local global [57144 x i8] zeroinitializer, align 1
@__compressedAssemblyData_162 = internal dso_local global [16168 x i8] zeroinitializer, align 1
@__compressedAssemblyData_163 = internal dso_local global [186152 x i8] zeroinitializer, align 1
@__compressedAssemblyData_164 = internal dso_local global [16176 x i8] zeroinitializer, align 1
@__compressedAssemblyData_165 = internal dso_local global [61752 x i8] zeroinitializer, align 1
@__compressedAssemblyData_166 = internal dso_local global [17208 x i8] zeroinitializer, align 1
@__compressedAssemblyData_167 = internal dso_local global [16184 x i8] zeroinitializer, align 1
@__compressedAssemblyData_168 = internal dso_local global [16168 x i8] zeroinitializer, align 1
@__compressedAssemblyData_169 = internal dso_local global [15672 x i8] zeroinitializer, align 1
@__compressedAssemblyData_170 = internal dso_local global [45352 x i8] zeroinitializer, align 1
@__compressedAssemblyData_171 = internal dso_local global [175912 x i8] zeroinitializer, align 1
@__compressedAssemblyData_172 = internal dso_local global [16672 x i8] zeroinitializer, align 1
@__compressedAssemblyData_173 = internal dso_local global [15656 x i8] zeroinitializer, align 1
@__compressedAssemblyData_174 = internal dso_local global [30520 x i8] zeroinitializer, align 1
@__compressedAssemblyData_175 = internal dso_local global [15656 x i8] zeroinitializer, align 1
@__compressedAssemblyData_176 = internal dso_local global [16184 x i8] zeroinitializer, align 1
@__compressedAssemblyData_177 = internal dso_local global [16168 x i8] zeroinitializer, align 1
@__compressedAssemblyData_178 = internal dso_local global [22320 x i8] zeroinitializer, align 1
@__compressedAssemblyData_179 = internal dso_local global [16680 x i8] zeroinitializer, align 1
@__compressedAssemblyData_180 = internal dso_local global [16168 x i8] zeroinitializer, align 1
@__compressedAssemblyData_181 = internal dso_local global [16168 x i8] zeroinitializer, align 1
@__compressedAssemblyData_182 = internal dso_local global [16168 x i8] zeroinitializer, align 1
@__compressedAssemblyData_183 = internal dso_local global [16168 x i8] zeroinitializer, align 1
@__compressedAssemblyData_184 = internal dso_local global [18224 x i8] zeroinitializer, align 1
@__compressedAssemblyData_185 = internal dso_local global [23848 x i8] zeroinitializer, align 1
@__compressedAssemblyData_186 = internal dso_local global [50984 x i8] zeroinitializer, align 1
@__compressedAssemblyData_187 = internal dso_local global [16680 x i8] zeroinitializer, align 1
@__compressedAssemblyData_188 = internal dso_local global [60200 x i8] zeroinitializer, align 1
@__compressedAssemblyData_189 = internal dso_local global [101160 x i8] zeroinitializer, align 1
@__compressedAssemblyData_190 = internal dso_local global [240168 x i8] zeroinitializer, align 1
@__compressedAssemblyData_191 = internal dso_local global [83000 x i8] zeroinitializer, align 1
@__compressedAssemblyData_192 = internal dso_local global [19000 x i8] zeroinitializer, align 1
@__compressedAssemblyData_193 = internal dso_local global [37449248 x i8] zeroinitializer, align 1
@__compressedAssemblyData_194 = internal dso_local global [4804392 x i8] zeroinitializer, align 1

; Metadata
!llvm.module.flags = !{!0, !1, !7, !8, !9, !10}
!0 = !{i32 1, !"wchar_size", i32 4}
!1 = !{i32 7, !"PIC Level", i32 2}
!llvm.ident = !{!2}
!2 = !{!".NET for Android remotes/origin/release/9.0.1xx @ 0ccdc57cf7fc59bd3f6cbf900c9cdbebadfe4609"}
!3 = !{!4, !4, i64 0}
!4 = !{!"any pointer", !5, i64 0}
!5 = !{!"omnipotent char", !6, i64 0}
!6 = !{!"Simple C++ TBAA"}
!7 = !{i32 1, !"branch-target-enforcement", i32 0}
!8 = !{i32 1, !"sign-return-address", i32 0}
!9 = !{i32 1, !"sign-return-address-all", i32 0}
!10 = !{i32 1, !"sign-return-address-with-bkey", i32 0}
