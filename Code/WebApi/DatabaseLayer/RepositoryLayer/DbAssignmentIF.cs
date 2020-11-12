﻿using ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLayer.RepositoryLayer
{
    public interface DbAssignmentIF
    {
        List<Assignment> GetAllAssignments();
        int CreateAssignment(Assignment assignment);
        Assignment GetByAssignmentId(int id);
        int UpdateAssignment(Assignment assignment, int id);
        int DeleteAssignment(int id);
    }
}
