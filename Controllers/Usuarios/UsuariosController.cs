using GestaoUsuarios.Data;
using GestaoUsuarios.Models.Usuarios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoUsuarios.Controllers.Usuarios;

public class UsuariosController(ApplicationDbContext context, ILogger<UsuariosController> logger) : Controller
{
    [HttpGet]
    public IActionResult Index() => View();

    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        try
        {
            var usuarios = await context.Usuarios.AsNoTracking().OrderBy(u => u.Nome).ToListAsync(cancellationToken);
            return Json(new { sucesso = true, dados = usuarios });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao consultar usuários");
            return StatusCode(500, new { sucesso = false, mensagem = "Não foi possível consultar os usuários." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Obter(int id, CancellationToken cancellationToken)
    {
        var usuario = await context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        return usuario is null
            ? NotFound(new { sucesso = false, mensagem = "Usuário não encontrado." })
            : Json(new { sucesso = true, dados = usuario });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Salvar([FromBody] UsuarioInput input, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { sucesso = false, mensagem = "Revise os campos informados.", erros = ModelState.ToDictionary(x => x.Key, x => x.Value!.Errors.Select(e => e.ErrorMessage)) });

        var usuario = input.ToEntity();
        var regras = usuario.ValidarRegrasDeNegocio().ToList();
        if (regras.Count > 0)
            return BadRequest(new { sucesso = false, mensagem = "Existem regras de negócio a corrigir.", erros = regras.Select(r => r.ErrorMessage) });

        try
        {
            if (input.Id == 0)
                context.Usuarios.Add(usuario);
            else
            {
                var existente = await context.Usuarios.FirstOrDefaultAsync(u => u.Id == input.Id, cancellationToken);
                if (existente is null) return NotFound(new { sucesso = false, mensagem = "Usuário não encontrado." });
                existente.Nome = usuario.Nome;
                existente.ValorHora = usuario.ValorHora;
                existente.DataCadastro = usuario.DataCadastro;
                existente.Ativo = usuario.Ativo;
            }

            await context.SaveChangesAsync(cancellationToken);
            return Json(new { sucesso = true, mensagem = "Usuário salvo com sucesso.", dados = usuario });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao salvar usuário");
            return StatusCode(500, new { sucesso = false, mensagem = "Não foi possível salvar o usuário." });
        }
    }

    [HttpDelete]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Excluir(int id, CancellationToken cancellationToken)
    {
        try
        {
            var usuario = await context.Usuarios.FindAsync([id], cancellationToken);
            if (usuario is null) return NotFound(new { sucesso = false, mensagem = "Usuário não encontrado." });
            context.Usuarios.Remove(usuario);
            await context.SaveChangesAsync(cancellationToken);
            return Json(new { sucesso = true, mensagem = "Usuário excluído com sucesso." });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao excluir usuário {Id}", id);
            return StatusCode(500, new { sucesso = false, mensagem = "Não foi possível excluir o usuário." });
        }
    }
}
