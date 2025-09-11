using System.ComponentModel.DataAnnotations;

namespace InsureYouAI.Dtos
{
    public class CreateUserRegisterDto
    {
        [Required(ErrorMessage = "Ad alanı boş bırakılamaz.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Soyad alanı boş bırakılamaz.")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Email alanı boş bırakılamaz.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email girin.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı boş bırakılamaz.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Şifre alanı boş bırakılamaz.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Kullanım şartlarını kabul etmeniz gerekmektedir.")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "Kullanım şartlarını kabul etmeniz gerekiyor.")]
        public bool TermsAccepted { get; set; }
    }
}
