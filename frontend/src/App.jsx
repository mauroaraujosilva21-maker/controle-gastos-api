import { useState, useEffect } from 'react';
import axios from 'axios';
import './App.css';

// Configuração da URL padrão do seu back-end C#
const API_URL = 'https://controle-gastos-api-66b5.onrender.com/api';

function App() {
  // Estados para armazenar os dados vindos do banco
  const [pessoas, setPessoas] = useState([]);
  const [transacoes, setTransacoes] = useState([]);
  const [relatorio, setRelatorio] = useState({ detalhesPorPessoa: [], totalGeralReceitas: 0, totalGeralDespesas: 0, saldoLiquidoGeral: 0 });

  // Estados para os formulários de cadastro
  const [nomePessoa, setNomePessoa] = useState('');
  const [idadePessoa, setIdadePessoa] = useState('');

  const [pessoaIdTransacao, setPessoaIdTransacao] = useState('');
  const [descricaoTransacao, setDescricaoTransacao] = useState('');
  const [valorTransacao, setValorTransacao] = useState('');
  const [tipoTransacao, setTipoTransacao] = useState('0'); // 0 = Receita, 1 = Despesa

  // Função para buscar todos os dados do back-end de uma vez só
  const carregarDados = async () => {
    try {
      const resPessoas = await axios.get(`${API_URL}/pessoas`);
      const resTransacoes = await axios.get(`${API_URL}/transacoes`);
      const resTotais = await axios.get(`${API_URL}/pessoas/totais`);

      setPessoas(resPessoas.data);
      setTransacoes(resTransacoes.data);
      setRelatorio(resTotais.data);
    } catch (error) {
      console.error("Erro ao conectar com o back-end:", error);
    }
  };

  // Carrega os dados assim que a página abre
  useEffect(() => {
    carregarDados();
  }, []);

  // Handler para cadastrar uma Pessoa
  const handleCadastrarPessoa = async (e) => {
    e.preventDefault();
    if (!nomePessoa || !idadePessoa) return alert("Preencha todos os campos!");

    try {
      await axios.post(`${API_URL}/pessoas`, {
        nome: nomePessoa,
        idade: parseInt(idadePessoa)
      });
      setNomePessoa('');
      setIdadePessoa('');
      carregarDados(); // Atualiza a tela
    } catch (err) {
      alert("Erro ao cadastrar pessoa.");
    }
  };

  // Handler para cadastrar uma Transação
  const handleCadastrarTransacao = async (e) => {
    e.preventDefault();
    if (!pessoaIdTransacao || !descricaoTransacao || !valorTransacao) {
      return alert("Preencha todos os campos da transação!");
    }

    try {
      await axios.post(`${API_URL}/transacoes`, {
        descricao: descricaoTransacao,
        valor: parseFloat(valorTransacao),
        tipo: Number(tipoTransacao), // Garante o formato numérico correto para o enum C#
        pessoaId: pessoaIdTransacao
      });
      setDescricaoTransacao('');
      setValorTransacao('');
      carregarDados(); // Atualiza a tela e os relatórios automaticamente
    } catch (err) {
      console.error("Erro completo da requisição:", err);
      if (err.response && err.response.data) {
        alert(`Erro: ${err.response.data}`);
      } else {
        alert("Erro ao cadastrar transação. Verifique o console.");
      }
    }
  };

  // Handler para deletar uma pessoa (Garante a deleção em cascata)
  const handleDeletarPessoa = async (id) => {
    if (confirm("Tem certeza que deseja deletar esta pessoa? Todas as transações dela serão apagadas!")) {
      try {
        await axios.delete(`${API_URL}/pessoas/${id}`);
        carregarDados();
      } catch (err) {
        alert("Erro ao deletar pessoa.");
      }
    }
  };

  return (
    <div className="container">
      <h1 className="header">🏡 Controle de Gastos Residenciais</h1>

      <div className="grid">
        {/* COLUNA 1: CADASTRO DE PESSOAS */}
        <div className="card">
          <h2>Membros da Família</h2>
          <form onSubmit={handleCadastrarPessoa}>
            <div className="form-group">
              <label>Nome:</label>
              <input type="text" value={nomePessoa} onChange={e => setNomePessoa(e.target.value)} placeholder="Ex: Mauro Silva" />
            </div>
            <div className="form-group">
              <label>Idade:</label>
              <input type="number" value={idadePessoa} onChange={e => setIdadePessoa(e.target.value)} placeholder="Ex: 27" />
            </div>
            <button type="submit" className="btn">Adicionar Pessoa</button>
          </form>

          <table>
            <thead>
              <tr>
                <th>Nome</th>
                <th>Idade</th>
                <th>Ações</th>
              </tr>
            </thead>
            <tbody>
              {pessoas.map(p => (
                <tr key={p.id}>
                  <td>{p.nome}</td>
                  <td>{p.idade} anos</td>
                  <td>
                    <button onClick={() => handleDeletarPessoa(p.id)} className="btn btn-danger">Excluir</button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {/* COLUNA 2: CADASTRO DE TRANSAÇÕES */}
        <div className="card">
          <h2>Nova Transação</h2>
          <form onSubmit={handleCadastrarTransacao}>
            <div className="form-group">
              <label>Quem realizou?</label>
              <select value={pessoaIdTransacao} onChange={e => setPessoaIdTransacao(e.target.value)}>
                <option value="">Selecione uma pessoa...</option>
                {pessoas.map(p => (
                  <option key={p.id} value={p.id}>{p.nome} ({p.idade} anos)</option>
                ))}
              </select>
            </div>
            <div className="form-group">
              <label>Descrição:</label>
              <input type="text" value={descricaoTransacao} onChange={e => setDescricaoTransacao(e.target.value)} placeholder="Ex: Salário ou Supermercado" />
            </div>
            <div className="form-group">
              <label>Valor (R$):</label>
              <input type="number" step="0.01" value={valorTransacao} onChange={e => setValorTransacao(e.target.value)} placeholder="0.00" />
            </div>
            <div className="form-group">
              <label>Tipo:</label>
              <select value={tipoTransacao} onChange={e => setTipoTransacao(e.target.value)}>
                <option value="0">Receita (+)</option>
                <option value="1">Despesa (-)</option>
              </select>
            </div>
            <button type="submit" className="btn">Lançar Transação</button>
          </form>
        </div>
      </div>

      {/* SEÇÃO INFERIOR: HISTÓRICO DE TRANSAÇÕES GERAIS */}
      <div className="card" style={{ marginBottom: '30px' }}>
        <h2>Histórico de Lançamentos</h2>
        <table>
          <thead>
            <tr>
              <th>Descrição</th>
              <th>Valor</th>
              <th>Tipo</th>
            </tr>
          </thead>
          <tbody>
            {transacoes.map(t => (
              <tr key={t.id}>
                <td>{t.descricao}</td>
                <td>R$ {t.valor.toFixed(2)}</td>
                <td>
                  <span className={`badge ${t.tipo === 0 ? 'badge-receita' : 'badge-despesa'}`}>
                    {t.tipo === 0 ? 'RECEITA' : 'DESPESA'}
                  </span>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* REQUISITO PRINCIPAL: RELATÓRIO FINANCEIRO INDIVIDUAL E GERAL */}
      <div className="total-card">
        <h2>📊 Painel de Saldos e Totais</h2>
        <table>
          <thead>
            <tr style={{ color: '#333' }}>
              <th>Morador</th>
              <th>Total Receitas</th>
              <th>Total Despesas</th>
              <th>Saldo Individual</th>
            </tr>
          </thead>
          <tbody style={{ color: '#fff' }}>
            {relatorio.detalhesPorPessoa && relatorio.detalhesPorPessoa.map(r => (
              <tr key={r.pessoaId} style={{ borderBottom: '1px solid rgba(255,255,255,0.2)' }}>
                <td>{r.nome}</td>
                <td style={{ color: '#2ecc71' }}>R$ {r.totalReceitas.toFixed(2)}</td>
                <td style={{ color: '#e74c3c' }}>R$ {r.totalDespesas.toFixed(2)}</td>
                <td style={{ fontWeight: 'bold', color: r.saldo >= 0 ? '#2ecc71' : '#e74c3c' }}>
                  R$ {r.saldo.toFixed(2)}
                </td>
              </tr>
            ))}
          </tbody>
        </table>

        <div className="total-grid">
          <div className="total-item">
            <h3>Total Receitas da Casa</h3>
            <p style={{ color: '#2ecc71', fontSize: '24px', fontWeight: 'bold' }}>
              R$ {relatorio.totalGeralReceitas.toFixed(2)}
            </p>
          </div>
          <div className="total-item">
            <h3>Total Despesas da Casa</h3>
            <p style={{ color: '#e74c3c', fontSize: '24px', fontWeight: 'bold' }}>
              R$ {relatorio.totalGeralDespesas.toFixed(2)}
            </p>
          </div>
          <div className="total-item" style={{ background: 'rgba(255,255,255,0.2)' }}>
            <h3>Saldo Líquido Geral</h3>
            <p style={{ color: relatorio.saldoLiquidoGeral >= 0 ? '#2ecc71' : '#e74c3c', fontSize: '24px', fontWeight: 'bold' }}>
              R$ {relatorio.saldoLiquidoGeral.toFixed(2)}
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}

export default App;