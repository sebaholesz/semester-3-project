using ModelLayer;

namespace BusinessLayer.Validation
{
    public class SolutionInputValidation
    {
        private const int _descriptionMinLength = 1;
        private const int _descriptionMaxLength = 500;


        public bool CheckInput(Solution solution)
        {
            bool value = solution.Description.Length > _descriptionMinLength && solution.Description.Length < _descriptionMaxLength;
            return value;

        }
    }
}
