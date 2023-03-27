using BenchmarkDotNet.Running;

namespace Alluseri.Riza;

public class Program {
	public static void Main(string[] Args) {
		BenchmarkRunner.Run<Bench>();
	}
}