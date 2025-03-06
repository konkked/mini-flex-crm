using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MiniFlexCrmApi.Cache;

public class ResourceBasedThresholdProvider : IResourceBasedThresholdProvider
{
    private const int SMALL_THRESHOLD = 2000;  // Small: ≤ 2 cores or ≤ 2GB
    private const int MEDIUM_THRESHOLD = 4000; // Medium: ≤ 8 cores or ≤ 8GB
    private const int LARGE_THRESHOLD = 15000; // Large: > 8 cores or > 8GB

    public int GetGlobalRpsThreshold()
    {
        try
        {
            // CPU threshold
            var cpuThreshold = Environment.ProcessorCount switch
            {
                <= 2 => SMALL_THRESHOLD,
                <= 8 => MEDIUM_THRESHOLD,
                _ => LARGE_THRESHOLD
            };

            // Memory threshold
            var memoryThreshold = GetAvailableMemoryMb() switch
            {
                <= 2048 => SMALL_THRESHOLD,
                <= 8192 => MEDIUM_THRESHOLD,
                _ => LARGE_THRESHOLD
            };

            // Take the smallest
            return Math.Min(cpuThreshold, memoryThreshold);
        }
        catch
        {
            return SMALL_THRESHOLD;
        }
    }

    private long GetAvailableMemoryMb()
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return GetWindowsAvailableMemoryMb();
                
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return GetLinuxAvailableMemoryMb();
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return GetMacOSAvailableMemoryMB();
            
            return GC.GetTotalMemory(false) / (1024 * 1024);
        }
        catch
        {
            // Fallback to the smallest threshold on error
            return 0; 
        }
    }

    private long GetWindowsAvailableMemoryMb()
    {
        using var counter = new PerformanceCounter("Memory", "Available MBytes", true);
        return (long)counter.NextValue();
    }

    private long GetLinuxAvailableMemoryMb()
    {
        var memInfo = File.ReadAllText("/proc/meminfo");
        foreach (var line in memInfo.Split('\n'))
        {
            if (!line.StartsWith("MemAvailable:")) 
                continue;
            
            var parts = line.Split([' '], StringSplitOptions.RemoveEmptyEntries);
            var kb = long.Parse(parts[1]);
            return kb / 1024; // KB to MB
        }
        throw new InvalidOperationException("MemAvailable not found in /proc/meminfo");
    }

    private long GetMacOSAvailableMemoryMB()
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "sysctl",
                Arguments = "hw.memsize",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        if (!output.StartsWith("hw.memsize:"))
            throw new InvalidOperationException("Failed to retrieve hw.memsize on macOS");
        
        var totalBytes = long.Parse(output.Split(' ')[1].Trim());
        var totalMB = totalBytes / (1024 * 1024);
        return totalMB / 2; // Approximate 50% available
    }
}