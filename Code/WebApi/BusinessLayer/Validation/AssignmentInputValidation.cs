using DatabaseLayer.DataAccessLayer;
using DatabaseLayer.RepositoryLayer;
using ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Validation
{
    public class AssignmentInputValidation
    {
        private DbAssignmentIF dbA;

        public AssignmentInputValidation()
        {
            dbA = new DbAssignment();
        }

        private const int _titleMinLength = 5;
        private const int _titleMaxLength = 30;

        private const int _descriptionMinLength = 1;
        private const int _descriptionMaxLength = 500;

        private const int _priceMinValue = 0;
        private const int _priceMaxValue = 10000;

        public bool CheckInput(Assignment assignment) 
        {
            //default return is true
            bool value = true;
            foreach (var item in assignment.GetType().GetProperties())
            {
                switch (item.Name)
                {
                    case "Title":
                        value = assignment.Title.Length > _titleMinLength && assignment.Title.Length < _titleMaxLength;
                        break;

                    case "Description":
                        value = assignment.Description.Length > _descriptionMinLength && assignment.Description.Length < _descriptionMaxLength;
                        break;

                    case "Price":
                        value = assignment.Price > _priceMinValue && assignment.Price < _priceMaxValue;
                        break;

                    case "Deadline":
                        value = DateTime.Compare(assignment.Deadline, DateTime.Now) > 0;
                        break;

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

                    default:
                        break;
                }
                if (!value) break;
            }
            return value;
        }
    }
}
