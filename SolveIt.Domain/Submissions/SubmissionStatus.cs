using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIt.Domain.Submissions
{
    public enum SubmissionStatus
    {
        Pending = 0,     // Created but not queued
        Queued = 1,      // Sent to Kafka
        Running = 2,     // Executing in sandbox
        Succeeded = 3,   // Passed all tests
        Failed = 4,      // Logical/runtime failure
        TimedOut = 5,    // Exceeded time limit
        Rejected = 6     // Invalid / disqualified
    }
}
