﻿using DatabaseLayer.DataAccessLayer;
using DatabaseLayer.RepositoryLayer;
using ModelLayer;
using System;

namespace BusinessLayer.Validation
{
    public class AssignmentInputValidation
    {
        private IDbAssignment dbA;
        private readonly int _titleMinLength = 1;
        private readonly int _titleMaxLength = 75;
        private readonly int _descriptionMinLength = 1;
        private readonly int _descriptionMaxLength = 1200;
        private readonly int _priceMinValue = 0;
        private readonly int _priceMaxValue = 10000;

        public AssignmentInputValidation()
        {
            dbA = new DbAssignment();
        }

        public bool CheckInput(Assignment assignment)
        {
            //default return is true
            bool value = true;
            foreach (var item in assignment.GetType().GetProperties())
            {
                switch (item.Name)
                {
                    #region Assignment.Title
                    case "Title":
                        value = assignment.Title.Length >= _titleMinLength && assignment.Title.Length <= _titleMaxLength;
                        break;
                    #endregion
                    #region Assignment.Description
                    case "Description":
                        value = assignment.Description.Length >= _descriptionMinLength && assignment.Description.Length <= _descriptionMaxLength;
                        break;
                    #endregion
                    #region Assignment.Price
                    case "Price":
                        value = assignment.Price > _priceMinValue && assignment.Price < _priceMaxValue;
                        break;
                    #endregion
                    #region Assignment.Deadline
                    case "Deadline":
                        value = DateTime.Compare(assignment.Deadline, DateTime.UtcNow) > 0;
                        break;
                    #endregion
                    #region Assignment.AcademicLevel
                    case "AcademicLevel":
                        var academicLevels = dbA.GetAllAcademicLevels();

                        foreach (var al in academicLevels)
                        {
                            if (assignment.AcademicLevel.Equals(al))
                            {
                                value = true;
                                break;
                            }
                            value = false;
                        }
                        break;
                    #endregion
                    #region Assignment.Subject
                    case "Subject":
                        var subjects = dbA.GetAllSubjects();

                        foreach (var s in subjects)
                        {
                            if (assignment.Subject.Equals(s))
                            {
                                value = true;
                                break;
                            }
                            value = false;
                        }
                        break;
                    #endregion
                    default:
                        break;
                }
                if (!value) break;
            }
            return value;
        }
    }
}
