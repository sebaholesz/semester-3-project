﻿using BusinessLayer.Validation;
using DatabaseLayer.DataAccessLayer;
using DatabaseLayer.RepositoryLayer;
using ModelLayer;
using System;
using System.Collections.Generic;

namespace BusinessLayer
{
    public sealed class SolutionBusiness
    {
        private static readonly SolutionBusiness _solutionBusinessInstance = new SolutionBusiness();
        private readonly IDbSolution _dbSolution;
        private readonly SolutionInputValidation _validateSolution;

        private SolutionBusiness()
        {
            _dbSolution = new DbSolution();
            _validateSolution = new SolutionInputValidation();
        }

        public static SolutionBusiness GetSolutionBusiness()
        {
            return _solutionBusinessInstance;
        }

        public int CreateSolution(Solution solution)
        {
            if (_validateSolution.CheckInput(solution))
            {
                int result = _dbSolution.CreateSolution(solution);
                if (result > 0)
                {
                    int queueLengthAfter = GetSolutionsCountByAssignmentId(solution.AssignmentId);
                    return queueLengthAfter + 1;
                }
            }
            return -1;
        }

        public List<Solution> GetAllSolutions()
        {
            return _dbSolution.GetAllSolutions();
        }

        public List<Solution> GetSolutionsByAssignmentId(int id)
        {
            return _dbSolution.GetSolutionsByAssignmentId(id);
        }

        public int GetSolutionsCountByAssignmentId(int assignmentId)
        {
            return _dbSolution.GetSolutionsCountByAssignmentId(assignmentId);
        }

        public Solution GetBySolutionId(int id)
        {
            return _dbSolution.GetBySolutionId(id);
        }

        public int UpdateSolution(Solution solution, int id)
        {

            return _dbSolution.UpdateSolution(solution, id);
        }
        
        public int DeleteSolution(int id)
        {
            return _dbSolution.DeleteSolution(id);
        }

        public bool ChooseSolution(int solutionId, int assignmentId)
        {
            bool successfulyAccepted = _dbSolution.ChooseSolution(solutionId) == 1 ? true : false; ;
            bool successfulyMadeInactive = AssignmentBusiness.GetAssignmentBusiness().MakeAssignmentInactive(assignmentId) == 1 ? true : false;
            return successfulyAccepted && successfulyMadeInactive;
        }

        public Solution GetSolutionByAssignmentId(int assignmentId)
        {
            Solution solution = _dbSolution.GetSolutionByAssignmentId(assignmentId);

            if (!solution.Equals(null))
            {
                return solution;
            }
            else
            {
                throw new Exception("Could not find your solution");
            }
        }

        public List<string> GetAllSolversForAssignment(int assignmentId)
        {
            return _dbSolution.GetAllSolversForAssignment(assignmentId);
        }

        public Solution GetAcceptedSolutionForAssignment(int assignmentId)
        {
            try
            {
                Solution solution = _dbSolution.GetAcceptedSolutionForAssignment(assignmentId);
                return solution ?? null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
