using System.Reflection;
using BenchmarkDotNet.Running;
using OneBRC.Benchmark;

BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run();