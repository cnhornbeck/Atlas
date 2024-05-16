// using UnityEngine;
// using Unity.Collections;
// using System;
// using System.Collections.Generic;
// using Unity.Jobs;

// public class JobManager : MonoBehaviour
// {
//     static List<IJobOrganizer> jobOrganizers = new List<IJobOrganizer>();

//     void Update()
//     {
//         ManageJobs();
//     }

//     public static void RegisterJob<TParam, TJobDataResult, TResult>(
//         Func<TParam, JobData<TJobDataResult>> scheduleJobMethod,
//         Func<JobData<TJobDataResult>, TResult> completeJobMethod,
//         TParam input,
//         ref TResult output,
//         Action scheduleNextJob = null
//     ) where TJobDataResult : struct
//     {
//         JobData<TJobDataResult> jobData = scheduleJobMethod(input);
//         JobOrganizer<TJobDataResult, TResult> jobOrganizer = new JobOrganizer<TJobDataResult, TResult>(
//             jobData, ref output, completeJobMethod, scheduleNextJob
//         );
//         jobOrganizers.Add(jobOrganizer);
//     }

//     public static void ManageJobs()
//     {
//         List<IJobOrganizer> completedJobOrganizers = new List<IJobOrganizer>();

//         foreach (var jobOrganizer in jobOrganizers)
//         {
//             if (jobOrganizer.GetJobHandle().IsCompleted)
//             {
//                 jobOrganizer.CompleteJob();
//                 completedJobOrganizers.Add(jobOrganizer);

//                 jobOrganizer.InvokeCompletionDelegate();
//             }
//         }

//         foreach (var completedJobOrganizer in completedJobOrganizers)
//         {
//             jobOrganizers.Remove(completedJobOrganizer);
//         }
//     }
// }



// public class JobOrganizer<T, TResult> : IJobOrganizer where T : struct
// {
//     private JobData<T> jobData;
//     public TResult output;
//     private Func<JobData<T>, TResult> completeJobMethod;
//     private Action jobCompletionDelegate;

//     public JobOrganizer(
//         JobData<T> jobData, ref TResult output, 
//         Func<JobData<T>, TResult> completeJobMethod, 
//         Action jobCompletionDelegate)
//     {
//         this.jobData = jobData;
//         this.output = output;
//         this.completeJobMethod = completeJobMethod;
//         this.jobCompletionDelegate = jobCompletionDelegate;
//     }

//     public void CompleteJob()
//     {
//         output = completeJobMethod(jobData);
//     }

//     public JobHandle GetJobHandle()
//     {
//         return jobData.jobHandle;
//     }

//     public void InvokeCompletionDelegate()
//     {
//         jobCompletionDelegate?.Invoke();
//     }
// }

// public interface IJobOrganizer
// {
//     void CompleteJob();
//     JobHandle GetJobHandle();
//     void InvokeCompletionDelegate();
// }
