// using Unity.Collections;
// using Unity.Jobs;
// using UnityEngine;

// public class NoiseGenJobScheduler : MonoBehaviour
// {
//     public int width = 16;
//     public int height = 16;
//     public float scale = 20.0f;

//     private NativeArray<float> heights;
//     private JobHandle jobHandle;
//     private bool jobScheduled = false;

//     void Update()
//     {
//         if (!jobScheduled)
//         {
//             // Allocate memory for the heights only once
//             heights = new NativeArray<float>(width * height, Allocator.Persistent);

//             // Setup the job
//             NoiseGenJob job = new()
//             {
//                 heights = heights,
//                 scale = scale
//             };

//             // Schedule the job
//             jobHandle = job.Schedule(heights.Length, 64);
//             jobScheduled = true;
//         }
//         else if (jobHandle.IsCompleted)
//         {
//             // Complete the job handle
//             jobHandle.Complete();

//             // Process the results here (if necessary)
//             // Debug.Log("Done");

//             // Dispose of the heights array
//             heights.Dispose();

//             // Reset the flag
//             jobScheduled = false;
//         }
//     }

//     void OnDestroy()
//     {
//         // Ensure the array is disposed if the GameObject is destroyed unexpectedly
//         if (heights.IsCreated)
//         {
//             heights.Dispose();
//         }
//     }
// }
