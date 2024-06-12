using BenchmarkDotNet.Running;
using System;
using System.Text;

namespace Alluseri.Riza;

public class Program {
	public static void Main(string[] Args) {
		BenchmarkRunner.Run<BenchmarkPack>();
	}
}