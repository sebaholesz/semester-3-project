using ModelLayer;

namespace BusinessLayer.Validation
{
    public class SolutionInputValidation
    {
        private readonly int _descriptionMinLength = 1;
        private readonly int _descriptionMaxLength = 1200;

        public bool CheckInput(Solution solution)
        {
            bool value = solution.Description.Length > _descriptionMinLength && solution.Description.Length < _descriptionMaxLength;
            return value;
        }
    }
}
