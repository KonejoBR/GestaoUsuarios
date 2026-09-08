$(function () {
    const modal = new bootstrap.Modal('#modalUsuario');
    const token = () => $('#formUsuario input[name="__RequestVerificationToken"]').val();

    function mensagem(texto, tipo = 'success') {
        $('#mensagem').removeClass('d-none alert-success alert-danger').addClass(`alert-${tipo}`).text(texto);
    }

    function retornaDataHoraFormatada(valor = new Date()) {
        const data = valor instanceof Date ? valor : new Date(valor);
        const dois = numero => String(numero).padStart(2, '0');

        return `${data.getFullYear()}-${dois(data.getMonth() + 1)}-${dois(data.getDate())}T${dois(data.getHours())}:${dois(data.getMinutes())}`;
    }

    function formatarData(valor) {
        return new Intl.DateTimeFormat('pt-BR', { dateStyle: 'short', timeStyle: 'short' }).format(new Date(valor));
    }

    function limparErros() { $('#formUsuario .is-invalid').removeClass('is-invalid'); $('#formUsuario .invalid-feedback').text(''); }

    function validarUsuario() {
        limparErros(); let valido = true;
        const nome = $('#Nome').val().trim();
        if (nome.length < 2 || nome.split(/\s+/).length < 2) { $('#Nome').addClass('is-invalid').next('.invalid-feedback').text('Informe nome e sobrenome.'); valido = false; }
        if (Number($('#ValorHora').val()) <= 0) { $('#ValorHora').addClass('is-invalid').next('.invalid-feedback').text('Informe um valor maior que zero.'); valido = false; }
        if (!$('#DataCadastro').val()) { $('#DataCadastro').addClass('is-invalid').next('.invalid-feedback').text('Informe a data de cadastro.'); valido = false; }
        return valido;
    }

    function carregarUsuarios() {
        $.getJSON('/Usuarios/Listar').done(function (res) {  
            const tbody = $('#tabelaUsuarios tbody').empty();
            if (!res.dados.length) { tbody.append('<tr><td colspan="5" class="text-center text-muted py-4">Nenhum usuário cadastrado.</td></tr>'); return; }
            res.dados.forEach(u => tbody.append(`<tr><td>${$('<div>').text(u.nome).html()}</td><td>R$ ${Number(u.valorHora).toFixed(2).replace('.', ',')}</td><td>${retornaDataHoraFormatada(u.dataCadastro)}</td><td><span class="${u.ativo ? 'text-bg-success' : 'text-bg-secondary'}">${u.ativo ? 'Ativo' : 'Inativo'}</span></td><td class="text-end"><button class="btn btn-sm btn-outline-primary btn-editar" data-id="${u.id}">Editar</button> <button class="btn btn-sm btn-outline-danger btn-excluir" data-id="${u.id}">Excluir</button></td></tr>`));
        }).fail(() => mensagem('Não foi possível consultar os usuários.', 'danger'));
    }

    function prepararNovo() {
        $('#formUsuario')[0].reset();
        $('#Id').val(0);
        $('#Ativo').prop('checked', true);
        $('#DataCadastro').val(retornaDataHoraFormatada());
        $('#modalTitulo').text('Novo usuário');
        limparErros();
        modal.show();
    }
    $('#btnNovo').on('click', prepararNovo);

    $('#tabelaUsuarios').on('click', '.btn-editar', function () {
        $.getJSON('/Usuarios/Obter?id=' + $(this).data('id')).done(res => { const u = res.dados; $('#Id').val(u.id); $('#Nome').val(u.nome); $('#ValorHora').val(u.valorHora); $('#DataCadastro').val(retornaDataHoraFormatada(u.dataCadastro)); $('#Ativo').prop('checked', u.ativo); $('#modalTitulo').text('Editar usuário'); limparErros(); modal.show(); }).fail(xhr => mensagem(xhr.responseJSON?.mensagem || 'Não foi possível carregar o usuário.', 'danger'));
    });

    $('#formUsuario').on('submit', function (event) {
        event.preventDefault(); if (!validarUsuario()) return;
        const data = { id: Number($('#Id').val()), nome: $('#Nome').val().trim(), valorHora: Number($('#ValorHora').val()), dataCadastro: $('#DataCadastro').val(), ativo: $('#Ativo').is(':checked') };
        $.ajax({ url: '/Usuarios/Salvar', method: 'POST', contentType: 'application/json', data: JSON.stringify(data), headers: { 'RequestVerificationToken': token() } }).done(res => { modal.hide(); mensagem(res.mensagem); carregarUsuarios(); }).fail(xhr => mensagem(xhr.responseJSON?.mensagem || 'Não foi possível salvar o usuário.', 'danger'));
    });

    $('#tabelaUsuarios').on('click', '.btn-excluir', function () {
        const id = $(this).data('id'); if (!window.confirm('Deseja realmente excluir este usuário?')) return;
        $.ajax({ url: '/Usuarios/Excluir?id=' + id, method: 'DELETE', headers: { 'RequestVerificationToken': token() } }).done(res => { mensagem(res.mensagem); carregarUsuarios(); }).fail(xhr => mensagem(xhr.responseJSON?.mensagem || 'Não foi possível excluir o usuário.', 'danger'));
    });

    carregarUsuarios();
});
