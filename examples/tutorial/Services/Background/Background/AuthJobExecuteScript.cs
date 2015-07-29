﻿/*
 * AuthJobExecuteScript.cs
 *
 * Copyright (C) 2010-2014 by Revolution Analytics Inc.
 *
 * This program is licensed to you under the terms of Version 2.0 of the
 * Apache License. This program is distributed WITHOUT
 * ANY EXPRESS OR IMPLIED WARRANTY, INCLUDING THOSE OF NON-INFRINGEMENT,
 * MERCHANTABILITY OR FITNESS FOR A PARTICULAR PURPOSE. Please refer to the
 * Apache License 2.0 (http://www.apache.org/licenses/LICENSE-2.0) for more details.
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DeployR;

namespace Background
{
    public class AuthJobExecuteScript
    {
        static public void Execute()
        {

            Console.WriteLine("AuthJobExecuteScript - start");
 
            // 
            // 1. Connect to the DeployR Server
            //
            RClient rClient = Utility.Connect();

            // 
            // 2. Authenticate the user
            //
            RUser rUser = Utility.Authenticate(rClient);

            //
            // 3. Submit a background job for execution based on a
            // repository-managed R script: /testuser/root/Histogram of Auto Sales.R
            //
            JobExecutionOptions options = new JobExecutionOptions();
            options.priority = JobExecutionOptions.HIGH_PRIORITY;  //Make this a High Priority job
            RJob rJob = rUser.submitJobScript("Background Script Execution",
                                                "Background script execution.",
                                                "Histogram of Auto Sales",
                                                "root",
                                                "testuser",
                                                "",
                                                options);

            Console.WriteLine("AuthJobExecuteScript: submitted background job " +
                                        "for execution, rJob=" + rJob);
            
            //
            // 4. Query the execution status of a background job and loop until the job has finished
            //
            if (rJob != null)
            {
                while (true)
                {
                    String sMsg = rJob.query().status.Value;
                    
                    if (sMsg == RJob.Status.COMPLETED.Value |
                        sMsg == RJob.Status.FAILED.Value |
                        sMsg == RJob.Status.CANCELLED.Value |
                        sMsg == RJob.Status.ABORTED.Value)
                    {
                        break;
                    }
                    else
                    {
                        Thread.Sleep(500);
                    }
                }
            }


            //
            // 5. Retrieve the project from completed job
            //
            RProject rProject = null;
            if (rJob != null)
            {
                // make sure we have a valid project id
                if (rJob.query().project.Length > 0)
                {
                    //get the project using the project id
                    rProject = rUser.getProject(rJob.query().project);
                
                    Console.WriteLine("AuthJobExecuteScript: retrieved background " +
                           "job result on project, rProject=" + rProject);
                }
            }

            //
            //  6. Cleanup
            //
            if (rProject != null)
            {
                rProject.close();
                //rProject.delete();  //un-comment if you wish to delete the project
            }

            if (rJob != null)
            {
                //rJob.delete();  //un-comment if you wish to delete the job
            }

            Utility.Cleanup(rUser, rClient);

            Console.WriteLine("AuthJobExecuteScript - end");
        }
    }
}
