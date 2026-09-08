using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoUsuarios.Models.Usuarios;

[Table("Usuario")]
public class Usuario
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 150 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Range(typeof(decimal), "0,01", "999999999,99", ErrorMessage = "Informe um valor por hora válido.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ValorHora { get; set; }

    [Required(ErrorMessage = "Informe a data de cadastro.")]
    public DateTime DataCadastro { get; set; } = DateTime.Now;

    public bool Ativo { get; set; } = true;

    public IEnumerable<ValidationResult> ValidarRegrasDeNegocio()
    {
        if (DataCadastro > DateTime.Now.AddMinutes(1))
            yield return new ValidationResult("A data de cadastro não pode estar no futuro.", new[] { nameof(DataCadastro) });

        if (Nome.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Length < 2)
            yield return new ValidationResult("Informe nome e sobrenome.", new[] { nameof(Nome) });
    }
}

public class UsuarioInput
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 150 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Range(typeof(decimal), "0,01", "999999999,99", ErrorMessage = "Informe um valor por hora válido.")]
    public decimal ValorHora { get; set; }

    [Required(ErrorMessage = "Informe a data de cadastro.")]
    public DateTime DataCadastro { get; set; } = DateTime.Now;

    public bool Ativo { get; set; } = true;

    public Usuario ToEntity() => new()
    {
        Id = Id,
        Nome = Nome.Trim(),
        ValorHora = ValorHora,
        DataCadastro = DataCadastro,
        Ativo = Ativo
    };
}
