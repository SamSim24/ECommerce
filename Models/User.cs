using System.ComponentModel.DataAnnotations;

namespace INF27523_TP1_ML_SS.Models
{
    public class User
    {
        public int Id { get; set; }

        [ValidateObjectMembers]
        public NameModel Name { get; set; } = null!;

        [Required(ErrorMessage = "L'adresse email est requise")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Le nom d'utilisateur est requis")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Le nom d'utilisateur doit avoir entre 3 et 50 caractères")]
        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public bool EstVendeur { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();

        public class NameModel
        {
            [Required(ErrorMessage = "Le prénom est requis")]
            [StringLength(50, ErrorMessage = "Le prénom ne peut pas dépasser 50 caractères")]
            public string Firstname { get; set; } = null!;

            [Required(ErrorMessage = "Le nom de famille est requis")]
            [StringLength(50, ErrorMessage = "Le nom de famille ne peut pas dépasser 50 caractères")]
            public string Lastname { get; set; } = null!;
        }
    }

    // Attribut personnalisé pour valider les objets imbriqués
    public class ValidateObjectMembersAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("L'objet ne peut pas être null");
            }

            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(value, serviceProvider: null, items: null);

            Validator.TryValidateObject(value, context, validationResults, validateAllProperties: true);

            if (validationResults.Count > 0)
            {
                return new ValidationResult(validationResults[0].ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }

}